//-----------------------------------------------------------------------
// <copyright file="GameRoomBaseInfo.cs" company="Ace Poker Solutions">
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
    internal class GameRoomBaseInfo
    {
        [ProtoMember(1)]
        public long GameRoomId { get; set; }

        [ProtoMember(2)]
        public string RoomName { get; set; }

        [ProtoMember(3)]
        public UserBaseInfoNet CreateUserBaseInfo { get; set; }

        [ProtoMember(4)]
        public int PrivateRoom { get; set; }

        [ProtoMember(5)]
        public long SmallBlinds { get; set; }

        [ProtoMember(6)]
        public long BigBlinds { get; set; }

        [ProtoMember(7)]
        public long BuyIn { get; set; }

        [ProtoMember(8)]
        public bool HasBuyIn { get; set; }

        [ProtoMember(9)]
        public long CreateTime { get; set; }

        [ProtoMember(10)]
        public long Duration { get; set; }

        [ProtoMember(11)]
        public long ChatRoomId { get; set; }

        [ProtoMember(12)]
        public bool Started { get; set; }

        [ProtoMember(13)]
        public int GameRoomUserNums { get; set; }

        [ProtoMember(14)]
        public string FromMsg { get; set; }

        [ProtoMember(15)]
        public CreateRoomType CreateRoomType { get; set; }

        [ProtoMember(16)]
        public GameRoomSeniorType GameRoomSeniorType { get; set; }

        [ProtoMember(17)]
        public int MaxBuyinRatio { get; set; }

        [ProtoMember(18)]
        public int MinBuyinRatio { get; set; }

        [ProtoMember(19)]
        public bool BuyinControl { get; set; }

        [ProtoMember(20)]
        public bool GamePause { get; set; }

        [ProtoMember(21)]
        public long GamePauseTime { get; set; }

        [ProtoMember(22)]
        public int GameRoomUserMaxNums { get; set; }

        [ProtoMember(23)]
        public int Ante { get; set; }

        public bool IsAnteRoom
        {
            get
            {
                return Ante > 0;
            }
        }

        [ProtoMember(24)]
        public bool Insurance { get; set; }

        [ProtoMember(25)]
        public bool InGame { get; set; }

        [ProtoMember(26)]
        public GameRoomType GameRoomType { get; set; }

        [ProtoMember(27)]
        public long StartTime { get; set; }

        [ProtoMember(28)]
        public bool IPLimit { get; set; }

        [ProtoMember(29)]
        public bool GPSLimit { get; set; }

        [ProtoMember(30)]
        public long LeagueID { get; set; }

        [ProtoMember(31)]
        public string LeagueName { get; set; }

        [ProtoMember(32)]
        public string FromClubName { get; set; }

        [ProtoMember(33)]
        public string FromClubUrl { get; set; }

        [ProtoMember(34)]
        public int ThinkingInterval { get; set; }

        [ProtoMember(35)]
        public bool Straddle { get; set; }

        [ProtoMember(36)]
        public string FromClubCreatorName { get; set; }

        [ProtoMember(37)]
        public int TableIndex { get; set; }

        [ProtoMember(38)]
        public long FromClubCreatorNameUuid { get; set; }
    }
}