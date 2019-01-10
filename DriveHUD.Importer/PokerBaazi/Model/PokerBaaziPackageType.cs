﻿//-----------------------------------------------------------------------
// <copyright file="PokerBaaziPackageType.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.PokerBaazi.Model
{
    internal enum PokerBaaziPackageType
    {
        Unknown = 0,
        InitResponse = 1,
        SpectatorResponse = 2,
        UserButtonActionResponse = 3,
        RoundResponse = 4,
        WinnerResponse = 5
    }
}