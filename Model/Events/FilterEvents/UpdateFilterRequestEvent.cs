using Prism.Events;
using System;

namespace Model.Events
{
    public class UpdateFilterRequestEventArgs : EventArgs
    {        
    }

    public class UpdateFilterRequestEvent : PubSubEvent<UpdateFilterRequestEventArgs>
    {
    }
}