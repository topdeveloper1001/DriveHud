//-----------------------------------------------------------------------
// <copyright file="NoticePlayerAction.cs" company="Ace Poker Solutions">
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
    internal class NoticePlayerAction
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public int LastActionSeatId { get; set; }

        [ProtoMember(3)]
        public ActionType ActionType { get; set; }

        [ProtoMember(4)]
        public long Amount { get; set; }

        [ProtoMember(5)]
        public PotInfo[] Pots { get; set; }

        [ProtoMember(6)]
        public bool DefaultFold { get; set; }

        [ProtoMember(7)]
        public int ActionSeq { get; set; }
    }
}
