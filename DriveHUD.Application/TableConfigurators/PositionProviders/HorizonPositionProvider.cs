﻿//-----------------------------------------------------------------------
// <copyright file="HorizonPositionProvider.cs" company="Ace Poker Solutions">
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
    public class HorizonPositionProvider : IPositionProvider
    {
        public HorizonPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {                    
                    2,
                    new int[,]
                    {
                        { 665, 251 }, { 15, 251 }
                    }
                },
                {                    
                    3,
                    new int[,]
                    {
                        { 665, 251 }, { 341, 422 }, { 15, 251 }
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
                        { 565, 101 }, { 656, 312 }, { 461, 422 }, { 211, 422 }, { 17, 312 }, { 110, 101 }
                    }
                },                
                {                    
                    8,
                    new int[,]
                    {
                        { 510, 101 }, { 656, 201 }, { 615, 388 }, { 432, 422 }, { 240, 422 }, { 58, 388 }, { 18, 201 }, { 192, 101 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 490, 101 }, { 640, 198 }, { 645, 337 }, { 511, 422 }, { 336, 422 }, { 169, 422 }, { 40, 337 }, { 40, 198 }, { 184, 101 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 474, 100 }, { 630, 139 }, { 657, 291 }, { 573, 418 }, { 415, 422 }, { 258, 422 }, { 101, 418 }, { 17, 291 }, { 49, 139 }, { 206, 100 }
                    }
                }
            };          

            PlayerLabelWidth = 125;
            PlayerLabelHeight = 45;
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