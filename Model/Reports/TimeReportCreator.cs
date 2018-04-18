//-----------------------------------------------------------------------
// <copyright file="TimeReportCreator.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Settings;
using System.Collections.Generic;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by daily hours.
    /// </summary>
    public class TimeReportCreator : CashGroupingReportCreator<ReportIndicators, int>
    {
        protected override ReportIndicators CreateIndicator(int groupKey)
        {
            return new ReportIndicators();
        }

        protected override int GroupBy(Playerstatistic statistic)
        {
            return statistic.Time.Hour;
        }

        protected override int GroupBy(ReportIndicators indicator)
        {
            return indicator.Source.Time.Hour;
        }

        protected override IEnumerable<ReportIndicators> OrderResult(IEnumerable<ReportIndicators> reports)
        {
            var timeZoneOffset = ServiceLocator.Current.GetInstance<ISettingsService>()
                .GetSettings()
                .GeneralSettings
                .TimeZoneOffset;

            return reports.OrderBy(x => x.Source.Time.AddHours(timeZoneOffset).Hour);
        }
    }
}