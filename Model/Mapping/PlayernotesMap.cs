using FluentNHibernate.Mapping;
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Mapping
{
    public partial class PlayernotesMap : ClassMap<Playernotes>
    {
        public PlayernotesMap()
        {
            Table("\"PlayerNotes\"");
            LazyLoad();
            Id(x => x.PlayerNoteId).GeneratedBy.Sequence("\"PlayerNotes_PlayerNoteId_seq\"").Column("\"PlayerNoteId\"");
            Map(x => x.Note).Column("\"Note\"");
            Map(x => x.PokersiteId).Column("\"PokerSiteId\"").Not.Nullable();
            References(x => x.Player).Column("\"PlayerId\"").ForeignKey("\"PlayerId\"");
        }
    }
}
