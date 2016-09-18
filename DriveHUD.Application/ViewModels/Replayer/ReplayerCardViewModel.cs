using DriveHUD.Common.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Replayer
{
    public class ReplayerCardViewModel : BaseViewModel
    {
        internal static int DEFAULT_CARD_ID = 52;

        internal ReplayerCardViewModel()
        {
            CardToDisplay = DEFAULT_CARD_ID;
            CardId = DEFAULT_CARD_ID;
            CanHideCards = false;
        }

        internal void HideCards()
        {
            if (CanHideCards)
            {
                CardId = 52;
            }
        }

        internal void ShowCards()
        {
            CardId = CardToDisplay;
        }

        private int _cardId;
        public int CardId
        {
            get { return _cardId; }
            set { SetProperty(ref _cardId, value); }
        }

        internal int CardToDisplay { get; set; }
        internal bool CanHideCards { get; set; }
    }
}
