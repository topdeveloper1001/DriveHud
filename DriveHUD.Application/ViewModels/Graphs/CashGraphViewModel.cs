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

namespace DriveHUD.Application.ViewModels.Graphs
{
    public class CashGraphViewModel : BaseViewModel
    {
        public CashGraphViewModel(IEnumerable<ChartSeries> chartSeries, ChartDisplayRange defaultDisplayRange = ChartDisplayRange.Hands)
        {
            chartCollection = new ObservableCollection<ChartSeries>(chartSeries);
            chartDisplayRange = defaultDisplayRange;
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

        private ChartDisplayRange chartDisplayRange;

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
            set
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
            set
            {
                SetProperty(ref showNonShowdown, value);
            }
        }

        private bool showEv;

        public bool ShowEV
        {
            get
            {
                return showEv;
            }
            set
            {
                SetProperty(ref showEv, value);
            }
        }

        internal void Update()
        {
            var chartItemDataBuilder = CreateCharItemDataBuilder(ChartDisplayRange);

            // filter and orders
            var stats = chartItemDataBuilder.PrepareStatistic(StorageModel.FilteredCashPlayerStatistic);

            object previousGroupKey = null;

            chartItemDataBuilder.Prepare(stats.Length);

            var chartSeriesItems = new Dictionary<ChartSeries, List<ChartSeriesItem>>();

            var itemsCounter = 0;

            for (var statIndex = 0; statIndex < stats.Length; statIndex++)
            {
                var stat = stats[statIndex];

                var currentGroupKey = chartItemDataBuilder.BuildGroupKey(stat, statIndex);

                var isNew = !currentGroupKey.Equals(previousGroupKey);

                if (isNew)
                {
                    itemsCounter++;
                }

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

            if (ChartDisplayRange == ChartDisplayRange.Hands)
            {
                HandsCount = itemsCounter;
            }

            if (chartSeriesItems.Count > 0)
            {
                chartSeriesItems.Keys.ForEach(charSerie =>
                {
                    charSerie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(chartSeriesItems[charSerie]);
                });
            }
            else
            {
                chartCollection.ForEach(x => x.ItemsCollection.Clear());
            }
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