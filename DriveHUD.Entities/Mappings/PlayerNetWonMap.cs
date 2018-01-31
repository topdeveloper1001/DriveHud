//-----------------------------------------------------------------------
// <copyright file="PlayerNetWonMap.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using FluentNHibernate.Mapping;

namespace DriveHUD.Entities.Mappings
{
    public partial class PlayerNetWonMap : ClassMap<PlayerNetWon>
    {
        public PlayerNetWonMap()
        {
            Table("PlayerNetWon");
            LazyLoad();
            Id(x => x.Id).GeneratedBy.Native().Column("PlayerNetWonId");
            Map(x => x.PlayerId).Column("PlayerId").Not.Nullable();
            Map(x => x.Currency).Column("Currency").Not.Nullable();
            Map(x => x.NetWon).Column("NetWon").Not.Nullable();
            References(x => x.Player).ReadOnly().Column("PlayerId");
        }
    }
}