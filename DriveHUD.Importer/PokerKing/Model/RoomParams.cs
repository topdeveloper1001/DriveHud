//-----------------------------------------------------------------------
// <copyright file="RoomParams.cs" company="Ace Poker Solutions">
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
    internal class RoomParams
    {
        [ProtoMember(1)]
        public int OwnerType { get; set; }

        [ProtoMember(2)]
        public int GameMode { get; set; }

        [ProtoMember(3)]
        public int PlayerCountMax { get; set; }

        [ProtoMember(4)]
        public int RuleBlindEnum { get; set; }

        [ProtoMember(5)]
        public int BuyinMinEnum { get; set; }

        [ProtoMember(6)]
        public int RuleBuyinFold { get; set; }

        [ProtoMember(7)]
        public int RuleTimeLimit { get; set; }

        [ProtoMember(8)]
        public int RuleSwitchBuyinControl { get; set; }

        [ProtoMember(9)]
        public int RuleSwitchInsurance { get; set; }

        [ProtoMember(10)]
        public int RuleSwitchAntiCheat { get; set; }

        [ProtoMember(11)]
        public int RuleSwitchForceStraddle { get; set; }

        [ProtoMember(12)]
        public int RuleSwitchRandomSeat { get; set; }

        [ProtoMember(13)]
        public int RuleAnteAmount { get; set; }

        [ProtoMember(14)]
        public string GameName { get; set; }

        [ProtoMember(15)]
        public uint ClubId { get; set; }

        [ProtoMember(16)]
        public bool IsAssociatedJackpot { get; set; }

        [ProtoMember(17)]
        public bool IsAllinAllFold { get; set; }

        [ProtoMember(18)]
        public uint[] AllianceIds { get; set; }

        [ProtoMember(19)]
        public string OwnerClubname { get; set; }

        [ProtoMember(20)]
        public uint CreatorId { get; set; }

        [ProtoMember(21)]
        public bool ShortGameDoubleAnte { get; set; }

        [ProtoMember(22)]
        public bool ShortFullHouseFlushStraightThree { get; set; }

        [ProtoMember(23)]
        public bool IsOpenedDrawback { get; set; }

        [ProtoMember(24)]
        public int DrawbackHoldTimes { get; set; }

        [ProtoMember(25)]
        public int DrawbackTimes { get; set; }

        [ProtoMember(26)]
        public bool ChooseOuts { get; set; }

        [ProtoMember(27)]
        public bool MuckSwitch { get; set; }

        [ProtoMember(28)]
        public bool AntiSimulator { get; set; }

        [ProtoMember(29)]
        public bool ForceShowcard { get; set; }

        [ProtoMember(30)]
        public bool UnlimitForceShowcard { get; set; }

        [ProtoMember(31)]
        public string ClubHead { get; set; }

        [ProtoMember(32)]
        public int AutoStartNum { get; set; }

        [ProtoMember(33)]
        public string JoinPassword { get; set; }

        [ProtoMember(34)]
        public string BuyinPassword { get; set; }

        [ProtoMember(35)]
        public bool IsCalcIncomePerHand { get; set; }

        [ProtoMember(36)]
        public int IsMirco { get; set; }
    }
}