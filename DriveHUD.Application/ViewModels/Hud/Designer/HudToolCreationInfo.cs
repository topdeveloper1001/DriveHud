﻿//-----------------------------------------------------------------------
// <copyright file="HudToolCreationInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Entities;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Defines data required to create new hud tool
    /// </summary>
    internal class HudToolCreationInfo
    {
        /// <summary>
        /// Gets or sets <see cref="HudDesignerToolType"/>
        /// </summary>
        public HudDesignerToolType ToolType { get; set; }

        /// <summary>
        /// Gets or sets <see cref="HudElementViewModel"/>
        /// </summary>
        public HudElementViewModel HudElement { get; set; }

        /// <summary>
        /// Gets or sets <see cref="EnumTableType"/>
        /// </summary>
        public EnumTableType TableType { get; set; }

        /// <summary>
        /// Gets or sets <see cref="Point"/>
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets <see cref="HudLayoutInfoV2"/>
        /// </summary>
        public HudLayoutInfoV2 Layout { get; set; }

        /// <summary>
        /// Gets or sets list of tools to add new tool
        /// </summary>
        public IList<HudLayoutTool> Tools { get; set; }

        /// <summary>
        /// Gets or sets the source of drag and drop target
        /// </summary>
        public object Source { get; set; }
    }
}