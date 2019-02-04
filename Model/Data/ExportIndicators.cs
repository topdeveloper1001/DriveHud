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

        public override decimal ThreeBetMPvsEP => 0;

        public override decimal ThreeBetCOvsEP => 0;

        public override decimal ThreeBetCOvsMP => 0;

        public override decimal ThreeBetBTNvsEP => 0;

        public override decimal ThreeBetBTNvsMP => 0;

        public override decimal ThreeBetBTNvsCO => 0;

        public override decimal ThreeBetSBvsEP => 0;

        public override decimal ThreeBetSBvsMP => 0;

        public override decimal ThreeBetSBvsCO => 0;

        public override decimal ThreeBetSBvsBTN => 0;

        public override decimal ThreeBetBBvsEP => 0;

        public override decimal ThreeBetBBvsMP => 0;

        public override decimal ThreeBetBBvsCO => 0;

        public override decimal ThreeBetBBvsBTN => 0;

        public override decimal ThreeBetBBvsSB => 0;

        public override decimal FoldTo3BetInEPvs3BetMP => 0;

        public override decimal FoldTo3BetInEPvs3BetCO => 0;

        public override decimal FoldTo3BetInEPvs3BetBTN => 0;

        public override decimal FoldTo3BetInEPvs3BetSB => 0;

        public override decimal FoldTo3BetInEPvs3BetBB => 0;

        public override decimal FoldTo3BetInMPvs3BetCO => 0;

        public override decimal FoldTo3BetInMPvs3BetBTN => 0;

        public override decimal FoldTo3BetInMPvs3BetSB => 0;

        public override decimal FoldTo3BetInMPvs3BetBB => 0;

        public override decimal FoldTo3BetInCOvs3BetBTN => 0;

        public override decimal FoldTo3BetInCOvs3BetSB => 0;

        public override decimal FoldTo3BetInCOvs3BetBB => 0;

        public override decimal FoldTo3BetInBTNvs3BetSB => 0;

        public override decimal FoldTo3BetInBTNvs3BetBB => 0;

        public override decimal CheckRaiseFlopAsPFR => 0;

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