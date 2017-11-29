//-----------------------------------------------------------------------
// <copyright file="CashGraphViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.ViewModels;
using Model.ChartData;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Main
{
    public class CashGraphViewModel : BaseViewModel
    {
        public CashGraphViewModel(IEnumerable<ChartSeries> chartSeries)
        {
            chartCollection = new ObservableCollection<ChartSeries>(chartSeries);
        }

        private ObservableCollection<ChartSeries> chartCollection;

        public ObservableCollection<ChartSeries> ChartCollection
        {
            get
            {
                return chartCollection;
            }
            set
            {
                SetProperty(ref chartCollection, value);
            }
        }

        private ChartDisplayRange chartDisplayRange = ChartDisplayRange.Hands;

        public ChartDisplayRange ChartDisplayRange
        {
            get
            {
                return chartDisplayRange;
            }
            set
            {
                if (chartDisplayRange == value)
                {
                    return;
                }

                chartDisplayRange = value;
                Update();

                OnPropertyChanged();
            }
        }

        private int handsCount;

        public int HandsCount
        {
            get
            {
                return handsCount;
            }
            private set
            {
                SetProperty(ref handsCount, value);
            }
        }

        private bool showShowdown;

        public bool ShowShowdown
        {
            get
            {
                return showShowdown;
            }
            private set
            {
                SetProperty(ref showShowdown, value);
            }
        }


        private bool showNonShowdown;

        public bool ShowNonShowdown
        {
            get
            {
                return showNonShowdown;
            }
            private set
            {
                SetProperty(ref showNonShowdown, value);
            }
        }

        internal void Update()
        {
            // filter stats if necessary (only for date ranges)
            // sort stats by date asc
            // loop stats
            //      build key from (stat, range, stat index)
            //      loop chart series
            //          if key equals to previous then update current item with Action<current, stat, previous> or Func<decimal, stat, previous>
            //          if key not equals to previous the create new item and update it with Action<current, stat, previous> or Func<decimal, stat, previous>
            // i=50, i=51, ... bb/100(50)= sum(nw1/bb1+...)/50, bb/100(51) = (bb/100(50)*50+nw51/bb51)/51 ...

            var chartItemDataBuilder = CreateCharItemDataBuilder(ChartDisplayRange);

            // filter and orders
            var stats = chartItemDataBuilder.PrepareStatistic(StorageModel.StatisticCollection.ToList().Where(x => !x.IsTourney));

            object previousGroupKey = null;

            if (ChartDisplayRange == ChartDisplayRange.Hands)
            {
                HandsCount = stats.Length;
            }           

            var chartSeriesItems = new Dictionary<ChartSeries, List<ChartSeriesItem>>();

            for (var statIndex = 0; statIndex < stats.Length; statIndex++)
            {
                var stat = stats[statIndex];

                var currentGroupKey = chartItemDataBuilder.BuildGroupKey(stat, statIndex);

                var isNew = !currentGroupKey.Equals(previousGroupKey);

                previousGroupKey = currentGroupKey;

                foreach (var chartSerie in chartCollection)
                {
                    ChartSeriesItem previousChartSeriesItem = null;
                    ChartSeriesItem chartSeriesItem = null;

                    if (!chartSeriesItems.ContainsKey(chartSerie))
                    {
                        chartSeriesItems.Add(chartSerie, new List<ChartSeriesItem>());
                    }

                    if (isNew)
                    {
                        chartSeriesItem = new ChartSeriesItem
                        {
                            Format = chartSerie.Format,
                            PointColor = chartSerie.ColorsPalette.PointColor,
                            TrackBallColor = chartSerie.ColorsPalette.TrackBallColor,
                            TooltipColor = chartSerie.ColorsPalette.TooltipColor,
                            Category = chartItemDataBuilder.GetValueFromGroupKey(currentGroupKey)
                        };

                        previousChartSeriesItem = chartSeriesItems[chartSerie].LastOrDefault();
                        chartSeriesItems[chartSerie].Add(chartSeriesItem);
                    }
                    else
                    {
                        previousChartSeriesItem = chartSeriesItem = chartSeriesItems[chartSerie].LastOrDefault();
                    }

                    chartSerie.UpdateChartSeriesItem(chartSeriesItem, previousChartSeriesItem, stat, statIndex);
                }
            }

            chartSeriesItems.Keys.ForEach(charSerie =>
            {
                charSerie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(chartSeriesItems[charSerie]);
            });
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