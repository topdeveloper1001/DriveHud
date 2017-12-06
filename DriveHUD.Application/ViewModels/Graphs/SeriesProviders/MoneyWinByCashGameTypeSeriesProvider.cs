//-----------------------------------------------------------------------
// <copyright file="MoneyWinByCashGameTypeSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Entities;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class MoneyWinByCashGameTypeSeriesProvider : IGraphSeriesProvider
    {
        private Dictionary<string, GraphSerieDataPoint> dataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            foreach (KeyValuePair<string, GraphSerieDataPoint> dataPoint in dataPoints)
            {
                var serie = new GraphSerie
                {
                    DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint.Value },
                    Legend = dataPoint.Key
                };

                series.Add(serie);
            }

            return series;
        }

        public void Process(Playerstatistic statistic)
        {
            if (statistic == null || statistic.IsTourney)
            {
                return;
            }

            if (dataPoints == null)
            {
                dataPoints = new Dictionary<string, GraphSerieDataPoint>();
            }

            if (!dataPoints.ContainsKey(statistic.GameType))
            {
                dataPoints.Add(statistic.GameType, new GraphSerieDataPoint
                {
                    Category = statistic.GameType,
                    Value = statistic.NetWon
                });
            }

            dataPoints[statistic.GameType].Value += statistic.NetWon;
        } 
    }
}