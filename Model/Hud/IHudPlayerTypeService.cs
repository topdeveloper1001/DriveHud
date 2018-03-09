//-----------------------------------------------------------------------
// <copyright file="IHudPlayerTypeService.cs" company="Ace Poker Solutions">
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
using Model.Data;
using System.Collections.Generic;

namespace Model.Hud
{
    public interface IHudPlayerTypeService
    {
        /// <summary>
        /// Gets default player types for the specified <see cref="EnumTableType"/>
        /// </summary>
        /// <param name="tableType">Table type to get player types</param>
        /// <returns>The collection of <see cref="HudPlayerType"/></returns>
        IEnumerable<HudPlayerType> CreateDefaultPlayerTypes(EnumTableType tableType);

        /// <summary>
        /// Get path to image directory
        /// </summary>
        /// <returns>Path to image directory</returns>
        string GetImageDirectory();

        /// <summary>
        /// Get link to image
        /// </summary>
        /// <param name="image">Image alias</param>
        /// <returns>Full path to image</returns>
        string GetImageLink(string image);

        /// <summary>
        /// Check whenever players <see cref="Indicators"/> does match the specified player type
        /// </summary>
        /// <param name="playerIndicators">Indicators to match</param>
        /// <param name="playerType">Player type to match</param>
        /// <param name="strictMatch">Determinse whenever player <see cref="Indicators"/> must strictly matches player type</param>
        /// <returns><see cref="HudPlayerType"/> if matches; otherwise - null</returns>
        HudPlayerType Match(Indicators playerIndicators, IEnumerable<HudPlayerType> playerType, bool strictMatch);
    }
}