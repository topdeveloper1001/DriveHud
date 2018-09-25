//-----------------------------------------------------------------------
// <copyright file="Adda52PositionProvider.cs" company="Ace Poker Solutions">
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
    internal sealed class Adda52PositionProvider : IPositionProvider
    {
        public Adda52PositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 677, 227 }, { 10,  227}
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 525, 98 }, { 525, 357 }, { 162, 357 }, { 162, 98 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 525, 98 }, { 677, 227 }, { 525, 357 }, { 162, 357 }, { 10, 227 }, { 162, 98 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 500, 91 }, { 677, 170 }, { 671, 278}, { 500, 361 },  { 187, 361 }, { 15, 278 }, { 10, 174 }, { 187, 91 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 500, 123 }, { 677, 170 }, { 671, 278}, { 525, 361 }, { 344, 372 }, { 162, 361 }, { 15, 278 }, { 10, 174 }, { 187, 91 } 
                    }
                }
            };

            PlayerLabelWidth = 143;
            PlayerLabelHeight = 44;
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