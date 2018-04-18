//-----------------------------------------------------------------------
// <copyright file="HoleCardsReportCreator.cs" company="Ace Poker Solutions">
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
using Model.Data;
using Model.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by cards that was dealt to player
    /// </summary>
    public class HoleCardsReportCreator : CashGroupingReportCreator<HoleCardsReportRecord, string>
    {
        protected override HoleCardsReportRecord CreateIndicator(string groupKey)
        {
            var report = new HoleCardsReportRecord
            {
                Cards = new ComparableReportCards
                {
                    CardsString = groupKey
                }
            };

            return report;
        }

        protected override string GroupBy(Playerstatistic statistic)
        {
            return statistic.Cards.ToCards();
        }

        protected override string GroupBy(HoleCardsReportRecord indicator)
        {
            return indicator.Cards.CardsString;
        }

        protected override IEnumerable<HoleCardsReportRecord> OrderResult(IEnumerable<HoleCardsReportRecord> reports)
        {
            return reports.Where(x => !string.IsNullOrEmpty(x.Cards.CardsString)).OrderByDescending(x => x.Cards);
        }
    }      
}