using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    #region Event Args

    public class LoadDefaultFilterRequestedEventArgs : EventArgs
    { }

    public class SaveDefaultFilterRequestedEvetnArgs : EventArgs
    { }

    #endregion

    #region Events

    public class SaveDefaultFilterRequestedEvent : PubSubEvent<SaveDefaultFilterRequestedEvetnArgs>
    { }

    public class LoadDefaultFilterRequestedEvent : PubSubEvent<LoadDefaultFilterRequestedEventArgs>
    { }

    #endregion
}
