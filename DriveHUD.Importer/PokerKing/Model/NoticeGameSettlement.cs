//-----------------------------------------------------------------------
// <copyright file="NoticeGameSettlement.cs" company="Ace Poker Solutions">
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
    internal class NoticeGameSettlement
    {
        [ProtoMember(1)]
        public int RoomId { get; set; }

        [ProtoMember(2)]
        public PlayerSettleInfo[] Winners { get; set; }

        [ProtoMember(3)]
        public PotInfo[] Pots { get; set; }

        [ProtoMember(4)]
        public ulong Gameuuid { get; set; }

        [ProtoMember(5)]
        public JackPotWinInfo[] Jinfo { get; set; }

        [ProtoMember(6)]
        public string GameuuidJs { get; set; }
    }
}