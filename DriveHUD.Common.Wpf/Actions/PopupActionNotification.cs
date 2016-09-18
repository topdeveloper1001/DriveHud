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

        public ICommand CloseCommand { get; set; }

        public PopupActionNotification()
        {
            CloseCommand = new DelegateCommand<object>(InvokeClose);
        }

        private void InvokeClose(object obj)
        {
            OnCloseRaised?.Invoke();
        }
    }
}
