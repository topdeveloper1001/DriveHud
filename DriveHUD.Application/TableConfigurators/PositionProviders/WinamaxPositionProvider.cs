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
                        { 293, 370 }, { 293, 65 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 293, 370 }, { 55, 103 }, { 531, 103 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 293, 370 }, { 25, 217 }, { 293, 65 }, { 560, 217 }
                    }
                },
                {
                    5,
                    new int[,]
                    {
                        { 293, 370 }, { 37, 288 }, { 135, 65 }, { 451, 65 }, { 549, 288 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 293, 370 }, { 56, 332 }, { 5, 102 }, { 293, 65 }, { 531, 102 }, { 531, 332 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 293, 370 }, { 96, 370 }, { 25, 217 }, { 96, 65 }, { 293, 65 }, { 490, 65 }, { 560, 217 }, { 490, 370 }
                    }
                },
                {

                    9,
                    new int[,]
                    {
                        { 293, 370 }, { 119, 370 }, { 30, 257 }, { 55, 102 }, { 205, 65 }, { 381, 65 }, { 531, 102 }, { 557, 257 }, { 468, 370 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 293, 370 }, { 136, 370 }, { 37, 288 }, { 37, 147 }, { 136, 65 }, { 293, 65 }, { 451, 65 }, { 549, 147 }, { 549, 288 }, { 451, 170 }
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