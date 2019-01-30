//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticCalculator.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using Model.Importer;
using System.Collections.Generic;
using System.Linq;

namespace Model
{
    internal sealed class PKShortPlayerStatisticCalculator : PlayerStatisticCalculator
    {
        public override Playerstatistic CalculateStatistic(PlayerStatisticCreationInfo creationInfo)
        {
            var stat = base.CalculateStatistic(creationInfo);

            stat.Buttonstealdefended = stat.Bigblindstealdefended;
            stat.Buttonstealfaced = stat.Bigblindstealfaced;
            stat.Buttonstealfolded = stat.Bigblindstealfolded;
            stat.Buttonstealreraised = stat.Bigblindstealreraised;

            return stat;
        }

        protected override EnumPosition GetPlayerPosition(HandHistory hand, Playerstatistic stat)
        {
            return Converter.ToPKShortDeckPosition(hand, stat.PlayerName, stat);
        }

        protected override EnumPosition GetPlayerPosition(HandHistory hand, string playerName)
        {
            return Converter.ToPKShortDeckPosition(hand, playerName, null);
        }

        protected override Player GetCutOffPlayer(HandHistory hand)
        {
            var players = hand.Players.ToList();

            var button = players.FirstOrDefault(x => x.SeatNumber == hand.DealerButtonPosition);

            var orderedPlayers = players.OrderBy(x => x.SeatNumber).ToArray();

            var btnPlayerIndex = orderedPlayers.FindIndex(x => x.SeatNumber == hand.DealerButtonPosition);

            if (btnPlayerIndex < 0)
            {
                var co = hand.HandActions
                    .Select(h => h.PlayerName)
                    .Distinct()
                    .Where(x => x != button?.PlayerName)
                    .LastOrDefault();

                return hand.Players.FirstOrDefault(x => x.PlayerName == co);
            }

            var coPlayer = btnPlayerIndex == 0 ? orderedPlayers.Last() : orderedPlayers[btnPlayerIndex - 1];

            return coPlayer;
        }

        protected override void CalculateSteal(StealAttempt stealAttempt, HandHistory parsedHand, string player, bool isBlindPosition)
        {
            var bbAction = parsedHand.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND);

            if (bbAction == null)
            {
                return;
            }

            var bbActionIndex = parsedHand.HandActions
                .FindIndex(x => !x.IsBlinds && x.PlayerName == bbAction.PlayerName);

            // no action from BB/BTN player means he goes all, so steal isn't possible
            if (bbActionIndex < 0)
            {
                return;
            }

            var stealers = new List<string>();

            if (bbActionIndex > 2 && !parsedHand.HandActions[bbActionIndex - 2].IsBlinds)
            {
                stealers.Add(parsedHand.HandActions[bbActionIndex - 2].PlayerName);
            }

            if (bbActionIndex > 1 && !parsedHand.HandActions[bbActionIndex - 1].IsBlinds)
            {
                stealers.Add(parsedHand.HandActions[bbActionIndex - 1].PlayerName);
            }

            bool wasSteal = false;

            foreach (var action in parsedHand.PreFlop.Where(x => !x.IsBlinds))
            {
                if (wasSteal)
                {
                    if (action.PlayerName == player)
                    {
                        if (!isBlindPosition)
                        {
                            return;
                        }

                        stealAttempt.Faced = true;
                        stealAttempt.Defended = action.IsCall() || action.IsRaise();
                        stealAttempt.Raised = action.IsRaise();
                        stealAttempt.Folded = action.IsFold;

                        return;
                    }

                    if (!action.IsFold)
                    {
                        return;
                    }
                }
                else
                {
                    if (stealers.Contains(action.PlayerName))
                    {
                        if (action.PlayerName == player)
                        {
                            stealAttempt.Possible = true;
                            stealAttempt.Attempted = action.IsRaise();
                            return;
                        }

                        if (action.IsRaise())
                        {
                            wasSteal = true;
                            continue;
                        }
                    }

                    if (!action.IsFold)
                    {
                        return;
                    }
                }
            }
        }

        protected override Player GetInPositionPlayer(HandHistory hand, Street street, string player, bool foldAllowed = false)
        {
            var actions = hand.HandActions.Street(street)
                 .Where(x => !string.IsNullOrWhiteSpace(x.PlayerName)
                     && x.HandActionType != HandActionType.ANTE)
                 .ToArray();

            var bbAction = actions.FirstOrDefault(x => x.HandActionType == HandActionType.BIG_BLIND);

            if (hand.Players.Count == 2)
            {
                if (bbAction != null)
                {
                    var bbPlayer = hand.Players.FirstOrDefault(x => x.PlayerName == bbAction.PlayerName);

                    if (bbPlayer != null && bbPlayer.SeatNumber == hand.DealerButtonPosition)
                    {
                        return bbPlayer;
                    }
                }
            }

            var players = new List<string>();

            foreach (var action in actions.Where(x => x.HandActionType != HandActionType.POSTS && x.HandActionType != HandActionType.BIG_BLIND))
            {
                if (players.Contains(action.PlayerName))
                {
                    break;
                }

                players.Add(action.PlayerName);
            }

            var foldedPlayers = actions.Where(x => x.HandActionType == HandActionType.FOLD).Select(x => x.PlayerName).ToList();

            if (foldAllowed && player == foldedPlayers.LastOrDefault())
            {
                foldedPlayers.Remove(player);
            }

            foldedPlayers.ForEach(x => players.Remove(x));

            if (players.Count > 0)
            {
                var ipPlayer = players.LastOrDefault();
                return hand.Players.FirstOrDefault(x => x.PlayerName == ipPlayer);
            }

            return null;
        }
    }
}