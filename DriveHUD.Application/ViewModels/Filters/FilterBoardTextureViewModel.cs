using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterBoardTextureViewModel : FilterViewModel<FilterBoardTextureModel>
    {
        #region Properties
        public InteractionRequest<PopupContainerCardSelectorViewModelNotification> CardSelectorRequest { get; private set; }
        #endregion

        #region ICommand
        public ICommand SelectCommand { get; set; }
        public ICommand SelectSpadesCommand { get; set; }
        public ICommand SelectHeartsCommand { get; set; }
        public ICommand SelectDiamondsCommand { get; set; }
        public ICommand SelectClubsCommand { get; set; }
        #endregion

        internal FilterBoardTextureViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterBoardTextureViewModel, service)
        {
            Initialize();
        }

        private void Initialize()
        {
            CardSelectorRequest = new InteractionRequest<PopupContainerCardSelectorViewModelNotification>();

            SelectCommand = new RelayCommand(Select);
            SelectSpadesCommand = new RelayCommand(SelectSpades);
            SelectHeartsCommand = new RelayCommand(SelectHearts);
            SelectDiamondsCommand = new RelayCommand(SelectDiamonds);
            SelectClubsCommand = new RelayCommand(SelectClubs);
        }

        #region ICommand Implementation
        private void Select(object obj)
        {
            if (obj == null || !(obj is BoardCardItem))
            {
                return;
            }
            CardSelectorRequest.Raise(new PopupContainerCardSelectorViewModelNotification()
            {
                Title = string.Empty,
                CardsCount = ((BoardCardItem)obj).CardsCount,
                CardCollection = FilterModel.GetCardItemsCollectionForStreet(((BoardCardItem)obj).TargetStreet),
            },
                returned =>
                {
                });
        }

        private void SelectClubs(object obj)
        {
            if (obj == null || !(obj is BoardCardItem))
            {
                return;
            }

            var card = (BoardCardItem)obj;
            card.Rank = RangeCardRank.None;
            card.Suit = RangeCardSuit.Clubs;
        }

        private void SelectDiamonds(object obj)
        {
            if (obj == null || !(obj is BoardCardItem))
            {
                return;
            }

            var card = (BoardCardItem)obj;
            card.Rank = RangeCardRank.None;
            card.Suit = RangeCardSuit.Diamonds;
        }

        private void SelectHearts(object obj)
        {
            if (obj == null || !(obj is BoardCardItem))
            {
                return;
            }

            var card = (BoardCardItem)obj;
            card.Rank = RangeCardRank.None;
            card.Suit = RangeCardSuit.Hearts;
        }

        private void SelectSpades(object obj)
        {
            if (obj == null || !(obj is BoardCardItem))
            {
                return;
            }

            var card = (BoardCardItem)obj;
            card.Rank = RangeCardRank.None;
            card.Suit = RangeCardSuit.Spades;
        }
        #endregion
    }
}
