using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Enums;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudLayoutInfo
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsDefault { get; set; }

        [XmlAttribute]
        public EnumTableType TableType { get; set; }

        [XmlAttribute]
        public HudViewType HudViewType { get; set; }

        public List<StatInfo> HudStats { get; set; }
        public List<HudPlayerType> HudPlayerTypes { get; set; }
        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }
        public List<HudPositionsInfo> HudPositionsInfo { get; set; }

        public int HudOpacity { get; set; }

        public HudLayoutInfo()
        {
            HudOpacity = 100;
            HudViewType = HudViewType.Plain;
            HudStats = new List<StatInfo>();
            HudPlayerTypes = new List<HudPlayerType>();
            HudBumperStickerTypes = new List<HudBumperStickerType>();
            HudPositionsInfo = new List<HudPositionsInfo>();
        }

        public HudLayoutInfo Clone()
        {
            return new HudLayoutInfo
            {
                Name = Name,
                TableType = TableType,
                IsDefault = IsDefault,
                HudOpacity = HudOpacity,
                HudViewType = HudViewType,
                HudStats = HudStats.Select(s => s.Clone()).ToList(),
                HudPlayerTypes = HudPlayerTypes.Select(p => p.Clone()).ToList(),
                HudPositionsInfo = HudPositionsInfo.Select(p => p.Clone()).ToList(),
                HudBumperStickerTypes = HudBumperStickerTypes.Select(p => p.Clone()).ToList()
            };
        }
    }
}