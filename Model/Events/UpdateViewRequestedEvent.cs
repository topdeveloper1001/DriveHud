using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class UpdateViewRequestedEventArgs: EventArgs
    {
        public bool IsUpdateReportRequested { get; set; }
    }

    public class UpdateViewRequestedEvent : PubSubEvent<UpdateViewRequestedEventArgs>
    {

    }
}
