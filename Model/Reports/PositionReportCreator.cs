using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Model.Data;
using DriveHUD.Entities;

namespace Model.Reports
{
    /// <summary>
    /// This report groups games by players position : Button, Small Blind, Early Position e.t.c
    /// </summary>
    public class PositionReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x => !x.IsTourney).GroupBy(x => x.PositionString))
            {
                Indicators stat = new Indicators();
                foreach (var playerstatistic in group)
                    stat.AddStatistic(playerstatistic);

                report.Add(stat);
            }

            return report;
        }
    }

    public class TournamentPositionReportCreator : IReportCreator
    {
        public ObservableCollection<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new ObservableCollection<Indicators>();

            if (statistics == null)
            {
                return report;
            }

            foreach (var group in statistics.Where(x => x.IsTourney).GroupBy(x => x.PositionString))
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