using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.EquityCalculator.Models;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class CardSelectorViewModel : BaseViewModel, IInteractionRequestAware
    {
        private readonly int suitLenght = 13;

        #region fields
        private CardSelectorNotification _notification;
        private CardSelectorType _selectorType = CardSelectorType.BoardSelector;
        private List<CardModel> _selectedCards = new List<CardModel>();
        private bool _isSelectError = false;
        private bool _isOneCard = false;

        private List<CheckableCardModel> _clubsSource = new List<CheckableCardModel>();
        private List<CheckableCardModel> _diamondsSource = new List<CheckableCardModel>();
        private List<CheckableCardModel> _heartsSource = new List<CheckableCardModel>();
        private List<CheckableCardModel> _spadesSource = new List<CheckableCardModel>();

        private List<CardModel> _usedCards = new List<CardModel>();
        #endregion

        #region Properties
        public List<CheckableCardModel> ClubsSource
        {
            get
            {
                return _clubsSource;
            }

            set
            {
                _clubsSource = value;
            }
        }
        public List<CheckableCardModel> DiamondsSource
        {
            get
            {
                return _diamondsSource;
            }

            set
            {
                _diamondsSource = value;
            }
        }
        public List<CheckableCardModel> HeartsSource
        {
            get
            {
                return _heartsSource;
            }

            set
            {
                _heartsSource = value;
            }
        }
        public List<CheckableCardModel> SpadesSource
        {
            get
            {
                return _spadesSource;
            }

            set
            {
                _spadesSource = value;
            }
        }

        public List<CardModel> UsedCards
        {
            get
            {
                return _usedCards;
            }
            set
            {
                _usedCards = value;
            }
        }

        public CardSelectorType SelectorType
        {
            get
            {
                return _selectorType;
            }

            set
            {
                SetProperty(ref _selectorType, value);
            }
        }

        public bool IsSelectError
        {
            get
            {
                return _isSelectError;
            }

            set
            {
                SetProperty(ref _isSelectError, value);
            }
        }

        public bool IsOneCard
        {
            get
            {
                return _isOneCard;
            }
            set
            {
                SetProperty(ref _isOneCard, value);
            }
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get
            {
                return this._notification;
            }
            set
            {
                if (value is CardSelectorNotification)
                {
                    _notification = value as CardSelectorNotification;
                    SelectorType = this._notification.SelectorType;
                    UsedCards = new List<CardModel>(this._notification.UsedCards);
                    SetSelectedCards();
                    RaisePropertyChanged();
                }
            }
        }
        #endregion

        #region ICommand
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        public ICommand ShowPreflopViewCommand { get; set; }
        #endregion

        public CardSelectorViewModel()
        {
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
            SaveCommand = new RelayCommand(Save);
            ResetCommand = new RelayCommand(Reset);
            ShowPreflopViewCommand = new RelayCommand(ShowPreflopView);

            Init();
        }

        private void ShowPreflopView(object obj)
        {
            ServiceLocator.Current.GetInstance<IEventAggregator>()
                .GetEvent<ChangeEquityCalculatorPopupViewEvent>()
                .Publish(new ChangeEquityCalculatorPopupViewEventArgs(ChangeEquityCalculatorPopupViewEventArgs
                    .ViewType
                    .PreflopSelectorView));
        }

        #region Class Members
        private void Init()
        {
            for (int i = suitLenght - 1; i >= 0; i--)
            {
                ClubsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Clubs));
                DiamondsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Diamonds));
                HeartsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Hearts));
                SpadesSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Spades));
            }
        }

        private void ResetView()
        {
            IsOneCard = false;
            IsSelectError = false;
            _selectedCards.Clear();
            ClubsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            DiamondsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            HeartsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            SpadesSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
        }

        private void SetSelectedCards()
        {
            ResetView();
            foreach (var card in this._notification.CardsContainer.Cards.Where(x => (x.Rank != RangeCardRank.None) && (x.Suit != RangeCardSuit.None)))
            {
                switch (card.Suit)
                {
                    case RangeCardSuit.Clubs:
                        ClubsSource.First(x => x.Rank == card.Rank).IsChecked = true;
                        break;
                    case RangeCardSuit.Diamonds:
                        DiamondsSource.First(x => x.Rank == card.Rank).IsChecked = true;
                        break;
                    case RangeCardSuit.Hearts:
                        HeartsSource.First(x => x.Rank == card.Rank).IsChecked = true;
                        break;
                    case RangeCardSuit.Spades:
                        SpadesSource.First(x => x.Rank == card.Rank).IsChecked = true;
                        break;
                }
                _selectedCards.Add(card);
            }
        }
        #endregion

        #region ICommand implementation
        private void SelectionChanged(object obj)
        {
            IsOneCard = false;
            if (obj != null)
            {
                if (obj is CheckableCardModel)
                {
                    var card = obj as CheckableCardModel;
                    CheckableCardModel selectedCard = null;
                    switch (card.Suit)
                    {
                        case RangeCardSuit.Clubs:
                            selectedCard = ClubsSource.FirstOrDefault(x => x.Rank == card.Rank);
                            break;
                        case RangeCardSuit.Diamonds:
                            selectedCard = DiamondsSource.FirstOrDefault(x => x.Rank == card.Rank);
                            break;
                        case RangeCardSuit.Hearts:
                            selectedCard = HeartsSource.FirstOrDefault(x => x.Rank == card.Rank);
                            break;
                        case RangeCardSuit.Spades:
                            selectedCard = SpadesSource.FirstOrDefault(x => x.Rank == card.Rank);
                            break;
                    }

                    if (selectedCard != null)
                    {
                        if (selectedCard.IsChecked)
                        {
                            selectedCard.IsChecked = false;
                            var c = _selectedCards
                                .FirstOrDefault(x => x.Suit == selectedCard.Suit && x.Rank == selectedCard.Rank);
                            if (c != null)
                            {
                                _selectedCards.Remove(c);
                            }
                        }
                        else
                        {
                            if (_selectedCards.Count < this._notification.CardsContainer.ContainerSize)
                            {
                                if (UsedCards.Any(x => x.Rank == selectedCard.Rank && x.Suit == selectedCard.Suit))
                                {
                                    IsSelectError = true;
                                }
                                else
                                {
                                    IsSelectError = false;
                                    selectedCard.IsChecked = true;
                                    _selectedCards.Add(new CardModel(selectedCard.Rank, selectedCard.Suit));
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Save(object obj)
        {
            IsSelectError = false;
            if ((_selectedCards.Count == 1) && (SelectorType.Equals(CardSelectorType.PlayerSelector)))
            {
                IsOneCard = true;
                return;
            }
            this._notification.ReturnType = CardSelectorReturnType.Cards;
            switch (SelectorType)
            {
                case CardSelectorType.BoardSelector:
                    this._notification.CardsContainer = new BoardModel(_selectedCards.Select(x => new CardModel(x.Rank, x.Suit)).ToList());
                    break;
                case CardSelectorType.PlayerSelector:
                    this._notification.CardsContainer = new PlayerModel(_selectedCards.Select(x => new CardModel(x.Rank, x.Suit)).ToList());
                    break;
            }
            this.FinishInteraction();
        }

        private void Reset(object obj)
        {
            ResetView();
        }
        #endregion
    }
}
