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

namespace Model.Data
{
    public class B100GraphIndicators : LightIndicators
    {
        private const int approximationRange = 5000;

        public override decimal BB
        {
            get
            {
                var totalhands = (statisticCount < approximationRange ? approximationRange : statisticCount) / 100m;
                return Math.Round(GetDivisionResult(netWonByBigBlind, totalhands), 2);
            }
        }
    }
}