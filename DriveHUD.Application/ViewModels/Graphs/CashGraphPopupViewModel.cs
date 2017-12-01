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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            }
        }

        public SingletonStorageModel StorageModel
        {
            get { return ServiceLocator.Current.TryResolve<SingletonStorageModel>(); }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> CloseCommand { get; private set; }

        #endregion

        private void Initialize()
        {
            InitializeCommands();
            OnInitialized();
        }

        private void InitializeCommands()
        {
            CloseCommand = ReactiveCommand.Create();
            CloseCommand.Subscribe(x => OnClosed());
        }

        #region Data initializers

        private void InitializeData(CashGraphPopupViewModelInfo viewModelInfo)
        {
            InitializeMainChart(viewModelInfo);
            InitializeExtraCharts();
        }

        private void InitializeMainChart(CashGraphPopupViewModelInfo viewModelInfo)
        {
            var moneyWonChartSeries = ChartSeriesProvider.CreateMoneyWonChartSeries();

            MainGraphViewModel = new CashGraphViewModel(moneyWonChartSeries);

            if (viewModelInfo.MoneyWonCashGraphViewModel != null)
            {
                MainGraphViewModel.ShowNonShowdown = viewModelInfo.MoneyWonCashGraphViewModel.ShowNonShowdown;
                MainGraphViewModel.ShowShowdown = viewModelInfo.MoneyWonCashGraphViewModel.ShowShowdown;
                MainGraphViewModel.ChartDisplayRange = viewModelInfo.MoneyWonCashGraphViewModel.ChartDisplayRange;
            }

            MainGraphViewModel.Update();
        }

        private void InitializeExtraCharts()
        {
            var seriesTypes = GetSeriesTypesToInitialize();

            var seriesProvider = ServiceLocator.Current.GetInstance<IGraphSeriesProvider>();
            seriesProvider.Initialize(seriesTypes);

            var statisticCollection = StorageModel.StatisticCollection.ToArray();

            foreach (var statistic in statisticCollection)
            {
                seriesProvider.Process(statistic);
            }

            seriesDictionary = seriesProvider.GetSeries();
        }

        private SerieType[] GetSeriesTypesToInitialize()
        {
            return Enum.GetValues(typeof(SerieType)).Cast<SerieType>().ToArray();
        }

        #endregion
    }
}