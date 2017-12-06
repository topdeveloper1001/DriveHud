//-----------------------------------------------------------------------
// <copyright file="WinningByMonthSeriesProvider.cs" company="Ace Poker Solutions">
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
using Model.ChartData;
using Model.Importer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class WinningByMonthSeriesProvider : IGraphSeriesProvider
    {
        private Dictionary<ChartItemDateKey, GraphSerieDataPoint> showDownDataPoints;
        private Dictionary<ChartItemDateKey, GraphSerieDataPoint> nonShowDownDataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            var showDownSerie = new GraphSerie
            {
                DataPoints = new ObservableCollection<GraphSerieDataPoint>(showDownDataPoints.Values),
                Legend = "Showdown"
            };

            var nonShowDownSerie = new GraphSerie
            {
                DataPoints = new ObservableCollection<GraphSerieDataPoint>(nonShowDownDataPoints.Values),
                Legend = "Non showdown"
            };

            series.Add(showDownSerie);
            series.Add(nonShowDownSerie);

            return series;
        }

        public void Process(Playerstatistic statistic)
        {
            if (statistic == null || statistic.IsTourney)
            {
                return;
            }

            if (showDownDataPoints == null)
            {
                showDownDataPoints = new Dictionary<ChartItemDateKey, GraphSerieDataPoint>();
            }

            if (nonShowDownDataPoints == null)
            {
                nonShowDownDataPoints = new Dictionary<ChartItemDateKey, GraphSerieDataPoint>();
            }

            var groupKey = BuildGroupKey(statistic);


            if (!nonShowDownDataPoints.ContainsKey(groupKey))
            {
                nonShowDownDataPoints.Add(groupKey, new GraphSerieDataPoint
                {
                    Category = GetDateTimeFromGroupKey(groupKey)
                });
            }

            if (!showDownDataPoints.ContainsKey(groupKey))
            {
                showDownDataPoints.Add(groupKey, new GraphSerieDataPoint
                {
                    Category = GetDateTimeFromGroupKey(groupKey)
                });
            }

            if (statistic.Sawshowdown == 0)
            {
                nonShowDownDataPoints[groupKey].Value += statistic.NetWon;
            }
            else
            {
                showDownDataPoints[groupKey].Value += statistic.NetWon;
            }
        }

        protected virtual ChartItemDateKey BuildGroupKey(Playerstatistic statistic)
        {
            if (statistic == null)
            {
                return null;
            }

            var time = Converter.ToLocalizedDateTime(statistic.Time);

            var groupKey = new ChartItemDateKey
            {
                Year = time.Year,
                Month = time.Month
            };

            return groupKey;
        }

        protected virtual DateTime GetDateTimeFromGroupKey(ChartItemDateKey dateKey)
        {
            var dateTime = new DateTime(dateKey.Year, dateKey.Month, 1);
            return dateTime;
        }
    }
}