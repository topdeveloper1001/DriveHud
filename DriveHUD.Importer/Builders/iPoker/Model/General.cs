using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace DriveHUD.Importers.Builders.iPoker
{
    public class General
    {
        [XmlElement("mode")]
        public string Mode { get; set; }

        [XmlElement("gametype")]
        public string GameType { get; set; }

        [XmlElement("tablename")]
        public string TableName { get; set; }

        [XmlElement("duration")]
        public string Duration { get; set; }

        [XmlElement("gamecount")]
        public int GameCount { get; set; }

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

        [XmlElement("TournamentCurrency")]
        public string TournamentCurrency { get; set; }

        [XmlElement("currency")]
        public Currency Currency { get; set; }

        [XmlElement("nickname")]
        public string Nickname { get; set; }

        [XmlElement("bets")]
        public decimal Bets { get; set; }

        [XmlElement("wins")]
        public decimal Wins { get; set; }

        [XmlElement("chipsin")]
        public decimal Chipsin { get; set; }

        [XmlElement("chipsout")]
        public decimal Chipsout { get; set; }

        [XmlElement("ipoints")]
        public double Ipoints { get; set; }

        [XmlElement("statuspoints")]
        public double StatusPoints { get; set; }

        [XmlElement("awardpoints")]
        public double Awardpoints { get; set; }

        [XmlElement("tournamentname")]
        public string TournamentName { get; set; }

        [XmlElement("place")]
        public int Place { get; set; }

        [XmlElement("buyin")]
        public string BuyIn { get; set; }

        [XmlElement("totalbuyin")]
        public string TotalBuyIn { get; set; }

        [XmlElement("totalprize")]
        public string TotalPrizePool { get; set; }

        [XmlElement("win")]
        public decimal Win { get; set; }

        [XmlElement("is_asian")]
        public string IsAsian { get; set; }

        [XmlElement("maxplayers"), DefaultValue(0)]
        public int MaxPlayers { get; set; }

        [XmlElement("pokersite")]
        public EnumPokerSites? Site { get; set; }

        [XmlElement("tournamentstags", IsNullable = true)]
        public TournamentsTags? TournamentsTags { get; set; }

        public bool ShouldSerializeTournamentsTags()
        {
            return TournamentsTags != null;
        }
    }
}