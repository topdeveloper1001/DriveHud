using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class RequestDisplayTournamentHandsEvent : EventArgs
    {
        public string TournamentNumber { get; set; }
        public RequestDisplayTournamentHandsEvent(string tournamentNumber)
        {
            this.TournamentNumber = tournamentNumber;
        }
    }

    public class RequestDisplayTournamentHands : Prism.Events.PubSubEvent<RequestDisplayTournamentHandsEvent>
    {
    }
}
