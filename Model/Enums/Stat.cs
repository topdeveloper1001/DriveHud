﻿//-----------------------------------------------------------------------
// <copyright file="Stat.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

namespace Model.Enums
{
    /// <summary>
    /// All Stats
    /// </summary>
    public enum Stat
    {
        VPIP,
        PFR,
        S3Bet,
        AGG,
        AF,
        CBet,
        WTSD,
        WSSD,
        WWSF,
        TotalHands,
        FoldToCBet,
        FoldTo3Bet,
        S4Bet,
        FoldTo4Bet,
        FlopAGG,
        FlopCheckRaise,
        TurnAGG,
        TurnCheckRaise,
        RiverAGG,
        RiverCheckRaise,
        ColdCall,
        Steal,
        FoldToSteal,
        Squeeze,
        CheckRaise,
        CBetIP,
        CBetOOP,
        S3BetMP,
        S3BetCO,
        S3BetBTN,
        S3BetSB,
        S3BetBB,
        S3BetEP,
        S4BetEP,
        S4BetMP,
        S4BetCO,
        S4BetBTN,
        S4BetSB,
        S4BetBB,
        ColdCallMP,
        ColdCallEP,
        ColdCallCO,
        ColdCallBTN,
        ColdCallSB,
        ColdCallBB,
        ColdCallThreeBet,
        ColdCallFourBet,
        ColdCallVsOpenRaiseBTN,
        ColdCallVsOpenRaiseCO,
        ColdCallVsOpenRaiseSB,
        S3BetIP,
        S3BetOOP,
        DonkBet,
        DelayedTurnCBet,
        FloatFlop,
        RaiseFlop,
        RaiseTurn,
        RaiseRiver,
        TurnSeen,
        RiverSeen,
        ThreeBetVsSteal,
        RaiseFrequencyFactor,
        CheckRiverOnBXLine,
        TrueAggression,
        DoubleBarrel,
        Limp,
        LimpEp,
        LimpMp,
        LimpCo,
        LimpBtn,
        LimpSb,
        LimpCall,
        LimpFold,
        LimpReraise,
        UO_PFR_EP,
        UO_PFR_MP,
        UO_PFR_CO,
        UO_PFR_BN,
        UO_PFR_SB,
        UO_PFR_BB,
        VPIP_BB,
        VPIP_MP,
        VPIP_SB,
        VPIP_BN,
        VPIP_CO,
        VPIP_EP,
        ColdCall_EP,
        ColdCall_MP,
        ColdCall_CO,
        ColdCall_BN,
        ColdCall_SB,
        ColdCall_BB,
        MRatio,
        BBs,
        ThreeBet_EP,
        ThreeBet_MP,
        ThreeBet_CO,
        ThreeBet_BN,
        ThreeBet_SB,
        ThreeBet_BB,
        RecentAgg,
        RaiseCBet,
        CBetInThreeBetPot,
        CBetInFourBetPot,
        FlopCBetVsOneOpp,
        FlopCBetVsTwoOpp,
        FlopCBetMW,
        FlopCBetMonotone,
        FlopCBetRag,
        FoldToCBetFromThreeBetPot,
        FoldToCBetFromFourBetPot,
        BTNDefendCORaise,
        CheckFoldFlopPfrOop,
        CheckFoldFlop3BetOop,
        BetFoldFlopPfrRaiser,
        BetFlopCalled3BetPreflopIp,
        PFRInEP,
        PFRInMP,
        PFRInCO,
        PFRInBTN,
        PFRInBB,
        PFRInSB,
        PlayerInfoIcon,
        LineBreak,
        FoldToSqueez,
        BetWhenCheckedTo,
        FoldToFlopRaise,
        FoldToTurnRaise,
        FoldToRiverCBet,
        NetWon,
        FoldToDoubleBarrel,
        FoldToFlopCheckRaise,
        FoldToTurnCheckRaise,
        FoldToRiverCheckRaise,
        CalledTurnCheckRaise,
        CheckRiverAfterBBLine,
        BetRiverOnBXLine,
        CallFlopCBetIP,
        CallFlopCBetOOP,
        FoldToFlopCBetIP,
        FoldToFlopCBetOOP,
        CallRiverRaise,
        RiverBet,
        DelayedTurnCBetIP,
        DelayedTurnCBetOOP,
        S5Bet,
        CalledCheckRaiseVsFlopCBet,
        FoldToCheckRaiseVsFlopCBet,
        CheckFlopAsPFRAndXCOnTurnOOP,
        CheckFlopAsPFRAndXFOnTurnOOP,
        CheckFlopAsPFRAndCallOnTurn,
        CheckFlopAsPFRAndFoldOnTurn,
        CheckFlopAsPFRAndRaiseOnTurn,
        CheckRaisedFlopCBet,
        FlopBetSizeOneHalfOrLess,
        FlopBetSizeOneQuarterOrLess,
        FlopBetSizeTwoThirdsOrLess,
        FlopBetSizeThreeQuartersOrLess,
        FlopBetSizeOneOrLess,
        FlopBetSizeMoreThanOne,
        TurnBetSizeOneHalfOrLess,
        TurnBetSizeOneQuarterOrLess,
        TurnBetSizeOneThirdOrLess,
        TurnBetSizeTwoThirdsOrLess,
        TurnBetSizeThreeQuartersOrLess,
        TurnBetSizeOneOrLess,
        TurnBetSizeMoreThanOne,
        WTSDAfterCalling3Bet,
        WTSDAfterCallingPfr,
        WTSDAfterNotCBettingFlopAsPfr,
        WTSDAfterSeeingTurn,
        WTSDAsPF3Bettor,
        DelayedTurnCBetIn3BetPot,
        FoldToDoubleBarrelIn3BetPot,
        FlopCheckBehind,
        FoldToDonkBet,
        FoldTurn
    }
}