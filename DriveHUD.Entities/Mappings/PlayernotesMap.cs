//-----------------------------------------------------------------------
// <copyright file="PlayernotesMap.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Entities.Mapping
{
    public partial class PlayernotesMap : ClassMap<Playernotes>
    {
        public PlayernotesMap()
        {
            Table("PlayerNotes");
            LazyLoad();
            Id(x => x.PlayerNoteId).GeneratedBy.Native().Column("PlayerNoteId");
            Map(x => x.Note).Column("Note");
            Map(x => x.PokersiteId).Column("PokerSiteId").Not.Nullable();
            Map(x => x.PlayerId).Column("PlayerId").Not.Nullable();
            References(x => x.Player).Column("PlayerId").ForeignKey("PlayerId");
        }
    }
}