//-----------------------------------------------------------------------
// <copyright file="ActionType.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ProtoBuf;

namespace DriveHUD.Importers.PokerKing.Model
{
    [ProtoContract]
    internal enum ActionType : int
    {
        Null = 0,
        Check = 1,
        Fold = 2,
        Call = 3,
        Bet = 4,
        Raise = 5,
        Allin = 6,
        CallMuck = 7,
        AddActionTime = 8,
        SendCard_Common = 9,
        Send_HoleCards = 10,
        Straddle = 11,
        Post = 12
    }
}