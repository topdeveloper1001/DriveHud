//-----------------------------------------------------------------------
// <copyright file="GameRoomInfo.cs" company="Ace Poker Solutions">
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
    internal class GameRoomInfo
    {
        [ProtoMember(1)]
        public GameRoomType GameRoomType { get; set; }

        [ProtoMember(2)]
        public GameRoomBaseInfo GameRoomBaseInfo { get; set; }

        [ProtoMember(3)]
        public SNGGameRoomBaseInfo SNGGameRoomBaseInfo { get; set; }

        [ProtoMember(4)]
        public GameRoomGameState GameState { get; set; }

        [ProtoMember(5)]
        public int[] CurrentCards { get; set; }

        [ProtoMember(6)]
        public long[] CurrentPots { get; set; }

        [ProtoMember(7)]
        public UserGameInfoNet[] UserGameInfos { get; set; }

        [ProtoMember(8)]
        public GameResultInfo GameResultInfo { get; set; }

        [ProtoMember(9)]
        public bool StateChange { get; set; }

        [ProtoMember(10)]
        public bool AllAllin { get; set; }

        [ProtoMember(11)]
        public long GameHandID { get; set; }

        public long GameNumber
        {
            get
            {
                var gameRoomId = GameRoomBaseInfo != null ?
                    GameRoomBaseInfo.GameRoomId :
                    (SNGGameRoomBaseInfo != null ? SNGGameRoomBaseInfo.GameRoomId : 0);

                return gameRoomId * 10000L + GameHandID + (GameState == GameRoomGameState.ROOM_GAME_STATE_GameStart ? 1L : 0L);
            }
        }

        [ProtoMember(12)]
        public long EffectiveRaise { get; set; }

        [ProtoMember(13)]
        public InsuranceStatus InsuranceStatus { get; set; }

        [ProtoMember(14)]
        public InsuranceInfo InsuranceInfos { get; set; }

        [ProtoMember(15)]
        public PotInfo[] PotInfos { get; set; }

        [ProtoMember(16)]
        public long[] KickUserIDs { get; set; }

        [ProtoMember(17)]
        public int DealTableIndex { get; set; }

        [ProtoMember(18)]
        public long[] BeginStacks { get; set; }

        public bool IsTournament
        {
            get
            {
                return SNGGameRoomBaseInfo != null;
            }
        }
    }
}