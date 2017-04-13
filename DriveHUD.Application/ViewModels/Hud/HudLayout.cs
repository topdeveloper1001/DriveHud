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

using DriveHUD.Entities;
using Model.Interfaces;
using ProtoBuf;
using System.Collections.Generic;
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

        // to remove
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
    }
}