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
                    CouldOccurred = Source.Totalhands
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
                    CouldOccurred = Source.Totalhands
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
                    Occurred = Source.Didfourbet,
                    CouldOccurred = Source.Couldfourbet
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
                    CouldOccurred = Source.Facedthreebetpreflop
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
                    Occurred = Source.FoldedtothreebetpreflopCounter,
                    CouldOccurred = Source.FacedthreebetpreflopCounter
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
                    CouldOccurred = Source.Totalhands
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
                    CouldOccurred = Source.Sawflop
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
                    CouldOccurred = Source.SawTurn
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
                    CouldOccurred = Source.SawRiver
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
                    CouldOccurred = Source.DidThreeBetOop
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
                    CouldOccurred = Source.PfrOop
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
                    Occurred = Source.LimpEp,
                    CouldOccurred = Source.LimpPossible
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
                    Occurred = Source.LimpMp,
                    CouldOccurred = Source.LimpPossible
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
                    Occurred = Source.LimpCo,
                    CouldOccurred = Source.LimpPossible
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
                    Occurred = Source.LimpBtn,
                    CouldOccurred = Source.LimpPossible
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
                    Occurred = Source.LimpSb,
                    CouldOccurred = Source.LimpPossible
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

        #endregion

        #region Positional

        #region Unopened Pot

        public override decimal UO_PFR_EP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_EP, Source.PositionUnoppened?.EP);
            }
        }

        public override decimal UO_PFR_MP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_MP, Source.PositionUnoppened?.MP);
            }
        }

        public override decimal UO_PFR_CO
        {
            get
            {
                return GetPercentage(Source.UO_PFR_CO, Source.PositionUnoppened?.CO);
            }
        }

        public override decimal UO_PFR_BN
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BN, Source.PositionUnoppened?.BN);
            }
        }

        public override decimal UO_PFR_SB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_SB, Source.PositionUnoppened?.SB);
            }
        }

        public override decimal UO_PFR_BB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BB, Source.PositionUnoppened?.BB);
            }
        }

        public virtual StatDto UO_PFR_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = UO_PFR_EP,
                    Occurred = Source.UO_PFR_EP,
                    CouldOccurred = Source.PositionUnoppened?.EP ?? 0
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
                    CouldOccurred = Source.PositionUnoppened?.MP ?? 0
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
                    CouldOccurred = Source.PositionUnoppened?.CO ?? 0
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
                    CouldOccurred = Source.PositionUnoppened?.BN ?? 0
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
                    CouldOccurred = Source.PositionUnoppened?.SB ?? 0
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
                    CouldOccurred = Source.PositionUnoppened?.BB ?? 0
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
                    Occurred = Source.PositionVPIP?.EP ?? 0,
                    CouldOccurred = Source.PositionTotal?.EP ?? 0
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
                    Occurred = Source.PositionVPIP?.MP ?? 0,
                    CouldOccurred = Source.PositionTotal?.MP ?? 0
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
                    Occurred = Source.PositionVPIP?.CO ?? 0,
                    CouldOccurred = Source.PositionTotal?.CO ?? 0
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
                    Occurred = Source.PositionVPIP?.BN ?? 0,
                    CouldOccurred = Source.PositionTotal?.BN ?? 0
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
                    Occurred = Source.PositionVPIP?.SB ?? 0,
                    CouldOccurred = Source.PositionTotal?.SB ?? 0
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
                    Occurred = Source.PositionVPIP?.BB ?? 0,
                    CouldOccurred = Source.PositionTotal?.BB ?? 0
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
                    Occurred = Source.PositionDidColdCall?.EP ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.EP ?? 0
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
                    Occurred = Source.PositionDidColdCall?.MP ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.MP ?? 0
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
                    Occurred = Source.PositionDidColdCall?.CO ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.CO ?? 0
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
                    Occurred = Source.PositionDidColdCall?.BN ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.BN ?? 0
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
                    Occurred = Source.PositionDidColdCall?.SB ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.SB ?? 0
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
                    Occurred = Source.PositionDidColdCall?.BB ?? 0,
                    CouldOccurred = Source.PositionCouldColdCall?.BB ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.EP ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.EP ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.MP ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.MP ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.CO ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.CO ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.BN ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.BN ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.SB ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.SB ?? 0
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
                    Occurred = Source.PositionDidThreeBet?.BB ?? 0,
                    CouldOccurred = Source.PositionCouldThreeBet?.BB ?? 0
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
                    Occurred = Source.PositionDidFourBet?.BB ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.BB ?? 0,
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
                    Occurred = Source.PositionDidFourBet?.BN ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.BN ?? 0,
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
                    Occurred = Source.PositionDidFourBet?.CO ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.CO ?? 0,
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
                    Occurred = Source.PositionDidFourBet?.MP ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.MP ?? 0,
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
                    Occurred = Source.PositionDidFourBet?.EP ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.EP ?? 0,
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
                    Occurred = Source.PositionDidFourBet?.SB ?? 0,
                    CouldOccurred = Source.PositionCouldFourBet?.SB ?? 0,
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