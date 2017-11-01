//-----------------------------------------------------------------------
// <copyright file="GGNDataType.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork
{
    internal enum GGNDataType
    {
        CashGameHandHistory = 0,
        CashGameHandHistories = 1,
        TourneyHandHistory = 2,
        TourneyHandHistories = 3,
        TourneyInfo = 4,
        Unknown = 5
    }
}