using DriveHUD.Inserter.Domain;
using FluentNHibernate.Mapping;

namespace DriveHUD.Inserter.Mapping {
    
    
    internal partial class HandhistoryMap : ClassMap<Handhistory> 
    {
        
        public HandhistoryMap() {
            Table("handhistories");
			LazyLoad();


            Id(x => x.HandhistoryId).GeneratedBy.SequenceIdentity("handhistories_handhistory_id_seq").Column("handhistory_id");
			Map(x => x.Gamenumber).Column("gamenumber").Unique().Not.Nullable();
			Map(x => x.Tourneynumber).Column("tourneynumber");
			Map(x => x.HandhistoryVal).Column("handhistory").Not.Nullable();
			Map(x => x.GametypeId).Column("gametype_id");
			Map(x => x.PokersiteId).Column("pokersite_id").Not.Nullable();
			Map(x => x.Handtimestamp).Column("handtimestamp");
			Map(x => x.Filename).Column("filename");
        }
    }
}
