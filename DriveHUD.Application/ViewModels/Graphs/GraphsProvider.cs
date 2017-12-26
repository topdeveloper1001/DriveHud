//-----------------------------------------------------------------------
// <copyright file="GraphsProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Graphs
{
    internal class GraphsProvider : IGraphsProvider
    {
        private Dictionary<SerieType, IGraphSeriesProvider> providers;

        public Dictionary<SerieType, IEnumerable<GraphSerie>> GetSeries()
        {
            var series = new Dictionary<SerieType, IEnumerable<GraphSerie>>();

            foreach (var provider in providers)
            {
                series.Add(provider.Key, provider.Value.GetSeries());
            }

            return series;
        }

        public void Initialize(IEnumerable<SerieType> seriesTypes)
        {
            providers = new Dictionary<SerieType, IGraphSeriesProvider>();

            foreach (var seriesType in seriesTypes)
            {                
                var provider = ServiceLocator.Current.GetInstance<IGraphSeriesProvider>(seriesType.ToString());

                providers.Add(seriesType, provider);
            }
        }

        public void Process(Playerstatistic statistic)
        {
            if (providers == null)
            {
                return;
            }

            foreach (var provider in providers.Values)
            {
                provider.Process(statistic);
            }
        }
    }
}
