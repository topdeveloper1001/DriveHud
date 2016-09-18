using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Model.Data;
using Model.Importer;
using System.Collections.Generic;
using System.Linq;

namespace Model.ChartData
{
    public class DayChartData : ICashChartData
    {
        public IEnumerable<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new List<Indicators>();
            if (statistics == null || statistics.Count() == 0)
                return report;

            var newStat = statistics.Select(x => x.Copy());
            newStat.ForEach(x => Converter.ToLocalizedDateTime(x.Time));

            var firstDate = newStat.Max(x => x.Time).AddDays(-1);
            Indicators stat = new Indicators();
            foreach (var group in newStat.Where(x => x.Time >= firstDate).OrderBy(x => x.Time).GroupBy(x => new { x.Time.Year, x.Time.Month, x.Time.Day, x.Time.Hour }))
            {
                var clone = new Indicators();
                stat.Statistcs.ForEach(x => clone.AddStatistic(x));

                foreach (var playerstatistic in group)
                {
                    clone.AddStatistic(playerstatistic);
                    stat.AddStatistic(playerstatistic);
                }
                var now = System.DateTime.Now;
                clone.Source.Time = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day, group.Key.Hour, 0, 0);
                report.Add(clone);
            }

            return report;
        }
    }

    public class WeekChartData : ICashChartData
    {
        public IEnumerable<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new List<Indicators>();
            if (statistics == null || statistics.Count() == 0)
                return report;

            var newStat = statistics.Select(x => x.Copy());
            newStat.ForEach(x => Converter.ToLocalizedDateTime(x.Time));

            var firstDate = newStat.Max(x => x.Time).AddDays(-7);
            Indicators stat = new Indicators();
            foreach (var group in newStat.Where(x => x.Time >= firstDate).OrderBy(x => x.Time).GroupBy(x => new { x.Time.Year, x.Time.Month, x.Time.Day }))
            {
                var clone = new Indicators();
                stat.Statistcs.ForEach(x => clone.AddStatistic(x));

                foreach (var playerstatistic in group)
                {
                    clone.AddStatistic(playerstatistic);
                    stat.AddStatistic(playerstatistic);
                }
                clone.Source.Time = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day);
                report.Add(clone);
            }

            return report;
        }
    }

    public class MonthChartData : ICashChartData
    {
        public IEnumerable<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new List<Indicators>();
            if (statistics == null || statistics.Count() == 0)
                return report;

            var newStat = statistics.Select(x => x.Copy());
            newStat.ForEach(x => Converter.ToLocalizedDateTime(x.Time));

            var firstDate = newStat.Max(x => x.Time).AddMonths(-1);
            Indicators stat = new Indicators();
            foreach (var group in newStat.Where(x => x.Time >= firstDate).OrderBy(x => x.Time).GroupBy(x => new { x.Time.Year, x.Time.Month, x.Time.Day }))
            {
                var clone = new Indicators();
                stat.Statistcs.ForEach(x => clone.AddStatistic(x));

                foreach (var playerstatistic in group)
                {
                    clone.AddStatistic(playerstatistic);
                    stat.AddStatistic(playerstatistic);
                }
                clone.Source.Time = new System.DateTime(group.Key.Year, group.Key.Month, group.Key.Day);
                report.Add(clone);
            }

            return report;
        }
    }

    public class YearChartData : ICashChartData
    {
        public IEnumerable<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new List<Indicators>();
            if (statistics == null || statistics.Count() == 0)
                return report;

            var newStat = statistics.Select(x => x.Copy());
            newStat.ForEach(x => Converter.ToLocalizedDateTime(x.Time));

            var firstDate = newStat.Max(x => x.Time).AddYears(-1);
            Indicators stat = new Indicators();
            foreach (var group in newStat.Where(x => x.Time >= firstDate).OrderBy(x => x.Time).GroupBy(x => new { x.Time.Year, x.Time.Month }))
            {
                var clone = new Indicators();
                stat.Statistcs.ForEach(x => clone.AddStatistic(x));

                foreach (var playerstatistic in group)
                {
                    clone.AddStatistic(playerstatistic);
                    stat.AddStatistic(playerstatistic);
                }
                clone.Source.Time = new System.DateTime(group.Key.Year, group.Key.Month, 1);
                report.Add(clone);
            }

            return report;
        }
    }
}
