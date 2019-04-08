//-----------------------------------------------------------------------
// <copyright file="ZoneHandAdjuster.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Importers.Bovada;
using Model.Export;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class ZoneHandAdjuster : IZoneHandAdjuster
    {
        public string Adjust(string hand)
        {
            var handHistory = SerializationHelper.DeserializeObject<HandHistory>(hand);

            var gameType = handHistory.General.GameType.ContainsIgnoreCase("Omaha") ?
                (handHistory.General.GameType.ContainsIgnoreCase("Hi-Lo") ? GameType.OmahaHiLo : GameType.Omaha) :
                GameType.Holdem;

            handHistory.Games.ForEach(x => Adjust(x, gameType));

            var adjustedHand = SerializationHelper.SerializeObject(handHistory, true);
            return adjustedHand;
        }

        public void Adjust(Game game, GameType gameType)
        {
            if (game.Rounds == null || game.Rounds.Count < 2)
            {
                LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: Rounds aren't set or preflop doesn't exist.");
                return;
            }

            // sort actions
            SortHandActions(game);

            // find the player who put the most amount into the pot, he will be a winner, 
            // if there are 2+ players with same amount in the pot then just set fist as winner                        
            var folded = new HashSet<string>();
            var playerPutInPot = new Dictionary<string, decimal>();

            foreach (var round in game.Rounds)
            {
                foreach (var action in round.Actions.ToArray())
                {
                    if (string.IsNullOrEmpty(action.Player))
                    {
                        LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: Action no={action.No} has no player.");
                        return;
                    }

                    if (action.Type == ActionType.Fold)
                    {
                        if (!folded.Contains(action.Player))
                        {
                            folded.Add(action.Player);
                        }
                        else
                        {
                            round.Actions.Remove(action);
                        }

                        continue;
                    }

                    if (!playerPutInPot.ContainsKey(action.Player))
                    {
                        playerPutInPot.Add(action.Player, action.Sum);
                        continue;
                    }

                    playerPutInPot[action.Player] += action.Sum;
                }
            }

            if (IsFullGame(game, folded))
            {
                AdjustWinnings(game, gameType);
                return;
            }

            var winnerName = playerPutInPot
                .OrderByDescending(x => x.Value)
                .Where(x => !folded.Contains(x.Key))
                .Select(x => x.Key)
                .FirstOrDefault();

            if (winnerName == null)
            {
                LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: The name of the winner of #{game.GameCode} couldn't be determined.");
                return;
            }

            var winner = game.General.Players.FirstOrDefault(x => x.Name == winnerName);

            if (winner == null)
            {
                LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: Winner {winnerName} of #{game.GameCode} doesn't exist in the players list.");
                return;
            }

            winner.Win = game.General.Players.Sum(x => x.Bet);

            // if there was preflop only we need to add fake actions for everybody except winners
            if (game.Rounds.Count < 3)
            {
                var preflop = game.Rounds[1];

                var preflopLastAction = preflop.Actions.LastOrDefault();

                if (preflopLastAction == null)
                {
                    return;
                }

                var actionNo = preflopLastAction.No;

                var winnerPutInPot = playerPutInPot[winnerName];

                foreach (var player in game.General.Players)
                {
                    if (folded.Contains(player.Name) || winnerName == player.Name)
                    {
                        continue;
                    }

                    if (!playerPutInPot.TryGetValue(player.Name, out decimal currentPlayerPutInPot))
                    {
                        currentPlayerPutInPot = 0;
                    }

                    if (currentPlayerPutInPot == winnerPutInPot)
                    {
                        continue;
                    }

                    var foldAction = new Action
                    {
                        Player = player.Name,
                        Sum = 0,
                        Type = ActionType.Fold,
                        SeatNumber = player.Seat,
                        No = ++actionNo
                    };

                    preflop.Actions.Add(foldAction);
                }

                SortHandActions(game);
            }
        }

        /// <summary>
        /// Determines whenever the specified game is full
        /// </summary>
        /// <returns>True if game is full; otherwise - false</returns>
        private static bool IsFullGame(Game game, HashSet<string> foldedPlayers)
        {
            if (foldedPlayers.Count == 0)
            {
                return true;
            }

            // Hero didn't folded
            var preflop = game.Rounds[1];

            var playerWithKnownHoleCards = preflop.Cards
                .Where(x => x.Type == CardsType.Pocket && !x.Value.StartsWith("X", StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Player)
                .ToArray();

            return playerWithKnownHoleCards.All(x => !foldedPlayers.Contains(x));
        }

        /// <summary>
        /// Sorts hand actions in each street
        /// </summary>
        /// <param name="game">Game to order actions</param>
        private void SortHandActions(Game game)
        {
            var orderedPlayers = GetOrderedPlayers(game);

            if (orderedPlayers == null)
            {
                return;
            }

            var orderedPlayersDictionary = OrderedPlayersToDict(orderedPlayers);

            for (var roundNo = 1; roundNo < game.Rounds.Count; roundNo++)
            {
                // preflop (order must be calculated differently from other streets)
                if (roundNo == 1)
                {
                    var blindsCount = game.Rounds[0].Actions
                        .Count(x => x.Type == ActionType.SB || x.Type == ActionType.BB);

                    blindsCount = orderedPlayers.Length == 2 ? 1 : blindsCount > 1 ? 2 : blindsCount;

                    var preflopOrderedPlayers = orderedPlayers.Skip(blindsCount).Concat(orderedPlayers.Take(blindsCount)).ToArray();
                    var preflopOrderedPlayersDictionary = OrderedPlayersToDict(preflopOrderedPlayers);

                    game.Rounds[roundNo].Actions = OrderRoundAction(game.Rounds[roundNo].Actions, preflopOrderedPlayersDictionary);
                    continue;
                }

                game.Rounds[roundNo].Actions = OrderRoundAction(game.Rounds[roundNo].Actions, orderedPlayersDictionary);
            }

            var actionNo = 1;
            game.Rounds.SelectMany(x => x.Actions).ForEach(x => x.No = actionNo++);
        }

        private Player[] GetOrderedPlayers(Game game)
        {
            var dealer = game.General?.Players?.FirstOrDefault(x => x.Dealer);

            if (dealer == null)
            {
                LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: Dealer isn't set in hand #{game.GameCode}.");
                return null;
            }

            var orderedPlayers = game.General
                .Players
                .OrderBy(x => x.Seat)
                .SkipWhile(x => x.Seat <= dealer.Seat)
                .Concat(game.General.Players.TakeWhile(x => x.Seat <= dealer.Seat))
                .ToArray();


            return orderedPlayers;
        }

        private static Dictionary<string, int> OrderedPlayersToDict(IEnumerable<Player> orderedPlayers)
        {
            var orderedPlayersDictionary = orderedPlayers
               .Select((x, index) => new { Index = index, x.Name })
               .GroupBy(x => x.Name)
               .ToDictionary(x => x.Key, x => x.FirstOrDefault().Index);

            return orderedPlayersDictionary;
        }

        private static List<Action> OrderRoundAction(List<Action> actions, Dictionary<string, int> orderedPlayersDictionary)
        {
            var orderedHandActions = new List<Action>();

            var isInvalid = false;

            var groupedStreetActions = actions
                .GroupBy(x => x.Player)
                .Select(x => new { PlayerName = x.Key, PlayerActions = x.ToList() })
                .OrderBy(x =>
                {
                    if (!orderedPlayersDictionary.ContainsKey(x.PlayerName))
                    {
                        isInvalid = true;
                        return int.MaxValue;
                    }

                    return orderedPlayersDictionary[x.PlayerName];
                })
                .ToList();

            if (isInvalid)
            {
                return actions;
            }

            while (orderedHandActions.Count != actions.Count)
            {
                foreach (var groupedAction in groupedStreetActions.ToArray())
                {
                    var playerAction = groupedAction.PlayerActions.FirstOrDefault();

                    if (playerAction != null)
                    {
                        orderedHandActions.Add(playerAction);
                        groupedAction.PlayerActions.Remove(playerAction);
                    }
                    else
                    {
                        groupedStreetActions.Remove(groupedAction);
                    }
                }
            }

            return orderedHandActions;
        }

        private void AdjustWinnings(Game game, GameType gameType)
        {
            SetPocketCardSeats(game);

            // check winners
            var winners = game.Rounds.GetWinners(gameType);

            if (winners.Count == 0)
            {
                return;
            }

            var winnersWonAmount = game.General
                .Players
                .Where(x => winners.Contains(x.Seat))
                .Sum(x => x.Win);

            // win data is missing
            if (winnersWonAmount == 0)
            {
                var totalBet = game.General.Players.Sum(x => x.Bet);

                var winner = game.General.Players.FirstOrDefault(x => winners.Contains(x.Seat));

                if (winner != null)
                {
                    winner.Win = totalBet;
                }
            }
        }

        private void SetPocketCardSeats(Game game)
        {
            var preflop = game.Rounds[1];

            var playersSeatsGrouped = game.General.Players
                .GroupBy(x => x.Name);

            if (playersSeatsGrouped.Any(x => x.Count() > 1))
            {
                LogProvider.Log.Warn(this, $"Couldn't adjust fast fold hand #{game.GameCode}: Couldn't adjust winnings. Found 2+ players with the same name.");
                return;
            }

            var playersSeats = playersSeatsGrouped.ToDictionary(x => x.Key, x => x.FirstOrDefault().Seat);

            preflop.Cards
                .Where(x => x.Type == CardsType.Pocket)
                .ForEach(x => x.Seat = playersSeats.ContainsKey(x.Player) ? playersSeats[x.Player] : x.Seat);
        }
    }
}