//-----------------------------------------------------------------------
// <copyright file="HoleCardsReportStringExtensions.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Extensions;
using System.Linq;

namespace Model.Reports
{
    public static class HoleCardsReportStringExtensions
    {
        public static string ToCards(this string holeCards)
        {
            if (string.IsNullOrWhiteSpace(holeCards))
            {
                return string.Empty;
            }

            var cards = CardHelper.Split(holeCards);

            if (cards.Count != 2)
            {
                return string.Empty;
            }

            var value1 = cards[0].TrimEnd('c', 'd', 'h', 's');
            var value2 = cards[1].TrimEnd('c', 'd', 'h', 's');

            var result = value1 + value2;

            if (value1 != value2)
            {
                result += cards[0].Last() == cards[1].Last() ? "s" : "o";
            }

            return result;
        }
    }
}