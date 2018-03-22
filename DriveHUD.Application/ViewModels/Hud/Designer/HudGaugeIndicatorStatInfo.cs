//-----------------------------------------------------------------------
// <copyright file="HudGaugeIndicatorStatInfo.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Stats;
using Prism.Mvvm;
using ProtoBuf;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudGaugeIndicatorStatInfo : BindableBase
    {
        private HudGaugeIndicatorStatInfo()
        {
        }

        public HudGaugeIndicatorStatInfo(StatInfo stat)
        {
            this.stat = stat;
        }

        [ProtoMember(1)]
        private StatInfo stat;

        public StatInfo Stat
        {
            get
            {
                return stat;
            }
        }

        [ProtoMember(2)]
        private HudHeatMapViewModel heatMapViewModel;

        public HudHeatMapViewModel HeatMapViewModel
        {
            get
            {
                return heatMapViewModel;
            }
            set
            {
                SetProperty(ref heatMapViewModel, value);
            }
        }

        public bool IsHeatMapVisible
        {
            get
            {
                return HeatMapViewModel != null;
            }
        }
    }
}