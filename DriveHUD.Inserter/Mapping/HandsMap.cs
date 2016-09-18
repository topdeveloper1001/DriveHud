using DriveHUD.Inserter.Domain;
using FluentNHibernate.Mapping;

namespace DriveHUD.Inserter.Mapping {
    
    
    internal partial class HandsMap : ClassMap<Hands> 
    {
        
        public HandsMap() {
			Table("hands");
			LazyLoad();
            Id(x => x.PokerhandId).GeneratedBy.SequenceIdentity("pokerhands_pokerhand_id_seq").Column("pokerhand_id");
			Map(x => x.Gamenumber).Column("gamenumber").Not.Nullable();
			Map(x => x.PokersiteId).Column("pokersite_id").Not.Nullable();
        }
    }
}
