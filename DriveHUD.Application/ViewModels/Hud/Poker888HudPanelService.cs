//-----------------------------------------------------------------------
// <copyright file="Poker888HudPanelService.cs" company="Ace Poker Solutions">
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
    internal class Poker888HudPanelService : HudPanelService
    {
        private static readonly Point initialTableSize = new Point(810, 585);

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
            { 2, new int[,] { { -61, -66 }, { -17, -34}  } },
            { 3, new int[,] { { 24, -52 }, { -17, -34}, { -39, -52 } } },            
            { 4, new int[,] { { -61, -66 }, { 24, -52}, { -17, -34 }, { -39, -52 } } },            
            { 5, new int[,] { { 102, -66 }, { 24, -62 }, { -17, -34 }, { -39, -62 }, { -61, -66 } } },
            { 6, new int[,] { { 35, -66 }, { 24, -52 }, { 35, -34 }, { -45, -34}, { -39, -52 }, { -45, -66 }  } },            
            { 8, new int[,] { { 35, -66 }, { 24, -63 }, { 24, -62 }, { 35, -34 }, { -45, -34}, { -39, -62 }, { -39, -63 }, { -45, -66 }  } },
            { 9, new int[,] { { 33, -66 }, { 24, -63 }, { 24, -62 }, { 27, -34}, { -17, -34 }, { -61, -34 }, { -39, -62 }, { -39, -63 }, { -45, -66 } } },            
            { 10, new int[,] { { 27, -66 }, { 24, -63 }, { 24, -62 }, { 27, -34}, { -17, -34 }, { -61, -34 }, { -39, -62 }, { -39, -63 }, { -61, -66 }, { -17, -66 } } }
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