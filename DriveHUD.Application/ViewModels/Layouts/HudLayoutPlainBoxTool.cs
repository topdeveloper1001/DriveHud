//-----------------------------------------------------------------------
// <copyright file="HudLayoutPlainBoxTool.cs" company="Ace Poker Solutions">
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
using DriveHUD.ViewModels;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the plain box tool of the hud
    /// </summary>
    public class HudLayoutPlainBoxTool : HudLayoutTool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HudLayoutPlainBoxTool"/> class
        /// </summary>
        public HudLayoutPlainBoxTool()
        {
            ToolType = HudDesignerToolType.PlainStatBox;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="StatInfo"/> stats of the plain box tool
        /// </summary>
        public List<StatInfo> Stats { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionsInfo"/> positions of the plain box tool on hud
        /// </summary>
        public List<HudPositionsInfo> Positions { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionInfo"/> UI positions of the plain box tool
        /// </summary>
        public List<HudPositionInfo> UIPositions { get; set; }
    }
}