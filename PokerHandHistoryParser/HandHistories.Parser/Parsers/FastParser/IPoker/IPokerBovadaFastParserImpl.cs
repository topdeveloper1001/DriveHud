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

using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Utils;
using System.Linq;

namespace HandHistories.Parser.Parsers.FastParser.IPoker
{
    internal class IPokerBovadaFastParserImpl : IPokerFastParserImpl
    {
        public override EnumPokerSites SiteName => EnumPokerSites.Ignition;

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
                return true;
            }
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

            handHistory.GameDescription.TableType = TableType.FromTableTypeDescriptions(TableTypeDescription.FastFold);

            AdjustFastFoldHandHistory(handHistory);
        }

        protected virtual void AdjustFastFoldHandHistory(HandHistory handHistory)
        {
            if (handHistory.HandActions == null || handHistory.HandActions.Count == 0 ||
                handHistory.Players == null || handHistory.Players.Count == 0)
            {
                return;
            }

            // if hero didn't fold on preflop then do nothing
            if (handHistory.HandActions.Any(x => x.Street == Street.Preflop && handHistory.HeroName == x.PlayerName && x.IsFold))
            {
                foreach (var player in handHistory.Players)
                {
                    var playerAction = new HandAction(player.PlayerName, HandActionType.FOLD, 0, Street.Preflop, 0);
                    handHistory.HandActions.Add(playerAction);
                }
            }

            HandHistoryUtils.SortHandActions(handHistory);
        }

        protected virtual bool IsFastFold(string tableName)
        {
            return tableName != null && (tableName.Contains("Zone Poker") || tableName.Contains("Fast Fold"));
        }
    }
}