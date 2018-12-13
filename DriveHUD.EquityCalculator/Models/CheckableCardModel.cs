//-----------------------------------------------------------------------
// <copyright file="CheckableCardModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;

namespace DriveHUD.EquityCalculator.Models
{
    public class CheckableCardModel : CardModel
    {
        public CheckableCardModel() : base()
        {
        }

        public CheckableCardModel(RangeCardRank rank, RangeCardSuit suit) :
            base(rank, suit)
        {
        }

        public static CheckableCardModel GetCheckableCardModel(CardModel card)
        {
            return new CheckableCardModel(card.Rank, card.Suit);
        }

        private bool isChecked = false;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                SetProperty(ref isChecked, value);
            }
        }

        private bool isVisible = true;

        public bool IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                SetProperty(ref isVisible, value);
            }
        }
    }
}