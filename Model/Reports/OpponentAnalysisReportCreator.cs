using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DriveHUD.Entities;
using Model.Data;

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

            foreach (var group in statistics.Where(x => !x.IsTourney).GroupBy(x => x.PlayerName).OrderByDescending(x=>x.Sum(p=>p.NetWon)))
            {
                Indicators stat = new Indicators();
                foreach (var playerstatistic in group)
                    stat.AddStatistic(playerstatistic);

                report.Add(stat);
            }

            return report;
        }
    }
}