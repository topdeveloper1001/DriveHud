//-----------------------------------------------------------------------
// <copyright file="StakesReportCreator.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by stakes. Ex - 0,25$/0,5$
    /// </summary>
    public class StakesReportCreator : CashGroupingReportCreator<ReportIndicators, string>
    {
        protected override ReportIndicators CreateIndicator(string groupKey)
        {
            return new ReportIndicators();
        }

        protected override string GroupBy(Playerstatistic statistic)
        {
            return statistic.GameType;
        }

        protected override string GroupBy(ReportIndicators indicator)
        {
            return indicator.Source.GameType;
        }

        protected override IEnumerable<ReportIndicators> OrderResult(IEnumerable<ReportIndicators> reports)
        {
            return reports.OrderBy(x => x.Source.GameType);
        }
    }
}