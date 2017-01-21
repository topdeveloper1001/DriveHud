﻿//-----------------------------------------------------------------------
// <copyright file="PokerStarsTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.TableConfigurators.SeatArea;
using DriveHUD.Entities;
using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators
{
    internal class PokerStarsTableConfigurator : Poker888TableConfigurator
    {     
        protected override Dictionary<int, int[,]> GetPredefinedLabelPositions()
        {
            var predefinedLablelPositions = new Dictionary<int, int[,]>
            {
                { 2, new int[,] { { 352, 105 }, { 352, 411 } } },
                { 3, new int[,] { { 352, 105 }, { 541, 422 }, { 161, 422 } } },
                { 4, new int[,] { { 352, 105 }, { 660, 256 }, { 352, 411 }, { 57, 256 } } },
                { 6, new int[,] { { 352, 105 }, { 638, 180 }, { 638, 353 }, { 352, 411 }, { 96, 353 }, { 96, 180 } } },
                { 8, new int[,] { { 352, 105 }, { 529, 128 }, { 698, 258 }, { 529, 393 }, { 352, 411 }, { 196, 393 },  { 13, 258 }, { 194, 122 } } },
                { 9, new int[,] { { 541, 113 }, { 670, 200 }, { 670, 342 }, { 541, 422 }, { 352, 411 }, { 161, 422 }, { 40, 342 }, { 40, 200 }, { 161, 113 }  } },
                { 10, new int[,] { { 494, 106 }, { 639, 150 }, { 688, 269 }, { 639, 376 }, { 494, 422}, { 235, 422 }, { 57, 376 }, { 15, 269 }, { 57, 150 }, { 235, 106 } } }
            };            

            return predefinedLablelPositions;
        }
    }
}