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

using DriveHUD.Application.ViewModels.Graphs;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Reports;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class DashboardViewModel : BaseViewModel, IMainTabViewModel
    {
        private readonly IReportStatusService reportStatusService;

        #region Properties

        public InteractionRequest<INotification> ShowMoneyWonGraphPopupRequest { get; private set; }

        private bool updateIsRequired = true;

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

        private GraphViewModel moneyWonGraphViewModel;

        public GraphViewModel MoneyWonGraphViewModel
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

        private GraphViewModel bb100GraphViewModel;

        public GraphViewModel BB100GraphViewModel
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

        private bool UpdateIsRequired
        {
            get
            {
                return updateIsRequired || reportStatusService.CashUpdated;
            }
        }

        public ICommand ShowMoneyWonGraphPopupCommand { get; private set; }

        public EnumViewModelType ViewModelType => EnumViewModelType.DashboardViewModel;

        #endregion

        internal DashboardViewModel()
        {
            reportStatusService = ServiceLocator.Current.GetInstance<IReportStatusService>();

            ServiceLocator.Current
                .GetInstance<IEventAggregator>()
                .GetEvent<BuiltFilterChangedEvent>()
                .Subscribe(e =>
                {
                    if (e.AffectedFilter.Contains(EnumFilterType.Cash))
                    {
                        updateIsRequired = true;
                        Update();
                    }
                });

            ShowMoneyWonGraphPopupRequest = new InteractionRequest<INotification>();
            ShowMoneyWonGraphPopupCommand = new RelayCommand(() =>
            {
                var cashGraphPopupViewModelInfo = new CashGraphPopupViewModelInfo
                {
                    MoneyWonCashGraphViewModel = MoneyWonGraphViewModel.ViewModel as CashGraphViewModel
                };

                var cashGraphPopupRequestInfo = new CashGraphPopupRequestInfo(cashGraphPopupViewModelInfo);

                ShowMoneyWonGraphPopupRequest.Raise(cashGraphPopupRequestInfo);
            });

            InitializeCharts();
        }

        internal void Update()
        {
            if (!IsActive || !UpdateIsRequired)
            {
                return;
            }

            InternalUpdate();
        }

        private void InternalUpdate()
        {
            if (StorageModel.StatisticCollection == null)
            {
                return;
            }

            UpdateFilteredData();            

            MoneyWonGraphViewModel?.Update();
            BB100GraphViewModel?.Update();

            updateIsRequired = false;
            reportStatusService.CashUpdated = false;
        }

        private void UpdateFilteredData()
        {
            if (IndicatorCollection == null)
            {
                IndicatorCollection = new DashboardIndicators();
            }
            else
            {
                IndicatorCollection.Clean();
            }

            if (StorageModel.FilteredCashPlayerStatistic == null)
            {
                return;
            }

            var statistics = StorageModel.FilteredCashPlayerStatistic.ToList();

            IndicatorCollection.UpdateSource(statistics);

            RaisePropertyChanged(nameof(IndicatorCollection));
        }

        private void InitializeCharts()
        {
            var cashGraphSettingsService = ServiceLocator.Current.GetInstance<ICashGraphSettingsService>();

            var moneyWonChartSeries = ChartSeriesProvider.CreateMoneyWonChartSeries();
            MoneyWonGraphViewModel = new GraphViewModel(new CashGraphViewModel(moneyWonChartSeries, cashGraphSettingsService.GetSettings(CashChartType.MoneyWon)));

            var bb100ChartSeries = ChartSeriesProvider.CreateBB100ChartSeries();
            BB100GraphViewModel = new GraphViewModel(new CashGraphViewModel(bb100ChartSeries, cashGraphSettingsService.GetSettings(CashChartType.BB100)));
        }
    }
}