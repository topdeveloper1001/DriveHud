//-----------------------------------------------------------------------
// <copyright file="WinningByYearSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.ChartData;
using Model.Importer;
using System;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class WinningByYearSeriesProvider : WinningByMonthSeriesProvider
    {
        protected override ChartItemDateKey BuildGroupKey(Playerstatistic statistic)
        {
            if (statistic == null)
            {
                return null;
            }

            var time = Converter.ToLocalizedDateTime(statistic.Time);

            var groupKey = new ChartItemDateKey
            {
                Year = time.Year,
            };

            return groupKey;
        }

        protected override DateTime GetDateTimeFromGroupKey(ChartItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, 1, 1);
            return dateTime;
        }
    }
}