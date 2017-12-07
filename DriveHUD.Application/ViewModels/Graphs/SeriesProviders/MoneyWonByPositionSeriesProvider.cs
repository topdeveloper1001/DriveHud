//-----------------------------------------------------------------------
// <copyright file="MoneyWonByPositionSeriesProvider.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class MoneyWonByPositionSeriesProvider : IGraphSeriesProvider
    {
        private Dictionary<string, GraphSerieDataPoint> dataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            if (dataPoints == null)
            {
                return series;
            }

            foreach (KeyValuePair<string, GraphSerieDataPoint> dataPoint in dataPoints)
            {
                var dataPointLabel = string.Format("{0}: ${1:#.##}", dataPoint.Key, dataPoint.Value.Value);

                dataPoint.Value.Category = dataPointLabel;
                dataPoint.Value.Value = Math.Abs(dataPoint.Value.Value);

                series.Add(new GraphSerie
                {
                    Legend = dataPointLabel,
                    DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint.Value }
                });
            }

            return series;
        }

        public void Process(Playerstatistic statistic)
        {
            if (statistic == null || statistic.IsTourney || string.IsNullOrEmpty(statistic.PositionString))
            {
                return;
            }

            if (dataPoints == null)
            {
                dataPoints = new Dictionary<string, GraphSerieDataPoint>();
            }

            if (!dataPoints.ContainsKey(statistic.PositionString))
            {
                dataPoints.Add(statistic.PositionString, new GraphSerieDataPoint());
            }

            dataPoints[statistic.PositionString].Value += statistic.NetWon;
        }
    }
}