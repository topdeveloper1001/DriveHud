using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    [XmlType("player")]    
    public class Player
    {
        [XmlAttribute("seat")]
        public int Seat { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore()]
        public decimal Chips { get; set; }

        [XmlAttribute("chips")]
        public string ChipsString
        {
            get
            {
                return Chips.ToString(PokerConfiguration.DecimalFormat);
            }
            set
            {
                Chips = decimal.Parse(value);
            }
        }

        [XmlIgnore]
        public bool Dealer { get; set; }

        [XmlAttribute("dealer")]
        public int DealerValue
        {
            get
            {
                return Dealer ? 1 : 0;
            }
            set
            {
                Dealer = value == 1;
            }
        }

        [XmlIgnore()]
        public decimal Win { get; set; }

        [XmlAttribute("win")]
        public string WinString
        {
            get
            {
                return Win.ToString(PokerConfiguration.DecimalFormat);
            }
            set
            {
                Win = decimal.Parse(value);
            }
        }

        [XmlIgnore()]
        public decimal Bet { get; set; }

        [XmlAttribute("bet")]
        public string BetString
        {
            get
            {
                return Bet.ToString(PokerConfiguration.DecimalFormat);
            }
            set
            {
                Bet = decimal.Parse(value);
            }
        }

        [XmlAttribute("rebuy")]
        public decimal Rebuy { get; set; }

        [XmlAttribute("addon")]
        public decimal Addon { get; set; }

        [XmlAttribute("reg_code")]
        public string RegCode { get; set; }

        [XmlIgnore()]
        public bool IsHero { get; set; }
    }
}
