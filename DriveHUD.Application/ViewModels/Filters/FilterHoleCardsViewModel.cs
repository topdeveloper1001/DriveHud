using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.ViewModels;
using HandHistories.Objects.Cards;
using Model;
using Model.Enums;
using Model.Filters;
using Model.LocalCalculator;
using Model.Notifications;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterHoleCardsViewModel : FilterViewModel<FilterHoleCardsModel>
    {

        #region Fields
        private int _sliderValue = 0;
        private bool _isSliderManualMove = true;
        private double _selectedPercentage = 0;
        private PreflopRange _preflopRange = new PreflopRange();
        #endregion

        #region Properties
        public InteractionRequest<PreDefinedRangesNotifcation> PreDefinedRangesRequest { get; private set; }

        public int SliderValue
        {
            get { return _sliderValue; }
            set
            {
                if (_isSliderManualMove)
                {
                    SelectNone(null);

                    foreach (var hand in this.FilterModel.HoleCardsCollection)
                    {
                        float deep_percentile = (float)_preflopRange.deepstack_percentile[hand.Name];
                        float short_percentile = (float)_preflopRange.shortstack_percentile[hand.Name];
                        float medium_percentile = (deep_percentile + short_percentile) / 2;

                        if (medium_percentile * 100 <= value / 10.0)
                        {
                            hand.IsChecked = true;
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

        #endregion

        internal FilterHoleCardsViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterHoleCardsViewModel, service)
        {
            Init();
        }

        #region ICommand
        public ICommand ShowPredefinedRangesViewCommand { get; set; }
        public ICommand OnClickCommand { get; set; }
        public ICommand OnDoubleClickCommand { get; set; }
        public ICommand OnCtrlClickCommand { get; set; }
        public ICommand OnAltClickCommand { get; set; }
        public ICommand OnMouseEnterCommand { get; set; }
        public ICommand SelectAllPairsCommand { get; set; }
        public ICommand SelectSuitedCommand { get; set; }
        public ICommand SelectOffSuitedCommand { get; set; }
        public ICommand SelectSuited1GappersCommand { get; set; }
        public ICommand SelectOffSuited1GappersCommand { get; set; }
        public ICommand SelectTop_4_7_Command { get; set; }
        public ICommand SelectTop_19_5_Command { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand SelectNoneCommand { get; set; }
        #endregion

        private void Init()
        {
            ShowPredefinedRangesViewCommand = new RelayCommand(ShowPredefinedRangesView);
            OnClickCommand = new RelayCommand(OnClick);
            OnDoubleClickCommand = new RelayCommand(OnDoubleClick);
            OnCtrlClickCommand = new RelayCommand(OnCtrlClick);
            OnAltClickCommand = new RelayCommand(OnAltClick);
            OnMouseEnterCommand = new RelayCommand(OnMouseEnter);
            SelectAllPairsCommand = new RelayCommand(SeleactAllPairs);
            SelectSuitedCommand = new RelayCommand(SelectedSuited);
            SelectOffSuitedCommand = new RelayCommand(SelectOffSuited);
            SelectSuited1GappersCommand = new RelayCommand(SelectSuited1Gappers);
            SelectOffSuited1GappersCommand = new RelayCommand(SelectOffSuited1Gappers);
            SelectTop_4_7_Command = new RelayCommand(SelectTop_4_7);
            SelectTop_19_5_Command = new RelayCommand(SelectTop_19_5);
            ResetCommand = new RelayCommand(Reset);
            SelectNoneCommand = new RelayCommand(SelectNone);

            _preflopRange.Init();
            PreDefinedRangesRequest = new InteractionRequest<PreDefinedRangesNotifcation>();
        }

        private int GetRanksLength()
        {
            return Card.PossibleRanksHighCardFirst.Count();
        }

        private void UpdateSlider()
        {
            _isSliderManualMove = false;

            int i = this.FilterModel.HoleCardsCollection.Where(x => x.IsChecked).Sum(x => x.GetSuitsCount());

            double prct = Math.Round((double)i * (double)100 / (double)1326, 1);
            SliderValue = (int)prct * 10;
            SelectedPercentage = prct;

            _isSliderManualMove = true;
        }

        #region ICommand implementation

        private void ShowPredefinedRangesView(object obj)
        {
            PreDefinedRangesRequest.Raise(new PreDefinedRangesNotifcation() { Title = "Pre-Defined Ranges" },
                returned =>
                {
                    if (returned != null && returned.ItemsList != null)
                    {
                        SelectNone(null);
                        foreach (var s in returned.ItemsList)
                        {
                            var current = FilterModel.HoleCardsCollection.FirstOrDefault(x => x.Name.Equals(s));
                            if (current != null)
                            {
                                current.IsChecked = true;
                            }
                        }
                        UpdateSlider();
                    }
                });
        }

        private void SelectTop_4_7(object obj)
        {
            SelectNone(null);
            var list = PreDefinedRangeModel.GetTop_4_7_Percent_Range();
            foreach(var item in FilterModel.HoleCardsCollection.Where(x=> list.Ranges.First().Value.Key.Any(l => l == x.Name)))
            {
                item.IsChecked = true;
            }

            UpdateSlider();
        }

        private void SelectTop_19_5(object obj)
        {
            SelectNone(null);
            var list = PreDefinedRangeModel.GetTop_19_5_Percent_Range();
            foreach (var item in FilterModel.HoleCardsCollection.Where(x => list.Ranges.First().Value.Key.Any(l => l == x.Name)))
            {
                item.IsChecked = true;
            }

            UpdateSlider();
        }

        private void SelectSuited1Gappers(object obj)
        {
            int length = GetRanksLength();

            for (int i = 0; i < length - 2; i++)
            {
                FilterModel.HoleCardsCollection.ElementAt(i * length + i + 2).IsChecked = true;
            }

            UpdateSlider();
        }

        private void SelectOffSuited1Gappers(object obj)
        {
            int length = GetRanksLength();

            for (int i = 2; i < length; i++)
            {
                FilterModel.HoleCardsCollection.ElementAt(i * length + i - 2).IsChecked = true;
            }

            UpdateSlider();
        }

        private void SelectOffSuited(object obj)
        {
            int length = GetRanksLength();

            for (int i = 1; i < length; i++)
            {
                FilterModel.HoleCardsCollection.ElementAt(i * length + i - 1).IsChecked = true;
            }

            UpdateSlider();
        }

        private void SelectedSuited(object obj)
        {
            int length = GetRanksLength();

            for (int i = 0; i < length - 1; i++)
            {
                FilterModel.HoleCardsCollection.ElementAt(i * length + i + 1).IsChecked = true;
            }
            UpdateSlider();
        }

        private void SeleactAllPairs(object obj)
        {
            int length = GetRanksLength();

            for (int i = 0; i < length; i++)
            {
                FilterModel.HoleCardsCollection.ElementAt(i * length + i).IsChecked = true;
            }
            UpdateSlider();
        }

        private void OnClick(object obj)
        {
            if (obj is HoleCardsItem)
            {
                var item = obj as HoleCardsItem;
                if (!item.IsChecked)
                {
                    item.IsChecked = true;
                }
                UpdateSlider();
            }
        }

        private void OnDoubleClick(object obj)
        {
            if (obj is HoleCardsItem)
            {
                var item = obj as HoleCardsItem;
                item.IsChecked = false;
                UpdateSlider();
            }
        }

        private void OnCtrlClick(object obj)
        {
            if (obj is HoleCardsItem)
            {
                var item = obj as HoleCardsItem;

                int length = GetRanksLength();
                int firstCardIndex = length - item.GetFirstCardRangeIndex() - 1;
                int secondCardIndex = length - item.GetSecondCardRangeIndex() - 1;

                for (int i = 0; i < length; i++)
                {
                    if (item.ItemType.Equals(RangeSelectorItemType.Suited))
                    {
                        FilterModel.HoleCardsCollection.ElementAt(i * length + secondCardIndex).IsChecked = true;
                        FilterModel.HoleCardsCollection.ElementAt(firstCardIndex * length + i).IsChecked = true;
                    }
                    else
                    {
                        FilterModel.HoleCardsCollection.ElementAt(i * length + firstCardIndex).IsChecked = true;
                        FilterModel.HoleCardsCollection.ElementAt(secondCardIndex * length + i).IsChecked = true;
                    }
                }

                UpdateSlider();
            }
        }

        private void OnAltClick(object obj)
        {
            if (obj is HoleCardsItem)
            {
                var item = obj as HoleCardsItem;

                int length = GetRanksLength();
                int firstCardIndex = length - item.GetFirstCardRangeIndex() - 1;


                int i = item.ItemType.Equals(RangeSelectorItemType.Pair) ? 0 : firstCardIndex + 1;

                for (; i < length; i++)
                {
                    HoleCardsItem itemToSelect  = new HoleCardsItem();
                    switch (item.ItemType)
                    {
                        case RangeSelectorItemType.Suited:
                            itemToSelect = FilterModel.HoleCardsCollection.ElementAt(firstCardIndex * length + i);
                            break;
                        case RangeSelectorItemType.OffSuited:
                            itemToSelect = FilterModel.HoleCardsCollection.ElementAt(i * length + firstCardIndex);
                            break;
                        case RangeSelectorItemType.Pair:
                            itemToSelect = FilterModel.HoleCardsCollection.ElementAt(i * length + i);
                            break;
                    }
                    if(FilterModel.HoleCardsCollection.Contains(itemToSelect))
                    {
                        itemToSelect.IsChecked = true;
                        if(itemToSelect == item)
                        {
                            break;
                        }
                    }
                }

                UpdateSlider();
            }
        }

        private void OnMouseEnter(object obj)
        {
            /* mouse down is handled in the backend of the page */
            if (obj is HoleCardsItem)
            {
                var item = obj as HoleCardsItem;
                item.IsChecked = true;
                UpdateSlider();
            }
        }

        private void Reset(object obj)
        {
            foreach(var item in FilterModel.HoleCardsCollection.Where(x=> !x.IsChecked))
            {
                item.IsChecked = true;
            }
            UpdateSlider();
        }

        private void SelectNone(object obj)
        {
            foreach (var item in FilterModel.HoleCardsCollection.Where(x => x.IsChecked))
            {
                item.IsChecked = false;
            }
            UpdateSlider();
        }

        #endregion
    }
}
