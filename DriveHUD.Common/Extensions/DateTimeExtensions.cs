using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day);
        }

        public static DateTime FirstDayOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            DateTime result = dt.StartOfDay();
            int diff = result.StartOfDay().DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return result.AddDays(-1 * diff).Date;
        }

        public static DateTime FirstDayOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, 1);
        }

        public static DateTime FirstDayOfYear(this DateTime value)
        {
            return new DateTime(value.Year, 1, 1);
        }
    }
}
