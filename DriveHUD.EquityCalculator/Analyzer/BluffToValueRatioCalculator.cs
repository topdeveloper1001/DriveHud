//-----------------------------------------------------------------------
// <copyright file="BluffToValueRatioCalculator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.Analyzer
{
    internal class BluffToValueRatioCalculator
    {
        private const double allowedDeviation = 0.05;

        private static readonly Dictionary<Street, double[]> PredefinedRatio = new Dictionary<Street, double[]>
        {
            [Street.Flop] = new[] { 1.6, 2.0 },
            [Street.Turn] = new[] { 1.0, 1.0 },
            [Street.River] = new[] { 0.5, 0.5 },
        };

        public static bool CheckRatio(int bluffCombos, int valueCombos, Street street, out int[] increaseBluffBy, out int[] increaseValueBy)
        {
            increaseBluffBy = new[] { 0, 0 };
            increaseValueBy = new[] { 0, 0 };

            if ((bluffCombos == 0 && valueCombos == 0) ||
                !PredefinedRatio.TryGetValue(street, out double[] expectedRatio))
            {
                return true;
            }

            var actualRatio = (double)bluffCombos / valueCombos;

            if (actualRatio <= expectedRatio[1] + allowedDeviation &&
                actualRatio >= expectedRatio[0] - allowedDeviation)
            {
                return true;
            }

            if (actualRatio > expectedRatio[1])
            {
                increaseValueBy[0] = valueCombos != 0 ? (int)Math.Round((Math.Round(bluffCombos / expectedRatio[0]) / valueCombos - 1) * 100) : 0;
                increaseValueBy[1] = valueCombos != 0 ? (int)Math.Round((Math.Round(bluffCombos / expectedRatio[1]) / valueCombos - 1) * 100) : 0;
            }
            else
            {
                increaseBluffBy[0] = bluffCombos != 0 ? (int)Math.Round((Math.Round(expectedRatio[0] * valueCombos) / bluffCombos - 1) * 100) : 0;
                increaseBluffBy[1] = bluffCombos != 0 ? (int)Math.Round((Math.Round(expectedRatio[1] * valueCombos) / bluffCombos - 1) * 100) : 0;
            }

            return false;
        }

        public static readonly Dictionary<Street, string> RecommendedRange = new Dictionary<Street, string>()
        {
            [Street.Flop] = "1.6-2:1",
            [Street.Turn] = "1:1",
            [Street.River] = "1:2",
        };
    }
}