//-----------------------------------------------------------------------
// <copyright file="CardModel.cs" company="Ace Poker Solutions">
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
using Model.Enums;

namespace DriveHUD.EquityCalculator.Models
{
    public class CardModel : BaseViewModel
    {
        #region Properties

        private RangeCardSuit _suit = RangeCardSuit.None;

        public RangeCardSuit Suit
        {
            get
            {
                return _suit;
            }

            set
            {
                SetProperty(ref _suit, value);
            }
        }

        private RangeCardRank _rank = RangeCardRank.None;

        public RangeCardRank Rank
        {
            get
            {
                return _rank;
            }

            set
            {
                SetProperty(ref _rank, value);
            }
        }
        #endregion

        public CardModel()
        {
            Rank = RangeCardRank.None;
            Suit = RangeCardSuit.None;
        }

        public CardModel(RangeCardRank rank, RangeCardSuit suit)
        {
            SetCard(rank, suit);
        }

        public void SetCard(RangeCardRank rank, RangeCardSuit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public bool Validate()
        {
            return (Rank != RangeCardRank.None) && (Suit != RangeCardSuit.None);
        }

        public override string ToString()
        {
            return string.Concat(Rank.ToRankString(), Suit.ToSuitString());
        }
    }
}