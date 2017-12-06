﻿//-----------------------------------------------------------------------
// <copyright file="Top20BiggestLosingHandsSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class Top20BiggestLosingHandsSeriesProvider : IGraphSeriesProvider
    {
        private List<GraphSerieDataPoint> dataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            var topIndex = 1;

            foreach (var dataPoint in dataPoints.OrderBy(x => x.Value).Take(20))
            {
                dataPoint.Category = string.Format("#{0} - ${1:#.##}", topIndex++, Math.Abs(dataPoint.Value));

                series.Add(new GraphSerie
                {
                    Legend = $"Hand #{dataPoint.Category}",
                    DataPoints = new ObservableCollection<GraphSerieDataPoint> { dataPoint }
                });
            }

            return series;
        }

        public void Process(Playerstatistic statistic)
        {
            if (statistic == null || statistic.IsTourney || statistic.NetWon >= 0)
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