//-----------------------------------------------------------------------
// <copyright file="PPPokerPositionProvider.cs" company="Ace Poker Solutions">
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
    internal class PPPokerPositionProvider : IPositionProvider
    {
        public PPPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                         { 189,  101}, { 189, 689 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 364, 232 }, { 189, 689 }, { 14, 232}
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 366, 260 }, { 366, 490 }, { 189, 689 }, { 12, 490 }, { 12, 260 }, { 189, 101 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 276, 100 }, { 366, 259 }, { 366, 445 }, { 366, 590 }, { 189, 689 },  { 12, 590 }, { 12, 445 }, { 12, 259 }, { 103, 100 }
                    }
                }
            };

            PlayerLabelWidth = 74;
            PlayerLabelHeight = 37;
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