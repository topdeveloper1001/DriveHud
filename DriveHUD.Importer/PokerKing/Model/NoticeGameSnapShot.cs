//-----------------------------------------------------------------------
// <copyright file="NoticeGameSnapShot.cs" company="Ace Poker Solutions">
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
    internal class NoticeGameSnapShot
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public uint RoomOwnerId { get; set; }

        [ProtoMember(3)] 
        public RoomParams Params { get; set; }

        [ProtoMember(4)] 
        public long SelfBuyinLimit { get; set; }

        [ProtoMember(5)]
        public long SelfBuyin { get; set; }

        [ProtoMember(6)]
        public long SelfStake { get; set; }

        [ProtoMember(7)]
        public RoomStates Rstate { get; set; }

        [ProtoMember(8)] 
        public BuyinPlayerInfo[] Buyins { get; set; }

        [ProtoMember(9)] 
        public TableStates Tstate { get; set; }

        [ProtoMember(10)]
        public int RoomCreateTime { get; set; }

        [ProtoMember(11)]
        public int RoomStartTime { get; set; }

        [ProtoMember(12)]
        public ulong[] GameUuids { get; set; }

        [ProtoMember(13)]
        public uint[] ProhibitSitdownList { get; set; }

        [ProtoMember(14)]
        public uint LastBuyinClubId { get; set; }

        [ProtoMember(15)]
        public uint LastBuyinOwnerId { get; set; }

        [ProtoMember(16)]
        public ClubInfo[] ClubInfos { get; set; }

        [ProtoMember(17)]
        public PlayerBuyinInfo[] BuyinInfos { get; set; }

        [ProtoMember(18)]
        public int AutoAddActiontimeCount { get; set; }

        [ProtoMember(19)]
        public int ActionTimeCount { get; set; }

        [ProtoMember(20)]
        public uint[] ClubCreaterIds { get; set; }

        [ProtoMember(21)]
        public long TotalBuyout { get; set; }

        [ProtoMember(22)]
        public uint LastBuyinAllianceId { get; set; }

        [ProtoMember(23)]
        public PayMoneyItems AllFeeItems { get; set; }

        [ProtoMember(24)]
        public bool IsQuickSit { get; set; }

        [ProtoMember(25)]
        public JsStringGameUUid[] GameUuidsJs { get; set; }

        [ProtoMember(26)]
        public uint GameId { get; set; }
    }
}