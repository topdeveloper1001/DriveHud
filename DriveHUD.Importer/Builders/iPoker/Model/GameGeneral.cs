using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    public class GameGeneral
    {
        [XmlElement("startdate")]
        public string StartDateString
        {
            get
            {
                return StartDate.ToString(PokerConfiguration.DateTimeFormat, CultureInfo.InvariantCulture);
            }
            set
            {
                // possible exception, but we aren't going to use deserialization
                StartDate = DateTime.ParseExact(value, PokerConfiguration.DateTimeFormat, null);
            }
        }

        [XmlIgnore()]
        public DateTime StartDate { get; set; }

        [XmlIgnore()]
        public int PlayersSeatShift { get; set; }

        [XmlArray("players")]
        public List<Player> Players { get; set; }
    }
}
