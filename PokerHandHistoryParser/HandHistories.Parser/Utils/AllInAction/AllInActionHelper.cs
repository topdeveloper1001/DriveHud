//-----------------------------------------------------------------------
// <copyright file="AllInActionHelper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HandHistories.Parser.Utils
{
    internal static class AllInActionHelper
    {
        /// <summary>
        /// Some sites (like IPoker) don't specifically identify All-In calls/raises. In these cases we need to parse the actions 
        /// and reclassify certain actions as all-in
        /// </summary>
        internal static List<HandAction> IdentifyAllInActions(PlayerList playerList, List<HandAction> handActions)
        {
            Dictionary<string, decimal> playerStackRemaining = new Dictionary<string, decimal>();

            foreach (Player player in playerList)
            {
                playerStackRemaining.Add(player.PlayerName, player.StartingStack);
            }

            List<HandAction> identifiedActions = new List<HandAction>(handActions.Count);

            foreach (HandAction action in handActions)
            {
                //Negative amounts represent putting money into the pot - ignore actions which aren't negative
                if (action.Amount >= 0)
                {
                    identifiedActions.Add(action);
                    continue;
                }

                //Skip actions which have already been identified
                if (action is AllInAction)
                {
                    identifiedActions.Add(action);
                    continue;
                }

                //Update the remaining stack with our action's amount
                playerStackRemaining[action.PlayerName] += action.Amount;

                if (playerStackRemaining[action.PlayerName] == 0 && !action.IsBlinds)
                {
                    //This was a bet/raise/call for our remaining chips - we are all in
                    AllInAction allInAction = new AllInAction(action.PlayerName, action.Amount, action.Street, true);
                    identifiedActions.Add(allInAction);
                }
                else
                {
                    identifiedActions.Add(action);
                }
            }

            return identifiedActions;
        }

        /// <summary>
        /// Some sites (like IPoker) dont specify if allins are CALL/BET/RAISE so we we fix that after parsing actions.
        /// We do that by assigning HandAction.HandActionType with HandActionType.ALL_IN
        /// </summary>
        internal static List<HandAction> UpdateAllInActions(List<HandAction> handActions)
        {
            List<HandAction> identifiedActions = new List<HandAction>(handActions.Count);
            foreach (HandAction action in handActions)
            {
                if (action is AllInAction)
                {
                    HandActionType actionType = GetAllInActionType(action.PlayerName, action.Amount, action.Street, identifiedActions);

                    identifiedActions.Add(new HandAction(action.PlayerName, actionType, action.Amount, action.Street, AllInAction: true));
                }
                else
                {
                    identifiedActions.Add(action);
                }
            }

            return identifiedActions;
        }

        /// <summary>
        /// Gets the ActionType for an unadjusted action amount
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="amount"></param>
        /// <param name="street"></param>
        /// <param name="actions"></param>
        /// <returns></returns>
        internal static HandActionType GetAllInActionType(string playerName, decimal amount, Street street, List<HandAction> actions)
        {
            try
            {
                var streetActions = actions.Street(street);

                if (street != Street.Preflop && streetActions.FirstOrDefault(p => p.HandActionType == HandActionType.BET) == null)
                {
                    return HandActionType.BET;
                }

                Dictionary<string, decimal> PutInPot = new Dictionary<string, decimal>();

                foreach (var action in streetActions)
                {
                    if (!PutInPot.ContainsKey(action.PlayerName))
                    {
                        PutInPot.Add(action.PlayerName, action.Amount);
                    }
                    else
                    {
                        PutInPot[action.PlayerName] += action.Amount;
                    }
                }

                var contributed = Math.Abs(amount);
                if (PutInPot.ContainsKey(playerName))
                {
                    contributed += Math.Abs(PutInPot[playerName]);
                }
                if (contributed <= Math.Abs(PutInPot.Min(p => p.Value)))
                {
                    return HandActionType.CALL;
                }
                else
                {
                    return HandActionType.RAISE;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("error");
                return HandActionType.BET;
            }
        }
    }
}