//-----------------------------------------------------------------------
// <copyright file="NoticePlayerActionTurn.cs" company="Ace Poker Solutions">
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
    internal class NoticePlayerActionTurn
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public int CurrActionSeatId { get; set; }

        [ProtoMember(3)]
        public int CurrActionUid { get; set; }

        [ProtoMember(4)]
        public int ActionTime { get; set; }

        [ProtoMember(5)]
        public int MinimumBet { get; set; }

        [ProtoMember(6)]
        public int LastActionSeatId { get; set; }

        [ProtoMember(7)]
        public int LastActionUid { get; set; }

        [ProtoMember(8)]
        public bool IsGreatestBet { get; set; }

        [ProtoMember(9)]
        public int ActionSeq { get; set; }

        [ProtoMember(10)]
        public PlayerInfo[] Players { get; set; }

        [ProtoMember(11)]
        public PotInfo[] Pots { get; set; }

        [ProtoMember(12)]
        public long LastBetAmount { get; set; }

        [ProtoMember(13)]
        public long CurrActionSeatRoundBet { get; set; }

        [ProtoMember(14)]
        public bool DefaultFold { get; set; }

        [ProtoMember(15)]
        public int CallAmount { get; set; }

        [ProtoMember(16)]
        public long MaxRoundBet { get; set; }

        [ProtoMember(17)]
        public long LastValidRaiseAmount { get; set; }
    }
}