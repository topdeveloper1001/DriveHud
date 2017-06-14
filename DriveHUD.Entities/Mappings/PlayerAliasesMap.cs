using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Entities.Mappings
{
    public class PlayerAliasesMap : ClassMap<PlayerAliases>
    {
        public PlayerAliasesMap()
        {
            Table("PlayerAliases");
            LazyLoad();
            Id(x => x.PlayerAliasesId).GeneratedBy.Native().Column("PlayerAliasId");
            Map(x => x.AliasId).Column("AliasId").Not.Nullable();
            Map(x => x.PlayersId).Column("PlayersId").Not.Nullable();
        }
    }
}
