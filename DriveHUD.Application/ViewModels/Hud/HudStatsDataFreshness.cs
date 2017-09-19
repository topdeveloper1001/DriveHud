//-----------------------------------------------------------------------
// <copyright file="HudStatsDataFreshness.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Application.ViewModels.Hud
{
    public enum HudStatsDataFreshness : int
    {
        All = 0,
        Last30Days = 30,
        Last60Days = 60,
        Last90Days = 90,
        Last120Days = 120,
        LastYear = 365,        
    }
}