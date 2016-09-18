using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class HandNoteUpdatedEventArgs : EventArgs
    {
        public long GameNumber { get; private set; }
        public string PlayerName { get; private set; }
        public HandNoteUpdatedEventArgs(long gameNumber, string  playerName)
        {
            this.GameNumber = gameNumber;
            this.PlayerName = playerName;
        }
    }


    public class HandNoteUpdatedEvent : PubSubEvent<HandNoteUpdatedEventArgs>
    {
    }
}
