//-----------------------------------------------------------------------
// <copyright file="PayMoneyItems.cs" company="Ace Poker Solutions">
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
    internal class PayMoneyItems
    {
        [ProtoMember(1)]
        public uint PlayWay { get; set; }

        [ProtoMember(2)]
        public int ActionCount { get; set; }

        [ProtoMember(3)]
        public int ShowCardCount { get; set; }

        [ProtoMember(4)]
        public int InsuranceCount { get; set; }

        [ProtoMember(5)]
        public FeeItem[] ActionDelayCountsFee { get; set; }

        [ProtoMember(6)]
        public FeeItem[] ShowCardCountsFee { get; set; }

        [ProtoMember(7)]
        public FeeItem[] InsurnanceCountsFee { get; set; }

        [ProtoMember(8)]
        public FeeItem[] ShowLeftCardFee { get; set; }

        [ProtoMember(9)]
        public FeeItem EmotionFee { get; set; }

        [ProtoMember(10)]
        public FeeItem EmotionFee2 { get; set; }
    }
}