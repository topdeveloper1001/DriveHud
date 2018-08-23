//-----------------------------------------------------------------------
// <copyright file="BuyinPlayerInfo.cs" company="Ace Poker Solutions">
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
    internal class BuyinPlayerInfo
    {
        [ProtoMember(1)]
        public int PlayerId { get; set; }

        [ProtoMember(2)]
        public long Amount { get; set; }

        [ProtoMember(3)]
        public string Playername { get; set; }

        [ProtoMember(4)]
        public string Playerhead { get; set; }

        [ProtoMember(5)]
        public int RequestId { get; set; }

        [ProtoMember(6)]
        public uint Timestamp { get; set; }

        [ProtoMember(8)]
        public uint LastClubId { get; set; }

        [ProtoMember(9)]
        public AllianceInfo[] AllianceInfos { get; set; }
    }
}