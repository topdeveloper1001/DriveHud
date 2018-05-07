//-----------------------------------------------------------------------
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
using DriveHUD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.LocalCalculator;
using Model.Notifications;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class PreflopSelectorViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields

        private CardSelectorNotification _notification;
        private RangeSelectorItemViewModel _selectedItem = new RangeSelectorItemViewModel();
        private ObservableCollection<RangeSelectorItemViewModel> _preflopSelectorItems = new ObservableCollection<RangeSelectorItemViewModel>();
        private List<RangeSelectorItemViewModel> _temporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();
        private string _suitsForCaption = string.Empty;
        private int _sliderValue = 0;
        private bool _isSliderManualMove = true;
        private double _selectedPercentage = 0;
        private PreflopRange _preflopRange = new PreflopRange();

        #endregion

        #region Properties

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

                    if (_notification.CardsContainer.Ranges != null)
                    {
                        foreach (var item in _notification.CardsContainer.Ranges)
                        {
                            var current = PreflopSelectorItems.FirstOrDefault(x => x.Caption == item.Caption);

                            if (current != null)
                            {
                                current.IsSelected = true;
                                current.HandSuitsModelList = new List<HandSuitsViewModel>(item.HandSuitsModelList);
                                current.ItemLikelihood = item.ItemLikelihood;
                                current.LikelihoodPercent = item.LikelihoodPercent;
                            }
                        }
                    }
                }
            }
        }

        public Action FinishInteraction
        {
            get;
            set;
        }

        public ObservableCollection<RangeSelectorItemViewModel> PreflopSelectorItems
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


        public RangeSelectorItemViewModel SelectedItem
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

                    SelectedPercentage = value / 10.0;
                }

                SetProperty(ref _sliderValue, value);
            }
        }

        public double SelectedPercentage
        {
            get { return _selectedPercentage; }
            set { SetProperty(ref _selectedPercentage, value); }
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

        #endregion

        #region ICommand
        public ICommand ShowCardsViewCommands { get; set; }
        public ICommand OnMouseDownCommand { get; set; }
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
        #endregion

        public PreflopSelectorViewModel()
        {
            Init();

            InitPreflopSelectorItems();
        }

        private void Init()
        {
            ShowCardsViewCommands = new RelayCommand(ShowCardsView);
            OnMouseDownCommand = new RelayCommand(OnMouseDown);
            OnDoubleClickCommand = new RelayCommand(OnDoubleClick);
            OnCtrlClickCommand = new RelayCommand(OnCtrlClick);
            OnAltClickCommand = new RelayCommand(OnAltClick);
            OnMouseEnterCommand = new RelayCommand(OnMouseEnter);
            ResetCommand = new RelayCommand(Reset);
            SaveCommand = new RelayCommand(Save);
            ShowPredefinedRangesViewCommand = new RelayCommand(ShowPredefinedRangesView);
            SelectAllPairsCommand = new RelayCommand(SeleactAllPairs);
            SelectSuitedCommand = new RelayCommand(SelectedSuited);
            SelectOffSuitedCommand = new RelayCommand(SelectOffSuited);
            OnSelectSuitCommand = new RelayCommand(OnSelectSuit);

            _preflopRange.Init();
            PreDefinedRangesRequest = new InteractionRequest<PreDefinedRangesNotifcation>();
        }

        private void InitPreflopSelectorItems()
        {
            var rankValues = Enum.GetValues(typeof(RangeCardRank)).Cast<RangeCardRank>().Where(x => x != RangeCardRank.None).Reverse();

            for (int i = 0; i < rankValues.Count(); i++)
            {
                bool startS = false;

                for (int j = 0; j < rankValues.Count(); j++)
                {
                    RangeCardRank card1 = i < j ? rankValues.ElementAt(i) : rankValues.ElementAt(j);
                    RangeCardRank card2 = i < j ? rankValues.ElementAt(j) : rankValues.ElementAt(i);

                    if (startS)
                    {
                        PreflopSelectorItems.Add(new RangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.Suited));
                    }
                    else
                    {
                        if (!card1.Equals(card2))
                        {
                            PreflopSelectorItems.Add(new RangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.OffSuited));
                        }
                        else
                        {
                            PreflopSelectorItems.Add(new RangeSelectorItemViewModel(card1, card2, RangeSelectorItemType.Pair));
                            startS = true;
                        }
                    }
                }
            }
        }

        private int GetRanksLength()
        {
            return Enum.GetValues(typeof(RangeCardRank)).Cast<RangeCardRank>().Where(x => x != RangeCardRank.None).Count();
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

        private void UpdateSlider()
        {
            _isSliderManualMove = false;

            int i = PreflopSelectorItems.Where(x => x.IsSelected).Sum(model => model.HandSuitsModelList.Count(x => x.IsVisible && x.IsSelected));

            double prct = Math.Round((double)i * (double)100 / (double)1326, 1);
            SliderValue = (int)prct * 10;
            SelectedPercentage = prct;

            _isSliderManualMove = true;
        }

        #region ICommand implementation

        private void OnSelectSuit(object obj)
        {
            if (obj is HandSuitsViewModel)
            {
                var model = obj as HandSuitsViewModel;

                if (!model.IsVisible)
                {
                    return;
                }

                model.IsSelected = !model.IsSelected;
                if (SelectedItem != null && SelectedItem.IsMainInSequence)
                {
                    if (TemporaryPreflopSelectorItems != null
                        && TemporaryPreflopSelectorItems.Count() > 0)
                    {
                        foreach (var item in this.TemporaryPreflopSelectorItems)
                        {
                            if (item.HandSuitsModelList.Any(x => x.HandSuit == model.HandSuit))
                            {
                                item.HandSuitsModelList
                                    .FirstOrDefault(x => x.HandSuit == model.HandSuit)
                                    .IsSelected = model.IsSelected;
                            }
                        }
                    }
                }

                UpdateSlider();
            }
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
            int length = GetRanksLength();

            for (int i = 1; i < length; i++)
            {
                PreflopSelectorItems.ElementAt(i * length + i - 1).IsSelected = true;
                PreflopSelectorItems.ElementAt(i * length + i - 1).HandUpdateAndRefresh();
            }
            SelectedItem = new RangeSelectorItemViewModel();
            RefreshSuits();
        }

        private void SelectedSuited(object obj)
        {
            int length = GetRanksLength();

            for (int i = 0; i < length - 1; i++)
            {
                PreflopSelectorItems.ElementAt(i * length + i + 1).IsSelected = true;
                PreflopSelectorItems.ElementAt(i * length + i + 1).HandUpdateAndRefresh();
            }
            SelectedItem = new RangeSelectorItemViewModel();
            RefreshSuits();
        }

        private void SeleactAllPairs(object obj)
        {
            int length = GetRanksLength();

            for (int i = 0; i < length; i++)
            {
                PreflopSelectorItems.ElementAt(i * length + i).IsSelected = true;
                PreflopSelectorItems.ElementAt(i * length + i).HandUpdateAndRefresh();
            }
            SelectedItem = new RangeSelectorItemViewModel();
            RefreshSuits();
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
                            if (current != null)
                            {
                                current.IsSelected = true;
                                current.HandUpdateAndRefresh();
                            }
                        }
                    }
                });
        }

        private void OnMouseDown(object obj)
        {
            if (obj is RangeSelectorItemViewModel)
            {
                var item = obj as RangeSelectorItemViewModel;
                SelectedItem = item;
                if (!item.IsSelected)
                {
                    item.IsSelected = true;
                    RefreshSuits();
                }
                else
                {
                    SelectedItem.HandRefreshVisibilityCheck();
                }
                UpdateSlider();
            }
        }

        private void OnDoubleClick(object obj)
        {
            if (obj is RangeSelectorItemViewModel)
            {
                var item = obj as RangeSelectorItemViewModel;
                item.IsSelected = false;
                SelectedItem = new RangeSelectorItemViewModel();
                RefreshSuits();
                UpdateSlider();
            }
        }

        private void OnCtrlClick(object obj)
        {
            if (obj is RangeSelectorItemViewModel)
            {
                var item = obj as RangeSelectorItemViewModel;

                int length = GetRanksLength();
                int firstCardIndex = length - (int)item.FisrtCard - 1;
                int secondCardIndex = length - (int)item.SecondCard - 1;

                TemporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();

                for (int i = 0; i < length; i++)
                {
                    if (item.ItemType.Equals(RangeSelectorItemType.Suited))
                    {
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * length + secondCardIndex));
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(firstCardIndex * length + i));
                    }
                    else
                    {
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * length + firstCardIndex));
                        TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(secondCardIndex * length + i));
                    }
                }

                TemporaryPreflopSelectorItems.ForEach(x =>
                {
                    x.IsSelected = true;
                    x.HandUpdateAndRefresh();
                });

                SelectedItem = item;
                SelectedItem.IsMainInSequence = true;
                HandSuitsViewModel.SetAllVisible(this.SelectedItem.HandSuitsModelList);
                SuitsForCaption = string.Join(",", this.TemporaryPreflopSelectorItems.Select(x => x.Caption));
            }
        }

        private void OnAltClick(object obj)
        {
            if (obj is RangeSelectorItemViewModel)
            {
                var item = obj as RangeSelectorItemViewModel;

                int length = GetRanksLength();
                int firstCardIndex = length - (int)item.FisrtCard - 1;

                TemporaryPreflopSelectorItems = new List<RangeSelectorItemViewModel>();

                int i = item.ItemType.Equals(RangeSelectorItemType.Pair) ? 0 : firstCardIndex + 1;

                for (; i < length; i++)
                {
                    switch (item.ItemType)
                    {
                        case RangeSelectorItemType.Suited:
                            TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(firstCardIndex * length + i));
                            break;
                        case RangeSelectorItemType.OffSuited:
                            TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * length + firstCardIndex));
                            break;
                        case RangeSelectorItemType.Pair:
                            TemporaryPreflopSelectorItems.Add(PreflopSelectorItems.ElementAt(i * length + i));
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
            }
        }

        private void OnMouseEnter(object obj)
        {
            /* mouse down is handled in the backend of the page */
            if (obj is RangeSelectorItemViewModel)
            {
                var item = obj as RangeSelectorItemViewModel;
                item.IsSelected = true;
                item.HandUpdateAndRefresh();
                UpdateSlider();
            }
        }

        private void Reset(object obj)
        {
            foreach (var item in PreflopSelectorItems.Where(x => x.IsSelected))
            {
                item.IsSelected = false;
            }
            SelectedItem = new RangeSelectorItemViewModel();
            RefreshSuits();
        }

        private void Save(object obj)
        {
            if (SelectedItem.IsMainInSequence)
            {
                //update suites
                SelectedItem = new RangeSelectorItemViewModel();
            }

            _notification.CardsContainer.Ranges = new List<RangeSelectorItemViewModel>(PreflopSelectorItems.Where(x => x.IsSelected));
            FinishInteraction();
        }

        #endregion
    }
}