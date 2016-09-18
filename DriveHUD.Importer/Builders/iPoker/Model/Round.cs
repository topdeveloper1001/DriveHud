using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    public class Round
    {
        [XmlAttribute("no")]
        public int No { get; set; }

        [XmlElement("cards")]
        public List<Cards> Cards { get; set; }

        [XmlElement("action")]
        public List<Action> Actions { get; set; }
    }
}