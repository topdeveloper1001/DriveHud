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
using Model.Stats;
using System.Collections.Generic;

namespace Model.Data
{
    public class HudIndicators : HudLightIndicators
    {
        private Dictionary<StatBase, Dictionary<string, StatDto>> heatMaps = new Dictionary<StatBase, Dictionary<string, StatDto>>();

        public HudIndicators() : base()
        {
            InitializeHeatMaps();
        }

        public HudIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
            InitializeHeatMaps();
        }

        private void InitializeHeatMaps()
        {
            var heatMapsStats = StatsProvider.GetHeatMapStats();
            heatMapsStats.ForEach(heatMapStat => heatMaps.Add(heatMapStat, new Dictionary<string, StatDto>()));
        }

        public Dictionary<StatBase, Dictionary<string, StatDto>> HeatMaps
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

                if (!heatMaps[stat].ContainsKey(cardsRange))
                {
                    heatMaps[stat].Add(cardsRange, new StatDto());
                }

                heatMaps[stat][cardsRange].Occurred += statDto.Occurred;
                heatMaps[stat][cardsRange].CouldOccurred += statDto.CouldOccurred;
            }
        }
    }
}