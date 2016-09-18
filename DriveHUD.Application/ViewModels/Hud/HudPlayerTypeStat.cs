//-----------------------------------------------------------------------
// <copyright file="HudPlayerTypeStat.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Hud player type stat range
    /// </summary>
    [Serializable]
    public class HudPlayerTypeStat : ReactiveObject
    {
        #region Properties

        private decimal? low;

        public decimal? Low
        {
            get
            {
                return low;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref low, value);
                SwapLowHigh();
            }
        }

        private decimal? high;

        public decimal? High
        {
            get
            {
                return high;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref high, value);
                SwapLowHigh();
            }
        }

        private Stat stat;

        public Stat Stat
        {
            get
            {
                return stat;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref stat, value);
            }
        }

        #endregion

        public HudPlayerTypeStat Clone()
        {
            return (HudPlayerTypeStat)MemberwiseClone();
        }

        private void SwapLowHigh()
        {
            if (!Low.HasValue || !High.HasValue || (Low <= High))
            {
                return;
            }

            var temp = Low;
            Low = High;
            High = temp;
        }
    }
}