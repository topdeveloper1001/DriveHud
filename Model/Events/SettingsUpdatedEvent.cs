using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class SettingsUpdatedEventArgs : EventArgs
    { }

    public class SettingsUpdatedEvent : PubSubEvent<SettingsUpdatedEventArgs>
    {
    }
}
