﻿//-----------------------------------------------------------------------
// <copyright file="PreflopSelectorViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.EquityCalculator.Analyzer;
using DriveHUD.EquityCalculator.Models;
using DriveHUD.ViewModels;
using DriveHUD.ViewModels.Replayer;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.LocalCalculator;
using Model.Notifications;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class PreflopSelectorViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields

        private static readonly int RanksLength = Enum.GetValues(typeof(RangeCardRank)).Cast<RangeCardRank>().Where(x => x != RangeCardRank.None).Count();

        private const int regularTotalPossibleCombos = 1326;
        private const int sixPlusTotalPossibleCombos = 630;

        private CardSelectorNotification _notification;
        private EquityRangeSelectorItemViewModel _selectedItem = new EquityRangeSelectorItemViewModel();
        private ReactiveList<EquityRangeSelectorItemViewModel> _preflopSelectorItems = new ReactiveList<EquityRangeSelectorItemViewModel>();
        private List<RangeSelectorItemViewModel> _temporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();
        private string _suitsForCaption = string.Empty;
        private int _sliderValue = 0;
        private bool _isSliderManualMove = true;
        private double _selectedPercentage = 0;
        private PreflopRange _preflopRange = new PreflopRange();
        private EquityCalculatorViewModel source;

        #endregion

        #region Properties

        private int TotalPossibleCombos
        {
            get
            {
                return _notification == null || _notification.EquityCalculatorMode == EquityCalculatorMode.Holdem ?
                    regularTotalPossibleCombos : sixPlusTotalPossibleCombos;
            }
        }

        public InteractionRequest<PreDefinedRangesNotifcation> PreDefinedRangesRequest { get; private set; }

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                if (value is CardSelectorNotification)
                {
                    _notification = value as CardSelectorNotification;
                    _notification.ReturnType = CardSelectorReturnType.Range;
                    source = _notification.Source as EquityCalculatorViewModel;

                    if (_notification.CardsContainer.Ranges != null)
                    {
                        foreach (EquityRangeSelectorItemViewModel item in _notification.CardsContainer.Ranges)
                        {
                            var current = PreflopSelectorItems.FirstOrDefault(x => x.Caption == item.Caption);

                            if (current != null)
                            {
                                current.IsSelected = true;
                                current.SetEquitySelectionMode(item.EquitySelectionMode);
                                current.HandSuitsModelList = new List<HandSuitsViewModel>(item.HandSuitsModelList);
                                current.ItemLikelihood = item.ItemLikelihood;
                                current.LikelihoodPercent = item.LikelihoodPercent;
                                current.RefreshCombos();
                            }
                        }
                    }
                }

                InitializePreflopSelectorItemsTracking();

                PreflopSelectorItems.ForEach(x =>
                {
                    x.UsedCards = _notification.BoardCards;
                    x.IsEnabled = _notification.EquityCalculatorMode == EquityCalculatorMode.Holdem ||
                        (x.FisrtCard > RangeCardRank.Five && x.SecondCard > RangeCardRank.Five);

                    if (!x.IsEnabled)
                    {
                        x.IsSelected = false;
                    }
                });

                UpdateSlider();

                CombosRaisePropertyChanged();
                RaisePropertyChanged(nameof(IsReplayHandVisible));
            }
        }

        public System.Action FinishInteraction
        {
            get;
            set;
        }

        public ReactiveList<EquityRangeSelectorItemViewModel> PreflopSelectorItems
        {
            get
            {
                return _preflopSelectorItems;
            }
            set
            {
                SetProperty(ref _preflopSelectorItems, value);
            }
        }

        public List<RangeSelectorItemViewModel> TemporaryPreflopSelectorItems
        {
            get
            {
                return _temporaryPreflopSelectorItems;
            }
            set
            {
                SetProperty(ref _temporaryPreflopSelectorItems, value);
            }
        }


        public EquityRangeSelectorItemViewModel SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem != null && _selectedItem.IsMainInSequence)
                {
                    _selectedItem.IsMainInSequence = false;
                    _selectedItem.HandUpdate();
                }

                SetProperty(ref _selectedItem, value);

                RefreshSuits();
            }
        }

        public string SuitsForCaption
        {
            get { return _suitsForCaption; }
            set { SetProperty(ref _suitsForCaption, value); }
        }

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (_isSliderManualMove)
                {
                    Reset(null);

                    foreach (var hand in PreflopSelectorItems)
                    {
                        float deep_percentile = (float)_preflopRange.deepstack_percentile[hand.Caption];
                        float short_percentile = (float)_preflopRange.shortstack_percentile[hand.Caption];
                        float medium_percentile = (deep_percentile + short_percentile) / 2;

                        if (medium_percentile * 100 <= value / 10.0)
                        {
                            hand.IsSelected = true;
                            hand.HandUpdateAndRefresh();
                        }
                    }

                    UpdateSlider();
                }

                SetProperty(ref _sliderValue, value);
            }
        }

        public double SelectedPercentage
        {
            get
            {
                return _selectedPercentage;
            }
            set
            {
                if (_isSliderManualMove)
                {
                    SliderValue = (int)value * 10;
                }

                SetProperty(ref _selectedPercentage, value);
            }
        }

        private EquitySelectionMode? equitySelectionMode;

        public EquitySelectionMode? EquitySelectionMode
        {
            get
            {
                return equitySelectionMode;
            }
            set
            {
                if (value.HasValue && value.Value == equitySelectionMode)
                {
                    EquitySelectionMode = null;
                    return;
                }

                SetProperty(ref equitySelectionMode, value);
            }
        }

        public int FoldCheckCombos
        {
            get
            {
                return PreflopSelectorItems
                    .Sum(x => x.FoldCheckCombos);
            }
        }

        public decimal FoldCheckPercentage
        {
            get
            {
                return GetSpecificRangePercentage(DriveHUD.ViewModels.EquitySelectionMode.FoldCheck);
            }
        }

        public int CallCombos
        {
            get
            {
                return PreflopSelectorItems
                   .Sum(x => x.CallCombos);
            }
        }

        public decimal CallPercentage
        {
            get
            {
                return GetSpecificRangePercentage(DriveHUD.ViewModels.EquitySelectionMode.Call);
            }
        }

        public int BluffCombos
        {
            get
            {
                return PreflopSelectorItems
                   .Sum(x => x.BluffCombos);
            }
        }

        public decimal BluffPercentage
        {
            get
            {
                return GetSpecificRangePercentage(DriveHUD.ViewModels.EquitySelectionMode.Bluff);
            }
        }

        public int ValueBetCombos
        {
            get
            {
                return PreflopSelectorItems
                   .Sum(x => x.ValueBetCombos);
            }
        }

        public decimal ValueBetPercentage
        {
            get
            {
                return GetSpecificRangePercentage(DriveHUD.ViewModels.EquitySelectionMode.ValueBet);
            }
        }

        private ReactiveList<HandsStatisticViewModel> handStatistics;

        public ReactiveList<HandsStatisticViewModel> HandStatistics
        {
            get
            {
                return handStatistics;
            }
            private set
            {
                SetProperty(ref handStatistics, value);
            }
        }

        private bool isHelpVisible;

        public bool IsHelpVisible
        {
            get
            {
                return isHelpVisible;
            }
            set
            {
                SetProperty(ref isHelpVisible, value);
            }
        }

        public bool IsReplayHandVisible
        {
            get
            {
                return _notification != null && source != null &&
                    source.CurrentHandHistory != null && source.CurrentHandHistory.HandId != 0;
            }
        }

        #endregion

        #region ICommand

        public ICommand ShowCardsViewCommands { get; set; }

        public ICommand OnMouseLeftDownCommand { get; set; }

        public ICommand OnMouseRightDownCommand { get; set; }

        public ICommand OnDoubleClickCommand { get; set; }

        public ICommand OnCtrlClickCommand { get; set; }

        public ICommand OnAltClickCommand { get; set; }

        public ICommand OnMouseEnterCommand { get; set; }

        public ICommand ResetCommand { get; set; }

        public ICommand SaveCommand { get; set; }

        public ICommand ShowPredefinedRangesViewCommand { get; set; }

        public ICommand SelectAllPairsCommand { get; set; }

        public ICommand SelectSuitedCommand { get; set; }

        public ICommand SelectOffSuitedCommand { get; set; }

        public ICommand OnSelectSuitCommand { get; set; }

        public ICommand AutoRangeCommand { get; set; }

        public ICommand HelpCommand { get; set; }

        public ICommand ReplayCommand { get; set; }

        #endregion

        public PreflopSelectorViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            ShowCardsViewCommands = new RelayCommand(ShowCardsView);
            OnMouseLeftDownCommand = new RelayCommand(x => OnMouseDown(x));
            OnMouseRightDownCommand = new RelayCommand(x => OnMouseDown(x, true));
            OnDoubleClickCommand = new RelayCommand(OnDoubleClick);
            OnCtrlClickCommand = new RelayCommand(OnCtrlClick);
            OnAltClickCommand = new RelayCommand(OnAltClick);
            OnMouseEnterCommand = new RelayCommand(OnMouseEnter);
            ResetCommand = new RelayCommand(Reset);
            SaveCommand = new RelayCommand(Save);
            ShowPredefinedRangesViewCommand = new RelayCommand(ShowPredefinedRangesView);
            SelectAllPairsCommand = new RelayCommand(SelectAllPairs);
            SelectSuitedCommand = new RelayCommand(SelectedSuited);
            SelectOffSuitedCommand = new RelayCommand(SelectOffSuited);
            OnSelectSuitCommand = new RelayCommand(OnSelectSuit);
            AutoRangeCommand = new RelayCommand(OnAutoRange);
            HelpCommand = new RelayCommand(() => IsHelpVisible = !IsHelpVisible);
            ReplayCommand = new RelayCommand(() => ReplayHand());

            _preflopRange.Init();
            PreDefinedRangesRequest = new InteractionRequest<PreDefinedRangesNotifcation>();

            HandStatistics = new ReactiveList<HandsStatisticViewModel>(
                Enum.GetValues(typeof(MadeHandType)).OfType<MadeHandType>().Select(x => new HandsStatisticViewModel(x)));

            InitializePreflopSelectorItems();
        }

        private void OnAutoRange(object obj)
        {
            if (source == null || !(_notification.CardsContainer is PlayerModel playerModel))
            {
                return;
            }

            var heroAutoRange = source.GetHeroAutoRange(playerModel.PlayerName);

            if (heroAutoRange == null)
            {
                return;
            }

            var mergeResult = (from range in PreflopSelectorItems
                               join heroRange in heroAutoRange on range.Caption equals heroRange.Caption
                               select new { Existing = range, Auto = heroRange }).ToArray();

            PreflopSelectorItems.ChangeTrackingEnabled = false;

            PreflopSelectorItems.ForEach(x =>
            {
                x.IsSelected = false;
            });

            foreach (var mergeItem in mergeResult)
            {
                mergeItem.Existing.IsSelected = true;
                mergeItem.Existing.SetEquitySelectionMode(mergeItem.Auto.EquitySelectionMode);

                var handSuitsMergeResult = (from existingHandSuit in mergeItem.Existing.HandSuitsModelList
                                            join autoHandSuit in mergeItem.Auto.HandSuitsModelList on existingHandSuit.HandSuit equals autoHandSuit.HandSuit
                                            select new { Existing = existingHandSuit, Auto = autoHandSuit }).ToArray();

                handSuitsMergeResult.ForEach(x =>
                {
                    x.Existing.SelectionMode = x.Auto.SelectionMode;
                });

                mergeItem.Existing.HandUpdate();
            }

            PreflopSelectorItems.ChangeTrackingEnabled = true;

            UpdateSlider();
            CombosRaisePropertyChanged();
        }

        private void ReplayHand()
        {
            if (!IsReplayHandVisible || !(_notification.CardsContainer is PlayerModel playerModel))
            {
                return;
            }

            ServiceLocator.Current.GetInstance<IReplayerService>()
              .ReplayHand(playerModel.PlayerName, source.CurrentHandHistory.HandId, (short)source.CurrentHandHistory.GameDescription.Site, true);
        }

        private void InitializePreflopSelectorItems()
        {
            var rankValues = Enum.GetValues(typeof(RangeCardRank))
                .Cast<RangeCardRank>()
                .Where(x => x != RangeCardRank.None)
                .Reverse()
                .ToArray();

            for (var i = 0; i < rankValues.Length; i++)
            {
                var startS = false;

                for (var j = 0; j < rankValues.Length; j++)
                {
                    var card1 = i < j ? rankValues.ElementAt(i) : rankValues.ElementAt(j);
                    var card2 = i < j ? rankValues.ElementAt(j) : rankValues.ElementAt(i);

                    if (startS)
                    {
                        PreflopSelectorItems.Add(new EquityRangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.Suited));
                    }
                    else
                    {
                        if (!card1.Equals(card2))
                        {
                            PreflopSelectorItems.Add(new EquityRangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.OffSuited));
                        }
                        else
                        {
                            PreflopSelectorItems.Add(new EquityRangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.Pair));
                            startS = true;
                        }
                    }
                }
            }
        }

        private void InitializePreflopSelectorItemsTracking()
        {
            PreflopSelectorItems.ChangeTrackingEnabled = true;

            PreflopSelectorItems.ItemChanged.Subscribe(x =>
            {
                if (x.PropertyName == nameof(RangeSelectorItemViewModel.IsSelected) && x.Sender.IsSelected)
                {
                    x.Sender.SetEquitySelectionMode(EquitySelectionMode);
                }
                else if (x.PropertyName == nameof(EquityRangeSelectorItemViewModel.EquitySelectionMode))
                {
                    x.Sender.RefreshCombos();
                    CombosRaisePropertyChanged();
                }
            });
        }

        private void CombosRaisePropertyChanged()
        {
            RaisePropertyChanged(nameof(FoldCheckCombos));
            RaisePropertyChanged(nameof(CallCombos));
            RaisePropertyChanged(nameof(BluffCombos));
            RaisePropertyChanged(nameof(ValueBetCombos));
            RaisePropertyChanged(nameof(FoldCheckPercentage));
            RaisePropertyChanged(nameof(CallPercentage));
            RaisePropertyChanged(nameof(BluffPercentage));
            RaisePropertyChanged(nameof(ValueBetPercentage));
        }

        private void RefreshSuits()
        {
            if (SelectedItem == null)
            {
                SuitsForCaption = string.Empty;
                return;
            }

            SuitsForCaption = SelectedItem.Caption;
            SelectedItem.HandUpdateAndRefresh();
        }

        private void RefreshHandsStatistic()
        {
            var rangeItems = PreflopSelectorItems.Where(x => x.IsSelected).ToArray();

            var totalCombos = rangeItems.Sum(x => x.Combos);
            var ranges = rangeItems.Select(x => x.Caption).ToList();

            var combosByHandType = source.GetCombosByHandType(ranges);

            foreach (var handStatistic in HandStatistics)
            {
                if (combosByHandType.ContainsKey(handStatistic.HandType))
                {
                    handStatistic.Combos = combosByHandType[handStatistic.HandType];
                }
                else
                {
                    handStatistic.Combos = 0;
                }
            }

            HandStatistics.ForEach(x => x.TotalCombos = totalCombos);
        }

        private void UpdateSlider()
        {
            _isSliderManualMove = false;

            var combos = PreflopSelectorItems.Where(x => !x.IsMainInSequence).Sum(model => model.HandSuitsModelList.Count(x => x.IsVisible && x.IsSelected));

            var mainSequnceItem = PreflopSelectorItems.FirstOrDefault(x => x.IsMainInSequence);

            if (mainSequnceItem != null)
            {
                combos += mainSequnceItem.Combos;
            }

            double prct = Math.Round((double)combos * 100 / TotalPossibleCombos, 1);

            SliderValue = (int)prct * 10;
            SelectedPercentage = prct;

            _isSliderManualMove = true;

            RefreshHandsStatistic();
        }

        #region ICommand implementation

        private void OnSelectSuit(object obj)
        {
            if (!(obj is HandSuitsViewModel model))
            {
                return;
            }

            if (!model.IsVisible)
            {
                return;
            }

            model.IsSelected = EquitySelectionMode.HasValue && EquitySelectionMode != model.SelectionMode || !model.IsSelected;

            model.SelectionMode = EquitySelectionMode ?? DriveHUD.ViewModels.EquitySelectionMode.None;

            if (SelectedItem != null && SelectedItem.IsMainInSequence &&
                TemporaryPreflopSelectorItems != null && TemporaryPreflopSelectorItems.Count() > 0)
            {
                foreach (var item in TemporaryPreflopSelectorItems)
                {
                    if (item.HandSuitsModelList.Any(x => x.HandSuit == model.HandSuit && x.IsVisible))
                    {
                        var handSuitItem = item.HandSuitsModelList
                            .FirstOrDefault(x => x.HandSuit == model.HandSuit);

                        handSuitItem.IsSelected = model.IsSelected;
                        handSuitItem.SelectionMode = model.SelectionMode;

                        if (item is EquityRangeSelectorItemViewModel rangeItem)
                        {
                            rangeItem.RefreshCombos();
                        }
                    }
                }
            }

            SelectedItem?.RefreshCombos();
            CombosRaisePropertyChanged();

            UpdateSlider();
        }

        private void ShowCardsView(object obj)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
               .GetEvent<ChangeEquityCalculatorPopupViewEvent>()
               .Publish(new ChangeEquityCalculatorPopupViewEventArgs(ChangeEquityCalculatorPopupViewEventArgs
                   .ViewType
                   .CardsSelectorView));
        }

        private void SelectOffSuited(object obj)
        {
            for (int i = 1; i < RanksLength; i++)
            {
                var item = PreflopSelectorItems.ElementAt(i * RanksLength + i - 1);

                if (!item.IsEnabled)
                {
                    continue;
                }

                item.IsSelected = true;
                item.HandUpdateAndRefresh();
            }

            SelectedItem = new EquityRangeSelectorItemViewModel();

            UpdateSlider();
        }

        private void SelectedSuited(object obj)
        {
            for (int i = 0; i < RanksLength - 1; i++)
            {
                var item = PreflopSelectorItems.ElementAt(i * RanksLength + i + 1);

                if (!item.IsEnabled)
                {
                    continue;
                }

                item.IsSelected = true;
                item.HandUpdateAndRefresh();
            }

            SelectedItem = new EquityRangeSelectorItemViewModel();

            UpdateSlider();
        }

        private void SelectAllPairs(object obj)
        {
            for (int i = 0; i < RanksLength; i++)
            {
                var item = PreflopSelectorItems.ElementAt(i * RanksLength + i);

                if (!item.IsEnabled)
                {
                    continue;
                }

                item.IsSelected = true;
                item.HandUpdateAndRefresh();
            }

            SelectedItem = new EquityRangeSelectorItemViewModel();

            UpdateSlider();
        }

        private void ShowPredefinedRangesView(object obj)
        {
            PreDefinedRangesRequest.Raise(new PreDefinedRangesNotifcation() { Title = "Pre-Defined Ranges" },
                returned =>
                {
                    if (returned != null && returned.ItemsList != null)
                    {
                        Reset(null);

                        foreach (var s in returned.ItemsList)
                        {
                            var current = PreflopSelectorItems.FirstOrDefault(x => x.Caption.Equals(s));

                            if (current != null && current.IsEnabled)
                            {
                                current.IsSelected = true;
                                current.HandUpdateAndRefresh();
                            }
                        }

                        UpdateSlider();
                    }
                });
        }

        private void OnMouseDown(object obj, bool isRight = false)
        {
            if (!(obj is EquityRangeSelectorItemViewModel item))
            {
                return;
            }

            SelectedItem = item;

            if (!item.IsSelected)
            {
                item.IsSelected = true;
            }
            else if (!isRight)
            {
                if (EquitySelectionMode.HasValue || !item.EquitySelectionMode.HasValue)
                {
                    item.SetEquitySelectionMode(EquitySelectionMode);
                }

                SelectedItem.HandRefreshVisibilityCheck();
            }

            UpdateSlider();
        }

        private void OnDoubleClick(object obj)
        {
            if (!(obj is EquityRangeSelectorItemViewModel item))
            {
                return;
            }

            item.IsSelected = false;

            SelectedItem = new EquityRangeSelectorItemViewModel();
            UpdateSlider();
        }

        private void OnCtrlClick(object obj)
        {
            if (!(obj is EquityRangeSelectorItemViewModel item))
            {
                return;
            }

            int firstCardIndex = RanksLength - (int)item.FisrtCard - 1;
            int secondCardIndex = RanksLength - (int)item.SecondCard - 1;

            TemporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();

            for (int i = 0; i < RanksLength; i++)
            {
                if (item.ItemType.Equals(RangeSelectorItemType.Suited))
                {
                    TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * RanksLength + secondCardIndex));
                    TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(firstCardIndex * RanksLength + i));
                }
                else
                {
                    TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * RanksLength + firstCardIndex));
                    TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(secondCardIndex * RanksLength + i));
                }
            }

            TemporaryPreflopSelectorItems.ForEach(x =>
            {
                x.IsSelected = true;
                x.HandUpdateAndRefresh();
            });

            SelectedItem = item;
            SelectedItem.IsMainInSequence = true;
            HandSuitsViewModel.SetAllVisible(SelectedItem.HandSuitsModelList);
            SuitsForCaption = string.Join(",", TemporaryPreflopSelectorItems.Select(x => x.Caption));

            UpdateSlider();
        }

        private void OnAltClick(object obj)
        {
            if (!(obj is EquityRangeSelectorItemViewModel item))
            {
                return;
            }

            int firstCardIndex = RanksLength - (int)item.FisrtCard - 1;

            TemporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();

            int i = item.ItemType.Equals(RangeSelectorItemType.Pair) ? 0 : firstCardIndex + 1;

            for (; i < RanksLength; i++)
            {
                switch (item.ItemType)
                {
                    case RangeSelectorItemType.Suited:
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(firstCardIndex * RanksLength + i));
                        break;
                    case RangeSelectorItemType.OffSuited:
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * RanksLength + firstCardIndex));
                        break;
                    case RangeSelectorItemType.Pair:
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * RanksLength + i));
                        break;
                }
                if (TemporaryPreflopSelectorItems.LastOrDefault() != null)
                {
                    if (TemporaryPreflopSelectorItems.LastOrDefault() == item)
                        break;
                }
            }

            TemporaryPreflopSelectorItems.ForEach(x =>
            {
                x.IsSelected = true;
                x.HandUpdateAndRefresh();
            });

            SelectedItem = item;
            SelectedItem.IsMainInSequence = true;
            SuitsForCaption = string.Join(",", this.TemporaryPreflopSelectorItems.Select(x => x.Caption));

            UpdateSlider();
        }

        private void OnMouseEnter(object obj)
        {
            /* mouse down is handled in the backend of the page */
            if (!(obj is RangeSelectorItemViewModel item))
            {
                return;
            }

            item.IsSelected = true;
            item.HandUpdateAndRefresh();
            UpdateSlider();
        }

        private void Reset(object obj)
        {
            foreach (var item in PreflopSelectorItems.Where(x => x.IsSelected))
            {
                item.IsSelected = false;
            }

            if (SelectedItem != null)
            {
                SelectedItem.IsMainInSequence = false;
                SelectedItem = null;
            }

            UpdateSlider();
        }

        private void Save(object obj)
        {
            if (SelectedItem != null && SelectedItem.IsMainInSequence)
            {
                //update suites
                SelectedItem = new EquityRangeSelectorItemViewModel();
            }

            _notification.CardsContainer.Ranges = new List<EquityRangeSelectorItemViewModel>(PreflopSelectorItems.Where(x => x.IsSelected));
            _notification.Confirmed = true;

            FinishInteraction();
        }

        private decimal GetSpecificRangePercentage(EquitySelectionMode equitySelectionMode)
        {
            var totalCombos = PreflopSelectorItems
                    .Where(x => x.EquitySelectionMode.HasValue)
                    .Sum(x => x.Combos);

            var rangeCombos = 0;

            switch (equitySelectionMode)
            {
                case DriveHUD.ViewModels.EquitySelectionMode.FoldCheck:
                    rangeCombos = FoldCheckCombos;
                    break;
                case DriveHUD.ViewModels.EquitySelectionMode.Bluff:
                    rangeCombos = BluffCombos;
                    break;
                case DriveHUD.ViewModels.EquitySelectionMode.Call:
                    rangeCombos = CallCombos;
                    break;
                case DriveHUD.ViewModels.EquitySelectionMode.ValueBet:
                    rangeCombos = ValueBetCombos;
                    break;
            }

            return rangeCombos == totalCombos || totalCombos == 0 ?
                (decimal)rangeCombos / TotalPossibleCombos :
                (decimal)rangeCombos / totalCombos;
        }

        #endregion
    }

    public class HandsStatisticViewModel : BindableBase
    {
        public HandsStatisticViewModel(MadeHandType handType)
        {
            this.handType = handType;
        }

        private MadeHandType handType;

        public MadeHandType HandType
        {
            get
            {
                return handType;
            }
        }

        private int combos;

        public int Combos
        {
            get
            {
                return combos;
            }
            set
            {
                SetProperty(ref combos, value);
                RaisePropertyChanged(nameof(IsVisible));
            }
        }

        private int totalCombos;

        public int TotalCombos
        {
            get
            {
                return totalCombos;
            }
            set
            {
                SetProperty(ref totalCombos, value);
                RaisePropertyChanged(nameof(Percentage));
            }
        }

        public double Percentage
        {
            get
            {
                return totalCombos != 0 ? combos / (double)totalCombos : 0;
            }
        }

        public bool IsVisible
        {
            get
            {
                return combos != 0;
            }
        }
    }
}