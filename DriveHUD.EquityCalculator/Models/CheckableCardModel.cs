using Model.Enums;

namespace DriveHUD.EquityCalculator.Models
{
    public class CheckableCardModel : CardModel
    {
        private bool _isChecked = false;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                SetProperty(ref _isChecked, value);
                
            }
        }

        public CheckableCardModel() : base() { }

        public CheckableCardModel(RangeCardRank rank, RangeCardSuit suit) : base(rank, suit) { }

        public static CheckableCardModel GetCheckableCardModel(CardModel card)
        {
            return new CheckableCardModel(card.Rank, card.Suit);
        }
    }
}
