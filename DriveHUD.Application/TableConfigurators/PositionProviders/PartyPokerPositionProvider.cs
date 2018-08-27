//-----------------------------------------------------------------------
// <copyright file="PartyPokerPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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
    public class PartyPokerPositionProvider : IPositionProvider
    {
        public Dictionary<int, int[,]> Positions { get; }

        public int PlayerLabelHeight
        {
            get;
        }

        public int PlayerLabelWidth
        {
            get;
        }

        public PartyPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 318, 39 }, { 316, 356 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 604, 102 }, { 316, 356 }, { 32, 102 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 440, 17 }, { 416, 310 }, { 245, 310 }, { 218, 17 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 318, 39 }, { 624, 106 }, { 624, 272 }, { 316, 356 }, { 8, 272 }, { 8, 106 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 443, 39 }, { 624, 115 }, { 624, 272 }, { 443, 356 }, { 192, 356 }, { 8, 272 }, { 8, 115 }, { 192, 39 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 450, 39 }, { 603, 94 }, { 624, 199 }, { 572, 332 }, { 316, 356 }, { 72, 332 }, { 6, 199 }, { 30, 94 }, { 184, 39 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 450, 39 }, { 603, 94 }, { 624, 199 }, { 599, 305 }, { 417, 356 }, { 217, 356 }, { 36, 305 }, { 6, 199 }, { 31 , 94 }, { 185, 39 }
                    }
                }
            };

            PlayerLabelWidth = 170;
            PlayerLabelHeight = 53;
        }
    }
}