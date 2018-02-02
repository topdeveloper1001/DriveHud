//-----------------------------------------------------------------------
// <copyright file="SessionsReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    public class SessionsReportCreator : CashBaseReportCreator
    {
        public override ObservableCollection<ReportIndicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<ReportIndicators>();

            if (statistics == null)
            {
                return report;
            }

            var stat = new ReportIndicators();
            var gametypes = new List<string>();

            foreach (var playerstatistic in statistics.OrderByDescending(x => x.Time).ToArray())
            {
                if (stat.Statistics == null || stat.StatisticsCount == 0)
                {
                    gametypes.Add(playerstatistic.GameType);
                    stat.AddStatistic(playerstatistic);
                }
                else if (Utils.IsDateInDateRange(playerstatistic.Time, stat.SessionStart, stat.SessionEnd, TimeSpan.FromMinutes(30)))
                {
                    if (!gametypes.Contains(playerstatistic.GameType))
                    {
                        gametypes.Add(playerstatistic.GameType);
                    }

                    stat.AddStatistic(playerstatistic);
                }
                else
                {
                    stat.GameType = string.Join(",", gametypes);
                    report.Add(stat);
                    stat = new ReportIndicators();
                    gametypes.Clear();
                    stat.AddStatistic(playerstatistic);
                    gametypes.Add(playerstatistic.GameType);
                }
            }

            if (stat.Statistics != null && stat.StatisticsCount > 0)
            {
                stat.GameType = string.Join(",", gametypes);
                report.Add(stat);
            }

            return report;
        }
    }
}