using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    public class TournamentDescriptor
    {
        [XmlElement]
        public string TournamentId { get; set; }

        [XmlElement]
        public string TournamentInGameId { get; set; }

        [XmlElement]
        public string TournamentName { get; set; }

        [XmlElement]
        public string Summary { get; set; }

        [XmlElement]
        public Buyin BuyIn { get; set; }

        [XmlElement]
        public decimal Bounty { get; set; }

        [XmlElement]
        public decimal Rebuy { get; set; }

        [XmlElement]
        public decimal Addon { get; set; }

        [XmlElement]
        public decimal Winning { get; set; }

        [XmlElement]
        public decimal TotalPrize { get; set; }

        [XmlElement]
        public short FinishPosition { get; set; }

        [XmlElement]
        public short TotalPlayers { get; set; }

        [XmlElement]
        public DateTime StartDate { get; set; }

        [XmlElement]
        public TournamentSpeed Speed { get; set; }

        [XmlElement]
        public bool IsSummary
        {
            get
            {
                return !string.IsNullOrEmpty(Summary);
            }
        }
    }
}