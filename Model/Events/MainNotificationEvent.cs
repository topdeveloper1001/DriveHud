using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class MainNotificationEventArgs : EventArgs
    {
        public string Title { get; private set; }
        public string Message { get; private set; }

        public MainNotificationEventArgs(string title, string message)
        {
            this.Title = title;
            this.Message = message;
        }
    }

    public class MainNotificationEvent : PubSubEvent<MainNotificationEventArgs>
    {
    }
}
