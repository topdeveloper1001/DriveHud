using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
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

        public List<StatInfo> HudStats { get; set; }
        public List<HudPlayerType> HudPlayerTypes { get; set; }
        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }
        public List<HudSavedPosition> HudPositions { get; set; }
        public int HudOpacity { get; set; }

        public HudLayoutInfo()
        {
            HudOpacity = 100;
        }

        public HudLayoutInfo Clone()
        {
            return new HudLayoutInfo
            {
                Name = Name,
                TableType = TableType,
                IsDefault = IsDefault,
                HudStats = HudStats.Select(s => s.Clone()).ToList(),
                HudPlayerTypes = HudPlayerTypes.Select(p => p.Clone()).ToList(),
                HudPositions = HudPositions.Select(p => p.Clone()).ToList(),
                HudBumperStickerTypes = HudBumperStickerTypes.Select(p => p.Clone()).ToList(),
                HudOpacity = HudOpacity
            };
        }
    }
}