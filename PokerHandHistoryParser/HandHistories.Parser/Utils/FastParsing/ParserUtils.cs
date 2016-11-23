//-----------------------------------------------------------------------
// <copyright file="ParserUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using HandHistories.Objects.GameDescription;
using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace HandHistories.Parser.Utils.FastParsing
{
    public static class ParserUtils
    {
        public static TournamentSpeed ParseTournamentSpeed(string input)
        {
            if (input.IndexOf("Super Turbo", StringComparison.InvariantCultureIgnoreCase) > 0 ||
                  input.IndexOf("Hyper-Turbo", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return TournamentSpeed.SuperTurbo;
            }

            if (input.IndexOf("Turbo", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                return TournamentSpeed.Turbo;
            }

            return TournamentSpeed.Regular;
        }

        private static readonly Regex MoneyRegex = new Regex(@"^(?<currency1>[^\d]+)?\s?(?<money>\d+(?:\.\d+)?)\s?(?<currency2>[^\.\d]+)?$", RegexOptions.Compiled);

        public static bool TryParseMoney(string moneyText, out decimal money, out Currency currency, NumberFormatInfo numberFormatInfo = null)
        {
            if (numberFormatInfo == null)
            {
                numberFormatInfo = new NumberFormatInfo
                {
                    NegativeSign = "-",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = ",",
                    CurrencySymbol = "$"
                };
            }

            money = 0;
            currency = Currency.USD;

            var match = MoneyRegex.Match(moneyText);

            if (!match.Success)
            {
                return false;
            }

            if (!decimal.TryParse(match.Groups["money"].Value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, numberFormatInfo, out money))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parse money text into money value (we expect that only 2 number are allowed after decimal separator)
        /// </summary>
        /// <param name="moneyText"></param>
        /// <returns></returns>
        public static decimal ParseMoney(string moneyText)
        {
            var money = 0m;

            if (!TryParseMoney(moneyText, out money))
            {
                throw new FormatException($"{moneyText} is not in the correct format");
            }

            return money;
        }

        public static bool TryParseMoney(string moneyText, out decimal money)
        {
            money = 0m;
            moneyText = moneyText.RemoveWhitespace().Replace(",", ".");

            var lastIndexOfDot = moneyText.LastIndexOf('.');

            if (lastIndexOfDot > 0)
            {
                if ((moneyText.Length > (lastIndexOfDot + 3)) && char.IsNumber(moneyText[lastIndexOfDot + 3]))
                {
                    moneyText = moneyText.Replace(".", string.Empty);
                }
                else
                {

                    var indexOfDot = moneyText.IndexOf(".");

                    while (indexOfDot < lastIndexOfDot)
                    {
                        moneyText = moneyText.Remove(indexOfDot, 1);
                        indexOfDot = moneyText.IndexOf(".", indexOfDot);
                        lastIndexOfDot--;
                    }
                }
            }

            var match = MoneyRegex.Match(moneyText);

            if (!match.Success)
            {
                return false;
            }

            money = decimal.Parse(match.Groups["money"].Value, NumberStyles.AllowCurrencySymbol | NumberStyles.Number, CultureInfo.InvariantCulture);

            return true;
        }
    }
}