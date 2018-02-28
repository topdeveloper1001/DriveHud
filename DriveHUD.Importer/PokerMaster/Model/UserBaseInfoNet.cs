//-----------------------------------------------------------------------
// <copyright file="UserBaseInfoNet.cs" company="Ace Poker Solutions">
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
    internal class UserBaseInfoNet
    {
        [ProtoMember(1)]
        public long Uuid { get; set; }

        [ProtoMember(2)]
        public IdType Type { get; set; }

        [ProtoMember(3)]
        public string ID { get; set; }

        [ProtoMember(4)]
        public string Cover { get; set; }

        [ProtoMember(5)]
        public string Nick { get; set; }

        [ProtoMember(6)]
        public string PhoneNumber { get; set; }

        [ProtoMember(7)]
        public VipType VipType { get; set; }

        [ProtoMember(8)]
        public int Gender { get; set; }

        [ProtoMember(9)]
        public long LoginTime { get; set; }

        [ProtoMember(10)]
        public string Desc { get; set; }

        [ProtoMember(11)]
        public long Coin { get; set; }

        [ProtoMember(12)]
        public long Popularity { get; set; }

        [ProtoMember(13)]
        public int Level { get; set; }

        [ProtoMember(14)]
        public int Experience { get; set; }

        [ProtoMember(15)]
        public long VIPLimitTime { get; set; }

        [ProtoMember(16)]
        public int Mute { get; set; }

        [ProtoMember(17)]
        public int Vibrate { get; set; }

        [ProtoMember(18)]
        public string SmallCover { get; set; }

        [ProtoMember(19)]
        public string[] SmallAlbums { get; set; }

        [ProtoMember(20)]
        public string[] BigAlbums { get; set; }

        [ProtoMember(21)]
        public long AndroidCoin { get; set; }

        [ProtoMember(22)]
        public long AndroidPopularity { get; set; }

        [ProtoMember(23)]
        public DeviceType DeviceType { get; set; }

        [ProtoMember(24)]
        public LanguageType LanguageType { get; set; }

        [ProtoMember(25)]
        public string CountryCode { get; set; }

        [ProtoMember(26)]
        public string Remark { get; set; }

        [ProtoMember(27)]
        public int FriendInviteMute { get; set; }

        [ProtoMember(28)]
        public string ShowID { get; set; }
    }
}