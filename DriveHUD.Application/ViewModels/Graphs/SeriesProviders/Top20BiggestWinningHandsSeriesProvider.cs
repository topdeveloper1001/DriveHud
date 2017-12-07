//-----------------------------------------------------------------------
// <copyright file="Top20BiggestWinningHandsSeriesProvider.cs" company="Ace Poker Solutions">
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
    internal class Top20BiggestWinningHandsSeriesProvider : IGraphSeriesProvider
    {
        private List<GraphSerieDataPoint> dataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            if (dataPoints == null)
            {
                return series;
            }

            var topIndex = 1;

            foreach (var dataPoint in dataPoints.OrderByDescending(x => x.Value).Take(20))
            {
                var dataPointLabel = string.Format("#{0}: ${1:#.##}", topIndex++, Math.Abs(dataPoint.Value));

                dataPoint.Category = dataPointLabel;

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
            if (statistic == null || statistic.IsTourney || statistic.NetWon <= 0)
            {
                return;
            }

            if (dataPoints == null)
            {
                dataPoints = new List<GraphSerieDataPoint>();
            }

            var dataPoint = new GraphSerieDataPoint
            {
                Value = statistic.NetWon
            };

            dataPoints.Add(dataPoint);
        }
    }
}