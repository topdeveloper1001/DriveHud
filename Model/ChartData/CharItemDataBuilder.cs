//-----------------------------------------------------------------------
// <copyright file="CharItemDataBuilder.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System;
using System.Linq;
using Model.Importer;

namespace Model.ChartData
{
    public abstract class CharItemDataBuilder
    {
        public abstract void Prepare(int statsCount);

        public abstract Playerstatistic[] PrepareStatistic(IEnumerable<Playerstatistic> stats);

        public abstract object BuildGroupKey(Playerstatistic stat, int index);

        public abstract object GetValueFromGroupKey(object groupKey);
    }

    public abstract class DateItemDataBuilder : CharItemDataBuilder
    {
        public override void Prepare(int statsCount)
        {
        }

        public override Playerstatistic[] PrepareStatistic(IEnumerable<Playerstatistic> stats)
        {
            if (stats == null || !stats.Any())
            {
                return new Playerstatistic[0];
            }

            var startDate = GetStartDate(stats.Max(x => x.Time));

            var preparedStats = stats
                .Where(x => x.Time >= startDate)
                .OrderBy(x => x.Time)
                .ToArray();

            return preparedStats;
        }

        public override object GetValueFromGroupKey(object groupKey)
        {
            var charItemDateKey = groupKey as ChartItemDateKey;

            if (charItemDateKey == null)
            {
                return null;
            }

            return GetDateTimeFromGroupKey(charItemDateKey);
        }

        public override object BuildGroupKey(Playerstatistic stat, int index)
        {
            var time = Converter.ToLocalizedDateTime(stat.Time);
            return BuildGroupKey(time);
        }

        protected abstract DateTime GetDateTimeFromGroupKey(ChartItemDateKey groupKey);

        protected abstract DateTime GetStartDate(DateTime maxDateTime);

        protected abstract object BuildGroupKey(DateTime time);
    }

    public class YearItemDataBuilder : DateItemDataBuilder
    {
        protected override object BuildGroupKey(DateTime time)
        {
            var dateKey = new ChartItemDateKey
            {
                Year = time.Year,
                Month = time.Month
            };

            return dateKey;
        }

        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddYears(1) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

            return maxDateTime.AddYears(-1);
        }

        protected override DateTime GetDateTimeFromGroupKey(ChartItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, 1);
            return dateTime;
        }
    }

    public class MonthItemDataBuilder : DateItemDataBuilder
    {
        protected override object BuildGroupKey(DateTime time)
        {
            var dateKey = new ChartItemDateKey
            {
                Year = time.Year,
                Month = time.Month,
                Day = time.Day
            };

            return dateKey;
        }

        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddMonths(1) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

            return maxDateTime.AddMonths(-1);
        }

        protected override DateTime GetDateTimeFromGroupKey(ChartItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class WeekItemDataBuilder : MonthItemDataBuilder
    {
        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            if (DateTime.MinValue.AddDays(7) >= maxDateTime)
            {
                return DateTime.MinValue;
            }

            return maxDateTime.AddDays(-7);
        }
    }

    public class HandsItemDataBuilder : CharItemDataBuilder
    {
        private double approximator = 10000d;

        private int groupingStep = 1;

        private int statsCount = 0;

        public override void Prepare(int statsCount)
        {
            this.statsCount = statsCount;

            if (statsCount > approximator)
            {
                groupingStep = (int)Math.Round(statsCount / approximator);
            }
        }

        public override object BuildGroupKey(Playerstatistic stat, int index)
        {
            var groupKey = groupingStep != 0 ? index / groupingStep : index;

            return groupKey;
        }

        public override object GetValueFromGroupKey(object groupKey)
        {
            var index = groupKey as int?;

            if (!index.HasValue)
            {
                return 0;
            }

            var value = (index.Value + 1) * groupingStep;

            if (value > statsCount)
            {
                return statsCount;
            }

            return value;
        }

        public override Playerstatistic[] PrepareStatistic(IEnumerable<Playerstatistic> stats)
        {
            if (stats == null || !stats.Any())
            {
                return new Playerstatistic[0];
            }

            var preparedStats = stats
                .OrderBy(x => x.Time)
                .ToArray();

            return preparedStats;
        }
    }
}