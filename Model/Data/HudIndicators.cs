//-----------------------------------------------------------------------
// <copyright file="HudIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using HandHistories.Parser.Utils.FastParsing;
using Model.Enums;
using Model.Stats;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    public class HudIndicators : HudLightIndicators
    {
        private Dictionary<StatBase, HeatMapDto> heatMaps = new Dictionary<StatBase, HeatMapDto>();

        public HudIndicators() : base()
        {
            InitializeHeatMaps();
        }

        public HudIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
            InitializeHeatMaps();
        }

        public HudIndicators(IEnumerable<Stat> heatMapStats) : base()
        {
            var heatMapStatsHash = new HashSet<Stat>(heatMapStats.Distinct());

            var heatMapsStats = StatsProvider.StatsBases.Values.Where(x => heatMapStatsHash.Contains(x.Stat));
            heatMapsStats.ForEach(heatMapStat => heatMaps.Add(heatMapStat, new HeatMapDto()));
        }

        private void InitializeHeatMaps()
        {
            var heatMapsStats = StatsProvider.GetHeatMapStats();
            heatMapsStats.ForEach(heatMapStat => heatMaps.Add(heatMapStat, new HeatMapDto()));
        }

        public Dictionary<StatBase, HeatMapDto> HeatMaps
        {
            get
            {
                return heatMaps;
            }
        }

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);

            foreach (var stat in heatMaps.Keys)
            {
                if (stat.CreateStatDto == null)
                {
                    continue;
                }

                var statDto = stat.CreateStatDto(statistic);

                var cardsRange = ParserUtils.ConvertToCardRange(statistic.Cards);

                if (string.IsNullOrEmpty(cardsRange))
                {
                    continue;
                }

                if (!heatMaps[stat].OccuredByCardRange.ContainsKey(cardsRange))
                {
                    heatMaps[stat].OccuredByCardRange.Add(cardsRange, 0);
                }

                heatMaps[stat].OccuredByCardRange[cardsRange] += statDto.Occurred;
            }
        }
    }
}