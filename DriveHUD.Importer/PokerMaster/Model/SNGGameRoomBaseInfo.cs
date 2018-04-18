//-----------------------------------------------------------------------
// <copyright file="SNGGameRoomBaseInfo.cs" company="Ace Poker Solutions">
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
    internal class SNGGameRoomBaseInfo
    {
        [ProtoMember(1)]
        public long GameRoomId { get; set; }

        [ProtoMember(2)]
        public string RoomName { get; set; }

        [ProtoMember(3)]
        public UserBaseInfoNet CreateUserBaseInfo { get; set; }

        [ProtoMember(4)]
        public bool PrivateRoom { get; set; }

        [ProtoMember(5)]
        public long SmallBlinds { get; set; }

        [ProtoMember(6)]
        public long BigBlinds { get; set; }

        [ProtoMember(7)]
        public int CurrentLevel { get; set; }

        [ProtoMember(8)]
        public int MaxiLevel { get; set; }

        [ProtoMember(9)]
        public bool LevelChanged { get; set; }

        [ProtoMember(10)]
        public long[] WinLadder { get; set; }

        [ProtoMember(11)]
        public long OriginalStacks { get; set; }

        [ProtoMember(12)]
        public int BlindInterval { get; set; }

        [ProtoMember(13)]
        public CreateRoomType CreateRoomType { get; set; }

        [ProtoMember(14)]
        public long FromRoomId { get; set; }

        [ProtoMember(15)]
        public int GameRoomUserMaxNums { get; set; }

        [ProtoMember(16)]
        public string FromMsg { get; set; }

        [ProtoMember(17)]
        public bool Started { get; set; }

        [ProtoMember(18)]
        public long CreateTime { get; set; }

        [ProtoMember(19)]
        public long StartTime { get; set; }

        [ProtoMember(20)]
        public SNGRoomType SNGRoomtype { get; set; }

        [ProtoMember(21)]
        public int GameRoomUserNums { get; set; }

        [ProtoMember(22)]
        public bool InGame { get; set; }

        [ProtoMember(23)]
        public bool SignupInviting { get; set; }

        [ProtoMember(24)]
        public bool BuyinControl { get; set; }

        [ProtoMember(25)]
        public int Ante { get; set; }

        [ProtoMember(26)]
        public long PrepareTime { get; set; }

        [ProtoMember(27)]
        public int CancelWaitInterval { get; set; }

        [ProtoMember(28)]
        public bool IPLimit { get; set; }

        [ProtoMember(29)]
        public bool GPSLimit { get; set; }

        [ProtoMember(30)]
        public long LeagueID { get; set; }

        [ProtoMember(31)]
        public string LeagueName { get; set; }

        [ProtoMember(32)]
        public int ThinkingInterval { get; set; }

        [ProtoMember(33)]
        public bool HasBuyin { get; set; }
    }
}