//-----------------------------------------------------------------------
// <copyright file="PKPositionProvider.cs" company="Ace Poker Solutions">
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
    internal class PKPositionProvider : IPositionProvider
    {
        public PKPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new int[,]
                    {
                        { 246, 746 }, { 246,  57}
                    }
                },
                {
                    4,
                    new int[,]
                    {
                        { 246, 746 }, { 20, 360 }, { 246, 57 }, { 475, 360 }
                    }
                },
                {
                    6,
                    new int[,]
                    {
                        { 246, 746 }, { 20, 540 }, { 20, 246 }, { 246, 57 }, { 475, 246 }, { 475, 540 }
                    }
                },
                {
                    7,
                    new int[,]
                    {
                        { 246, 746 }, { 20, 541 }, { 20, 285 }, { 146, 57}, { 348, 57 }, { 475, 285 }, { 475, 541 }
                    }
                },
                {
                    8,
                    new int[,]
                    {
                        { 246, 746 }, { 20, 582 }, { 20, 390}, { 20, 216 },  { 246, 57 }, { 475, 216 }, { 475, 390 }, { 475, 582 }
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