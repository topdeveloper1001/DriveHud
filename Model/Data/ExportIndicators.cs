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
    public class ExportIndicators : Indicators, IThreadSafeIndicators
    {
        private int vpiphands;

        private int totalHands;

        private int numberOfWalks;

        private int pfrhands;

        private int didThreeBet;

        private int couldThreeBet;

        private int totalbets;

        private int totalpostflopstreetsplayed;

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

        public override decimal ThreeBet
        {
            get
            {
                return GetPercentage(didThreeBet, couldThreeBet);
            }
        }

        public override decimal AggPr
        {
            get
            {
                return GetPercentage(totalbets, totalpostflopstreetsplayed);
            }
        }

        public override decimal TotalHands => totalHands;

        public override void AddStatistic(Playerstatistic statistic)
        {
            AddStatValue(ref vpiphands, statistic.Vpiphands);
            AddStatValue(ref pfrhands, statistic.Pfrhands);
            AddStatValue(ref numberOfWalks, statistic.NumberOfWalks);
            AddStatValue(ref totalHands, statistic.Totalhands);
            AddStatValue(ref didThreeBet, statistic.Didthreebet);
            AddStatValue(ref couldThreeBet, statistic.Couldthreebet);
            AddStatValue(ref totalbets, statistic.Totalbets);
            AddStatValue(ref totalpostflopstreetsplayed, statistic.Totalpostflopstreetsplayed);
        }

        private void AddStatValue(ref int statValue, int value)
        {
            int initialValue, computedValue;

            do
            {
                initialValue = statValue;
                computedValue = statValue + value;
            }
            while (initialValue != Interlocked.CompareExchange(ref statValue, computedValue, initialValue));
        }
    }
}