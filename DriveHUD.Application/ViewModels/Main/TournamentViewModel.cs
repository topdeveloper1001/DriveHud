//-----------------------------------------------------------------------
// <copyright file="TournamentViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Interfaces;
using Model.Reports;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class TournamentViewModel : BaseViewModel, IMainTabViewModel
    {
        private readonly IReportStatusService reportStatusService;

        #region Fields

        private bool showLabels = true;
        private bool isShowLabelsEnabled = true;
        private ObservableCollection<ChartSeries> chartSeriesCollection;                
        private Bracelet goldenBracelet;
        private Bracelet silverBracelet;
        private Bracelet bronzeBracelet;

        private int totalMTT;
        private int totalSTT;
        private decimal sttWon;
        private decimal mttWon;

        private bool isExpanded = true;

        private readonly IEventAggregator eventAggregator;
        private readonly IFilterModelManagerService filterModelManagerService;

        private bool updateIsRequired = true;

        #endregion

        internal TournamentViewModel()
        {
            reportStatusService = ServiceLocator.Current.GetInstance<IReportStatusService>();

            InitializeChartSeries();

            GoldenBracelet = new Bracelet { PlaceNumber = 1 };
            SilverBracelet = new Bracelet { PlaceNumber = 2 };
            BronzeBracelet = new Bracelet { PlaceNumber = 3 };

            BraceletTournamentClickCommand = new RelayCommand(BraceletTournamentClick);

            filterModelManagerService = ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterServices.Main.ToString());

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<TournamentDataUpdatedEvent>().Subscribe(x => Update());
            eventAggregator.GetEvent<BuiltFilterChangedEvent>()
                .Subscribe(e =>
                {
                    if (e.AffectedFilter.Contains(EnumFilterType.Tournament))
                    {
                        updateIsRequired = true;
                        Update();
                    }
                });
        }

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

        public bool ShowLabels
        {
            get
            {
                return showLabels;
            }
            set
            {
                SetProperty(ref showLabels, value);
            }
        }

        public bool IsShowLabelsEnabled
        {
            get
            {
                return isShowLabelsEnabled;
            }
            set
            {
                SetProperty(ref isShowLabelsEnabled, value);
            }
        }

        public ObservableCollection<ChartSeries> ChartSeriesCollection
        {
            get
            {
                return chartSeriesCollection;
            }
            set
            {
                SetProperty(ref chartSeriesCollection, value);
            }
        }

        public Bracelet GoldenBracelet
        {
            get
            {
                return goldenBracelet;
            }
            private set
            {
                SetProperty(ref goldenBracelet, value);
            }
        }

        public Bracelet SilverBracelet
        {
            get
            {
                return silverBracelet;
            }
            private set
            {
                SetProperty(ref silverBracelet, value);
            }
        }

        public Bracelet BronzeBracelet
        {
            get
            {
                return bronzeBracelet;
            }
            private set
            {
                SetProperty(ref bronzeBracelet, value);
            }
        }

        public int TotalMTT
        {
            get
            {
                return totalMTT;
            }
            private set
            {
                SetProperty(ref totalMTT, value);
            }
        }

        public int TotalSTT
        {
            get
            {
                return totalSTT;
            }
            private set
            {
                SetProperty(ref totalSTT, value);
            }
        }

        public decimal MTTWon
        {
            get
            {
                return mttWon;
            }
            private set
            {
                SetProperty(ref mttWon, value);
            }
        }

        public decimal STTWon
        {
            get
            {
                return sttWon;
            }
            private set
            {
                SetProperty(ref sttWon, value);
            }
        }

        private GraphViewModel tournamentGraphViewModel;

        public GraphViewModel TournamentGraphViewModel
        {
            get
            {
                return tournamentGraphViewModel;
            }
            set
            {
                SetProperty(ref tournamentGraphViewModel, value);
            }
        }

        private bool UpdateIsRequired
        {
            get
            {
                return updateIsRequired || reportStatusService.TournamentUpdated;
            }
        }

        public EnumViewModelType ViewModelType => EnumViewModelType.TournamentViewModel;

        #endregion

        #region Commands

        public ICommand BraceletTournamentClickCommand { get; private set; }

        #endregion

        internal void Update()
        {
            if (!IsActive || !UpdateIsRequired)
            {
                return;
            }

            InternalUpdate();
        }

        internal void InternalUpdate()
        {
            if (StorageModel.StatisticCollection == null)
            {
                return;
            }

            TournamentGraphViewModel?.Update();

            var playerTournaments = StorageModel.PlayerSelectedItem != null ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(StorageModel.PlayerSelectedItem.PlayerIds) :
                new List<Tournaments>();

            MTTWon = 0;
            STTWon = 0;
            TotalMTT = 0;
            TotalSTT = 0;

            var dateFilter = filterModelManagerService.FilterModelCollection?.OfType<FilterDateModel>().FirstOrDefault();

            var filteredTournaments = dateFilter != null ? dateFilter.FilterTournaments(playerTournaments) : playerTournaments;

            foreach (var tournament in filteredTournaments)
            {
                if (!Enum.TryParse(tournament.Tourneytagscsv, out TournamentsTags tag))
                {
                    continue;
                }

                switch (tag)
                {
                    case TournamentsTags.MTT:
                        MTTWon += (tournament.Winningsincents - tournament.Buyinincents - tournament.Rakeincents) / 100m;
                        TotalMTT++;
                        break;
                    case TournamentsTags.STT:
                        STTWon += (tournament.Winningsincents - tournament.Buyinincents - tournament.Rakeincents) / 100m;
                        TotalSTT++;
                        break;
                }
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                SetBraceletData(GoldenBracelet, filteredTournaments);
                SetBraceletData(SilverBracelet, filteredTournaments);
                SetBraceletData(BronzeBracelet, filteredTournaments);
            });

            updateIsRequired = false;
            reportStatusService.TournamentUpdated = false;
        }

        private void InitializeChartSeries()
        {
            var tournamentChartSeries = ChartSeriesProvider.CreateTournamentChartSeries();
            TournamentGraphViewModel = new GraphViewModel(new TournamentGraphViewModel(tournamentChartSeries));          
        }

        private void SetBraceletData(Bracelet bracelet, IEnumerable<Tournaments> playerTournaments)
        {
            bracelet.NumberOfWins = playerTournaments.Where(x => x.Finishposition == bracelet.PlaceNumber).Count();
            bracelet.BraceletItems.Clear();
            foreach (var item in playerTournaments.Where(x => x.Finishposition == bracelet.PlaceNumber))
            {
                HandHistories.Objects.GameDescription.Limit limit =
                    HandHistories.Objects.GameDescription.Limit.FromSmallBlindBigBlind(0, 0, HandHistories.Objects.GameDescription.Currency.USD);//(HandHistories.Objects.GameDescription.Currency)item.CurrencyId);
                bracelet.BraceletItems.Add(new BraceletItem()
                {
                    Id = item.Tourneynumber,
                    AmountString = String.Format("{0} {1:0.##}", limit.GetCurrencySymbol(), item.Winningsincents / 100m)
                });
            }
        }

        #region ICommand Implementation

        private void BraceletTournamentClick(object obj)
        {
            var item = obj as BraceletItem;

            if (item == null)
            {
                return;
            }

            eventAggregator.GetEvent<RequestDisplayTournamentHands>().Publish(new RequestDisplayTournamentHandsEvent(item.Id));
        }

        #endregion
    }
}