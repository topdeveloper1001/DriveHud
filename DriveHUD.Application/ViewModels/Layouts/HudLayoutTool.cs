﻿//-----------------------------------------------------------------------
// <copyright file="HudLayoutTool.cs" company="Ace Poker Solutions">
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
using System;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents base tool in hud layout
    /// </summary>
    [Serializable]
    [XmlInclude(typeof(HudLayoutPlainBoxTool))]
    [XmlInclude(typeof(HudLayoutTextBoxTool))]
    public abstract class HudLayoutTool
    {
        /// <summary>
        /// Gets the type <see cref="HudDesignerToolType"/> of the tool
        /// </summary>     
        [XmlIgnore()]
        public HudDesignerToolType ToolType { get; protected set; }

        /// <summary>
        /// Creates a copy of the current <see cref="HudLayoutTool"/> instance
        /// </summary>
        /// <returns>Copy of the current <see cref="HudLayoutTool"/> instance</returns>
        public abstract HudLayoutTool Clone();
    }
}