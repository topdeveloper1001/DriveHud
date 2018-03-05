//-----------------------------------------------------------------------
// <copyright file="SCGameRoomStateChange.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Importers.PokerMaster.Model
{
    [ProtoContract]
    internal class SCGameRoomStateChange
    {
        [ProtoMember(1)]
        public GameRoomInfo GameRoomInfo { get; set; }

        [ProtoMember(2)]
        public int[] GivenCards { get; set; }

        [ProtoMember(3)]
        public int[] GivenHands { get; set; }

        [ProtoMember(4)]
        public long Uuid { get; set; }

        public long GameNumber
        {
            get
            {
                return GameRoomInfo.GameNumber;
            }
        }

        public bool IsStart
        {
            get
            {
                return GameRoomInfo != null && GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_GameStart;
            }
        }

        public bool IsAction
        {
            get
            {
                return GameRoomInfo != null &&
                    (GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Ante ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Flop ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Flop_One ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Flop_Three ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Flop_Two ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_PreFlop ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_River ||
                    GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Turn);
            }
        }

        public bool IsShowdown
        {
            get
            {
                return GameRoomInfo != null && GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_SHOWCARD;
            }
        }

        public bool IsSummary
        {
            get
            {
                return GameRoomInfo != null && GameRoomInfo.GameState == GameRoomGameState.ROOM_GAME_STATE_Result;
            }
        }
    }
}