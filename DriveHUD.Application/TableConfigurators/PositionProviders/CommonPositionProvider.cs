//-----------------------------------------------------------------------
// <copyright file="CommonPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    /// <summary>
    /// Provides positions and other specific data for BetOnline based clients
    /// </summary>
    public class CommonPositionProvider : IPositionProvider
    {
        public CommonPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new[,] { { 324, 64 }, { 324, 389 } }
                },
                // not presented in BOL
                {
                    3,
                    new[,] { { 352, 105 }, { 541, 422 }, { 161, 422 }}
                },
                {
                    4,
                    new[,] { { 324, 64 }, { 642, 225 }, { 324, 389 }, { 6, 225 } }
                },
                // not presented in BOL
                {
                    5,
                    new[,] { { 352, 105 }, { 660, 256 }, { 352, 411 }, { 57, 256 }, { 0, 0 } }
                },
                {
                    6,
                    new[,] { { 324, 64 }, { 608, 135 }, { 608, 325 }, { 324, 389 }, { 39, 325 }, { 39, 135 } }
                },
                {
                    8,
                    new[,] { { 324, 64 }, { 519, 115 }, { 642, 225 }, { 519, 353 }, { 324, 389 }, { 128, 353 }, { 1, 225 }, { 128, 115 } }
                },
                {
                    9,
                    new[,]
                    {
                        { 438, 68}, { 624, 140 }, { 641, 255 }, { 509, 366 }, { 324, 389 }, { 138, 366 }, { 6, 255 }, { 24, 140 }, { 208, 68 }
                    }
                },
                {
                    10,
                    new[,]
                    {
                        { 324, 64 }, { 504, 88 }, { 642, 175 }, { 642, 286 }, { 504, 368 }, { 324, 389 }, { 144, 368 }, { 6, 286 }, { 6, 175 }, { 144, 88 }
                    }
                }
            };

            PlayerLabelHeight = 52;
            PlayerLabelWidth = 155;
        }

        public Dictionary<int, int[,]> Positions { get; }

        public int PlayerLabelHeight { get; }

        public int PlayerLabelWidth { get; }
    }
}