using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class TournamentDataUpdatedEventArgs : EventArgs
    {

    }

    public class TournamentDataUpdatedEvent : PubSubEvent<TournamentDataUpdatedEventArgs>
    {
    }
}
