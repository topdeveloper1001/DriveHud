//-----------------------------------------------------------------------
// <copyright file="PokerSiteReportCreator.cs" company="Ace Poker Solutions">
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

namespace Model.Reports
{
    public class PokerSiteReportCreator : CashGroupingReportCreator<ReportIndicators, int>
    {
        protected override ReportIndicators CreateIndicator(int groupKey)
        {
            var report = new ReportIndicators();
            return report;
        }

        protected override int GroupBy(Playerstatistic statistic)
        {
            return statistic.PokersiteId;
        }

        protected override int GroupBy(ReportIndicators indicator)
        {
            return indicator.Source.PokersiteId;
        }
    }    
}