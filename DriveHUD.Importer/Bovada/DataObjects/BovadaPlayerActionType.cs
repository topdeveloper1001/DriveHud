//-----------------------------------------------------------------------
// <copyright file="BovadaPlayerActionType.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Bovada player action types (based on bovada stream)
    /// </summary>
    internal enum BovadaPlayerActionType
    {
        Unknown = 0,
        Check = 64,
        Bet = 128,
        Call = 256,
        Raise = 512,
        Fold = 1024,
        AllIn = 2048,
        AllInRaise = 4096,
        FoldShow = 1048576,
        Ante = 16384
    }
}