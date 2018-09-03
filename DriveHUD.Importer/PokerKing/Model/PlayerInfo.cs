//-----------------------------------------------------------------------
// <copyright file="PlayerInfo.cs" company="Ace Poker Solutions">
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
    internal class PlayerInfo
    {
        [ProtoMember(1)]
        public uint Playerid { get; set; }

        [ProtoMember(2)]
        public int Seatid { get; set; }

        [ProtoMember(3)]
        public string Name { get; set; }

        [ProtoMember(4)]
        public string Headurl { get; set; }

        [ProtoMember(5)]
        public string Marks { get; set; }

        [ProtoMember(6)]
        public int Gender { get; set; }

        [ProtoMember(7)]
        public long Stake { get; set; }

        [ProtoMember(8)]
        public string LastVoice { get; set; }

        [ProtoMember(9)]
        public ActionType LastAction { get; set; }

        [ProtoMember(10)]
        public bool InGame { get; set; }

        [ProtoMember(12)]
        public bool InStay { get; set; }

        [ProtoMember(13)]
        public int LeftStayTime { get; set; }

        [ProtoMember(14)]
        public long RoundBet { get; set; }

        [ProtoMember(15)]
        public CardItem[] Cards { get; set; }

        [ProtoMember(16)]
        public PositionInfo Position { get; set; }
    }
}