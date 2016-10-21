using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Events
{
    public class RequestEquityCalculatorEventArgs : EventArgs
    {
        public long GameNumber { get; set; }
        public bool IsEmptyRequest { get; set; }
        public short PokersiteId { get; set; }

        public RequestEquityCalculatorEventArgs()
        {
            this.IsEmptyRequest = true;
        }

        public RequestEquityCalculatorEventArgs(long gameNumber, short pokersiteId)
        {
            this.GameNumber = gameNumber;
            this.PokersiteId = pokersiteId;
        }
    }


    public class RequestEquityCalculatorEvent : Prism.Events.PubSubEvent<RequestEquityCalculatorEventArgs>
    {

    }
}
