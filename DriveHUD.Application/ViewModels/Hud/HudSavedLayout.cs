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

using Model.Enums;
using Model.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Hud
{
    [Serializable]
    public class HudSavedLayouts
    {
        public HudSavedLayouts()
        {
            Layouts = new List<HudSavedLayout>();
        }

        public List<HudSavedLayout> Layouts { get; set; }

        public HudSavedLayouts Clone()
        {
            var clone = new HudSavedLayouts();
            clone.Layouts = new List<HudSavedLayout>(Layouts.Select(x => x.Clone()));
            return clone;
        }
    }

    [Serializable]
    public class HudSavedLayout
    {
        public HudSavedLayout()
        {
            HudStats = new List<StatInfo>();
            HudPositions = new List<HudSavedPosition>();
        }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public bool IsDefault { get; set; }

        [XmlAttribute]
        public int LayoutId { get; set; }

        public List<StatInfo> HudStats { get; set; }

        public List<HudPlayerType> HudPlayerTypes { get; set; }

        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }

        public List<HudSavedPosition> HudPositions { get; set; }

        public HudSavedLayout Clone()
        {
            var clone = (HudSavedLayout) MemberwiseClone();

            clone.HudStats = new List<StatInfo>(HudStats.Select(x =>
            {
                var statInfoBreak = x as StatInfoBreak;

                if (statInfoBreak != null)
                {
                    return statInfoBreak.Clone();
                }

                return x.Clone();
            }));
            clone.HudPlayerTypes = new List<HudPlayerType>(HudPlayerTypes.Select(x => x.Clone()));
            clone.HudBumperStickerTypes = new List<HudBumperStickerType>(HudBumperStickerTypes.Select(x => x.Clone()));
            clone.HudPositions = new List<HudSavedPosition>(HudPositions.Select(x => x.Clone()));

            return clone;
        }
    }

    [Serializable]
    public class HudSavedPosition
    {
        public Point Position { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        public int Seat { get; set; }

        public HudType HudType { get; set; }

        public HudSavedPosition Clone()
        {
            var clone = (HudSavedPosition) MemberwiseClone();
            return clone;
        }
    }
}