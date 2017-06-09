//-----------------------------------------------------------------------
// <copyright file="HudLayoutMapping.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents mapping between hud layout and various poker sites, table types and game types
    /// </summary>
    [Serializable]
    public class HudLayoutMapping
    {
        /// <summary>
        /// Gets or sets the <see cref="EnumTableType"/> type of the table of the mapping
        /// </summary>
        public EnumTableType TableType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EnumGameType"/> type of the game of the mapping
        /// </summary>
        public EnumGameType? GameType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EnumPokerSites"/> the poker site of the mapping
        /// </summary>
        public EnumPokerSites? PokerSite { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="HudViewType"/> type of hud view of the mapping
        /// </summary>
        public HudViewType HudViewType { get; set; }

        /// <summary>
        /// Gets or sets the name of layout of the mapping
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of file of the layout of the mapping
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the layout is selected for current poker site, table type and game type
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the layout is default for current poker site, table type and game type
        /// </summary>
        public bool IsDefault { get; set; }
    }
}