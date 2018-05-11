using DriveHUD.EquityCalculator.Models;
using Prism.Interactivity.InteractionRequest;
using System.Collections.Generic;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public enum CardSelectorType
    {
        BoardSelector,
        PlayerSelector
    }

    internal enum CardSelectorReturnType
    {
        Cards,
        Range
    }


    public class CardSelectorNotification : Confirmation
    {
        #region Fields
        private ICardCollectionContainer _cardsContainer;
        private CardSelectorType _selectorType = CardSelectorType.BoardSelector;
        private IEnumerable<CardModel> _usedCards;
        private CardSelectorReturnType _returnType = CardSelectorReturnType.Cards;
        #endregion

        #region Properties
        internal ICardCollectionContainer CardsContainer
        {
            get
            {
                return _cardsContainer;
            }

            set
            {
                _cardsContainer = value;
            }
        }

        internal CardSelectorType SelectorType
        {
            get
            {
                return _selectorType;
            }

            set
            {
                _selectorType = value;
            }
        }

        internal IEnumerable<CardModel> UsedCards
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

        private IEnumerable<CardModel> boardCards;

        internal IEnumerable<CardModel> BoardCards
        {
            get
            {
                return boardCards;
            }
            set
            {
                boardCards = value;
            }
        }

        internal CardSelectorReturnType ReturnType
        {
            get
            {
                return _returnType;
            }

            set
            {
                _returnType = value;
            }
        }

        #endregion

        public CardSelectorNotification() : base() { }

    }
}
