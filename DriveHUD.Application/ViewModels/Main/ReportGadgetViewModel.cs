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
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
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
using System.Threading.Tasks;
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

            ReportCollection = new RangeObservableCollection<ReportIndicators>();
            FilteredReportSelectedItemStatisticsCollection = new RangeObservableCollection<ReportHandViewModel>();

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
            filterAmountDictionary = new Dictionary<int, string>
            {
                { 100, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last100") },
                { 250, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last250") },
                { 1000, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_Last1000") },
                { int.MaxValue, CommonResourceManager.Instance.GetResourceString("Main_ReportGadgetView_LastAll") }
            };

            filterAmountDictionarySelectedItem = filterAmountDictionary.First().Key;

            filterHandTagDictionary = new Dictionary<EnumHandTag, string>
            {
                { EnumHandTag.All, CommonResourceManager.Instance.GetResourceString(ResourceStrings.AllResourceString) },
                { EnumHandTag.ForReview, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagForReview) },
                { EnumHandTag.Bluff, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBluff) },
                { EnumHandTag.HeroCall, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagHeroCall) },
                { EnumHandTag.BigFold, CommonResourceManager.Instance.GetResourceString(ResourceStrings.HandTagBigFold) }
            };

            filterHandTagSelectedItem = filterHandTagDictionary.First().Key;
        }

        #endregion

        #region ICommand implementation

        private void ShowCalculateEquityView(object obj)
        {
            var reportHand = obj as ReportHandViewModel;

            var requestEquityCalculatorEventArgs = reportHand != null ?
                new RequestEquityCalculatorEventArgs(reportHand.GameNumber, (short)reportHand.PokerSiteId) :
                new RequestEquityCalculatorEventArgs();

            eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Publish(requestEquityCalculatorEventArgs);
        }

        private void ButtonFilterModelSectionRemove_OnClick(object param)
        {
            eventAggregator.GetEvent<ResetFiltersEvent>().Publish(new ResetFiltersEventArgs((FilterSectionItem)param));
        }

        private void MakeNote(object obj)
        {
            var reportHand = obj as ReportHandViewModel;

            if (reportHand == null)
            {
                return;
            }

            var handNoteViewModel = new HandNoteViewModel(reportHand.GameNumber, (short)reportHand.PokerSiteId);

            var handNoteView = new HandNoteView(handNoteViewModel);

            handNoteView.ShowDialog();

            if (handNoteViewModel.HandNoteEntity != null)
            {
                reportHand.HandNote = handNoteViewModel.HandNoteEntity.Note;
            }

            var statistic = ReportSelectedItem.Statistics?.FirstOrDefault(x => x.GameNumber == reportHand.GameNumber && x.PokersiteId == reportHand.PokerSiteId);

            if (statistic != null)
            {
                statistic.HandNote = handNoteViewModel.HandNoteEntity;
            }
        }

        private void DeleteHands()
        {
            var handsToDelete = SelectedReportHands?.Select(x => new { x.GameNumber, x.PokerSiteId }).ToArray();

            if (handsToDelete == null || handsToDelete.Length == 0)
            {
                return;
            }

            var notification = new PopupBaseNotification()
            {
                Title = handsToDelete.Length == 1 ?
                    CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_SingleTitle") :
                    string.Format(CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_MultipleTitle"), handsToDelete.Length),

                CancelButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_Cancel"),
                ConfirmButtonCaption = CommonResourceManager.Instance.GetResourceString("Notifications_DeleteHand_Yes"),

                Content = handsToDelete.Length == 1 ?
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

                          handsToDelete.ForEach(x => dataservice.DeleteHandHistory(x.GameNumber, x.PokerSiteId));

                          SelectedReportHands.ForEach(x =>
                          {
                              ReportSelectedItemStatisticsCollection.Remove(x);
                              ReportSelectedItem.ReportHands.Remove(x);
                              StorageModel.StatisticCollection.RemoveByCondition(s => s.GameNumber == x.GameNumber && s.PokersiteId == x.PokerSiteId);
                          });

                          eventAggregator.GetEvent<UpdateViewRequestedEvent>().Publish(new UpdateViewRequestedEventArgs { IsUpdateReportRequested = true });
                      }
                  });

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
            FilterReportSelectedItemStatisticsCollection();
        }

        private void FilterReportSelectedItemStatisticsCollection()
        {
            if (ReportSelectedItemStatisticsCollection == null)
            {
                return;
            }

            var predicate = PredicateBuilder.True<ReportHandViewModel>();

            if (FilterTaggedHands_IsChecked && FilterHandTagSelectedItem != EnumHandTag.None)
            {
                predicate = predicate.And(GetHandTagPredicate());
            }

            var filteredCollection = ReportSelectedItemStatisticsCollection
                .AsQueryable()
                .Where(predicate)
                .OrderByDescending(x => x.Time)
                .Take(FilterAmountDictionarySelectedItem);

            FilteredReportSelectedItemStatisticsCollection.Reset(filteredCollection);
        }

        private Expression<Func<ReportHandViewModel, bool>> GetHandTagPredicate()
        {
            if (FilterHandTagSelectedItem == EnumHandTag.All)
            {
                return PredicateBuilder.Create<ReportHandViewModel>(x => x.HandTag != EnumHandTag.None);
            }

            return PredicateBuilder.Create<ReportHandViewModel>(x => x.HandTag == FilterHandTagSelectedItem);
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

        private async void LoadOpponentReportHands()
        {
            var report = ReportSelectedItem as OpponentReportIndicators;

            if (report == null || report.ReportHands.Count >= filterAmountDictionarySelectedItem)
            {
                FilterReportSelectedItemStatisticsCollection();
                return;
            }

            var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            IsHandGridBusy = true;

            if (!ReferenceEquals(report.ReportHands, ReportSelectedItemStatisticsCollection))
            {
                ReportSelectedItemStatisticsCollection?.Clear();
                GC.Collect();
            }

            await Task.Run(() =>
            {
                try
                {
                    var statistic = dataService.GetPlayerStatisticFromFile(report.PlayerId, x => !x.IsTourney)
                        .OrderByDescending(x => x.Time)
                        .Take(filterAmountDictionarySelectedItem)
                        .ToArray();

                    ReportSelectedItemStatisticsCollection = new ObservableCollection<ReportHandViewModel>(statistic.Select(x => new ReportHandViewModel(x)));
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Could not load player hands for opponent report", e);
                }
            });

            IsHandGridBusy = false;
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

        private bool isShowTournamentData;

        public bool IsShowTournamentData
        {
            get
            {
                return isShowTournamentData;
            }
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
            get
            {
                return !isShowTournamentData;
            }
        }

        private RangeObservableCollection<ReportIndicators> reportCollection;

        public RangeObservableCollection<ReportIndicators> ReportCollection
        {
            get
            {
                return reportCollection;
            }
            set
            {
                SetProperty(ref reportCollection, value);
            }
        }

        private EnumReports reportSelectedItemStat;

        public EnumReports ReportSelectedItemStat
        {
            get
            {
                return reportSelectedItemStat;
            }
            set
            {
                SetProperty(ref reportSelectedItemStat, value);

            }
        }

        private ReportIndicators reportSelectedItem;

        public ReportIndicators ReportSelectedItem
        {
            get
            {
                return reportSelectedItem;
            }
            set
            {
                if (ReportSelectedItemStat == EnumReports.OpponentAnalysis)
                {
                    if (reportSelectedItem is OpponentReportIndicators report)
                    {
                        report.ShrinkReportHands();
                    }
                }

                SetProperty(ref reportSelectedItem, value);

                if (reportSelectedItem != null)
                {
                    ReportSelectedItemStatisticsCollection = reportSelectedItem.ReportHands;

                    if (ReportSelectedItemStat == EnumReports.OpponentAnalysis)
                    {
                        LoadOpponentReportHands();
                        return;
                    }
                }
                else
                {
                    ReportSelectedItemStatisticsCollection = new ObservableCollection<ReportHandViewModel>();
                }
            }
        }

        private ObservableCollection<ReportHandViewModel> reportSelectedItemStatisticsCollection;

        public ObservableCollection<ReportHandViewModel> ReportSelectedItemStatisticsCollection
        {
            get
            {
                return reportSelectedItemStatisticsCollection;
            }
            set
            {
                SetProperty(ref reportSelectedItemStatisticsCollection, value);
                FilterReportSelectedItemStatisticsCollection();
            }
        }

        private RangeObservableCollection<ReportHandViewModel> filteredReportSelectedItemStatisticsCollection;

        public RangeObservableCollection<ReportHandViewModel> FilteredReportSelectedItemStatisticsCollection
        {
            get
            {
                return filteredReportSelectedItemStatisticsCollection;
            }
            set
            {
                SetProperty(ref filteredReportSelectedItemStatisticsCollection, value);
            }
        }

        private ObservableCollection<ReportHandViewModel> selectedReportHands = new ObservableCollection<ReportHandViewModel>();

        public ObservableCollection<ReportHandViewModel> SelectedReportHands
        {
            get
            {
                return selectedReportHands;
            }
            private set
            {
                SetProperty(ref selectedReportHands, value);
            }
        }

        private bool filterTaggedHands_IsChecked = false;

        public bool FilterTaggedHands_IsChecked
        {
            get
            {
                return filterTaggedHands_IsChecked;
            }
            set
            {
                SetProperty(ref filterTaggedHands_IsChecked, value);
                FilterReportSelectedItemStatisticsCollection();
            }
        }

        private bool replayerShowHolecards_IsChecked = true;

        public bool ReplayerShowHolecards_IsChecked
        {
            get
            {
                return replayerShowHolecards_IsChecked;
            }
            set
            {
                SetProperty(ref replayerShowHolecards_IsChecked, value);
            }
        }

        private Dictionary<int, string> filterAmountDictionary;

        public Dictionary<int, string> FilterAmountDictionary
        {
            get
            {
                return filterAmountDictionary;
            }
            set
            {
                SetProperty(ref filterAmountDictionary, value);
            }
        }

        private int filterAmountDictionarySelectedItem;

        public int FilterAmountDictionarySelectedItem
        {
            get
            {
                return filterAmountDictionarySelectedItem;
            }
            set
            {
                SetProperty(ref filterAmountDictionarySelectedItem, value);

                if (ReportSelectedItemStat == EnumReports.OpponentAnalysis)
                {
                    LoadOpponentReportHands();
                    return;
                }

                FilterReportSelectedItemStatisticsCollection();
            }
        }

        private Dictionary<EnumHandTag, string> filterHandTagDictionary;

        public Dictionary<EnumHandTag, string> FilterHandTagDictionary
        {
            get
            {
                return filterHandTagDictionary;
            }
            set
            {
                SetProperty(ref filterHandTagDictionary, value);
            }
        }

        private EnumHandTag filterHandTagSelectedItem;

        public EnumHandTag FilterHandTagSelectedItem
        {
            get
            {
                return filterHandTagSelectedItem;
            }
            set
            {
                SetProperty(ref filterHandTagSelectedItem, value);

                if (FilterTaggedHands_IsChecked)
                {
                    FilterReportSelectedItemStatisticsCollection();
                }
            }
        }

        private bool showHands;

        public bool ShowHands
        {
            get
            {
                return showHands;
            }
            set
            {
                SetProperty(ref showHands, value);
            }
        }

        private BuiltFilterModel currentlyBuiltFilter;

        public BuiltFilterModel CurrentlyBuiltFilter
        {
            get
            {
                return currentlyBuiltFilter;
            }
            set
            {
                SetProperty(ref currentlyBuiltFilter, value);
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                SetProperty(ref isBusy, value);
            }
        }

        private bool isHandGridBusy;

        public bool IsHandGridBusy
        {
            get
            {
                return isHandGridBusy;
            }
            set
            {
                SetProperty(ref isHandGridBusy, value);
            }
        }

        private bool isEquityCalculatorEnabled = true;

        public bool IsEquityCalculatorEnabled
        {
            get
            {
                return isEquityCalculatorEnabled;
            }
            set
            {
                SetProperty(ref isEquityCalculatorEnabled, value);
            }
        }

        #endregion

        #region Events

        private void DisplayTournamentHands(RequestDisplayTournamentHandsEvent obj)
        {
            if (obj == null)
            {
                return;
            }

            ReportSelectedItemStatisticsCollection = new ObservableCollection<ReportHandViewModel>(StorageModel
                .StatisticCollection
                .ToList()
                .Where(x => x.TournamentId == obj.TournamentNumber)
                .Select(x => new ReportHandViewModel(x)));
        }

        private void UpdateBuiltFilter(BuiltFilterChangedEventArgs obj)
        {
            CurrentlyBuiltFilter = obj.BuiltFilter;
            UpdateReport();
        }

        private void UpdateHandNote(HandNoteUpdatedEventArgs args)
        {
            if (args == null)
            {
                return;
            }

            var reportHand = FilteredReportSelectedItemStatisticsCollection.FirstOrDefault(x => x.GameNumber == args.GameNumber);

            if (reportHand == null)
            {
                return;
            }

            reportHand.HandNote = args.NoteText;
        }

        #endregion
    }
}