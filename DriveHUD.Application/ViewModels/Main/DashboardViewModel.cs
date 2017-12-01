﻿//-----------------------------------------------------------------------
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

using DriveHUD.Application.ViewModels.Graphs;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Enums;
using Model.Events;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        #region Properties

        public InteractionRequest<INotification> ShowMoneyWonGraphPopupRequest { get; private set; }

        private bool isExpanded = true;

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

        private CashGraphViewModel moneyWonGraphViewModel;

        public CashGraphViewModel MoneyWonGraphViewModel
        {
            get
            {
                return moneyWonGraphViewModel;
            }
            private set
            {
                SetProperty(ref moneyWonGraphViewModel, value);
            }
        }

        private CashGraphViewModel bb100GraphViewModel;

        public CashGraphViewModel BB100GraphViewModel
        {
            get
            {
                return bb100GraphViewModel;
            }
            private set
            {
                SetProperty(ref bb100GraphViewModel, value);
            }
        }

        private LightIndicators indicatorCollection;

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

        public ICommand ShowMoneyWonGraphPopupCommand { get; private set; }

        #endregion

        internal DashboardViewModel()
        {
            ServiceLocator.Current
                .GetInstance<IEventAggregator>()
                .GetEvent<BuiltFilterChangedEvent>()
                .Subscribe(UpdateFilteredData);

            ShowMoneyWonGraphPopupRequest = new InteractionRequest<INotification>();
            ShowMoneyWonGraphPopupCommand = new RelayCommand(() =>
            {
                var cashGraphPopupViewModelInfo = new CashGraphPopupViewModelInfo
                {
                    MoneyWonCashGraphViewModel = MoneyWonGraphViewModel
                };

                var cashGraphPopupRequestInfo = new CashGraphPopupRequestInfo(cashGraphPopupViewModelInfo);

                ShowMoneyWonGraphPopupRequest.Raise(cashGraphPopupRequestInfo);
            });

            InitializeWinningsChart();
            InitializeBB100Chart();
        }

        internal void UpdateFilteredData(BuiltFilterChangedEventArgs args)
        {
            if (!IsActive)
            {
                return;
            }

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

            MoneyWonGraphViewModel?.Update();
            BB100GraphViewModel?.Update();
        }

        private void InitializeWinningsChart()
        {
            var moneyWonChartSeries = ChartSeriesProvider.CreateMoneyWonChartSeries();
            MoneyWonGraphViewModel = new CashGraphViewModel(moneyWonChartSeries);
        }

        private void InitializeBB100Chart()
        {
            var bb100ChartSeries = ChartSeriesProvider.CreateBB100ChartSeries();
            BB100GraphViewModel = new CashGraphViewModel(bb100ChartSeries, ChartDisplayRange.Month);
        }
    }
}