using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.ViewModels;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Replayer
{
    public class ReplayerPlayerViewModel : BaseViewModel
    {
        internal ReplayerPlayerViewModel()
        {
        }

        internal void Reset(bool keepCards = false)
        {
            ChipsContainer = new ReplayerChipsContainer();
            StatInfoCollection = new ObservableCollection<StatInfo>();

            if (Cards == null)
            {
                Cards = new ObservableCollection<ReplayerCardViewModel>();
                for (int i = 0; i < 4; i++)
                {
                    var card = new ReplayerCardViewModel();
                    card.CardToDisplay = ReplayerCardViewModel.DEFAULT_CARD_ID;
                    card.CardId = ReplayerCardViewModel.DEFAULT_CARD_ID;
                    card.CanHideCards = false;

                    Cards.Add(card);
                }
            }
            else if (!keepCards)
            {
                foreach (var card in Cards)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        card.CardToDisplay = ReplayerCardViewModel.DEFAULT_CARD_ID;
                        card.CardId = ReplayerCardViewModel.DEFAULT_CARD_ID;
                        card.CanHideCards = false;
                    }
                }
            }

            Name = "Empty";
            IsFinished = true;
            IsActive = false;
            IsDealer = false;
            IsWin = false;
            Bank = 0;
            OldBank = 0;
            ActiveAmount = 0;
            OldAmount = 0;
            CurrentStreet = Street.Preflop;
            ActionString = string.Empty;
        }

        internal void SetCards(int[] cards, bool canHide)
        {
            for (int i = 0; i < cards.Count(); i++)
            {
                if (i > Cards.Count)
                {
                    LogProvider.Log.Error(this, "ReplayerPlayerViewModel.setCards(): method received more then 4 cards: " + string.Join(" ", cards));
                    break;
                }
                Cards[i].CardToDisplay = cards[i];
                Cards[i].CardId = cards[i];
                Cards[i].CanHideCards = canHide;
            }
        }

        internal void HideCards()
        {
            Cards.ForEach(x => x.HideCards());
        }

        internal void ShowCards()
        {
            Cards.ForEach(x => x.ShowCards());
        }

        internal void UpdateChips()
        {
            this.ChipsContainer.UpdateChips(this.ActiveAmount);
        }

        internal static void Copy(ReplayerPlayerViewModel from, ReplayerPlayerViewModel to)
        {
            if (to == null || from == null)
            {
                return;
            }

            to.Reset(keepCards: true);

            to.ChipsContainer = from.ChipsContainer;
            to.StatInfoCollection = from.StatInfoCollection;

            to.Name = from.Name;
            to.IsActive = from.IsActive;
            to.IsFinished = from.IsFinished;
            to.IsDealer = from.IsDealer;
            to.IsWin = from.IsWin;

            to.Bank = from.Bank;
            to.OldBank = from.OldBank;
            to.ActiveAmount = from.ActiveAmount;
            to.OldAmount = from.OldAmount;

            to.ActionString = from.ActionString;

            to.CurrentStreet = from.CurrentStreet;
        }

        #region Properties
        private string _name;
        private bool _isFinished;
        private bool _isActive;
        private bool _isDealer;
        private bool _isWin;
        private decimal _bank;
        private decimal _oldBank;
        private decimal _activeAmount;
        private decimal _oldAmount;
        private ObservableCollection<ReplayerCardViewModel> _cards;
        private string _actionString;
        private ReplayerChipsContainer _chipsContainer;
        private Street _currentStreet;
        private ObservableCollection<StatInfo> _statInfoCollection;

        public string Name
        {
            get { return _name; }
            set
            {
                SetProperty(ref _name, value);
                ChipsContainer.ChipsShape.Name = "chips_" + SafeNameString;
            }
        }

        public string SafeNameString
        {
            get { return "a" + Name.Replace(" ", "").Replace(".", "").Replace("$", ""); }
        }

        public bool IsFinished
        {
            get { return _isFinished; }
            set { SetProperty(ref _isFinished, value); }
        }

        public new bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

        public decimal Bank
        {
            get { return _bank; }
            set { SetProperty(ref _bank, value); }
        }

        public decimal OldBank
        {
            get { return _oldBank; }
            set { _oldBank = value; }
        }

        public decimal ActiveAmount
        {
            get { return _activeAmount; }
            set { SetProperty(ref _activeAmount, value); }
        }

        public decimal OldAmount
        {
            get { return _oldAmount; }
            set { _oldAmount = value; }
        }

        public bool IsDealer
        {
            get { return _isDealer; }
            set { SetProperty(ref _isDealer, value); }
        }

        public bool IsWin
        {
            get { return _isWin; }
            set { _isWin = value; }
        }

        public ObservableCollection<ReplayerCardViewModel> Cards
        {
            get { return _cards; }
            set { _cards = value; }
        }

        public string ActionString
        {
            get { return _actionString; }
            set
            {
                SetProperty(ref _actionString, value);
                OnPropertyChanged(() => DisplayString);
            }
        }

        public string DisplayString
        {
            get { return string.IsNullOrEmpty(ActionString) ? Name : ActionString; }
        }

        public ReplayerChipsContainer ChipsContainer
        {
            get { return _chipsContainer; }
            set { SetProperty(ref _chipsContainer, value); }
        }

        public Street CurrentStreet
        {
            get { return _currentStreet; }
            set { _currentStreet = value; }
        }

        public ObservableCollection<StatInfo> StatInfoCollection
        {
            get { return _statInfoCollection; }
            set { SetProperty(ref _statInfoCollection, value); }
        }

        public bool IsNoteIconVisible
        {
            get { return false; }
        }

        #endregion
    }
}
