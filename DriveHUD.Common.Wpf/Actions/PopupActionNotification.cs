using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using Prism.Commands;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Common.Wpf.Actions
{
    public class PopupActionNotification : INotification
    {
        public Action OnCloseRaised;

        public object Content { get; set; }

        public string Title { get; set; }

        public string HyperLinkText { get; set; }

        public bool IsHyperLinkPresent
        {
            get { return !string.IsNullOrWhiteSpace(HyperLinkText); }
        }

        public ICommand CloseCommand { get; set; }

        public ICommand OpenHyperLinkCommand { get; set; }

        public PopupActionNotification()
        {
            CloseCommand = new DelegateCommand(InvokeClose);
            OpenHyperLinkCommand = new DelegateCommand(OpenHyperLink);
        }

        private void InvokeClose()
        {
            OnCloseRaised?.Invoke();
        }

        private void OpenHyperLink()
        {
            if (IsHyperLinkPresent)
            {
                try
                {
                    Process.Start(BrowserHelper.GetDefaultBrowserPath(), HyperLinkText);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(ex);
                }
            }

            InvokeClose();
        }
    }
}
