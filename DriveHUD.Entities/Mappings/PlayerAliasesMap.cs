using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Entities.Mappings
{
    public class PlayerAliasesMap : ClassMap<AliasPlayer>
    {
        public PlayerAliasesMap()
        {
            Table("AliasPlayers");
            LazyLoad();
            CompositeId().KeyProperty(x => x.AliasId, "AliasId").KeyProperty(x => x.PlayersId, "PlayerId");
        }
    }
}
