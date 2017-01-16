//-----------------------------------------------------------------------
// <copyright file="HudTrackConditionsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Model.Enums;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// View model for track conditions meter
    /// </summary>
    public class HudTrackConditionsViewModel : ViewModelBase, IHudWindowElement
    {
        private HudTrackConditionsViewModelInfo viewModelInfo;

        private const decimal MaxScores = 10;

        // average pot limits
        private static decimal[,] sixAvgPotLimits = new decimal[,] { { 2, 1.5m }, { 5, 2 }, { 10, 3.5m }, { 25, 7 }, { 50, 14 }, { 100, 22 }, { 200, 34 }, { 400, 46 }, { 600, 70 }, { 1000, 90 }, { 2000, 180 } };
        private static decimal[,] fullRingAvgPotLimits = new decimal[,] { { 2, 3 }, { 5, 4 }, { 10, 6 }, { 25, 10 }, { 50, 18 }, { 100, 30 }, { 200, 42 }, { 400, 60 }, { 600, 90 }, { 1000, 130 }, { 2000, 220 } };

        // vpip limits
        private static decimal[,] sixVPIPLimits = new decimal[,] { { 25, 41 }, { 200, 36 }, { 400, 32 } };
        private static decimal[,] fullVPIPLimits = new decimal[,] { { 25, 40 }, { 200, 34 }, { 400, 30 } };

        // 3-bet limits
        private static decimal[,] six3BetLimits = new decimal[,] { { 25, 4 }, { 200, 5 }, { 400, 6 } };
        private static decimal[,] full3BetLimits = new decimal[,] { { 25, 3 }, { 200, 4 }, { 400, 5 } };

        public HudTrackConditionsViewModel(HudTrackConditionsViewModelInfo viewModelInfo)
        {
            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        #region IHudWindowElement

        private double offsetX;

        public double OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetX, value);
            }
        }

        private double offsetY;

        public double OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref offsetY, value);
            }
        }

        #endregion

        private void Initialize()
        {
            CalculatePotScores();
            CalculateVPIPScores();
            CalculateThreeBetScores();
        }

        private void CalculatePotScores()
        {
            var limits = viewModelInfo.TableType <= EnumTableType.Six ? sixAvgPotLimits : fullRingAvgPotLimits;
            PotScores = GetScores(limits, viewModelInfo.AveragePot, viewModelInfo.BuyInNL);
        }

        private void CalculateVPIPScores()
        {
            var limits = viewModelInfo.TableType <= EnumTableType.Six ? sixVPIPLimits : fullVPIPLimits;
            VPIPScores = GetScores(limits, viewModelInfo.VPIP, viewModelInfo.BuyInNL);
        }

        private void CalculateThreeBetScores()
        {
            var limits = viewModelInfo.TableType <= EnumTableType.Six ? six3BetLimits : full3BetLimits;
            ThreeBetScores = GetScores(limits, viewModelInfo.ThreeBet, viewModelInfo.BuyInNL, isReverse: true);
        }

        private static decimal GetScores(decimal max, decimal current, decimal maxScores, bool isReverse)
        {
            if (max == 0)
            {
                return 0;
            }

            if ((current >= max && !isReverse) || (current <= max && isReverse))
            {
                return maxScores;
            }

            var scores = current / max * maxScores;

            if (isReverse)
            {
                scores = maxScores - (scores - maxScores);
                if (scores < 0)
                {
                    scores = 0;
                }
            }

            return scores;
        }

        private static decimal GetScores(decimal[,] limits, decimal current, int buyInNL, bool isReverse = false)
        {
            var max = 0m;

            var length = limits.Length / 2;

            for (int i = 0; i < length; i++)
            {
                if (buyInNL <= limits[i, 0])
                {
                    max = limits[i, 1];
                    break;
                }

                // if more than last limit
                if (i == length - 1)
                {
                    max = limits[i, 1];
                }
            }

            var scores = GetScores(max, current, MaxScores, isReverse);

            return scores;
        }

        public void Cleanup()
        {

        }

        #region Properties

        public decimal PotScores
        {
            get;
            private set;
        }

        public decimal VPIPScores
        {
            get;
            private set;
        }

        public decimal ThreeBetScores
        {
            get;
            private set;
        }

        public decimal VPIP
        {
            get
            {
                return viewModelInfo.VPIP;
            }

        }

        public decimal ThreeBet
        {
            get
            {
                return viewModelInfo.ThreeBet;
            }
        }

        public decimal AveragePot
        {
            get
            {
                return viewModelInfo.AveragePot;
            }
        }

        public decimal AverageScores
        {
            get
            {
                return (PotScores + VPIPScores + ThreeBetScores) / 3;
            }
        }

        #endregion
    }
}