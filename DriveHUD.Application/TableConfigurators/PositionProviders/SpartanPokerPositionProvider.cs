//-----------------------------------------------------------------------
// <copyright file="SpartanPokerPositionProvider.cs" company="Ace Poker Solutions">
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
    internal sealed class SpartanPokerPositionProvider : IPositionProvider
    {
        public SpartanPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 685, 222 }, { 16,  222}
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 685, 198 }, { 350,  369}, { 16,  198}
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 530, 63 }, { 485, 363 }, { 215, 363 }, { 170, 63 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 530, 63 }, { 685, 222 }, { 485, 364 }, { 215, 364 }, { 16, 222}, { 170, 63 }
                    }
                },
                {
                    7,
                    new int[,]
                    {
                        { 525, 63 }, { 685, 197 }, { 587, 352 }, { 350, 369 }, { 115, 352 }, { 16, 197 }, { 178, 63 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 530, 63 }, { 685, 150 }, { 655, 317}, { 455, 363 },  { 245, 363 }, { 57, 317 }, { 16, 150 }, { 170, 63 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 530, 63 }, { 685, 140 }, { 685, 287}, { 525, 358 }, { 350, 370 }, { 175, 358 }, { 16, 287 }, { 16, 140 }, { 187, 63 }
                    }
                }
            };

            PlayerLabelWidth = 100; // +10
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
