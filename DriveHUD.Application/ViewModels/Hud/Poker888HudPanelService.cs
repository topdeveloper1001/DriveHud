//-----------------------------------------------------------------------
// <copyright file="Poker888HudPanelService.cs" company="Ace Poker Solutions">
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
    internal class Poker888HudPanelService : HudPanelService
    {
        private readonly Dictionary<int, int[,]> plainPositionsShifts = new Dictionary<int, int[,]>
        {
            // done
            { 2, new int[,] { { -61, -66 }, { -17, -34}  } },
            // done
            { 3, new int[,] { { 24, -52 }, { -17, -34}, { -39, -52 } } },
            // done
            { 4, new int[,] { { -61, -66 }, { 24, -52}, { -17, -34 }, { -39, -52 } } },
            // done 
            { 5, new int[,] { { 102, -66 }, { 24, -62 }, { -17, -34 }, { -39, -62 }, { -61, -66 } } },
            // done
            { 6, new int[,] { { 35, -66 }, { 24, -52 }, { 35, -34 }, { -45, -34}, { -39, -52 }, { -45, -66 }  } },
            // done
            { 8, new int[,] { { 35, -66 }, { 24, -63 }, { 24, -62 }, { 35, -34 }, { -45, -34}, { -39, -62 }, { -39, -63 }, { -45, -66 }  } },
            // done
            { 9, new int[,] { { 33, -66 }, { 24, -63 }, { 24, -62 }, { 27, -34}, { -17, -34 }, { -61, -34 }, { -39, -62 }, { -39, -63 }, { -45, -66 } } },
            // done
            { 10, new int[,] { { 27, -66 }, { 24, -63 }, { 24, -62 }, { 27, -34}, { -17, -34 }, { -61, -34 }, { -39, -62 }, { -39, -63 }, { -61, -66 }, { -17, -66 } } }
        };
        
        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public override Tuple<double, double> GetInitialTableSize()
        {            
            return new Tuple<double, double>(810, 585);
        }      
    }
}