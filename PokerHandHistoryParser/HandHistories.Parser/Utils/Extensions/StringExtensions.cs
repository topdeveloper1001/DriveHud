using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandHistories.Parser.Utils.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Removes any currency symbols before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmount(this string str)
        {
            str = str.Trim('£', '€', '$');
            return Decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Removes any currency symbols before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmount(this string str, NumberFormatInfo numberFormat)
        {
            str = str.Trim('£', '€', '$');
            return Decimal.Parse(str, numberFormat);
        }

        static readonly char[] ParseWSTrimChars = new char[]
        {
            '£', '€', '$', ' ', (char)160
        };

        /// <summary>
        /// Removes any currency symbols and whitespaces before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmountWS(this string str)
        {
            str = str.Trim(ParseWSTrimChars);
            return Decimal.Parse(str, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Removes any currency symbols and whitespaces before parsing
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static decimal ParseAmountWS(this string str, NumberFormatInfo numberFormat)
        {
            str = str.Trim(ParseWSTrimChars);
            return Decimal.Parse(str, numberFormat);
        }
    }
}
