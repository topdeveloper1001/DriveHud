//-----------------------------------------------------------------------
// <copyright file="HudLayoutMappings.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.Layouts
{
    /// <summary>
    /// This class represents the mappings of the layout
    /// </summary>
    [Serializable]
    public class HudLayoutMappings
    {
        /// <summary>
        /// Gets or sets the list of <see cref="HudLayoutMapping"/> the mappings
        /// </summary>
        public List<HudLayoutMapping> Mappings { get; set; } = new List<HudLayoutMapping>();
    }
}