//-----------------------------------------------------------------------
// <copyright file="BB100ByTimeOfDaySeriesProvider.cs" company="Ace Poker Solutions">
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
using System.Linq;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class BB100ByTimeOfDaySeriesProvider : IGraphSeriesProvider
    {
        private Dictionary<int, GraphSerieDataPoint> dataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            if (dataPoints == null)
            {
                return series;
            }

            foreach (var dataPointKey in dataPoints.Keys.OrderBy(x => x))
            {
                var dataPoint = dataPoints[dataPointKey];

                var handsCount = (int)dataPoint.Category;
                var bb100value = dataPoint.Value / handsCount * 100;

                var dataPointLabel = string.Format("{0}:00 - {0}:59: {1:#.##}", dataPointKey, bb100value);

                dataPoint.Category = dataPointLabel;
                dataPoint.Value = Math.Abs(bb100value);

                series.Add(new GraphSerie
                {
                    Legend = dataPointLabel,
                    DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint }
                });
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
                dataPoints = new Dictionary<int, GraphSerieDataPoint>();
            }

            if (!dataPoints.ContainsKey(statistic.Time.Hour))
            {
                dataPoints.Add(statistic.Time.Hour, new GraphSerieDataPoint
                {
                    Category = 0
                });
            }

            dataPoints[statistic.Time.Hour].Category = (int)dataPoints[statistic.Time.Hour].Category + 1;
            dataPoints[statistic.Time.Hour].Value += statistic.NetWon / statistic.BigBlind;
        }
    }
}