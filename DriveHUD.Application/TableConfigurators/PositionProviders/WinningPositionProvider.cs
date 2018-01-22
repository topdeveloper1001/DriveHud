//-----------------------------------------------------------------------
// <copyright file="WinningPositionProvider.cs" company="Ace Poker Solutions">
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
    public class WinningPositionProvider : IPositionProvider
    {
        public WinningPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 400, 105 }, { 400, 547 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 636, 211 }, { 355, 409 }, { 72, 211 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 352, 105 }, { 638, 180 }, { 352, 411 }, { 96, 180 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 400, 105 }, { 761, 184 }, { 794, 471 }, { 400, 547 }, { 5, 471 }, { 38, 184 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 400, 105 }, { 761, 188 }, { 794, 375 }, { 658, 515 }, { 400, 547 }, { 141, 515 }, { 5, 375 }, { 38, 188 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 534, 105 }, { 761, 184 }, { 794, 343 }, { 658, 500 }, { 400, 547 }, { 141, 500 }, { 5, 343 }, { 38, 184 }, { 262, 105 }
                    }
                },
            };

            PlayerLabelWidth = 205;
            PlayerLabelHeight = 60;
        }

        public Dictionary<int, int[,]> Positions { get; }

        public int PlayerLabelHeight
        {
            get;
        }

        public int PlayerLabelWidth
        {
            get;
        }
    }
}