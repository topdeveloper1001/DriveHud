﻿//-----------------------------------------------------------------------
// <copyright file="PlayerPositionType.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.GGNetwork.Model
{
    [Flags]
    public enum PlayerPositionType
    {
        None = 0,
        Dealer = 1,
        SmallBlind = 2,
        BigBlind = 4,
        Straddle = 8
    }
}