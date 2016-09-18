using DriveHUD.Entities;
using System;
using System.Runtime.Serialization;

namespace HandHistories.Objects.GameDescription
{
    [Serializable]
    [DataContract()]
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

        [DataMember]
        public PokerFormat PokerFormat { get; set; }

        [DataMember]
        public EnumPokerSites Site { get; set; }

        [DataMember]
        public GameType GameType { get; set; }

        [DataMember]
        public Limit Limit { get; set; }

        [DataMember]
        public SeatType SeatType { get; set; }

        [DataMember]
        public TableType TableType { get; set; }

        [DataMember]
        public TournamentDescriptor Tournament { get; set; }

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