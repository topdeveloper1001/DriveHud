//-----------------------------------------------------------------------
// <copyright file="GametypesMap.cs" company="Ace Poker Solutions">
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
using FluentNHibernate.Mapping;

namespace DriveHUD.Entities.Mapping
{
    public partial class GametypesMap : ClassMap<Gametypes>
    {
        public GametypesMap()
        {
            Table("GameInfo");
            LazyLoad();
            Id(x => x.GametypeId).GeneratedBy.Native().Column("GameInfoId");
            Map(x => x.PokergametypeId).Column("GameType").Not.Nullable();
            Map(x => x.Tablesize).Column("TableSize").Not.Nullable();
            Map(x => x.Smallblindincents).Column("SmallBlind").Not.Nullable();
            Map(x => x.CurrencytypeId).Column("Currency").Not.Nullable();
            Map(x => x.Bigblindincents).Column("BigBlind").Not.Nullable();
            Map(x => x.Istourney).Column("IsTournament").Not.Nullable();
            Map(x => x.Anteincents).Column("Ante").Not.Nullable();
        }
    }
}