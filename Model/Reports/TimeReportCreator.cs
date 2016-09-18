using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model.Data;
using DriveHUD.Entities;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by daily hours.
    /// </summary>
    public class TimeReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x=> !x.IsTourney).GroupBy(x => x.Time.Hour))
            {
                Indicators stat = new Indicators();
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