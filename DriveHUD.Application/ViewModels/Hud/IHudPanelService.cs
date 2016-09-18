﻿//-----------------------------------------------------------------------
// <copyright file="IHudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Views;
using Model.Enums;
using System;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Creates Hud Panels 
    /// </summary>
    public interface IHudPanelService
    {
        /// <summary>
        /// Create Hud Panel based on specified HUD element view model
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <returns>HUD panel</returns>
        FrameworkElement Create(HudElementViewModel hudElement, HudType hudType);

        /// <summary>
        /// Calculates hudElement position in window
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        Tuple<double, double> CalculatePositions(HudElementViewModel hudElement, HudWindow window);

        /// <summary>
        /// Get handle of window on which hud has to be attached
        /// </summary>
        /// <returns>Handle of window</returns>
        IntPtr GetWindowHandle(IntPtr handle);

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        Tuple<double, double> GetInitialTableSize();

        /// <summary>
        /// Get initial track condition meter positions
        /// </summary>
        /// <returns>Item1 - X, Item2 - Y</returns>
        Tuple<double, double> GetInitialTrackConditionMeterPosition();
    }
}