//-----------------------------------------------------------------------
// <copyright file="InsuranceBetStatus.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.PokerMaster.Model
{
    internal enum InsuranceBetStatus
    {
        INSURANCE_BET_UNDEFINED = 0,
        INSURANCE_BET_BETTING = 1,
        INSURANCE_BET_FOLD = 2,
        INSURANCE_BET_BETED = 3,
    }
}