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

using System;
using System.Collections.Generic;
using System.Globalization;

namespace DriveHUD.Importers.ExternalImporter
{
    internal class ExternalImporterUtils
    {
        public static string ReplaceMoneyWithChipsInTitle(string tableName)
        {
            var splittedTableName = SplitTournamentTableName(tableName);

            if (splittedTableName == null)
            {
                return tableName;
            }

            var tournamentName = splittedTableName.Item1;
            var tableNumber = splittedTableName.Item2;

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

        public static bool IsTournamentTableMatch(string title, string tableName)
        {
            var colonIndex = title.IndexOf(':');
            var lastHyphenIndex = title.LastIndexOf('-');

            if (colonIndex <= 0 || lastHyphenIndex <= 0)
            {
                return false;
            }

            var tournamentName = title.Substring(0, colonIndex - 1).Replace("undefined", string.Empty).Trim();
            var tableNumber = title.Substring(lastHyphenIndex).Trim();

            var titleToCompare = $"{tournamentName} {tableNumber}";

            var result = IsTitleMatchTableName(tableName, titleToCompare);

            if (!result)
            {
                var splittedTableName = SplitTournamentTableName(tableName);

                if (splittedTableName == null)
                {
                    return result;
                }

                if (tournamentDict.ContainsKey(splittedTableName.Item1))
                {
                    foreach (var chineseTournament in tournamentDict[splittedTableName.Item1])
                    {
                        var newTableName = $"{chineseTournament} {splittedTableName.Item2.Replace("Table", "牌桌")}";
                        result = IsTitleMatchTableName(newTableName, titleToCompare);

                        if (result)
                        {
                            return true;
                        }
                    }
                }
            }

            return result;
        }

        private static bool IsTitleMatchTableName(string tableName, string titleToCompare)
        {
            var result = titleToCompare.Equals(tableName, StringComparison.OrdinalIgnoreCase);

            if (!result)
            {
                tableName = ReplaceMoneyWithChipsInTitle(tableName);
                result = titleToCompare.Equals(tableName, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        private static Tuple<string, string> SplitTournamentTableName(string tableName)
        {
            var lastHyphenIndex = tableName.LastIndexOf('-');

            if (lastHyphenIndex <= 0)
            {
                return null;
            }

            var tournamentName = tableName.Substring(0, lastHyphenIndex - 1).Trim();
            var tableNumber = tableName.Substring(lastHyphenIndex).Trim();

            return Tuple.Create(tournamentName, tableNumber);
        }

        #region Eng-Chinese dictionary

        private static readonly Dictionary<string, string[]> tournamentDict = new Dictionary<string, string[]>
        {
            ["$1 Fortune Spin"] = new[] { "$1 幸运转盘", "$1 幸运转盘" },
            ["$2 Fortune Spin"] = new[] { "$2 幸运转盘", "$2 幸运转盘" },
            ["$10 Fortune Spin"] = new[] { "$10 幸运转盘", "$10 幸运转盘" },
            ["$5 Fortune Spin"] = new[] { "$5 幸运转盘", "$5 幸运转盘" },
            ["$8 Fortune Spin"] = new[] { "$8 幸运转盘", "$8 幸运转盘" },
            ["$5 Fortune Spin"] = new[] { "$20 幸运转盘", "$20 幸运转盘" },
            ["$15 Fortune Spin"] = new[] { "$15幸运转盘", "$15幸运转盘" },
            ["Daily Red $50"] = new[] { "每日红色 $50", "每日紅色 $50" },
            ["Daily Rebuys $15, $1.5K GTD"] = new[] { "每日续买 $15, $1.5K GTD", "每日續買 $15, $1.5K GTD" },
            ["Daily Red $80"] = new[] { "每日红色 $80", "每日紅色 $80" },
            ["Daily Red $150"] = new[] { "每日红色 $150", "每日紅色 $150" },
            ["Daily Rebuys $25, $2.5K GTD"] = new[] { "每日续买 $25, $2.5K GTD", "每日續買 $25, $2.5K GTD" },
            ["Daily Red $100"] = new[] { "每日红色 $100", "每日紅色 $100" },
            ["Turbo Satellite to Daily Red $50"] = new[] { "每日红色 $50 的重购卫星赛", "每日紅色 $50 的快速卫星赛" },
            ["Satellite to Daily Red $50"] = new[] { "每日红色 $50 的卫星赛", "每日紅色 $50 的卫星赛" },
            ["Rebuy Satellite to Daily Red $50"] = new[] { "每日红色 $50 的重购卫星赛", "每日紅色 $50 的重购卫星赛" },
            ["Turbo Satellite to Daily Rebuys $15, $1.5K GTD"] = new[] { "每日续买 $15, $1.5K GTD 的重购卫星赛", "每日續買 $15, $1.5K GTD 的快速卫星赛" },
            ["Satellite to Daily Rebuys $15, $1.5K GTD"] = new[] { "每日续买 $15, $1.5K GTD 的卫星赛", "每日續買 $15, $1.5K GTD 的卫星赛" },
            ["Turbo Satellite to Daily Red $80"] = new[] { "每日红色 $80 的重购卫星赛", "每日紅色 $80 的快速卫星赛" },
            ["Satellite to Daily Red $80"] = new[] { "每日红色 $80 的卫星赛", "每日紅色 $80 的卫星赛" },
            ["Rebuy Satellite to Daily Red $80"] = new[] { "每日红色 $80 的重购卫星赛", "每日紅色 $80 的重购卫星赛" },
            ["Turbo Satellite to Daily Red $150"] = new[] { "每日红色 $150 的重购卫星赛", "每日紅色 $150 的快速卫星赛" },
            ["Satellite to Daily Red $150"] = new[] { "每日红色 $150 的卫星赛", "每日紅色 $150 的卫星赛" },
            ["Rebuy Satellite to Daily Red $150"] = new[] { "每日红色 $150 的重购卫星赛", "每日紅色 $150 的重购卫星赛" },
            ["Turbo Satellite to Daily Rebuys $25, $2.5K GTD"] = new[] { "每日续买 $25, $2.5K GTD 的重购卫星赛", "每日續買 $25, $2.5K GTD 的快速卫星赛" },
            ["Satellite to Daily Rebuys $25, $2.5K GTD"] = new[] { "每日续买 $25, $2.5K GTD 的卫星赛", "每日續買 $25, $2.5K GTD 的卫星赛" },
            ["Rebuy Satellite to Daily Rebuys $25, $3K GTD"] = new[] { "每日续买 $25, $3K GTD 的重购卫星赛", "每日續買 $25, $3K GTD 的重购卫星赛" },
            ["Turbo Satellite to Daily Red $100"] = new[] { "每日红色 $100 的重购卫星赛", "每日紅色 $100 的快速卫星赛" },
            ["Satellite to Daily Red $100"] = new[] { "每日红色 $100 的卫星赛", "每日紅色 $100 的卫星赛" },
            ["Rebuy Satellite to Daily Red $100"] = new[] { "每日红色 $100 的重购卫星赛", "每日紅色 $100 的重购卫星赛" },
            ["Daily 6-Max $10"] = new[] { "每日蓝色六人桌 $10", "每日藍色六人桌 $10" },
            ["Daily Blue $30"] = new[] { "每日蓝色 $30", "每日藍色 $30" },
            ["Daily Rebuys $5, $500 GTD"] = new[] { "每日续买 $5, $500 GTD", "每日續買 $5, $500 GTD" },
            ["Daily Blue $50"] = new[] { "每日蓝色 $50", "每日藍色 $50" },
            ["Daily 6-Max $20"] = new[] { "每日六人桌 $20", "每日六人桌 $20" },
            ["Daily Blue $100"] = new[] { "每日蓝色 $100", "每日藍色 $100" },
            ["Daily Blue $10"] = new[] { "每日蓝色 $10", "每日藍色 $10" },
            ["Daily Blue $40"] = new[] { "每日蓝色 $40", "每日藍色 $40" },
            ["Daily Rebuys $10, $800 GTD"] = new[] { "每日续买 $10, $800 GTD", "每日續買 $10, $800 GTD" },
            ["Daily Blue $20"] = new[] { "每日蓝色 $20", "每日藍色 $20" },
            ["Daily Blue $5"] = new[] { "每日蓝色 $5", "每日藍色 $5" },
            ["Daily 6-Max $50"] = new[] { "每日六人桌 $50", "每日六人桌 $50" },
            ["Turbo Satellite to Daily 6-Max $10"] = new[] { "每日六人桌 $10 的重购卫星赛", "每日六人桌 $10 的快速卫星赛" },
            ["Satellite to Daily 6-Max $10"] = new[] { "每日六人桌 $10 的卫星赛", "每日六人桌 $10 的卫星赛" },
            ["Turbo Satellite to Daily Blue $30"] = new[] { "每日蓝色 $30 的重购卫星赛", "每日藍色 $30 的快速卫星赛" },
            ["Satellite to Daily Blue $30"] = new[] { "每日蓝色 $30 的卫星赛", "每日藍色 $30 的卫星赛" },
            ["Turbo Satellite to Daily Rebuys $5, $500 GTD"] = new[] { "每日续买 $5, $500 GTD 的重购卫星赛", "每日續買 $5, $500 GTD 的快速卫星赛" },
            ["Turbo Satellite to Daily Blue $50"] = new[] { "每日蓝色 $50 的重购卫星赛", "每日藍色 $50 的快速卫星赛" },
            ["Satellite to Daily Blue $50"] = new[] { "每日蓝色 $50 的卫星赛", "每日藍色 $50 的卫星赛" },
            ["Rebuy Satellite to Daily Blue $50"] = new[] { "每日蓝色 $50 的重购卫星赛", "每日藍色 $50 的重购卫星赛" },
            ["Turbo Satellite to Daily 6-Max $20"] = new[] { "每日六人桌 $20 的重购卫星赛", "每日六人桌 $20 的快速卫星赛" },
            ["Satellite to Daily 6-Max $20"] = new[] { "每日六人桌 $20 的卫星赛", "每日六人桌 $20 的卫星赛" },
            ["Turbo Satellite to Daily Blue $100"] = new[] { "每日蓝色 $100 的重购卫星赛", "每日藍色 $100 的快速卫星赛" },
            ["Satellite to Daily Blue $100"] = new[] { "每日蓝色 $100 的卫星赛", "每日藍色 $100 的卫星赛" },
            ["Rebuy Satellite to Daily Blue $100"] = new[] { "每日蓝色 $100 的重购卫星赛", "每日藍色 $100 的重购卫星赛" },
            ["Turbo Satellite to Daily Blue $10"] = new[] { "每日蓝色 $10 的重购卫星赛", "每日藍色 $10 的快速卫星赛" },
            ["Satellite to Daily Blue $10"] = new[] { "每日蓝色 $10 的卫星赛", "每日藍色 $10 的卫星赛" },
            ["Turbo Satellite to Daily Blue $40"] = new[] { "每日蓝色 $40 的重购卫星赛", "每日藍色 $40 的快速卫星赛" },
            ["Satellite to Daily Blue $40"] = new[] { "每日蓝色 $40 的卫星赛", "每日藍色 $40 的卫星赛" },
            ["Turbo Satellite to Daily Rebuys $10, $800 GTD"] = new[] { "每日续买 $10, $800 GTD 的重购卫星赛", "每日續買 $10, $800 GTD 的快速卫星赛" },
            ["Satellite to Daily Rebuys $10, $800 GTD"] = new[] { "每日续买 $10, $800 GTD 的卫星赛", "每日續買 $10, $800 GTD 的卫星赛" },
            ["Turbo Satellite to Daily Blue $20"] = new[] { "每日蓝色 $20 的重购卫星赛", "每日藍色 $20 的快速卫星赛" },
            ["Satellite to Daily Blue $20"] = new[] { "每日蓝色 $20 的卫星赛", "每日藍色 $20 的卫星赛" },
            ["Turbo Satellite to Daily Blue $5"] = new[] { "每日蓝色 $5 的重购卫星赛", "每日藍色 $5 的快速卫星赛" },
            ["Turbo Satellite to Daily 6-Max $50"] = new[] { "每日六人桌 $50 的重购卫星赛", "每日六人桌 $50 的快速卫星赛" },
            ["Satellite to Daily 6-Max $50"] = new[] { "每日六人桌 $50 的卫星赛", "每日六人桌 $50 的卫星赛" },
            ["Rebuy Satellite to Daily 6-Max $50"] = new[] { "每日六人桌 $50 的重购卫星赛", "每日六人桌 $50 的重购卫星赛" },
            ["Daily Green $100"] = new[] { "每日绿色 $100", "每日綠色 $100" },
            ["Daily Green $10"] = new[] { "每日绿色 $10", "每日綠色 $10" },
            ["Daily Green $40"] = new[] { "每日绿色 $40", "每日綠色 $40" },
            ["Daily Rebuys $8, $800 GTD"] = new[] { "每日续买 $8, $800 GTD", "每日續買 $8, $800 GTD" },
            ["Daily 6-Max $5"] = new[] { "每日绿色六人桌 $5", "每日綠色六人桌 $5" },
            ["Daily Green $20"] = new[] { "每日绿色 $20", "每日綠色 $20" },
            ["Daily 6-Max $40"] = new[] { "每日六人桌 $40", "每日六人桌 $40" },
            ["Daily Green $15"] = new[] { "每日绿色 $15", "每日綠色 $15" },
            ["Daily Green $25"] = new[] { "每日绿色 $25", "每日綠色 $25" },
            ["Daily 6-Max $20"] = new[] { "每日六人桌 $20", "每日六人桌 $20" },
            ["Turbo Satellite to Daily Green $100"] = new[] { "每日绿色 $100 的重购卫星赛", "每日綠色 $100 的快速卫星赛" },
            ["Satellite to Daily Green $100"] = new[] { "每日绿色 $100 的卫星赛", "每日綠色 $100 的卫星赛" },
            ["Rebuy Satellite to Daily Green $100"] = new[] { "每日绿色 $100 的重购卫星赛", "每日綠色 $100 的重购卫星赛" },
            ["Turbo Satellite to Daily Green $10"] = new[] { "每日绿色 $10 的重购卫星赛", "每日綠色 $10 的快速卫星赛" },
            ["Satellite to Daily Green $10"] = new[] { "每日绿色 $10 的卫星赛", "每日綠色 $10 的卫星赛" },
            ["Turbo Satellite to Daily Green $40"] = new[] { "每日绿色 $40 的重购卫星赛", "每日綠色 $40 的快速卫星赛" },
            ["Satellite to Daily Green $40"] = new[] { "每日绿色 $40 的卫星赛", "每日綠色 $40 的卫星赛" },
            ["Turbo Satellite to Daily Rebuys $8, $800 GTD"] = new[] { "每日续买 $8, $800 GTD 的重购卫星赛", "每日續買 $8, $800 GTD 的快速卫星赛" },
            ["Turbo Satellite to Daily 6-Max $5"] = new[] { "每日六人桌 $5 的重购卫星赛", "每日六人桌 $5 的快速卫星赛" },
            ["Satellite to Daily 6-Max $5"] = new[] { "每日六人桌 $5 的卫星赛", "每日六人桌 $5 的卫星赛" },
            ["Turbo Satellite to Daily Green $20"] = new[] { "每日绿色 $20 的重购卫星赛", "每日綠色 $20 的快速卫星赛" },
            ["Satellite to Daily Green $20"] = new[] { "每日绿色 $20 的卫星赛", "每日綠色 $20 的卫星赛" },
            ["Turbo Satellite to Daily 6-Max $40"] = new[] { "每日六人桌 $40 的重购卫星赛", "每日六人桌 $40 的快速卫星赛" },
            ["Satellite to Daily 6-Max $40"] = new[] { "每日六人桌 $40 的卫星赛", "每日六人桌 $40 的卫星赛" },
            ["Turbo Satellite to Daily Green $15"] = new[] { "每日绿色 $15 的重购卫星赛", "每日綠色 $15 的快速卫星赛" },
            ["Satellite to Daily Green $15"] = new[] { "每日绿色 $15 的卫星赛", "每日綠色 $15 的卫星赛" },
            ["Turbo Satellite to Daily Green $25"] = new[] { "每日绿色 $25 的重购卫星赛", "每日綠色 $25 的快速卫星赛" },
            ["Satellite to Daily Green $25"] = new[] { "每日绿色 $25 的卫星赛", "每日綠色 $25 的卫星赛" },
            ["Turbo Satellite to Daily 6-Max $20"] = new[] { "每日六人桌 $20 的重购卫星赛", "每日六人桌 $20 的快速卫星赛" },
            ["Satellite to Daily 6-Max $20"] = new[] { "每日六人桌 $20 的卫星赛", "每日六人桌 $20 的卫星赛" },
            ["Bounty Hunters $2.10"] = new[] { "悬赏金猎人 $2.10", "悬赏金猎人 $2.10" },
            ["Bounty Hunters $31.50"] = new[] { "悬赏金猎人 $31.50", "悬赏金猎人 $31.50" },
            ["Bounty Hunters $10.50"] = new[] { "悬赏金猎人 $10.50", "悬赏金猎人 $10.50" },
            ["Turbo Satellite to Bounty Hunters $10.50"] = new[] { "悬赏金猎人 $10.50 的重购卫星赛", "悬赏金猎人 $10.50 的快速卫星赛" },
            ["Satellite to Bounty Hunters $10.50"] = new[] { "悬赏金猎人 $10.50 的卫星赛", "悬赏金猎人 $10.50 的卫星赛" },
            ["Turbo Satellite to Bounty Hunters $31.50"] = new[] { "悬赏金猎人 $31.50 的重购卫星赛", "悬赏金猎人 $31.50 的快速卫星赛" },
            ["Satellite to Bounty Hunters $31.50"] = new[] { "悬赏金猎人 $31.50 的卫星赛", "悬赏金猎人 $31.50 的卫星赛" },
            ["Omaholic $10"] = new[] { "奥马哈 $10", "奥马哈 $10" },
            ["Turbo Satellite to Omaholic $10"] = new[] { "奥马哈 $10 的重购卫星赛", "奥马哈 $10 的快速卫星赛" },
            ["Satellite to Omaholic $10"] = new[] { "奥马哈 $10 的卫星赛", "奥马哈 $10 的卫星赛" },
            ["Sunday Red $100"] = new[] { "周日红色 $100", "周日紅色 $100" },
            ["Sunday Rebuys $30, $3K GTD"] = new[] { "周日重购 $30, $3K GTD", "周日重購 $30, $3K GTD" },
            ["Sunday Red $160"] = new[] { "周日红色 $160", "周日紅色 $160" },
            ["Sunday Red $300"] = new[] { "周日红色 $300", "周日紅色 $300" },
            ["Sunday Rebuys $50, $5K GTD"] = new[] { "周日重购 $50, $5K GTD", "周日重購 $50, $5K GTD" },
            ["Sunday Red $200"] = new[] { "周日红色 $200", "周日紅色 $200" },
            ["Turbo Satellite to Sunday Red $100"] = new[] { "周日红色 $100 的重购卫星赛", "周日紅色 $100 的快速卫星赛" },
            ["Satellite to Sunday Red $100"] = new[] { "周日红色 $100 的卫星赛", "周日紅色 $100 的卫星赛" },
            ["Rebuy Satellite to Sunday Red $100"] = new[] { "周日红色 $100 的重购卫星赛", "周日紅色 $100 的重购卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $30, $3K GTD"] = new[] { "周日重购 $30, $3K GTD 的重购卫星赛", "周日重購 $30, $3K GTD 的快速卫星赛" },
            ["Satellite to Sunday Rebuys $30, $3K GTD"] = new[] { "周日重购 $30, $3K GTD 的卫星赛", "周日重購 $30, $3K GTD 的卫星赛" },
            ["Turbo Satellite to Sunday Red $160"] = new[] { "周日红色 $160 的重购卫星赛", "周日紅色 $160 的快速卫星赛" },
            ["Satellite to Sunday Red $160"] = new[] { "周日红色 $160 的卫星赛", "周日紅色 $160 的卫星赛" },
            ["Rebuy Satellite to Sunday Red $160"] = new[] { "周日红色 $160 的重购卫星赛", "周日紅色 $160 的重购卫星赛" },
            ["Turbo Satellite to Sunday Red $300"] = new[] { "周日红色 $300 的重购卫星赛", "周日紅色 $300 的快速卫星赛" },
            ["Satellite to Sunday Red $300"] = new[] { "周日红色 $300 的卫星赛", "周日紅色 $300 的卫星赛" },
            ["Rebuy Satellite to Sunday Red $300"] = new[] { "周日红色 $300 的重购卫星赛", "周日紅色 $300 的重购卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $50, $5K GTD"] = new[] { "周日重购 $50, $5K GTD 的重购卫星赛", "周日重購 $50, $5K GTD 的快速卫星赛" },
            ["Satellite to Sunday Rebuys $50, $5K GTD"] = new[] { "周日重购 $50, $5K GTD 的卫星赛", "周日重購 $50, $5K GTD 的卫星赛" },
            ["Rebuy Satellite to Sunday Rebuys $50, $5K GTD"] = new[] { "周日重购 $50, $5K GTD 的重购卫星赛", "周日重購 $50, $5K GTD 的重购卫星赛" },
            ["Turbo Satellite to Sunday Red $200"] = new[] { "周日红色 $200 的重购卫星赛", "周日紅色 $200 的快速卫星赛" },
            ["Satellite to Sunday Red $200"] = new[] { "周日红色 $200 的卫星赛", "周日紅色 $200 的卫星赛" },
            ["Rebuy Satellite to Sunday Red $200"] = new[] { "周日红色 $200 的重购卫星赛", "周日紅色 $200 的重购卫星赛" },
            ["Sunday 6-Max $20"] = new[] { "周日蓝色六人桌 $20", "周日藍色六人桌 $20" },
            ["Sunday Blue $60"] = new[] { "周日蓝色 $60", "周日藍色 $60" },
            ["Sunday Rebuys $10, $1K GTD"] = new[] { "周日重购 $10, $1K GTD", "周日重購 $10, $1K GTD" },
            ["Sunday Blue $100"] = new[] { "周日蓝色 $100", "周日藍色 $100" },
            ["Sunday 6-Max $40"] = new[] { "周日六人桌 $40", "周日六人桌 $40" },
            ["Sunday Blue $200"] = new[] { "周日蓝色 $200", "周日藍色 $200" },
            ["Sunday Blue $20"] = new[] { "周日蓝色 $20", "周日藍色 $20" },
            ["Sunday Blue $80"] = new[] { "周日蓝色 $80", "周日藍色 $80" },
            ["Sunday Rebuys $20, $1.6K GTD"] = new[] { "周日重购 $20, $1.6K GTD", "周日重購 $20, $1.6K GTD" },
            ["Sunday Blue $40"] = new[] { "周日蓝色 $40", "周日藍色 $40" },
            ["Sunday Blue $10"] = new[] { "周日蓝色 $10", "周日藍色 $10" },
            ["Sunday 6-Max $100"] = new[] { "周日六人桌 $100", "周日六人桌 $100" },
            ["Turbo Satellite to Sunday 6-Max $20"] = new[] { "周日六人桌 $20 的重购卫星赛", "周日六人桌 $20 的快速卫星赛" },
            ["Satellite to Sunday 6-Max $20"] = new[] { "周日六人桌 $20 的卫星赛", "周日六人桌 $20 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $60"] = new[] { "周日蓝色 $60 的重购卫星赛", "周日藍色 $60 的快速卫星赛" },
            ["Satellite to Sunday Blue $60"] = new[] { "周日蓝色 $60 的卫星赛", "周日藍色 $60 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $10, $1K GTD"] = new[] { "周日重购 $10, $1K GTD 的重购卫星赛", "周日重購 $10, $1K GTD 的快速卫星赛" },
            ["Turbo Satellite to Sunday Blue $100"] = new[] { "周日蓝色 $100 的重购卫星赛", "周日藍色 $100 的快速卫星赛" },
            ["Satellite to Sunday Blue $100"] = new[] { "周日蓝色 $100 的卫星赛", "周日藍色 $100 的卫星赛" },
            ["Rebuy Satellite to Sunday Blue $100"] = new[] { "周日蓝色 $100 的重购卫星赛", "周日藍色 $100 的重购卫星赛" },
            ["Turbo Satellite to Sunday 6-Max $40"] = new[] { "周日六人桌 $40 的重购卫星赛", "周日六人桌 $40 的快速卫星赛" },
            ["Satellite to Sunday 6-Max $40"] = new[] { "周日六人桌 $40 的卫星赛", "周日六人桌 $40 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $200"] = new[] { "周日蓝色 $200 的重购卫星赛", "周日藍色 $200 的快速卫星赛" },
            ["Satellite to Sunday Blue $200"] = new[] { "周日蓝色 $200 的卫星赛", "周日藍色 $200 的卫星赛" },
            ["Rebuy Satellite to Sunday Blue $200"] = new[] { "周日蓝色 $200 的重购卫星赛", "周日藍色 $200 的重购卫星赛" },
            ["Turbo Satellite to Sunday Blue $20"] = new[] { "周日蓝色 $20 的重购卫星赛", "周日藍色 $20 的快速卫星赛" },
            ["Satellite to Sunday Blue $20"] = new[] { "周日蓝色 $20 的卫星赛", "周日藍色 $20 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $80"] = new[] { "周日蓝色 $80 的重购卫星赛", "周日藍色 $80 的快速卫星赛" },
            ["Satellite to Sunday Blue $80"] = new[] { "周日蓝色 $80 的卫星赛", "周日藍色 $80 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $20, $1.6K GTD"] = new[] { "周日重购 $20, $1.6K GTD 的重购卫星赛", "周日重購 $20, $1.6K GTD 的快速卫星赛" },
            ["Satellite to Sunday Rebuys $20, $1.6K GTD"] = new[] { "周日重购 $20, $1.6K GTD 的卫星赛", "周日重購 $20, $1.6K GTD 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $40"] = new[] { "周日蓝色 $40 的重购卫星赛", "周日藍色 $40 的快速卫星赛" },
            ["Satellite to Sunday Blue $40"] = new[] { "周日蓝色 $40 的卫星赛", "周日藍色 $40 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $10"] = new[] { "周日蓝色 $10 的重购卫星赛", "周日藍色 $10 的快速卫星赛" },
            ["Turbo Satellite to Sunday 6-Max $100"] = new[] { "周日六人桌 $100 的重购卫星赛", "周日六人桌 $100 的快速卫星赛" },
            ["Satellite to Sunday 6-Max $100"] = new[] { "周日六人桌 $100 的卫星赛", "周日六人桌 $100 的卫星赛" },
            ["Rebuy Satellite to Sunday 6-Max $100"] = new[] { "周日六人桌 $100 的重购卫星赛", "周日六人桌 $100 的重购卫星赛" },
            ["Sunday Green $200"] = new[] { "周日绿色 $200", "周日綠色 $200" },
            ["Sunday Green $20"] = new[] { "周日绿色 $20", "周日綠色 $20" },
            ["Sunday Green $80"] = new[] { "周日绿色 $80", "周日綠色 $80" },
            ["Sunday Rebuys $16, $1.6K GTD"] = new[] { "周日重购 $16, $1.6K GTD", "周日重購 $16, $1.6K GTD" },
            ["Sunday 6-Max $10"] = new[] { "周日 6-Max $10", "周日 6-Max $10" },
            ["Sunday Green $40"] = new[] { "周日绿色 $40", "周日綠色 $40" },
            ["Sunday 6-Max $80"] = new[] { "周日 6-Max $80", "周日 6-Max $80" },
            ["Sunday Green $30"] = new[] { "周日绿色 $30", "周日綠色 $30" },
            ["Sunday Green $50"] = new[] { "周日绿色 $50", "周日綠色 $50" },
            ["Sunday 6-Max $40"] = new[] { "周日绿色 6-Max $40", "周日綠色 6-Max $40" },
            ["Turbo Satellite to Sunday Green $200"] = new[] { "周日绿色 $200 的重购卫星赛", "周日綠色 $200 的快速卫星赛" },
            ["Satellite to Sunday Green $200"] = new[] { "周日绿色 $200 的卫星赛", "周日綠色 $200 的卫星赛" },
            ["Rebuy Satellite to Sunday Green $200"] = new[] { "周日绿色 $200 的重购卫星赛", "周日綠色 $200 的重购卫星赛" },
            ["Turbo Satellite to Sunday Green $20"] = new[] { "周日绿色 $20 的重购卫星赛", "周日綠色 $20 的快速卫星赛" },
            ["Satellite to Sunday Green $20"] = new[] { "周日绿色 $20 的卫星赛", "周日綠色 $20 的卫星赛" },
            ["Turbo Satellite to Sunday Green $80"] = new[] { "周日绿色 $80 的重购卫星赛", "周日綠色 $80 的快速卫星赛" },
            ["Satellite to Sunday Green $80"] = new[] { "周日绿色 $80 的卫星赛", "周日綠色 $80 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $16, $1.6K GTD"] = new[] { "周日重购 $16, $1.6K GTD 的重购卫星赛", "周日重購 $16, $1.6K GTD 的快速卫星赛" },
            ["Turbo Satellite to Sunday 6-Max $10"] = new[] { "周日绿色 6-Max $10 的重购卫星赛", "周日綠色 6-Max $10 的快速卫星赛" },
            ["Satellite to Sunday 6-Max $10"] = new[] { "周日绿色 6-Max $10 的卫星赛", "周日綠色 6-Max $10 的卫星赛" },
            ["Turbo Satellite to Sunday Green $40"] = new[] { "周日绿色 $40 的重购卫星赛", "周日綠色 $40 的快速卫星赛" },
            ["Satellite to Sunday Green $40"] = new[] { "周日绿色 $40 的卫星赛", "周日綠色 $40 的卫星赛" },
            ["Turbo Satellite to Sunday 6-Max $80"] = new[] { "周日 $6-Max $80 的重购卫星赛", "周日 $6-Max $80 的快速卫星赛" },
            ["Satellite to Sunday 6-Max $80"] = new[] { "周日 $6-Max $80 的卫星赛", "周日 $6-Max $80 的卫星赛" },
            ["Turbo Satellite to Sunday Green $30"] = new[] { "周日绿色 $30 的重购卫星赛", "周日綠色 $30 的快速卫星赛" },
            ["Satellite to Sunday Green $30"] = new[] { "周日绿色 $30 的卫星赛", "周日綠色 $30 的卫星赛" },
            ["Turbo Satellite to Sunday Green $50"] = new[] { "周日绿色 $50 的重购卫星赛", "周日綠色 $50 的快速卫星赛" },
            ["Satellite to Sunday Green $50"] = new[] { "周日绿色 $50 的卫星赛", "周日綠色 $50 的卫星赛" },
            ["Satellite to Sunday 6-Max $40"] = new[] { "周日绿色 6-Max $40 的卫星赛", "周日綠色 6-Max $40 的卫星赛" },
            ["Turbo Satellite to Sunday 6-Max $40"] = new[] { "周日绿色 6-Max $40 的重购卫星赛", "周日綠色 6-Max $40 的快速卫星赛" },
            ["Bounty Hunters $63"] = new[] { "悬赏金猎人 $63", "悬赏金猎人 $63" },
            ["Turbo Satellite to Bounty Hunters $63"] = new[] { "悬赏金猎人 $63 的重购卫星赛", "悬赏金猎人 $63 的快速卫星赛" },
            ["Satellite to Bounty Hunters $63"] = new[] { "悬赏金猎人 $63 的卫星赛", "悬赏金猎人 $63 的卫星赛" },
            ["Rebuy Satellite to Bounty Hunters $63"] = new[] { "悬赏金猎人 $63 的重购卫星赛", "悬赏金猎人 $63 的重购卫星赛" },
            ["Rebuy Satellite to Sunday Rebuys $60, $8K GTD"] = new[] { "周日重购 $60, $8K GTD 的重购卫星赛", "周日重購 $60, $8K GTD 的重购卫星赛" },
            ["Rebuy Satellite to Sunday Blue $60"] = new[] { "周日蓝色 60 的重购卫星赛", "周日藍色 60 的重购卫星赛" },
            ["Satellite to Sunday Rebuys $10, $1K GTD"] = new[] { "周日重购 $10, 1K GTD 的卫星赛", "周日重購 $10, 1K GTD 的卫星赛" },
            ["Rebuy Satellite to Sunday Blue $80"] = new[] { "周日蓝色 80 的重购卫星赛", "周日藍色 80 的重购卫星赛" },
            ["Satellite to Sunday Blue $10"] = new[] { "周日蓝色 10 的卫星赛", "周日藍色 10 的卫星赛" },
            ["Rebuy Satellite to Sunday Green $80"] = new[] { "周日绿色 80 的重购卫星赛", "周日綠色 80 的重购卫星赛" },
            ["Satellite to Sunday Rebuys $16, $1.6K GTD"] = new[] { "周日重购 $16, 1.6K GTD 的卫星赛", "周日重購 $16, 1.6K GTD 的卫星赛" },
            ["Satellite to Sunday 6-Max $10"] = new[] { "周日六人桌 10 的卫星赛", "周日六人桌 10 的卫星赛" },
            ["Rebuy Satellite to Sunday Green $50"] = new[] { "周日绿色 50 的重购卫星赛", "周日綠色 50 的重购卫星赛" },
            ["Rebuy Satellite to Sunday 6-Max $80"] = new[] { "周日六人桌 80 的重购卫星赛", "周日六人桌 80 的重购卫星赛" },
            ["Satellite to Daily Red $15"] = new[] { "每日红色 $15 的卫星赛", "每日紅色 $15 的卫星赛" },
            ["Turbo Satellite to Daily Red $15"] = new[] { "每日红色 $15 的重购卫星赛", "每日紅色 $15 的快速卫星赛" },
            ["Daily Red $15"] = new[] { "每日红色 $15", "每日紅色 $15" },
            ["Satellite to Daily Red $25"] = new[] { "每日红色 $25 的卫星赛", "每日紅色 $25 的卫星赛" },
            ["Turbo Satellite to Daily Red $25"] = new[] { "每日红色 $25 的重购卫星赛", "每日紅色 $25 的快速卫星赛" },
            ["Daily Red $25"] = new[] { "每日红色 $25", "每日紅色 $25" },
            ["Satellite to Daily Red $30"] = new[] { "每日红色 $30 的卫星赛", "每日紅色 $30 的卫星赛" },
            ["Turbo Satellite to Daily Red $30"] = new[] { "每日红色 $30 的重购卫星赛", "每日紅色 $30 的快速卫星赛" },
            ["Daily Red $30"] = new[] { "每日红色 $30", "每日紅色 $30" },
            ["Rebuy Satellite to Daily Red $120"] = new[] { "每日红色 $120 的重购卫星赛", "每日紅色 $120 的重购卫星赛" },
            ["Satellite to Daily Red $120"] = new[] { "每日红色 $120 的卫星赛", "每日紅色 $120 的卫星赛" },
            ["Turbo Satellite to Daily Red $120"] = new[] { "每日红色 $120 的重购卫星赛", "每日紅色 $120 的快速卫星赛" },
            ["Daily Red $120"] = new[] { "每日红色 $120", "每日紅色 $120" },
            ["Satellite to Daily Green $30"] = new[] { "每日绿色 $30 的卫星赛", "每日綠色 $30 的卫星赛" },
            ["Turbo Satellite to Daily Green $30"] = new[] { "每日绿色 $30 的重购卫星赛", "每日綠色 $30 的快速卫星赛" },
            ["Daily Green $30"] = new[] { "每日绿色 $30", "每日綠色 $30" },
            ["Satellite to Sunday Red $30"] = new[] { "周日红色 $30 的卫星赛", "周日紅色 $30 的卫星赛" },
            ["Turbo Satellite to Sunday Red $30"] = new[] { "周日红色 $30 的重购卫星赛", "周日紅色 $30 的快速卫星赛" },
            ["Sunday Red $30"] = new[] { "周日红色 $30", "周日紅色 $30" },
            ["Rebuy Satellite to Sunday Red $50"] = new[] { "周日红色 $50 的重购卫星赛", "周日紅色 $50 的重购卫星赛" },
            ["Satellite to Sunday Red $50"] = new[] { "周日红色 $50 的卫星赛", "周日紅色 $50 的卫星赛" },
            ["Turbo Satellite to Sunday Red $50"] = new[] { "周日红色 $50 的重购卫星赛", "周日紅色 $50 的快速卫星赛" },
            ["Sunday Red $50"] = new[] { "周日红色 $50", "周日紅色 $50" },
            ["Rebuy Satellite to Sunday Red $60"] = new[] { "周日红色 $60 的重购卫星赛", "周日紅色 $60 的重购卫星赛" },
            ["Satellite to Sunday Red $60"] = new[] { "周日红色 $60 的卫星赛", "周日紅色 $60 的卫星赛" },
            ["Turbo Satellite to Sunday Red $60"] = new[] { "周日红色 $60 的重购卫星赛", "周日紅色 $60 的快速卫星赛" },
            ["Sunday Red $60"] = new[] { "周日红色 $60", "周日紅色 $60" },
            ["Rebuy Satellite to Sunday Red $240"] = new[] { "周日红色 $240 的重购卫星赛", "周日紅色 $240 的重购卫星赛" },
            ["Satellite to Sunday Red $240"] = new[] { "周日红色 $240 的卫星赛", "周日紅色 $240 的卫星赛" },
            ["Turbo Satellite to Sunday Red $240"] = new[] { "周日红色 $240 的重购卫星赛", "周日紅色 $240 的快速卫星赛" },
            ["Sunday Red $240"] = new[] { "周日红色 $240", "周日紅色 $240" },
            ["Rebuy Satellite to Sunday Green $60"] = new[] { "周日绿色 $60 的重购卫星赛", "周日綠色 $60 的重购卫星赛" },
            ["Satellite to Sunday Green $60"] = new[] { "周日绿色 $60 的卫星赛", "周日綠色 $60 的卫星赛" },
            ["Turbo Satellite to Sunday Green $60"] = new[] { "周日绿色 $60 的重购卫星赛", "周日綠色 $60 的快速卫星赛" },
            ["Sunday Green $60"] = new[] { "周日绿色 $60", "周日綠色 $60" },
            ["T$ Builder $1"] = new[] { "T$ Builder $1", "T$ Builder $1" },
            ["T$ Builder $2"] = new[] { "T$ Builder $2", "T$ Builder $2" },
            ["T$ Builder $4"] = new[] { "T$ Builder $4", "T$ Builder $4" },
            ["T$ Builder $8"] = new[] { "T$ Builder $8", "T$ Builder $8" },
            ["Turbo Satellite to Daily Rebuys $8, $600 GTD"] = new[] { "每日续买 $8, $600 GTD 的重购卫星赛", "每日續買 $8, $600 GTD 的快速卫星赛" },
            ["Daily Rebuys $8, $600 GTD"] = new[] { "每日续买 $8, $600 GTD", "每日續買 $8, $600 GTD" },
            ["Turbo Satellite to Daily Red $30"] = new[] { "每日红色 $30 的重购卫星赛", "每日紅色 $30 的快速卫星赛" },
            ["Turbo Satellite to Daily Red $30"] = new[] { "每日红色 $30 的重购卫星赛", "每日紅色 $30 的快速卫星赛" },
            ["Daily Red $30"] = new[] { "每日红色 $30", "每日紅色 $30" },
            ["Satellite to Daily Rebuys $12, $1K GTD"] = new[] { "每日续买 $12, $1K GTD 的卫星赛", "每日續買 $12, $1K GTD 的卫星赛" },
            ["Turbo Satellite to Daily Rebuys $12, $1K GTD"] = new[] { "每日续买 $12, $1K GTD 的重购卫星赛", "每日續買 $12, $1K GTD 的快速卫星赛" },
            ["Daily Rebuys $12, $1K GTD"] = new[] { "每日续买 $12, $1K GTD", "每日續買 $12, $1K GTD" },
            ["Turbo Satellite to Daily Rebuys $5, $400 GTD"] = new[] { "每日续买 $5, $400 GTD 的重购卫星赛", "每日續買 $5, $400 GTD 的快速卫星赛" },
            ["Daily Rebuys $5, $400 GTD"] = new[] { "每日续买 $5, $400 GTD", "每日續買 $5, $400 GTD" },
            ["Turbo Satellite to Daily Blue $15"] = new[] { "每日蓝色 $15 的重购卫星赛", "每日藍色 $15 的快速卫星赛" },
            ["Daily Blue $15"] = new[] { "每日蓝色 $15", "每日藍色 $15" },
            ["Daily Rebuys $4, $300 GTD"] = new[] { "每日续买 $4, $300 GTD", "每日續買 $4, $300 GTD" },
            ["Turbo Satellite to Daily Green $5"] = new[] { "每日绿色 $5 的重购卫星赛", "每日綠色 $5 的快速卫星赛" },
            ["Daily Green $5"] = new[] { "每日绿色 $5", "每日綠色 $5" },
            ["Satellite to Sunday Rebuys $16, $1.2K GTD"] = new[] { "周日重购 $16, $1.2K GTD 的卫星赛", "周日重購 $16, $1.2K GTD 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $16, $1.2K GTD"] = new[] { "周日重购 $16, $1.2K GTD 的重购卫星赛", "周日重購 $16, $1.2K GTD 的快速卫星赛" },
            ["Sunday Rebuys $16, $1.2K GTD"] = new[] { "周日重购 $16, $1.2K GTD", "周日重購 $16, $1.2K GTD" },
            ["Rebuy Satellite to Sunday Red $80"] = new[] { "周日红色 $80 的重购卫星赛", "周日紅色 $80 的重购卫星赛" },
            ["Satellite to Sunday Red $80"] = new[] { "周日红色 $80 的卫星赛", "周日紅色 $80 的卫星赛" },
            ["Turbo Satellite to Sunday Red $80"] = new[] { "周日红色 $80 的重购卫星赛", "周日紅色 $80 的快速卫星赛" },
            ["Sunday Red $100"] = new[] { "周日红色 $100", "周日紅色 $100" },
            ["Satellite to Sunday Rebuys $24, $2K GTD"] = new[] { "周日重购 $24, $2K GTD 的卫星赛", "周日重購 $24, $2K GTD 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $24, $2K GTD"] = new[] { "周日重购 $24, $2K GTD 的重购卫星赛", "周日重購 $24, $2K GTD 的快速卫星赛" },
            ["Sunday Rebuys $24, $2K GTD"] = new[] { "周日重购 $24, $2K GTD", "周日重購 $24, $2K GTD" },
            ["Satellite to Sunday Rebuys $10, $800 GTD"] = new[] { "周日重购 $10, $800 GTD 的卫星赛", "周日重購 $10, $800 GTD 的卫星赛" },
            ["Turbo Satellite to Sunday Rebuys $10, $800 GTD"] = new[] { "周日重购 $10, $800 GTD 的重购卫星赛", "周日重購 $10, $800 GTD 的快速卫星赛" },
            ["Sunday Rebuys $10, $800 GTD"] = new[] { "周日重购 $10, $800 GTD", "周日重購 $10, $800 GTD" },
            ["Satellite to Sunday Blue $30"] = new[] { "周日蓝色 $30 的卫星赛", "周日藍色 $30 的卫星赛" },
            ["Turbo Satellite to Sunday Blue $30"] = new[] { "周日蓝色 $30 的重购卫星赛", "周日藍色 $30 的快速卫星赛" },
            ["Sunday Blue $30"] = new[] { "周日蓝色 $30", "周日藍色 $30" },
            ["Turbo Satellite to Sunday Rebuys $8, $600 GTD"] = new[] { "周日重购 $8, $600 GTD 的重购卫星赛", "周日重購 $8, $600 GTD 的快速卫星赛" },
            ["Sunday Rebuys $8, $600 GTD"] = new[] { "周日重购 $8, $600 GTD", "周日重購 $8, $600 GTD" },
            ["Satellite to Sunday Green $10"] = new[] { "周日绿色 $10 的卫星赛", "周日綠色 $10 的卫星赛" },
            ["Rebuy Satellite to Sunday Green $10"] = new[] { "周日绿色 $10 的重购卫星赛", "周日綠色 $10 的重购卫星赛" },
            ["Sunday Green $10"] = new[] { "周日绿色 $10", "周日綠色 $10" },
            ["Satellite to Daily Blue $15"] = new[] { "每日蓝色 $15 的卫星赛", "每日藍色 $15 的卫星赛" },
            ["Chinese Zodiac Rat"] = new[] { "中国十二生肖 鼠", "中國十二生肖 鼠" },
            ["Chinese Zodiac Ox"] = new[] { "中国十二生肖 牛", "中國十二生肖 牛" },
            ["Chinese Zodiac Tiger"] = new[] { "中国十二生肖 虎", "中國十二生肖 虎" },
            ["Chinese Zodiac Rabbit"] = new[] { "中国十二生肖 兔", "中國十二生肖 兔" },
            ["Chinese Zodiac Dragon"] = new[] { "中国十二生肖 龙", "中國十二生肖 龍" },
            ["Chinese Zodiac Snake"] = new[] { "中国十二生肖 蛇", "中國十二生肖 蛇" },
            ["Chinese Zodiac Horse"] = new[] { "中国十二生肖 马", "中國十二生肖 馬" },
            ["Chinese Zodiac Goat"] = new[] { "中国十二生肖 羊", "中國十二生肖 羊" },
            ["Chinese Zodiac Monkey"] = new[] { "中国十二生肖 猴", "中國十二生肖 猴" },
            ["Chinese Zodiac Rooster"] = new[] { "中国十二生肖 鸡", "中國十二生肖 雞" },
            ["Chinese Zodiac Dog"] = new[] { "中国十二生肖 狗", "中國十二生肖 狗" },
            ["Chinese Zodiac Pig"] = new[] { "中国十二生肖 猪", "中國十二生肖 豬" }
        };

        #endregion
    }
}