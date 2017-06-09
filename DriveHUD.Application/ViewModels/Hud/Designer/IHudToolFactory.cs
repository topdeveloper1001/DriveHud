//-----------------------------------------------------------------------
// <copyright file="IHudToolFactory.cs" company="Ace Poker Solutions">
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
    /// Defines factory to create new hud tool
    /// </summary>
    internal interface IHudToolFactory
    {
        /// <summary>
        /// Creates <see cref="HudBaseToolViewModel"/> based on the specified <see cref="HudToolCreationInfo"/>
        /// </summary>
        /// <param name="creationInfo"><see cref="HudToolCreationInfo"/> to create new hud tool</param>
        /// <returns>Created <see cref="HudBaseToolViewModel"/></returns>
        HudBaseToolViewModel CreateTool(HudToolCreationInfo creationInfo);

        /// <summary>
        /// Gets the list of <see cref="HudPositionInfo"/> related to base position in designer 
        /// </summary>
        /// <param name="tableType"><see cref="EnumTableType"/> of table</param>
        /// <param name="relativeTableType"><see cref="EnumTableType"/> to which position is set</param>
        /// <param name="position"><see cref="Point"/> position of base element</param>
        /// <returns>The list of <see cref="HudPositionInfo"/></returns>
        List<HudPositionInfo> GetHudUIPositions(EnumTableType tableType, EnumTableType relativeTableType, Point position);
    }
}