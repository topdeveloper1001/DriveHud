//-----------------------------------------------------------------------
// <copyright file="AliasesMap.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Entities.Mappings
{
    public partial class AliasesMap : ClassMap<Aliases>
    {
        public AliasesMap()
        {
            Table("Aliases");
            LazyLoad();
            Id(x => x.AliasId).GeneratedBy.Native().Column("AliasId");
            Map(x => x.AliasName).Column("AliasName").Not.Nullable();
            HasManyToMany(x => x.Players)
                .Table("AliasPlayers")
                .ParentKeyColumn("AliasId")
                .ChildKeyColumn("PlayerId")
                .Cascade.None();
        }
    }
}