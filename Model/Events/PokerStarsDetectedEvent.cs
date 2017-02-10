using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class PokerStarsDetectedEventArgs : EventArgs
    {
        public bool IsDetected { get; private set; }

        public PokerStarsDetectedEventArgs(bool isDetected)
        {
            IsDetected = isDetected;
        }
    }

    public class PokerStarsDetectedEvent : PubSubEvent<PokerStarsDetectedEventArgs>
    { }
}
