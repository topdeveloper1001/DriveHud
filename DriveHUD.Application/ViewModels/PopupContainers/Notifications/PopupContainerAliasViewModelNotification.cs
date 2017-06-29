using DriveHUD.Application.Models;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupContainerAliasViewModelNotification : Confirmation 
    {
        public PopupContainerAliasViewModelNotification() { }

        public PopupContainerAliasViewModelNotification(object items) : this() { }

        public PubSubMessage PubSubMessage { get; set; }
    }
}
