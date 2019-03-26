//-----------------------------------------------------------------------
// <copyright file="NoticeQuickLeave.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
    internal class NoticeQuickLeave
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public string RoomName { get; set; }

        [ProtoMember(3)]
        public int PlayerId { get; set; }

        [ProtoMember(4)]
        public string Name { get; set; }

        [ProtoMember(5)]
        public long CurrStake { get; set; }

        [ProtoMember(6)]
        public long SettleStake { get; set; }

        [ProtoMember(7)]
        public long InGameTime { get; set; }

        [ProtoMember(8)]
        public long HandCount { get; set; }
    }
}
