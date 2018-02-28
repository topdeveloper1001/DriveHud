//-----------------------------------------------------------------------
// <copyright file="DefaultSettingInfoNet.cs" company="Ace Poker Solutions">
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
    internal class DefaultSettingInfoNet
    {
        [ProtoMember(1)]
        public int MaxInGameUserInfo { get; set; }

        [ProtoMember(2)]
        public int SimultaneousPlays { get; set; }

        [ProtoMember(3)]
        public int KickOff { get; set; }

        [ProtoMember(4)]
        public int FavoriteNum { get; set; }

        [ProtoMember(5)]
        public int[] SBs { get; set; }

        [ProtoMember(6)]
        public long[] PlayDuration { get; set; }

        [ProtoMember(7)]
        public int MaxLevel { get; set; }

        [ProtoMember(8)]
        public int MaxClubNums { get; set; }

        [ProtoMember(9)]
        public int SeniorGamrRoomCost { get; set; }

        [ProtoMember(10)]
        public SngBlindsStructure[] SNGBlindsStructures { get; set; }

        [ProtoMember(11)]
        public OddsStructure[] OddsStructure { get; set; }

        [ProtoMember(12)]
        public int InsuranceBool { get; set; }

        [ProtoMember(13)]
        public ShowCardItemFee[] ShowCardItemFees { get; set; }

        [ProtoMember(14)]
        public LeagueGameFeeLadders[] LeagueGameFeeLadders { get; set; }

        [ProtoMember(15)]
        public GameServiceRate[] GameServiceRates { get; set; }

        [ProtoMember(16)]
        public int EnableFourColorDeck { get; set; }

        [ProtoMember(17)]
        public int EnableGamingMute { get; set; }

        [ProtoMember(18)]
        public int EnableChattingMute { get; set; }

        [ProtoMember(19)]
        public int EnableRaiseAssistant { get; set; }

        [ProtoMember(20)]
        public RaiseShortcutsOptions[] RaiseShortcutsOptions { get; set; }

        [ProtoMember(21)]
        public RaiseShortcutsOptions[] RaiseShortcutsOptionsPotLimit { get; set; }

        [ProtoMember(22)]
        public int TableThemeOrdinal { get; set; }
    }
}