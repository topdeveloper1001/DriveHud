//-----------------------------------------------------------------------
// <copyright file="ChartItemDateKey.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.ChartData
{
    public class ChartItemDateKey
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int Day { get; set; }

        public int Hour { get; set; }

        public override bool Equals(object obj)
        {
            var dateKey = obj as ChartItemDateKey;

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

        private bool Equals(ChartItemDateKey dateKey)
        {
            return Year == dateKey.Year && Month == dateKey.Month && Day == dateKey.Day && Hour == dateKey.Hour;
        }
    }
}