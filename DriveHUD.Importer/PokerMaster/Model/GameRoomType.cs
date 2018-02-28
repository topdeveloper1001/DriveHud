//-----------------------------------------------------------------------
// <copyright file="GameRoomType.cs" company="Ace Poker Solutions">
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
    internal enum GameRoomType
    {
        GAME_ROOM_NORMAL = 1,
        GAME_ROOM_SNG = 2,
        GAME_ROOM_NORMAL_INSURANCE = 3,
        GAME_ROOM_SIX = 4,
        GAME_ROOM_OMAHA = 5,
        GAME_ROOM_OMAHA_INSURANCE = 6,
        GAME_ROOM_MIXED = 7,
        GAME_ROOM_MTT = 8,
    }
}