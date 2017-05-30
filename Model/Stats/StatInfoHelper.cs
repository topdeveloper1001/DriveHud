//-----------------------------------------------------------------------
// <copyright file="StatInfoHelper.cs" company="Ace Poker Solutions">
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
using Model.Data;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Stats
{
    /// <summary>
    /// Defines methods to create stats collections
    /// </summary>
    public class StatInfoHelper
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
            Stat.BBs
        };

        /// <summary>
        /// Gets all available stats wrapped into <see cref="StatInfo"/>
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
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.VPIP, PropertyName = nameof(Indicators.VPIP) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.PFR, PropertyName = nameof(Indicators.PFR), GetStatDtoExpression = x => new StatDto(x.Pfrhands, x.Totalhands) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S3Bet, PropertyName = nameof(Indicators.ThreeBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AF, PropertyName = nameof(Indicators.Agg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.AGG, PropertyName = nameof(Indicators.AggPr) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CBet, PropertyName = nameof(Indicators.FlopCBet)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WTSD, PropertyName = nameof(Indicators.WTSD) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WSSD, PropertyName = nameof(Indicators.WSSD) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.WWSF, PropertyName = nameof(Indicators.WSWSF) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TotalHands, PropertyName = nameof(Indicators.TotalHands) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToCBet, PropertyName = nameof(Indicators.FoldCBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo3Bet, PropertyName = nameof(Indicators.FoldToThreeBet)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.S4Bet, PropertyName = nameof(Indicators.FourBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldTo4Bet, PropertyName = nameof(Indicators.FoldToFourBet) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FlopAGG, PropertyName = nameof(Indicators.FlopAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.TurnAGG, PropertyName = nameof(Indicators.TurnAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.RiverAGG, PropertyName = nameof(Indicators.RiverAgg) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.RecentAgg, PropertyName = nameof(HudLightIndicators.RecentAggPr) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.ColdCall, PropertyName = nameof(Indicators.ColdCall)},
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Steal, PropertyName = nameof(Indicators.Steal) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.FoldToSteal, PropertyName = nameof(Indicators.BlindsFoldSteal) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.Squeeze, PropertyName = nameof(Indicators.Squeeze) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.CheckRaise, PropertyName = nameof(Indicators.CheckRaise) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.BetWhenCheckedTo, PropertyName = nameof(Indicators.BetWhenCheckedTo) },
                new StatInfo { GroupName = "1", StatInfoGroup = statInfoGroups[0], Stat = Stat.NetWon, PropertyName = nameof(Indicators.NetWon), GraphToolIconSource = "/DriveHUD.Common.Resources;Component/images/hud/dollar_sign.png", Format = "{0:0.00}" },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetIP, PropertyName = nameof(Indicators.CBetIP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.CBetOOP, PropertyName = nameof(Indicators.CBetOOP) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_EP, PropertyName = nameof(HudLightIndicators.ThreeBet_EP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_MP, PropertyName = nameof(HudLightIndicators.ThreeBet_MP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_CO, PropertyName = nameof(HudLightIndicators.ThreeBet_CO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_BN, PropertyName = nameof(HudLightIndicators.ThreeBet_BN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_SB, PropertyName = nameof(HudLightIndicators.ThreeBet_SB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.ThreeBet_BB, PropertyName = nameof(HudLightIndicators.ThreeBet_BB) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetEP, PropertyName = nameof(Indicators.FourBetInEP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetMP, PropertyName = nameof(Indicators.FourBetInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetCO, PropertyName = nameof(Indicators.FourBetInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBTN, PropertyName = nameof(Indicators.FourBetInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetSB, PropertyName = nameof(Indicators.FourBetInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.S4BetBB, PropertyName = nameof(Indicators.FourBetInBB) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpEp, PropertyName = nameof(Indicators.LimpEp) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpMp, PropertyName = nameof(Indicators.LimpMp) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpCo, PropertyName = nameof(Indicators.LimpCo) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpBtn, PropertyName = nameof(Indicators.LimpBtn) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.LimpSb, PropertyName = nameof(Indicators.LimpSb) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInEP, PropertyName = nameof(Indicators.PFRInEP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInMP, PropertyName = nameof(Indicators.PFRInMP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInCO, PropertyName = nameof(Indicators.PFRInCO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBTN, PropertyName = nameof(Indicators.PFRInBTN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInSB, PropertyName = nameof(Indicators.PFRInSB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.PFRInBB, PropertyName = nameof(Indicators.PFRInBB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.BTNDefendCORaise, PropertyName = nameof(Indicators.BTNDefendCORaise) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_EP, PropertyName = nameof(HudLightIndicators.VPIP_EP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_MP, PropertyName = nameof(HudLightIndicators.VPIP_MP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_CO, PropertyName = nameof(HudLightIndicators.VPIP_CO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_BN, PropertyName = nameof(HudLightIndicators.VPIP_BN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_SB, PropertyName = nameof(HudLightIndicators.VPIP_SB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.VPIP_BB, PropertyName = nameof(HudLightIndicators.VPIP_BB) },

                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_EP, PropertyName = nameof(HudLightIndicators.UO_PFR_EP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_MP, PropertyName = nameof(HudLightIndicators.UO_PFR_MP) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_CO, PropertyName = nameof(HudLightIndicators.UO_PFR_CO) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_BN, PropertyName = nameof(HudLightIndicators.UO_PFR_BN) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_SB, PropertyName = nameof(HudLightIndicators.UO_PFR_SB) },
                new StatInfo { GroupName = "2", StatInfoGroup = statInfoGroups[1], Stat = Stat.UO_PFR_BB, PropertyName = nameof(HudLightIndicators.UO_PFR_BB) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetIP, PropertyName = nameof(Indicators.ThreeBetIP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.S3BetOOP, PropertyName = nameof(Indicators.ThreeBetOOP) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBetVsSteal, PropertyName = nameof(Indicators.ThreeBetVsSteal) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.CBetInThreeBetPot, PropertyName = nameof(Indicators.FlopCBetInThreeBetPot) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.FoldToCBetFromThreeBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromThreeBetPot) },

                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_EP, PropertyName = nameof(HudLightIndicators.ThreeBet_EP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_MP, PropertyName = nameof(HudLightIndicators.ThreeBet_MP) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_CO, PropertyName = nameof(HudLightIndicators.ThreeBet_CO) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_BN, PropertyName = nameof(HudLightIndicators.ThreeBet_BN) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_SB, PropertyName = nameof(HudLightIndicators.ThreeBet_SB) },
                new StatInfo { GroupName = "3", StatInfoGroup = statInfoGroups[2], Stat = Stat.ThreeBet_BB, PropertyName = nameof(HudLightIndicators.ThreeBet_BB) },

                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetMP, PropertyName = nameof(Indicators.FourBetInMP) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetCO, PropertyName = nameof(Indicators.FourBetInCO) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBTN, PropertyName = nameof(Indicators.FourBetInBTN) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetSB, PropertyName = nameof(Indicators.FourBetInSB) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.S4BetBB, PropertyName = nameof(Indicators.FourBetInBB) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.CBetInFourBetPot, PropertyName = nameof(Indicators.FlopCBetInFourBetPot) },
                new StatInfo { GroupName = "4", StatInfoGroup = statInfoGroups[3], Stat = Stat.FoldToCBetFromFourBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromFourBetPot) },

                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3Bet, PropertyName = nameof(Indicators.ThreeBet) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ColdCallThreeBet, PropertyName = nameof(Indicators.ColdCallThreeBet) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ColdCallFourBet, PropertyName = nameof(Indicators.ColdCallFourBet) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_EP, PropertyName = nameof(HudLightIndicators.ThreeBet_EP) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_MP, PropertyName = nameof(HudLightIndicators.ThreeBet_MP) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_CO, PropertyName = nameof(HudLightIndicators.ThreeBet_CO) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_BN, PropertyName = nameof(HudLightIndicators.ThreeBet_BN) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_SB, PropertyName = nameof(HudLightIndicators.ThreeBet_SB) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.ThreeBet_BB, PropertyName = nameof(HudLightIndicators.ThreeBet_BB) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3BetIP, PropertyName = nameof(Indicators.ThreeBetIP) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S3BetOOP, PropertyName = nameof(Indicators.ThreeBetOOP) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4Bet, PropertyName = nameof(Indicators.FourBet) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetMP, PropertyName = nameof(Indicators.FourBetInMP) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetCO, PropertyName = nameof(Indicators.FourBetInCO) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetBTN, PropertyName = nameof(Indicators.FourBetInBTN) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetSB, PropertyName = nameof(Indicators.FourBetInSB) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.S4BetBB, PropertyName = nameof(Indicators.FourBetInBB) },
                new StatInfo { GroupName = "5", StatInfoGroup = statInfoGroups[4], Stat = Stat.FoldToSqueez, PropertyName = nameof(Indicators.FoldToSqueez) },

                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.WWSF, PropertyName = nameof(Indicators.WSWSF) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopCheckRaise, PropertyName = nameof(Indicators.FlopCheckRaise) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CBet, PropertyName = nameof(Indicators.FlopCBet)},
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FloatFlop, PropertyName = nameof(Indicators.FloatFlop)},
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FlopAGG, PropertyName = nameof(Indicators.FlopAgg) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.RaiseFlop, PropertyName = nameof(Indicators.RaiseFlop) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckFoldFlopPfrOop, PropertyName = nameof(Indicators.CheckFoldFlopPfrOop) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.CheckFoldFlop3BetOop, PropertyName = nameof(Indicators.CheckFoldFlop3BetOop) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.BetFoldFlopPfrRaiser, PropertyName = nameof(Indicators.BetFoldFlopPfrRaiser) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.BetFlopCalled3BetPreflopIp, PropertyName = nameof(Indicators.BetFlopCalled3BetPreflopIp) },
                new StatInfo { GroupName = "6", StatInfoGroup = statInfoGroups[5], Stat = Stat.FoldToFlopRaise, PropertyName = nameof(Indicators.FoldToFlopRaise) },

                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DelayedTurnCBet, PropertyName = nameof(Indicators.DidDelayedTurnCBet) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnCheckRaise, PropertyName = nameof(Indicators.TurnCheckRaise) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.RaiseTurn, PropertyName = nameof(Indicators.RaiseTurn) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnAGG, PropertyName = nameof(Indicators.TurnAgg) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.TurnSeen, PropertyName = nameof(Indicators.TurnSeen) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.DoubleBarrel, PropertyName = nameof(Indicators.TurnCBet) },
                new StatInfo { GroupName = "7", StatInfoGroup = statInfoGroups[6], Stat = Stat.FoldToTurnRaise, PropertyName = nameof(Indicators.FoldToTurnRaise) },

                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RaiseRiver, PropertyName = nameof(Indicators.RaiseRiver) },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverAGG, PropertyName = nameof(Indicators.RiverAgg) },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.RiverSeen, PropertyName = nameof(Indicators.RiverSeen) },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.CheckRiverOnBXLine, PropertyName = nameof(Indicators.CheckRiverOnBXLine) },
                new StatInfo { GroupName = "8", StatInfoGroup = statInfoGroups[7], Stat = Stat.FoldToRiverCBet, PropertyName = nameof(Indicators.FoldToRiverCBet) },

                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.MRatio, PropertyName = nameof(Indicators.MRatio) },
                new StatInfo { GroupName = "9", StatInfoGroup = statInfoGroups[8], Stat = Stat.BBs, PropertyName = nameof(Indicators.StackInBBs) },

                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBet, PropertyName = nameof(Indicators.FlopCBet) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBet, PropertyName = nameof(Indicators.FoldCBet) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetIP, PropertyName = nameof(Indicators.CBetIP) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetOOP, PropertyName = nameof(Indicators.CBetOOP) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetInThreeBetPot, PropertyName = nameof(Indicators.FlopCBetInThreeBetPot) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.CBetInFourBetPot, PropertyName = nameof(Indicators.FlopCBetInFourBetPot) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetVsOneOpp, PropertyName = nameof(Indicators.FlopCBetVsOneOpp) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetVsTwoOpp, PropertyName = nameof(Indicators.FlopCBetVsTwoOpp) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetMW, PropertyName = nameof(Indicators.FlopCBetMW) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetMonotone, PropertyName = nameof(Indicators.FlopCBetMonotone) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FlopCBetRag, PropertyName = nameof(Indicators.FlopCBetRag) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBetFromThreeBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromThreeBetPot) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.FoldToCBetFromFourBetPot, PropertyName = nameof(Indicators.FoldFlopCBetFromFourBetPot) },
                new StatInfo { GroupName = "91", StatInfoGroup = statInfoGroups[9], Stat = Stat.RaiseCBet, PropertyName = nameof(Indicators.RaiseCBet) },

                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.Limp, PropertyName = nameof(Indicators.DidLimp) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpCall, PropertyName = nameof(Indicators.DidLimpCall) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpFold, PropertyName = nameof(Indicators.DidLimpFold) },
                new StatInfo { GroupName = "92", StatInfoGroup = statInfoGroups[10], Stat = Stat.LimpReraise, PropertyName = nameof(Indicators.DidLimpReraise) },

                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseBTN, PropertyName = nameof(Indicators.ColdCallVsBtnOpen) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseCO, PropertyName = nameof(Indicators.ColdCallVsCoOpen) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCallVsOpenRaiseSB, PropertyName = nameof(Indicators.ColdCallVsSbOpen) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_EP, PropertyName = nameof(HudLightIndicators.ColdCall_EP) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_MP, PropertyName = nameof(HudLightIndicators.ColdCall_MP) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_CO, PropertyName = nameof(HudLightIndicators.ColdCall_CO) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_BN, PropertyName = nameof(HudLightIndicators.ColdCall_BN) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_SB, PropertyName = nameof(HudLightIndicators.ColdCall_SB) },
                new StatInfo { GroupName = "93", StatInfoGroup = statInfoGroups[11], Stat = Stat.ColdCall_BB, PropertyName = nameof(HudLightIndicators.ColdCall_BB) },

                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.RaiseFrequencyFactor, PropertyName = nameof(Indicators.RaiseFrequencyFactor) },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.TrueAggression, PropertyName = nameof(Indicators.TrueAggression) },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DonkBet, PropertyName = nameof(Indicators.DonkBet) },
                new StatInfo { GroupName = "94", StatInfoGroup = statInfoGroups[12], Stat = Stat.DelayedTurnCBet, PropertyName = nameof(Indicators.DidDelayedTurnCBet) }
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
                x.StatToUpdate.Format = x.OriginalStat.Format;
                x.StatToUpdate.GraphToolIconSource = x.OriginalStat.GraphToolIconSource;
            });
        }

        /// <summary>
        /// Gets the array of <see cref="Stat"/> which heat map supports
        /// </summary>
        /// <returns>The array of stats which heat map supports</returns>
        public static IEnumerable<StatInfoBase> GetHeatMapStats()
        {
            var statInfoComparer = new LambdaComparer<StatInfoBase>((x, y) => x.Stat == y.Stat);

            var allStats = GetAllStats()
                .Select(x => new StatInfoBase { Stat = x.Stat, GetStatDtoExpression = x.GetStatDtoExpression })
                .Distinct(statInfoComparer)
                .ToArray();

            var heatMapSupportedStats = allStats
                .Except(HeapMapNotSupportedStats.Select(x => new StatInfoBase { Stat = x }), statInfoComparer)
                .ToArray();

            return heatMapSupportedStats;
        }
    }
}