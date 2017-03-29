//-----------------------------------------------------------------------
// <copyright file="HudLayoutTextboxTool.cs" company="Ace Poker Solutions">
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
using System.Collections.Generic;
using System;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the text box tool of the hud
    /// </summary>
    public class HudLayoutTextBoxTool : HudLayoutTool
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HudLayoutTextBoxTool"/> class
        /// </summary>
        public HudLayoutTextBoxTool()
        {
            ToolType = HudDesignerToolType.TextBox;
        }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionsInfo"/> positions of the text box tool on hud
        /// </summary>
        public List<HudPositionsInfo> Positions { get; set; }

        /// <summary>
        /// Gets or sets the list of <see cref="HudPositionInfo"/> UI positions of the plain box tool
        /// </summary>
        public List<HudPositionInfo> UIPositions { get; set; }

        /// <summary>
        /// Gets or sets the text content of the text box tool
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutTextBoxTool"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutTool"/> instance</returns>
        public override HudLayoutTool Clone()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a view model of the current <see cref="HudBaseToolViewModel"/> instance
        /// </summary>
        public override HudBaseToolViewModel CreateViewModel(HudElementViewModel hudElement)
        {
            throw new NotImplementedException();
        }
    }
}