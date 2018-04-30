//-----------------------------------------------------------------------
// <copyright file="ExportIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Threading;

namespace Model.Data
{
    public class ExportIndicators : Indicators
    {
        private int vpiphands;

        private int totalHands;

        private int numberOfWalks;

        private int pfrhands;

        public override decimal VPIP
        {
            get
            {
                return GetPercentage(vpiphands, totalHands - numberOfWalks);
            }
        }

        public override decimal PFR
        {
            get
            {
                return GetPercentage(pfrhands, totalHands - numberOfWalks);
            }
        }

        public override decimal TotalHands => totalHands;

        public override void AddStatistic(Playerstatistic statistic)
        {
            if (statistic.Vpiphands > 0)
            {
                Interlocked.Increment(ref vpiphands);
            }

            if (statistic.Pfrhands > 0)
            {
                Interlocked.Increment(ref pfrhands);
            }

            if (statistic.NumberOfWalks > 0)
            {
                Interlocked.Increment(ref numberOfWalks);
            }

            Interlocked.Increment(ref totalHands);
        }
    }
}