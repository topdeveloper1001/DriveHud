//-----------------------------------------------------------------------
// <copyright file="IgnitionPositionProvider.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Application.TableConfigurators.PositionProviders
{
    /// <summary>
    /// Provides positions and other specific data for Ignition based clients
    /// </summary>
    public class IgnitionPositionProvider : IPositionProvider
    {
        public IgnitionPositionProvider()
        {
            Positions = new Dictionary<int, int[,]>
            {
                {
                    2,
                    new[,]
                    {
                        { 331, 98 }, { 331, 442 }
                    }
                },
                {
                    6,
                    new[,]
                    {
                        { 331, 98 }, { 627, 184 }, { 627, 372 }, { 331, 450 }, { 30, 372 }, { 30, 184 }
                    }
                },
                {
                    9,
                    new[,]
                    {
                        { 446, 98 }, { 627, 168 }, { 641, 315 }, { 519, 436 }, { 331, 450 }, { 144, 436 }, { 19, 315 }, { 35, 168 }, { 225, 98 }
                    }
                }
            };

            PlayerLabelHeight = 37;
            PlayerLabelWidth = 153;
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