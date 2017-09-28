using System.Collections.Generic;

namespace AcePokerSolutions.DataAccessHelper.DriveHUD
{
    public class HudLightIndicators : Indicators
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

        //public virtual StatDto ThreeBetIPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBetIP,
        //            Occurred = Source.DidThreeBetIp,
        //            CouldOccurred = Source.CouldThreeBetIp
        //        };
        //    }
        //}

        //public virtual StatDto ThreeBetOOPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBetOOP,
        //            Occurred = Source.DidThreeBetOop,
        //            CouldOccurred = Source.CouldThreeBetOop
        //        };
        //    }
        //}

        //public virtual StatDto FourBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBet,
        //            Occurred = Source.Didfourbet,
        //            CouldOccurred = Source.Couldfourbet
        //        };
        //    }
        //}

        //public virtual StatDto ThreeBetCallObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBetCall,
        //            Occurred = Source.Calledthreebetpreflop,
        //            CouldOccurred = Source.Facedthreebetpreflop
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBet,
        //            Occurred = Source.Flopcontinuationbetmade,
        //            CouldOccurred = Source.Flopcontinuationbetpossible
        //        };
        //    }
        //}

        //public virtual StatDto TurnCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = TurnCBet,
        //            Occurred = Source.Turncontinuationbetmade,
        //            CouldOccurred = Source.Turncontinuationbetpossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetInThreeBetPotObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetInThreeBetPot,
        //            Occurred = Source.FlopContinuationBetInThreeBetPotMade,
        //            CouldOccurred = Source.FlopContinuationBetInThreeBetPotPossible
        //        };

        //    }
        //}

        //public virtual StatDto FlopCBetInFourBetPotObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetInFourBetPot,
        //            Occurred = Source.FlopContinuationBetInFourBetPotMade,
        //            CouldOccurred = Source.FlopContinuationBetInFourBetPotPossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetVsOneOppObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetVsOneOpp,
        //            Occurred = Source.FlopContinuationBetVsOneOpponentMade,
        //            CouldOccurred = Source.FlopContinuationBetVsOneOpponentPossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetVsTwoOppObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetVsTwoOpp,
        //            Occurred = Source.FlopContinuationBetVsTwoOpponentsMade,
        //            CouldOccurred = Source.FlopContinuationBetVsTwoOpponentsPossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetMWObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetMW,
        //            Occurred = Source.MultiWayFlopContinuationBetMade,
        //            CouldOccurred = Source.MultiWayFlopContinuationBetPossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetMonotoneObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetMonotone,
        //            Occurred = Source.FlopContinuationBetMonotonePotMade,
        //            CouldOccurred = Source.FlopContinuationBetMonotonePotPossible
        //        };
        //    }
        //}

        //public virtual StatDto FlopCBetRagObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCBetRag,
        //            Occurred = Source.FlopContinuationBetRagPotMade,
        //            CouldOccurred = Source.FlopContinuationBetRagPotPossible
        //        };
        //    }
        //}

        //public virtual StatDto FoldFlopCBetFromThreeBetPotObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldFlopCBetFromThreeBetPot,
        //            Occurred = Source.FoldedToFlopContinuationBetFromThreeBetPot,
        //            CouldOccurred = Source.FoldedToFlopContinuationBetFromFourBetPot,
        //        };
        //    }
        //}

        //public virtual StatDto FoldFlopCBetFromFourBetPotObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldFlopCBetFromFourBetPot,
        //            Occurred = Source.FoldedToFlopContinuationBetFromFourBetPot,
        //            CouldOccurred = Source.FoldedToFlopContinuationBetFromFourBetPot,
        //        };
        //    }
        //}

        //public virtual StatDto StealObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = Steal,
        //            Occurred = Source.StealMade,
        //            CouldOccurred = Source.StealPossible
        //        };
        //    }
        //}

        //public virtual StatDto FoldToThreeBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToThreeBet,
        //            Occurred = Source.Foldedtothreebetpreflop,
        //            CouldOccurred = Source.Facedthreebetpreflop
        //        };
        //    }
        //}

        //public virtual StatDto BlindsReraiseStealObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BlindsReraiseSteal,
        //            Occurred = Source.Bigblindstealreraised + Source.Smallblindstealreraised,
        //            CouldOccurred = Source.Bigblindstealfaced + Source.Smallblindstealfaced
        //        };
        //    }
        //}

        //public virtual StatDto BlindsFoldStealObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BlindsFoldSteal,
        //            Occurred = Source.Bigblindstealfolded + Source.Smallblindstealfolded,
        //            CouldOccurred = Source.Bigblindstealfaced + Source.Smallblindstealfaced
        //        };
        //    }
        //}

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

        //public virtual StatDto WWSFObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = WWSF,
        //            Occurred = Source.Wonhandwhensawflop,
        //            CouldOccurred = Source.Sawflop
        //        };
        //    }
        //}

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

        //public virtual StatDto TrueAggressionObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = TrueAggression,
        //            Occurred = Source.TotalAggressiveBets,
        //            CouldOccurred = Source.Totalpostflopstreetsplayed - Source.Flopcontinuationbetmade
        //        };
        //    }
        //}

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

        //public virtual StatDto ColdCallObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCall,
        //            Occurred = Source.Didcoldcall,
        //            CouldOccurred = Source.Couldcoldcall
        //        };
        //    }
        //}

        //public virtual StatDto FlopAggObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopAgg,
        //            Occurred = Source.TotalbetsFlop,
        //            CouldOccurred = Source.FlopAggPossible
        //        };
        //    }
        //}

        //public virtual StatDto TurnAggObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = TurnAgg,
        //            Occurred = Source.TotalbetsTurn,
        //            CouldOccurred = Source.TurnAggPossible
        //        };
        //    }
        //}

        //public virtual StatDto RiverAggObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RiverAgg,
        //            Occurred = Source.TotalbetsRiver,
        //            CouldOccurred = Source.RiverAggPossible
        //        };
        //    }
        //}

        //public virtual StatDto FoldCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldCBet,
        //            Occurred = Source.Foldedtoflopcontinuationbet,
        //            CouldOccurred = Source.Facingflopcontinuationbet
        //        };
        //    }
        //}

        //public virtual StatDto RaiseCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RaiseCBet,
        //            Occurred = Source.Raisedflopcontinuationbet,
        //            CouldOccurred = Source.Facingflopcontinuationbet
        //        };
        //    }
        //}

        //public virtual StatDto FoldToFourBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToFourBet,
        //            Occurred = Source.Foldedtofourbetpreflop,
        //            CouldOccurred = Source.Facedfourbetpreflop
        //        };
        //    }
        //}

        //public virtual StatDto SqueezeObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = Squeeze,
        //            Occurred = Source.Didsqueeze,
        //            CouldOccurred = Source.Totalhands
        //        };
        //    }
        //}

        //public virtual StatDto CheckRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CheckRaise,
        //            Occurred = Source.DidCheckRaise,
        //            CouldOccurred = Source.Totalhands
        //        };
        //    }
        //}

        //public virtual StatDto FlopCheckRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FlopCheckRaise,
        //            Occurred = Source.DidFlopCheckRaise,
        //            CouldOccurred = Source.Sawflop
        //        };
        //    }
        //}

        //public virtual StatDto TurnCheckRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = TurnCheckRaise,
        //            Occurred = Source.DidTurnCheckRaise,
        //            CouldOccurred = Source.SawTurn
        //        };
        //    }
        //}

        //public virtual StatDto RiverCheckRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RiverCheckRaise,
        //            Occurred = Source.DidRiverCheckRaise,
        //            CouldOccurred = Source.SawRiver
        //        };
        //    }
        //}

        //public virtual StatDto FloatFlopObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FloatFlop,
        //            Occurred = Source.PlayedFloatFlop,
        //            CouldOccurred = Source.Facingflopcontinuationbet
        //        };
        //    }
        //}

        //public virtual StatDto RaiseFlopObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RaiseFlop,
        //            Occurred = Source.DidRaiseFlop,
        //            CouldOccurred = Source.CouldRaiseFlop
        //        };
        //    }
        //}

        //public virtual StatDto RaiseTurnObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RaiseTurn,
        //            Occurred = Source.DidRaiseTurn,
        //            CouldOccurred = Source.CouldRaiseTurn
        //        };
        //    }
        //}

        //public virtual StatDto RaiseRiverObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RaiseRiver,
        //            Occurred = Source.DidRaiseRiver,
        //            CouldOccurred = Source.CouldRaiseRiver
        //        };
        //    }
        //}

        //public virtual StatDto RaiseFrequencyFactorObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RaiseFrequencyFactor,
        //            Occurred = Source.DidRaiseFlop + Source.DidRaiseTurn + Source.DidRaiseRiver,
        //            CouldOccurred = Source.CouldRaiseFlop + Source.CouldRaiseTurn + Source.CouldRaiseRiver
        //        };
        //    }
        //}

        //public virtual StatDto TurnSeenObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = TurnSeen,
        //            Occurred = Source.SawTurn,
        //            CouldOccurred = Source.WasTurn
        //        };
        //    }
        //}

        //public virtual StatDto RiverSeenObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = RiverSeen,
        //            Occurred = Source.SawRiver,
        //            CouldOccurred = Source.WasRiver
        //        };
        //    }
        //}

        //public virtual StatDto ThreeBetVsStealObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBetVsSteal,
        //            Occurred = Source.DidThreeBetVsSteal,
        //            CouldOccurred = Source.CouldThreeBetVsSteal
        //        };
        //    }
        //}

        //public virtual StatDto CheckRiverOnBXLineObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CheckRiverOnBXLine,
        //            Occurred = Source.DidCheckRiverOnBXLine,
        //            CouldOccurred = Source.CouldCheckRiverOnBXLine
        //        };
        //    }
        //}

        //public virtual StatDto CBetIPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CBetIP,
        //            Occurred = Source.Flopcontinuationipbetmade,
        //            CouldOccurred = Source.Flopcontinuationipbetpossible
        //        };
        //    }
        //}

        //public virtual StatDto CBetOOPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CBetOOP,
        //            Occurred = Source.Flopcontinuationoopbetmade,
        //            CouldOccurred = Source.Flopcontinuationoopbetpossible
        //        };
        //    }
        //}

        //public virtual StatDto DonkBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DonkBet,
        //            Occurred = Source.DidDonkBet,
        //            CouldOccurred = Source.CouldDonkBet
        //        };
        //    }
        //}

        //public virtual StatDto DidDelayedTurnCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DidDelayedTurnCBet,
        //            Occurred = Source.DidDelayedTurnCBet,
        //            CouldOccurred = Source.CouldDelayedTurnCBet
        //        };
        //    }
        //}

        //public virtual StatDto BTNDefendCORaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BTNDefendCORaise,
        //            Occurred = Source.Buttonstealdefended,
        //            CouldOccurred = Source.Buttonstealfaced
        //        };
        //    }
        //}

        //public virtual StatDto BetFlopCalled3BetPreflopIpObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BetFlopCalled3BetPreflopIp,
        //            Occurred = Source.BetFlopCalled3BetPreflopIp,
        //            CouldOccurred = Source.Totalhands
        //        };
        //    }
        //}

        //public virtual StatDto BetFoldFlopPfrRaiserObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BetFoldFlopPfrRaiser,
        //            Occurred = Source.BetFoldFlopPfrRaiser,
        //            CouldOccurred = Source.Pfrhands
        //        };
        //    }
        //}

        //public virtual StatDto CheckFoldFlop3BetOopObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CheckFoldFlop3BetOop,
        //            Occurred = Source.CheckFoldFlop3BetOop,
        //            CouldOccurred = Source.DidThreeBetOop
        //        };
        //    }
        //}

        //public virtual StatDto CheckFoldFlopPfrOopObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = CheckFoldFlopPfrOop,
        //            Occurred = Source.CheckFoldFlopPfrOop,
        //            CouldOccurred = Source.PfrOop
        //        };
        //    }
        //}

        //#endregion

        //#region Limp

        //public virtual StatDto DidLimpObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DidLimp,
        //            Occurred = Source.LimpMade,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto LimpEpObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = LimpEp,
        //            Occurred = Source.LimpEp,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto LimpMpObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = LimpMp,
        //            Occurred = Source.LimpMp,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto LimpCoObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = LimpCo,
        //            Occurred = Source.LimpCo,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto LimpBtnObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = LimpBtn,
        //            Occurred = Source.LimpBtn,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto LimpSbObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = LimpSb,
        //            Occurred = Source.LimpSb,
        //            CouldOccurred = Source.LimpPossible
        //        };
        //    }
        //}

        //public virtual StatDto DidLimpCallObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DidLimpCall,
        //            Occurred = Source.LimpCalled,
        //            CouldOccurred = Source.LimpFaced
        //        };
        //    }
        //}

        //public virtual StatDto DidLimpFoldObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DidLimpFold,
        //            Occurred = Source.LimpFolded,
        //            CouldOccurred = Source.LimpFaced
        //        };
        //    }
        //}

        //public virtual StatDto DidLimpReraiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = DidLimpReraise,
        //            Occurred = Source.LimpReraised,
        //            CouldOccurred = Source.LimpFaced
        //        };
        //    }
        //}

        //public virtual StatDto BetWhenCheckedToObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = BetWhenCheckedTo,
        //            Occurred = Source.DidBetWhenCheckedToFlop + Source.DidBetWhenCheckedToTurn + Source.DidBetWhenCheckedToRiver,
        //            CouldOccurred = Source.CanBetWhenCheckedToFlop + Source.CanBetWhenCheckedToTurn + Source.CanBetWhenCheckedToRiver,
        //        };
        //    }
        //}

        //public virtual StatDto FoldToFlopRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToFlopRaise,
        //            Occurred = Source.FoldedFacedRaiseFlop,
        //            CouldOccurred = Source.FacedRaiseFlop,
        //        };
        //    }
        //}

        //public virtual StatDto FoldToTurnRaiseObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToTurnRaise,
        //            Occurred = Source.FoldedFacedRaiseTurn,
        //            CouldOccurred = Source.FacedRaiseTurn,
        //        };
        //    }
        //}

        //public virtual StatDto FoldToRiverCBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToRiverCBet,
        //            Occurred = Source.Foldedtorivercontinuationbet,
        //            CouldOccurred = Source.Facingrivercontinuationbet
        //        };
        //    }
        //}

        //public virtual StatDto FoldToSqueezObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FoldToSqueez,
        //            Occurred = Source.FoldedFacedSqueez,
        //            CouldOccurred = Source.FacedSqueez,
        //        };
        //    }
        //}

        //#endregion

        //#region Positional


        //#region Positional VPIP

        ////public virtual StatDto VPIP_EPObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_EP,
        ////            Occurred = Source.PositionVPIP?.EP ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.EP ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto VPIP_MPObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_MP,
        ////            Occurred = Source.PositionVPIP?.MP ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.MP ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto VPIP_COObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_CO,
        ////            Occurred = Source.PositionVPIP?.CO ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.CO ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto VPIP_BNObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_BN,
        ////            Occurred = Source.PositionVPIP?.BN ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.BN ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto VPIP_SBObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_SB,
        ////            Occurred = Source.PositionVPIP?.SB ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.SB ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto VPIP_BBObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = VPIP_BB,
        ////            Occurred = Source.PositionVPIP?.BB ?? 0,
        ////            CouldOccurred = Source.PositionTotal?.BB ?? 0
        ////        };
        ////    }
        ////}

        ////#endregion

        ////#region Positional Cold Call

        ////public virtual StatDto ColdCall_EPObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_EP,
        ////            Occurred = Source.PositionDidColdCall?.EP ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.EP ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto ColdCall_MPObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_MP,
        ////            Occurred = Source.PositionDidColdCall?.MP ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.MP ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto ColdCall_COObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_CO,
        ////            Occurred = Source.PositionDidColdCall?.CO ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.CO ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto ColdCall_BNObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_BN,
        ////            Occurred = Source.PositionDidColdCall?.BN ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.BN ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto ColdCall_SBObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_SB,
        ////            Occurred = Source.PositionDidColdCall?.SB ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.SB ?? 0
        ////        };
        ////    }
        ////}

        ////public virtual StatDto ColdCall_BBObject
        ////{
        ////    get
        ////    {
        ////        return new StatDto
        ////        {
        ////            Value = ColdCall_BB,
        ////            Occurred = Source.PositionDidColdCall?.BB ?? 0,
        ////            CouldOccurred = Source.PositionCouldColdCall?.BB ?? 0
        ////        };
        ////    }
        ////}

        //public virtual StatDto ColdCallThreeBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCallThreeBet,
        //            Occurred = Source.DidColdCallThreeBet,
        //            CouldOccurred = Source.CouldColdCallThreeBet
        //        };
        //    }
        //}

        //public virtual StatDto ColdCallFourBetObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCallFourBet,
        //            Occurred = Source.DidColdCallFourBet,
        //            CouldOccurred = Source.CouldColdCallFourBet
        //        };
        //    }
        //}

        //public virtual StatDto ColdCallVsBtnOpenObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCallVsBtnOpen,
        //            Occurred = Source.DidColdCallVsOpenRaiseBtn,
        //            CouldOccurred = Source.CouldColdCallVsOpenRaiseBtn
        //        };
        //    }
        //}

        //public virtual StatDto ColdCallVsCoOpenObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCallVsCoOpen,
        //            Occurred = Source.DidColdCallVsOpenRaiseCo,
        //            CouldOccurred = Source.CouldColdCallVsOpenRaiseCo
        //        };
        //    }
        //}

        //public virtual StatDto ColdCallVsSbOpenObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ColdCallVsSbOpen,
        //            Occurred = Source.DidColdCallVsOpenRaiseSb,
        //            CouldOccurred = Source.CouldColdCallVsOpenRaiseSb
        //        };
        //    }
        ////}

        //#endregion

        //#region Positional 3-Bet  

        //public virtual StatDto ThreeBet_EPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_EP,
        //            Occurred = Source.PositionDidThreeBet?.EP ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.EP ?? 0
        //        };
        //    }
        //}
        //public virtual StatDto ThreeBet_MPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_MP,
        //            Occurred = Source.PositionDidThreeBet?.MP ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.MP ?? 0
        //        };
        //    }
        //}
        //public virtual StatDto ThreeBet_COObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_CO,
        //            Occurred = Source.PositionDidThreeBet?.CO ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.CO ?? 0
        //        };
        //    }
        //}
        //public virtual StatDto ThreeBet_BNObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_BN,
        //            Occurred = Source.PositionDidThreeBet?.BN ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.BN ?? 0
        //        };
        //    }
        //}
        //public virtual StatDto ThreeBet_SBObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_SB,
        //            Occurred = Source.PositionDidThreeBet?.SB ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.SB ?? 0
        //        };
        //    }
        //}
        //public virtual StatDto ThreeBet_BBObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = ThreeBet_BB,
        //            Occurred = Source.PositionDidThreeBet?.BB ?? 0,
        //            CouldOccurred = Source.PositionCouldThreeBet?.BB ?? 0
        //        };
        //    }
        //}

        //#endregion

        //#region Positional 4-Bet

        //public virtual StatDto FourBetInBBObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInBB,
        //            Occurred = Source.PositionDidFourBet?.BB ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.BB ?? 0,
        //        };
        //    }
        //}

        //public virtual StatDto FourBetInBTNObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInBTN,
        //            Occurred = Source.PositionDidFourBet?.BN ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.BN ?? 0,
        //        };
        //    }
        //}

        //public virtual StatDto FourBetInCOObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInCO,
        //            Occurred = Source.PositionDidFourBet?.CO ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.CO ?? 0,
        //        };
        //    }
        //}

        //public virtual StatDto FourBetInMPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInMP,
        //            Occurred = Source.PositionDidFourBet?.MP ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.MP ?? 0,
        //        };
        //    }
        //}

        //public virtual StatDto FourBetInEPObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInMP,
        //            Occurred = Source.PositionDidFourBet?.EP ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.EP ?? 0,
        //        };
        //    }
        //}

        //public virtual StatDto FourBetInSBObject
        //{
        //    get
        //    {
        //        return new StatDto
        //        {
        //            Value = FourBetInSB,
        //            Occurred = Source.PositionDidFourBet?.SB ?? 0,
        //            CouldOccurred = Source.PositionCouldFourBet?.SB ?? 0,
        //        };
        //    }
        //}

        //#endregion

        #endregion  

   
    }
}
