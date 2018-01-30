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

using DriveHUD.Common.Wpf.Mvvm;
using Model.Stats;
using ReactiveUI;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudGaugeIndicatorStatsGroupViewModel : ViewModelBase
    {
        public static ReactiveList<HudGaugeIndicatorStatsGroupViewModel> GroupStats(IEnumerable<StatInfo> stats)
        {
            var groupedStats = new ReactiveList<HudGaugeIndicatorStatsGroupViewModel>();

            if (stats == null)
            {
                return groupedStats;
            }

            HudGaugeIndicatorStatsGroupViewModel group = null;

            foreach (var stat in stats)
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

                group.Stats.Add(stat);
            }

            return groupedStats;
        }

        private HudGaugeIndicatorStatGroupType groupType;

        public HudGaugeIndicatorStatGroupType GroupType
        {
            get
            {
                return groupType;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref groupType, value);
            }
        }

        private readonly ReactiveList<StatInfo> stats = new ReactiveList<StatInfo>();

        public ReactiveList<StatInfo> Stats
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