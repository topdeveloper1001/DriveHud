//-----------------------------------------------------------------------
// <copyright file="CashGraphPopupViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Graphs
{
    public class CashGraphPopupViewModel : PopupWindowViewModel, ICashGraphPopupViewModel
    {
        private Dictionary<SerieType, IEnumerable<GraphSerie>> seriesDictionary;

        public override void Configure(object viewModelInfo)
        {
            var popupViewModelInfo = viewModelInfo as CashGraphPopupViewModelInfo;

            if (popupViewModelInfo == null)
            {
                return;
            }

            StartAsyncOperation(
                () => InitializeData(popupViewModelInfo),
                () => Initialize());
        }

        #region Properties

        private CashGraphViewModel mainGraphViewModel;

        public CashGraphViewModel MainGraphViewModel
        {
            get
            {
                return mainGraphViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref mainGraphViewModel, value);
            }
        }

        private SerieType barMetric;

        public SerieType BarMetric
        {
            get
            {
                return barMetric;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref barMetric, value);

                if (seriesDictionary.ContainsKey(barMetric))
                {
                    BarSeries = new ObservableCollection<GraphSerie>(seriesDictionary[barMetric]);
                }
                else
                {
                    BarSeries = new ObservableCollection<GraphSerie>();
                }
            }
        }

        private ObservableCollection<SerieType> barMetricsCollection;

        public ObservableCollection<SerieType> BarMetricsCollection
        {
            get
            {
                return barMetricsCollection;
            }
        }

        private SerieType pieMetric;

        public SerieType PieMetric
        {
            get
            {
                return pieMetric;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref pieMetric, value);

                if (seriesDictionary.ContainsKey(pieMetric))
                {
                    PieSeries = new ObservableCollection<GraphSerieDataPoint>(seriesDictionary[pieMetric]
                        .SelectMany(x => x.DataPoints));
                }
                else
                {
                    PieSeries = new ObservableCollection<GraphSerieDataPoint>();
                }
            }
        }

        private ObservableCollection<SerieType> pieMetricsCollection;

        public ObservableCollection<SerieType> PieMetricsCollection
        {
            get
            {
                return pieMetricsCollection;
            }
        }

        public SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        private ObservableCollection<GraphSerie> barSeries;

        public ObservableCollection<GraphSerie> BarSeries
        {
            get
            {
                return barSeries;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref barSeries, value);
            }
        }

        private ObservableCollection<GraphSerieDataPoint> pieSeries;

        public ObservableCollection<GraphSerieDataPoint> PieSeries
        {
            get
            {
                return pieSeries;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref pieSeries, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> CloseCommand { get; private set; }

        #endregion

        private void Initialize()
        {
            InitializeMetricsCollections();
            InitializeCommands();
            OnInitialized();
        }

        private void InitializeCommands()
        {
            CloseCommand = ReactiveCommand.Create();
            CloseCommand.Subscribe(x => OnClosed());
        }

        private void InitializeMetricsCollections()
        {
            barMetricsCollection = new ObservableCollection<SerieType>
            {
                SerieType.WinningsByMonth,
                SerieType.WinningsByYear,
                SerieType.MoneyWonByCashGameType,
                SerieType.MoneyWonByTournamentGameType,
                SerieType.EVDiffToRealizedEVByMonth
            };

            pieMetricsCollection = new ObservableCollection<SerieType>
            {
                SerieType.Top20BiggestLosingHands,
                SerieType.Top20BiggestWinningHands,
                SerieType.MoneyWonByPosition,
                SerieType.BB100ByTimeOfDay,
                // SerieType.Top20ToughestOpponents
            };
        }

        #region Data initializers

        private void InitializeData(CashGraphPopupViewModelInfo viewModelInfo)
        {
            InitializeMainChart(viewModelInfo);
            InitializeExtraCharts();

            // initialize defaults
            BarMetric = SerieType.WinningsByMonth;
            PieMetric = SerieType.Top20BiggestLosingHands;
        }

        private void InitializeMainChart(CashGraphPopupViewModelInfo viewModelInfo)
        {
            var moneyWonChartSeries = ChartSeriesProvider.CreateMoneyWonChartSeries();

            MainGraphViewModel = new CashGraphViewModel(moneyWonChartSeries);

            if (viewModelInfo.MoneyWonCashGraphViewModel != null)
            {
                MainGraphViewModel.ShowNonShowdown = viewModelInfo.MoneyWonCashGraphViewModel.ShowNonShowdown;
                MainGraphViewModel.ShowShowdown = viewModelInfo.MoneyWonCashGraphViewModel.ShowShowdown;
                MainGraphViewModel.ShowEV = viewModelInfo.MoneyWonCashGraphViewModel.ShowEV;
                MainGraphViewModel.ChartDisplayRange = viewModelInfo.MoneyWonCashGraphViewModel.ChartDisplayRange;
            }

            MainGraphViewModel.Update();
        }

        private void InitializeExtraCharts()
        {
            var seriesTypes = GetSeriesTypesToInitialize();

            var seriesProvider = ServiceLocator.Current.GetInstance<IGraphsProvider>();
            seriesProvider.Initialize(seriesTypes);

            var statisticCollection = StorageModel.StatisticCollection.ToArray();

            foreach (var statistic in statisticCollection)
            {
                seriesProvider.Process(statistic);
            }

            seriesDictionary = seriesProvider.GetSeries();
        }

        private static SerieType[] GetSeriesTypesToInitialize()
        {
            return Enum.GetValues(typeof(SerieType))
                .Cast<SerieType>()
                .Where(x => x != SerieType.Top20ToughestOpponents)
                .ToArray();
        }

        #endregion
    }
}