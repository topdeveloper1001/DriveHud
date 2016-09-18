using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    [XmlRoot("game")]
    public class Game
    {
        [XmlAttribute("gamecode")]
        public ulong GameCode { get; set; }

        [XmlElement("general")]
        public GameGeneral General { get; set; }

        [XmlElement("round")]
        public List<Round> Rounds { get; set; }
    }
}