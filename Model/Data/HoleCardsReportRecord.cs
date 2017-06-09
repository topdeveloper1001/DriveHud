//-----------------------------------------------------------------------
// <copyright file="HoleCardsReportRecord.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Extensions;
using System;

namespace Model.Data
{
    /// <summary>
    /// Extended indicator for Hole Cards Report
    /// </summary>
    public class HoleCardsReportRecord : ReportIndicators
    {
        public virtual decimal WonHandProc
        {
            get
            {
                if (TotalHands == 0)
                {
                    return 0;
                }

                return (Source.Wonhand / TotalHands) * 100;
            }
        }

        public virtual ComparableReportCards Cards { get; set; }
    }

    public class ComparableReportCards : IComparable<ComparableReportCards>
    {
        public string CardsString { get; set; }

        public override string ToString()
        {
            return CardsString;
        }

        public int CompareTo(ComparableReportCards other)
        {
            if (string.IsNullOrEmpty(CardsString) || string.IsNullOrEmpty(other.CardsString))
            {
                if (string.IsNullOrEmpty(CardsString) && string.IsNullOrEmpty(other.CardsString))
                {
                    return 0;
                }
                else if (string.IsNullOrEmpty(CardsString))
                {
                    return -1;
                }
                else
                {
                    return 1;
                }
            }

            if (CardsString.Length >= 2 && other.CardsString.Length >= 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    int result = CardHelper.GetCardRank(CardsString[i].ToString()) - CardHelper.GetCardRank(other.CardsString[i].ToString());

                    if (result != 0)
                    {
                        return result;
                    }
                }
            }

            return CardsString.Length - other.CardsString.Length;
        }
    }
}