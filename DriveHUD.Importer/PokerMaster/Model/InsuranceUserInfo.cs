//-----------------------------------------------------------------------
// <copyright file="InsuranceUserInfo.cs" company="Ace Poker Solutions">
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
    internal class InsuranceUserInfo
    {
        [ProtoMember(1)]
        public long Uuid { get; set; }

        [ProtoMember(2)]
        public InsuranceBetStatus InsuranceBetStatus { get; set; }

        [ProtoMember(3)]
        public InsurancePotInfo[] InsurancePotInfos { get; set; }

        [ProtoMember(4)]
        public long TotalGetStacks { get; set; }

        [ProtoMember(5)]
        public bool CanInsurance { get; set; }

        [ProtoMember(6)]
        public bool DefaultInsurance { get; set; }

        [ProtoMember(7)]
        public long InsuranceStartTime { get; set; }

        [ProtoMember(8)]
        public long InsuranceActDuration { get; set; }

        [ProtoMember(9)]
        public int InsuranceDelayCost { get; set; }

        [ProtoMember(10)]
        public int InsuranceDelayLong { get; set; }

        [ProtoMember(11)]
        public bool FirstInsurance { get; set; }
    }
}