//-----------------------------------------------------------------------
// <copyright file="InsurancePotInfo.cs" company="Ace Poker Solutions">
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
    internal class InsurancePotInfo
    {
        [ProtoMember(1)]
        public long PotID { get; set; }

        [ProtoMember(2)]
        public long PotStacks { get; set; }

        [ProtoMember(3)]
        public int[] Outs { get; set; }

        [ProtoMember(4)]
        public int Odds { get; set; }

        [ProtoMember(5)]
        public long TotalPotStacks { get; set; }

        [ProtoMember(6)]
        public long NoLosses { get; set; }

        [ProtoMember(7)]
        public long MinStacks { get; set; }

        [ProtoMember(8)]
        public long MaxStacks { get; set; }

        [ProtoMember(9)]
        public long BetStacks { get; set; }

        [ProtoMember(10)]
        public long GetStacks { get; set; }

        [ProtoMember(11)]
        public int PotIndex { get; set; }

        [ProtoMember(12)]
        public long SysStacks { get; set; }

        [ProtoMember(13)]
        public int[] FailOuts { get; set; }

        [ProtoMember(14)]
        public int[] FailBetOuts { get; set; }

        [ProtoMember(15)]
        public int[] EvenOuts { get; set; }

        [ProtoMember(16)]
        public int[] EvenBetOuts { get; set; }

        [ProtoMember(17)]
        public long SelectBetStack { get; set; }

        [ProtoMember(18)]
        public long ForceBetStack { get; set; }

        [ProtoMember(19)]
        public long PotBets { get; set; }

        [ProtoMember(20)]
        public long PreBetStacks { get; set; }

        [ProtoMember(21)]
        public long[] PotUuids { get; set; }

        [ProtoMember(22)]
        public long[] PotWinners { get; set; }
    }
}