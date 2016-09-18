using HandHistories.Objects.Cards;
using Model.Filters;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupContainerCardSelectorViewModelNotification : Confirmation
    {
        public int CardsCount { get; set; }
        public IEnumerable<BoardCardItem> CardCollection { get; set; }
    }
}
