//-----------------------------------------------------------------------
// <copyright file="PlayerstatisticMap.cs" company="Ace Poker Solutions">
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
    public partial class PlayerstatisticMap : ClassMap<Playerstatistic>
    {
        public PlayerstatisticMap()
        {
            Table("PlayerStatistic");
            LazyLoad();
            Id(x => x.CompiledplayerresultsId).GeneratedBy.Native().Column("PlayerStatisticId");
            Map(x => x.PlayerId).Column("PlayerId").Not.Nullable();
            Map(x => x.Playedyearandmonth).Column("PlayedDate").Not.Nullable();
            Map(x => x.Numberofplayers).Column("TotalPlayers").Not.Nullable();
            Map(x => x.GametypeId).Column("GameInfoId").Not.Nullable();
            Map(x => x.Totalhands).Column("TotalHands").Not.Nullable();
            Map(x => x.Totalamountwonincents).Column("TotalWon").Not.Nullable();
            Map(x => x.Totalrakeincents).Column("TotalRake").Not.Nullable();
            Map(x => x.Totalbbswon).Column("TotalWonBB").Not.Nullable();
            Map(x => x.Bigblindstealfaced).Column("BigBlindStealAttempted").Not.Nullable();
            Map(x => x.Bigblindstealdefended).Column("BigBlindStealDefended").Not.Nullable();
            Map(x => x.Bigblindstealreraised).Column("BigBlindStealReraised").Not.Nullable();
            Map(x => x.Calledflopcontinuationbet).Column("CalledFlopContinuationBet").Not.Nullable();
            Map(x => x.Calledfourbetpreflop).Column("CalledFourBetPreflop").Not.Nullable();
            Map(x => x.Calledrivercontinuationbet).Column("CalledRiverContinuationBet").Not.Nullable();
            Map(x => x.Calledthreebetpreflop).Column("CalledThreeBetPreflop").Not.Nullable();
            Map(x => x.Calledturncontinuationbet).Column("CalledTurnContinuationBet").Not.Nullable();
            Map(x => x.Calledtwopreflopraisers).Column("CalledTwoPreflopRaisers").Not.Nullable();
            Map(x => x.Couldcoldcall).Column("CouldColdCall").Not.Nullable();
            Map(x => x.Couldsqueeze).Column("CouldSqueeze").Not.Nullable();
            Map(x => x.Couldthreebet).Column("CouldThreeBet").Not.Nullable();
            Map(x => x.Didcoldcall).Column("DidColdCall").Not.Nullable();
            Map(x => x.Didsqueeze).Column("DidSqueeze").Not.Nullable();
            Map(x => x.Didthreebet).Column("DidThreeBet").Not.Nullable();
            Map(x => x.Facedfourbetpreflop).Column("FacedFourBetPreflop").Not.Nullable();
            Map(x => x.Facedthreebetpreflop).Column("FacedThreeBetPreflop").Not.Nullable();
            Map(x => x.Facingflopcontinuationbet).Column("FacingFlopContinuationBet").Not.Nullable();
            Map(x => x.Facingrivercontinuationbet).Column("FacingRiverContinuationBet").Not.Nullable();
            Map(x => x.Facingturncontinuationbet).Column("FacingTurnContinuationBet").Not.Nullable();
            Map(x => x.Facingtwopreflopraisers).Column("FacingTwoPreFlopRaisers").Not.Nullable();
            Map(x => x.Flopcontinuationbetmade).Column("FlopContinuationBetMade").Not.Nullable();
            Map(x => x.Flopcontinuationbetpossible).Column("FlopContinuationBetPossible").Not.Nullable();
            Map(x => x.Foldedtoflopcontinuationbet).Column("FoldedToFlopContinuationBet").Not.Nullable();
            Map(x => x.Foldedtofourbetpreflop).Column("FoldedToFourBetPreflop").Not.Nullable();
            Map(x => x.Foldedtorivercontinuationbet).Column("FoldedToRiverContinuationBet").Not.Nullable();
            Map(x => x.Foldedtothreebetpreflop).Column("FoldedToThreeBetPreflop").Not.Nullable();
            Map(x => x.Foldedtoturncontinuationbet).Column("FoldedToTurnContinuationBet").Not.Nullable();
            Map(x => x.Pfrhands).Column("Pfr").Not.Nullable();
            Map(x => x.Raisedflopcontinuationbet).Column("RaisedFlopContinuationBet").Not.Nullable();
            Map(x => x.Raisedfourbetpreflop).Column("RaisedFourBetPreflop").Not.Nullable();
            Map(x => x.Raisedrivercontinuationbet).Column("RaisedRiverContinuationBet").Not.Nullable();
            Map(x => x.Raisedthreebetpreflop).Column("RaisedThreeBetPreflop").Not.Nullable();
            Map(x => x.Raisedturncontinuationbet).Column("RaisedTurnContinuationBet").Not.Nullable();
            Map(x => x.Raisedtwopreflopraisers).Column("RaisedTwoPreflopRaisers").Not.Nullable();
            Map(x => x.Rivercallippassonturncb).Column("RiverCallIpPassOnTurnCb").Not.Nullable();
            Map(x => x.Rivercontinuationbetmade).Column("RiverContinuationBetMade").Not.Nullable();
            Map(x => x.Rivercontinuationbetpossible).Column("RiverContinuationBetPossible").Not.Nullable();
            Map(x => x.Riverfoldippassonturncb).Column("RiverFoldIpPassOnTurnCb").Not.Nullable();
            Map(x => x.Riverraiseippassonturncb).Column("RiverRaiseIpPassOnTurnCb").Not.Nullable();
            Map(x => x.Sawflop).Column("SawFlop").Not.Nullable();
            Map(x => x.Sawlargeshowdown).Column("SawLargeShowdown").Not.Nullable();
            Map(x => x.Sawlargeshowdownlimpedflop).Column("SawLargeShowdownLimpedFlop").Not.Nullable();
            Map(x => x.Sawnonsmallshowdown).Column("SawNonSmallShowdown").Not.Nullable();
            Map(x => x.Sawnonsmallshowdownlimpedflop).Column("SawNonSmallShowdownLimpedFlop").Not.Nullable();
            Map(x => x.Sawshowdown).Column("SawShowdown").Not.Nullable();
            Map(x => x.Smallblindstealfaced).Column("SmallBlindStealAttempted").Not.Nullable();
            Map(x => x.Smallblindstealdefended).Column("SmallBlindStealDefended").Not.Nullable();
            Map(x => x.Smallblindstealreraised).Column("SmallBlindStealReraised").Not.Nullable();
            Map(x => x.Totalaggressivepostflopstreetsseen).Column("TotalAggressivePostflopStreetsSeen").Not.Nullable();
            Map(x => x.Totalbets).Column("TotalBets").Not.Nullable();
            Map(x => x.Totalcalls).Column("TotalCalls").Not.Nullable();
            Map(x => x.Totalpostflopstreetsplayed).Column("TotalPostflopStreetsSeen").Not.Nullable();
            Map(x => x.Turncallippassonflopcb).Column("TurnCallIpPassOnFlopCb").Not.Nullable();
            Map(x => x.Turncontinuationbetmade).Column("TurnContinuationBetMade").Not.Nullable();
            Map(x => x.Turncontinuationbetpossible).Column("TurnContinuationBetPossible").Not.Nullable();
            Map(x => x.Turnfoldippassonflopcb).Column("TurnFoldIpPassOnFlopCb").Not.Nullable();
            Map(x => x.Turnraiseippassonflopcb).Column("TurnRaiseIpPassOnFlopCb").Not.Nullable();
            Map(x => x.Vpiphands).Column("Vpip").Not.Nullable();
            Map(x => x.Wonhand).Column("WonHand").Not.Nullable();
            Map(x => x.Wonhandwhensawflop).Column("WonHandWhenSawFlop").Not.Nullable();
            Map(x => x.Wonhandwhensawriver).Column("WonHandWhenSawRiver").Not.Nullable();
            Map(x => x.Wonhandwhensawturn).Column("WonHandWhenSawTurn").Not.Nullable();
            Map(x => x.Wonlargeshowdown).Column("WonLargeShowdown").Not.Nullable();
            Map(x => x.Wonlargeshowdownlimpedflop).Column("WonLargeShowdownLimpedFlop").Not.Nullable();
            Map(x => x.Wonnonsmallshowdown).Column("WonNonSmallShowdown").Not.Nullable();
            Map(x => x.Wonnonsmallshowdownlimpedflop).Column("WonNonSmallShowdownLimpedFlop").Not.Nullable();
            Map(x => x.Wonshowdown).Column("WonShowdown").Not.Nullable();
        }
    }
}