using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model.Data;
using DriveHUD.Entities;
using System;

namespace Model.Reports
{
    public class SessionsReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            Indicators stat = new Indicators();
            List<string> gametypes = new List<string>();
            
            foreach (var playerstatistic in statistics.Where(x => !x.IsTourney).OrderByDescending(x => x.Time))
            {
                if (stat.Statistics == null || stat.Statistics.Count == 0)
                {
                    gametypes.Add(playerstatistic.GameType);
                    stat.AddStatistic(playerstatistic);
                }
                else if (stat.Statistics.Any(x => Math.Abs((x.Time - playerstatistic.Time).TotalMinutes) < TimeSpan.FromMinutes(30).TotalMinutes))
                {
                    if(!gametypes.Contains(playerstatistic.GameType))
                    {
                        gametypes.Add(playerstatistic.GameType);
                    }
                    stat.AddStatistic(playerstatistic);
                }
                else
                {
                    stat.GameType = string.Join(",", gametypes);
                    report.Add(stat);
                    stat = new Indicators();
                    gametypes.Clear();
                    stat.AddStatistic(playerstatistic);
                    gametypes.Add(playerstatistic.GameType);
                }
            }
            if (stat.Statistics != null && stat.Statistics.Count > 0)
            {
                stat.GameType = string.Join(",", gametypes);
                report.Add(stat);
            }

            return report;
        }
    }
}
