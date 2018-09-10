//-----------------------------------------------------------------------
// <copyright file="NoticeBuyin.cs" company="Ace Poker Solutions">
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
    internal class NoticeBuyin
    {
        [ProtoMember(1)]
        public long SelfBuyinLimit { get; set; }

        [ProtoMember(2)]
        public long SelfBuyin { get; set; }

        [ProtoMember(3)]
        public long SelfStake { get; set; }

        [ProtoMember(4)]
        public long BankChips { get; set; }

        [ProtoMember(5)]
        public long SelfBuyout { get; set; }

        [ProtoMember(6)]
        public int RoomId { get; set; }
    }
}