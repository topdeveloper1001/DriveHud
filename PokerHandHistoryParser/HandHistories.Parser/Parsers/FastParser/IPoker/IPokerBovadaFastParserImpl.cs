//-----------------------------------------------------------------------
// <copyright file="HandHistoryParserFactoryImpl.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using System.Collections.Generic;
using System.Linq;
using HandHistories.Objects.Hand;
using System;

namespace HandHistories.Parser.Parsers.FastParser.IPoker
{
    internal class IPokerBovadaFastParserImpl : IPokerFastParserImpl
    {
        public override bool RequiresSeatTypeAdjustment
        {
            get
            {
                return false;
            }
        }

        public override bool RequiresTournamentSpeedAdjustment
        {
            get
            {
                return false;
            }
        }

        public override bool RequiresBetWinAdjustment
        {
            get
            {
                return false;
            }
        }

        protected override List<HandAction> OrderHandActions(List<HandAction> handActions, PlayerList players, HandHistory handHistory)
        {
            try
            {
                if (handActions == null || handActions.Count == 0)
                {
                    return handActions;
                }

                if (!IsFastFold(handHistory.TableName))
                {
                    return handActions;
                }

                var orderedPlayers = OrderPlayers(handActions, players);

                if (orderedPlayers == null)
                {
                    return handActions;
                }

                var orderedPlayersDictionary = orderedPlayers
                     .Select((x, index) => new { Index = index, PlayerName = x.PlayerName })
                     .GroupBy(x => x.PlayerName)
                     .ToDictionary(x => x.Key, x => x.FirstOrDefault().Index);

                var orderedHandActions = new List<HandAction>();

                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Init));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Preflop));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Flop));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Turn));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.River));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Showdown));
                orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, x => x.Street == Street.Summary));

                if (orderedHandActions.Count != handActions.Count)
                {
                    LogProvider.Log.Error(this, "Could not order hand actions. Arrays sizes doesn't match.");
                    return base.OrderHandActions(handActions, players, handHistory);
                }

                return orderedHandActions;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not ordered hands actions.", e);
            }

            return handActions;
        }

        private List<HandAction> OrderStreetHandActions(List<HandAction> handActions, Dictionary<string, int> orderedPlayersDictionary, Func<HandAction, bool> filter)
        {
            var streetActions = handActions.Where(filter).ToList();

            if (streetActions.Count == 0)
            {
                return streetActions;
            }

            var orderedHandActions = new List<HandAction>();

            var isInvalid = false;

            var groupedStreetActions = streetActions
                .GroupBy(x => x.PlayerName)
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
                return streetActions;
            }

            while (orderedHandActions.Count != streetActions.Count)
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

        private List<Player> OrderPlayers(List<HandAction> handActions, PlayerList players)
        {
            var blindAction = handActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);

            if (blindAction == null)
            {
                blindAction = handActions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND);

                if (blindAction == null)
                {
                    return null;
                }
            }

            var bPlayer = players.FirstOrDefault(x => x.PlayerName == blindAction.PlayerName);

            if (bPlayer == null)
            {
                return null;
            }

            var orderedPlayers = new List<Player>(players);

            foreach (var player in orderedPlayers.ToArray())
            {
                if (bPlayer == player)
                {
                    break;
                }

                orderedPlayers.Remove(player);
                orderedPlayers.Add(player);
            }

            return orderedPlayers;
        }

        protected override void ParseExtraHandInformation(string[] handLines, HandHistorySummary handHistorySummary)
        {
            base.ParseExtraHandInformation(handLines, handHistorySummary);

            var handHistory = handHistorySummary as HandHistory;

            if (handHistory == null || handHistory.HandActions.Count == 0)
            {
                return;
            }

            var isFastFold = IsFastFold(handHistory.TableName);

            if (!isFastFold)
            {
                return;
            }

            AdjustFastFoldHandHistory(handHistory);
        }

        protected virtual void AdjustFastFoldHandHistory(HandHistory handHistory)
        {
            // if hero didn't fold on preflop then do nothing
            if (handHistory.HandActions == null || handHistory.HandActions.Count == 0 ||
                handHistory.Players == null | handHistory.Players.Count == 0 ||
                !handHistory.HandActions.Any(x => x.Street == Street.Preflop && handHistory.HeroName == x.PlayerName && x.IsFold))
            {
                return;
            }

            try
            {
                var orderedPlayers = OrderPlayers(handHistory.HandActions, handHistory.Players);

                if (orderedPlayers == null)
                {
                    return;
                }

                var indexOfAction = 0;

                foreach (var player in orderedPlayers)
                {
                    var playerAction = handHistory.HandActions.FirstOrDefault(x => x.PlayerName == player.PlayerName && x.Street == Street.Preflop);

                    if (playerAction != null)
                    {
                        indexOfAction = handHistory.HandActions.IndexOf(playerAction);
                        continue;
                    }

                    playerAction = new HandAction(player.PlayerName, HandActionType.FOLD, 0, Street.Preflop, 0);

                    handHistory.HandActions.Insert(++indexOfAction, playerAction);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not adjust fast fold hands actions.", e);
            }
        }

        protected virtual bool IsFastFold(string tableName)
        {
            return tableName != null && tableName.Contains("Zone Poker");
        }
    }
}