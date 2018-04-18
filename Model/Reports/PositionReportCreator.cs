//-----------------------------------------------------------------------
// <copyright file="PositionReportCreator.cs" company="Ace Poker Solutions">
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
    /// This report groups games by players position : Button, Small Blind, Early Position e.t.c
    /// </summary>
    public class PositionReportCreator : CashGroupingReportCreator<ReportIndicators, string>
    {
        protected override ReportIndicators CreateIndicator(string groupKey)
        {
            var report = new ReportIndicators();
            return report;
        }

        protected override string GroupBy(Playerstatistic statistic)
        {
            return statistic.PositionString;
        }

        protected override string GroupBy(ReportIndicators indicator)
        {
            return indicator.Source.PositionString;
        }

        protected override IEnumerable<ReportIndicators> OrderResult(IEnumerable<ReportIndicators> reports)
        {
#if DEBUG
            return reports.OrderBy(x => x.Source.Position);
#else
            return reports.Where(x => x.Source.Position != EnumPosition.Undefined).OrderBy(x => x.Source.Position);
#endif
        }
    }   
}