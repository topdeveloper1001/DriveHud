using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    public class Action
    {
        [XmlAttribute("no")]
        public int No { get; set; }

        [XmlAttribute("player")]
        public string Player { get; set; }

        [XmlIgnore()]
        public int SeatNumber { get; set; }

        [XmlIgnore]
        public ActionType Type { get; set; }

        [XmlAttribute("type")]
        public string TypeString
        {
            get
            {
                return ((int)Type).ToString();
            }
            set
            {
                Type = (ActionType)int.Parse(value);
            }
        }

        [XmlIgnore()]
        public decimal Sum { get; set; }

        [XmlAttribute("sum")]
        public string SumString
        {
            get
            {
                return Sum.ToString(PokerConfiguration.DecimalFormat, CultureInfo.InvariantCulture);
            }
            set
            {
                Sum = decimal.Parse(value, CultureInfo.InvariantCulture);
            }
        }

        private string cards = string.Empty;

        [XmlAttribute("cards")]
        public string Cards
        {
            get
            {
                return cards;
            }
            set
            {
                cards = value;
            }
        }
    }
}