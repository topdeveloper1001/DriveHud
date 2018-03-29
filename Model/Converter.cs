using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.EquityCalculator.Base.OmahaCalculations;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HoldemHand;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Extensions;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Importer
{
    public static class Converter
    {
        private static List<EnumPosition[]> PositionList = new List<EnumPosition[]>() {
            new EnumPosition[2] { EnumPosition.SB, EnumPosition.BB }, // 2-handed table
            new EnumPosition[3] { EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 3-handed table
            new EnumPosition[4] { EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB,}, // 4-handed table
            new EnumPosition[5] { EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB}, // 5-handed table
            new EnumPosition[6] { EnumPosition.UTG, EnumPosition.MP1, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB}, // 6-handed table
            new EnumPosition[7] { EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 7-handed table
            new EnumPosition[8] { EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 8-handed table
            new EnumPosition[9] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 9-handed table
            new EnumPosition[10] { EnumPosition.UTG, EnumPosition.UTG_1, EnumPosition.UTG_2, EnumPosition.MP1, EnumPosition.MP2, EnumPosition.MP3, EnumPosition.CO, EnumPosition.BTN, EnumPosition.SB, EnumPosition.BB }, // 10-handed table
        };

        public static string ToHoleCards(HoleCards holeCards)
        {
            if (holeCards == null)
                return string.Empty;

            var mask = Hand.ParseHand(holeCards.ToString());
            var cards = Hand.Cards(mask).ToList();
            cards.Sort(new CardComparer());

            return string.Join(string.Empty, cards);
        }

        public static HandHistoryRecord ToHandHistoryRecord(HandHistory hand, Playerstatistic stat)
        {
            var record = new HandHistoryRecord
            {
                Time = stat.Time,
                Cards = stat.Cards,
                Line = stat.Line,
                Board = stat.Board,
                NetWonInCents = stat.Totalamountwonincents,
                BBinCents = stat.Totalamountwonincents / stat.BigBlind,
                Pos = stat.PositionString,
                Equity = stat.Equity,
                EquityDiff = stat.EquityDiff,
                FacingPreflop = stat.FacingPreflop.ToString()
            };

            return record;
        }

        public static string ActionToString(HandAction action)
        {
            if (action is StreetAction)
                return ",";

            if (action.IsRaise())
                return "R";

            if (action.IsCall())
                return "C";

            if (action.IsBet())
                return "B";

            if (action.HandActionType == HandActionType.FOLD)
                return "F";

            if (action.HandActionType == HandActionType.CHECK)
                return "X";

            return string.Empty;
        }

        public static EnumPosition ToPosition(HandHistory hand, Playerstatistic stat)
        {
            return ToPosition(hand, stat?.PlayerName, stat);
        }

        public static EnumPosition ToPosition(HandHistory hand, string playerName, Playerstatistic stat = null)
        {
            if (stat != null && stat.IsDealer)
            {
                return EnumPosition.BTN;
            }
            else if (stat != null && stat.IsSmallBlind)
            {
                return EnumPosition.SB;
            }
            else if (stat != null && stat.IsBigBlind)
            {
                return EnumPosition.BB;
            }
            else if (stat != null && stat.IsStraddle)
            {
                return EnumPosition.STRDL;
            }

            var tableSize = hand.HandActions.Select(x => x.PlayerName).Distinct().Count();

            var table = PositionList.FirstOrDefault(x => x.Count() == tableSize);

            var handActions = hand.HandActions
                .Where(x => x.HandActionType != HandActionType.ANTE).ToList();

            var firstPlayerAction = handActions
                .Where(x => x.HandActionType != HandActionType.SMALL_BLIND && x.HandActionType != HandActionType.BIG_BLIND && x.HandActionType != HandActionType.POSTS)
                .FirstOrDefault(x => x.PlayerName == playerName);

            int firstPlayerActionIndex;

            if (firstPlayerAction != null)
            {
                if (firstPlayerAction.HandActionType == HandActionType.STRADDLE)
                {
                    return EnumPosition.STRDL;
                }

                var blindActionsCount = handActions
                    .Where(x => x.HandActionType == HandActionType.SMALL_BLIND ||
                        x.HandActionType == HandActionType.BIG_BLIND ||
                        x.HandActionType == HandActionType.POSTS)
                    .Count();

                firstPlayerActionIndex = handActions.IndexOf(firstPlayerAction) - blindActionsCount;

                if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
                {
                    return table[firstPlayerActionIndex];
                }
            }

            // get position using button place
            var playersBeforeDealerPosition = new List<Player>();
            var playersAfterDealerPosition = new List<Player>();

            for (var i = 0; i < hand.Players.Count; i++)
            {
                if (i <= hand.DealerButtonPosition - 1)
                {
                    playersBeforeDealerPosition.Add(hand.Players[i]);
                }
                else
                {
                    playersAfterDealerPosition.Add(hand.Players[i]);
                }
            }

            var playersOrderedByPosition = playersAfterDealerPosition.Concat(playersAfterDealerPosition);

            firstPlayerActionIndex = playersOrderedByPosition.FindIndex(x => x.PlayerName == playerName) - 2;

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

            // check ante
            var handAnteActions = hand.HandActions.Where(x => x.HandActionType == HandActionType.ANTE).ToList();

            firstPlayerAction = handAnteActions.FirstOrDefault(x => x.PlayerName == playerName);

            firstPlayerActionIndex = handAnteActions.IndexOf(firstPlayerAction) - handActions.Where(x => x.HandActionType == HandActionType.SMALL_BLIND).Take(1).Count() - handActions.Where(x => x.HandActionType == HandActionType.BIG_BLIND).Take(1).Count();

            if (table != null && firstPlayerActionIndex >= 0 && firstPlayerActionIndex < table.Count())
            {
                return table[firstPlayerActionIndex];
            }

            return EnumPosition.Undefined;
        }

        public static string ToPositionString(EnumPosition position)
        {
            switch (position)
            {
                case EnumPosition.BTN:
                    return "BTN";
                case EnumPosition.SB:
                    return "SB";
                case EnumPosition.BB:
                    return "BB";
                case EnumPosition.CO:
                    return "CO";
                case EnumPosition.MP3:
                case EnumPosition.MP2:
                case EnumPosition.MP1:
                case EnumPosition.MP:
                    return "MP";
                case EnumPosition.UTG:
                case EnumPosition.UTG_1:
                case EnumPosition.UTG_2:
                case EnumPosition.EP:
                    return "EP";
                case EnumPosition.Undefined:
                default:
                    return "Undefined";
            }

        }

        public static string ToAction(Playerstatistic stat)
        {
            if (stat.Vpiphands > 0)
                return "VPIP";

            if (stat.Pfrhands > 0)
                return "PFR";

            return string.Empty;
        }

        public static string ToAllin(HandHistory hand, Playerstatistic stat)
        {
            var wasAllinAction = hand.HandActions.FirstOrDefault(x => x.IsAllIn || x.IsAllInAction);

            if (wasAllinAction == null)
            {
                return string.Empty;
            }

            return wasAllinAction.Street.ToString();
        }



        public static List<decimal> CalculateEquity(HandHistory currentGame,
                                                    List<Player> playersHasHoleCards,
                                                    List<HoleCards> listDeadCards,
                                                    string deadCardsString,
                                                    string currentBoardString,
                                                    HandHistories.Objects.Cards.Card[] currentBoardArray)
        {
            var equity = new List<decimal>();

            int count = playersHasHoleCards.Count;

            if (count == 0)
            {
                return null;
            }

            var cards = playersHasHoleCards.Select((x, i) => new { x.HoleCards, x.PlayerName, Index = i }).ToArray();

            try
            {
                var gameType = new GeneralGameTypeEnum().ParseGameType(currentGame.GameDescription.GameType);

                if (gameType == GeneralGameTypeEnum.Holdem)
                {
                    var wins = new long[count];
                    var losses = new long[count];
                    var ties = new long[count];
                    long totalhands = 0;

                    Hand.HandWinOdds(cards.Select(x => x.HoleCards.ToString()).ToArray(), currentBoardString, deadCardsString, wins, ties, losses, ref totalhands);

                    if (totalhands != 0)
                    {
                        var playersOdds = new List<PlayerOdds>();

                        for (var i = 0; i < wins.Length; i++)
                        {
                            var playerCards = cards.FirstOrDefault(x => x.Index == i);

                            var playerOdds = new PlayerOdds
                            {
                                PlayerName = playerCards.PlayerName,
                                Wins = wins[i],
                                Ties = ties[i]
                            };

                            playersOdds.Add(playerOdds);
                        }

                        equity = CalculateEquity(playersOdds, totalhands).Values
                            .Select(x => Math.Round(x * 100, 2))
                            .ToList();
                    }
                }
                else if (gameType == GeneralGameTypeEnum.Omaha || gameType == GeneralGameTypeEnum.OmahaHiLo)
                {
                    OmahaEquityCalculatorMain calc = new OmahaEquityCalculatorMain(true, gameType == GeneralGameTypeEnum.OmahaHiLo);

                    MEquity[] eq = calc.Equity(currentBoardArray.Select(x => x.ToString()).ToArray(),
                                         cards.Select(x => x.HoleCards.Select(c => c.ToString()).ToArray()).ToArray(),
                                         new string[] { },
                                         0);

                    equity = eq.Select(x => Math.Round((decimal)x.TotalEq, 2)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(typeof(Converter), $"Error in CalculateEquityForListOfPlayers method of Converter class", ex);
            }

            return equity;
        }

        public static bool CanAllInEquity(HandHistory hand, Playerstatistic stat, out HandAction lastStreetAction)
        {
            var lastHeroStreetAction = hand.HandActions
                .LastOrDefault(x => x.PlayerName == stat.PlayerName
                                    && x.Street >= Street.Preflop
                                    && x.Street <= Street.River);

            lastStreetAction = lastHeroStreetAction;

            return lastHeroStreetAction != null && !lastHeroStreetAction.IsFold && lastHeroStreetAction.Street != Street.River &&
                hand.HandActions.Any(x => x.Street == lastHeroStreetAction.Street && (x.IsAllInAction || x.IsAllIn));
        }

        /// <summary>
        /// Calculates equities for all players with known hole cards on the street where hero of the specified <paramref name="stat"/> went all-in
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="stat"></param>
        /// <returns>Dictionary with players equities</returns>
        public static Dictionary<string, decimal> CalculateAllInEquity(HandHistory hand, Playerstatistic stat)
        {
            // last hero street action
            var lastHeroStreetAction = hand.HandActions
                .LastOrDefault(x => x.PlayerName == stat.PlayerName
                                    && x.Street >= Street.Preflop
                                    && x.Street <= Street.River);

            if (lastHeroStreetAction == null)
            {
                return null;
            }

            var currentPlayer = hand.Players.FirstOrDefault(x => x.PlayerName == stat.PlayerName);

            if (currentPlayer == null || !currentPlayer.hasHoleCards)
            {
                return null;
            }

            var activePlayers = hand.Players.Where(p => hand.HandActions.Any(x => p.PlayerName == x.PlayerName))
                .Where(p => !hand.HandActions.Any(h => h.PlayerName == p.PlayerName && h.IsFold)).Where(x => x.hasHoleCards).ToList();

            if (activePlayers.Count() < 2)
            {
                return null;
            }

            var cards = activePlayers.Select((x, i) => new { x.HoleCards, x.PlayerName, Index = i }).ToArray();

            int count = cards.Count();

            // something went wrong
            if (count == 0 || !cards.Any(x => x.PlayerName == stat.PlayerName))
            {
                LogProvider.Log.Debug($"Wasn't able to calculate AllIn equity for hand {hand.HandId}. Hand date: {hand.DateOfHandUtc}. HandHistory:{Environment.NewLine}{hand.FullHandHistoryText}");
                return null;
            }

            var allInEquity = new Dictionary<string, decimal>();

            try
            {
                var gameType = new GeneralGameTypeEnum().ParseGameType(hand.GameDescription.GameType);

                // holdem
                var boardCards = CardHelper.IsStreetAvailable(hand.CommunityCards.ToString(), lastHeroStreetAction.Street)
                      ? hand.CommunityCards.GetBoardOnStreet(lastHeroStreetAction.Street)
                      : hand.CommunityCards;

                if (gameType == GeneralGameTypeEnum.Holdem && cards.All(x => x.HoleCards.Count <= 2))
                {
                    var wins = new long[count];
                    var losses = new long[count];
                    var ties = new long[count];
                    long totalhands = 0;

                    Hand.HandWinOdds(cards.Select(x => x.HoleCards.ToString()).ToArray(), boardCards.ToString(), string.Empty, wins, ties, losses, ref totalhands);

                    if (totalhands != 0)
                    {
                        var playersOdds = new List<PlayerOdds>();

                        for (var i = 0; i < wins.Length; i++)
                        {
                            var playerCards = cards.FirstOrDefault(x => x.Index == i);

                            var playerOdds = new PlayerOdds
                            {
                                PlayerName = playerCards.PlayerName,
                                Wins = wins[i],
                                Ties = ties[i]
                            };

                            playersOdds.Add(playerOdds);
                        }

                        allInEquity = CalculateEquity(playersOdds, totalhands);
                    }
                }
                else if ((gameType == GeneralGameTypeEnum.Omaha || gameType == GeneralGameTypeEnum.OmahaHiLo) && cards.All(x => x.HoleCards.Count >= 2 && x.HoleCards.Count < 5))
                {
                    var calc = new OmahaEquityCalculatorMain(true, gameType == GeneralGameTypeEnum.OmahaHiLo);

                    var eq = calc.Equity(boardCards.Select(x => x.ToString()).ToArray(), cards.Select(x => x.HoleCards.Select(c => c.ToString()).ToArray()).ToArray(), new string[] { }, 0);

                    for (var i = 0; i < eq.Length; i++)
                    {
                        var playerCards = cards.FirstOrDefault(x => x.Index == i);

                        if (playerCards != null && !allInEquity.ContainsKey(playerCards.PlayerName))
                        {
                            var equity = (decimal)eq[i].TotalEq / 100;
                            allInEquity.Add(playerCards.PlayerName, equity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return allInEquity;
        }

        private static Dictionary<string, decimal> CalculateEquity(IEnumerable<PlayerOdds> playersOdds, long total)
        {
            var sortedPlayerOdds = playersOdds.OrderBy(x => x.Ties).ToList();

            while (sortedPlayerOdds.Count > 0)
            {
                var playerOdds = sortedPlayerOdds.FirstOrDefault();
                playerOdds.Ties = playerOdds.Ties / sortedPlayerOdds.Count;
                playerOdds.Equity = (decimal)(playerOdds.Ties + playerOdds.Wins) / total;

                sortedPlayerOdds.Remove(playerOdds);
                sortedPlayerOdds.ForEach(x => x.Ties -= playerOdds.Ties);
            }

            var result = playersOdds
                .Select(x => new { x.PlayerName, x.Equity })
                .ToDictionary(x => x.PlayerName, x => x.Equity);

            return result;
        }

        private class PlayerOdds
        {
            public string PlayerName { get; set; }

            public long Wins { get; set; }

            public long Ties { get; set; }

            public decimal Equity { get; set; }
        }

        public static string ActionToString(HandActionType type)
        {
            switch (type)
            {
                case HandActionType.FOLD:
                    return "Folds";
                case HandActionType.CALL:
                    return "Calls";
                case HandActionType.CHECK:
                    return "Checks";
                case HandActionType.RAISE:
                    return "Raises";
                case HandActionType.BET:
                    return "Bets";
                case HandActionType.SMALL_BLIND:
                    return "Small Blind";
                case HandActionType.BIG_BLIND:
                    return "Big Blind";
                case HandActionType.ALL_IN:
                    return "All in";
                case HandActionType.POSTS:
                    return "Posts";
                case HandActionType.STRADDLE:
                    return "Straddle";
                case HandActionType.WINS:
                case HandActionType.WINS_SIDE_POT:
                case HandActionType.WINS_THE_LOW:
                case HandActionType.WINS_TOURNAMENT:
                    return "Wins";
            }

            return null;
        }

        public static EnumFacingPreflop ToFacingPreflop(IEnumerable<HandAction> preflopHandActions, string playerName)
        {
            HandAction firstPlayerAction = preflopHandActions.FirstOrDefault(x => x.PlayerName == playerName && !x.IsBlinds);
            if (firstPlayerAction == null)
            {
                return EnumFacingPreflop.None;
            }

            int indexOfFirstPlayerAction = preflopHandActions.ToList().IndexOf(firstPlayerAction);

            IEnumerable<HandAction> actions = preflopHandActions
                                                    .Take(indexOfFirstPlayerAction)
                                                    .Where(x => !x.IsBlinds);

            if (actions.Any(x => x.IsRaise()))
            {
                switch (actions.Count(x => x.IsRaise()))
                {
                    case 1:
                        int indexOfRaiseAction = actions.ToList().IndexOf(actions.First(x => x.IsRaise()));
                        var postRaiseActions = actions.Skip(indexOfRaiseAction);
                        if (postRaiseActions.Any(x => x.IsCall()))
                        {
                            if (postRaiseActions.Count(x => x.IsCall()) > 1)
                            {
                                return EnumFacingPreflop.MultipleCallers;
                            }
                            else
                            {
                                return EnumFacingPreflop.RaiserAndCaller;
                            }
                        }
                        else
                        {
                            return EnumFacingPreflop.Raiser;
                        }
                    case 2:
                        return EnumFacingPreflop.ThreeBet;
                    case 3:
                        return EnumFacingPreflop.FourBet;
                    case 4:
                        return EnumFacingPreflop.FiveBet;
                }
            }

            int limpersNumber = actions.Where(x => x.HandActionType == HandActionType.CHECK
                                                || x.HandActionType == HandActionType.CALL).Count();
            if (limpersNumber > 0)
            {
                if (limpersNumber > 1)
                {
                    return EnumFacingPreflop.MultipleLimpers;
                }
                else
                {
                    return EnumFacingPreflop.Limper;
                }
            }

            return EnumFacingPreflop.Unopened;
        }

        public static STTTypes ToSitNGoType(string tournamentName)
        {
            if (tournamentName.IndexOf("Beginner", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.Beginner;
            }

            if (tournamentName.IndexOf("Double-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("1-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("One-Up", StringComparison.OrdinalIgnoreCase) >= 0 ||
                tournamentName.IndexOf("Double Or Nothing", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.DoubleUp;
            }

            if (tournamentName.IndexOf("Triple-Up", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return STTTypes.TripleUp;
            }

            return STTTypes.Normal;
        }

        /// <summary>
        /// Converts the provided utc date time value to local time using <see cref="GeneralSettingsModel.TimeZoneOffset"/> setting
        /// </summary>
        /// <param name="utcDateTime"></param>
        /// <returns></returns>
        public static DateTime ToLocalizedDateTime(DateTime utcDateTime)
        {
            var offset = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.TimeZoneOffset;
            return utcDateTime.AddHours(offset);
        }
    }
}
