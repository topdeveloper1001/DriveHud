//-----------------------------------------------------------------------
// <copyright file="HudPositionsInfo.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents information about positions for specific poker site and game type
    /// </summary>
    [Serializable]
    public class HudPositionsInfo
    {
        /// <summary>
        /// Gets or sets the <see cref="EnumPokerSites"/> poker site
        /// </summary>
        [XmlAttribute]
        public EnumPokerSites PokerSite { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EnumGameType"/> game type
        /// </summary>
        [XmlAttribute]
        public EnumGameType GameType { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionInfo"/> positions
        /// </summary>
        public List<HudPositionInfo> HudPositions { get; set; } = new List<HudPositionInfo>();

        /// <summary>
        /// Creates a copy of current <see cref="HudPositionsInfo"/>
        /// </summary>
        /// <returns>Copy of current <see cref="HudPositionsInfo"/></returns>
        public HudPositionsInfo Clone()
        {
            return new HudPositionsInfo
            {
                PokerSite = PokerSite,
                GameType = GameType,
                HudPositions = HudPositions.Select(p => p.Clone()).ToList()
            };
        }
    }
}