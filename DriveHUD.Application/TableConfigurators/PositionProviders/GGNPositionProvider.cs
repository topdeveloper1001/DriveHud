//-----------------------------------------------------------------------
// <copyright file="GGNPositionProvider.cs" company="Ace Poker Solutions">
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
    public class GGNPositionProvider : IPositionProvider
    {
        public GGNPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    4,
                    new int[,]
                    {
                        { 349,  91 }, { 672, 259 }, { 349, 515 }, { 24, 259 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 348, 84 }, { 656, 147 }, { 672, 380 }, { 348, 508 }, { 24, 380 }, { 48, 147 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                       { 256, 93 }, { 450, 93 }, { 616, 148 }, { 684, 308 }, { 560, 433 }, { 348, 516 }, { 136, 433 }, { 16, 308 }, { 80, 148 }
                    }
                }
            };

            PlayerLabelWidth = 104;
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