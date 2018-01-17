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

using System.Collections.Generic;
using HandHistories.Objects.Actions;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Players;
using System.Linq;
using HandHistories.Objects.Cards;
using DriveHUD.Common.Log;

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

        protected override List<HandAction> OrderHandActions(List<HandAction> handActions, PlayerList players)
        {
            if (handActions == null || handActions.Count == 0)
            {
                return handActions;
            }

            var sbAction = handActions.FirstOrDefault(x => x.HandActionType == HandActionType.SMALL_BLIND);

            if (sbAction == null)
            {
                return base.OrderHandActions(handActions, players);
            }

            var sbPlayer = players.FirstOrDefault(x => x.PlayerName == sbAction.PlayerName);

            if (sbPlayer == null)
            {
                return base.OrderHandActions(handActions, players);
            }

            var orderedPlayers = new List<Player>(players);

            foreach (var player in orderedPlayers.ToArray())
            {
                if (sbPlayer == player)
                {
                    break;
                }

                orderedPlayers.Remove(player);
                orderedPlayers.Add(player);
            }

            var orderedPlayersDictionary = orderedPlayers
                 .Select((x, index) => new { Index = index, PlayerName = x.PlayerName })
                 .GroupBy(x => x.PlayerName)
                 .ToDictionary(x => x.Key, x => x.FirstOrDefault().Index);

            var orderedHandActions = new List<HandAction>();

            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Init));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Preflop));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Flop));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Turn));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.River));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Showdown));
            orderedHandActions.AddRange(OrderStreetHandActions(handActions, orderedPlayersDictionary, Street.Summary));

            if (orderedHandActions.Count != handActions.Count)
            {
                LogProvider.Log.Error(this, "Could not order hand actions. Arrays sizes doesn't match.");
                return base.OrderHandActions(handActions, players);
            }

            return orderedHandActions;
        }

        private List<HandAction> OrderStreetHandActions(List<HandAction> handActions, Dictionary<string, int> orderedPlayersDictionary, Street street)
        {
            var streetActions = handActions.Where(x => x.Street == street).ToList();

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
    }
}