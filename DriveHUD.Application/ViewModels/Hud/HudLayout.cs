//-----------------------------------------------------------------------
// <copyright file="HudLayout.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using Model.Interfaces;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DriveHUD.Application.ViewModels.Hud
{
    [ProtoContract]
    public class HudLayout : ICleanable
    {
        public HudLayout()
        {
            ListHUDPlayer = new List<PlayerHudContent>();
        }

        public void Cleanup()
        {
            ListHUDPlayer?.ForEach(x => x?.HudElement?.Cleanup());
            ListHUDPlayer.Clear();
        }

        [ProtoMember(1)]
        public EnumGameType GameType { get; set; }

        [ProtoMember(2)]
        public List<PlayerHudContent> ListHUDPlayer { get; set; }

        [XmlIgnore, ProtoMember(3)]
        public HudTrackConditionsViewModelInfo HudTrackConditionsMeter { get; set; }

        [XmlIgnore, ProtoMember(4)]
        public int WindowId { get; set; }

        [XmlIgnore, ProtoMember(5)]
        public EnumTableType TableType { get; set; }

        [XmlIgnore, ProtoMember(6)]
        public long GameNumber { get; set; }

        [XmlIgnore, ProtoMember(7)]
        public EnumPokerSites PokerSite { get; set; }

        [XmlIgnore, ProtoMember(8)]
        public string LayoutName { get; set; }

        [XmlIgnore, ProtoMember(9)]
        public IEnumerable<string> AvailableLayouts { get; set; }

        [XmlIgnore, ProtoMember(10)]
        public bool PreloadMode { get; set; }

        [XmlIgnore, ProtoMember(11)]
        public string PreloadText { get; set; }

        [XmlIgnore, ProtoMember(12)]
        public bool IsSpecialMode { get; set; }

        [XmlIgnore, ProtoMember(13)]
        public IEnumerable<HudElementViewModel> EmptySeatsViewModels { get; set; }

        public override string ToString()
        {
            try
            {
                var players = ListHUDPlayer != null ? string.Join(", ", ListHUDPlayer.Select(x => $"{x.Name}({x.SeatNumber})")) : string.Empty;
                return $"handle={WindowId}, title={WinApi.GetWindowText(new IntPtr(WindowId))}, hand={GameNumber}, site={PokerSite}, tableType={TableType}, gameType={GameType}, layout={LayoutName}, players=[{players}]";
            }
            catch
            {
                return base.ToString();
            }
        }
    }
}