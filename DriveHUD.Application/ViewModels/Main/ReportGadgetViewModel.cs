//-----------------------------------------------------------------------
// <copyright file="ReportGadgetViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.Views;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Data;
using Model.Enums;
using Model.Events;
using Model.Events.FilterEvents;
using Model.Extensions;
using Model.Filters;
using Model.Interfaces;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Report view model
    /// </summary>
    public class ReportGadgetViewModel : BaseViewModel
    {
        #region Private Fields
        private EnumReports _invisibleSelectedItemStat;
        private readonly IEventAggregator _eventAggregator;
        //private readonly ITopPlayersService _topPlayersService;
        #endregion

        #region ICommand
        public ICommand CalculateEquityCommand { get; set; }
        public ICommand ButtonFilterModelSectionRemove_CommandClick { get; set; }
        public ICommand MakeNoteCommand { get; set; }
        public ICommand DeleteHandCommand { get; set; }
        public ICommand EditTournamentCommand { get; set; }
        public ICommand ReportRadioButtonClickCommand { get; set; }
        #endregion

        #region Initialize

        internal ReportGadgetViewModel()
        {
            _eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            // TODO: Opponent Analysis report turned off
            //_topPlayersService = ServiceLocator.Current.GetInstance<ITopPlayersService>();
            this.PopupRequest = new InteractionRequest<PopupBaseNotification>();
            this.NotificationRequest = new InteractionRequest<INotification>();

            this.ReportCollection = new RangeObservableCollection<Indicators>();
            this.ReportSelectedItemStatisticsCollection_Filtered = new RangeObservableCollection<ComparableCardsStatistic>();

            CalculateEquityCommand = new RelayCommand(ShowCalculateEquityView);
            ButtonFilterModelSectionRemove_CommandClick = new RelayCommand(ButtonFilterModelSectionRemove_OnClick);
            MakeNoteCommand = new RelayCommand(MakeNote);
            DeleteHandCommand = new RelayCommand(x => DeleteHands());
            EditTournamentCommand = new RelayCommand(EditTournament);
            ReportRadioButtonClickCommand = new RelayCommand(ReportRadioButtonClick);

            InitializeFilter();
            UpdateReport();

            _eventAggregator.GetEvent<RequestDisplayTournamentHands>().Subscribe(DisplayTournamentHands);
            _eventAggregator.GetEvent<BuiltFilterChangedEvent>().Subscribe(UpdateBuiltFilter);
            _eventAggregator.GetEvent<HandNoteUpdatedEvent>().Subscribe(UpdateHandNote);
            _eventAggregator.GetEvent<TournamentDataUpdatedEvent>().Subscribe(UpdateReport);
            _eventAggregator.GetEvent<OpponentAnalysisBuildedEvent>().Subscribe(OnOpponentAnalysisBuilded, ThreadOption.UIThread);
            _eventAggregator.GetEvent<OpponentAnalysisBuildingEvent>().Subscribe(OnOpponentAnalysisBuilding, ThreadOption.UIThread);
        }

        private void OnOpponentAnalysisBuilding()
        {
            IsBusy = true;
        }

        private void OnOpponentAnalysisBuilded()
        {
            IsBusy = false;
            OnPropertyChanged(nameof(ReportSelectedItemStat));
        }

        private void InitializeFilter()
        {
            _filterAmountDictionary = new Dictionary<int, string>();

            _filterAmountDictionary.Add(100, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last100"));
            _filterAmountDictionary.Add(250, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last250"));
            _filterAmountDictionary.Add(1000, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last1000"));
            _filterAmountDictionary.Add(int.MaxValue, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_LastAll"));

            _filterAmountDictionarySelectedItem = _filterAmountDictionary.First().Key;

            _filterHandTagDictionary = new Dictionary<EnumHandTag, string>();

            _filterHandTagDictionary.Add(EnumHandTag.All, CommonResourceManager.Instance.GetResourceString(ResourceStrings.AllResourceString));
            _filterHandTagDictionary.Add(EnumHandTag.ForReview, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagForReview));
            _filterHandTagDictionary.Add(EnumHandTag.Bluff, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBluff));
            _filterHandTagDictionary.Add(EnumHandTag.HeroCall, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagHeroCall));
            _filterHandTagDictionary.Add(EnumHandTag.BigFold, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBigFold));

            _filterHandTagSelectedItem = _filterHandTagDictionary.First().Key;
        }

        #endregion

        #region ICommand implementation

        private void ShowCalculateEquityView(object obj)
        {
            if (obj is Playerstatistic)
            {
                var stat = obj as Playerstatistic;
                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RequestEquityCalculatorEvent>().Publish(new RequestEquityCalculatorEventArgs(stat.GameNumber, (short)stat.PokersiteId));
            }
            else
            {
                ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RequestEquityCalculatorEvent>().Publish(new RequestEquityCalculatorEventArgs());
            }
        }

        private void ButtonFilterModelSectionRemove_OnClick(object param)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<ResetFiltersEvent>().Publish(new ResetFiltersEventArgs((FilterSectionItem)param));
        }

        private void MakeNote(object obj)
        {
            if (obj != null && obj is ComparableCardsStatistic)
            {
                var stat = (obj as ComparableCardsStatistic);
                HandNoteViewModel viewModel = new HandNoteViewModel(stat.Statistic.GameNumber, (short)stat.Statistic.PokersiteId);
                var frm = new HandNoteView(viewModel);
                ((dynamic)frm).ShowDialog();
                stat.Statistic.HandNote = viewModel.HandNoteEntity;
                stat.UpdateHandNoteText();
            }
        }

        private void DeleteHands()
        {
            var itemsToDelete = SelectedComparableCardsStatistic?.Select(x => x.Statistic).ToArray();

            if (itemsToDelete != null && itemsToDelete.Length > 0)
            {
                var notification = new PopupBaseNotification()
                {
                    Title = itemsToDelete.Length == 1 ?
                        CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_SingleTitle") :
                        string.Format(CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_MultipleTitle"), itemsToDelete.Length),

                    CancelButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_Cancel"),
                    ConfirmButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_Yes"),

                    Content = itemsToDelete.Length == 1 ?
                        CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_SingleContent") :
                        CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_MultipleContent"),

                    IsDisplayH1Text = true
                };

                PopupRequest.Raise(notification,
                      confirmation =>
                      {
                          if (confirmation.Confirmed)
                          {
                              var dataservice = ServiceLocator.Current.GetInstance<IDataService>();
                              var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                              foreach (var stat in itemsToDelete)
                              {
                                  dataservice.DeletePlayerStatisticFromFile(stat);
                                  StorageModel.StatisticCollection.Remove(stat);
                              }

                              eventAggregator.GetEvent<UpdateViewRequestedEvent>().Publish(new UpdateViewRequestedEventArgs { IsUpdateReportRequested = true });
                          }
                      });
            }
        }

        private void EditTournament(object obj)
        {
            if (obj != null && obj is TournamentReportRecord)
            {
                var tournament = obj as TournamentReportRecord;
                EditTournamentViewModel viewModel = new EditTournamentViewModel();
                viewModel.LoadTournament(tournament.PlayerName, tournament.TournamentId, (short)tournament.Source.PokersiteId);
                var frm = Activator.CreateInstance(typeof(EditTournamentView), viewModel);
                ((dynamic)frm).ShowDialog();
            }
        }

        private void ReportRadioButtonClick(object obj)
        {
            _eventAggregator.GetEvent<UpdateViewRequestedEvent>().Publish(new UpdateViewRequestedEventArgs { IsUpdateReportRequested = false });
        }
        #endregion

        #region Methods

        internal void UpdateReport(object obj = null)
        {
            _eventAggregator.GetEvent<UpdateReportEvent>().Publish(StorageModel?.FilterPredicate);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (ReportSelectedItemStat == EnumReports.None)
                {
                    SetDefaultItemStat(this.IsShowTournamentData);
                }
                else
                {
                    OnPropertyChanged(nameof(ReportGadgetViewModel.ReportSelectedItemStat));
                }
            });

            if (ReportCollection.Any())
            {
                ReportSelectedItem = ReportCollection.FirstOrDefault();
            }
        }

        internal void RefreshReport()
        {
            ReportSelectedItemStatisticsCollection_FilterApply();
        }

        private void ReportSelectedItemStatisticsCollection_FilterApply()
        {
            if (this.ReportSelectedItemStatisticsCollection == null) return;
            var predicate = PredicateBuilder.True<ComparableCardsStatistic>();

            if (this.FilterTaggedHands_IsChecked && this.FilterHandTagSelectedItem != EnumHandTag.None) predicate = predicate.And(GetHandTagPredicate());

            var filteredCollection = this.ReportSelectedItemStatisticsCollection.AsQueryable().Select(x => new ComparableCardsStatistic(x)).Where(predicate).OrderByDescending(x => x.Statistic.Time).Take(this.FilterAmountDictionarySelectedItem);
            this.ReportSelectedItemStatisticsCollection_Filtered.Reset(filteredCollection);
        }

        private Expression<Func<ComparableCardsStatistic, bool>> GetHandTagPredicate()
        {
            if (this.FilterHandTagSelectedItem == EnumHandTag.All)
            {
                return PredicateBuilder.Create<ComparableCardsStatistic>(x => x.Statistic.HandTag != EnumHandTag.None);
            }
            else
            {
                return PredicateBuilder.Create<ComparableCardsStatistic>(x => x.Statistic.HandTag == FilterHandTagSelectedItem);
            }
        }

        private void SetDefaultSelection()
        {
            var report = ReportCollection.FirstOrDefault();
            if (report == null) return;

            ReportSelectedItem = report;
        }

        private void SetDefaultItemStat(bool isTournament)
        {
            if (isTournament)
            {
                _reportSelectedItemStat = EnumReports.TournamentResults;
                _invisibleSelectedItemStat = EnumReports.OverAll;
            }
            else
            {
                _reportSelectedItemStat = EnumReports.OverAll;
                _invisibleSelectedItemStat = EnumReports.TournamentResults;
            }
        }

        private void RestoreSelectedItemStat()
        {
            var temp = _invisibleSelectedItemStat;
            _invisibleSelectedItemStat = ReportSelectedItemStat;
            if (temp != EnumReports.None)
            {
                _reportSelectedItemStat = temp;
            }
        }

        internal void RaiseNotification(string content, string title)
        {
            this.NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = content,
                        Title = title,
                    },
                    n => { });
        }

        // TODO: Opponent Analysis report turned off
        //public async Task<IList<Playerstatistic>> GetTop()
        //{
        //    return await _topPlayersService.GetTop();
        //}
        #endregion

        #region Properties

        public InteractionRequest<PopupBaseNotification> PopupRequest { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private bool _isShowTournamentData = false;

        private RangeObservableCollection<Indicators> _reportCollection;
        private EnumReports _reportSelectedItemStat;

        private Indicators _reportSelectedItem;
        private IEnumerable<Playerstatistic> _reportSelectedItemStatisticsCollection;
        private IEnumerable<Playerstatistic> _selectedStatistics;

        private RangeObservableCollection<ComparableCardsStatistic> _reportSelectedItemStatisticsCollection_Filtered;

        private bool _filterTaggedHands_IsChecked = false;
        private bool _replayerShowHolecards_IsChecked = true;

        private bool _showHands;

        private Dictionary<int, string> _filterAmountDictionary;
        private int _filterAmountDictionarySelectedItem;

        private Dictionary<EnumHandTag, string> _filterHandTagDictionary;
        private EnumHandTag _filterHandTagSelectedItem;

        private BuiltFilterModel _currentlyBuiltFilter;
        private bool _isBusy;

        private bool _isEquityCalculatorEnabled = true;

        public bool IsShowTournamentData
        {
            get { return _isShowTournamentData; }
            set
            {
                if (value != IsShowTournamentData)
                {
                    if (_invisibleSelectedItemStat != EnumReports.None)
                    {
                        RestoreSelectedItemStat();
                    }
                    else
                    {
                        SetDefaultItemStat(value);
                    }
                }
                SetProperty(ref _isShowTournamentData, value);
                OnPropertyChanged("IsShowCashData");
            }
        }

        public bool IsShowCashData
        {
            get { return !_isShowTournamentData; }
        }

        public RangeObservableCollection<Indicators> ReportCollection
        {
            get { return _reportCollection; }
            set
            {
                _reportCollection = value;
                OnPropertyChanged();
            }
        }

        public EnumReports ReportSelectedItemStat
        {
            get { return _reportSelectedItemStat; }
            set
            {
                _reportSelectedItemStat = value;
                OnPropertyChanged();
            }
        }

        public Indicators ReportSelectedItem
        {
            get { return _reportSelectedItem; }
            set
            {
                _reportSelectedItem = value;
                OnPropertyChanged();

                if (value != null)
                {
                    this.ReportSelectedItemStatisticsCollection = new List<Playerstatistic>(value.Statistics);
                }
                else
                {
                    this.ReportSelectedItemStatisticsCollection = new List<Playerstatistic>();
                }
            }
        }

        public IEnumerable<Playerstatistic> ReportSelectedItemStatisticsCollection
        {
            get { return _reportSelectedItemStatisticsCollection; }
            set
            {
                _reportSelectedItemStatisticsCollection = value;
                OnPropertyChanged();

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public IEnumerable<Playerstatistic> SelectedStatistics
        {
            get { return _selectedStatistics; }
            set
            {
                _selectedStatistics = value;
                OnPropertyChanged();
            }
        }

        public RangeObservableCollection<ComparableCardsStatistic> ReportSelectedItemStatisticsCollection_Filtered
        {
            get { return _reportSelectedItemStatisticsCollection_Filtered; }
            set
            {
                _reportSelectedItemStatisticsCollection_Filtered = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ComparableCardsStatistic> selectedComparableCardsStatistic = new ObservableCollection<ComparableCardsStatistic>();

        public ObservableCollection<ComparableCardsStatistic> SelectedComparableCardsStatistic
        {
            get
            {
                return selectedComparableCardsStatistic;
            }
            private set
            {
                if (ReferenceEquals(selectedComparableCardsStatistic, value))
                {
                    return;
                }

                selectedComparableCardsStatistic = value;
                OnPropertyChanged();
            }
        }

        public bool FilterTaggedHands_IsChecked
        {
            get { return _filterTaggedHands_IsChecked; }
            set
            {
                SetProperty(ref _filterTaggedHands_IsChecked, value);

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public bool ReplayerShowHolecards_IsChecked
        {
            get { return _replayerShowHolecards_IsChecked; }
            set
            {
                SetProperty(ref _replayerShowHolecards_IsChecked, value);
            }
        }

        public Dictionary<int, string> FilterAmountDictionary
        {
            get { return _filterAmountDictionary; }
            set
            {
                _filterAmountDictionary = value;
                OnPropertyChanged();
            }
        }

        public int FilterAmountDictionarySelectedItem
        {
            get { return _filterAmountDictionarySelectedItem; }
            set
            {
                _filterAmountDictionarySelectedItem = value;
                OnPropertyChanged();

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public Dictionary<EnumHandTag, string> FilterHandTagDictionary
        {
            get { return _filterHandTagDictionary; }
            set
            {
                SetProperty(ref _filterHandTagDictionary, value);
            }
        }

        public EnumHandTag FilterHandTagSelectedItem
        {
            get { return _filterHandTagSelectedItem; }
            set
            {
                SetProperty(ref _filterHandTagSelectedItem, value);

                if (FilterTaggedHands_IsChecked)
                {
                    ReportSelectedItemStatisticsCollection_FilterApply();
                }
            }
        }

        public bool ShowHands
        {
            get { return _showHands; }
            set
            {
                if (value.Equals(_showHands)) return;
                _showHands = value;
                OnPropertyChanged();
            }
        }

        public BuiltFilterModel CurrentlyBuiltFilter
        {
            get { return _currentlyBuiltFilter; }
            set
            {
                SetProperty(ref _currentlyBuiltFilter, value);
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                if ((value && ReportSelectedItemStat == EnumReports.OpponentAnalysis))
                    SetProperty(ref _isBusy, true);
                else
                    SetProperty(ref _isBusy, false);
            }
        }

        public bool IsEquityCalculatorEnabled
        {
            get { return _isEquityCalculatorEnabled; }
            set { SetProperty(ref _isEquityCalculatorEnabled, value); }
        }

        #endregion

        #region Events

        private void DisplayTournamentHands(RequestDisplayTournamentHandsEvent obj)
        {
            ReportSelectedItemStatisticsCollection = new ObservableCollection<Playerstatistic>(StorageModel.StatisticCollection.ToList().Where(x => x.TournamentId == obj.TournamentNumber));
        }

        private void UpdateBuiltFilter(BuiltFilterChangedEventArgs obj)
        {
            this.CurrentlyBuiltFilter = obj.BuiltFilter;
            UpdateReport();
        }

        private void UpdateHandNote(HandNoteUpdatedEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            var item = this.ReportSelectedItemStatisticsCollection_Filtered.FirstOrDefault(x => x.Statistic.GameNumber == obj.GameNumber && x.Statistic.PlayerName == obj.PlayerName);
            if (item != null)
            {
                item.UpdateHandNoteText();
            }
        }
        #endregion
    }
}