//-----------------------------------------------------------------------
// <copyright file="Poker888PositionProvider.cs" company="Ace Poker Solutions">
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
    public class Poker888PositionProvider : IPositionProvider
    {
        public Poker888PositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 311, 40 }, { 311, 363 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 633, 198 }, { 311, 363 }, { 6, 198 }
                    }
                },
                {
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
                        { 490, 40 }, { 633, 244 }, { 311, 363 }, { 6, 244 }, { 132, 40 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 430, 40 }, { 633, 198 }, { 430, 363 }, { 192, 363 }, { 6, 198 }, { 192, 40 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 430, 40 }, { 633, 136 }, { 633, 244 }, { 430, 363 }, { 192, 363 }, { 6, 244 }, { 6, 136 }, { 192, 40 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 420, 40 }, { 633, 136 }, { 633, 244 }, { 490, 363 }, { 311, 363 }, { 132, 363 }, { 6, 244 }, { 6, 136 }, { 201, 40 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 490, 40 }, { 633, 136 }, { 633, 244 }, { 490, 363 }, { 311, 363 }, { 132, 363 }, { 6, 244 }, { 6, 136 }, { 132, 40 }, { 311, 40 }
                    }
                }
            };

            PlayerLabelWidth = 155;
            PlayerLabelHeight = 46;
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