using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.EquityCalculator.Models;
using Model.Enums;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Model.Filters;
using DriveHUD.Common.Linq;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerCardSelectorViewModel : BaseViewModel, IInteractionRequestAware
    {
        private const int SUIT_LENGTH = 13;

        #region Properties
        private PopupContainerCardSelectorViewModelNotification _notification;
        private ObservableCollection<CheckableCardModel> _clubsSource = new ObservableCollection<CheckableCardModel>();
        private ObservableCollection<CheckableCardModel> _diamondsSource = new ObservableCollection<CheckableCardModel>();
        private ObservableCollection<CheckableCardModel> _heartsSource = new ObservableCollection<CheckableCardModel>();
        private ObservableCollection<CheckableCardModel> _spadesSource = new ObservableCollection<CheckableCardModel>();

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return _notification; }
            set
            {
                if (value is PopupContainerCardSelectorViewModelNotification)
                {
                    _notification = (PopupContainerCardSelectorViewModelNotification)value;
                    SetCards(_notification.CardCollection);
                }
            }
        }

        public ObservableCollection<CheckableCardModel> ClubsSource
        {
            get { return _clubsSource; }
            set { _clubsSource = value; }
        }
        public ObservableCollection<CheckableCardModel> DiamondsSource
        {
            get { return _diamondsSource; }
            set { _diamondsSource = value; }
        }
        public ObservableCollection<CheckableCardModel> HeartsSource
        {
            get { return _heartsSource; }
            set { _heartsSource = value; }
        }
        public ObservableCollection<CheckableCardModel> SpadesSource
        {
            get { return _spadesSource; }
            set { _spadesSource = value; }
        }
        public IEnumerable<CheckableCardModel> AllCards
        {
            get { return ClubsSource.Union(DiamondsSource).Union(HeartsSource).Union(SpadesSource); }
        }
        #endregion

        #region ICommand
        public ICommand SelectionChangedCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand ResetCommand { get; set; }
        #endregion

        public PopupContainerCardSelectorViewModel()
        {
            SelectionChangedCommand = new RelayCommand(SelectionChanged);
            SaveCommand = new RelayCommand(Save);
            ResetCommand = new RelayCommand(Reset);

            Init();
        }

        private void Init()
        {
            for (int i = SUIT_LENGTH - 1; i >= 0; i--)
            {
                ClubsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Clubs));
                DiamondsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Diamonds));
                HeartsSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Hearts));
                SpadesSource.Add(new CheckableCardModel((RangeCardRank)i, RangeCardSuit.Spades));
            }
        }

        private void SetCards(IEnumerable<BoardCardItem> cardCollection)
        {
            var list = AllCards;
            list.ForEach(x => x.IsChecked = false);
            foreach(var card in cardCollection.Where(x=> x.Rank != RangeCardRank.None && x.Suit != RangeCardSuit.None))
            {
                var listCard = list.FirstOrDefault(x => x.Rank == card.Rank && x.Suit == card.Suit);
                if(listCard != null)
                {
                    listCard.IsChecked = true;
                }
            }
        }

        private void Save(object obj)
        {
            this._notification.CardCollection.ForEach(x =>
            {
                x.Reset();
            });

            int i = 0;
            var cardCollection = _notification.CardCollection;
            foreach (var card  in AllCards.Where(x=> x.IsChecked))
            {
                if(i >= cardCollection.Count())
                {
                    break;
                }
                cardCollection.ElementAt(i).Suit = card.Suit;
                cardCollection.ElementAt(i).Rank = card.Rank;
                i++;
            }

            this._notification.Confirmed = true;
            this.FinishInteraction();
        }

        private void Reset(object obj)
        {
            ClubsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            DiamondsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            HeartsSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
            SpadesSource.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
        }

        private void SelectionChanged(object obj)
        {
            if (obj != null)
            {
                if (obj is CheckableCardModel)
                {
                    var card = obj as CheckableCardModel;
                    if (card == null)
                    {
                        return;
                    }

                    if (!card.IsChecked && AllCards.Count(x => x.IsChecked) == this._notification.CardsCount)
                    {
                        return;
                    }

                    card.IsChecked = !card.IsChecked;
                }
            }
        }
    }
}
