//-----------------------------------------------------------------------
// <copyright file="WinningPokerNetworkHudPanelService.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class WinningPokerNetworkHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { 63, 15 }, { 106, 168}  } },
            { 3, new  int[,] { { 181, -2 }, { 95, 164 }, { -18, 1 } } },
            { 4, new int[,] { { 63, 15 }, { 179, 29 }, { 106, 168 }, { -41, 34 } } },
            { 6, new int[,] { { 66, 28 }, { 179, 29 }, { 208, 46 }, { 109, 166}, { -70, 46 }, { -41, 34 }  } },
            { 8, new int[,] { { 66, 28 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 } } },
            { 9, new int[,] { { 185, 11 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 }, { 12, 13 } } },
        };

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(1016, 759);
        }
    }
}