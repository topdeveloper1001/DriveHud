//-----------------------------------------------------------------------
// <copyright file="EquitySolver.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.EquityCalculator.Base.OmahaCalculations;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HoldemHand;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Model.Solvers
{
    internal class EquitySolver : IEquitySolver
    {
        public EquitySolver()
        {
            LoadPokerEvalLibrary();
        }

        public Dictionary<string, EquityData> CalculateEquity(HandHistory handHistory)
        {
            if (handHistory == null)
            {
                throw new ArgumentNullException(nameof(handHistory));
            }

            var result = new Dictionary<string, EquityData>();

            if (handHistory.HandActions == null)
            {
                return result;
            }

            var equityPlayers = GetEquityPlayers(handHistory);

            var pots = BuildPots(equityPlayers);

            // if there is no pots except main pot do nothing
            if (pots.Count == 0)
            {
                return result;
            }

            // intialize equity for all plaeyers
            equityPlayers.ForEach(p => p.Equity = new decimal[pots.Count]);

            var gameType = new GeneralGameTypeEnum().ParseGameType(handHistory.GameDescription.GameType);

            CalculatePotEquity(pots, handHistory, gameType);
            CalculatePotRakes(pots, handHistory);

            pots.ForEach(pot => CalculateEvDiff(pot, handHistory, gameType));

            result = equityPlayers.Select(x => new EquityData
            {
                Equity = x.Equity[0],
                EVDiff = x.EvDiff,
                PlayerName = x.Name
            }).ToDictionary(x => x.PlayerName);

            return result;
        }

        public decimal[] CalculateEquity(HoleCards[] playersCards, HandHistories.Objects.Cards.Card[] boardCards, HoleCards[] dead, GeneralGameTypeEnum gameType)
        {
            var cardsDelimeter = " ";

            var gameTypeArg = gameType == GeneralGameTypeEnum.Holdem ? "-h" :
                    (gameType == GeneralGameTypeEnum.OmahaHiLo ? "-o8" : "-o");

            var playerCardsArgs = string.Join(" - ", playersCards.Select(x => x.ToString(cardsDelimeter)).ToArray());
            var boardCardsArgs = string.Join(cardsDelimeter, boardCards.Select(x => x.ToString()));
            var deadCardsArgs = string.Join(cardsDelimeter, dead.Select(x => x.ToString(cardsDelimeter)).ToArray());

            // use monte-carlo simulation for omaha w/o board
            var prefix = gameType != GeneralGameTypeEnum.Holdem && string.IsNullOrEmpty(boardCardsArgs) ? "p -mc 10000" : "p";

            var argsLine = $"{prefix} {gameTypeArg} {playerCardsArgs}";

            if (!string.IsNullOrEmpty(boardCardsArgs))
            {
                argsLine += $" -- {boardCardsArgs}";
            }

            if (!string.IsNullOrEmpty(deadCardsArgs))
            {
                argsLine += $" / {deadCardsArgs}";
            }

            var argv = argsLine.Split(' ');

            CalculateEquity(argv.Length, argv, out CalculationResult result);

            var equity = new decimal[playersCards.Length];

            unsafe
            {
                for (var i = 0; i < playersCards.Length; i++)
                {
                    equity[i] = (decimal)result.ev[i];
                }
            }

            return equity;
        }

        /// <summary>
        /// Calculates rake of each pot accordingly on pot sizes
        /// </summary>
        /// <param name="pots">Pots to calculate rakes</param>
        private void CalculatePotRakes(List<EquityPot> pots, HandHistory handHistory)
        {
            var totalPot = pots.Sum(x => x.Pot);

            if (totalPot == 0)
            {
                return;
            }

            var rake = totalPot - handHistory.WinningActions.Sum(x => x.Amount);

            pots.ForEach(pot => pot.Rake = pot.Pot / totalPot * rake);
        }

        /// <summary>
        /// Calculates EV Diff for the players involved into the specified pots
        /// </summary>
        /// <param name="pot">Pots to calculate EV diff</param>
        /// <param name="handHistory">Hand history</param>
        /// <param name="gameType">Type of game</param>
        private void CalculateEvDiff(EquityPot pot, HandHistory handHistory, GeneralGameTypeEnum gameType)
        {
            if (pot.Players
                .Where(x => x.HoleCards != null)
                .Count() < 2)
            {
                return;
            }

            var pokerEvaluator = ServiceLocator.Current.GetInstance<IPokerEvaluator>(gameType.ToString());

            pokerEvaluator.SetCardsOnTable(handHistory.CommunityCards);

            pot.Players
                .Where(x => x.HoleCards != null)
                .ForEach(x => pokerEvaluator.SetPlayerCards(x.Seat, x.HoleCards));

            var winners = pokerEvaluator.GetWinners();

            if (winners.Lo == null || winners.Lo.IsNullOrEmpty())
            {
                CalculateEvDiffByPot(pot, handHistory, winners.Hi.ToList());
                return;
            }

            var splitPot = new EquityPot
            {
                Index = pot.Index,
                Players = pot.Players,
                PlayersPutInPot = pot.PlayersPutInPot,
                Pot = pot.Pot / 2,
                Rake = pot.Rake / 2,
                Street = pot.Street
            };

            CalculateEvDiffByPot(splitPot, handHistory, winners.Hi.ToList());
            CalculateEvDiffByPot(splitPot, handHistory, winners.Lo.ToList());
        }

        private void CalculateEvDiffByPot(EquityPot pot, HandHistory handHistory, List<int> winnersBySeat)
        {
            if (winnersBySeat.Count == 0)
            {
                LogProvider.Log.Error($"Could not find a winner of pot #{pot.Index} hand #{handHistory.HandId}");
                return;
            }

            foreach (var player in pot.Players)
            {
                if (!pot.PlayersPutInPot.ContainsKey(player))
                {
                    continue;
                }

                var netWonPerWinner = (pot.Pot - pot.Rake) / winnersBySeat.Count - pot.PlayersPutInPot[player];

                var ev = (pot.Pot - pot.Rake) * player.Equity[pot.Index] - pot.PlayersPutInPot[player];

                if (winnersBySeat.Contains(player.Seat))
                {
                    player.EvDiff += ev - netWonPerWinner;
                    continue;
                }

                player.EvDiff += ev + pot.PlayersPutInPot[player];
            }
        }

        private void CalculatePotEquity(List<EquityPot> pots, HandHistory handHistory, GeneralGameTypeEnum gameType)
        {
            var mainPotPlayers = pots[0].Players;

            for (var potIndex = 0; potIndex < pots.Count; potIndex++)
            {
                CalculateEquity(pots[potIndex].Players, mainPotPlayers, handHistory, gameType, potIndex);
            }
        }

        /// <summary>
        /// Calculates equity for the specified player if possible
        /// </summary>
        /// <param name="equityPlayers">Players to calculate equity</param>
        /// <param name="handHistory">Hand history</param>
        private void CalculateEquity(List<EquityPlayer> equityPlayers, List<EquityPlayer> mainPotPlayers, HandHistory handHistory, GeneralGameTypeEnum gameType, int potIndex)
        {
            var playersByName = handHistory.Players
                .GroupBy(x => x.PlayerName)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            // equity can be calculated only for player who didn't fold, whose last action was before river, whose hole cards are known
            var eligibleEquityPlayers = equityPlayers
                .Where(x => x.LastAction.Street != Street.River &&
                    !x.LastAction.IsFold &&
                    playersByName.ContainsKey(x.Name) && playersByName[x.Name].hasHoleCards)
                .ToList();

            // equity can't be calculated for single player
            if (eligibleEquityPlayers.Count < 2)
            {
                return;
            }

            // set known hole cards
            eligibleEquityPlayers.ForEach(p => p.HoleCards = playersByName[p.Name].HoleCards);

            try
            {
                var street = eligibleEquityPlayers.Select(x => x.LastAction.Street).Distinct().Max();

                var boardCards = CardHelper.IsStreetAvailable(handHistory.CommunityCards.ToString(), street)
                     ? handHistory.CommunityCards.GetBoardOnStreet(street)
                     : handHistory.CommunityCards;

                var deadCards = mainPotPlayers.Except(equityPlayers).Where(x => x.HoleCards != null).Select(x => x.HoleCards).ToArray();

                if (pokerEvalLibLoaded)
                {
                    CalculateEquity(eligibleEquityPlayers, street, boardCards, deadCards, gameType, potIndex);
                }
                else if (gameType == GeneralGameTypeEnum.Holdem)
                {
                    CalculateHoldemEquity(eligibleEquityPlayers, street, boardCards, deadCards, potIndex);
                }
                else
                {
                    CalculateOmahaEquity(eligibleEquityPlayers, street, boardCards, gameType == GeneralGameTypeEnum.OmahaHiLo, potIndex);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not calculate equity for hand #{handHistory.HandId}", e);
            }
        }
        
        private void CalculateEquity(List<EquityPlayer> players, Street street, BoardCards boardCards, HoleCards[] dead, GeneralGameTypeEnum gameType, int potIndex)
        {
            var equity = CalculateEquity(players.Select(x => x.HoleCards).ToArray(),
                boardCards.ToArray(),
                dead,
                gameType);

            for (var i = 0; i < players.Count; i++)
            {
                players[i].Equity[potIndex] = equity[i];
            }
        }

        /// <summary>
        /// Calculates Holdem equity for the specified players which last action was at the specified street
        /// </summary>
        /// <param name="players">Player to calculate equity</param>
        /// <param name="street">Street</param>
        /// <param name="boardCards">Board cards</param>
        private void CalculateHoldemEquity(List<EquityPlayer> players, Street street, BoardCards boardCards, HoleCards[] dead, int potIndex)
        {
            var wins = new long[players.Count];
            var losses = new long[players.Count];
            var ties = new long[players.Count];

            long totalhands = 0;

            var deadCards = string.Join(string.Empty, dead.Select(x => x.ToString()));

            Hand.HandWinOdds(players.Select(x => x.HoleCards.ToString()).ToArray(), boardCards.ToString(), deadCards, wins, ties, losses, ref totalhands);

            if (totalhands == 0)
            {
                return;
            }

            var playersOdds = new List<PlayerOdds>();

            for (var i = 0; i < wins.Length; i++)
            {
                var playerOdds = new PlayerOdds
                {
                    PlayerName = players[i].Name,
                    Wins = wins[i],
                    Ties = ties[i]
                };

                playersOdds.Add(playerOdds);
            }

            var allInEquity = CalculateEquity(playersOdds, totalhands);

            players.Where(p => allInEquity.ContainsKey(p.Name))
                .ForEach(p => p.Equity[potIndex] = allInEquity[p.Name]);
        }

        /// <summary>
        /// Calculates Omaha equity for the specified players which last action was at the specified street
        /// </summary>
        /// <param name="players">Player to calculate equity</param>
        /// <param name="street">Street</param>
        /// <param name="boardCards">Board cards</param>
        /// <param name="isHiLo">Determine whenever Omaha is Omaha HiLo</param>
        private void CalculateOmahaEquity(List<EquityPlayer> players, Street street, BoardCards boardCards, bool isHiLo, int potIndex)
        {
            var calc = new OmahaEquityCalculatorMain(true, isHiLo);

            var eq = calc.Equity(boardCards.Select(x => x.ToString()).ToArray(),
                players.Select(x => x.HoleCards.Select(c => c.ToString()).ToArray()).ToArray(),
                new string[] { }, 0);

            var allInEquity = new Dictionary<string, decimal>();

            for (var i = 0; i < eq.Length; i++)
            {
                var player = players.Count > i ? players[i] : null;

                if (player != null && !allInEquity.ContainsKey(player.Name))
                {
                    var equity = (decimal)eq[i].TotalEq / 100;
                    allInEquity.Add(player.Name, equity);
                }
            }

            players.Where(p => p.LastAction.Street == street && allInEquity.ContainsKey(p.Name))
                .ForEach(p => p.Equity[potIndex] = allInEquity[p.Name]);
        }

        /// <summary>
        /// Calculates equity by player odd
        /// </summary>
        /// <param name="playersOdds">Odds of player</param>
        /// <param name="total">Total</param>
        /// <returns>Dictionary with players odds</returns>
        private static Dictionary<string, decimal> CalculateEquity(IEnumerable<PlayerOdds> playersOdds, long total)
        {
            var sortedPlayerOdds = playersOdds.OrderBy(x => x.Ties).ToList();

            while (sortedPlayerOdds.Count > 0)
            {
                var playerOdds = sortedPlayerOdds.FirstOrDefault();
                playerOdds.Ties = playerOdds.Ties / sortedPlayerOdds.Count;
                playerOdds.Equity = (playerOdds.Ties + playerOdds.Wins) / total;

                sortedPlayerOdds.Remove(playerOdds);
                sortedPlayerOdds.ForEach(x => x.Ties -= playerOdds.Ties);
            }

            var result = playersOdds
                .Select(x => new { x.PlayerName, x.Equity })
                .ToDictionary(x => x.PlayerName, x => x.Equity);

            return result;
        }

        /// <summary>
        /// Builds pots except pot of players who didn't go all-in
        /// </summary>
        /// <param name="equityPlayers">Player to build pots</param>
        /// <returns>List of <see cref="EquityPot"/></returns>
        private List<EquityPot> BuildPots(List<EquityPlayer> equityPlayers)
        {
            var pots = new List<EquityPot>();

            var equityPlayersWentAllIn = equityPlayers
                .Where(x => x.WentAllIn)
                .OrderBy(x => x.PutInPot)
                .ToList();

            // nobody went all-in, then there will be only one pot
            if (equityPlayersWentAllIn.Count == 0)
            {
                return pots;
            }

            var potIndex = 0;

            EquityPot pot;

            while ((pot = BuildPot(equityPlayersWentAllIn.Where(x => x.PutInPot > 0),
                equityPlayers.Where(x => x.PutInPot > 0), potIndex++)) != null)
            {
                pots.Add(pot);
            }

            return pots;
        }

        /// <summary>
        /// Builds pot of the specified players
        /// </summary>
        /// <param name="equityPlayersWentAllIn">All-in players</param>
        /// <param name="equityPlayers">Players</param>
        /// <param name="potIndex">Index of pot</param>
        /// <returns>Pot of the specified players or null if there is no all-in players</returns>
        private EquityPot BuildPot(IEnumerable<EquityPlayer> equityPlayersWentAllIn, IEnumerable<EquityPlayer> equityPlayers, int potIndex)
        {
            if (equityPlayersWentAllIn.IsNullOrEmpty())
            {
                return null;
            }

            var eligibleEquityPlayers = equityPlayers.Where(x => x.PutInPot > 0).ToList();

            if (eligibleEquityPlayers.Count < 2)
            {
                return null;
            }

            var equityAllInPlayer = equityPlayersWentAllIn.First();

            var pot = new EquityPot
            {
                Index = potIndex++,
                Street = equityAllInPlayer.LastAction.Street
            };

            var allInPlayerPutInPot = equityAllInPlayer.PutInPot;

            foreach (var equityPlayer in eligibleEquityPlayers)
            {
                var putInThisPot = 0m;

                if (equityPlayer.PutInPot < allInPlayerPutInPot)
                {
                    pot.Pot += equityPlayer.PutInPot;
                    putInThisPot = equityPlayer.PutInPot;
                    equityPlayer.PutInPot = 0;
                }
                else
                {
                    pot.Pot += allInPlayerPutInPot;
                    putInThisPot = allInPlayerPutInPot;
                    equityPlayer.PutInPot -= allInPlayerPutInPot;
                }

                if (equityPlayer.PutInPot != 0 || !equityPlayer.LastAction.IsFold)
                {
                    pot.Players.Add(equityPlayer);
                }

                pot.PlayersPutInPot.Add(equityPlayer, putInThisPot);
            }

            return pot;
        }

        private List<EquityPlayer> GetEquityPlayers(HandHistory handHistory)
        {
            var playerSeats = handHistory.Players
                .GroupBy(x => x.PlayerName)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault().SeatNumber);

            var equityPlayers = new Dictionary<string, EquityPlayer>();

            foreach (var handAction in handHistory.HandActions.Where(x => x.IsGameAction))
            {
                if (!equityPlayers.TryGetValue(handAction.PlayerName, out EquityPlayer equityPlayer))
                {
                    equityPlayer = new EquityPlayer
                    {
                        Name = handAction.PlayerName,
                        Seat = playerSeats.ContainsKey(handAction.PlayerName) ?
                                playerSeats[handAction.PlayerName] : 0
                    };

                    equityPlayers.Add(equityPlayer.Name, equityPlayer);
                }

                if (handAction.IsAllIn || handAction.IsAllInAction)
                {
                    equityPlayer.WentAllIn = true;
                }

                equityPlayer.PutInPot += Math.Abs(handAction.Amount);
                equityPlayer.LastAction = handAction;
            }

            // apply uncalled bets
            if (equityPlayers.Count > 1)
            {
                var orderedEquityPlayers = equityPlayers.Values.OrderBy(x => x.PutInPot).ToArray();
                var uncalledBet = orderedEquityPlayers[orderedEquityPlayers.Length - 1].PutInPot - orderedEquityPlayers[orderedEquityPlayers.Length - 2].PutInPot;

                if (uncalledBet > 0)
                {
                    orderedEquityPlayers[orderedEquityPlayers.Length - 1].PutInPot -= uncalledBet;
                }
            }

            return equityPlayers.Values.ToList();
        }

        private static bool pokerEvalLibLoaded;

        private void LoadPokerEvalLibrary()
        {
            if (pokerEvalLibLoaded)
            {
                return;
            }

#if DEBUG
            var pokerEvalLib = Path.Combine(Environment.CurrentDirectory, "pokereval.dll");
#else
            var pokerEvalLib = Path.Combine(Environment.CurrentDirectory, "bin", "pokereval.dll");
#endif

            if (!File.Exists(pokerEvalLib))
            {
                LogProvider.Log.Error($"Library {pokerEvalLib} has not been found");
            }

            if (WinApi.LoadLibraryEx(pokerEvalLib, IntPtr.Zero, 0) == IntPtr.Zero)
            {
                LogProvider.Log.Error($"Library {pokerEvalLib} has not been loaded: {Marshal.GetLastWin32Error()}");
            }

            pokerEvalLibLoaded = true;
        }

        [DllImport("pokereval.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        private static extern int CalculateEquity(int argc,
           [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = UnmanagedType.LPStr)] string[] argv,
           out CalculationResult result);

        [StructLayout(LayoutKind.Sequential)]
        private unsafe struct CalculationResult
        {
            public fixed uint nwinhi[12];
            public fixed uint ntiehi[12];
            public fixed uint nlosehi[12];
            public fixed uint nwinlo[12];
            public fixed uint ntielo[12];
            public fixed uint nloselo[12];
            public fixed double ev[12];
        }

        /// <summary>
        /// Represents players for equity calculations
        /// </summary>
        private class EquityPlayer
        {
            public string Name { get; set; }

            public int Seat { get; set; }

            public decimal PutInPot { get; set; }

            public bool WentAllIn { get; set; }

            public HandAction LastAction { get; set; }

            public decimal[] Equity { get; set; }

            public decimal EvDiff { get; set; }

            public HoleCards HoleCards { get; set; }
        }

        /// <summary>
        /// Represents the pot of hand
        /// </summary>
        private class EquityPot
        {
            public int Index { get; set; }

            public decimal Pot { get; set; }

            public Street Street { get; set; }

            public List<EquityPlayer> Players { get; set; } = new List<EquityPlayer>();

            public Dictionary<EquityPlayer, decimal> PlayersPutInPot { get; set; } = new Dictionary<EquityPlayer, decimal>();

            public decimal Rake { get; set; }
        }

        /// <summary>
        /// Represents odds of player
        /// </summary>
        private class PlayerOdds
        {
            public string PlayerName { get; set; }

            public decimal Wins { get; set; }

            public decimal Ties { get; set; }

            public decimal Equity { get; set; }
        }

    }
}