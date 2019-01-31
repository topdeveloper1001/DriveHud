//-----------------------------------------------------------------------
// <copyright file="B100GraphIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using DriveHUD.Entities;

namespace Model.Data
{
    public class B100GraphIndicators : LightIndicators
    {
        public B100GraphIndicators(int totalHand = 0)
        {
            if (totalHand < 100)
            {
                approximationRange = 0;
            }
            else if (totalHand < 1000)
            {
                approximationRange = 100;
            }
            else if (totalHand < 10000)
            {
                approximationRange = 1000;
            }
            else
            {
                approximationRange = 5000;
            }
        }

        private int approximationRange;

        public override decimal BB
        {
            get
            {
                var totalhands = (statisticCount < approximationRange ? approximationRange : statisticCount) / 100m;
                return Math.Round(GetDivisionResult(netWonByBigBlind, totalhands), 2);
            }
        }

        public override decimal TotalHands => statisticCount;

        public override void AddStatistic(Playerstatistic statistic)
        {
            statisticCount++;
            netWonByBigBlind += GetDivisionResult(statistic.NetWon, statistic.BigBlind);
        }
    }
}