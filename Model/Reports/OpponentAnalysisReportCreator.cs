//-----------------------------------------------------------------------
// <copyright file="OpponentAnalysisReportCreator.cs" company="Ace Poker Solutions">
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
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Reports
{
    public class OpponentAnalysisReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null || !statistics.Any())
            {
                return report;
            }

            foreach (var group in statistics.Where(x => !x.IsTourney).GroupBy(x => x.PlayerName).OrderByDescending(x => x.Sum(p => p.NetWon)).ToArray())
            {
                var stat = new ReportIndicators();

                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                }

                report.Add(stat);
            }

            return report;
        }
    }
}