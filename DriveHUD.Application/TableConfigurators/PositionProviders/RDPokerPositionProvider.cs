//-----------------------------------------------------------------------
// <copyright file="RDPokerPositionProvider.cs" company="Ace Poker Solutions">
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
    internal class RDPokerPositionProvider : IPositionProvider
    {
        public RDPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                         { 237, 813 }, { 237, 165 }
                    }
                },              
                {
                    6,
                    new int[,]
                    {
                        { 237, 813 }, { 8, 651 }, { 8, 343 }, { 237, 165 }, { 419, 343 }, { 419, 651 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 237, 813 }, { 8, 655 }, { 8, 478 }, { 8, 301 }, { 237, 165 },  { 419, 301 }, { 419, 478 }, { 419, 655 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 237, 813 }, { 8, 655 }, { 8, 478 }, { 8, 301 }, { 187, 165 }, { 287, 165 },  { 419, 301 }, { 419, 478 }, { 419, 655 }
                    }
                }
            };

            PlayerLabelWidth = 80;
            PlayerLabelHeight = 40;
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