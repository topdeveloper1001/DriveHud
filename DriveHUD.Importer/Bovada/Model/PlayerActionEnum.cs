//-----------------------------------------------------------------------
// <copyright file="PlayerActionEnum.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Player actions
    /// </summary>
    [Serializable]
    public enum PlayerActionEnum
    {
        None = 0,
        Fold = 1,
        DontShow = 2,
        PostSB = 3,
        PostBB = 4,
        Post = 5,
        RaiseTo = 6,
        Call = 7,
        Check = 8,
        Bet = 9,
        Allin = 10,
        AllinRaise = 11,
        PostDead = 12,
        ShowCards = 13,
        MuckCards = 14,
        WaitBB = 15,
        FoldShow = 16,
        Ante = 17
    }
}