﻿//-----------------------------------------------------------------------
// <copyright file="HudLayoutInfoV2.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents HUD layout information
    /// </summary>
    [Serializable]
    public class HudLayoutInfoV2
    {
        /// <summary>
        /// Gets or sets the name of the layout
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the layout is default
        /// </summary>
        [XmlAttribute]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EnumTableType"/> type of table of the layout
        /// </summary>
        [XmlAttribute]
        public EnumTableType TableType { get; set; }

        /// <summary>
        /// Gets or sets the opacity of the layout
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPlayerType"/> player types of the layout
        /// </summary>
        public List<HudPlayerType> HudPlayerTypes { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudBumperStickerType"/> bumper stickers of the layout
        /// </summary>
        public List<HudBumperStickerType> HudBumperStickerTypes { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudLayoutTool"/> tools of the layout
        /// </summary>
        public List<HudLayoutTool> LayoutTools { get; set; }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutInfoV2"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutInfoV2"/> instance</returns>
        public HudLayoutInfoV2 Clone()
        {
            var cloned = new HudLayoutInfoV2
            {
                Name = Name,
                IsDefault = IsDefault,
                Opacity = Opacity,
                TableType = TableType,
                HudBumperStickerTypes = HudBumperStickerTypes.Select(x => x.Clone()).ToList(),
                HudPlayerTypes = HudPlayerTypes.Select(x => x.Clone()).ToList(),
                LayoutTools = LayoutTools.Select(x => x.Clone()).ToList()
            };

            return cloned;
        }
    }
}