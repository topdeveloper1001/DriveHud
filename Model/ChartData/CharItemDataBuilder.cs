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

namespace Model.ChartData
{
    public abstract class CharItemDataBuilder
    {
        public abstract Playerstatistic[] PrepareStatistic(IEnumerable<Playerstatistic> stats);

        public abstract object BuildGroupKey(Playerstatistic stat, int index);

        public abstract object GetValueFromGroupKey(object groupKey);
    }

    public abstract class DateItemDataBuilder : CharItemDataBuilder
    {
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
            var charItemDateKey = groupKey as CharItemDateKey;

            if (charItemDateKey == null)
            {
                return null;
            }

            return GetDateTimeFromGroupKey(charItemDateKey);
        }

        protected abstract DateTime GetDateTimeFromGroupKey(CharItemDateKey groupKey);

        protected abstract DateTime GetStartDate(DateTime maxDateTime);

        protected class CharItemDateKey
        {
            public int Year { get; set; }

            public int Month { get; set; }

            public int Day { get; set; }

            public int Hour { get; set; }

            public override bool Equals(object obj)
            {
                var dateKey = obj as CharItemDateKey;

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

            private bool Equals(CharItemDateKey dateKey)
            {
                return Year == dateKey.Year && Month == dateKey.Month && Day == dateKey.Day && Hour == dateKey.Hour;
            }
        }
    }

    public class YearItemDataBuilder : DateItemDataBuilder
    {
        public override object BuildGroupKey(Playerstatistic stat, int index)
        {
            var dateKey = new CharItemDateKey
            {
                Year = stat.Time.Year,
                Month = stat.Time.Month
            };

            return dateKey;
        }

        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            return maxDateTime.AddYears(-1);
        }

        protected override DateTime GetDateTimeFromGroupKey(CharItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, 1);
            return dateTime;
        }
    }

    public class MonthItemDataBuilder : DateItemDataBuilder
    {
        public override object BuildGroupKey(Playerstatistic stat, int index)
        {
            var dateKey = new CharItemDateKey
            {
                Year = stat.Time.Year,
                Month = stat.Time.Month,
                Day = stat.Time.Day
            };

            return dateKey;
        }

        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            return maxDateTime.AddMonths(-1);
        }

        protected override DateTime GetDateTimeFromGroupKey(CharItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, dateKey.Day);
            return dateTime;
        }
    }

    public class WeekItemDataBuilder : MonthItemDataBuilder
    {
        protected override DateTime GetStartDate(DateTime maxDateTime)
        {
            return maxDateTime.AddDays(-7);
        }
    }

    public class HandsItemDataBuilder : CharItemDataBuilder
    {
        public override object BuildGroupKey(Playerstatistic stat, int index)
        {
            return index;
        }

        public override object GetValueFromGroupKey(object groupKey)
        {
            var index = groupKey as int?;

            if (!index.HasValue)
            {
                return 0;
            }

            return index.Value + 1;
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