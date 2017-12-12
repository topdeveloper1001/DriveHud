//-----------------------------------------------------------------------
// <copyright file="GGNUtils.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Globalization;

namespace DriveHUD.Importers.ExternalImporter
{
    internal class ExternalImporterUtils
    {
        public static string ReplaceMoneyWithChipsInTitle(string tableName)
        {
            var lastHyphenIndex = tableName.LastIndexOf('-');

            if (lastHyphenIndex <= 0)
            {
                return tableName;
            }

            var tournamentName = tableName.Substring(0, lastHyphenIndex - 1).Trim();
            var tableNumber = tableName.Substring(lastHyphenIndex).Trim();

            var dollarIndex = tournamentName.LastIndexOf('$');

            if (dollarIndex <= 0)
            {
                return tableName;
            }

            var buyinText = tournamentName.Substring(dollarIndex + 1);

            decimal buyin = 0;

            if (!decimal.TryParse(buyinText, NumberStyles.Any, new CultureInfo("en-US"), out buyin))
            {
                return tableName;
            }

            buyin *= 1000;

            tournamentName = tournamentName.Substring(0, dollarIndex).Trim();

            return $"{tournamentName} {buyin.ToString("N0", new CultureInfo("en-US"))} {tableNumber}";
        }
    }
}