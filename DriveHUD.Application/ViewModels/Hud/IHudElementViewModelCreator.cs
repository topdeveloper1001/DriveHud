//-----------------------------------------------------------------------
// <copyright file="IHudElementViewModelCreator.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Creates HUD element based on elements on table
    /// </summary>
    public interface IHudElementViewModelCreator
    {
        /// <summary>
        /// Create HUD element based on table settings
        /// </summary>
        /// <param name="creationInfo">Creation Info</param>      
        /// <returns>Hud panel element view model</returns>
        HudElementViewModel Create(HudElementViewModelCreationInfo creationInfo);
    }
}