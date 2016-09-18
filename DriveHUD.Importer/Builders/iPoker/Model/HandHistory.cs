using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    [XmlRoot("session")]
    public class HandHistory
    {
        [XmlAttribute("sessioncode")]
        public int SessionCode { get; set; }

        [XmlElement("general")]
        public General General { get; set; }

        [XmlElement("game")]
        public List<Game> Games { get; set; }
    }
}