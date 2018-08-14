//-----------------------------------------------------------------------
// <copyright file="TournamentGraphViewModel.cs" company="Ace Poker Solutions">
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
    public class TournamentGraphViewModel : BaseGraphViewModel<TournamentChartSeries>
    {
        public TournamentGraphViewModel(IEnumerable<TournamentChartSeries> chartSeries) : base(chartSeries)
        {
            chartDisplayRange = ChartDisplayRange.Year;
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

                RaisePropertyChanged();
            }
        }

        public override void Update()
        {
            var chartItemDataBuilder = CreateChartItemDataBuilder(ChartDisplayRange);

            var tournaments = chartItemDataBuilder.Create();

            var chartSeriesItems = new Dictionary<TournamentChartSeries, List<ChartSeriesItem>>();

            foreach (var tournament in tournaments)
            {
                foreach (var chartSerie in ChartCollection)
                {
                    ChartSeriesItem previousChartSeriesItem = null;
                    ChartSeriesItem chartSeriesItem = null;

                    if (!chartSeriesItems.ContainsKey(chartSerie))
                    {
                        chartSeriesItems.Add(chartSerie, new List<ChartSeriesItem>());
                    }

                    chartSeriesItem = new ChartSeriesItem
                    {
                        Format = chartSerie.Format,
                        Category = tournament.Started,
                        PointColor = chartSerie.ColorsPalette.PointColor,
                        TrackBallColor = chartSerie.ColorsPalette.TrackBallColor,
                        TooltipColor = chartSerie.ColorsPalette.TooltipColor,
                        TooltipForegroundColor = chartSerie.ColorsPalette.TooltipForeground
                    };

                    previousChartSeriesItem = chartSeriesItems[chartSerie].LastOrDefault();
                    chartSeriesItems[chartSerie].Add(chartSeriesItem);

                    chartSerie.UpdateChartSeriesItem(chartSeriesItem, previousChartSeriesItem, tournament);
                }
            }

            App.Current.Dispatcher.Invoke(() => ChartCollection.ForEach(x => x.ItemsCollection.Clear()));

            if (chartSeriesItems.Count > 0)
            {
                chartSeriesItems.Keys.ForEach(charSerie =>
                {
                    charSerie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(chartSeriesItems[charSerie]);
                });
            }
        }

        private ITournamentChartData CreateChartItemDataBuilder(ChartDisplayRange displayRange)
        {
            switch (displayRange)
            {
                case ChartDisplayRange.Month:
                    return new MonthTournamentChartData();
                case ChartDisplayRange.Week:
                    return new WeekTournamentChartData();
                case ChartDisplayRange.Year:
                    return new YearTournamentChartData();
                default:
                    throw new ArgumentException("Unknown chart display range type");
            }
        }
    }
}