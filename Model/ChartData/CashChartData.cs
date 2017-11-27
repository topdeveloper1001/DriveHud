//-----------------------------------------------------------------------
// <copyright file="DayChartData.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.ChartData
{
    public abstract class BaseChartData : ICashChartData
    {
        public virtual IEnumerable<Indicators> Create(IList<Playerstatistic> statistics)
        {
            var report = new List<Indicators>();

            if (statistics == null || statistics.Count == 0)
            {
                return report;
            }

            var firstDate = GetFirstDate(statistics.Max(x => x.Time));

            var aggregatingStats = new LightIndicators();

            var groupedStatistics = statistics
               .Where(x => x.Time >= firstDate)
               .OrderBy(x => x.Time)
               .GroupBy(x => BuildGroupedDateKey(x));

            foreach (var group in groupedStatistics)
            {
                var aggregatingStatsSourceCopy = aggregatingStats.Source.Copy();

                var stat = new LightIndicators(new[] { aggregatingStatsSourceCopy });

                foreach (var playerstatistic in group)
                {
                    stat.AddStatistic(playerstatistic);
                    aggregatingStats.AddStatistic(playerstatistic);
                }

                stat.Source.Time = CreateDateTimeFromDateKey(group.Key);

                report.Add(stat);
            }

            return report;
        }

        protected abstract DateTime GetFirstDate(DateTime maxDateTime);

        protected abstract GroupedDateKey BuildGroupedDateKey(Playerstatistic statistic);

        protected abstract DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey);

        protected class GroupedDateKey
        {
            public int Year { get; set; }

            public int Month { get; set; }

            public int Day { get; set; }

            public int Hour { get; set; }

            public override bool Equals(object obj)
            {
                var dateKey = obj as GroupedDateKey;

                if (dateKey == null)
                {
                    return false;
                }

                return Equals(dateKey);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + Year;
                    hashcode = (hashcode * 31) + Month;
                    hashcode = (hashcode * 31) + Day;
                    hashcode = (hashcode * 31) + Hour;
                    return hashcode;
                }
            }

            private bool Equals(GroupedDateKey dateKey)
            {
                return Year == dateKey.Year && Month == dateKey.Month && Day == dateKey.Day && Hour == dateKey.Hour;
            }
        }
    }

    public class DayChartData : BaseChartData, ICashChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddDays(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Playerstatistic statistic)
        {
            var dateKey = new GroupedDateKey
            {
                Year = statistic.Time.Year,
                Month = statistic.Time.Month,
                Day = statistic.Time.Day,
                Hour = statistic.Time.Hour
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day, dateKey.Hour, 0, 0);
            return dateTime;
        }
    }

    public class WeekChartData : BaseChartData, ICashChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddDays(-7);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Playerstatistic statistic)
        {
            var dateKey = new GroupedDateKey
            {
                Year = statistic.Time.Year,
                Month = statistic.Time.Month,
                Day = statistic.Time.Day
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class MonthChartData : BaseChartData, ICashChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddMonths(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Playerstatistic statistic)
        {
            var dateKey = new GroupedDateKey
            {
                Year = statistic.Time.Year,
                Month = statistic.Time.Month,
                Day = statistic.Time.Day
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class YearChartData : BaseChartData, ICashChartData
    {
        protected override DateTime GetFirstDate(DateTime maxDateTime)
        {
            return maxDateTime.AddYears(-1);
        }

        protected override GroupedDateKey BuildGroupedDateKey(Playerstatistic statistic)
        {
            var dateKey = new GroupedDateKey
            {
                Year = statistic.Time.Year,
                Month = statistic.Time.Month
            };

            return dateKey;
        }

        protected override DateTime CreateDateTimeFromDateKey(GroupedDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, 1);
            return dateTime;
        }
    }
}