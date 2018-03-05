//-----------------------------------------------------------------------
// <copyright file="UserGameStates.cs" company="Ace Poker Solutions">
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
    internal enum UserGameStates
    {
        USER_GAME_STATE_STANDBY = 1,
        USER_GAME_STATE_WAIT = 2,
        USER_GAME_STATE_READY = 3,
        USER_GAME_STATE_BETTING = 4,
        USER_GAME_STATE_FOLD = 5,
        USER_GAME_STATE_CHECK = 7,
        USER_GAME_STATE_CALL = 8,
        USER_GAME_STATE_RAISE = 9,
        USER_GAME_STATE_ALLIN = 10,
        USER_GAME_STATE_BLIND = 11,
        USER_GAME_STATE_DEAD = 12,
    }
}