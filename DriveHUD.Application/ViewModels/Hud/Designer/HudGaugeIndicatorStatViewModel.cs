//-----------------------------------------------------------------------
// <copyright file="HudGaugeIndicatorViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Layouts;
using Model.Stats;
using Prism.Mvvm;
using ProtoBuf;
using ReactiveUI;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudGaugeIndicatorStatsGroupViewModel : BindableBase
    {
        public static ReactiveList<HudGaugeIndicatorStatsGroupViewModel> GroupStats(HudLayoutGaugeIndicator tool, HudElementViewModel hudElementViewModel)
        {
            var groupedStats = new ReactiveList<HudGaugeIndicatorStatsGroupViewModel>();

            if (tool.Stats == null)
            {
                return groupedStats;
            }

            HudGaugeIndicatorStatsGroupViewModel group = null;

            foreach (var stat in tool.Stats)
            {
                var groupType = GetStatGroupType(stat);

                if (group == null || group.GroupType != groupType)
                {
                    group = new HudGaugeIndicatorStatsGroupViewModel
                    {
                        GroupType = groupType
                    };

                    groupedStats.Add(group);
                }

                var gaugeIndicatorStatInfo = new HudGaugeIndicatorStatInfo(stat);

                if (hudElementViewModel != null)
                {
                    gaugeIndicatorStatInfo.HeatMapViewModel = (HudHeatMapViewModel)tool.Tools
                        .OfType<HudLayoutHeatMapTool>()
                        .FirstOrDefault(x => x.BaseStat != null && x.BaseStat.Stat == stat.Stat)?
                        .CreateViewModel(hudElementViewModel);
                }

                group.Stats.Add(gaugeIndicatorStatInfo);
            }

            return groupedStats;
        }

        [ProtoMember(1)]
        private HudGaugeIndicatorStatGroupType groupType;

        public HudGaugeIndicatorStatGroupType GroupType
        {
            get
            {
                return groupType;
            }
            private set
            {
                SetProperty(ref groupType, value);
            }
        }

        [ProtoMember(2)]
        private ReactiveList<HudGaugeIndicatorStatInfo> stats = new ReactiveList<HudGaugeIndicatorStatInfo>();

        public ReactiveList<HudGaugeIndicatorStatInfo> Stats
        {
            get
            {
                return stats;
            }
        }

        private static HudGaugeIndicatorStatGroupType GetStatGroupType(StatInfo stat)
        {
            if (stat is StatInfoBreak)
            {
                return HudGaugeIndicatorStatGroupType.BreakLine;
            }

            if (stat != null && stat.IsPopupBarNotSupported)
            {
                return HudGaugeIndicatorStatGroupType.Text;
            }

            return HudGaugeIndicatorStatGroupType.LineBar;
        }
    }    
}