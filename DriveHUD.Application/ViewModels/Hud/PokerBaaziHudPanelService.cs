﻿//-----------------------------------------------------------------------
// <copyright file="PokerBaaziHudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal sealed class PokerBaaziHudPanelService : HudPanelService
    {
        private static readonly Point initialTableSize = new Point(682, 482);

        /// <summary>
        /// Gets the initial(default) size of the table 
        /// </summary>
        public override Point InitialTableSize
        {
            get
            {
                return initialTableSize;
            }
        }

        public override Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(120, -30);
        }
    }
}