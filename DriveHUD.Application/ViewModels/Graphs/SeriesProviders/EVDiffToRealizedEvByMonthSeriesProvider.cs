//-----------------------------------------------------------------------
// <copyright file="EVDiffToRealizedEvByMonthSeriesProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using Model.ChartData;
using System.Collections.ObjectModel;
using Model.Importer;

namespace DriveHUD.Application.ViewModels.Graphs.SeriesProviders
{
    internal class EVDiffToRealizedEvByMonthSeriesProvider : IGraphSeriesProvider
    {
        private Dictionary<ChartItemDateKey, GraphSerieDataPoint> evDiffDataPoints;
        private Dictionary<ChartItemDateKey, GraphSerieDataPoint> evDataPoints;

        public IEnumerable<GraphSerie> GetSeries()
        {
            var series = new List<GraphSerie>();

            var showDownSerie = new GraphSerie
            {
                DataPoints = new ObservableCollection<GraphSerieDataPoint>(evDiffDataPoints.Values),
                Legend = "EV Diff"
            };

            var nonShowDownSerie = new GraphSerie
            {
                DataPoints = new ObservableCollection<GraphSerieDataPoint>(evDataPoints.Values),
                Legend = "EV"
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

            if (evDiffDataPoints == null)
            {
                evDiffDataPoints = new Dictionary<ChartItemDateKey, GraphSerieDataPoint>();
            }

            if (evDataPoints == null)
            {
                evDataPoints = new Dictionary<ChartItemDateKey, GraphSerieDataPoint>();
            }

            var groupKey = BuildGroupKey(statistic);

            if (!evDiffDataPoints.ContainsKey(groupKey))
            {
                evDiffDataPoints.Add(groupKey, new GraphSerieDataPoint
                {
                    Category = GetDateTimeFromGroupKey(groupKey)
                });
            }

            if (!evDataPoints.ContainsKey(groupKey))
            {
                evDataPoints.Add(groupKey, new GraphSerieDataPoint
                {
                    Category = GetDateTimeFromGroupKey(groupKey)
                });
            }

            evDiffDataPoints[groupKey].Value += statistic.EVDiff;
            evDataPoints[groupKey].Value += statistic.Equity;
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