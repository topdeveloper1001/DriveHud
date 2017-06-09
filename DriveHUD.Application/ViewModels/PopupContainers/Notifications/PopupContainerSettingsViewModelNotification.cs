using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DriveHUD.Application.Models;

using Prism.Interactivity.InteractionRequest;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupContainerSettingsViewModelNotification : Confirmation
    {
        public string Parameter;
        public PopupContainerSettingsViewModelNotification()
        {
        }

        public PopupContainerSettingsViewModelNotification(object items)
            : this()
        {
        }

        public PubSubMessage PubSubMessage { get; set; }
    }
}

