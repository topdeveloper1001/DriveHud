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
        FoldTurn,
        RiverCheckCall,
        RiverCheckFold,
        RiverBetSizeMoreThanOne,
        RiverCallEffeciency,
        FoldTo5Bet,
        TurnAF,
        ShovedFlopAfter4Bet,
        RaiseFlopCBetIn3BetPot,
        FoldTo3BetIP,
        FoldTo3BetOOP,
        BetFlopWhenCheckedToSRP,
        BB100,
        TripleBarrel,
        TurnBet,
        FlopBet,
        FoldFlop,
        RaiseLimpers,
        RaiseLimpersInMP,
        RaiseLimpersInCO,
        RaiseLimpersInBN,
        RaiseLimpersInSB,
        RaiseLimpersInBB,
        ThreeBetMPvsEP,
        ThreeBetCOvsEP,
        ThreeBetCOvsMP,
        ThreeBetBTNvsEP,
        ThreeBetBTNvsMP,
        ThreeBetBTNvsCO,
        ThreeBetSBvsEP,
        ThreeBetSBvsMP,
        ThreeBetSBvsCO,
        ThreeBetSBvsBTN,
        ThreeBetBBvsEP,
        ThreeBetBBvsMP,
        ThreeBetBBvsCO,
        ThreeBetBBvsBTN,
        ThreeBetBBvsSB,
        FoldTo3BetInEPvs3BetMP,
        FoldTo3BetInEPvs3BetCO,
        FoldTo3BetInEPvs3BetBTN,
        FoldTo3BetInEPvs3BetSB,
        FoldTo3BetInEPvs3BetBB,
        FoldTo3BetInMPvs3BetCO,
        FoldTo3BetInMPvs3BetBTN,
        FoldTo3BetInMPvs3BetSB,
        FoldTo3BetInMPvs3BetBB,
        FoldTo3BetInCOvs3BetBTN,
        FoldTo3BetInCOvs3BetSB,
        FoldTo3BetInCOvs3BetBB,
        FoldTo3BetInBTNvs3BetSB,
        FoldTo3BetInBTNvs3BetBB,
        FoldToRiverRaise,
        CheckRaiseFlopAsPFR,
        ProbeBetTurn,
        ProbeBetRiver,
        FloatFlopThenBetTurn,
        FoldBBvsSBSteal,
        BetTurnWhenCheckedToSRP,
        BetRiverWhenCheckedToSRP,
        BetFlopWhenCheckedToIn3BetPot,
        BetTurnWhenCheckedToIn3BetPot,
        BetRiverWhenCheckedToIn3BetPot,
        ColdCall3BetInBB,
        ColdCall3BetInSB,
        ColdCall3BetInMP,
        ColdCall3BetInCO,
        ColdCall3BetInBTN,
        ColdCall4BetInBB,
        ColdCall4BetInSB,
        ColdCall4BetInMP,
        ColdCall4BetInCO,
        ColdCall4BetInBTN,
        DoubleBarrelSRP,
        DoubleBarrel3BetPot,
        TripleBarrelSRP,
        TripleBarrel3BetPot,
        CBetThenFoldFlopSRP,
        FoldToTurnProbeIP,
        FoldToRiverProbeIP,
        CheckFlopAsPFRAndFoldToTurnBetIPSRP,
        CheckFlopAsPFRAndFoldToTurnBetOOPSRP,
        CheckFlopAsPFRAndFoldToRiverBetIPSRP,
        CheckFlopAsPFRAndFoldToRiverBetOOPSRP,
        CheckFlopAsPFRAndFoldToTurnBetIP3BetPot,
        CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot,
        CheckFlopAsPFRAndFoldToRiverBetIP3BetPot,
        CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot,
        FoldToTripleBarrelSRP,
        FoldToTripleBarrel3BetPot,
        FoldToTripleBarrel4BetPot,
        FoldToDoubleBarrelSRP,
        FoldToDoubleBarrel4BetPot,
        FoldToCBetSRP,
        SBOpenShove1to8bbUOPot,
        SBOpenShove9to14bbUOPot,
        SBOpenShove15to25bbUOPot,
        SBOpenShove26to50bbUOPot,
        SBOpenShove51plusbbUOPot,
        BTNOpenShove1to8bbUOPot,
        BTNOpenShove9to14bbUOPot,
        BTNOpenShove15to25bbUOPot,
        BTNOpenShove26to50bbUOPot,
        BTNOpenShove51plusbbUOPot,
        COOpenShove1to8bbUOPot,
        COOpenShove9to14bbUOPot,
        COOpenShove15to25bbUOPot,
        COOpenShove26to50bbUOPot,
        COOpenShove51plusbbUOPot,
        MPOpenShove1to8bbUOPot,
        MPOpenShove9to14bbUOPot,
        MPOpenShove15to25bbUOPot,
        MPOpenShove26to50bbUOPot,
        MPOpenShove51plusbbUOPot,
        EPOpenShove1to8bbUOPot,
        EPOpenShove9to14bbUOPot,
        EPOpenShove15to25bbUOPot,
        EPOpenShove26to50bbUOPot,
        EPOpenShove51plusbbUOPot,
        LimpEPFoldToPFR,
        LimpMPFoldToPFR,
        LimpCOFoldToPFR,
        LimpBTNFoldToPFR,
        LimpSBFoldToPFR,
        SBShoveOverLimpers1to8bb,
        SBShoveOverLimpers9to14bb,
        SBShoveOverLimpers15to25bb,
        SBShoveOverLimpers26to50bb,
        SBShoveOverLimpers51plusbb,
        BTNShoveOverLimpers1to8bb,
        BTNShoveOverLimpers9to14bb,
        BTNShoveOverLimpers15to25bb,
        BTNShoveOverLimpers26to50bb,
        BTNShoveOverLimpers51plusbb,
        COShoveOverLimpers1to8bb,
        COShoveOverLimpers9to14bb,
        COShoveOverLimpers15to25bb,
        COShoveOverLimpers26to50bb,
        COShoveOverLimpers51plusbb,
        MPShoveOverLimpers1to8bb,
        MPShoveOverLimpers9to14bb,
        MPShoveOverLimpers15to25bb,
        MPShoveOverLimpers26to50bb,
        MPShoveOverLimpers51plusbb,
        EPShoveOverLimpers1to8bb,
        EPShoveOverLimpers9to14bb,
        EPShoveOverLimpers15to25bb,
        EPShoveOverLimpers26to50bb,
        EPShoveOverLimpers51plusbb,
        OpenMinraise,
        EPOpenMinraiseUOPFR,
        MPOpenMinraiseUOPFR,
        COOpenMinraiseUOPFR,
        BTNOpenMinraiseUOPFR,
        SBOpenMinraiseUOPFR,
        BBOpenMinraiseUOPFR,
        SqueezeEP,
        SqueezeMP,
        SqueezeCO,
        SqueezeBTN,
        SqueezeSB,
        SqueezeBB,
        SqueezeBBVsBTNPFR,
        SqueezeBBVsCOPFR,
        SqueezeBBVsMPPFR,
        SqueezeBBVsEPPFR,
        SqueezeSBVsCOPFR,
        SqueezeSBVsMPPFR,
        SqueezeSBVsEPPFR,
        SqueezeBTNVsMPPFR,
        SqueezeBTNVsEPPFR,
        SqueezeCOVsMPPFR,
        SqueezeCOVsEPPFR,
        SqueezeMPVsEPPFR,
        SqueezeEPVsEPPFR,
        FoldToSqueezeAsColdCaller,
        FourBetVsBlind3Bet,
        BTNReStealVsCOSteal,
        BTNDefendVsCOSteal,
        FoldToStealInSB,
        FoldToStealInBB,
        CalledStealInSB,
        CalledStealInBB,
        FoldToBTNStealInSB,
        FoldToBTNStealInBB,
        FoldToCOStealInSB,
        FoldToCOStealInBB,
        CalledBTNStealInSB,
        CalledBTNStealInBB,
        CalledCOStealInSB,
        CalledCOStealInBB,
        OvercallBTNStealInBB,
        WTSDAsPFR,
        WTSDAs4Bettor,
        Call4BetIP,
        Call4BetOOP,
        Call4BetEP,
        Call4BetMP,
        Call4BetCO,
        Call4BetBTN,
        Call4BetSB,
        Call4BetBB,
        TotalOverCallSRP,
        LimpedPotFlopStealIP,
        FlopCheckCall,
        CallFlopFoldTurn,
        RiverFoldInSRP,
        RiverFoldIn3Bet,
        RiverFoldIn4Bet,
        DelayedTurnCBetInSRP,
        DelayedTurnCBetIn4BetPot,
        CheckRaiseFlopAsPFRInSRP,
        CheckRaiseFlopAsPFRIn3BetPot,
        OpenLimpEP,
        OpenLimpMP,
        OpenLimpCO,
        OpenLimpBTN,
        OpenLimpSB,
        CheckInStraddle,
        PFRInStraddle,
        ThreeBetInStraddle,
        FourBetInStraddle,
        FoldInStraddle,
        WTSDInStraddle,
        SkipFlopCBetInSRPandCheckFoldFlopOOP,
        FoldedToDelayedCBet,
        TurnCBet,
        RiverCBet
    }
}