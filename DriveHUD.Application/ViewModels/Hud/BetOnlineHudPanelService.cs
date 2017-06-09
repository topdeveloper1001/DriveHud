//-----------------------------------------------------------------------
// <copyright file="BetOnlineHudPanelService.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Service to calculate hud positions for BetOnline
    /// </summary>
    internal class BetOnlineHudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { -2, -9 }, { -2, 26}  } },
            { 3, new int[,] { { -2, -24 }, { 9, 1 }, { -18, 1 } } },
            { 4, new int[,] { { 0, -26 }, { 0, -13 }, { -22, 2 }, { -22, -6 } } },
            { 6, new int[,] { { -2, -4 }, { 0, -30 }, { 0, -12 }, { -2, 31}, { -22, 2 }, { -22, -28 }  } },
            { 8, new int[,] { { 0, -16 }, { 18, -2 }, { -28, -9 }, { 18, -19 }, { 0, -2 }, { -32, -16 }, { 20, -9 }, { -38, 11 } } },
            { 9, new int[,] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } } },
            { 10, new int[,] { { 0, -36 }, { 0, -28 }, { 9, 1 }, { -2, 1}, { -18, 1 }, { -22, -28 }, { -22, -36 }, { 18, -24 }, { -2, -24 }, { -27, -24 } } }
        };

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(816, 631);
        }

        public override Point GetPositionShift(EnumTableType tableType, int seat)
        {
            var tableSize = (int)tableType;

            if (!plainPositionsShifts.ContainsKey(tableSize))
            {
                return base.GetPositionShift(tableType, seat);
            }

            var shift = plainPositionsShifts[tableSize];

            return new Point(shift[seat - 1, 0], shift[seat - 1, 1]);
        }
    }
}