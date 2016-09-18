using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class PreferredSeatChangedEventArgs : EventArgs
    {
        public int SeatNumber { get; set; }

        public PreferredSeatChangedEventArgs(int newPreferredSeatNumber)
        {
            this.SeatNumber = newPreferredSeatNumber;
        }
    }

    public class PreferredSeatChangedEvent : PubSubEvent<PreferredSeatChangedEventArgs>
    {
    }
}
