//-----------------------------------------------------------------------
// <copyright file="IPokerPositionProvider.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    public class IPokerPositionProvider : IPositionProvider
    {
        public IPokerPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 414, 73 }, { 414, 501 }
                    }
                },
                {
                    3,
                    new int[,]
                    {
                        { 770, 145 }, { 414, 501 }, { 50, 145 }
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 320, 50 }, { 626, 179 }, { 320, 360 }, { 4, 179 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 770, 145 }, { 770, 420 }, { 414, 501 }, { 50, 420 }, { 50, 145 }, { 414, 73 }
                    }
                },
                {
                    9,
                    new int[,]
                    {
                        { 551, 73 }, { 770, 146 }, { 815, 299 }, { 716, 451 }, { 415, 501 }, { 105, 451 }, { 5, 299 }, { 50, 146 }, { 269, 73 }
                    }
                },
                {
                    10,
                    new int[,]
                    {
                        { 434, 50 }, { 592, 99 }, { 635, 203 }, { 600, 321 }, { 438, 364 }, { 196, 364 }, { 35, 321 }, { 1, 203 }, { 43, 99 }, { 200, 50 }
                    }
                }
            };

            PlayerLabelWidth = 205;
            PlayerLabelHeight = 75;
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