using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    public class Cards
    {
        [XmlAttribute("type")]
        public CardsType Type { get; set; }

        [XmlAttribute("player")]
        public string Player { get; set; }

        [XmlText()]
        public string Value { get; set; }

        [XmlIgnore()]
        public int Seat { get; set; }
    }
}