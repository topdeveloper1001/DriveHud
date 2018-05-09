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