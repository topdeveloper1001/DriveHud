using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Data;
using Prism.Events;
using Model;
using Model.Data;
using DriveHUD.Entities;
using Model.Enums;
using DriveHUD.Common.Infrastructure.Base;
using Model.Events;
using Microsoft.Practices.ServiceLocation;
using System;
using Model.Interfaces;
using DriveHUD.Common.Resources;
using Model.Extensions;
using System.Linq.Expressions;
using DriveHUD.Common.Utils;
using Model.Filters;
using DriveHUD.Common.Reflection;
using DriveHUD.Application.Views;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using Prism.Interactivity.InteractionRequest;
using DriveHUD.Common.Wpf.Actions;
using System.IO;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Report view model
    /// </summary>
    public class ReportGadgetViewModel : BaseViewModel
    {
        #region Private Fields
        private EnumReports _invisibleSelectedItemStat;
        #endregion

        #region ICommand
        public ICommand CalculateEquityCommand { get; set; }
        public ICommand ButtonFilterModelSectionRemove_CommandClick { get; set; }
        public ICommand MakeNoteCommand { get; set; }
        public ICommand DeleteHandCommand { get; set; }
        public ICommand EditTournamentCommand { get; set; }
        #endregion

        #region Initialize

        internal ReportGadgetViewModel()
        {
            this.PopupRequest = new InteractionRequest<PopupBaseNotification>();
            this.NotificationRequest = new InteractionRequest<INotification>();

            this.ReportCollection = new RangeObservableCollection<Indicators>();
            this.ReportSelectedItemStatisticsCollection_Filtered = new RangeObservableCollection<ComparableCardsStatistic>();

            CalculateEquityCommand = new RelayCommand(ShowCalculateEquityView);
            ButtonFilterModelSectionRemove_CommandClick = new RelayCommand(ButtonFilterModelSectionRemove_OnClick);
            MakeNoteCommand = new RelayCommand(MakeNote);
            DeleteHandCommand = new RelayCommand(DeleteHand);
            EditTournamentCommand = new RelayCommand(EditTournament);

            InitializeFilter();
            UpdateReport();

            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<RequestDisplayTournamentHands>().Subscribe(DisplayTournamentHands);
            eventAggregator.GetEvent<BuiltFilterChangedEvent>().Subscribe(UpdateBuiltFilter);
            eventAggregator.GetEvent<HandNoteUpdatedEvent>().Subscribe(UpdateHandNote);
            eventAggregator.GetEvent<TournamentDataUpdatedEvent>().Subscribe(UpdateReport);
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
                HandNoteViewModel viewModel = new HandNoteViewModel();
                viewModel.GameNumber = stat.Statistic.GameNumber;
                var frm = Activator.CreateInstance(typeof(HandNoteView), viewModel, (short)stat.Statistic.PokersiteId);
                ((dynamic)frm).ShowDialog();
                stat.Statistic.HandNote = viewModel.HandNoteEntity;
                stat.UpdateHandNoteText();
            }
        }

        private void DeleteHand(object obj)
        {
            if (obj != null && obj is ComparableCardsStatistic)
            {
                var stat = (obj as ComparableCardsStatistic)?.Statistic;

                if (stat == null)
                {
                    return;
                }

                var notification = new PopupBaseNotification()
                {
                    Title = "Delete Hand",
                    CancelButtonCaption = "Cancel",
                    ConfirmButtonCaption = "Yes",
                    Content = "Are you sure you want to delete this hand?",
                    IsDisplayH1Text = true
                };

                PopupRequest.Raise(notification,
                      confirmation =>
                      {
                          if (confirmation.Confirmed)
                          {
                              var dataservice = ServiceLocator.Current.GetInstance<IDataService>();
                              var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                              dataservice.DeletePlayerStatisticFromFile(stat);
                              StorageModel.StatisticCollection.Remove(stat);

                              eventAggregator.GetEvent<UpdateViewRequestedEvent>().Publish(null);
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
        #endregion

        #region Methods

        internal void UpdateReport(object obj = null)
        {
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
        #endregion

        #region Properties

        public InteractionRequest<PopupBaseNotification> PopupRequest { get; set; }
        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private bool _isShowTournamentData = false;

        private RangeObservableCollection<Indicators> _reportCollection;
        private EnumReports _reportSelectedItemStat;

        private Indicators _reportSelectedItem;
        private IEnumerable<Playerstatistic> _reportSelectedItemStatisticsCollection;

        private RangeObservableCollection<ComparableCardsStatistic> _reportSelectedItemStatisticsCollection_Filtered;

        private bool _filterTaggedHands_IsChecked = false;
        private bool _replayerShowHolecards_IsChecked = true;

        private bool _showHands;

        private Dictionary<int, string> _filterAmountDictionary;
        private int _filterAmountDictionarySelectedItem;

        private Dictionary<EnumHandTag, string> _filterHandTagDictionary;
        private EnumHandTag _filterHandTagSelectedItem;

        private BuiltFilterModel _currentlyBuiltFilter;

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
                    this.ReportSelectedItemStatisticsCollection = new List<Playerstatistic>(value.Statistcs);
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

        public RangeObservableCollection<ComparableCardsStatistic> ReportSelectedItemStatisticsCollection_Filtered
        {
            get { return _reportSelectedItemStatisticsCollection_Filtered; }
            set
            {
                _reportSelectedItemStatisticsCollection_Filtered = value;
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

        #endregion

        #region Events

        private void DisplayTournamentHands(RequestDisplayTournamentHandsEvent obj)
        {
            ReportSelectedItemStatisticsCollection = new ObservableCollection<Playerstatistic>(this.StorageModel.StatisticCollection.Where(x => x.TournamentId == obj.TournamentNumber));
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
