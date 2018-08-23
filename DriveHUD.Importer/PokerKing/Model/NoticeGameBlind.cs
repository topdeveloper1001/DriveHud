//-----------------------------------------------------------------------
// <copyright file="NoticeGameBlind.cs" company="Ace Poker Solutions">
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
    internal class NoticeGameBlind
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public long SBAmount { get; set; }

        [ProtoMember(3)]
        public long BBAmount { get; set; }

        [ProtoMember(5)]
        public int[] StraddleSeatList { get; set; }

        [ProtoMember(6)]
        public int[] StraddleAmountList { get; set; }

        [ProtoMember(7)]
        public int[] PostSeatList { get; set; }

        [ProtoMember(8)]
        public int SBSeatId { get; set; }

        [ProtoMember(9)]
        public int BBSeatId { get; set; }

        [ProtoMember(10)]
        public int DealerSeatId { get; set; }
    }
}