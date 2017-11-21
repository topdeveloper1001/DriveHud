//-----------------------------------------------------------------------
// <copyright file="ActionType.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.GGNetwork.Model
{
    public enum ActionType
    {
        Unknown = 0,
        Check = 1,
        Call = 2,
        Bet = 3,
        Raise = 4,
        Fold = 5
    }
}