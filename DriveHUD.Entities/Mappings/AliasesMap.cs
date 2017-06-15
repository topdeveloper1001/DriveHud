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
        }
    }
}
