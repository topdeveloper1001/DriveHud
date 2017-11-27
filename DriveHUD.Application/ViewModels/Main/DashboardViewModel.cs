//-----------------------------------------------------------------------
// <copyright file="DashboardViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Common.Log;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.ChartData;
using Model.Data;
using Model.Enums;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Fields

        private ObservableCollection<ChartSeries> winningsChartCollection;
        private ObservableCollection<ChartSeries> secondChartCollection = new ObservableCollection<ChartSeries>();
        private ChartDisplayRange winningsChartDisplayRange = ChartDisplayRange.Week;
        private ChartDisplayRange secondChartDisplayRange = ChartDisplayRange.Month;
        private LightIndicators indicatorCollection;
        private bool isExpanded = true;

        #endregion

        #region Properties

        public bool IsExpanded
        {
            get
            {
                return isExpanded;
            }
            set
            {
                SetProperty(ref isExpanded, value);
            }
        }

        public ObservableCollection<ChartSeries> WinningsChartCollection
        {
            get
            {
                return winningsChartCollection;
            }
            set
            {
                SetProperty(ref winningsChartCollection, value);
            }
        }

        public ObservableCollection<ChartSeries> SecondChartCollection
        {
            get
            {
                return secondChartCollection;
            }

            set
            {
                secondChartCollection = value;
            }
        }

        public ChartDisplayRange WinningsChartDisplayRange
        {
            get
            {
                return winningsChartDisplayRange;
            }
            set
            {
                SetProperty(ref winningsChartDisplayRange, value);

                SetSerieData(WinningsChartCollection, ChartSerieResourceHelper.GetSerieGreenPalette(), WinningsChartDisplayRange);
            }
        }

        public ChartDisplayRange SecondChartDisplayRange
        {
            get
            {
                return secondChartDisplayRange;
            }
            set
            {
                SetProperty(ref secondChartDisplayRange, value);

                SetSerieData(SecondChartCollection, ChartSerieResourceHelper.GetSerieOrangePalette(), SecondChartDisplayRange);
            }
        }

        public LightIndicators IndicatorCollection
        {
            get
            {
                return indicatorCollection;
            }
            set
            {
                SetProperty(ref indicatorCollection, value);
            }
        }

        #endregion

        internal DashboardViewModel()
        {
            ServiceLocator.Current
                .GetInstance<IEventAggregator>()
                .GetEvent<BuiltFilterChangedEvent>()
                .Subscribe(UpdateFilteredData);

            InitializeCharts();
        }

        private void SetSerieData(IEnumerable<ChartSeries> chartSeriesCollection, ChartSerieResourceHelper resource, ChartDisplayRange displayRange)
        {
            using (var pf = new PerformanceMonitor(nameof(SetSerieData)))
            {
                // filter stats if necessary (only for date ranges)
                // sort stats by date asc
                // loop stats
                //      build key from (stat, range, stat index)
                //      loop chart series
                //          if key equals to previous then update current item with Action<current, stat, previous> or Func<decimal, stat, previous>
                //          if key not equals to previous the create new item and update it with Action<current, stat, previous> or Func<decimal, stat, previous>
                // i=50, i=51, ... bb/100(50)= sum(nw1/bb1+...)/50, bb/100(51) = (bb/100(50)*50+nw51/bb51)/51 ...

                // clear all data
                chartSeriesCollection.ForEach(x => x.ItemsCollection?.Clear());

                var chartItemDataBuilder = CreateCharItemDataBuilder(displayRange);

                // filter and orders
                var stats = chartItemDataBuilder.PrepareStatistic(StorageModel.StatisticCollection.ToList().Where(x => !x.IsTourney));

                object previousGroupKey = null;

                for (var statIndex = 0; statIndex < stats.Length; statIndex++)
                {
                    var stat = stats[statIndex];

                    var currentGroupKey = chartItemDataBuilder.BuildGroupKey(stat, statIndex);

                    var isNew = !currentGroupKey.Equals(previousGroupKey);

                    previousGroupKey = currentGroupKey;

                    foreach (var chartSerie in chartSeriesCollection)
                    {
                        ChartSeriesItem previousChartSeriesItem = null;
                        ChartSeriesItem chartSeriesItem = null;

                        if (isNew)
                        {
                            chartSeriesItem = new ChartSeriesItem
                            {
                                Format = chartSerie.Format,
                                PointColor = resource.PointColor,
                                TrackBallColor = resource.TrackBallColor,
                                TooltipColor = resource.TooltipColor,
                                Category = chartItemDataBuilder.GetValueFromGroupKey(currentGroupKey)
                            };

                            previousChartSeriesItem = chartSerie.ItemsCollection.LastOrDefault();
                            chartSerie.ItemsCollection.Add(chartSeriesItem);
                        }
                        else
                        {
                            previousChartSeriesItem = chartSeriesItem = chartSerie.ItemsCollection.LastOrDefault();
                        }

                        chartSerie.UpdateChartSeriesItem(chartSeriesItem, previousChartSeriesItem, stat);
                    }
                }
            }
        }

        internal void UpdateFilteredData(BuiltFilterChangedEventArgs args)
        {
            if (IndicatorCollection == null)
            {
                IndicatorCollection = new LightIndicators();
            }
            else
            {
                IndicatorCollection.Clean();
            }

            if (StorageModel.FilteredPlayerStatistic == null)
            {
                return;
            }

            var statistics = StorageModel.FilteredPlayerStatistic.Where(x => !x.IsTourney).ToList();

            IndicatorCollection.UpdateSource(statistics);
            OnPropertyChanged(() => IndicatorCollection);
        }

        internal void Update()
        {
            if (StorageModel.StatisticCollection == null)
            {
                return;
            }

            UpdateFilteredData(null);

            SetSerieData(WinningsChartCollection, ChartSerieResourceHelper.GetSerieGreenPalette(), WinningsChartDisplayRange);

            //SetSerieData(SecondChartCollection, ChartSerieResourceHelper.GetSerieOrangePalette(), SecondChartDisplayRange);
        }

        private void InitializeCharts()
        {
            if (WinningsChartCollection == null)
            {
                WinningsChartCollection = new ObservableCollection<ChartSeries>();
            }
            else
            {
                WinningsChartCollection.Clear();
            }

            var resource = ChartSerieResourceHelper.GetSerieGreenPalette();

            WinningsChartCollection.Add(new ChartSeries
            {
                ChartSeriesType = ChartSeriesType.Winnings,
                LineColor = resource.LineColor,
                AreaStyle = resource.AreaBrush,
                Format = "{0:0.##}$",
                UpdateChartSeriesItem = (current, previous, stat) =>
                {
                    if (current == null)
                    {
                        return;
                    }

                    if (previous != null)
                    {
                        current.Value = previous.Value;
                    }

                    current.Value += stat.NetWon;
                }
            });

            //WinningsChartCollection.Add(new ChartSeries
            //{
            //    ChartSeriesType = ChartSeriesType.WinningsNonShowdown,
            //    LineColor = Colors.Red,
            //    Format = "{0:0.##}",
            //    AreaStyle = resource.AreaBrush,
            //    UpdateChartSeriesItem = (current, previous, stat) =>
            //    {
            //        if (current == null)
            //        {
            //            return;
            //        }

            //        if (previous != null)
            //        {
            //            current.Value = previous.Value;
            //        }

            //        if (stat.Sawshowdown == 0)
            //        {
            //            current.Value += stat.NetWon;
            //        }
            //    }
            //});

            //WinningsChartCollection.Add(new ChartSeries
            //{
            //    ChartSeriesType = ChartSeriesType.WinningsShowdown,
            //    LineColor = Colors.Yellow,
            //    Format = "{0:0.##}",
            //    AreaStyle = resource.AreaBrush,
            //    UpdateChartSeriesItem = (current, previous, stat) =>
            //    {
            //        if (current == null)
            //        {
            //            return;
            //        }

            //        if (previous != null)
            //        {
            //            current.Value = previous.Value;
            //        }

            //        if (stat.Sawshowdown == 1)
            //        {
            //            current.Value += stat.NetWon;
            //        }
            //    }
            //});

            //WinningsChartCollection.Add(new ChartSeries
            //{
            //    ChartSeriesType = ChartSeriesType.WinningsInBB,
            //    LineColor = resource.LineColor,
            //    AreaStyle = resource.AreaBrush
            //});

            //WinningsChartCollection.Add(new ChartSeries
            //{
            //    ChartSeriesType = ChartSeriesType.WinningsInBBNonShowdown,
            //    LineColor = Colors.Red,
            //    AreaStyle = resource.AreaBrush
            //});

            //WinningsChartCollection.Add(new ChartSeries
            //{
            //    ChartSeriesType = ChartSeriesType.WinningsInBBShowdown,
            //    LineColor = Colors.Yellow,
            //    AreaStyle = resource.AreaBrush
            //});
        }

        private static CharItemDataBuilder CreateCharItemDataBuilder(ChartDisplayRange displayRange)
        {
            switch (displayRange)
            {
                case ChartDisplayRange.Hands:
                    return new HandsItemDataBuilder();
                case ChartDisplayRange.Month:
                    return new MonthItemDataBuilder();
                case ChartDisplayRange.Week:
                    return new WeekItemDataBuilder();
                case ChartDisplayRange.Year:
                    return new YearItemDataBuilder();
                default:
                    throw new ArgumentException("Unknown char display range type");
            }
        }
    }
}