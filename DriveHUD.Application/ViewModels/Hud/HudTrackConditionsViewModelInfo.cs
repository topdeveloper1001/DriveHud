//-----------------------------------------------------------------------
// <copyright file="HudTrackConditionsViewModelInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using ProtoBuf;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudTrackConditionsViewModelInfo
    {
        [ProtoMember(1)]
        public decimal AveragePot { get; set; }

        [ProtoMember(2)]
        public decimal VPIP { get; set; }

        [ProtoMember(3)]
        public decimal ThreeBet { get; set; }

        [ProtoMember(4)]
        public EnumTableType TableType { get; set; }

        [ProtoMember(5)]
        public int BuyInNL { get; set; }
    }
}