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
                        { 331, 78 }, { 331, 422 }
                    }
                },
                {
                    6,
                    new[,]
                    {
                        { 331, 78 }, { 627, 164 }, { 627, 352 }, { 331, 430 }, { 30, 352 }, { 30, 164 }
                    }
                },
                {
                    9,
                    new[,]
                    {
                        { 446, 78 }, { 627, 148 }, { 641, 295 }, { 519, 416 }, { 331, 430 }, { 144, 416 }, { 19, 295 }, { 35, 148 }, { 225, 78 }
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