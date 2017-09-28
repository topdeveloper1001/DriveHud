using System.Collections.Generic;
using System.Xml.Serialization;
using AcePokerSolutions.DataTypes;

namespace AcePokerSolutions.DataAccessHelper.DriveHUD
{
    public class HandHistory
    {
        public string HandId { get; set; }
        public string TableName { get; set; } 
        public GameDescription GameDescription { get; set; }
        public List<HandAction> Actions { get; set; }
        public List<Player> Players { get; set; }
    }

    public class Player
    {
        [XmlAttribute]
        public string PlayerName { get; set; }

        [XmlAttribute]
        public decimal StartingStack { get; set; }
    }

    public class GameDescription
    {
        public SeatType SeatType { get; set; }
    }

    public class SeatType
    {
        [XmlAttribute]
        public short MaxPlayers { get; set; }
    }

    public class WinningsAction : HandAction
    {
    }

    public class AllInAction : HandAction
    {
    }

    public class StreetAction : HandAction
    {
    }

    [XmlInclude(typeof(WinningsAction))]     
    [XmlInclude(typeof(AllInAction))]  
    [XmlInclude(typeof(StreetAction))]  
    public class HandAction
    {
        [XmlAttribute]
        public string PlayerName { get; set; }
        [XmlAttribute]
        public HandActionType HandActionType { get; set; }
        [XmlAttribute]
        public decimal Amount { get; set; }
        [XmlAttribute]
        public Street Street { get; set; }
        [XmlAttribute]
        public bool IsAllIn { get; set; }
    }
}
