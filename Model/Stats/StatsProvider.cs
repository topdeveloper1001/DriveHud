//-----------------------------------------------------------------------
// <copyright file="StatsProvider.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Model.Data;
using Model.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.Stats
{
    /// <summary>
    /// Defines methods to create stats collections
    /// </summary>
    public class StatsProvider
    {
        /// <summary>
        /// The array of not supported stats in Heat Map
        /// </summary>
        private static Stat[] HeapMapNotSupportedStats = new[]
        {
            Stat.LineBreak,
            Stat.PlayerInfoIcon,
            Stat.TotalHands,
            Stat.NetWon,
            Stat.BBs,
            Stat.BB100,
            Stat.AF,
            Stat.TurnAF,
            Stat.RecentAgg,
            Stat.MRatio,
            Stat.RiverCallEffeciency
        };

        /// <summary>
        /// The dictionary of all stats with the property of <see cref="HudLightIndicators"/> and <see cref="Func{bool, Playerstatistic}"/> to calculate value
        /// </summary>
        public static ReadOnlyDictionary<Stat, StatBase> StatsBases = new ReadOnlyDictionary<Stat, StatBase>((new[]
        {
            new StatBase { Stat = Stat.AF, PropertyName = nameof(Indicators.Agg) },
            new StatBase { Stat = Stat.AGG, PropertyName = nameof(Indicators.AggPr), CreateStatDto = x => new StatDto(x.Totalbets, x.Totalpostflopstreetsplayed) },
            new StatBase { Stat = Stat.BBs, PropertyName = nameof(Indicators.StackInBBs) },
            new StatBase { Stat = Stat.BB100, PropertyName = nameof(Indicators.BB) },
            new StatBase { Stat = Stat.BetFlopCalled3BetPreflopIp, PropertyName = nameof(Indicators.BetFlopCalled3BetPreflopIp), CreateStatDto = x => new StatDto(x.BetFlopCalled3BetPreflopIp, x.CouldBetFlopCalled3BetPreflopIp) },
            new StatBase { Stat = Stat.BetFoldFlopPfrRaiser, PropertyName = nameof(Indicators.BetFoldFlopPfrRaiser), CreateStatDto = x => new StatDto(x.BetFoldFlopPfrRaiser, x.CouldBetFoldFlopPfrRaiser) },
            new StatBase { Stat = Stat.BetRiverOnBXLine, PropertyName = nameof(Indicators.BetRiverOnBXLine), CreateStatDto = x => new StatDto(x.DidBetRiverOnBXLine, x.CouldBetRiverOnBXLine) },
            new StatBase { Stat = Stat.BetWhenCheckedTo, PropertyName = nameof(Indicators.BetWhenCheckedTo), CreateStatDto = x => new StatDto(x.DidBetWhenCheckedToFlop + x.DidBetWhenCheckedToTurn + x.DidBetWhenCheckedToRiver, x.CanBetWhenCheckedToFlop + x.CanBetWhenCheckedToTurn + x.CanBetWhenCheckedToRiver) },
            new StatBase { Stat = Stat.BetFlopWhenCheckedToSRP, PropertyName = nameof(Indicators.BetFlopWhenCheckedToSRP), CreateStatDto = x => new StatDto(x.BetFlopWhenCheckedToSRP, x.CouldBetFlopWhenCheckedToSRP) },
            new StatBase { Stat = Stat.BetTurnWhenCheckedToSRP, PropertyName = nameof(Indicators.BetTurnWhenCheckedToSRP), CreateStatDto = x => new StatDto(x.BetTurnWhenCheckedToSRP, x.CouldBetTurnWhenCheckedToSRP) },
            new StatBase { Stat = Stat.BetRiverWhenCheckedToSRP, PropertyName = nameof(Indicators.BetRiverWhenCheckedToSRP), CreateStatDto = x => new StatDto(x.BetRiverWhenCheckedToSRP, x.CouldBetRiverWhenCheckedToSRP) },
            new StatBase { Stat = Stat.BTNDefendCORaise, PropertyName = nameof(Indicators.BTNDefendCORaise), CreateStatDto = x => new StatDto(x.Buttonstealdefended, x.Buttonstealfaced) },
            new StatBase { Stat = Stat.CalledCheckRaiseVsFlopCBet, PropertyName = nameof(Indicators.CalledCheckRaiseVsFlopCBet), CreateStatDto = x => new StatDto(x.CalledCheckRaiseVsFlopCBet, x.FacedCheckRaiseVsFlopCBet) },
            new StatBase { Stat = Stat.CallFlopCBetIP, PropertyName = nameof(Indicators.CallFlopCBetIP), CreateStatDto = x => new StatDto(x.CalledflopcontinuationbetIP, x.FacingflopcontinuationbetIP) },
            new StatBase { Stat = Stat.CallFlopCBetOOP, PropertyName = nameof(Indicators.CallFlopCBetOOP), CreateStatDto = x => new StatDto(x.CalledflopcontinuationbetOOP, x.FacingflopcontinuationbetOOP) },
            new StatBase { Stat = Stat.CalledTurnCheckRaise, PropertyName = nameof(Indicators.CalledTurnCheckRaise), CreateStatDto = x => new StatDto(x.CalledTurnCheckRaise, x.FacedTurnCheckRaise) },
            new StatBase { Stat = Stat.CallRiverRaise, PropertyName = nameof(Indicators.CallRiverRaise), CreateStatDto = x => new StatDto(x.CalledFacedRaiseRiver, x.FacedRaiseRiver) },
            new StatBase { Stat = Stat.CBet, PropertyName = nameof(Indicators.FlopCBet), CreateStatDto = x => new StatDto(x.Flopcontinuationbetmade, x.Flopcontinuationbetpossible) },
            new StatBase { Stat = Stat.CBetInFourBetPot, PropertyName = nameof(Indicators.FlopCBetInFourBetPot), CreateStatDto = x => new StatDto(x.FlopContinuationBetInFourBetPotMade, x.FlopContinuationBetInFourBetPotPossible) },
            new StatBase { Stat = Stat.CBetInThreeBetPot, PropertyName = nameof(Indicators.FlopCBetInThreeBetPot), CreateStatDto = x => new StatDto(x.FlopContinuationBetInThreeBetPotMade, x.FlopContinuationBetInThreeBetPotPossible)  },
            new StatBase { Stat = Stat.CBetIP, PropertyName = nameof(Indicators.CBetIP), CreateStatDto = x => new StatDto(x.Flopcontinuationipbetmade, x.Flopcontinuationipbetpossible) },
            new StatBase { Stat = Stat.CBetOOP, PropertyName = nameof(Indicators.CBetOOP), CreateStatDto = x => new StatDto(x.Flopcontinuationoopbetmade, x.Flopcontinuationoopbetpossible) },
            new StatBase { Stat = Stat.CBetThenFoldFlopSRP, PropertyName = nameof(Indicators.CBetThenFoldFlopSRP), CreateStatDto = x => new StatDto(x.CBetThenFoldFlopSRP, x.CouldCBetThenFoldFlopSRP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndXCOnTurnOOP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndXCOnTurnOOP), CreateStatDto = x => new StatDto(x.CheckedCalledTurnWhenCheckedFlopAsPfr, x.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndXFOnTurnOOP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndXFOnTurnOOP), CreateStatDto = x => new StatDto(x.CheckedFoldedToTurnWhenCheckedFlopAsPfr, x.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndCallOnTurn, PropertyName = nameof(Indicators.CheckFlopAsPFRAndCallOnTurn), CreateStatDto = x => new StatDto(x.CalledTurnBetWhenCheckedFlopAsPfr, x.FacedTurnBetWhenCheckedFlopAsPfr) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldOnTurn, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldOnTurn), CreateStatDto = x => new StatDto(x.FoldedToTurnBetWhenCheckedFlopAsPfr, x.FacedTurnBetWhenCheckedFlopAsPfr) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndRaiseOnTurn, PropertyName = nameof(Indicators.CheckFlopAsPFRAndRaiseOnTurn), CreateStatDto = x => new StatDto(x.RaisedTurnBetWhenCheckedFlopAsPfr, x.FacedTurnBetWhenCheckedFlopAsPfr) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetIPSRP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetIPSRP), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToTurnBetIPSRP, x.CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetOOPSRP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetOOPSRP), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToTurnBetOOPSRP, x.CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetIPSRP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetIPSRP), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToRiverBetIPSRP, x.CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetOOPSRP, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetOOPSRP), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToRiverBetOOPSRP, x.CouldCheckFlopAsPFRAndFoldToRiverBetOOPSRP) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot, x.CouldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot, x.CouldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot, x.CouldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot) },
            new StatBase { Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot, PropertyName = nameof(Indicators.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot), CreateStatDto = x => new StatDto(x.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot, x.CouldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot) },
            new StatBase { Stat = Stat.CheckFoldFlop3BetOop, PropertyName = nameof(Indicators.CheckFoldFlop3BetOop), CreateStatDto = x => new StatDto(x.CheckFoldFlop3BetOop, x.DidThreeBetOop) },
            new StatBase { Stat = Stat.CheckFoldFlopPfrOop, PropertyName = nameof(Indicators.CheckFoldFlopPfrOop), CreateStatDto = x => new StatDto(x.CheckFoldFlopPfrOop, x.PfrOop) },
            new StatBase { Stat = Stat.CheckRaise, PropertyName = nameof(Indicators.CheckRaise), CreateStatDto = x => new StatDto(x.DidCheckRaise, x.Totalhands) },
            new StatBase { Stat = Stat.CheckRaiseFlopAsPFR, PropertyName = nameof(Indicators.CheckRaiseFlopAsPFR), CreateStatDto = x => new StatDto(x.CheckRaiseFlopAsPFR, x.CouldCheckRaiseFlopAsPFR) },
            new StatBase { Stat = Stat.CheckRaisedFlopCBet, PropertyName = nameof(Indicators.CheckRaisedFlopCBet), CreateStatDto = x => new StatDto(x.CheckRaisedFlopCBet, x.CouldCheckRaiseFlopCBet) },
            new StatBase { Stat = Stat.CheckRiverOnBXLine, PropertyName = nameof(Indicators.CheckRiverOnBXLine), CreateStatDto = x => new StatDto(x.DidCheckRiverOnBXLine, x.CouldCheckRiverOnBXLine)  },
            new StatBase { Stat = Stat.CheckRiverAfterBBLine, PropertyName = nameof(Indicators.CheckRiverAfterBBLine), CreateStatDto = x => new StatDto(x.CheckedRiverAfterBBLine, x.CouldCheckRiverAfterBBLine)  },
            new StatBase { Stat = Stat.ColdCall_BB, PropertyName = nameof(HudLightIndicators.ColdCall_BB), CreateStatDto = x => new StatDto(x.DidColdCallBB, x.CouldColdCallBB) },
            new StatBase { Stat = Stat.ColdCall_BN, PropertyName = nameof(HudLightIndicators.ColdCall_BN), CreateStatDto = x => new StatDto(x.DidColdCallBN, x.CouldColdCallBN) },
            new StatBase { Stat = Stat.ColdCall_CO, PropertyName = nameof(HudLightIndicators.ColdCall_CO), CreateStatDto = x => new StatDto(x.DidColdCallCO, x.CouldColdCallCO) },
            new StatBase { Stat = Stat.ColdCall_EP, PropertyName = nameof(HudLightIndicators.ColdCall_EP), CreateStatDto = x => new StatDto(x.DidColdCallEP, x.CouldColdCallEP) },
            new StatBase { Stat = Stat.ColdCall_MP, PropertyName = nameof(HudLightIndicators.ColdCall_MP), CreateStatDto = x => new StatDto(x.DidColdCallMP, x.CouldColdCallMP) },
            new StatBase { Stat = Stat.ColdCall_SB, PropertyName = nameof(HudLightIndicators.ColdCall_SB), CreateStatDto = x => new StatDto(x.DidColdCallSB, x.CouldColdCallSB) },
            new StatBase { Stat = Stat.ColdCall, PropertyName = nameof(Indicators.ColdCall), CreateStatDto = x => new StatDto(x.Didcoldcall, x.Couldcoldcall)},
            new StatBase { Stat = Stat.ColdCallFourBet, PropertyName = nameof(Indicators.ColdCallFourBet), CreateStatDto = x => new StatDto(x.DidColdCallFourBet, x.CouldColdCallFourBet) },
            new StatBase { Stat = Stat.ColdCall4BetInBB, PropertyName = nameof(Indicators.ColdCall4BetInBB), CreateStatDto = x => new StatDto(x.DidColdCall4BetInBB, x.CouldColdCall4BetInBB) },
            new StatBase { Stat = Stat.ColdCall4BetInSB, PropertyName = nameof(Indicators.ColdCall4BetInSB), CreateStatDto = x => new StatDto(x.DidColdCall4BetInSB, x.CouldColdCall4BetInSB) },
            new StatBase { Stat = Stat.ColdCall4BetInMP, PropertyName = nameof(Indicators.ColdCall4BetInMP), CreateStatDto = x => new StatDto(x.DidColdCall4BetInMP, x.CouldColdCall4BetInMP) },
            new StatBase { Stat = Stat.ColdCall4BetInCO, PropertyName = nameof(Indicators.ColdCall4BetInCO), CreateStatDto = x => new StatDto(x.DidColdCall4BetInCO, x.CouldColdCall4BetInCO) },
            new StatBase { Stat = Stat.ColdCall4BetInBTN, PropertyName = nameof(Indicators.ColdCall4BetInBTN), CreateStatDto = x => new StatDto(x.DidColdCall4BetInBTN, x.CouldColdCall4BetInBTN) },
            new StatBase { Stat = Stat.ColdCallThreeBet, PropertyName = nameof(Indicators.ColdCallThreeBet), CreateStatDto = x => new StatDto(x.DidColdCallThreeBet, x.CouldColdCallThreeBet) },
            new StatBase { Stat = Stat.ColdCall3BetInBB, PropertyName = nameof(Indicators.ColdCall3BetInBB), CreateStatDto = x => new StatDto(x.DidColdCall3BetInBB, x.CouldColdCall3BetInBB) },
            new StatBase { Stat = Stat.ColdCall3BetInSB, PropertyName = nameof(Indicators.ColdCall3BetInSB), CreateStatDto = x => new StatDto(x.DidColdCall3BetInSB, x.CouldColdCall3BetInSB) },
            new StatBase { Stat = Stat.ColdCall3BetInMP, PropertyName = nameof(Indicators.ColdCall3BetInMP), CreateStatDto = x => new StatDto(x.DidColdCall3BetInMP, x.CouldColdCall3BetInMP) },
            new StatBase { Stat = Stat.ColdCall3BetInCO, PropertyName = nameof(Indicators.ColdCall3BetInCO), CreateStatDto = x => new StatDto(x.DidColdCall3BetInCO, x.CouldColdCall3BetInCO) },
            new StatBase { Stat = Stat.ColdCall3BetInBTN, PropertyName = nameof(Indicators.ColdCall3BetInBTN), CreateStatDto = x => new StatDto(x.DidColdCall3BetInBTN, x.CouldColdCall3BetInBTN) },
            new StatBase { Stat = Stat.ColdCallVsOpenRaiseBTN, PropertyName = nameof(Indicators.ColdCallVsBtnOpen), CreateStatDto = x => new StatDto(x.DidColdCallVsOpenRaiseBtn, x.CouldColdCallVsOpenRaiseBtn) },
            new StatBase { Stat = Stat.ColdCallVsOpenRaiseCO, PropertyName = nameof(Indicators.ColdCallVsCoOpen), CreateStatDto = x => new StatDto(x.DidColdCallVsOpenRaiseCo, x.CouldColdCallVsOpenRaiseCo) },
            new StatBase { Stat = Stat.ColdCallVsOpenRaiseSB, PropertyName = nameof(Indicators.ColdCallVsSbOpen), CreateStatDto = x => new StatDto(x.DidColdCallVsOpenRaiseSb, x.CouldColdCallVsOpenRaiseSb) },
            new StatBase { Stat = Stat.DelayedTurnCBet, PropertyName = nameof(Indicators.DidDelayedTurnCBet), CreateStatDto = x => new StatDto(x.DidDelayedTurnCBet, x.CouldDelayedTurnCBet) },
            new StatBase { Stat = Stat.DelayedTurnCBetIP, PropertyName = nameof(Indicators.DelayedTurnCBetIP), CreateStatDto = x => new StatDto(x.DidDelayedTurnCBetIP, x.CouldDelayedTurnCBetOOP) },
            new StatBase { Stat = Stat.DelayedTurnCBetOOP, PropertyName = nameof(Indicators.DelayedTurnCBetOOP), CreateStatDto = x => new StatDto(x.DidDelayedTurnCBetOOP, x.CouldDelayedTurnCBetOOP) },
            new StatBase { Stat = Stat.DelayedTurnCBetIn3BetPot, PropertyName = nameof(Indicators.DelayedTurnCBetIn3BetPot), CreateStatDto = x => new StatDto(x.DidDelayedTurnCBetIn3BetPot, x.CouldDelayedTurnCBetIn3BetPot) },
            new StatBase { Stat = Stat.DonkBet, PropertyName = nameof(Indicators.DonkBet), CreateStatDto = x => new StatDto(x.DidDonkBet, x.CouldDonkBet) },
            new StatBase { Stat = Stat.DoubleBarrel, PropertyName = nameof(Indicators.TurnCBet), CreateStatDto = x => new StatDto(x.Turncontinuationbetmade, x.Turncontinuationbetpossible) },
            new StatBase { Stat = Stat.DoubleBarrelSRP, PropertyName = nameof(Indicators.DoubleBarrelSRP), CreateStatDto = x => new StatDto(x.DoubleBarrelSRP, x.CouldDoubleBarrelSRP) },
            new StatBase { Stat = Stat.DoubleBarrel3BetPot, PropertyName = nameof(Indicators.DoubleBarrel3BetPot), CreateStatDto = x => new StatDto(x.DoubleBarrel3BetPot, x.CouldDoubleBarrel3BetPot) },
            new StatBase { Stat = Stat.FloatFlop, PropertyName = nameof(Indicators.FloatFlop), CreateStatDto = x => new StatDto(x.PlayedFloatFlop, x.Facingflopcontinuationbet)},
            new StatBase { Stat = Stat.FloatFlopThenBetTurn, PropertyName = nameof(Indicators.FloatFlopThenBetTurn), CreateStatDto = x => new StatDto(x.FloatFlopThenBetTurn, x.CouldFloatFlopThenBetTurn)},
            new StatBase { Stat = Stat.FlopAGG, PropertyName = nameof(Indicators.FlopAgg), CreateStatDto = x => new StatDto(x.TotalbetsFlop, x.FlopAggPossible) },
            new StatBase { Stat = Stat.FlopBet, PropertyName = nameof(Indicators.FlopBet), CreateStatDto = x => new StatDto(x.DidFlopBet, x.CouldFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeOneHalfOrLess, PropertyName = nameof(Indicators.FlopBetSizeOneHalfOrLess), CreateStatDto = x => new StatDto(x.FlopBetSizeOneHalfOrLess, x.DidFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeOneQuarterOrLess, PropertyName = nameof(Indicators.FlopBetSizeOneQuarterOrLess), CreateStatDto = x => new StatDto(x.FlopBetSizeOneQuarterOrLess, x.DidFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeTwoThirdsOrLess, PropertyName = nameof(Indicators.FlopBetSizeTwoThirdsOrLess), CreateStatDto = x => new StatDto(x.FlopBetSizeTwoThirdsOrLess, x.DidFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeThreeQuartersOrLess, PropertyName = nameof(Indicators.FlopBetSizeThreeQuartersOrLess), CreateStatDto = x => new StatDto(x.FlopBetSizeThreeQuartersOrLess, x.DidFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeOneOrLess, PropertyName = nameof(Indicators.FlopBetSizeOneOrLess), CreateStatDto = x => new StatDto(x.FlopBetSizeOneOrLess, x.DidFlopBet) },
            new StatBase { Stat = Stat.FlopBetSizeMoreThanOne, PropertyName = nameof(Indicators.FlopBetSizeMoreThanOne), CreateStatDto = x => new StatDto(x.FlopBetSizeMoreThanOne, x.DidFlopBet) },
            new StatBase { Stat = Stat.FoldBBvsSBSteal, PropertyName = nameof(Indicators.FoldBBvsSBSteal), CreateStatDto = x => new StatDto(x.FoldBBvsSBSteal, x.CouldFoldBBvsSBSteal) },
            new StatBase { Stat = Stat.FlopCBetMonotone, PropertyName = nameof(Indicators.FlopCBetMonotone), CreateStatDto = x => new StatDto(x.FlopContinuationBetMonotonePotMade, x.FlopContinuationBetMonotonePotPossible) },
            new StatBase { Stat = Stat.FlopCBetMW, PropertyName = nameof(Indicators.FlopCBetMW), CreateStatDto = x => new StatDto(x.MultiWayFlopContinuationBetMade, x.MultiWayFlopContinuationBetPossible) },
            new StatBase { Stat = Stat.FlopCBetRag, PropertyName = nameof(Indicators.FlopCBetRag), CreateStatDto = x => new StatDto(x.FlopContinuationBetRagPotMade, x.FlopContinuationBetRagPotPossible) },
            new StatBase { Stat = Stat.FlopCBetVsOneOpp, PropertyName = nameof(Indicators.FlopCBetVsOneOpp), CreateStatDto = x => new StatDto(x.FlopContinuationBetVsOneOpponentMade, x.FlopContinuationBetVsOneOpponentPossible) },
            new StatBase { Stat = Stat.FlopCBetVsTwoOpp, PropertyName = nameof(Indicators.FlopCBetVsTwoOpp), CreateStatDto = x => new StatDto(x.FlopContinuationBetVsTwoOpponentsMade, x.FlopContinuationBetVsTwoOpponentsPossible) },
            new StatBase { Stat = Stat.FlopCheckBehind, PropertyName = nameof(Indicators.FlopCheckBehind), CreateStatDto = x => new StatDto(x.DidFlopCheckBehind, x.CouldFlopCheckBehind) },
            new StatBase { Stat = Stat.FlopCheckRaise, PropertyName = nameof(Indicators.FlopCheckRaise), CreateStatDto = x => new StatDto(x.DidFlopCheckRaise, x.Sawflop) },
            new StatBase { Stat = Stat.FoldFlop, PropertyName = nameof(Indicators.FoldFlop), CreateStatDto = x => new StatDto(x.FoldedFlop, x.FacedBetOnFlop) },
            new StatBase { Stat = Stat.FoldTo3Bet, PropertyName = nameof(Indicators.FoldToThreeBet), CreateStatDto = x => new StatDto(x.Foldedtothreebetpreflop, x.Facedthreebetpreflop) },
            new StatBase { Stat = Stat.FoldTo3BetIP, PropertyName = nameof(Indicators.FoldToThreeBetIP), CreateStatDto = x => new StatDto(x.FoldToThreeBetIP, x.FacedThreeBetIP) },
            new StatBase { Stat = Stat.FoldTo3BetOOP, PropertyName = nameof(Indicators.FoldToThreeBetOOP), CreateStatDto = x => new StatDto(x.FoldToThreeBetOOP, x.FacedThreeBetOOP) },
            new StatBase { Stat = Stat.FoldTo4Bet, PropertyName = nameof(Indicators.FoldToFourBet), CreateStatDto = x => new StatDto(x.Foldedtofourbetpreflop, x.Facedfourbetpreflop) },
            new StatBase { Stat = Stat.FoldTo5Bet, PropertyName = nameof(Indicators.FoldToFiveBet), CreateStatDto = x => new StatDto(x.FoldedTo5Bet, x.Faced5Bet) },
            new StatBase { Stat = Stat.FoldToCBet, PropertyName = nameof(Indicators.FoldCBet), CreateStatDto = x => new StatDto(x.Foldedtoflopcontinuationbet, x.Facingflopcontinuationbet) },
            new StatBase { Stat = Stat.FoldToCBetFromFourBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromFourBetPot), CreateStatDto = x => new StatDto(x.FoldedToFlopContinuationBetFromFourBetPot, x.FacingFlopContinuationBetFromFourBetPot) },
            new StatBase { Stat = Stat.FoldToCBetFromThreeBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromThreeBetPot), CreateStatDto = x => new StatDto(x.FoldedToFlopContinuationBetFromThreeBetPot, x.FacingFlopContinuationBetFromThreeBetPot) },
            new StatBase { Stat = Stat.FoldToCheckRaiseVsFlopCBet, PropertyName = nameof(Indicators.FoldedCheckRaiseVsFlopCBet), CreateStatDto = x => new StatDto(x.FoldedCheckRaiseVsFlopCBet, x.FacedCheckRaiseVsFlopCBet) },
            new StatBase { Stat = Stat.FoldToDonkBet, PropertyName =  nameof(Indicators.FoldToDonkBet), CreateStatDto = x => new StatDto(x.FoldedToDonkBet, x.FacedDonkBet) },
            new StatBase { Stat = Stat.FoldToDoubleBarrel, PropertyName =  nameof(Indicators.FoldToTurnCBet), CreateStatDto = x => new StatDto(x.Foldedtoturncontinuationbet, x.Facingturncontinuationbet) },
            new StatBase { Stat = Stat.FoldToDoubleBarrelIn3BetPot, PropertyName =  nameof(Indicators.FoldToTurnCBetIn3BetPot), CreateStatDto = x => new StatDto(x.FoldToTurnCBetIn3BetPot, x.FacedToTurnCBetIn3BetPot) },
            new StatBase { Stat = Stat.FoldToFlopRaise, PropertyName = nameof(Indicators.FoldToFlopRaise), CreateStatDto = x => new StatDto(x.FoldedFacedRaiseFlop, x.FacedRaiseFlop) },
            new StatBase { Stat = Stat.FoldToFlopCBetIP, PropertyName = nameof(Indicators.FoldToFlopCBetIP), CreateStatDto = x => new StatDto(x.FoldToFlopcontinuationbetIP, x.FacingflopcontinuationbetIP) },
            new StatBase { Stat = Stat.FoldToFlopCBetOOP, PropertyName = nameof(Indicators.FoldToFlopCBetOOP), CreateStatDto = x => new StatDto(x.FoldToFlopcontinuationbetOOP, x.FacingflopcontinuationbetOOP) },
            new StatBase { Stat = Stat.FoldToFlopCheckRaise, PropertyName = nameof(Indicators.FoldToFlopCheckRaise), CreateStatDto = x => new StatDto(x.FoldedToFlopCheckRaise, x.FacedFlopCheckRaise) },
            new StatBase { Stat = Stat.FoldToRiverCBet, PropertyName = nameof(Indicators.FoldToRiverCBet), CreateStatDto = x => new StatDto(x.Foldedtorivercontinuationbet, x.Facingrivercontinuationbet) },
            new StatBase { Stat = Stat.FoldToSqueez, PropertyName = nameof(Indicators.FoldToSqueez), CreateStatDto = x => new StatDto(x.FoldedFacedSqueez, x.FacedSqueez) },
            new StatBase { Stat = Stat.FoldToSteal, PropertyName = nameof(Indicators.BlindsFoldSteal), CreateStatDto = x => new StatDto(x.Bigblindstealfolded + x.Smallblindstealfolded, x.Bigblindstealfaced + x.Smallblindstealfaced) },
            new StatBase { Stat = Stat.FoldTo3BetInEPvs3BetMP, PropertyName = nameof(Indicators.FoldTo3BetInEPvs3BetMP), CreateStatDto = x => new StatDto(x.FoldTo3BetInEPvs3BetMP, x.CouldFoldTo3BetInEPvs3BetMP) },
            new StatBase { Stat = Stat.FoldTo3BetInEPvs3BetCO, PropertyName = nameof(Indicators.FoldTo3BetInEPvs3BetCO), CreateStatDto = x => new StatDto(x.FoldTo3BetInEPvs3BetCO, x.CouldFoldTo3BetInEPvs3BetCO) },
            new StatBase { Stat = Stat.FoldTo3BetInEPvs3BetBTN, PropertyName = nameof(Indicators.FoldTo3BetInEPvs3BetBTN), CreateStatDto = x => new StatDto(x.FoldTo3BetInEPvs3BetBTN, x.CouldFoldTo3BetInEPvs3BetBTN) },
            new StatBase { Stat = Stat.FoldTo3BetInEPvs3BetSB, PropertyName = nameof(Indicators.FoldTo3BetInEPvs3BetSB), CreateStatDto = x => new StatDto(x.FoldTo3BetInEPvs3BetSB, x.CouldFoldTo3BetInEPvs3BetSB) },
            new StatBase { Stat = Stat.FoldTo3BetInEPvs3BetBB, PropertyName = nameof(Indicators.FoldTo3BetInEPvs3BetBB), CreateStatDto = x => new StatDto(x.FoldTo3BetInEPvs3BetBB, x.CouldFoldTo3BetInEPvs3BetBB) },
            new StatBase { Stat = Stat.FoldTo3BetInMPvs3BetCO, PropertyName = nameof(Indicators.FoldTo3BetInMPvs3BetCO), CreateStatDto = x => new StatDto(x.FoldTo3BetInMPvs3BetCO, x.CouldFoldTo3BetInMPvs3BetCO) },
            new StatBase { Stat = Stat.FoldTo3BetInMPvs3BetBTN, PropertyName = nameof(Indicators.FoldTo3BetInMPvs3BetBTN), CreateStatDto = x => new StatDto(x.FoldTo3BetInMPvs3BetBTN, x.CouldFoldTo3BetInMPvs3BetBTN) },
            new StatBase { Stat = Stat.FoldTo3BetInMPvs3BetSB, PropertyName = nameof(Indicators.FoldTo3BetInMPvs3BetSB), CreateStatDto = x => new StatDto(x.FoldTo3BetInMPvs3BetSB, x.CouldFoldTo3BetInMPvs3BetSB) },
            new StatBase { Stat = Stat.FoldTo3BetInMPvs3BetBB, PropertyName = nameof(Indicators.FoldTo3BetInMPvs3BetBB), CreateStatDto = x => new StatDto(x.FoldTo3BetInMPvs3BetBB, x.CouldFoldTo3BetInMPvs3BetBB) },
            new StatBase { Stat = Stat.FoldTo3BetInCOvs3BetBTN, PropertyName = nameof(Indicators.FoldTo3BetInCOvs3BetBTN), CreateStatDto = x => new StatDto(x.FoldTo3BetInCOvs3BetBTN, x.CouldFoldTo3BetInCOvs3BetBTN) },
            new StatBase { Stat = Stat.FoldTo3BetInCOvs3BetSB, PropertyName = nameof(Indicators.FoldTo3BetInCOvs3BetSB), CreateStatDto = x => new StatDto(x.FoldTo3BetInCOvs3BetSB, x.CouldFoldTo3BetInCOvs3BetSB) },
            new StatBase { Stat = Stat.FoldTo3BetInCOvs3BetBB, PropertyName = nameof(Indicators.FoldTo3BetInCOvs3BetBB), CreateStatDto = x => new StatDto(x.FoldTo3BetInCOvs3BetBB, x.CouldFoldTo3BetInCOvs3BetBB) },
            new StatBase { Stat = Stat.FoldTo3BetInBTNvs3BetSB, PropertyName = nameof(Indicators.FoldTo3BetInBTNvs3BetSB), CreateStatDto = x => new StatDto(x.FoldTo3BetInBTNvs3BetSB, x.CouldFoldTo3BetInBTNvs3BetSB) },
            new StatBase { Stat = Stat.FoldTo3BetInBTNvs3BetBB, PropertyName = nameof(Indicators.FoldTo3BetInBTNvs3BetBB), CreateStatDto = x => new StatDto(x.FoldTo3BetInBTNvs3BetBB, x.CouldFoldTo3BetInBTNvs3BetBB) },
            new StatBase { Stat = Stat.FoldTurn, PropertyName = nameof(Indicators.FoldTurn), CreateStatDto = x => new StatDto(x.FoldedTurn, x.FacedBetOnTurn) },
            new StatBase { Stat = Stat.FoldToTurnRaise, PropertyName = nameof(Indicators.FoldToTurnRaise), CreateStatDto = x => new StatDto(x.FoldedFacedRaiseTurn, x.FacedRaiseTurn) },
            new StatBase { Stat = Stat.FoldToTurnCheckRaise, PropertyName = nameof(Indicators.FoldToTurnCheckRaise), CreateStatDto = x => new StatDto(x.FoldedToTurnCheckRaise, x.FacedTurnCheckRaise) },
            new StatBase { Stat = Stat.FoldToTurnProbeIP, PropertyName = nameof(Indicators.FoldToProbeBetTurn), CreateStatDto = x => new StatDto(x.FoldedToProbeBetTurn, x.FacedProbeBetTurn) },
            new StatBase { Stat = Stat.FoldToRiverCheckRaise, PropertyName = nameof(Indicators.FoldToRiverCheckRaise), CreateStatDto = x => new StatDto(x.FoldedToRiverCheckRaise, x.FacedRiverCheckRaise) },
            new StatBase { Stat = Stat.FoldToRiverRaise, PropertyName = nameof(Indicators.FoldToRiverRaise), CreateStatDto = x => new StatDto(x.FoldedFacedRaiseRiver, x.FacedRaiseRiver) },
            new StatBase { Stat = Stat.FoldToRiverProbeIP, PropertyName = nameof(Indicators.FoldToProbeBetRiver), CreateStatDto = x => new StatDto(x.FoldedToProbeBetRiver, x.FacedProbeBetRiver) },
            new StatBase { Stat = Stat.Limp, PropertyName = nameof(Indicators.DidLimp), CreateStatDto = x => new StatDto(x.LimpMade, x.LimpPossible) },
            new StatBase { Stat = Stat.LimpBtn, PropertyName = nameof(Indicators.LimpBtn), CreateStatDto = x => new StatDto(x.LimpBtn, x.LimpPossibleBN) },
            new StatBase { Stat = Stat.LimpCall, PropertyName = nameof(Indicators.DidLimpCall), CreateStatDto = x => new StatDto(x.LimpCalled, x.LimpFaced) },
            new StatBase { Stat = Stat.LimpCo, PropertyName = nameof(Indicators.LimpCo), CreateStatDto = x => new StatDto(x.LimpCo, x.LimpPossibleCO) },
            new StatBase { Stat = Stat.LimpEp, PropertyName = nameof(Indicators.LimpEp), CreateStatDto = x => new StatDto(x.LimpEp, x.LimpPossibleEP) },
            new StatBase { Stat = Stat.LimpFold, PropertyName = nameof(Indicators.DidLimpFold), CreateStatDto = x => new StatDto(x.LimpFolded, x.LimpFaced) },
            new StatBase { Stat = Stat.LimpMp, PropertyName = nameof(Indicators.LimpMp), CreateStatDto = x => new StatDto(x.LimpMp, x.LimpPossibleMP) },
            new StatBase { Stat = Stat.LimpReraise, PropertyName = nameof(Indicators.DidLimpReraise), CreateStatDto = x => new StatDto(x.LimpReraised, x.LimpFaced) },
            new StatBase { Stat = Stat.LimpSb, PropertyName = nameof(Indicators.LimpSb), CreateStatDto = x => new StatDto(x.LimpSb, x.LimpPossibleSB) },
            new StatBase { Stat = Stat.MRatio, PropertyName = nameof(Indicators.MRatio) },
            new StatBase { Stat = Stat.NetWon, PropertyName = nameof(Indicators.NetWon) },
            new StatBase { Stat = Stat.PFR, PropertyName = nameof(Indicators.PFR), CreateStatDto = x => new StatDto(x.Pfrhands, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInBB, PropertyName = nameof(Indicators.PFRInBB), CreateStatDto = x => new StatDto(x.PfrInBb, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInBTN, PropertyName = nameof(Indicators.PFRInBTN), CreateStatDto = x => new StatDto(x.PfrInBtn, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInCO, PropertyName = nameof(Indicators.PFRInCO), CreateStatDto = x => new StatDto(x.PfrInCo, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInEP, PropertyName = nameof(Indicators.PFRInEP), CreateStatDto = x => new StatDto(x.PfrInEp, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInMP, PropertyName = nameof(Indicators.PFRInMP), CreateStatDto = x => new StatDto(x.PfrInMp, x.Totalhands) },
            new StatBase { Stat = Stat.PFRInSB, PropertyName = nameof(Indicators.PFRInSB), CreateStatDto = x => new StatDto(x.PfrInSb, x.Totalhands) },
            new StatBase { Stat = Stat.PlayerInfoIcon },
            new StatBase { Stat = Stat.ProbeBetTurn, PropertyName = nameof(Indicators.ProbeBetTurn), CreateStatDto = x => new StatDto(x.ProbeBetTurn, x.CouldProbeBetTurn) },
            new StatBase { Stat = Stat.ProbeBetRiver, PropertyName = nameof(Indicators.ProbeBetRiver), CreateStatDto = x => new StatDto(x.ProbeBetRiver, x.CouldProbeBetRiver) },
            new StatBase { Stat = Stat.RaiseCBet, PropertyName = nameof(Indicators.RaiseCBet), CreateStatDto = x => new StatDto(x.Raisedflopcontinuationbet, x.Facingflopcontinuationbet) },
            new StatBase { Stat = Stat.RaiseFlop, PropertyName = nameof(Indicators.RaiseFlop), CreateStatDto = x => new StatDto(x.DidRaiseFlop, x.CouldRaiseFlop) },
            new StatBase { Stat = Stat.RaiseFlopCBetIn3BetPot, PropertyName = nameof(Indicators.RaiseFlopCBetIn3BetPot), CreateStatDto = x => new StatDto(x.RaisedFlopCBetIn3BetPot, x.CouldRaiseFlopCBetIn3BetPot) },
            new StatBase { Stat = Stat.RaiseFrequencyFactor, PropertyName = nameof(Indicators.RaiseFrequencyFactor), CreateStatDto = x => new StatDto(x.DidRaiseFlop + x.DidRaiseTurn + x.DidRaiseRiver, x.CouldRaiseFlop + x.CouldRaiseTurn + x.CouldRaiseRiver) },
            new StatBase { Stat = Stat.RaiseLimpers, PropertyName = nameof(Indicators.RaiseLimpers), CreateStatDto = x => new StatDto(x.IsRaisedLimpers, x.CouldRaiseLimpers) },
            new StatBase { Stat = Stat.RaiseLimpersInMP, PropertyName = nameof(Indicators.RaiseLimpersInMP), CreateStatDto = x => new StatDto(x.RaisedLimpersMP, x.CouldRaiseLimpersMP) },
            new StatBase { Stat = Stat.RaiseLimpersInCO, PropertyName = nameof(Indicators.RaiseLimpersInCO), CreateStatDto = x => new StatDto(x.RaisedLimpersCO, x.CouldRaiseLimpersCO) },
            new StatBase { Stat = Stat.RaiseLimpersInBN, PropertyName = nameof(Indicators.RaiseLimpersInBN), CreateStatDto = x => new StatDto(x.RaisedLimpersBN, x.CouldRaiseLimpersBN) },
            new StatBase { Stat = Stat.RaiseLimpersInSB, PropertyName = nameof(Indicators.RaiseLimpersInSB), CreateStatDto = x => new StatDto(x.RaisedLimpersSB, x.CouldRaiseLimpersSB) },
            new StatBase { Stat = Stat.RaiseLimpersInBB, PropertyName = nameof(Indicators.RaiseLimpersInBB), CreateStatDto = x => new StatDto(x.RaisedLimpersBB, x.CouldRaiseLimpersBB) },
            new StatBase { Stat = Stat.RaiseRiver, PropertyName = nameof(Indicators.RaiseRiver), CreateStatDto = x => new StatDto(x.DidRaiseRiver, x.CouldRaiseRiver) },
            new StatBase { Stat = Stat.RaiseTurn, PropertyName = nameof(Indicators.RaiseTurn), CreateStatDto = x => new StatDto(x.DidRaiseTurn, x.CouldRaiseTurn) },
            new StatBase { Stat = Stat.RecentAgg, PropertyName = nameof(HudLightIndicators.RecentAggPr), CreateStatDto = x => new StatDto(0,0) },
            new StatBase { Stat = Stat.RiverAGG, PropertyName = nameof(Indicators.RiverAgg), CreateStatDto = x => new StatDto(x.TotalbetsRiver, x.RiverAggPossible) },
            new StatBase { Stat = Stat.RiverBet, PropertyName = nameof(Indicators.RiverBet), CreateStatDto = x => new StatDto(x.DidRiverBet, x.CouldRiverBet) },
            new StatBase { Stat = Stat.RiverBetSizeMoreThanOne, PropertyName = nameof(Indicators.RiverBetSizeMoreThanOne), CreateStatDto = x => new StatDto(x.RiverBetSizeMoreThanOne, x.DidRiverBet) },
            new StatBase { Stat = Stat.RiverCallEffeciency, PropertyName = nameof(Indicators.RiverCallEffeciency) },
            new StatBase { Stat = Stat.RiverCheckCall, PropertyName = nameof(Indicators.RiverCheckCall), CreateStatDto = x => new StatDto(x.CheckedCalledRiver, x.CheckedThenFacedBetOnRiver) },
            new StatBase { Stat = Stat.RiverCheckFold, PropertyName = nameof(Indicators.RiverCheckFold), CreateStatDto = x => new StatDto(x.CheckedFoldedRiver, x.CheckedThenFacedBetOnRiver) },
            new StatBase { Stat = Stat.RiverSeen, PropertyName = nameof(Indicators.RiverSeen), CreateStatDto = x => new StatDto(x.SawRiver, x.WasRiver) },
            new StatBase { Stat = Stat.S3Bet, PropertyName = nameof(Indicators.ThreeBet), CreateStatDto = x => new StatDto(x.Didthreebet, x.Couldthreebet) },
            new StatBase { Stat = Stat.S3BetIP, PropertyName = nameof(Indicators.ThreeBetIP), CreateStatDto = x => new StatDto(x.DidThreeBetIp, x.CouldThreeBetIp) },
            new StatBase { Stat = Stat.S3BetOOP, PropertyName = nameof(Indicators.ThreeBetOOP), CreateStatDto = x => new StatDto(x.DidThreeBetOop, x.CouldThreeBetOop) },
            new StatBase { Stat = Stat.S4Bet, PropertyName = nameof(Indicators.FourBet), CreateStatDto = x => new StatDto(x.Didfourbet, x.Couldfourbet) },
            new StatBase { Stat = Stat.S4BetBB, PropertyName = nameof(HudLightIndicators.FourBetInBB), CreateStatDto = x => new StatDto(x.DidFourBetBB, x.CouldFourBetBB) },
            new StatBase { Stat = Stat.S4BetBTN, PropertyName = nameof(HudLightIndicators.FourBetInBTN), CreateStatDto = x => new StatDto(x.DidFourBetBN, x.CouldFourBetBN) },
            new StatBase { Stat = Stat.S4BetCO, PropertyName = nameof(HudLightIndicators.FourBetInCO), CreateStatDto = x => new StatDto(x.DidFourBetCO, x.CouldFourBetCO) },
            new StatBase { Stat = Stat.S4BetEP, PropertyName = nameof(HudLightIndicators.FourBetInEP), CreateStatDto = x => new StatDto(x.DidFourBetEP, x.CouldFourBetEP) },
            new StatBase { Stat = Stat.S4BetMP, PropertyName = nameof(HudLightIndicators.FourBetInMP), CreateStatDto = x => new StatDto(x.DidFourBetMP, x.CouldFourBetMP) },
            new StatBase { Stat = Stat.S4BetSB, PropertyName = nameof(HudLightIndicators.FourBetInSB), CreateStatDto = x => new StatDto(x.DidFourBetSB, x.CouldFourBetSB) },
            new StatBase { Stat = Stat.S5Bet, PropertyName = nameof(Indicators.FiveBet), CreateStatDto = x => new StatDto(x.Did5Bet, x.Could5Bet) },
            new StatBase { Stat = Stat.ShovedFlopAfter4Bet, PropertyName = nameof(Indicators.ShovedFlopAfter4Bet), CreateStatDto = x => new StatDto(x.ShovedFlopAfter4Bet, x.CouldShoveFlopAfter4Bet) },
            new StatBase { Stat = Stat.Squeeze, PropertyName = nameof(Indicators.Squeeze), CreateStatDto = x => new StatDto(x.Didsqueeze, x.Totalhands) },
            new StatBase { Stat = Stat.Steal, PropertyName = nameof(Indicators.Steal), CreateStatDto = x => new StatDto(x.StealMade, x.StealPossible) },
            new StatBase { Stat = Stat.ThreeBet_BB, PropertyName = nameof(HudLightIndicators.ThreeBet_BB), CreateStatDto = x => new StatDto(x.DidThreeBetBB, x.CouldThreeBetBB) },
            new StatBase { Stat = Stat.ThreeBet_BN, PropertyName = nameof(HudLightIndicators.ThreeBet_BN), CreateStatDto = x => new StatDto(x.DidThreeBetBN, x.CouldThreeBetBN) },
            new StatBase { Stat = Stat.ThreeBet_CO, PropertyName = nameof(HudLightIndicators.ThreeBet_CO), CreateStatDto = x => new StatDto(x.DidThreeBetCO, x.CouldThreeBetCO) },
            new StatBase { Stat = Stat.ThreeBet_EP, PropertyName = nameof(HudLightIndicators.ThreeBet_EP), CreateStatDto = x => new StatDto(x.DidThreeBetEP, x.CouldThreeBetEP) },
            new StatBase { Stat = Stat.ThreeBet_MP, PropertyName = nameof(HudLightIndicators.ThreeBet_MP), CreateStatDto = x => new StatDto(x.DidThreeBetMP, x.CouldThreeBetMP) },
            new StatBase { Stat = Stat.ThreeBet_SB, PropertyName = nameof(HudLightIndicators.ThreeBet_SB), CreateStatDto = x => new StatDto(x.DidThreeBetSB, x.CouldThreeBetSB) },
            new StatBase { Stat = Stat.ThreeBetVsSteal, PropertyName = nameof(Indicators.ThreeBetVsSteal), CreateStatDto = x => new StatDto(x.DidThreeBetVsSteal, x.CouldThreeBetVsSteal)  },
            new StatBase { Stat = Stat.ThreeBetMPvsEP, PropertyName = nameof(Indicators.ThreeBetMPvsEP), CreateStatDto = x => new StatDto(x.ThreeBetMPvsEP, x.CouldThreeBetMPvsEP) },
            new StatBase { Stat = Stat.ThreeBetCOvsEP, PropertyName = nameof(Indicators.ThreeBetCOvsEP), CreateStatDto = x => new StatDto(x.ThreeBetCOvsEP, x.CouldThreeBetCOvsEP) },
            new StatBase { Stat = Stat.ThreeBetCOvsMP, PropertyName = nameof(Indicators.ThreeBetCOvsMP), CreateStatDto = x => new StatDto(x.ThreeBetCOvsMP, x.CouldThreeBetCOvsMP) },
            new StatBase { Stat = Stat.ThreeBetBTNvsEP, PropertyName = nameof(Indicators.ThreeBetBTNvsEP), CreateStatDto = x => new StatDto(x.ThreeBetBTNvsEP, x.CouldThreeBetBTNvsEP) },
            new StatBase { Stat = Stat.ThreeBetBTNvsMP, PropertyName = nameof(Indicators.ThreeBetBTNvsMP), CreateStatDto = x => new StatDto(x.ThreeBetBTNvsMP, x.CouldThreeBetBTNvsMP) },
            new StatBase { Stat = Stat.ThreeBetBTNvsCO, PropertyName = nameof(Indicators.ThreeBetBTNvsCO), CreateStatDto = x => new StatDto(x.ThreeBetBTNvsCO, x.CouldThreeBetBTNvsCO) },
            new StatBase { Stat = Stat.ThreeBetSBvsEP, PropertyName = nameof(Indicators.ThreeBetSBvsEP), CreateStatDto = x => new StatDto(x.ThreeBetSBvsEP, x.CouldThreeBetSBvsEP) },
            new StatBase { Stat = Stat.ThreeBetSBvsMP, PropertyName = nameof(Indicators.ThreeBetSBvsMP), CreateStatDto = x => new StatDto(x.ThreeBetSBvsMP, x.CouldThreeBetSBvsMP) },
            new StatBase { Stat = Stat.ThreeBetSBvsCO, PropertyName = nameof(Indicators.ThreeBetSBvsCO), CreateStatDto = x => new StatDto(x.ThreeBetSBvsCO, x.CouldThreeBetSBvsCO) },
            new StatBase { Stat = Stat.ThreeBetSBvsBTN, PropertyName = nameof(Indicators.ThreeBetSBvsBTN), CreateStatDto = x => new StatDto(x.ThreeBetSBvsBTN, x.CouldThreeBetSBvsBTN) },
            new StatBase { Stat = Stat.ThreeBetBBvsEP, PropertyName = nameof(Indicators.ThreeBetBBvsEP), CreateStatDto = x => new StatDto(x.ThreeBetBBvsEP, x.CouldThreeBetBBvsEP) },
            new StatBase { Stat = Stat.ThreeBetBBvsMP, PropertyName = nameof(Indicators.ThreeBetBBvsMP), CreateStatDto = x => new StatDto(x.ThreeBetBBvsMP, x.CouldThreeBetBBvsMP) },
            new StatBase { Stat = Stat.ThreeBetBBvsCO, PropertyName = nameof(Indicators.ThreeBetBBvsCO), CreateStatDto = x => new StatDto(x.ThreeBetBBvsCO, x.CouldThreeBetBBvsCO) },
            new StatBase { Stat = Stat.ThreeBetBBvsBTN, PropertyName = nameof(Indicators.ThreeBetBBvsBTN), CreateStatDto = x => new StatDto(x.ThreeBetBBvsBTN, x.CouldThreeBetBBvsBTN) },
            new StatBase { Stat = Stat.ThreeBetBBvsSB, PropertyName = nameof(Indicators.ThreeBetBBvsSB), CreateStatDto = x => new StatDto(x.ThreeBetBBvsSB, x.CouldThreeBetBBvsSB) },
            new StatBase { Stat = Stat.TripleBarrel, PropertyName = nameof(Indicators.RiverCBet), CreateStatDto = x => new StatDto(x.Rivercontinuationbetmade, x.Rivercontinuationbetpossible) },
            new StatBase { Stat = Stat.TripleBarrelSRP, PropertyName = nameof(Indicators.TripleBarrelSRP), CreateStatDto = x => new StatDto(x.TripleBarrelSRP, x.CouldTripleBarrelSRP) },
            new StatBase { Stat = Stat.TripleBarrel3BetPot, PropertyName = nameof(Indicators.TripleBarrel3BetPot), CreateStatDto = x => new StatDto(x.TripleBarrel3BetPot, x.CouldTripleBarrel3BetPot) },
            new StatBase { Stat = Stat.TotalHands, PropertyName = nameof(Indicators.TotalHands) },
            new StatBase { Stat = Stat.TurnAF, PropertyName = nameof(Indicators.TurnAF) },
            new StatBase { Stat = Stat.TrueAggression, PropertyName = nameof(Indicators.TrueAggression), CreateStatDto = x => new StatDto(x.TotalAggressiveBets, x.Totalpostflopstreetsplayed - x.Flopcontinuationbetmade) },
            new StatBase { Stat = Stat.TurnAGG, PropertyName = nameof(Indicators.TurnAgg), CreateStatDto = x => new StatDto(x.TotalbetsTurn, x.TurnAggPossible) },
            new StatBase { Stat = Stat.TurnBet, PropertyName = nameof(Indicators.TurnBet), CreateStatDto = x => new StatDto(x.DidTurnBet, x.CouldTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeOneHalfOrLess, PropertyName = nameof(Indicators.TurnBetSizeOneHalfOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeOneHalfOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeOneQuarterOrLess, PropertyName = nameof(Indicators.TurnBetSizeOneQuarterOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeOneQuarterOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeOneThirdOrLess, PropertyName = nameof(Indicators.TurnBetSizeOneThirdOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeOneThirdOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeTwoThirdsOrLess, PropertyName = nameof(Indicators.TurnBetSizeTwoThirdsOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeTwoThirdsOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeThreeQuartersOrLess, PropertyName = nameof(Indicators.TurnBetSizeThreeQuartersOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeThreeQuartersOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeOneOrLess, PropertyName = nameof(Indicators.TurnBetSizeOneOrLess), CreateStatDto = x => new StatDto(x.TurnBetSizeOneOrLess, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnBetSizeMoreThanOne, PropertyName = nameof(Indicators.TurnBetSizeMoreThanOne), CreateStatDto = x => new StatDto(x.TurnBetSizeMoreThanOne, x.DidTurnBet) },
            new StatBase { Stat = Stat.TurnCheckRaise, PropertyName = nameof(Indicators.TurnCheckRaise), CreateStatDto = x => new StatDto(x.DidTurnCheckRaise, x.SawTurn) },
            new StatBase { Stat = Stat.TurnSeen, PropertyName = nameof(Indicators.TurnSeen), CreateStatDto = x => new StatDto(x.SawTurn, x.WasTurn) },
            new StatBase { Stat = Stat.UO_PFR_BB, PropertyName = nameof(HudLightIndicators.UO_PFR_BB), CreateStatDto = x => new StatDto(x.UO_PFR_BB, x.IsUnopened && x.Position.IsBBPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.UO_PFR_BN, PropertyName = nameof(HudLightIndicators.UO_PFR_BN), CreateStatDto = x => new StatDto(x.UO_PFR_BN, x.IsUnopened && x.Position.IsBTNPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.UO_PFR_CO, PropertyName = nameof(HudLightIndicators.UO_PFR_CO), CreateStatDto = x => new StatDto(x.UO_PFR_CO, x.IsUnopened && x.Position.IsCOPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.UO_PFR_EP, PropertyName = nameof(HudLightIndicators.UO_PFR_EP), CreateStatDto = x => new StatDto(x.UO_PFR_EP, x.IsUnopened && x.Position.IsEPPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.UO_PFR_MP, PropertyName = nameof(HudLightIndicators.UO_PFR_MP), CreateStatDto = x => new StatDto(x.UO_PFR_MP, x.IsUnopened && x.Position.IsMPPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.UO_PFR_SB, PropertyName = nameof(HudLightIndicators.UO_PFR_SB), CreateStatDto = x => new StatDto(x.UO_PFR_SB, x.IsUnopened && x.Position.IsSBPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_BB, PropertyName = nameof(HudLightIndicators.VPIP_BB), CreateStatDto = x => new StatDto(x.VPIPBB, x.Position.IsBBPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_BN, PropertyName = nameof(HudLightIndicators.VPIP_BN), CreateStatDto = x => new StatDto(x.VPIPBN, x.Position.IsBTNPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_CO, PropertyName = nameof(HudLightIndicators.VPIP_CO), CreateStatDto = x => new StatDto(x.VPIPCO, x.Position.IsCOPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_EP, PropertyName = nameof(HudLightIndicators.VPIP_EP), CreateStatDto = x => new StatDto(x.VPIPEP, x.Position.IsEPPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_MP, PropertyName = nameof(HudLightIndicators.VPIP_MP), CreateStatDto = x => new StatDto(x.VPIPMP, x.Position.IsMPPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP_SB, PropertyName = nameof(HudLightIndicators.VPIP_SB), CreateStatDto = x => new StatDto(x.VPIPSB, x.Position.IsSBPosition() ? 1 : 0) },
            new StatBase { Stat = Stat.VPIP, PropertyName = nameof(Indicators.VPIP), CreateStatDto = x => new StatDto(x.Vpiphands, x.Totalhands) },
            new StatBase { Stat = Stat.WSSD, PropertyName = nameof(Indicators.WSSD), CreateStatDto = x => new StatDto(x.Wonshowdown, x.Sawshowdown) },
            new StatBase { Stat = Stat.WTSD, PropertyName = nameof(Indicators.WTSD), CreateStatDto = x => new StatDto(x.Sawshowdown, x.Sawflop) },
            new StatBase { Stat = Stat.WTSDAfterCalling3Bet, PropertyName = nameof(Indicators.WTSDAfterCalling3Bet), CreateStatDto = x => new StatDto(x.WTSDAfterCalling3Bet, x.WTSDAfterCalling3BetOpportunity) },
            new StatBase { Stat = Stat.WTSDAfterCallingPfr, PropertyName = nameof(Indicators.WTSDAfterCallingPfr), CreateStatDto = x => new StatDto(x.WTSDAfterCallingPfr, x.WTSDAfterCallingPfrOpportunity) },
            new StatBase { Stat = Stat.WTSDAfterNotCBettingFlopAsPfr, PropertyName = nameof(Indicators.WTSDAfterNotCBettingFlopAsPfr), CreateStatDto = x => new StatDto(x.WTSDAfterNotCBettingFlopAsPfr, x.WTSDAfterNotCBettingFlopAsPfrOpportunity) },
            new StatBase { Stat = Stat.WTSDAfterSeeingTurn, PropertyName = nameof(Indicators.WTSDAfterSeeingTurn), CreateStatDto = x => new StatDto(x.WTSDAfterSeeingTurn, x.SawTurn) },
            new StatBase { Stat = Stat.WTSDAsPF3Bettor, PropertyName = nameof(Indicators.WTSDAsPF3Bettor), CreateStatDto = x => new StatDto(x.WTSDAsPF3Bettor, x.WTSDAsPF3BettorOpportunity) },
            new StatBase { Stat = Stat.WWSF, PropertyName = nameof(Indicators.WSWSF), CreateStatDto = x => new StatDto(x.Wonhandwhensawflop, x.Sawflop) }
            }).ToDictionary(x => x.Stat)
        );

        /// <summary>
        /// Gets the mapped property name for the specified stat
        /// </summary>
        /// <param name="stat">Stat to get property name</param>
        /// <returns>The name of the mapped property</returns>
        public static string GetStatProperyName(Stat stat)
        {
            if (!StatsBases.ContainsKey(stat))
            {
                return null;
            }

            return StatsBases[stat].PropertyName;
        }

        /// <summary>
        /// Gets all available stats grouped and wrapped into <see cref="StatInfo"/>
        /// </summary>
        /// <returns>Collection of <see cref="StatInfo"/></returns>
        public static IEnumerable<StatInfo> GetAllStats()
        {
            // Make a list of StatInfoGroups
            var statInfoGroups = new[]
            {
                new StatInfoGroup { Name = "Most Popular" },
                new StatInfoGroup { Name = "Positional" },
                new StatInfoGroup { Name = "3-Bet" },
                new StatInfoGroup { Name = "4-Bet" },
                new StatInfoGroup { Name = "Preflop" },
                new StatInfoGroup { Name = "Flop" },
                new StatInfoGroup { Name = "Turn" },
                new StatInfoGroup { Name = "River" },
                new StatInfoGroup { Name = "Tournament" },
                new StatInfoGroup { Name = "Continuation Bet" },
                new StatInfoGroup { Name = "Limp" },
                new StatInfoGroup { Name = "Cold call" },
                new StatInfoGroup { Name = "Advanced Stats" },
            };

            // Make a collection of StatInfo
            var statsCollection = new[]
            {
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.PlayerInfoIcon },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.VPIP },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.PFR },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S3Bet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AF },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AGG },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.BB100 },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CBet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WTSD },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WSSD },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WWSF },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TotalHands },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToCBet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo3Bet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S4Bet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo4Bet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo5Bet },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FlopAGG },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TurnAGG  },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.RiverAGG },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.RecentAgg },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.ColdCall},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Steal },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToSteal },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Squeeze },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CheckRaise },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.BetWhenCheckedTo },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.NetWon, GraphToolIconSource = "/DriveHUD.Common.Resources;Component/images/hud/dollar_sign.png", DigitsAfterDecimalPoint = 2 },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetIP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetOOP },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_EP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_MP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_CO },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_BN },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_SB },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_BB },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetEP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetMP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetCO },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBTN },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetSB },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBB },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpEp },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpMp },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpCo },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpBtn },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpSb },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInEP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInMP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInCO },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBTN },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInSB },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBB },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.BTNDefendCORaise },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_EP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_MP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_CO },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_BN },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_SB },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_BB },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_EP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_MP },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_CO },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_BN },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_SB },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetIP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetOOP },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetVsSteal  },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.CBetInThreeBetPot  },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldToCBetFromThreeBetPot  },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_EP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_MP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_CO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_BN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_SB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_BB },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetMPvsEP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetCOvsEP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetCOvsMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBTNvsEP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBTNvsMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBTNvsCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetSBvsEP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetSBvsMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetSBvsCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetSBvsBTN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBBvsEP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBBvsMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBBvsCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBBvsBTN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetBBvsSB },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall3BetInBB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall3BetInSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall3BetInMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall3BetInCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall3BetInBTN },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall4BetInBB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall4BetInSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall4BetInMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall4BetInCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ColdCall4BetInBTN },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInEPvs3BetMP },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInEPvs3BetCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInEPvs3BetBTN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInEPvs3BetSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInEPvs3BetBB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInMPvs3BetCO },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInMPvs3BetBTN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInMPvs3BetSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInMPvs3BetBB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInCOvs3BetBTN },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInCOvs3BetSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInCOvs3BetBB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInBTNvs3BetSB },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldTo3BetInBTNvs3BetBB },

                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetEP },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetMP },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetCO },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBTN },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetSB },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBB },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.CBetInFourBetPot },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.FoldToCBetFromFourBetPot },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.FoldTo4Bet },

                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3Bet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ColdCallThreeBet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ColdCallFourBet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_EP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_MP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_CO },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_BN },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_SB },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_BB },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3BetIP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3BetOOP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4Bet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetMP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetCO },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetBTN },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetSB },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetBB },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldToSqueez },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldTo3Bet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldTo3BetIP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldTo3BetOOP },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldTo4Bet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldTo5Bet },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldBBvsSBSteal },

                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.WWSF },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopCheckRaise },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopCheckBehind },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CBetThenFoldFlopSRP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FloatFlop },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FloatFlopThenBetTurn, },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopAGG },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.RaiseFlop },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.RaiseFlopCBetIn3BetPot },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CallFlopCBetIP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CallFlopCBetOOP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CalledCheckRaiseVsFlopCBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckFoldFlopPfrOop },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckFoldFlop3BetOop },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckRaisedFlopCBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckRaiseFlopAsPFR },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.BetFoldFlopPfrRaiser },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.BetFlopCalled3BetPreflopIp },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.BetFlopWhenCheckedToSRP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeOneHalfOrLess },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeOneQuarterOrLess },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeTwoThirdsOrLess },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeThreeQuartersOrLess },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeOneOrLess },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopBetSizeMoreThanOne },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldFlop },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToFlopCheckRaise },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToCheckRaiseVsFlopCBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToDonkBet },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToFlopRaise },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToFlopCBetIP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToFlopCBetOOP },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.ShovedFlopAfter4Bet },

                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.BetTurnWhenCheckedToSRP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CalledTurnCheckRaise },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndXCOnTurnOOP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndXFOnTurnOOP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndCallOnTurn },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndFoldOnTurn },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndRaiseOnTurn },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetIPSRP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetOOPSRP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DelayedTurnCBet },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DelayedTurnCBetIP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DelayedTurnCBetOOP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DelayedTurnCBetIn3BetPot },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DoubleBarrel },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DoubleBarrelSRP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DoubleBarrel3BetPot },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToDoubleBarrel },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToDoubleBarrelIn3BetPot },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToTurnRaise },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToTurnCheckRaise },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldTurn },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToTurnProbeIP },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnCheckRaise },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnAF },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnAGG },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBet },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeOneHalfOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeOneQuarterOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeOneThirdOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeTwoThirdsOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeThreeQuartersOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeOneOrLess },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnBetSizeMoreThanOne },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnSeen },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.ProbeBetTurn },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.RaiseTurn },

                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.BetRiverOnBXLine },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.BetRiverWhenCheckedToSRP },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CallRiverRaise },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckRiverOnBXLine },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckRiverAfterBBLine },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetIPSRP },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetOOPSRP },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.FoldToRiverCBet },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.FoldToRiverCheckRaise },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.FoldToRiverRaise },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.FoldToRiverProbeIP },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.ProbeBetRiver },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RaiseRiver },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverAGG },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverBet },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverBetSizeMoreThanOne },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverCallEffeciency },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverCheckCall },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverCheckFold },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverSeen },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.TripleBarrel },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.TripleBarrelSRP },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.TripleBarrel3BetPot },

                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.MRatio },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.BBs },

                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBet },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBet },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetIP },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetOOP },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetInThreeBetPot },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetInFourBetPot },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetVsOneOpp },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetVsTwoOpp },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetMW },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetMonotone },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetRag },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBetFromThreeBetPot },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBetFromFourBetPot },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.RaiseCBet },

                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.Limp },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpCall },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpFold },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpReraise },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpers },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpersInMP },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpersInCO },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpersInBN },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpersInSB },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.RaiseLimpersInBB },

                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseBTN },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseCO },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseSB },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_EP },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_MP },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_CO },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_BN },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_SB },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_BB },

                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.RaiseFrequencyFactor },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.TrueAggression },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DonkBet },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DelayedTurnCBet },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DelayedTurnCBetIP },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DelayedTurnCBetOOP },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.S5Bet },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.WTSDAfterCalling3Bet },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.WTSDAfterCallingPfr },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.WTSDAfterNotCBettingFlopAsPfr },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.WTSDAfterSeeingTurn },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.WTSDAsPF3Bettor }
            };

            return statsCollection;
        }

        /// <summary>
        /// Gets <see cref="StatInfo"/> for the specified <see cref="Stat"/>
        /// </summary>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static StatInfo GetStat(Stat stat)
        {
            return GetAllStats().FirstOrDefault(x => x.Stat == stat);
        }

        /// <summary>
        /// Updates <see cref="StatInfo.Format"/> and <see cref="StatInfo.GraphToolIconSource"/> properties with predefined values
        /// </summary>
        /// <param name="stats">Stats to update</param>
        public static void UpdateStats(IEnumerable<StatInfo> stats)
        {
            if (stats == null)
            {
                return;
            }

            var statsUpdateMap = (from stat in stats
                                  join allStat in GetAllStats() on stat.Stat equals allStat.Stat
                                  select new { StatToUpdate = stat, OriginalStat = allStat }).ToArray();

            statsUpdateMap.ForEach(x =>
            {
                x.StatToUpdate.GraphToolIconSource = x.OriginalStat.GraphToolIconSource;
            });
        }

        /// <summary>
        /// Gets the array of <see cref="Stat"/> which heat map supports
        /// </summary>
        /// <returns>The array of stats which heat map supports</returns>
        public static IEnumerable<StatBase> GetHeatMapStats()
        {
            var statInfoComparer = new LambdaComparer<StatBase>((x, y) => x.Stat == y.Stat);

            var heatMapSupportedStats = StatsBases.Values
                .ToArray()
                .Except(HeapMapNotSupportedStats.Select(x => new StatBase { Stat = x }), statInfoComparer)
                .ToArray();

            return heatMapSupportedStats;
        }
    }
}