//-----------------------------------------------------------------------
// <copyright file="HudLightIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Reflection;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Enums;
using Model.Stats;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    public class HudLightIndicators : LightIndicators
    {
        public HudLightIndicators() : base()
        {
        }

        public HudLightIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        #region Rich stats

        public virtual StatDto VPIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP,
                    Occurred = Source.Vpiphands,
                    CouldOccurred = Source.Totalhands - Source.NumberOfWalks
                };
            }
        }

        public virtual StatDto PFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = PFR,
                    Occurred = Source.Pfrhands,
                    CouldOccurred = Source.Totalhands - Source.NumberOfWalks
                };
            }
        }

        public virtual StatDto ThreeBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet,
                    Occurred = Source.Didthreebet,
                    CouldOccurred = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto ThreeBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetIP,
                    Occurred = Source.DidThreeBetIp,
                    CouldOccurred = Source.CouldThreeBetIp
                };
            }
        }

        public virtual StatDto ThreeBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetOOP,
                    Occurred = Source.DidThreeBetOop,
                    CouldOccurred = Source.CouldThreeBetOop
                };
            }
        }

        public virtual StatDto FourBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBet,
                    Occurred = did4Bet,
                    CouldOccurred = could4Bet
                };
            }
        }

        public virtual StatDto FiveBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FiveBet,
                    Occurred = Source.Did5Bet,
                    CouldOccurred = Source.Could5Bet
                };
            }
        }

        public virtual StatDto FoldToFiveBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFiveBet,
                    Occurred = Source.FoldedTo5Bet,
                    CouldOccurred = Source.Faced5Bet
                };
            }
        }

        public virtual StatDto ThreeBetCallObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetCall,
                    Occurred = Source.Calledthreebetpreflop,
                    CouldOccurred = faced3Bet
                };
            }
        }

        public virtual StatDto FlopCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBet,
                    Occurred = Source.Flopcontinuationbetmade,
                    CouldOccurred = Source.Flopcontinuationbetpossible
                };
            }
        }

        public virtual StatDto TurnCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnCBet,
                    Occurred = Source.Turncontinuationbetmade,
                    CouldOccurred = Source.Turncontinuationbetpossible
                };
            }
        }

        public virtual StatDto FoldToTurnCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTurnCBet,
                    Occurred = Source.Foldedtoturncontinuationbet,
                    CouldOccurred = Source.Facingturncontinuationbet
                };
            }
        }

        public virtual StatDto RiverCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverCBet,
                    Occurred = Source.Rivercontinuationbetmade,
                    CouldOccurred = Source.Rivercontinuationbetpossible
                };
            }
        }

        public virtual StatDto FlopCBetInThreeBetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetInThreeBetPot,
                    Occurred = Source.FlopContinuationBetInThreeBetPotMade,
                    CouldOccurred = Source.FlopContinuationBetInThreeBetPotPossible
                };

            }
        }

        public virtual StatDto FlopCBetInFourBetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetInFourBetPot,
                    Occurred = Source.FlopContinuationBetInFourBetPotMade,
                    CouldOccurred = Source.FlopContinuationBetInFourBetPotPossible
                };
            }
        }

        public virtual StatDto FlopCBetVsOneOppObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetVsOneOpp,
                    Occurred = Source.FlopContinuationBetVsOneOpponentMade,
                    CouldOccurred = Source.FlopContinuationBetVsOneOpponentPossible
                };
            }
        }

        public virtual StatDto FlopCBetVsTwoOppObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetVsTwoOpp,
                    Occurred = Source.FlopContinuationBetVsTwoOpponentsMade,
                    CouldOccurred = Source.FlopContinuationBetVsTwoOpponentsPossible
                };
            }
        }

        public virtual StatDto FlopCBetMWObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetMW,
                    Occurred = Source.MultiWayFlopContinuationBetMade,
                    CouldOccurred = Source.MultiWayFlopContinuationBetPossible
                };
            }
        }

        public virtual StatDto FlopCBetMonotoneObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetMonotone,
                    Occurred = Source.FlopContinuationBetMonotonePotMade,
                    CouldOccurred = Source.FlopContinuationBetMonotonePotPossible
                };
            }
        }

        public virtual StatDto FlopCBetRagObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCBetRag,
                    Occurred = Source.FlopContinuationBetRagPotMade,
                    CouldOccurred = Source.FlopContinuationBetRagPotPossible
                };
            }
        }

        public virtual StatDto FoldFlopCBetFromThreeBetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldFlopCBetFromThreeBetPot,
                    Occurred = Source.FoldedToFlopContinuationBetFromThreeBetPot,
                    CouldOccurred = Source.FacingFlopContinuationBetFromThreeBetPot,
                };
            }
        }

        public virtual StatDto FoldFlopCBetFromFourBetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldFlopCBetFromFourBetPot,
                    Occurred = Source.FoldedToFlopContinuationBetFromFourBetPot,
                    CouldOccurred = Source.FacingFlopContinuationBetFromFourBetPot,
                };
            }
        }

        public virtual StatDto StealObject
        {
            get
            {
                return new StatDto
                {
                    Value = Steal,
                    Occurred = Source.StealMade,
                    CouldOccurred = Source.StealPossible
                };
            }
        }

        public virtual StatDto FoldToThreeBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToThreeBet,
                    Occurred = foldedTo3Bet,
                    CouldOccurred = faced3Bet
                };
            }
        }

        public virtual StatDto BlindsReraiseStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = BlindsReraiseSteal,
                    Occurred = Source.Bigblindstealreraised + Source.Smallblindstealreraised,
                    CouldOccurred = Source.Bigblindstealfaced + Source.Smallblindstealfaced
                };
            }
        }

        public virtual StatDto BlindsFoldStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = BlindsFoldSteal,
                    Occurred = Source.Bigblindstealfolded + Source.Smallblindstealfolded,
                    CouldOccurred = Source.Bigblindstealfaced + Source.Smallblindstealfaced
                };
            }
        }

        public virtual StatDto WSSDObject
        {
            get
            {
                return new StatDto
                {
                    Value = WSSD,
                    Occurred = Source.Wonshowdown,
                    CouldOccurred = Source.Sawshowdown
                };
            }
        }

        public virtual StatDto WWSFObject
        {
            get
            {
                return new StatDto
                {
                    Value = WWSF,
                    Occurred = Source.Wonhandwhensawflop,
                    CouldOccurred = Source.Sawflop
                };
            }
        }

        public virtual StatDto WTSDObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSD,
                    Occurred = Source.Sawshowdown,
                    CouldOccurred = Source.Sawflop
                };
            }
        }

        public virtual StatDto AggPrObject
        {
            get
            {
                return new StatDto
                {
                    Value = AggPr,
                    Occurred = Source.Totalbets,
                    CouldOccurred = Source.Totalpostflopstreetsplayed
                };
            }
        }

        public virtual StatDto TrueAggressionObject
        {
            get
            {
                return new StatDto
                {
                    Value = TrueAggression,
                    Occurred = Source.TotalAggressiveBets,
                    CouldOccurred = Source.Totalpostflopstreetsplayed - Source.Flopcontinuationbetmade
                };
            }
        }

        public virtual StatDto WSWSFObject
        {
            get
            {
                return new StatDto
                {
                    Value = WSWSF,
                    Occurred = Source.Wonhandwhensawflop,
                    CouldOccurred = Source.Sawflop
                };
            }
        }

        public virtual StatDto ColdCallObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall,
                    Occurred = Source.Didcoldcall,
                    CouldOccurred = Source.Couldcoldcall
                };
            }
        }

        public virtual StatDto FlopAggObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopAgg,
                    Occurred = Source.TotalbetsFlop,
                    CouldOccurred = Source.FlopAggPossible
                };
            }
        }

        public virtual StatDto TurnAggObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnAgg,
                    Occurred = Source.TotalbetsTurn,
                    CouldOccurred = Source.TurnAggPossible
                };
            }
        }

        public virtual StatDto RiverAggObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverAgg,
                    Occurred = Source.TotalbetsRiver,
                    CouldOccurred = Source.RiverAggPossible
                };
            }
        }

        public virtual StatDto FoldCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldCBet,
                    Occurred = Source.Foldedtoflopcontinuationbet,
                    CouldOccurred = Source.Facingflopcontinuationbet
                };
            }
        }

        public virtual StatDto RaiseCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseCBet,
                    Occurred = Source.Raisedflopcontinuationbet,
                    CouldOccurred = Source.Facingflopcontinuationbet
                };
            }
        }

        public virtual StatDto FoldToFourBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFourBet,
                    Occurred = Source.Foldedtofourbetpreflop,
                    CouldOccurred = Source.Facedfourbetpreflop
                };
            }
        }

        public virtual StatDto SqueezeObject
        {
            get
            {
                return new StatDto
                {
                    Value = Squeeze,
                    Occurred = Source.Didsqueeze,
                    CouldOccurred = Source.Couldsqueeze
                };
            }
        }

        public virtual StatDto CheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRaise,
                    Occurred = Source.DidCheckRaise,
                    CouldOccurred = Source.CouldFlopCheckRaise + Source.CouldTurnCheckRaise + Source.CouldRiverCheckRaise
                };
            }
        }

        public virtual StatDto FlopCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCheckRaise,
                    Occurred = Source.DidFlopCheckRaise,
                    CouldOccurred = Source.CouldFlopCheckRaise
                };
            }
        }

        public virtual StatDto TurnCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnCheckRaise,
                    Occurred = Source.DidTurnCheckRaise,
                    CouldOccurred = Source.CouldTurnCheckRaise
                };
            }
        }

        public virtual StatDto RiverCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverCheckRaise,
                    Occurred = Source.DidRiverCheckRaise,
                    CouldOccurred = Source.CouldRiverCheckRaise
                };
            }
        }

        public virtual StatDto FloatFlopObject
        {
            get
            {
                return new StatDto
                {
                    Value = FloatFlop,
                    Occurred = Source.PlayedFloatFlop,
                    CouldOccurred = Source.Facingflopcontinuationbet
                };
            }
        }

        public virtual StatDto RaiseFlopObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseFlop,
                    Occurred = Source.DidRaiseFlop,
                    CouldOccurred = Source.CouldRaiseFlop
                };
            }
        }

        public virtual StatDto RaiseTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseTurn,
                    Occurred = Source.DidRaiseTurn,
                    CouldOccurred = Source.CouldRaiseTurn
                };
            }
        }

        public virtual StatDto RaiseRiverObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseRiver,
                    Occurred = Source.DidRaiseRiver,
                    CouldOccurred = Source.CouldRaiseRiver
                };
            }
        }

        public virtual StatDto RaiseFrequencyFactorObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseFrequencyFactor,
                    Occurred = Source.DidRaiseFlop + Source.DidRaiseTurn + Source.DidRaiseRiver,
                    CouldOccurred = Source.CouldRaiseFlop + Source.CouldRaiseTurn + Source.CouldRaiseRiver
                };
            }
        }

        public virtual StatDto TurnSeenObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnSeen,
                    Occurred = Source.SawTurn,
                    CouldOccurred = Source.WasTurn
                };
            }
        }

        public virtual StatDto RiverSeenObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverSeen,
                    Occurred = Source.SawRiver,
                    CouldOccurred = Source.WasRiver
                };
            }
        }

        public virtual StatDto ThreeBetVsStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetVsSteal,
                    Occurred = Source.DidThreeBetVsSteal,
                    CouldOccurred = Source.CouldThreeBetVsSteal
                };
            }
        }

        public virtual StatDto CheckRiverOnBXLineObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRiverOnBXLine,
                    Occurred = Source.DidCheckRiverOnBXLine,
                    CouldOccurred = Source.CouldCheckRiverOnBXLine
                };
            }
        }

        public virtual StatDto CheckRiverAfterBBLineObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRiverAfterBBLine,
                    Occurred = Source.CheckedRiverAfterBBLine,
                    CouldOccurred = Source.CouldCheckRiverAfterBBLine
                };
            }
        }

        public virtual StatDto BetRiverOnBXLineObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetRiverOnBXLine,
                    Occurred = Source.DidBetRiverOnBXLine,
                    CouldOccurred = Source.CouldBetRiverOnBXLine
                };
            }
        }

        public virtual StatDto RiverBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverBet,
                    Occurred = Source.DidRiverBet,
                    CouldOccurred = Source.CouldRiverBet
                };
            }
        }

        public virtual StatDto TurnBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBet,
                    Occurred = didTurnBet,
                    CouldOccurred = Source.CouldTurnBet
                };
            }
        }

        public virtual StatDto FlopBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBet,
                    Occurred = didFlopBet,
                    CouldOccurred = Source.CouldFlopBet
                };
            }
        }

        public virtual StatDto CBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CBetIP,
                    Occurred = Source.Flopcontinuationipbetmade,
                    CouldOccurred = Source.Flopcontinuationipbetpossible
                };
            }
        }

        public virtual StatDto CBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CBetOOP,
                    Occurred = Source.Flopcontinuationoopbetmade,
                    CouldOccurred = Source.Flopcontinuationoopbetpossible
                };
            }
        }

        public virtual StatDto DonkBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = DonkBet,
                    Occurred = Source.DidDonkBet,
                    CouldOccurred = Source.CouldDonkBet
                };
            }
        }

        public virtual StatDto DidDelayedTurnCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidDelayedTurnCBet,
                    Occurred = Source.DidDelayedTurnCBet,
                    CouldOccurred = Source.CouldDelayedTurnCBet
                };
            }
        }

        public virtual StatDto DelayedTurnCBetIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = DelayedTurnCBetIn3BetPot,
                    Occurred = Source.DidDelayedTurnCBetIn3BetPot,
                    CouldOccurred = Source.CouldDelayedTurnCBetIn3BetPot
                };
            }
        }

        public virtual StatDto BTNDefendCORaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNDefendCORaise,
                    Occurred = Source.Buttonstealdefended,
                    CouldOccurred = Source.Buttonstealfaced
                };
            }
        }

        public virtual StatDto BetFlopCalled3BetPreflopIpObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetFlopCalled3BetPreflopIp,
                    Occurred = Source.BetFlopCalled3BetPreflopIp,
                    CouldOccurred = Source.CouldBetFlopCalled3BetPreflopIp
                };
            }
        }

        public virtual StatDto BetFoldFlopPfrRaiserObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetFoldFlopPfrRaiser,
                    Occurred = Source.BetFoldFlopPfrRaiser,
                    CouldOccurred = Source.CouldBetFoldFlopPfrRaiser
                };
            }
        }

        public virtual StatDto CheckFoldFlop3BetOopObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFoldFlop3BetOop,
                    Occurred = Source.CheckFoldFlop3BetOop,
                    CouldOccurred = couldCheckFoldFlop3BetOop
                };
            }
        }

        public virtual StatDto CheckFoldFlopPfrOopObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFoldFlopPfrOop,
                    Occurred = Source.CheckFoldFlopPfrOop,
                    CouldOccurred = couldCheckFoldFlopPfrOop
                };
            }
        }

        public virtual StatDto CallFlopCBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CallFlopCBetIP,
                    Occurred = Source.CalledflopcontinuationbetIP,
                    CouldOccurred = Source.FacingflopcontinuationbetIP
                };
            }
        }

        public virtual StatDto CallFlopCBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CallFlopCBetOOP,
                    Occurred = Source.CalledflopcontinuationbetOOP,
                    CouldOccurred = Source.FacingflopcontinuationbetOOP
                };
            }
        }

        public virtual StatDto FoldToFlopCBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFlopCBetIP,
                    Occurred = Source.FoldToFlopcontinuationbetIP,
                    CouldOccurred = Source.FacingflopcontinuationbetIP
                };
            }
        }

        public virtual StatDto FoldToFlopCBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFlopCBetOOP,
                    Occurred = Source.FoldToFlopcontinuationbetOOP,
                    CouldOccurred = Source.FacingflopcontinuationbetOOP
                };
            }
        }

        public virtual StatDto CallRiverRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = CallRiverRaise,
                    Occurred = Source.CalledFacedRaiseRiver,
                    CouldOccurred = Source.FacedRaiseRiver
                };
            }
        }

        public virtual StatDto DelayedTurnCBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = DelayedTurnCBetIP,
                    Occurred = didDelayedTurnCBetIP,
                    CouldOccurred = couldDelayedTurnCBetIP
                };
            }
        }

        public virtual StatDto DelayedTurnCBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = DelayedTurnCBetOOP,
                    Occurred = didDelayedTurnCBetOOP,
                    CouldOccurred = couldDelayedTurnCBetOOP
                };
            }
        }

        public virtual StatDto CheckRaisedFlopCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRaisedFlopCBet,
                    Occurred = checkRaisedFlopCBet,
                    CouldOccurred = couldCheckRaiseFlopCBet
                };
            }
        }

        public virtual StatDto FoldToTurnCBetIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTurnCBetIn3BetPot,
                    Occurred = foldToTurnCBetIn3BetPot,
                    CouldOccurred = facedToTurnCBetIn3BetPot
                };
            }
        }

        public virtual StatDto FlopCheckBehindObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCheckBehind,
                    Occurred = Source.DidFlopCheckBehind,
                    CouldOccurred = Source.CouldFlopCheckBehind
                };
            }
        }

        public virtual StatDto FoldToDonkBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToDonkBet,
                    Occurred = Source.FoldedToDonkBet,
                    CouldOccurred = Source.FacedDonkBet
                };
            }
        }

        public virtual StatDto FoldTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTurn,
                    Occurred = Source.FoldedTurn,
                    CouldOccurred = Source.FacedBetOnTurn
                };
            }
        }

        public virtual StatDto FoldFlopObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldFlop,
                    Occurred = Source.FoldedFlop,
                    CouldOccurred = Source.FacedBetOnFlop
                };
            }
        }

        public virtual StatDto RiverCheckCallObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverCheckCall,
                    Occurred = Source.CheckedCalledRiver,
                    CouldOccurred = Source.CheckedThenFacedBetOnRiver
                };
            }
        }

        public virtual StatDto RiverCheckFoldObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverCheckFold,
                    Occurred = Source.CheckedFoldedRiver,
                    CouldOccurred = Source.CheckedThenFacedBetOnRiver
                };
            }
        }

        public virtual StatDto ShovedFlopAfter4BetObject
        {
            get
            {
                return new StatDto
                {
                    Value = ShovedFlopAfter4Bet,
                    Occurred = Source.ShovedFlopAfter4Bet,
                    CouldOccurred = Source.CouldShoveFlopAfter4Bet
                };
            }
        }

        public virtual StatDto RaiseFlopCBetIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseFlopCBetIn3BetPot,
                    Occurred = raisedFlopCBetIn3BetPot,
                    CouldOccurred = couldRaiseFlopCBetIn3BetPot
                };
            }
        }

        public virtual StatDto FoldToThreeBetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToThreeBetIP,
                    Occurred = Source.FoldToThreeBetIP,
                    CouldOccurred = Source.FacedThreeBetIP
                };
            }
        }

        public virtual StatDto FoldToThreeBetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToThreeBetOOP,
                    Occurred = Source.FoldToThreeBetOOP,
                    CouldOccurred = Source.FacedThreeBetOOP
                };
            }
        }

        public virtual StatDto BetFlopWhenCheckedToSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetFlopWhenCheckedToSRP,
                    Occurred = Source.BetFlopWhenCheckedToSRP,
                    CouldOccurred = Source.CouldBetFlopWhenCheckedToSRP
                };
            }
        }

        #region FlopBetSize stats

        public virtual StatDto FlopBetSizeOneHalfOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeOneHalfOrLess,
                    Occurred = flopBetSizeOneHalfOrLess,
                    CouldOccurred = didFlopBet
                };
            }
        }

        public virtual StatDto FlopBetSizeOneQuarterOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeOneQuarterOrLess,
                    Occurred = flopBetSizeOneQuarterOrLess,
                    CouldOccurred = didFlopBet
                };
            }
        }

        public virtual StatDto FlopBetSizeTwoThirdsOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeTwoThirdsOrLess,
                    Occurred = flopBetSizeTwoThirdsOrLess,
                    CouldOccurred = didFlopBet
                };
            }
        }

        public virtual StatDto FlopBetSizeThreeQuartersOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeThreeQuartersOrLess,
                    Occurred = flopBetSizeThreeQuartersOrLess,
                    CouldOccurred = didFlopBet
                };
            }
        }

        public virtual StatDto FlopBetSizeOneOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeOneOrLess,
                    Occurred = flopBetSizeOneOrLess,
                    CouldOccurred = didFlopBet
                };
            }
        }

        public virtual StatDto FlopBetSizeMoreThanOneObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopBetSizeMoreThanOne,
                    Occurred = flopBetSizeMoreThanOne,
                    CouldOccurred = didFlopBet
                };
            }
        }

        #endregion

        #region TurnBetSize stats

        public virtual StatDto TurnBetSizeOneHalfOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeOneHalfOrLess,
                    Occurred = turnBetSizeOneHalfOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeOneQuarterOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeOneQuarterOrLess,
                    Occurred = turnBetSizeOneQuarterOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeOneThirdOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeOneThirdOrLess,
                    Occurred = turnBetSizeOneThirdOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeTwoThirdsOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeTwoThirdsOrLess,
                    Occurred = turnBetSizeTwoThirdsOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeThreeQuartersOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeThreeQuartersOrLess,
                    Occurred = turnBetSizeThreeQuartersOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeOneOrLessObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeOneOrLess,
                    Occurred = turnBetSizeOneOrLess,
                    CouldOccurred = didTurnBet
                };
            }
        }

        public virtual StatDto TurnBetSizeMoreThanOneObject
        {
            get
            {
                return new StatDto
                {
                    Value = TurnBetSizeMoreThanOne,
                    Occurred = turnBetSizeMoreThanOne,
                    CouldOccurred = didTurnBet
                };
            }
        }

        #endregion

        #region RiverBetSize stats

        public virtual StatDto RiverBetSizeMoreThanOneObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverBetSizeMoreThanOne,
                    Occurred = riverBetSizeMoreThanOne,
                    CouldOccurred = Source.DidRiverBet
                };
            }
        }

        #endregion

        #region WTSD After stats

        public virtual StatDto WTSDAfterCalling3BetObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAfterCalling3Bet,
                    Occurred = wtsdAfterCalling3Bet,
                    CouldOccurred = wtsdAfterCalling3BetOpportunity
                };
            }
        }

        public virtual StatDto WTSDAfterCallingPfrObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAfterCallingPfr,
                    Occurred = wtsdAfterCallingPfr,
                    CouldOccurred = wtsdAfterCallingPfrOpportunity
                };
            }
        }

        public virtual StatDto WTSDAfterNotCBettingFlopAsPfrObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAfterNotCBettingFlopAsPfr,
                    Occurred = wtsdAfterNotCBettingFlopAsPfr,
                    CouldOccurred = wtsdAfterNotCBettingFlopAsPfrOpportunity
                };
            }
        }

        public virtual StatDto WTSDAfterSeeingTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAfterSeeingTurn,
                    Occurred = wtsdAfterSeeingTurn,
                    CouldOccurred = Source.SawTurn
                };
            }
        }

        public virtual StatDto WTSDAsPF3BettorObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAsPF3Bettor,
                    Occurred = wtsdAsPF3Bettor,
                    CouldOccurred = wtsdAsPF3BettorOpportunity
                };
            }
        }

        #endregion

        #endregion

        #region Limp

        public virtual StatDto DidLimpObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidLimp,
                    Occurred = Source.LimpMade,
                    CouldOccurred = Source.LimpPossible
                };
            }
        }

        public virtual StatDto LimpEpObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpEp,
                    Occurred = positionLimpMade?.EP ?? 0,
                    CouldOccurred = positionLimpPossible?.EP ?? 0,
                };
            }
        }

        public virtual StatDto LimpMpObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpMp,
                    Occurred = positionLimpMade?.MP ?? 0,
                    CouldOccurred = positionLimpPossible?.MP ?? 0,
                };
            }
        }

        public virtual StatDto LimpCoObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpCo,
                    Occurred = positionLimpMade?.CO ?? 0,
                    CouldOccurred = positionLimpPossible?.CO ?? 0,
                };
            }
        }

        public virtual StatDto LimpBtnObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpBtn,
                    Occurred = positionLimpMade?.BN ?? 0,
                    CouldOccurred = positionLimpPossible?.BN ?? 0,
                };
            }
        }

        public virtual StatDto LimpSbObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpSb,
                    Occurred = positionLimpMade?.SB ?? 0,
                    CouldOccurred = positionLimpPossible?.SB ?? 0,
                };
            }
        }

        public virtual StatDto DidLimpCallObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidLimpCall,
                    Occurred = Source.LimpCalled,
                    CouldOccurred = Source.LimpFaced
                };
            }
        }

        public virtual StatDto DidLimpFoldObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidLimpFold,
                    Occurred = Source.LimpFolded,
                    CouldOccurred = Source.LimpFaced
                };
            }
        }

        public virtual StatDto DidLimpReraiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidLimpReraise,
                    Occurred = Source.LimpReraised,
                    CouldOccurred = Source.LimpFaced
                };
            }
        }

        public virtual StatDto RaiseLimpersObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpers,
                    Occurred = raisedLimpers,
                    CouldOccurred = couldRaiseLimpers
                };
            }
        }

        public virtual StatDto RaiseLimpersInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpersInMP,
                    Occurred = positionRaiseLimpers?.MP ?? 0,
                    CouldOccurred = positionCouldRaiseLimpers?.MP ?? 0
                };
            }
        }

        public virtual StatDto RaiseLimpersInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpersInMP,
                    Occurred = positionRaiseLimpers?.CO ?? 0,
                    CouldOccurred = positionCouldRaiseLimpers?.CO ?? 0
                };
            }
        }

        public virtual StatDto RaiseLimpersInBNObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpersInBN,
                    Occurred = positionRaiseLimpers?.BN ?? 0,
                    CouldOccurred = positionCouldRaiseLimpers?.BN ?? 0
                };
            }
        }

        public virtual StatDto RaiseLimpersInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpersInSB,
                    Occurred = positionRaiseLimpers?.SB ?? 0,
                    CouldOccurred = positionCouldRaiseLimpers?.SB ?? 0
                };
            }
        }

        public virtual StatDto RaiseLimpersInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = RaiseLimpersInBB,
                    Occurred = positionRaiseLimpers?.BB ?? 0,
                    CouldOccurred = positionCouldRaiseLimpers?.BB ?? 0
                };
            }
        }

        #endregion

        public virtual StatDto BetWhenCheckedToObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetWhenCheckedTo,
                    Occurred = Source.DidBetWhenCheckedToFlop + Source.DidBetWhenCheckedToTurn + Source.DidBetWhenCheckedToRiver,
                    CouldOccurred = Source.CanBetWhenCheckedToFlop + Source.CanBetWhenCheckedToTurn + Source.CanBetWhenCheckedToRiver,
                };
            }
        }

        public virtual StatDto FoldToFlopCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFlopCheckRaise,
                    Occurred = Source.FoldedToFlopCheckRaise,
                    CouldOccurred = Source.FacedFlopCheckRaise,
                };
            }
        }


        public virtual StatDto FoldToFlopRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToFlopRaise,
                    Occurred = Source.FoldedFacedRaiseFlop,
                    CouldOccurred = Source.FacedRaiseFlop,
                };
            }
        }

        public virtual StatDto FoldToTurnCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTurnCheckRaise,
                    Occurred = Source.FoldedToTurnCheckRaise,
                    CouldOccurred = Source.FacedTurnCheckRaise,
                };
            }
        }

        public virtual StatDto CalledTurnCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledTurnCheckRaise,
                    Occurred = Source.CalledTurnCheckRaise,
                    CouldOccurred = Source.FacedTurnCheckRaise,
                };
            }
        }

        public virtual StatDto FoldToTurnRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTurnRaise,
                    Occurred = Source.FoldedFacedRaiseTurn,
                    CouldOccurred = Source.FacedRaiseTurn,
                };
            }
        }

        public virtual StatDto FoldToRiverCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToRiverCBet,
                    Occurred = Source.Foldedtorivercontinuationbet,
                    CouldOccurred = Source.Facingrivercontinuationbet
                };
            }
        }

        public virtual StatDto FoldToRiverCheckRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToRiverCheckRaise,
                    Occurred = Source.FoldedToRiverCheckRaise,
                    CouldOccurred = Source.FacedRiverCheckRaise,
                };
            }
        }

        public virtual StatDto FoldToRiverRaiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToRiverRaise,
                    Occurred = Source.FoldedFacedRaiseRiver,
                    CouldOccurred = Source.FacedRaiseRiver,
                };
            }
        }

        public virtual StatDto FoldToSqueezObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToSqueez,
                    Occurred = Source.FoldedFacedSqueez,
                    CouldOccurred = Source.FacedSqueez,
                };
            }
        }

        public virtual StatDto CalledCheckRaiseVsFlopCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledCheckRaiseVsFlopCBet,
                    Occurred = Source.CalledCheckRaiseVsFlopCBet,
                    CouldOccurred = Source.FacedCheckRaiseVsFlopCBet,
                };
            }
        }

        public virtual StatDto FoldedCheckRaiseVsFlopCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldedCheckRaiseVsFlopCBet,
                    Occurred = Source.FoldedCheckRaiseVsFlopCBet,
                    CouldOccurred = Source.FacedCheckRaiseVsFlopCBet,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndXCOnTurnOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndXCOnTurnOOP,
                    Occurred = Source.CheckedCalledTurnWhenCheckedFlopAsPfr,
                    CouldOccurred = Source.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndXFOnTurnOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndXFOnTurnOOP,
                    Occurred = Source.CheckedFoldedToTurnWhenCheckedFlopAsPfr,
                    CouldOccurred = Source.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndCallOnTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndCallOnTurn,
                    Occurred = Source.CalledTurnBetWhenCheckedFlopAsPfr,
                    CouldOccurred = Source.FacedTurnBetWhenCheckedFlopAsPfr,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldOnTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldOnTurn,
                    Occurred = Source.FoldedToTurnBetWhenCheckedFlopAsPfr,
                    CouldOccurred = Source.FacedTurnBetWhenCheckedFlopAsPfr,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndRaiseOnTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndRaiseOnTurn,
                    Occurred = Source.RaisedTurnBetWhenCheckedFlopAsPfr,
                    CouldOccurred = Source.FacedTurnBetWhenCheckedFlopAsPfr,
                };
            }
        }

        #region 3-Bet vs Pos stats

        public virtual StatDto ThreeBetMPvsEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetMPvsEP,
                    Occurred = threeBetMPvsEP,
                    CouldOccurred = couldThreeBetMPvsEP,
                };
            }
        }

        public virtual StatDto ThreeBetCOvsEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetCOvsEP,
                    Occurred = threeBetCOvsEP,
                    CouldOccurred = couldThreeBetCOvsEP,
                };
            }
        }

        public virtual StatDto ThreeBetCOvsMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetCOvsMP,
                    Occurred = threeBetCOvsMP,
                    CouldOccurred = couldThreeBetCOvsMP,
                };
            }
        }

        public virtual StatDto ThreeBetBTNvsEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBTNvsEP,
                    Occurred = threeBetBTNvsEP,
                    CouldOccurred = couldThreeBetBTNvsEP,
                };
            }
        }

        public virtual StatDto ThreeBetBTNvsMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBTNvsMP,
                    Occurred = threeBetBTNvsMP,
                    CouldOccurred = couldThreeBetBTNvsMP,
                };
            }
        }

        public virtual StatDto ThreeBetBTNvsCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBTNvsCO,
                    Occurred = threeBetBTNvsCO,
                    CouldOccurred = couldThreeBetBTNvsCO,
                };
            }
        }

        public virtual StatDto ThreeBetSBvsEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetSBvsEP,
                    Occurred = threeBetSBvsEP,
                    CouldOccurred = couldThreeBetSBvsEP,
                };
            }
        }

        public virtual StatDto ThreeBetSBvsMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetSBvsMP,
                    Occurred = threeBetSBvsMP,
                    CouldOccurred = couldThreeBetSBvsMP,
                };
            }
        }

        public virtual StatDto ThreeBetSBvsCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetSBvsCO,
                    Occurred = threeBetSBvsCO,
                    CouldOccurred = couldThreeBetSBvsCO,
                };
            }
        }

        public virtual StatDto ThreeBetSBvsBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetSBvsBTN,
                    Occurred = threeBetSBvsBTN,
                    CouldOccurred = couldThreeBetSBvsBTN,
                };
            }
        }

        public virtual StatDto ThreeBetBBvsEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBBvsEP,
                    Occurred = threeBetBBvsEP,
                    CouldOccurred = couldThreeBetBBvsEP,
                };
            }
        }

        public virtual StatDto ThreeBetBBvsMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBBvsMP,
                    Occurred = threeBetBBvsMP,
                    CouldOccurred = couldThreeBetBBvsMP,
                };
            }
        }

        public virtual StatDto ThreeBetBBvsCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBBvsCO,
                    Occurred = threeBetBBvsCO,
                    CouldOccurred = couldThreeBetBBvsCO,
                };
            }
        }

        public virtual StatDto ThreeBetBBvsBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBBvsBTN,
                    Occurred = threeBetBBvsBTN,
                    CouldOccurred = couldThreeBetBBvsBTN,
                };
            }
        }

        public virtual StatDto ThreeBetBBvsSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetBBvsSB,
                    Occurred = threeBetBBvsSB,
                    CouldOccurred = couldThreeBetBBvsSB,
                };
            }
        }

        #endregion

        #region Fold to 3-Bet in Pos vs 3-bet Pos

        public virtual StatDto FoldTo3BetInEPvs3BetMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInEPvs3BetMP,
                    Occurred = foldTo3BetInEPvs3BetMP,
                    CouldOccurred = couldFoldTo3BetInEPvs3BetMP,
                };
            }
        }

        public virtual StatDto FoldTo3BetInEPvs3BetCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInEPvs3BetCO,
                    Occurred = foldTo3BetInEPvs3BetCO,
                    CouldOccurred = couldFoldTo3BetInEPvs3BetCO,
                };
            }
        }

        public virtual StatDto FoldTo3BetInEPvs3BetBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInEPvs3BetBTN,
                    Occurred = foldTo3BetInEPvs3BetBTN,
                    CouldOccurred = couldFoldTo3BetInEPvs3BetBTN,
                };
            }
        }

        public virtual StatDto FoldTo3BetInEPvs3BetSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInEPvs3BetSB,
                    Occurred = foldTo3BetInEPvs3BetSB,
                    CouldOccurred = couldFoldTo3BetInEPvs3BetSB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInEPvs3BetBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInEPvs3BetBB,
                    Occurred = foldTo3BetInEPvs3BetBB,
                    CouldOccurred = couldFoldTo3BetInEPvs3BetBB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInMPvs3BetCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInMPvs3BetCO,
                    Occurred = foldTo3BetInMPvs3BetCO,
                    CouldOccurred = couldFoldTo3BetInMPvs3BetCO,
                };
            }
        }

        public virtual StatDto FoldTo3BetInMPvs3BetBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInMPvs3BetBTN,
                    Occurred = foldTo3BetInMPvs3BetBTN,
                    CouldOccurred = couldFoldTo3BetInMPvs3BetBTN,
                };
            }
        }

        public virtual StatDto FoldTo3BetInMPvs3BetSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInMPvs3BetSB,
                    Occurred = foldTo3BetInMPvs3BetSB,
                    CouldOccurred = couldFoldTo3BetInMPvs3BetSB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInMPvs3BetBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInMPvs3BetBB,
                    Occurred = foldTo3BetInMPvs3BetBB,
                    CouldOccurred = couldFoldTo3BetInMPvs3BetBB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInCOvs3BetBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInCOvs3BetBTN,
                    Occurred = foldTo3BetInCOvs3BetBTN,
                    CouldOccurred = couldFoldTo3BetInCOvs3BetBTN,
                };
            }
        }

        public virtual StatDto FoldTo3BetInCOvs3BetSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInCOvs3BetSB,
                    Occurred = foldTo3BetInCOvs3BetSB,
                    CouldOccurred = couldFoldTo3BetInCOvs3BetSB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInCOvs3BetBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInCOvs3BetBB,
                    Occurred = foldTo3BetInCOvs3BetBB,
                    CouldOccurred = couldFoldTo3BetInCOvs3BetBB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInBTNvs3BetSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInBTNvs3BetSB,
                    Occurred = foldTo3BetInBTNvs3BetSB,
                    CouldOccurred = couldFoldTo3BetInBTNvs3BetSB,
                };
            }
        }

        public virtual StatDto FoldTo3BetInBTNvs3BetBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldTo3BetInBTNvs3BetBB,
                    Occurred = foldTo3BetInBTNvs3BetBB,
                    CouldOccurred = couldFoldTo3BetInBTNvs3BetBB,
                };
            }
        }

        #endregion

        #region Check Flop as PFR and Fold to Turn Bet SRP

        public virtual StatDto CheckFlopAsPFRAndFoldToTurnBetIPSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToTurnBetIPSRP,
                    Occurred = checkFlopAsPFRAndFoldToTurnBetIPSRP,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToTurnBetIPSRP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToTurnBetOOPSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToTurnBetOOPSRP,
                    Occurred = checkFlopAsPFRAndFoldToTurnBetOOPSRP,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToRiverBetIPSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToRiverBetIPSRP,
                    Occurred = checkFlopAsPFRAndFoldToRiverBetIPSRP,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToRiverBetIPSRP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToRiverBetOOPSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToRiverBetOOPSRP,
                    Occurred = checkFlopAsPFRAndFoldToRiverBetOOPSRP,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToTurnBetIP3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToTurnBetIP3BetPot,
                    Occurred = checkFlopAsPFRAndFoldToTurnBetIP3BetPot,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToTurnBetOOP3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot,
                    Occurred = checkFlopAsPFRAndFoldToTurnBetOOP3BetPot,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToRiverBetIP3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToRiverBetIP3BetPot,
                    Occurred = checkFlopAsPFRAndFoldToRiverBetIP3BetPot,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot,
                };
            }
        }

        public virtual StatDto CheckFlopAsPFRAndFoldToRiverBetOOP3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot,
                    Occurred = checkFlopAsPFRAndFoldToRiverBetOOP3BetPot,
                    CouldOccurred = couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot,
                };
            }
        }

        #region Fold to continuation bets in SRP/3Bet/4Bet

        public virtual StatDto FoldToTripleBarrelSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTripleBarrelSRP,
                    Occurred = foldToTripleBarrelSRP,
                    CouldOccurred = facingTripleBarrelSRP,
                };
            }
        }

        public virtual StatDto FoldToTripleBarrel3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTripleBarrel3BetPot,
                    Occurred = foldToTripleBarrel3BetPot,
                    CouldOccurred = facingTripleBarrel3BetPot,
                };
            }
        }

        public virtual StatDto FoldToTripleBarrel4BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToTripleBarrel4BetPot,
                    Occurred = foldToTripleBarrel4BetPot,
                    CouldOccurred = facingTripleBarrel4BetPot,
                };
            }
        }

        public virtual StatDto FoldToDoubleBarrelSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToDoubleBarrelSRP,
                    Occurred = foldToDoubleBarrelSRP,
                    CouldOccurred = facingDoubleBarrelSRP,
                };
            }
        }

        public virtual StatDto FoldToDoubleBarrel4BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToDoubleBarrel4BetPot,
                    Occurred = foldToDoubleBarrel4BetPot,
                    CouldOccurred = facingDoubleBarrel4BetPot,
                };
            }
        }

        public virtual StatDto FoldToCBetSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToCBetSRP,
                    Occurred = foldToCBetSRP,
                    CouldOccurred = facingCBetSRP,
                };
            }
        }

        #endregion

        #region Open Shove UO Pot positional

        public virtual StatDto SBOpenShove1to8bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenShove1to8bbUOPot,
                    Occurred = sbOpenShove1to8bbUOPot,
                    CouldOccurred = sbOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto SBOpenShove9to14bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenShove9to14bbUOPot,
                    Occurred = sbOpenShove9to14bbUOPot,
                    CouldOccurred = sbOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto SBOpenShove15to25bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenShove15to25bbUOPot,
                    Occurred = sbOpenShove15to25bbUOPot,
                    CouldOccurred = sbOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto SBOpenShove26to50bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenShove26to50bbUOPot,
                    Occurred = sbOpenShove26to50bbUOPot,
                    CouldOccurred = sbOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto SBOpenShove51plusbbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenShove51plusbbUOPot,
                    Occurred = sbOpenShove51plusbbUOPot,
                    CouldOccurred = sbOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto BTNOpenShove1to8bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenShove1to8bbUOPot,
                    Occurred = btnOpenShove1to8bbUOPot,
                    CouldOccurred = btnOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto BTNOpenShove9to14bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenShove9to14bbUOPot,
                    Occurred = btnOpenShove9to14bbUOPot,
                    CouldOccurred = btnOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto BTNOpenShove15to25bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenShove15to25bbUOPot,
                    Occurred = btnOpenShove15to25bbUOPot,
                    CouldOccurred = btnOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto BTNOpenShove26to50bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenShove26to50bbUOPot,
                    Occurred = btnOpenShove26to50bbUOPot,
                    CouldOccurred = btnOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto BTNOpenShove51plusbbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenShove51plusbbUOPot,
                    Occurred = btnOpenShove51plusbbUOPot,
                    CouldOccurred = btnOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto COOpenShove1to8bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenShove1to8bbUOPot,
                    Occurred = coOpenShove1to8bbUOPot,
                    CouldOccurred = coOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto COOpenShove9to14bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenShove9to14bbUOPot,
                    Occurred = coOpenShove9to14bbUOPot,
                    CouldOccurred = coOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto COOpenShove15to25bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenShove15to25bbUOPot,
                    Occurred = coOpenShove15to25bbUOPot,
                    CouldOccurred = coOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto COOpenShove26to50bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenShove26to50bbUOPot,
                    Occurred = coOpenShove26to50bbUOPot,
                    CouldOccurred = coOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto COOpenShove51plusbbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenShove51plusbbUOPot,
                    Occurred = coOpenShove51plusbbUOPot,
                    CouldOccurred = coOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto MPOpenShove1to8bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenShove1to8bbUOPot,
                    Occurred = mpOpenShove1to8bbUOPot,
                    CouldOccurred = mpOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto MPOpenShove9to14bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenShove9to14bbUOPot,
                    Occurred = mpOpenShove9to14bbUOPot,
                    CouldOccurred = mpOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto MPOpenShove15to25bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenShove15to25bbUOPot,
                    Occurred = mpOpenShove15to25bbUOPot,
                    CouldOccurred = mpOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto MPOpenShove26to50bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenShove26to50bbUOPot,
                    Occurred = mpOpenShove26to50bbUOPot,
                    CouldOccurred = mpOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto MPOpenShove51plusbbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenShove51plusbbUOPot,
                    Occurred = mpOpenShove51plusbbUOPot,
                    CouldOccurred = mpOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto EPOpenShove1to8bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenShove1to8bbUOPot,
                    Occurred = epOpenShove1to8bbUOPot,
                    CouldOccurred = epOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto EPOpenShove9to14bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenShove9to14bbUOPot,
                    Occurred = epOpenShove9to14bbUOPot,
                    CouldOccurred = epOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto EPOpenShove15to25bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenShove15to25bbUOPot,
                    Occurred = epOpenShove15to25bbUOPot,
                    CouldOccurred = epOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto EPOpenShove26to50bbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenShove26to50bbUOPot,
                    Occurred = epOpenShove26to50bbUOPot,
                    CouldOccurred = epOpenShoveUOPot,
                };
            }
        }

        public virtual StatDto EPOpenShove51plusbbUOPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenShove51plusbbUOPot,
                    Occurred = epOpenShove51plusbbUOPot,
                    CouldOccurred = epOpenShoveUOPot,
                };
            }
        }

        #endregion

        #region Limp Positional & Fold to PFR%

        public virtual StatDto LimpEPFoldToPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpEPFoldToPFR,
                    Occurred = limpEPFoldToPFR,
                    CouldOccurred = limpEPFacedPFR,
                };
            }
        }

        public virtual StatDto LimpMPFoldToPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpMPFoldToPFR,
                    Occurred = limpMPFoldToPFR,
                    CouldOccurred = limpMPFacedPFR,
                };
            }
        }

        public virtual StatDto LimpCOFoldToPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpCOFoldToPFR,
                    Occurred = limpCOFoldToPFR,
                    CouldOccurred = limpCOFacedPFR,
                };
            }
        }

        public virtual StatDto LimpBTNFoldToPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpBTNFoldToPFR,
                    Occurred = limpBTNFoldToPFR,
                    CouldOccurred = limpBTNFacedPFR,
                };
            }
        }

        public virtual StatDto LimpSBFoldToPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpSBFoldToPFR,
                    Occurred = limpSBFoldToPFR,
                    CouldOccurred = limpSBFacedPFR,
                };
            }
        }

        #endregion

        #region Shoves over limpers positional

        public virtual StatDto SBShoveOverLimpers1to8bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBShoveOverLimpers1to8bb,
                    Occurred = sbShoveOverLimpers1to8bb,
                    CouldOccurred = sbShoveOverLimpers,
                };
            }
        }

        public virtual StatDto SBShoveOverLimpers9to14bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBShoveOverLimpers9to14bb,
                    Occurred = sbShoveOverLimpers9to14bb,
                    CouldOccurred = sbShoveOverLimpers,
                };
            }
        }

        public virtual StatDto SBShoveOverLimpers15to25bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBShoveOverLimpers15to25bb,
                    Occurred = sbShoveOverLimpers15to25bb,
                    CouldOccurred = sbShoveOverLimpers,
                };
            }
        }

        public virtual StatDto SBShoveOverLimpers26to50bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBShoveOverLimpers26to50bb,
                    Occurred = sbShoveOverLimpers26to50bb,
                    CouldOccurred = sbShoveOverLimpers,
                };
            }
        }

        public virtual StatDto SBShoveOverLimpers51plusbbObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBShoveOverLimpers51plusbb,
                    Occurred = sbShoveOverLimpers51plusbb,
                    CouldOccurred = sbShoveOverLimpers,
                };
            }
        }

        public virtual StatDto BTNShoveOverLimpers1to8bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNShoveOverLimpers1to8bb,
                    Occurred = btnShoveOverLimpers1to8bb,
                    CouldOccurred = btnShoveOverLimpers,
                };
            }
        }

        public virtual StatDto BTNShoveOverLimpers9to14bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNShoveOverLimpers9to14bb,
                    Occurred = btnShoveOverLimpers9to14bb,
                    CouldOccurred = btnShoveOverLimpers,
                };
            }
        }

        public virtual StatDto BTNShoveOverLimpers15to25bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNShoveOverLimpers15to25bb,
                    Occurred = btnShoveOverLimpers15to25bb,
                    CouldOccurred = btnShoveOverLimpers,
                };
            }
        }

        public virtual StatDto BTNShoveOverLimpers26to50bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNShoveOverLimpers26to50bb,
                    Occurred = btnShoveOverLimpers26to50bb,
                    CouldOccurred = btnShoveOverLimpers,
                };
            }
        }

        public virtual StatDto BTNShoveOverLimpers51plusbbObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNShoveOverLimpers51plusbb,
                    Occurred = btnShoveOverLimpers51plusbb,
                    CouldOccurred = btnShoveOverLimpers,
                };
            }
        }

        public virtual StatDto COShoveOverLimpers1to8bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = COShoveOverLimpers1to8bb,
                    Occurred = coShoveOverLimpers1to8bb,
                    CouldOccurred = coShoveOverLimpers,
                };
            }
        }

        public virtual StatDto COShoveOverLimpers9to14bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = COShoveOverLimpers9to14bb,
                    Occurred = coShoveOverLimpers9to14bb,
                    CouldOccurred = coShoveOverLimpers,
                };
            }
        }

        public virtual StatDto COShoveOverLimpers15to25bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = COShoveOverLimpers15to25bb,
                    Occurred = coShoveOverLimpers15to25bb,
                    CouldOccurred = coShoveOverLimpers,
                };
            }
        }

        public virtual StatDto COShoveOverLimpers26to50bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = COShoveOverLimpers26to50bb,
                    Occurred = coShoveOverLimpers26to50bb,
                    CouldOccurred = coShoveOverLimpers,
                };
            }
        }

        public virtual StatDto COShoveOverLimpers51plusbbObject
        {
            get
            {
                return new StatDto
                {
                    Value = COShoveOverLimpers51plusbb,
                    Occurred = coShoveOverLimpers51plusbb,
                    CouldOccurred = coShoveOverLimpers,
                };
            }
        }

        public virtual StatDto MPShoveOverLimpers1to8bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPShoveOverLimpers1to8bb,
                    Occurred = mpShoveOverLimpers1to8bb,
                    CouldOccurred = mpShoveOverLimpers,
                };
            }
        }

        public virtual StatDto MPShoveOverLimpers9to14bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPShoveOverLimpers9to14bb,
                    Occurred = mpShoveOverLimpers9to14bb,
                    CouldOccurred = mpShoveOverLimpers,
                };
            }
        }

        public virtual StatDto MPShoveOverLimpers15to25bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPShoveOverLimpers15to25bb,
                    Occurred = mpShoveOverLimpers15to25bb,
                    CouldOccurred = mpShoveOverLimpers,
                };
            }
        }

        public virtual StatDto MPShoveOverLimpers26to50bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPShoveOverLimpers26to50bb,
                    Occurred = mpShoveOverLimpers26to50bb,
                    CouldOccurred = mpShoveOverLimpers,
                };
            }
        }

        public virtual StatDto MPShoveOverLimpers51plusbbObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPShoveOverLimpers51plusbb,
                    Occurred = mpShoveOverLimpers51plusbb,
                    CouldOccurred = mpShoveOverLimpers,
                };
            }
        }

        public virtual StatDto EPShoveOverLimpers1to8bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPShoveOverLimpers1to8bb,
                    Occurred = epShoveOverLimpers1to8bb,
                    CouldOccurred = epShoveOverLimpers,
                };
            }
        }

        public virtual StatDto EPShoveOverLimpers9to14bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPShoveOverLimpers9to14bb,
                    Occurred = epShoveOverLimpers9to14bb,
                    CouldOccurred = epShoveOverLimpers,
                };
            }
        }

        public virtual StatDto EPShoveOverLimpers15to25bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPShoveOverLimpers15to25bb,
                    Occurred = epShoveOverLimpers15to25bb,
                    CouldOccurred = epShoveOverLimpers,
                };
            }
        }

        public virtual StatDto EPShoveOverLimpers26to50bbObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPShoveOverLimpers26to50bb,
                    Occurred = epShoveOverLimpers26to50bb,
                    CouldOccurred = epShoveOverLimpers,
                };
            }
        }

        public virtual StatDto EPShoveOverLimpers51plusbbObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPShoveOverLimpers51plusbb,
                    Occurred = epShoveOverLimpers51plusbb,
                    CouldOccurred = epShoveOverLimpers,
                };
            }
        }

        #endregion

        #region Positional Open minraise UO-PFR

        public virtual StatDto OpenMinraiseObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenMinraise,
                    Occurred = openMinraise,
                    CouldOccurred = Source.Totalhands - Source.NumberOfWalks,
                };
            }
        }

        public virtual StatDto EPOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = EPOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.EP ?? 0,
                    CouldOccurred = positionUnoppened?.EP ?? 0,
                };
            }
        }

        public virtual StatDto MPOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = MPOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.MP ?? 0,
                    CouldOccurred = positionUnoppened?.MP ?? 0,
                };
            }
        }

        public virtual StatDto COOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = COOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.CO ?? 0,
                    CouldOccurred = positionUnoppened?.CO ?? 0,
                };
            }
        }

        public virtual StatDto BTNOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.BN ?? 0,
                    CouldOccurred = positionUnoppened?.BN ?? 0,
                };
            }
        }

        public virtual StatDto SBOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SBOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.SB ?? 0,
                    CouldOccurred = positionUnoppened?.SB ?? 0,
                };
            }
        }

        public virtual StatDto BBOpenMinraiseUOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = BBOpenMinraiseUOPFR,
                    Occurred = positionOpenMinraiseUOPFR?.BB ?? 0,
                    CouldOccurred = positionUnoppened?.BB ?? 0,
                };
            }
        }

        #endregion

        #region  Positional squeeze

        public virtual StatDto SqueezeEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeEP,
                    Occurred = positionDidSqueeze?.EP ?? 0,
                    CouldOccurred = positionCouldSqueeze?.EP ?? 0,
                };
            }
        }

        public virtual StatDto SqueezeMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeMP,
                    Occurred = positionDidSqueeze?.MP ?? 0,
                    CouldOccurred = positionCouldSqueeze?.MP ?? 0,
                };
            }
        }

        public virtual StatDto SqueezeCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeCO,
                    Occurred = positionDidSqueeze?.CO ?? 0,
                    CouldOccurred = positionCouldSqueeze?.CO ?? 0,
                };
            }
        }

        public virtual StatDto SqueezeBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBTN,
                    Occurred = positionDidSqueeze?.BN ?? 0,
                    CouldOccurred = positionCouldSqueeze?.BN ?? 0,
                };
            }
        }

        public virtual StatDto SqueezeSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeSB,
                    Occurred = positionDidSqueeze?.SB ?? 0,
                    CouldOccurred = positionCouldSqueeze?.SB ?? 0,
                };
            }
        }

        public virtual StatDto SqueezeBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBB,
                    Occurred = positionDidSqueeze?.BB ?? 0,
                    CouldOccurred = positionCouldSqueeze?.BB ?? 0,
                };
            }
        }

        #endregion

        #region  Squeeze vs PFR

        public virtual StatDto SqueezeBBVsBTNPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBBVsBTNPFR,
                    Occurred = didSqueezeBBVsBTNPFR,
                    CouldOccurred = couldSqueezeBBVsBTNPFR,
                };
            }
        }

        public virtual StatDto SqueezeBBVsCOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBBVsCOPFR,
                    Occurred = didSqueezeBBVsCOPFR,
                    CouldOccurred = couldSqueezeBBVsCOPFR,
                };
            }
        }

        public virtual StatDto SqueezeBBVsMPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBBVsMPPFR,
                    Occurred = didSqueezeBBVsMPPFR,
                    CouldOccurred = couldSqueezeBBVsMPPFR,
                };
            }
        }

        public virtual StatDto SqueezeBBVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBBVsEPPFR,
                    Occurred = didSqueezeBBVsEPPFR,
                    CouldOccurred = couldSqueezeBBVsEPPFR,
                };
            }
        }

        public virtual StatDto SqueezeSBVsCOPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeSBVsCOPFR,
                    Occurred = didSqueezeSBVsCOPFR,
                    CouldOccurred = couldSqueezeSBVsCOPFR,
                };
            }
        }

        public virtual StatDto SqueezeSBVsMPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeSBVsMPPFR,
                    Occurred = didSqueezeSBVsMPPFR,
                    CouldOccurred = couldSqueezeSBVsMPPFR,
                };
            }
        }

        public virtual StatDto SqueezeSBVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeSBVsEPPFR,
                    Occurred = didSqueezeSBVsEPPFR,
                    CouldOccurred = couldSqueezeSBVsEPPFR,
                };
            }
        }

        public virtual StatDto SqueezeBTNVsMPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBTNVsMPPFR,
                    Occurred = didSqueezeBTNVsMPPFR,
                    CouldOccurred = couldSqueezeBTNVsMPPFR,
                };
            }
        }

        public virtual StatDto SqueezeBTNVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeBTNVsEPPFR,
                    Occurred = didSqueezeBTNVsEPPFR,
                    CouldOccurred = couldSqueezeBTNVsEPPFR,
                };
            }
        }

        public virtual StatDto SqueezeCOVsMPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeCOVsMPPFR,
                    Occurred = didSqueezeCOVsMPPFR,
                    CouldOccurred = couldSqueezeCOVsMPPFR,
                };
            }
        }

        public virtual StatDto SqueezeCOVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeCOVsEPPFR,
                    Occurred = didSqueezeCOVsEPPFR,
                    CouldOccurred = couldSqueezeCOVsEPPFR,
                };
            }
        }

        public virtual StatDto SqueezeMPVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeMPVsEPPFR,
                    Occurred = didSqueezeMPVsEPPFR,
                    CouldOccurred = couldSqueezeMPVsEPPFR,
                };
            }
        }

        public virtual StatDto SqueezeEPVsEPPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = SqueezeEPVsEPPFR,
                    Occurred = didSqueezeEPVsEPPFR,
                    CouldOccurred = couldSqueezeEPVsEPPFR,
                };
            }
        }

        #endregion

        #region Fold to Squeeze as Cold Caller

        public virtual StatDto FoldToSqueezeAsColdCallerObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToSqueezeAsColdCaller,
                    Occurred = foldToSqueezeAsColdCaller,
                    CouldOccurred = facedSqueezeAsColdCaller
                };
            }
        }

        #endregion

        #region 4-Bet vs Blind 3-Bet%

        public virtual StatDto FourBetVsBlind3BetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetVsBlind3Bet,
                    Occurred = did4BetVsBlind3Bet,
                    CouldOccurred = could4BetVsBlind3Bet
                };
            }
        }

        #endregion

        #region BTN Re/Def vs CO Steal

        public virtual StatDto BTNReStealVsCOStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNReStealVsCOSteal,
                    Occurred = btnReStealVsCOSteal,
                    CouldOccurred = btnFacedCOSteal
                };
            }
        }

        public virtual StatDto BTNDefendVsCOStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = BTNDefendVsCOSteal,
                    Occurred = btnDefendVsCOSteal,
                    CouldOccurred = btnFacedCOSteal
                };
            }
        }

        #endregion

        #region Positional Call & Fold to Steal

        public virtual StatDto FoldToStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToStealInSB,
                    Occurred = foldToStealInSB,
                    CouldOccurred = facedStealInSB
                };
            }
        }

        public virtual StatDto FoldToStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToStealInBB,
                    Occurred = foldToStealInBB,
                    CouldOccurred = facedStealInBB
                };
            }
        }

        public virtual StatDto CalledStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledStealInSB,
                    Occurred = calledStealInSB,
                    CouldOccurred = facedStealInSB
                };
            }
        }

        public virtual StatDto CalledStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledStealInBB,
                    Occurred = calledStealInBB,
                    CouldOccurred = facedStealInBB
                };
            }
        }

        public virtual StatDto FoldToBTNStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToBTNStealInSB,
                    Occurred = foldToBTNStealInSB,
                    CouldOccurred = facedBTNStealInSB
                };
            }
        }

        public virtual StatDto FoldToBTNStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToBTNStealInBB,
                    Occurred = foldToBTNStealInBB,
                    CouldOccurred = facedBTNStealInBB
                };
            }
        }

        public virtual StatDto FoldToCOStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToCOStealInSB,
                    Occurred = foldToCOStealInSB,
                    CouldOccurred = facedCOStealInSB
                };
            }
        }

        public virtual StatDto FoldToCOStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToCOStealInBB,
                    Occurred = foldToCOStealInBB,
                    CouldOccurred = facedCOStealInBB
                };
            }
        }

        public virtual StatDto CalledBTNStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledBTNStealInSB,
                    Occurred = calledBTNStealInSB,
                    CouldOccurred = facedBTNStealInSB
                };
            }
        }

        public virtual StatDto CalledBTNStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledBTNStealInBB,
                    Occurred = calledBTNStealInBB,
                    CouldOccurred = facedBTNStealInBB
                };
            }
        }

        public virtual StatDto CalledCOStealInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledCOStealInSB,
                    Occurred = calledCOStealInSB,
                    CouldOccurred = facedCOStealInSB
                };
            }
        }

        public virtual StatDto CalledCOStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = CalledCOStealInBB,
                    Occurred = calledCOStealInBB,
                    CouldOccurred = facedCOStealInBB
                };
            }
        }

        public virtual StatDto OvercallBTNStealInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = OvercallBTNStealInBB,
                    Occurred = overcallBTNStealInBB,
                    CouldOccurred = couldOvercallBTNStealInBB
                };
            }
        }

        #endregion

        #region

        public virtual StatDto WTSDAsPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAsPFR,
                    Occurred = wtsdAsPFR,
                    CouldOccurred = wtsdAsPFROpportunity
                };
            }
        }

        public virtual StatDto WTSDAs4BettorObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDAs4Bettor,
                    Occurred = wtsdAs4Bettor,
                    CouldOccurred = wtsdAs4BettorOpportunity
                };
            }
        }

        #endregion

        #region Call 4-Bet positional 

        public virtual StatDto Call4BetIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetIP,
                    Occurred = call4BetIP,
                    CouldOccurred = faced4BetIP
                };
            }
        }

        public virtual StatDto Call4BetOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetOOP,
                    Occurred = call4BetOOP,
                    CouldOccurred = faced4BetOOP
                };
            }
        }

        public virtual StatDto Call4BetEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetEP,
                    Occurred = positionCall4Bet?.EP ?? 0,
                    CouldOccurred = positionFaced4Bet?.EP ?? 0
                };
            }
        }

        public virtual StatDto Call4BetMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetMP,
                    Occurred = positionCall4Bet?.MP ?? 0,
                    CouldOccurred = positionFaced4Bet?.MP ?? 0
                };
            }
        }

        public virtual StatDto Call4BetCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetCO,
                    Occurred = positionCall4Bet?.CO ?? 0,
                    CouldOccurred = positionFaced4Bet?.CO ?? 0
                };
            }
        }

        public virtual StatDto Call4BetBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetBTN,
                    Occurred = positionCall4Bet?.BN ?? 0,
                    CouldOccurred = positionFaced4Bet?.BN ?? 0
                };
            }
        }

        public virtual StatDto Call4BetSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetSB,
                    Occurred = positionCall4Bet?.SB ?? 0,
                    CouldOccurred = positionFaced4Bet?.SB ?? 0
                };
            }
        }

        public virtual StatDto Call4BetBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = Call4BetBB,
                    Occurred = positionCall4Bet?.BB ?? 0,
                    CouldOccurred = positionFaced4Bet?.BB ?? 0
                };
            }
        }

        #endregion

        #region Total overcall SRP%

        public virtual StatDto TotalOverCallSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = TotalOverCallSRP,
                    Occurred = totalOverCallSRP,
                    CouldOccurred = couldTotalOverCallSRP
                };
            }
        }

        #endregion

        #region Limped pot Flop Steal IP%

        public virtual StatDto LimpedPotFlopStealIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = LimpedPotFlopStealIP,
                    Occurred = limpedPotFlopStealIP,
                    CouldOccurred = couldLimpedPotFlopStealIP
                };
            }
        }

        #endregion

        #region Flop-Check Call

        public virtual StatDto FlopCheckCallObject
        {
            get
            {
                return new StatDto
                {
                    Value = FlopCheckCall,
                    Occurred = flopCheckCall,
                    CouldOccurred = couldFlopCheckCall
                };
            }
        }

        #endregion

        #region Call Flop & Fold Turn

        public virtual StatDto CallFlopFoldTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = CallFlopFoldTurn,
                    Occurred = didCallFlopFoldTurn,
                    CouldOccurred = couldCallFlopFoldTurn
                };
            }
        }

        #endregion

        #region River fold in SRP/3-Bet/4-Bet

        public virtual StatDto RiverFoldInSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverFoldInSRP,
                    Occurred = didRiverFoldInSRP,
                    CouldOccurred = couldRiverFoldInSRP
                };
            }
        }

        public virtual StatDto RiverFoldIn3BetObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverFoldIn3Bet,
                    Occurred = didRiverFoldIn3Bet,
                    CouldOccurred = couldRiverFoldIn3Bet
                };
            }
        }

        public virtual StatDto RiverFoldIn4BetObject
        {
            get
            {
                return new StatDto
                {
                    Value = RiverFoldIn4Bet,
                    Occurred = didRiverFoldIn4Bet,
                    CouldOccurred = couldRiverFoldIn4Bet
                };
            }
        }

        #endregion

        #region Delayed Turn C-Bet in SRP/4-Bet Pot%

        public virtual StatDto DelayedTurnCBetInSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = DelayedTurnCBetInSRP,
                    Occurred = didDelayedTurnCBetInSRP,
                    CouldOccurred = couldDelayedTurnCBetInSRP
                };
            }
        }

        public virtual StatDto DelayedTurnCBetIn4BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = DelayedTurnCBetIn4BetPot,
                    Occurred = didDelayedTurnCBetIn4BetPot,
                    CouldOccurred = couldDelayedTurnCBetIn4BetPot
                };
            }
        }

        #endregion

        #region  Check-Raise Flop as PFR SRP/3-Bet pot

        public virtual StatDto CheckRaiseFlopAsPFRInSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRaiseFlopAsPFRInSRP,
                    Occurred = didCheckRaiseFlopAsPFRInSRP,
                    CouldOccurred = couldCheckRaiseFlopAsPFRInSRP
                };
            }
        }

        public virtual StatDto CheckRaiseFlopAsPFRIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRaiseFlopAsPFRIn3BetPot,
                    Occurred = didCheckRaiseFlopAsPFRIn3BetPot,
                    CouldOccurred = couldCheckRaiseFlopAsPFRIn3BetPot
                };
            }
        }

        #endregion

        #region Open Limp positional

        public virtual StatDto OpenLimpEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenLimpEP,
                    Occurred = positionDidOpenLimp?.EP ?? 0,
                    CouldOccurred = positionCouldOpenLimp?.EP ?? 0
                };
            }
        }

        public virtual StatDto OpenLimpMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenLimpMP,
                    Occurred = positionDidOpenLimp?.MP ?? 0,
                    CouldOccurred = positionCouldOpenLimp?.MP ?? 0
                };
            }
        }

        public virtual StatDto OpenLimpCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenLimpCO,
                    Occurred = positionDidOpenLimp?.CO ?? 0,
                    CouldOccurred = positionCouldOpenLimp?.CO ?? 0
                };
            }
        }

        public virtual StatDto OpenLimpBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenLimpBTN,
                    Occurred = positionDidOpenLimp?.BN ?? 0,
                    CouldOccurred = positionCouldOpenLimp?.BN ?? 0
                };
            }
        }

        public virtual StatDto OpenLimpSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = OpenLimpSB,
                    Occurred = positionDidOpenLimp?.SB ?? 0,
                    CouldOccurred = positionCouldOpenLimp?.SB ?? 0
                };
            }
        }

        #endregion

        #region Straddle stats 

        public virtual StatDto CheckInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckInStraddle,
                    Occurred = didCheckInStraddle,
                    CouldOccurred = couldActInStraddle
                };
            }
        }

        public virtual StatDto PFRInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = PFRInStraddle,
                    Occurred = didPFRInStraddle,
                    CouldOccurred = couldActInStraddle
                };
            }
        }

        public virtual StatDto ThreeBetInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInStraddle,
                    Occurred = did3BetInStraddle,
                    CouldOccurred = could3BetInStraddle
                };
            }
        }

        public virtual StatDto FourBetInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInStraddle,
                    Occurred = did4BetInStraddle,
                    CouldOccurred = could4BetInStraddle
                };
            }
        }

        public virtual StatDto FoldInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldInStraddle,
                    Occurred = didFoldInStraddle,
                    CouldOccurred = couldActInStraddle
                };
            }
        }

        public virtual StatDto WTSDInStraddleObject
        {
            get
            {
                return new StatDto
                {
                    Value = WTSDInStraddle,
                    Occurred = wtsdInStraddle,
                    CouldOccurred = wtsdOpportunityInStraddle
                };
            }
        }

        #endregion

        #region Skip Flop C-Bet SRP & C/F Flop OOP%

        public virtual StatDto SkipFlopCBetInSRPandCheckFoldFlopOOPObject
        {
            get
            {
                return new StatDto
                {
                    Value = SkipFlopCBetInSRPandCheckFoldFlopOOP,
                    Occurred = didSkipFlopCBetInSRPandCheckFoldFlopOOP,
                    CouldOccurred = couldSkipFlopCBetInSRPandCheckFoldFlopOOP
                };
            }
        }

        #endregion

        #region Fold to delayed Turn C-Bet

        public virtual StatDto FoldedToDelayedCBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldedToDelayedCBet,
                    Occurred = foldedToDelayedCBet,
                    CouldOccurred = Source.FacedDelayedCBet
                };
            }
        }

        #endregion

        #region Double/Triple barrel

        public virtual StatDto DoubleBarrelObject
        {
            get
            {
                return new StatDto
                {
                    Value = DoubleBarrel,
                    Occurred = didDoubleBarrel,
                    CouldOccurred = couldDoubleBarrel
                };
            }
        }

        public virtual StatDto TripleBarrelObject
        {
            get
            {
                return new StatDto
                {
                    Value = TripleBarrel,
                    Occurred = didTripleBarrel,
                    CouldOccurred = couldTripleBarrel
                };
            }
        }

        #endregion

        #endregion

        public virtual StatDto CheckRaiseFlopAsPFRObject
        {
            get
            {
                return new StatDto
                {
                    Value = CheckRaiseFlopAsPFR,
                    Occurred = checkRaiseFlopAsPFR,
                    CouldOccurred = couldCheckRaiseFlopAsPFR,
                };
            }
        }

        public virtual StatDto ProbeBetTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = ProbeBetTurn,
                    Occurred = probeBetTurn,
                    CouldOccurred = Source.CouldProbeBetTurn,
                };
            }
        }

        public virtual StatDto ProbeBetRiverObject
        {
            get
            {
                return new StatDto
                {
                    Value = ProbeBetRiver,
                    Occurred = probeBetRiver,
                    CouldOccurred = Source.CouldProbeBetRiver,
                };
            }
        }

        public virtual StatDto FloatFlopThenBetTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = FloatFlopThenBetTurn,
                    Occurred = floatFlopThenBetTurn,
                    CouldOccurred = couldFloatFlopThenBetTurn,
                };
            }
        }

        public virtual StatDto FoldBBvsSBStealObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldBBvsSBSteal,
                    Occurred = foldBBvsSBSteal,
                    CouldOccurred = couldFoldBBvsSBSteal,
                };
            }
        }

        public virtual StatDto BetTurnWhenCheckedToSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetTurnWhenCheckedToSRP,
                    Occurred = betTurnWhenCheckedToSRP,
                    CouldOccurred = couldBetTurnWhenCheckedToSRP,
                };
            }
        }

        public virtual StatDto BetRiverWhenCheckedToSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetRiverWhenCheckedToSRP,
                    Occurred = betRiverWhenCheckedToSRP,
                    CouldOccurred = couldBetRiverWhenCheckedToSRP,
                };
            }
        }

        public virtual StatDto BetFlopWhenCheckedToIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetFlopWhenCheckedToIn3BetPot,
                    Occurred = betFlopWhenCheckedToIn3BetPot,
                    CouldOccurred = couldBetFlopWhenCheckedToIn3BetPot,
                };
            }
        }

        public virtual StatDto BetTurnWhenCheckedToIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetTurnWhenCheckedToIn3BetPot,
                    Occurred = betTurnWhenCheckedToIn3BetPot,
                    CouldOccurred = couldBetTurnWhenCheckedToIn3BetPot,
                };
            }
        }

        public virtual StatDto BetRiverWhenCheckedToIn3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = BetRiverWhenCheckedToIn3BetPot,
                    Occurred = betRiverWhenCheckedToIn3BetPot,
                    CouldOccurred = couldBetRiverWhenCheckedToIn3BetPot,
                };
            }
        }

        public virtual StatDto FoldToProbeBetTurnObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToProbeBetTurn,
                    Occurred = foldedToProbeBetTurn,
                    CouldOccurred = facedProbeBetTurn,
                };
            }
        }

        public virtual StatDto FoldToProbeBetRiverObject
        {
            get
            {
                return new StatDto
                {
                    Value = FoldToProbeBetRiver,
                    Occurred = foldedToProbeBetRiver,
                    CouldOccurred = facedProbeBetRiver,
                };
            }
        }

        #region Positional

        #region Unopened Pot        

        public virtual StatDto UO_PFR_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_EP,
                    Occurred = Source.UO_PFR_EP,
                    CouldOccurred = positionUnoppened?.EP ?? 0
                };
            }
        }

        public virtual StatDto UO_PFR_MPObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_MP,
                    Occurred = Source.UO_PFR_MP,
                    CouldOccurred = positionUnoppened?.MP ?? 0
                };
            }
        }

        public virtual StatDto UO_PFR_COObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_CO,
                    Occurred = Source.UO_PFR_CO,
                    CouldOccurred = positionUnoppened?.CO ?? 0
                };
            }
        }

        public virtual StatDto UO_PFR_BNObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_BN,
                    Occurred = Source.UO_PFR_BN,
                    CouldOccurred = positionUnoppened?.BN ?? 0
                };
            }
        }

        public virtual StatDto UO_PFR_SBObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_SB,
                    Occurred = Source.UO_PFR_SB,
                    CouldOccurred = positionUnoppened?.SB ?? 0
                };
            }
        }

        public virtual StatDto UO_PFR_BBObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_BB,
                    Occurred = Source.UO_PFR_BB,
                    CouldOccurred = positionUnoppened?.BB ?? 0
                };
            }
        }

        #endregion

        #region Positional VPIP

        public virtual StatDto VPIP_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_EP,
                    Occurred = positionVPIP?.EP ?? 0,
                    CouldOccurred = positionTotal?.EP ?? 0
                };
            }
        }

        public virtual StatDto VPIP_MPObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_MP,
                    Occurred = positionVPIP?.MP ?? 0,
                    CouldOccurred = positionTotal?.MP ?? 0
                };
            }
        }

        public virtual StatDto VPIP_COObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_CO,
                    Occurred = positionVPIP?.CO ?? 0,
                    CouldOccurred = positionTotal?.CO ?? 0
                };
            }
        }

        public virtual StatDto VPIP_BNObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_BN,
                    Occurred = positionVPIP?.BN ?? 0,
                    CouldOccurred = positionTotal?.BN ?? 0
                };
            }
        }

        public virtual StatDto VPIP_SBObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_SB,
                    Occurred = positionVPIP?.SB ?? 0,
                    CouldOccurred = positionTotal?.SB ?? 0
                };
            }
        }

        public virtual StatDto VPIP_BBObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_BB,
                    Occurred = positionVPIP?.BB ?? 0,
                    CouldOccurred = positionTotal?.BB ?? 0
                };
            }
        }

        #endregion

        #region Positional Cold Call

        public virtual StatDto ColdCall_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_EP,
                    Occurred = positionDidColdCall?.EP ?? 0,
                    CouldOccurred = positionCouldColdCall?.EP ?? 0
                };
            }
        }

        public virtual StatDto ColdCall_MPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_MP,
                    Occurred = positionDidColdCall?.MP ?? 0,
                    CouldOccurred = positionCouldColdCall?.MP ?? 0
                };
            }
        }

        public virtual StatDto ColdCall_COObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_CO,
                    Occurred = positionDidColdCall?.CO ?? 0,
                    CouldOccurred = positionCouldColdCall?.CO ?? 0
                };
            }
        }

        public virtual StatDto ColdCall_BNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_BN,
                    Occurred = positionDidColdCall?.BN ?? 0,
                    CouldOccurred = positionCouldColdCall?.BN ?? 0
                };
            }
        }

        public virtual StatDto ColdCall_SBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_SB,
                    Occurred = positionDidColdCall?.SB ?? 0,
                    CouldOccurred = positionCouldColdCall?.SB ?? 0
                };
            }
        }

        public virtual StatDto ColdCall_BBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_BB,
                    Occurred = positionDidColdCall?.BB ?? 0,
                    CouldOccurred = positionCouldColdCall?.BB ?? 0
                };
            }
        }

        public virtual StatDto ColdCallThreeBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallThreeBet,
                    Occurred = Source.DidColdCallThreeBet,
                    CouldOccurred = Source.CouldColdCallThreeBet
                };
            }
        }

        public virtual StatDto ColdCallFourBetObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallFourBet,
                    Occurred = Source.DidColdCallFourBet,
                    CouldOccurred = Source.CouldColdCallFourBet
                };
            }
        }

        public virtual StatDto ColdCallVsBtnOpenObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallVsBtnOpen,
                    Occurred = Source.DidColdCallVsOpenRaiseBtn,
                    CouldOccurred = Source.CouldColdCallVsOpenRaiseBtn
                };
            }
        }

        public virtual StatDto ColdCallVsCoOpenObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallVsCoOpen,
                    Occurred = Source.DidColdCallVsOpenRaiseCo,
                    CouldOccurred = Source.CouldColdCallVsOpenRaiseCo
                };
            }
        }

        public virtual StatDto ColdCallVsSbOpenObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallVsSbOpen,
                    Occurred = Source.DidColdCallVsOpenRaiseSb,
                    CouldOccurred = Source.CouldColdCallVsOpenRaiseSb
                };
            }
        }

        #endregion

        #region Positional 3-Bet

        public virtual StatDto ThreeBet_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_EP,
                    Occurred = positionDidThreeBet?.EP ?? 0,
                    CouldOccurred = positionCouldThreeBet?.EP ?? 0
                };
            }
        }
        public virtual StatDto ThreeBet_MPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_MP,
                    Occurred = positionDidThreeBet?.MP ?? 0,
                    CouldOccurred = positionCouldThreeBet?.MP ?? 0
                };
            }
        }
        public virtual StatDto ThreeBet_COObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_CO,
                    Occurred = positionDidThreeBet?.CO ?? 0,
                    CouldOccurred = positionCouldThreeBet?.CO ?? 0
                };
            }
        }
        public virtual StatDto ThreeBet_BNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_BN,
                    Occurred = positionDidThreeBet?.BN ?? 0,
                    CouldOccurred = positionCouldThreeBet?.BN ?? 0
                };
            }
        }
        public virtual StatDto ThreeBet_SBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_SB,
                    Occurred = positionDidThreeBet?.SB ?? 0,
                    CouldOccurred = positionCouldThreeBet?.SB ?? 0
                };
            }
        }
        public virtual StatDto ThreeBet_BBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_BB,
                    Occurred = positionDidThreeBet?.BB ?? 0,
                    CouldOccurred = positionCouldThreeBet?.BB ?? 0
                };
            }
        }

        #endregion

        #region Positional 4-Bet

        public virtual StatDto FourBetInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInBB,
                    Occurred = positionDidFourBet?.BB ?? 0,
                    CouldOccurred = positionCouldFourBet?.BB ?? 0,
                };
            }
        }

        public virtual StatDto FourBetInBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInBTN,
                    Occurred = positionDidFourBet?.BN ?? 0,
                    CouldOccurred = positionCouldFourBet?.BN ?? 0,
                };
            }
        }

        public virtual StatDto FourBetInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInCO,
                    Occurred = positionDidFourBet?.CO ?? 0,
                    CouldOccurred = positionCouldFourBet?.CO ?? 0,
                };
            }
        }

        public virtual StatDto FourBetInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInMP,
                    Occurred = positionDidFourBet?.MP ?? 0,
                    CouldOccurred = positionCouldFourBet?.MP ?? 0,
                };
            }
        }

        public virtual StatDto FourBetInEPObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInMP,
                    Occurred = positionDidFourBet?.EP ?? 0,
                    CouldOccurred = positionCouldFourBet?.EP ?? 0,
                };
            }
        }

        public virtual StatDto FourBetInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInSB,
                    Occurred = positionDidFourBet?.SB ?? 0,
                    CouldOccurred = positionCouldFourBet?.SB ?? 0,
                };
            }
        }

        #endregion

        #region Positional Cold Call 3-Bet

        public virtual StatDto ColdCall3BetInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall3BetInBB,
                    Occurred = positionDidColdCallThreeBet?.BB ?? 0,
                    CouldOccurred = positionCouldColdCallThreeBet?.BB ?? 0
                };
            }
        }

        public virtual StatDto ColdCall3BetInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall3BetInSB,
                    Occurred = positionDidColdCallThreeBet?.SB ?? 0,
                    CouldOccurred = positionCouldColdCallThreeBet?.SB ?? 0
                };
            }
        }

        public virtual StatDto ColdCall3BetInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall3BetInMP,
                    Occurred = positionDidColdCallThreeBet?.MP ?? 0,
                    CouldOccurred = positionCouldColdCallThreeBet?.MP ?? 0
                };
            }
        }

        public virtual StatDto ColdCall3BetInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall3BetInCO,
                    Occurred = positionDidColdCallThreeBet?.CO ?? 0,
                    CouldOccurred = positionCouldColdCallThreeBet?.CO ?? 0
                };
            }
        }

        public virtual StatDto ColdCall3BetInBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall3BetInBTN,
                    Occurred = positionDidColdCallThreeBet?.BN ?? 0,
                    CouldOccurred = positionCouldColdCallThreeBet?.BN ?? 0
                };
            }
        }

        #endregion

        #region Positional Cold Call 4-Bet

        public virtual StatDto ColdCall4BetInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall4BetInBB,
                    Occurred = positionDidColdCallFourBet?.BB ?? 0,
                    CouldOccurred = positionCouldColdCallFourBet?.BB ?? 0
                };
            }
        }

        public virtual StatDto ColdCall4BetInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall4BetInSB,
                    Occurred = positionDidColdCallFourBet?.SB ?? 0,
                    CouldOccurred = positionCouldColdCallFourBet?.SB ?? 0
                };
            }
        }

        public virtual StatDto ColdCall4BetInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall4BetInMP,
                    Occurred = positionDidColdCallFourBet?.MP ?? 0,
                    CouldOccurred = positionCouldColdCallFourBet?.MP ?? 0
                };
            }
        }

        public virtual StatDto ColdCall4BetInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall4BetInCO,
                    Occurred = positionDidColdCallFourBet?.CO ?? 0,
                    CouldOccurred = positionCouldColdCallFourBet?.CO ?? 0
                };
            }
        }

        public virtual StatDto ColdCall4BetInBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall4BetInBTN,
                    Occurred = positionDidColdCallFourBet?.BN ?? 0,
                    CouldOccurred = positionCouldColdCallFourBet?.BN ?? 0
                };
            }
        }

        public virtual StatDto DoubleBarrelSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = DoubleBarrelSRP,
                    Occurred = doubleBarrelSRP,
                    CouldOccurred = couldDoubleBarrelSRP
                };
            }
        }

        public virtual StatDto DoubleBarrel3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = DoubleBarrel3BetPot,
                    Occurred = doubleBarrel3BetPot,
                    CouldOccurred = couldDoubleBarrel3BetPot
                };
            }
        }

        public virtual StatDto TripleBarrelSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = TripleBarrelSRP,
                    Occurred = tripleBarrelSRP,
                    CouldOccurred = couldTripleBarrelSRP
                };
            }
        }

        public virtual StatDto TripleBarrel3BetPotObject
        {
            get
            {
                return new StatDto
                {
                    Value = TripleBarrel3BetPot,
                    Occurred = tripleBarrel3BetPot,
                    CouldOccurred = couldTripleBarrel3BetPot
                };
            }
        }

        public virtual StatDto CBetThenFoldFlopSRPObject
        {
            get
            {
                return new StatDto
                {
                    Value = CBetThenFoldFlopSRP,
                    Occurred = cBetThenFoldFlopSRP,
                    CouldOccurred = couldCBetThenFoldFlopSRP
                };
            }
        }

        #endregion

        #endregion

        #region Session only

        #region CardsLists

        private FixedSizeList<string> cardsList;

        public virtual FixedSizeList<string> CardsList
        {
            get
            {
                return cardsList;
            }
            set
            {
                if (ReferenceEquals(cardsList, value))
                {
                    return;
                }

                cardsList = value;
            }
        }

        private FixedSizeList<string> threeBetCardsList;

        public virtual FixedSizeList<string> ThreeBetCardsList
        {
            get
            {
                return threeBetCardsList;
            }
            set
            {
                if (ReferenceEquals(threeBetCardsList, value))
                {
                    return;
                }

                threeBetCardsList = value;
            }
        }

        private FixedSizeList<Tuple<int, int>> recentAggList;

        public virtual FixedSizeList<Tuple<int, int>> RecentAggList
        {
            get
            {
                return recentAggList;
            }
            set
            {
                if (ReferenceEquals(recentAggList, value))
                {
                    return;
                }

                recentAggList = value;
            }
        }

        #endregion

        #region Recent Aggressive

        public virtual decimal RecentAggPr
        {
            get
            {
                var occured = recentAggList?.Sum(x => x.Item1);
                var couldOccured = recentAggList?.Sum(x => x.Item2);

                return GetPercentage(occured, couldOccured);
            }
        }

        public virtual StatDto RecentAggPrObject
        {
            get
            {
                return new StatDto
                {
                    Value = RecentAggPr,
                    Occurred = recentAggList?.Sum(x => x.Item1) ?? 0,
                    CouldOccurred = recentAggList?.Sum(x => x.Item2) ?? 0
                };
            }
        }

        #endregion

        #region  Session stats 

        private Dictionary<Stat, IList<decimal>> statsSessionCollection;

        public Dictionary<Stat, IList<decimal>> StatsSessionCollection
        {
            get
            {
                return statsSessionCollection;
            }
            set
            {
                if (ReferenceEquals(statsSessionCollection, value))
                {
                    return;
                }

                statsSessionCollection = value;
            }
        }

        #endregion

        #endregion

        #region overridden methods

        public override void AddStatistic(Playerstatistic statistic)
        {
            base.AddStatistic(statistic);

            AddCards(statistic);
            AddThreeBetCards(statistic);
            AddStatsToSession(statistic);
            AddRecentAgg(statistic);
        }

        protected virtual void AddCards(Playerstatistic statistic)
        {
            if (CardsList != null && !string.IsNullOrWhiteSpace(statistic.Cards))
            {
                CardsList.Add(statistic.Cards);
            }
        }

        protected virtual void AddThreeBetCards(Playerstatistic statistic)
        {
            if (threeBetCardsList != null && !string.IsNullOrWhiteSpace(statistic.Cards) && statistic.Didthreebet != 0)
            {
                threeBetCardsList.Add(statistic.Cards);
            }
        }

        protected virtual void AddRecentAgg(Playerstatistic statistic)
        {
            if (recentAggList != null)
            {
                recentAggList.Add(new Tuple<int, int>(statistic.Totalbets, statistic.Totalpostflopstreetsplayed));
            }
        }

        public virtual void AddStatsToSession(Playerstatistic statistic)
        {
            if (statsSessionCollection == null)
            {
                return;
            }

            foreach (var statBase in StatsProvider.StatsBases.Values)
            {
                if (string.IsNullOrEmpty(statBase.PropertyName))
                {
                    continue;
                }

                if (!statsSessionCollection.ContainsKey(statBase.Stat))
                {
                    statsSessionCollection.Add(statBase.Stat, new List<decimal>());
                }

                var statValue = statBase.Stat == Stat.NetWon ?
                    statistic.NetWon :
                    (decimal)ReflectionHelper.GetMemberValue(this, statBase.PropertyName);

                statsSessionCollection[statBase.Stat].Add(statValue);
            }
        }

        #endregion
    }
}