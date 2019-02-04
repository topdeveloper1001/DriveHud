//-----------------------------------------------------------------------
// <copyright file="PokerBaaziPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
    internal sealed class PokerBaaziPositionProvider : IPositionProvider
    {
        public PokerBaaziPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 38, 173 }, { 523,  173}
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 38, 173 }, { 147, 76 }, { 399, 76 }, { 523, 173 }, { 399, 288 }, { 147, 288 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 35, 218 }, { 35, 139 }, { 133, 76}, { 402, 76 }, { 528, 139 }, { 528, 218 }, { 433, 288 }, { 274, 302 }, { 120, 288 }
                    }
                }
            };

            PlayerLabelWidth = 128;
            PlayerLabelHeight = 38;
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