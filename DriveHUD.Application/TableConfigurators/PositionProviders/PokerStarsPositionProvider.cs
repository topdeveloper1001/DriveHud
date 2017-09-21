//-----------------------------------------------------------------------
// <copyright file="PokerStarsPositionProvider.cs" company="Ace Poker Solutions">
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
    public class PokerStarsPositionProvider : IPositionProvider
    {
        public PokerStarsPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 320, 50 }, { 320, 360 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 320, 50 }, { 541, 422 }, { 161, 422 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 320, 50 }, { 626, 179 }, { 320, 360 }, { 4, 179 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 616, 106 }, { 616, 279 }, { 320, 360 }, { 23, 279 }, { 23, 106 }, { 320, 50 }
                    }
                },
                {
                8, 
                    new int[,]
                    {
                        { 440, 43 }, { 619, 136 }, { 619, 250 }, { 426, 352 }, { 208, 352 }, { 16, 250 }, { 16, 136 }, { 194, 43 }
                    }
                },
                {
                9,
                    new int[,]
                    {
                        { 435, 50 }, { 591, 105 }, { 635, 218 }, { 543, 334 }, { 320, 376 }, { 92, 334 }, { 1, 218 }, { 44, 105 }, { 200, 50 }
                    }
                },
                {
                10,
                    new int[,]
                    {
                        { 434, 50 }, { 592, 99 }, { 635, 203 }, { 600, 321 }, { 438, 364 }, { 196, 364 }, { 35, 321 }, { 1, 203 }, { 43, 99 }, { 200, 50 }
                    }
                }
        };

            PlayerLabelWidth = 160;
            PlayerLabelHeight = 50;
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