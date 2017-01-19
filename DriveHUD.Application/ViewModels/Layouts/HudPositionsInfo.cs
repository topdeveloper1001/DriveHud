using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudPositionsInfo
    {
        [XmlAttribute]
        public EnumPokerSites PokerSite { get; set; }

        [XmlAttribute]
        public EnumGameType GameType { get; set; }

        [XmlAttribute]
        public HudType HudType { get; set; }

        public List<HudPositionInfo> HudPositions { get; set; }


        public HudPositionsInfo()
        {
            HudType = HudType.Plain;
            HudPositions = new List<HudPositionInfo>();
        }

        public HudPositionsInfo Clone()
        {
            return new HudPositionsInfo
            {
                PokerSite = PokerSite,
                GameType = GameType,
                HudType = HudType,
                HudPositions = HudPositions.Select(p => p.Clone()).ToList()
            };
        }
    }
}