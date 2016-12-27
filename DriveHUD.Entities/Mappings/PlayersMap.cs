//-----------------------------------------------------------------------
// <copyright file="PlayersMap.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using FluentNHibernate.Mapping;
using DriveHUD.Entities;

namespace DriveHUD.Entities.Mapping
{
    public partial class PlayersMap : ClassMap<Players>
    {
        public PlayersMap()
        {
            Table("Players");
            LazyLoad();
            Id(x => x.PlayerId).GeneratedBy.Native().Column("PlayerId");
            Map(x => x.Playername).Column("PlayerName").Not.Nullable();
            Map(x => x.PokersiteId).Column("PokerSiteId").Not.Nullable();
            Map(x => x.Tourneyhands).Column("TournamentHandsPlayed").Not.Nullable();
            Map(x => x.Cashhands).Column("CashHandsPlayed").Not.Nullable();
        }
    }
}