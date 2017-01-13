using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using DriveHUD.Entities;
using DriveHUD.ViewModels;
using Model.Enums;
using Telerik.Windows.Diagrams.Core;

namespace DriveHUD.Application.ViewModels.Layouts
{
    [Serializable]
    public class HudLayoutInfo
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsDefault { get; set; }

        public List<HudTableDefinedProperties> HudTableDefinedProperties { get; set; }
        public List<HudTableDefinition> ActiveFor { get; set; }

        public HudLayoutInfo()
        {
            HudTableDefinedProperties = new List<HudTableDefinedProperties>();
            ActiveFor = new List<HudTableDefinition>();
        }

        public HudLayoutInfo Clone()
        {
            return new HudLayoutInfo
            {
                Name = Name,
                IsDefault = IsDefault,
                HudTableDefinedProperties = HudTableDefinedProperties.Clone().ToList(),
                ActiveFor = ActiveFor.Clone().ToList()
            };
        }
    }

    [Serializable]
    public class HudTableDefinition
    {
        [XmlAttribute]
        public EnumTableType TableType { get; set; }

        [XmlIgnore]
        public EnumPokerSites? PokerSite { get; set; }

        [XmlIgnore]
        public EnumGameType? GameType { get; set; }

        [XmlElement("PokerSite")]
        public object PokerSiteValue
        {
            get { return PokerSite; }
            set
            {
                if (value == null)
                    PokerSite = null;
                else
                    PokerSite = (EnumPokerSites) value;
            }
        }

        [XmlElement("GameType")]
        public object GameTypeValue
        {
            get { return GameType; }
            set
            {
                if (value == null)
                    GameType = null;
                else
                    GameType = (EnumGameType) value;
            }
        }

        public override bool Equals(object obj)
        {
            var tableDefinition = obj as HudTableDefinition;
            if (tableDefinition == null)
                return false;
            return tableDefinition.GameType == GameType && tableDefinition.PokerSite == PokerSite &&
                   tableDefinition.TableType == TableType;
        }

        public HudTableDefinition Clone()
        {
            return new HudTableDefinition {GameType = GameType, PokerSite = PokerSite, TableType = TableType};
        }
    }

    [Serializable]
    public class HudTableDefinedProperties
    {
        public HudTableDefinition HudTableDefinition { get; set; }
        public List<StatInfo> HudStats { get; set; }
        public List<HudPlayerType> HudPlayerTypes { get; set; }
        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }
        public List<HudSavedPosition> HudPositions { get; set; }

        public HudTableDefinedProperties()
        {
            HudTableDefinition = new HudTableDefinition();
            HudStats = new List<StatInfo>();
            HudPlayerTypes = new List<HudPlayerType>();
            HudBumperStickerTypes = new List<HudBumperStickerType>();
            HudPositions = new List<HudSavedPosition>();
        }

        public HudTableDefinedProperties Clone()
        {
            return new HudTableDefinedProperties
            {
                HudTableDefinition = HudTableDefinition.Clone(),
                HudStats = HudStats.Clone().ToList(),
                HudPlayerTypes = HudPlayerTypes.Clone().ToList(),
                HudPositions = HudPositions.Clone().ToList()
            };
        }
    }
}