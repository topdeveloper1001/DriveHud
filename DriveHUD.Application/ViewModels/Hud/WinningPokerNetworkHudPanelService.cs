//-----------------------------------------------------------------------
// <copyright file="WinningPokerNetworkHudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Collections.Generic;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class WinningPokerNetworkHudPanelService : HudPanelService
    {
        private static readonly Point initialTableSize = new Point(1016, 759);

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

        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            { 2, new int[,] { { 63, 15 }, { 106, 168}  } },
            { 3, new  int[,] { { 181, -2 }, { 95, 164 }, { -18, 1 } } },
            { 4, new int[,] { { 63, 15 }, { 179, 29 }, { 106, 168 }, { -41, 34 } } },
            { 6, new int[,] { { 66, 28 }, { 179, 29 }, { 208, 46 }, { 109, 166}, { -70, 46 }, { -41, 34 }  } },
            { 8, new int[,] { { 66, 28 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 } } },
            { 9, new int[,] { { 185, 11 }, { 181, -2 }, { 215, 52 }, { 228, 115}, { 95, 164 }, { -43, 120 }, { -45, 52 }, { -18, 1 }, { 12, 13 } } },
        };
        
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