//-----------------------------------------------------------------------
// <copyright file="HudSavedLayouts.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Serialization;
using DriveHUD.Entities;

namespace DriveHUD.Application.ViewModels
{
    //    [Serializable]
    //    public class HudSavedLayout
    //    {
    //        public HudSavedLayout()
    //        {
    //            HudStats = new List<StatInfo>();
    //            HudPositions = new List<HudSavedPositions>();
    //            IsActiveFor = new List<HudSavedTableDefinition>();
    //            TableDefinition = null;
    //        }

    //        [XmlAttribute]
    //        public string Name { get; set; }

    //        [XmlAttribute]
    //        public bool IsDefault { get; set; }

    //        public HudSavedTableDefinition TableDefinition { get; set; }

    //        public List<StatInfo> HudStats { get; set; }

    //        public List<HudPlayerType> HudPlayerTypes { get; set; }

    //        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }

    //        public List<HudSavedPositions> HudPositions { get; set; }
    //        public List<HudSavedTableDefinition> IsActiveFor { get; set; }
    //    }

    //    [Serializable]
    //    public class HudSavedTableDefinition
    //    {
    //        public EnumTableType TableType { get; set; }
    //        public EnumPokerSites PokerSite { get; set; }
    //        public EnumGameType GameType { get; set; }
    //    }

    //    [Serializable]
    //    public class HudSavedPositions
    //    {
    //        public HudSavedTableDefinition TableDefinition { get; set; }
    //        public List<HudSavedPosition> Positions { get; set; }

    //        public HudSavedPositions()
    //        {
    //            TableDefinition = new HudSavedTableDefinition();
    //            Positions = new List<HudSavedPosition>();
    //        }
    //    }

    [Serializable]
    public class HudSavedPosition
    {
        public Point Position { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int Seat { get; set; }

        public HudType HudType { get; set; }
    }
}