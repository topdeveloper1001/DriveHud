//-----------------------------------------------------------------------
// <copyright file="GameRoomGameState.cs" company="Ace Poker Solutions">
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
    internal enum GameRoomGameState
    {
        ROOM_GAME_STATE_GameWait = 1,
        ROOM_GAME_STATE_GameStart = 2,
        ROOM_GAME_STATE_PreFlop = 3,
        ROOM_GAME_STATE_Flop = 4,
        ROOM_GAME_STATE_Turn = 5,
        ROOM_GAME_STATE_River = 6,
        ROOM_GAME_STATE_Result = 7,
        ROOM_GAME_STATE_SHOWCARD = 8,
        ROOM_GAME_STATE_Over = 9,
        ROOM_GAME_STATE_GamePrepare = 10,
        ROOM_GAME_STATE_Ante = 11,
        ROOM_GAME_STATE_Flop_One = 12,
        ROOM_GAME_STATE_Flop_Two = 13,
        ROOM_GAME_STATE_Flop_Three = 14,
    }
}