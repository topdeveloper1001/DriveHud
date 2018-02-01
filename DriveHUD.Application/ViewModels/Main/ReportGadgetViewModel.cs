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

        private EnumReports invisibleSelectedItemStat;
        private readonly IEventAggregator eventAggregator;

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
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            PopupRequest = new InteractionRequest<PopupBaseNotification>();
            NotificationRequest = new InteractionRequest<INotification>();

            ReportCollection = new RangeObservableCollection<Indicators>();
            ReportSelectedItemStatisticsCollection_Filtered = new RangeObservableCollection<ComparableCardsStatistic>();

            CalculateEquityCommand = new RelayCommand(ShowCalculateEquityView);
            ButtonFilterModelSectionRemove_CommandClick = new RelayCommand(ButtonFilterModelSectionRemove_OnClick);
            MakeNoteCommand = new RelayCommand(MakeNote);
            DeleteHandCommand = new RelayCommand(x => DeleteHands());
            EditTournamentCommand = new RelayCommand(EditTournament);
            ReportRadioButtonClickCommand = new RelayCommand(ReportRadioButtonClick);

            InitializeFilter();
            UpdateReport();

            eventAggregator.GetEvent<RequestDisplayTournamentHands>().Subscribe(DisplayTournamentHands);
            eventAggregator.GetEvent<BuiltFilterChangedEvent>().Subscribe(UpdateBuiltFilter);
            eventAggregator.GetEvent<HandNoteUpdatedEvent>().Subscribe(UpdateHandNote);
            eventAggregator.GetEvent<TournamentDataUpdatedEvent>().Subscribe(UpdateReport);
        }

        private void InitializeFilter()
        {
            filterAmountDictionary = new Dictionary<int, string>();

            filterAmountDictionary.Add(100, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last100"));
            filterAmountDictionary.Add(250, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last250"));
            filterAmountDictionary.Add(1000, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last1000"));
            filterAmountDictionary.Add(int.MaxValue, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_LastAll"));

            filterAmountDictionarySelectedItem = filterAmountDictionary.First().Key;

            filterHandTagDictionary = new Dictionary<EnumHandTag, string>();

            filterHandTagDictionary.Add(EnumHandTag.All, CommonResourceManager.Instance.GetResourceString(ResourceStrings.AllResourceString));
            filterHandTagDictionary.Add(EnumHandTag.ForReview, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagForReview));
            filterHandTagDictionary.Add(EnumHandTag.Bluff, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBluff));
            filterHandTagDictionary.Add(EnumHandTag.HeroCall, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagHeroCall));
            filterHandTagDictionary.Add(EnumHandTag.BigFold, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBigFold));

            filterHandTagSelectedItem = filterHandTagDictionary.First().Key;
        }

        #endregion

        #region ICommand implementation

        private void ShowCalculateEquityView(object obj)
        {
            var stat = obj as Playerstatistic;

            var requestEquityCalculatorEventArgs = stat != null ?
                new RequestEquityCalculatorEventArgs(stat.GameNumber, (short)stat.PokersiteId) :
                new RequestEquityCalculatorEventArgs();

            eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Publish(requestEquityCalculatorEventArgs);
        }

        private void ButtonFilterModelSectionRemove_OnClick(object param)
        {
            eventAggregator.GetEvent<ResetFiltersEvent>().Publish(new ResetFiltersEventArgs((FilterSectionItem)param));
        }

        private void MakeNote(object obj)
        {
            var stat = obj as ComparableCardsStatistic;

            if (stat == null)
            {
                return;
            }

            var handNoteViewModel = new HandNoteViewModel(stat.Statistic.GameNumber, (short)stat.Statistic.PokersiteId);

            var handNoteView = new HandNoteView(handNoteViewModel);

            handNoteView.ShowDialog();

            stat.Statistic.HandNote = handNoteViewModel.HandNoteEntity;

            stat.UpdateHandNoteText();
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
            var tournament = obj as TournamentReportRecord;

            if (tournament == null)
            {
                return;
            }

            var editTournamentViewModel = new EditTournamentViewModel();
            editTournamentViewModel.LoadTournament(tournament.PlayerName, tournament.TournamentId, (short)tournament.Source.PokersiteId);

            var tournamentView = new EditTournamentView(editTournamentViewModel);
            tournamentView.ShowDialog();
        }

        private void ReportRadioButtonClick(object obj)
        {
            eventAggregator.GetEvent<UpdateViewRequestedEvent>().Publish(new UpdateViewRequestedEventArgs { IsUpdateReportRequested = false });
        }

        #endregion

        #region Methods

        internal void UpdateReport(object obj = null)
        {
            eventAggregator.GetEvent<UpdateReportEvent>().Publish(StorageModel?.FilterPredicate);

            App.Current.Dispatcher.Invoke(() =>
            {
                if (ReportSelectedItemStat == EnumReports.None)
                {
                    SetDefaultItemStat(this.IsShowTournamentData);
                }
                else
                {
                    OnPropertyChanged(nameof(ReportSelectedItemStat));
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
            if (ReportSelectedItemStatisticsCollection == null)
            {
                return;
            }

            var predicate = PredicateBuilder.True<ComparableCardsStatistic>();

            if (FilterTaggedHands_IsChecked && FilterHandTagSelectedItem != EnumHandTag.None)
            {
                predicate = predicate.And(GetHandTagPredicate());
            }



            var filteredCollection = ReportSelectedItemStatisticsCollection
                .AsQueryable()
                .Select(x => new ComparableCardsStatistic(x))
                .Where(predicate)
                .OrderByDescending(x => x.Statistic.Time)
                .Take(FilterAmountDictionarySelectedItem);

            ReportSelectedItemStatisticsCollection_Filtered.Reset(filteredCollection);
        }

        private Expression<Func<ComparableCardsStatistic, bool>> GetHandTagPredicate()
        {
            if (FilterHandTagSelectedItem == EnumHandTag.All)
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

            if (report == null)
            {
                return;
            }

            ReportSelectedItem = report;
        }

        private void SetDefaultItemStat(bool isTournament)
        {
            if (isTournament)
            {
                reportSelectedItemStat = EnumReports.TournamentResults;
                invisibleSelectedItemStat = EnumReports.OverAll;
            }
            else
            {
                reportSelectedItemStat = EnumReports.OverAll;
                invisibleSelectedItemStat = EnumReports.TournamentResults;
            }
        }

        private void RestoreSelectedItemStat()
        {
            var temp = invisibleSelectedItemStat;

            invisibleSelectedItemStat = ReportSelectedItemStat;

            if (temp != EnumReports.None)
            {
                reportSelectedItemStat = temp;
            }
        }

        internal void RaiseNotification(string content, string title)
        {
            NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = content,
                        Title = title,
                    },
                    n => { });
        }

        #endregion

        #region Properties

        public InteractionRequest<PopupBaseNotification> PopupRequest { get; private set; }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private bool isShowTournamentData = false;

        private RangeObservableCollection<Indicators> reportCollection;
        private EnumReports reportSelectedItemStat;

        private Indicators reportSelectedItem;
        private IEnumerable<Playerstatistic> reportSelectedItemStatisticsCollection;
        private IEnumerable<Playerstatistic> selectedStatistics;

        private RangeObservableCollection<ComparableCardsStatistic> reportSelectedItemStatisticsCollection_Filtered;

        private bool filterTaggedHands_IsChecked = false;
        private bool replayerShowHolecards_IsChecked = true;

        private bool showHands;

        private Dictionary<int, string> filterAmountDictionary;
        private int filterAmountDictionarySelectedItem;

        private Dictionary<EnumHandTag, string> filterHandTagDictionary;
        private EnumHandTag filterHandTagSelectedItem;

        private BuiltFilterModel currentlyBuiltFilter;
        private bool isBusy;

        private bool isEquityCalculatorEnabled = true;

        public bool IsShowTournamentData
        {
            get { return isShowTournamentData; }
            set
            {
                if (value != IsShowTournamentData)
                {
                    if (invisibleSelectedItemStat != EnumReports.None)
                    {
                        RestoreSelectedItemStat();
                    }
                    else
                    {
                        SetDefaultItemStat(value);
                    }
                }

                SetProperty(ref isShowTournamentData, value);
                OnPropertyChanged(nameof(IsShowCashData));
            }
        }

        public bool IsShowCashData
        {
            get { return !isShowTournamentData; }
        }

        public RangeObservableCollection<Indicators> ReportCollection
        {
            get { return reportCollection; }
            set
            {
                reportCollection = value;
                OnPropertyChanged();
            }
        }

        public EnumReports ReportSelectedItemStat
        {
            get { return reportSelectedItemStat; }
            set
            {
                reportSelectedItemStat = value;
                OnPropertyChanged();
            }
        }

        public Indicators ReportSelectedItem
        {
            get
            {
                return reportSelectedItem;
            }
            set
            {
                SetProperty(ref reportSelectedItem, value);

                if (value != null)
                {
                    ReportSelectedItemStatisticsCollection = new List<Playerstatistic>(value.Statistics);
                }
                else
                {
                    ReportSelectedItemStatisticsCollection = new List<Playerstatistic>();
                }
            }
        }

        public IEnumerable<Playerstatistic> ReportSelectedItemStatisticsCollection
        {
            get { return reportSelectedItemStatisticsCollection; }
            set
            {
                SetProperty(ref reportSelectedItemStatisticsCollection, value);

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public IEnumerable<Playerstatistic> SelectedStatistics
        {
            get { return selectedStatistics; }
            set
            {
                selectedStatistics = value;
                OnPropertyChanged();
            }
        }

        public RangeObservableCollection<ComparableCardsStatistic> ReportSelectedItemStatisticsCollection_Filtered
        {
            get { return reportSelectedItemStatisticsCollection_Filtered; }
            set
            {
                reportSelectedItemStatisticsCollection_Filtered = value;
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
            get { return filterTaggedHands_IsChecked; }
            set
            {
                SetProperty(ref filterTaggedHands_IsChecked, value);

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public bool ReplayerShowHolecards_IsChecked
        {
            get { return replayerShowHolecards_IsChecked; }
            set
            {
                SetProperty(ref replayerShowHolecards_IsChecked, value);
            }
        }

        public Dictionary<int, string> FilterAmountDictionary
        {
            get { return filterAmountDictionary; }
            set
            {
                filterAmountDictionary = value;
                OnPropertyChanged();
            }
        }

        public int FilterAmountDictionarySelectedItem
        {
            get
            {
                return filterAmountDictionarySelectedItem;
            }
            set
            {
                SetProperty(ref filterAmountDictionarySelectedItem, value);

                ReportSelectedItemStatisticsCollection_FilterApply();
            }
        }

        public Dictionary<EnumHandTag, string> FilterHandTagDictionary
        {
            get { return filterHandTagDictionary; }
            set
            {
                SetProperty(ref filterHandTagDictionary, value);
            }
        }

        public EnumHandTag FilterHandTagSelectedItem
        {
            get { return filterHandTagSelectedItem; }
            set
            {
                SetProperty(ref filterHandTagSelectedItem, value);

                if (FilterTaggedHands_IsChecked)
                {
                    ReportSelectedItemStatisticsCollection_FilterApply();
                }
            }
        }

        public bool ShowHands
        {
            get { return showHands; }
            set
            {
                if (value.Equals(showHands)) return;
                showHands = value;
                OnPropertyChanged();
            }
        }

        public BuiltFilterModel CurrentlyBuiltFilter
        {
            get { return currentlyBuiltFilter; }
            set
            {
                SetProperty(ref currentlyBuiltFilter, value);
            }
        }

        public bool IsBusy
        {
            get { return isBusy; }
            set { SetProperty(ref isBusy, value); }
        }

        public bool IsEquityCalculatorEnabled
        {
            get { return isEquityCalculatorEnabled; }
            set { SetProperty(ref isEquityCalculatorEnabled, value); }
        }

        #endregion

        #region Events

        private void DisplayTournamentHands(RequestDisplayTournamentHandsEvent obj)
        {
            ReportSelectedItemStatisticsCollection = new ObservableCollection<Playerstatistic>(StorageModel.StatisticCollection.ToList().Where(x => x.TournamentId == obj.TournamentNumber));
        }

        private void UpdateBuiltFilter(BuiltFilterChangedEventArgs obj)
        {
            CurrentlyBuiltFilter = obj.BuiltFilter;
            UpdateReport();
        }

        private void UpdateHandNote(HandNoteUpdatedEventArgs obj)
        {
            if (obj == null)
            {
                return;
            }

            var item = ReportSelectedItemStatisticsCollection_Filtered.FirstOrDefault(x => x.Statistic.GameNumber == obj.GameNumber && x.Statistic.PlayerName == obj.PlayerName);

            if (item != null)
            {
                item.UpdateHandNoteText();
            }
        }

        #endregion
    }
}