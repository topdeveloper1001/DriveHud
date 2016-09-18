using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.PopupContainers.Notifications
{
    public class PopupBaseNotification : Confirmation
    {
        public string ConfirmButtonCaption { get; set; } = "OK";
        public string CancelButtonCaption { get; set; } = "Cancel";
        public bool IsDisplayH1Text { get; set; } = true;
    } 
}
