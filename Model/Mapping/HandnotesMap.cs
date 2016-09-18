using System;
using System.Collections.Generic;
using System.Text;
using FluentNHibernate.Mapping;
using DriveHUD.Entities;

namespace Model.Mapping
{


    public partial class HandnotesMap : ClassMap<Handnotes>
    {

        public HandnotesMap()
        {
            Table("\"HandNotes\"");
            LazyLoad();
            Id(x => x.HandNoteId).GeneratedBy.Sequence("\"HandNotes_HandNoteId_seq\"").Column("\"HandNoteId\"");
            Map(x => x.Gamenumber).Column("\"HandNumber\"").Not.Nullable();
            Map(x => x.Note).Column("\"Note\"");
            Map(x => x.PokersiteId).Column("\"PokerSiteId\"").Not.Nullable();
            Map(x => x.CategoryId).Column("\"HandTag\"");
        }
    }
}
