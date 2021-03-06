//-----------------------------------------------------------------------
// <copyright file="HandhistoryMap.cs" company="Ace Poker Solutions">
// Copyright � 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using FluentNHibernate.Mapping;

namespace DriveHUD.Entities.Mapping
{
    public partial class HandhistoryMap : ClassMap<Handhistory>
    {
        public HandhistoryMap()
        {
            Table("HandHistories");
            LazyLoad();
            Id(x => x.HandhistoryId).GeneratedBy.Native().Column("HandHistoryId");
            Map(x => x.Gamenumber).Column("HandNumber").Unique().Not.Nullable();
            Map(x => x.Tourneynumber).Column("TournamentNumber");
            Map(x => x.HandhistoryVal).Column("HandHistory").Not.Nullable();
            Map(x => x.GametypeId).Column("GameType");
            Map(x => x.PokersiteId).Column("PokerSiteId").Not.Nullable();
            Map(x => x.Handtimestamp).Column("HandHistoryTimestamp");
        }
    }
}