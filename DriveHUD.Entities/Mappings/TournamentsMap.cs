//-----------------------------------------------------------------------
// <copyright file="TournamentsMap.cs" company="Ace Poker Solutions">
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
    public partial class TournamentsMap : ClassMap<Tournaments>
    {
        public TournamentsMap()
        {
            Table("Tournaments");
            LazyLoad();
            Id(x => x.TourneydataId).GeneratedBy.Native().Column("TournamentId");
            Map(x => x.PokergametypeId).Column("GameType").Not.Nullable();
            Map(x => x.Finishposition).Column("PlayerEndPosition").Not.Nullable();
            Map(x => x.Bountyincents).Column("Bounty");
            Map(x => x.Buyinincents).Column("BuyIn").Not.Nullable();
            Map(x => x.CurrencyId).Column("Currency").Not.Nullable();
            Map(x => x.ImporttypeId).Column("ImportType").Not.Nullable();
            Map(x => x.Tablesize).Column("TableSize").Not.Nullable();
            Map(x => x.Tourneynumber).Column("TournamentNumber").Not.Nullable();
            Map(x => x.Tourneytables).Column("TablesQty").Not.Nullable();
            Map(x => x.Firsthandtimestamp).Column("FirstHandTimestamp");
            Map(x => x.Winningsincents).Column("Winnings").Not.Nullable();
            Map(x => x.Lasthandtimestamp).Column("LastHandTimestamp");
            Map(x => x.SpeedtypeId).Column("SpeedType").Not.Nullable();
            Map(x => x.Filelastmodifiedtime).Column("FileLastModifiedTime").Not.Nullable();
            Map(x => x.Rebuyamountincents).Column("Rebuy").Not.Nullable();
            Map(x => x.Rakeincents).Column("Rake").Not.Nullable();
            Map(x => x.Tourneyendedforplayer).Column("PlayerFinished").Not.Nullable();
            Map(x => x.Tourneysize).Column("TournamentSize").Not.Nullable();
            Map(x => x.Tourneytagscsv).Column("Tag").Not.Nullable();
            Map(x => x.SiteId).Column("PokerSiteId").Not.Nullable();
            Map(x => x.Filename).Column("Filename").Not.Nullable();
            Map(x => x.Startingstacksizeinchips).Column("StartingStacks").Not.Nullable();
            References(x => x.Player).Column("PlayerId");
        }
    }
}