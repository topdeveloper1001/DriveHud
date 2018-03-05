//-----------------------------------------------------------------------
// <copyright file="PMPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
    internal class PMPositionProvider : IPositionProvider
    {
        public PMPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 246, 756 }, { 246,  56}
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 261}, { 480, 261 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 398 }, { 246,  56 }, { 480, 398 }
                    }
                },
                {
                    5,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 398 }, { 152, 76 }, { 342,  76 }, { 480, 398 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 573 }, { 11, 257 }, { 246, 56 }, { 480, 257 }, { 480, 573 }
                    }
                },
                {
                    7,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 562 }, { 11, 262 }, { 152, 76}, { 342, 76 }, { 480, 262 }, { 480, 562 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 246, 756 }, { 11, 581 }, { 11, 398}, { 11, 218 },  { 342, 76 }, { 480, 218 }, { 480, 398 }, { 480, 581 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                       { 246, 756 }, { 11, 581 }, { 11, 398 }, { 11, 218 }, { 152, 76 }, { 342, 76 }, { 480, 218 }, { 480, 398 }, { 480, 581 }
                    }
                }
            };

            PlayerLabelWidth = 67;
            PlayerLabelHeight = 67;
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