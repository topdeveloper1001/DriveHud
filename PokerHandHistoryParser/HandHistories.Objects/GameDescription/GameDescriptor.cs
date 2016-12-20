using DriveHUD.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    public class GameDescriptor
    {
        public GameDescriptor() : this(PokerFormat.Unknown,
                                       EnumPokerSites.Unknown,
                                       GameType.Unknown,
                                       null,
                                       TableType.FromTableTypeDescriptions(),
                                       SeatType.AllSeatType(),
                                       null)
        {
        }

        public GameDescriptor(EnumPokerSites siteName,
                              GameType gameType,
                              Limit limit,
                              TableType tableType,
                              SeatType seatType, TournamentDescriptor tournament)
            : this(PokerFormat.CashGame, siteName, gameType, limit, tableType, seatType, tournament)
        {
        }

        public GameDescriptor(PokerFormat pokerFormat,
                              EnumPokerSites siteName,
                              GameType gameType,
                              Limit limit,
                              TableType tableType,
                              SeatType seatType,
                              TournamentDescriptor tournament)
        {
            PokerFormat = pokerFormat;
            Site = siteName;
            GameType = gameType;
            Limit = limit;
            TableType = tableType;
            SeatType = seatType;
            Tournament = tournament;
        }

        [XmlElement]
        public PokerFormat PokerFormat { get; set; }

        [XmlElement]
        public EnumPokerSites Site { get; set; }

        [XmlElement]
        public GameType GameType { get; set; }

        [XmlElement]
        public Limit Limit { get; set; }

        [XmlElement]
        public SeatType SeatType { get; set; }

        [XmlIgnore]
        public TableType TableType { get; set; }

        [XmlArray]
        public List<TableTypeDescription> TableTypeDescriptors
        {
            get
            {
                return TableType.GetTableTypeDescriptions().ToList();
            }
            set
            {

            }
        }

        [XmlElement]
        public TournamentDescriptor Tournament { get; set; }

        [XmlElement]
        public bool IsTournament
        {
            get
            {
                return Tournament != null;
            }
        }

        public override bool Equals(object obj)
        {
            var descriptor = obj as GameDescriptor;

            if (descriptor == null)
            {
                return false;
            }

            return (descriptor.ToString().Equals(ToString()));
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}.{4}.{5}", Site, GameType, Limit, TableType, SeatType, Tournament);
        }
    }
}