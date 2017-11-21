﻿//-----------------------------------------------------------------------
// <copyright file="GGNHudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2017Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class GGNHudPanelService : HudPanelService
    {
        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(800, 600);
        }
    }
}