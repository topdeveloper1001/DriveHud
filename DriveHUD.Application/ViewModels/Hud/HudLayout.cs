//-----------------------------------------------------------------------
// <copyright file="HudLayout.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Linq;
using DriveHUD.ViewModels;
using Model.Enums;
using Model.Interfaces;
using ProtoBuf;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels
{
    [ProtoContract]
    public class HudLayout : ICleanable
    {
        public HudLayout()
        {
            TableHud = new HudTableViewModel();
            HudStats = new List<StatInfo>();
            ListHUDPlayer = new List<PlayerHudContent>();
        }

        public HudLayout(HudViewModel viewModel)
        {
            TableHud = viewModel.CurrentHudTableViewModel;
            TableType = viewModel.CurrentTableType?.TableType ?? EnumTableType.Six;
            HudStats = viewModel.StatInfoObserveCollection.ToList();
            ListHUDPlayer = viewModel.PlayerCollection.ToList();
        }

        public void Cleanup()
        {
            TableHud?.HudElements?.ForEach(x => x?.Cleanup());
            TableHud?.HudElements?.Clear();
            ListHUDPlayer?.ForEach(x => x?.HudElement?.Cleanup());
            ListHUDPlayer.Clear();
            HudStats?.ForEach(x => x?.Reset());
            HudStats.Clear();
        }

        [ProtoMember(1)]
        public HudTableViewModel TableHud { get; set; }

        [ProtoMember(2)]
        public List<StatInfo> HudStats { get; set; }

        [ProtoMember(3)]
        public List<PlayerHudContent> ListHUDPlayer { get; set; }

        [XmlIgnore, ProtoMember(4)]
        public HudTrackConditionsViewModelInfo HudTrackConditionsMeter { get; set; }

        [XmlIgnore, ProtoMember(5)]
        public int WindowId { get; set; }

        [XmlIgnore, ProtoMember(6)]
        public HudType HudType { get; set; }

        [XmlIgnore, ProtoMember(7)]
        public EnumTableType TableType { get; set; }

        [XmlIgnore, ProtoMember(8)]
        public long GameNumber { get; set; }

        [XmlIgnore, ProtoMember(9)]
        public short PokerSiteId { get; set; }

        [XmlIgnore, ProtoMember(10)]
        public int LayoutId { get; set; }

        [XmlIgnore, ProtoMember(11)]
        public string LayoutName { get; set; }

        [XmlIgnore, ProtoMember(12)]
        public IEnumerable<string> AvailableLayouts { get; set; }
    }
}