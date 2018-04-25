//-----------------------------------------------------------------------
// <copyright file="WinamaxHudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using DriveHUD.Application.Views;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class WinamaxHudPanelService : HudPanelService
    {
        private static readonly Point initialTableSize = new Point(716, 540);

        /// <summary>
        /// Gets the initial(default) size of the table 
        /// </summary>
        public override Point InitialTableSize
        {
            get
            {
                return initialTableSize;
            }
        }

        /// <summary>
        /// Calculates x-scale for the specified Winamax windowk
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public override double GetScaleX(HudWindow window)
        {
            return base.GetScaleY(window);
        }
    }
}