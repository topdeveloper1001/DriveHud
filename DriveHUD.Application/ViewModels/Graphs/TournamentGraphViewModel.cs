//-----------------------------------------------------------------------
// <copyright file="TournamentGraphViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.ChartData;
using Model.Enums;
using Model.Interfaces;
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
            seriesValueType = ChartTournamentSeriesValueType.Currency;

            tournamentChartFilterTypes = new ObservableCollection<TournamentChartFilterType>
            {
                TournamentChartFilterType.All,
                TournamentChartFilterType.STT,
                TournamentChartFilterType.MTT
            };

            seriesValueTypes = new ObservableCollection<ChartTournamentSeriesValueType>
            {
                 ChartTournamentSeriesValueType.Currency,
                 ChartTournamentSeriesValueType.Chips,
                 ChartTournamentSeriesValueType.BB
            };
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

        private TournamentChartFilterType tournamentChartFilterType;

        public TournamentChartFilterType TournamentChartFilterType
        {
            get
            {
                return tournamentChartFilterType;
            }
            set
            {
                SetProperty(ref tournamentChartFilterType, value);
                Update();
            }
        }

        private ObservableCollection<TournamentChartFilterType> tournamentChartFilterTypes;

        public ObservableCollection<TournamentChartFilterType> TournamentChartFilterTypes
        {
            get
            {
                return tournamentChartFilterTypes;
            }
        }

        private ChartTournamentSeriesValueType seriesValueType;

        public ChartTournamentSeriesValueType SeriesValueType
        {
            get
            {
                return seriesValueType;
            }
            set
            {
                SetProperty(ref seriesValueType, value);
            }
        }

        private ObservableCollection<ChartTournamentSeriesValueType> seriesValueTypes;

        public ObservableCollection<ChartTournamentSeriesValueType> SeriesValueTypes
        {
            get
            {
                return seriesValueTypes;
            }
        }

        public override void Update()
        {
            var player = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var tournaments = ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player?.PlayerIds);

            var tournamentsOfStatistic = new HashSet<TournamentKey>(StorageModel
                .FilteredTournamentPlayerStatistic
                .ToArray()
                .Select(x => new TournamentKey(x.PokersiteId, x.TournamentId))
                .Distinct());

            var filteredTournaments = tournaments.Where(x => tournamentsOfStatistic.Contains(x.BuildKey())).ToList();

            var chartSeriesItems = new Dictionary<TournamentChartSeries, List<ChartSeriesItem>>();

            UpdateBasedOnTournamentsSeries(chartSeriesItems, filteredTournaments);
            UpdateBasedOnStatisticSeries(chartSeriesItems, filteredTournaments);

            App.Current.Dispatcher.Invoke(() => ChartCollection.ForEach(x => x.ItemsCollection.Clear()));

            if (chartSeriesItems.Count > 0)
            {
                chartSeriesItems.Keys.ForEach(charSerie =>
                {
                    charSerie.ItemsCollection = new ObservableCollection<ChartSeriesItem>(chartSeriesItems[charSerie]);
                });
            }
        }

        private void UpdateBasedOnTournamentsSeries(Dictionary<TournamentChartSeries, List<ChartSeriesItem>> chartSeriesItems, IList<Tournaments> tournaments)
        {
            var chartItemDataBuilder = CreateTournamentChartItemDataBuilder(ChartDisplayRange);

            var tournamentRecords = chartItemDataBuilder.Create(tournaments, TournamentChartFilterType);

            foreach (var tournament in tournamentRecords)
            {
                foreach (var chartSerie in ChartCollection.Where(x => !x.IsBasedOnStatistic))
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

                    chartSerie.UpdateChartSeriesItemByTournament?.Invoke(chartSeriesItem, previousChartSeriesItem, tournament);
                }
            }
        }

        private void UpdateBasedOnStatisticSeries(Dictionary<TournamentChartSeries, List<ChartSeriesItem>> chartSeriesItems, IList<Tournaments> tournaments)
        {
            if (tournaments == null || tournaments.Count == 0)
            {
                return;
            }

            var chartItemDataBuilder = CreateChartItemDataBuilder(ChartDisplayRange);
            var tournamentChartItemDataBuilder = CreateTournamentChartItemDataBuilder(ChartDisplayRange);

            var firstDate = tournamentChartItemDataBuilder.GetFirstDate(tournaments.Max(x => x.Firsthandtimestamp));

            var groupedTournaments = tournaments
                .Where(x => x.Firsthandtimestamp >= firstDate && (tournamentChartFilterType == TournamentChartFilterType.All ||
                        tournamentChartFilterType == TournamentChartFilterType.MTT && x.Tourneytagscsv == TournamentsTags.MTT.ToString() ||
                        tournamentChartFilterType == TournamentChartFilterType.STT && x.Tourneytagscsv == TournamentsTags.STT.ToString()))
                .GroupBy(x => x.BuildKey()).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var filteredTournamentPlayerStatistic = StorageModel
                .FilteredTournamentPlayerStatistic
                .Where(x => groupedTournaments.ContainsKey(new TournamentKey(x.PokersiteId, x.TournamentId)))
                .ToArray();

            // filter and orders
            var stats = chartItemDataBuilder.PrepareStatistic(filteredTournamentPlayerStatistic);

            object previousGroupKey = null;

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

                foreach (var chartSerie in ChartCollection.Where(x => x.IsBasedOnStatistic))
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
                            Category = chartItemDataBuilder.GetValueFromGroupKey(currentGroupKey),
                            PointColor = chartSerie.ColorsPalette.PointColor,
                            TrackBallColor = chartSerie.ColorsPalette.TrackBallColor,
                            TooltipColor = chartSerie.ColorsPalette.TooltipColor,
                            TooltipForegroundColor = chartSerie.ColorsPalette.TooltipForeground
                        };

                        previousChartSeriesItem = chartSeriesItems[chartSerie].LastOrDefault();
                        chartSeriesItems[chartSerie].Add(chartSeriesItem);
                    }
                    else
                    {
                        previousChartSeriesItem = chartSeriesItem = chartSeriesItems[chartSerie].LastOrDefault();
                    }

                    chartSerie.UpdateChartSeriesItemByStatistic?.Invoke(chartSeriesItem, previousChartSeriesItem, stat);
                }
            }
        }

        private static ITournamentChartData CreateTournamentChartItemDataBuilder(ChartDisplayRange displayRange)
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

        private static CharItemDataBuilder CreateChartItemDataBuilder(ChartDisplayRange displayRange)
        {
            switch (displayRange)
            {
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