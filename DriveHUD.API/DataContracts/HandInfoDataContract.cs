using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.API.DataContracts
{
    [DataContract]
    public class HandInfoDataContract
    {
        [DataMember(Order = 1)]
        public long GameNumber { get; set; }

        [DataMember(Order = 2)]
        public string DateUtc { get; set; }

        [DataMember(Order = 3)]
        public string PlayerName { get; set; }

        [DataMember(Order = 4)]
        public string HoleCards { get; set; }

        [DataMember(Order = 5)]
        public string CommunityCards { get; set; }

        [DataMember(Order = 6)]
        public decimal Win { get; set; }

        [DataMember(Order = 7)]
        public string PokerSite { get; set; }

        public static HandInfoDataContract Map(HandHistories.Objects.Hand.HandHistory hh)
        {
            if (hh == null)
                throw new ArgumentNullException("hh");

            var handInfo = new HandInfoDataContract();
            handInfo.GameNumber = hh.HandId;
            handInfo.DateUtc = hh.DateOfHandUtc.ToString();
            handInfo.PlayerName = hh.Hero?.PlayerName ?? string.Empty;
            handInfo.HoleCards = (hh.Hero?.hasHoleCards ?? false) ? hh.Hero.HoleCards.ToString() : string.Empty;
            handInfo.CommunityCards = hh.CommunityCardsString;
            handInfo.Win = hh.Hero?.Win ?? 0;
            handInfo.PokerSite = hh.GameDescription.Site.ToString();

            return handInfo;
        }

        public static HandInfoDataContract Map(Playerstatistic stat)
        {
            if (stat == null)
                throw new ArgumentNullException("stat");

            var handInfo = new HandInfoDataContract();
            handInfo.GameNumber = stat.GameNumber;
            handInfo.DateUtc = stat.Time.ToString();
            handInfo.PlayerName = stat.PlayerName;
            handInfo.HoleCards = stat.Cards;
            handInfo.CommunityCards = stat.Board;
            handInfo.Win = stat.NetWon;
            handInfo.PokerSite = ((EnumPokerSites)stat.PokersiteId).ToString();

            return handInfo;
        }
    }
}
