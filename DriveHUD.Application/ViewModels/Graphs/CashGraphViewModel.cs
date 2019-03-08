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
    public class CashGraphViewModel : BaseGraphViewModel<ChartSeries>
    {
        private readonly CashGraphSettings settings;

        public CashGraphViewModel(IEnumerable<ChartSeries> chartSeries) : this(chartSeries, new CashGraphSettings())
        {
        }

        public CashGraphViewModel(IEnumerable<ChartSeries> chartSeries, CashGraphSettings cashGraphSettings) : base(chartSeries)
        {
            settings = cashGraphSettings;
        }

        public ChartDisplayRange ChartDisplayRange
        {
            get
            {
                return settings.ChartDisplayRange;
            }
            set
            {
                if (settings.ChartDisplayRange == value)
                {
                    return;
                }

                settings.ChartDisplayRange = value;
                Update();

                RaisePropertyChanged();
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

        public bool ShowShowdown
        {
            get
            {
                return settings.ShowShowdown;
            }
            set
            {
                if (settings.ShowShowdown == value)
                {
                    return;
                }

                settings.ShowShowdown = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowNonShowdown
        {
            get
            {
                return settings.ShowNonShowdown;
            }
            set
            {
                if (settings.ShowNonShowdown == value)
                {
                    return;
                }

                settings.ShowNonShowdown = value;
                RaisePropertyChanged();
            }
        }

        public bool ShowEV
        {
            get
            {
                return settings.ShowEV;
            }
            set
            {
                if (settings.ShowEV == value)
                {
                    return;
                }

                settings.ShowEV = value;
                RaisePropertyChanged();
            }
        }

        public ChartCashSeriesValueType ValueType
        {
            get
            {
                return settings.ValueType;
            }
            set
            {
                if (settings.ValueType == value)
                {
                    return;
                }

                settings.ValueType = value;
                RaisePropertyChanged();
            }
        }

        public override void Update()
        {
            var chartItemDataBuilder = CreateChartItemDataBuilder(ChartDisplayRange);

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

                foreach (var chartSerie in ChartCollection)
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
                    
                    chartSerie.UpdateChartSeriesItem(chartSeriesItem, previousChartSeriesItem, stat, statIndex, stats.Length);
                }
            }

            if (ChartDisplayRange == ChartDisplayRange.Hands)
            {
                HandsCount = itemsCounter;
            }

            App.Current.Dispatcher.Invoke(() => ChartCollection?.ForEach(x => x.ItemsCollection.Clear()));

            if (chartSeriesItems.Count > 0)
            {
                chartSeriesItems.Keys.ForEach(charSerie =>
                {
                    charSerie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(chartSeriesItems[charSerie]);
                });
            }
        }

        private static CharItemDataBuilder CreateChartItemDataBuilder(ChartDisplayRange displayRange)
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