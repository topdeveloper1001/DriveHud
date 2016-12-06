//-----------------------------------------------------------------------
// <copyright file="BovadaHudPanelService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common;
using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Service to calculate hud positions for Bovada
    /// </summary>
    internal class BovadaHudPanelService : HudPanelService
    {
        /// <summary>
        /// Calculates hudElement position in window
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public override Tuple<double, double> CalculatePositions(HudElementViewModel hudElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => hudElement);
            Check.ArgumentNotNull(() => window);

            var panelOffset = window.GetPanelOffset(hudElement);

            // temporary workaround
            var extraOffsetY = 27;

            var offsetX = panelOffset.X != 0 ? panelOffset.X : (hudElement.Position.X);
            var offsetY = panelOffset.Y != 0 ? panelOffset.Y : (hudElement.Position.Y - extraOffsetY);

            // linear formula to adjust position on big sizes
            var additionOffsetY = 35.4 * window.YFraction - 35.4;

            return new Tuple<double, double>(offsetX * window.XFraction, offsetY * window.YFraction + additionOffsetY);
        }

        /// <summary>
        /// Converts offset values into position value
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public override Tuple<double, double> GetOffsetPosition(HudElementViewModel hudElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => hudElement);
            Check.ArgumentNotNull(() => window);

            var panelOffset = window.GetPanelOffset(hudElement);

            // temporary workaround
            var extraOffsetY = 27;

            var offsetX = panelOffset.X != 0 ? panelOffset.X : (hudElement.Position.X);
            var offsetY = panelOffset.Y != 0 ? panelOffset.Y + extraOffsetY : (hudElement.Position.Y);

            // linear formula to adjust position on big sizes
            var additionOffsetY = 35.4 * window.YFraction - 35.4;

            return new Tuple<double, double>(offsetX, offsetY);
        }
    }
}