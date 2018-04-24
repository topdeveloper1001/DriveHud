//-----------------------------------------------------------------------
// <copyright file="WinamaxPositionProvider.cs" company="Ace Poker Solutions">
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
    public class WinamaxPositionProvider : IPositionProvider
    {
        public WinamaxPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 293, 402 }, { 293, 97 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 293, 402 }, { 55, 135 }, { 531, 135 }
                    }
                },
                {
                    // not done
                    4,
                    new int[,]
                    {
                        { 311, 40 }, { 633, 198 }, { 311, 363 }, { 6, 198 }
                    }
                },
                {
                    5,
                    new int[,]
                    {
                        { 293, 402 }, { 37, 320 }, { 135, 97 }, { 451, 97 }, { 549, 320 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 293, 402 }, { 56, 364 }, { 5, 135 }, { 293, 97 }, { 531, 135 }, { 531, 364 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 293, 402 }, { 96, 402 }, { 25, 249 }, { 96, 97 }, { 293, 402 }, { 490, 402 }, { 560, 249 }, { 490, 101 }
                    }
                },
                {

                    9,
                    new int[,]
                    {
                        { 293, 402 }, { 119, 402 }, { 30, 289 }, { 55, 135 }, { 205, 97 }, { 381, 97 }, { 531, 135 }, { 557, 289 }, { 468, 402 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 293, 402 }, { 136, 402 }, { 37, 320 }, { 37, 179 }, { 136, 97 }, { 293, 97 }, { 451, 97 }, { 549, 179 }, { 549, 320 }, { 451, 402 }
                    }
                }
            };

            PlayerLabelWidth = 131;
            PlayerLabelHeight = 42;
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