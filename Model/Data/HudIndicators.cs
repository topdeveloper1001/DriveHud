using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Data
{
    /// <summary>
    /// This class holds all the indicators for displaying HUD data
    /// </summary>
    public class HudIndicators : Indicators
    {
        public HudIndicators() : base()
        { }

        public HudIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        { }

        public virtual StatDto VPIPObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP,
                    Occured = Source.Vpiphands,
                    CouldOccured = Source.Totalhands
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
                    Occured = Source.Pfrhands,
                    CouldOccured = Source.Totalhands
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
                    Occured = Source.Didthreebet,
                    CouldOccured = Source.Couldthreebet
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
                    Occured = Source.DidThreeBetIp,
                    CouldOccured = Source.Couldthreebet
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
                    Occured = Source.DidThreeBetOop,
                    CouldOccured = Source.Couldthreebet
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
                    Occured = Source.Didfourbet,
                    CouldOccured = Source.Couldfourbet
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
                    Occured = Source.Calledthreebetpreflop,
                    CouldOccured = Source.Facedthreebetpreflop
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
                    Occured = Source.Flopcontinuationbetmade,
                    CouldOccured = Source.Flopcontinuationbetpossible
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
                    Occured = Source.Turncontinuationbetmade,
                    CouldOccured = Source.Turncontinuationbetpossible
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
                    Occured = Source.StealMade,
                    CouldOccured = Source.StealPossible
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
                    Occured = Source.Foldedtothreebetpreflop,
                    CouldOccured = Source.Facedthreebetpreflop
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
                    Occured = Source.Bigblindstealreraised + Source.Smallblindstealreraised,
                    CouldOccured = Source.Bigblindstealattempted + Source.Smallblindstealattempted
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
                    Occured = Source.Bigblindstealfolded + Source.Smallblindstealfolded,
                    CouldOccured = Source.Bigblindstealattempted + Source.Smallblindstealattempted
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
                    Occured = Source.Wonshowdown,
                    CouldOccured = Source.Sawshowdown
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
                    Occured = Source.Wonshowdown,
                    CouldOccured = Source.Sawflop
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
                    Occured = Source.Sawshowdown,
                    CouldOccured = Source.Sawflop
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
                    Occured = Source.Totalbets,
                    CouldOccured = Source.Totalpostflopstreetsseen
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
                    Occured = Source.TotalAggressiveBets,
                    CouldOccured = Source.Totalpostflopstreetsseen - Source.Flopcontinuationbetmade
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
                    Occured = Source.Wonhandwhensawflop,
                    CouldOccured = Source.Sawflop
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
                    Occured = Source.Didcoldcall,
                    CouldOccured = Source.Couldcoldcall
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
                    Occured = Source.Flopcontinuationbetmade,
                    CouldOccured = Source.Calledflopcontinuationbet
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
                    Occured = Source.Turncontinuationbetmade,
                    CouldOccured = Source.Calledturncontinuationbet
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
                    Occured = Source.Turncontinuationbetmade,
                    CouldOccured = Source.Calledturncontinuationbet
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
                    Occured = Source.Foldedtoflopcontinuationbet,
                    CouldOccured = Source.Facingflopcontinuationbet
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
                    Occured = Source.Foldedtofourbetpreflop,
                    CouldOccured = Source.Facedfourbetpreflop
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
                    Occured = Source.Didsqueeze,
                    CouldOccured = Source.Totalhands
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
                    Occured = Source.DidCheckRaise,
                    CouldOccured = Source.Totalhands
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
                    Occured = Source.DidFlopCheckRaise,
                    CouldOccured = Source.Sawflop
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
                    Occured = Source.DidTurnCheckRaise,
                    CouldOccured = Source.SawTurn
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
                    Occured = Source.DidRiverCheckRaise,
                    CouldOccured = Source.SawRiver
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
                    Occured = Source.PlayedFloatFlop,
                    CouldOccured = Source.Facingflopcontinuationbet
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
                    Occured = Source.DidRaiseFlop,
                    CouldOccured = Source.CouldRaiseFlop
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
                    Occured = Source.DidRaiseTurn,
                    CouldOccured = Source.CouldRaiseTurn
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
                    Occured = Source.DidRaiseRiver,
                    CouldOccured = Source.CouldRaiseRiver
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
                    Occured = Source.DidRaiseFlop + Source.DidRaiseTurn + Source.DidRaiseRiver,
                    CouldOccured = Source.CouldRaiseFlop + Source.CouldRaiseTurn + Source.CouldRaiseRiver
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
                    Occured = Source.SawTurn,
                    CouldOccured = Source.WasTurn
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
                    Occured = Source.SawRiver,
                    CouldOccured = Source.WasRiver
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
                    Occured = Source.DidThreeBetVsSteal,
                    CouldOccured = Source.CouldThreeBetVsSteal
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
                    Occured = Source.DidCheckRiverOnBXLine,
                    CouldOccured = Source.CouldCheckRiverOnBXLine
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
                    Occured = Source.Flopcontinuationipbetmade,
                    CouldOccured = Source.Flopcontinuationbetpossible
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
                    Occured = Source.Flopcontinuationoopbetmade,
                    CouldOccured = Source.Flopcontinuationbetpossible
                };
            }
        }

        public virtual StatDto ThreeBetInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInBB,
                    Occured = Source.DidThreeBetInBb,
                    CouldOccured = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto ThreeBetInBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInBTN,
                    Occured = Source.DidThreeBetInBtn,
                    CouldOccured = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto ThreeBetInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInCO,
                    Occured = Source.DidThreeBetInCo,
                    CouldOccured = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto ThreeBetInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInMP,
                    Occured = Source.DidThreeBetInMp,
                    CouldOccured = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto ThreeBetInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBetInSB,
                    Occured = Source.DidThreeBetInSb,
                    CouldOccured = Source.Couldthreebet
                };
            }
        }

        public virtual StatDto FourBetInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = FourBetInBB,
                    Occured = Source.DidFourBetInBb,
                    CouldOccured = Source.Couldfourbet
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
                    Occured = Source.DidFourBetInBtn,
                    CouldOccured = Source.Couldfourbet
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
                    Occured = Source.DidFourBetInCo,
                    CouldOccured = Source.Couldfourbet
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
                    Occured = Source.DidFourBetInMp,
                    CouldOccured = Source.Couldfourbet
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
                    Occured = Source.DidFourBetInSb,
                    CouldOccured = Source.Couldfourbet
                };
            }
        }

        public virtual StatDto ColdCallInBBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallInBB,
                    Occured = Source.DidColdCallInBb,
                    CouldOccured = Source.Couldcoldcall
                };
            }
        }

        public virtual StatDto ColdCallInBTNObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallInBTN,
                    Occured = Source.DidColdCallInBtn,
                    CouldOccured = Source.Couldcoldcall
                };
            }
        }

        public virtual StatDto ColdCallInCOObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallInCO,
                    Occured = Source.DidColdCallInCo,
                    CouldOccured = Source.Couldcoldcall
                };
            }
        }

        public virtual StatDto ColdCallInMPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallInMP,
                    Occured = Source.DidColdCallInMp,
                    CouldOccured = Source.Couldcoldcall
                };
            }
        }

        public virtual StatDto ColdCallInSBObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCallInSB,
                    Occured = Source.DidColdCallInSb,
                    CouldOccured = Source.Couldcoldcall
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
                    Occured = Source.DidDonkBet,
                    CouldOccured = Source.CouldDonkBet
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
                    Occured = Source.DidDelayedTurnCBet,
                    CouldOccured = Source.CouldDelayedTurnCBet
                };
            }
        }

        #region Limp

        public virtual StatDto DidLimpObject
        {
            get
            {
                return new StatDto
                {
                    Value = DidLimp,
                    Occured = Source.LimpMade,
                    CouldOccured = Source.LimpPossible
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
                    Occured = Source.LimpCalled,
                    CouldOccured = Source.LimpFaced
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
                    Occured = Source.LimpFolded,
                    CouldOccured = Source.LimpFaced
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
                    Occured = Source.LimpReraised,
                    CouldOccured = Source.LimpFaced
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
                    Occured = Source.UO_PFR_EP,
                    CouldOccured = Source.PositionUnoppened?.EP ?? 0
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
                    Occured = Source.UO_PFR_MP,
                    CouldOccured = Source.PositionUnoppened?.MP ?? 0
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
                    Occured = Source.UO_PFR_CO,
                    CouldOccured = Source.PositionUnoppened?.CO ?? 0
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
                    Occured = Source.UO_PFR_BN,
                    CouldOccured = Source.PositionUnoppened?.BN ?? 0
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
                    Occured = Source.UO_PFR_SB,
                    CouldOccured = Source.PositionUnoppened?.SB ?? 0
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
                    Occured = Source.UO_PFR_BB,
                    CouldOccured = Source.PositionUnoppened?.BB ?? 0
                };
            }
        }

        #endregion

        #region Vpip

        public virtual decimal VPIP_EP
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.EP, Source.PositionTotal?.EP);
            }
        }

        public virtual decimal VPIP_MP
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.MP, Source.PositionTotal?.MP);
            }
        }

        public virtual decimal VPIP_CO
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.CO, Source.PositionTotal?.CO);
            }
        }

        public virtual decimal VPIP_BN
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.BN, Source.PositionTotal?.BN);
            }
        }

        public virtual decimal VPIP_SB
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.SB, Source.PositionTotal?.SB);
            }
        }

        public virtual decimal VPIP_BB
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.BB, Source.PositionTotal?.BB);
            }
        }

        public virtual StatDto VPIP_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = VPIP_EP,
                    Occured = Source.PositionVPIP?.EP ?? 0,
                    CouldOccured = Source.PositionTotal?.EP ?? 0
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
                    Occured = Source.PositionVPIP?.MP ?? 0,
                    CouldOccured = Source.PositionTotal?.MP ?? 0
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
                    Occured = Source.PositionVPIP?.CO ?? 0,
                    CouldOccured = Source.PositionTotal?.CO ?? 0
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
                    Occured = Source.PositionVPIP?.BN ?? 0,
                    CouldOccured = Source.PositionTotal?.BN ?? 0
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
                    Occured = Source.PositionVPIP?.SB ?? 0,
                    CouldOccured = Source.PositionTotal?.SB ?? 0
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
                    Occured = Source.PositionVPIP?.BB ?? 0,
                    CouldOccured = Source.PositionTotal?.BB ?? 0
                };
            }
        }

        #endregion

        #region ColdCall

        public virtual decimal ColdCall_EP
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.EP, Source.PositionCouldColdCall?.EP);
            }
        }
        public virtual decimal ColdCall_MP
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.MP, Source.PositionCouldColdCall?.MP);
            }
        }
        public virtual decimal ColdCall_CO
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.CO, Source.PositionCouldColdCall?.CO);
            }
        }
        public virtual decimal ColdCall_BN
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.BN, Source.PositionCouldColdCall?.BN);
            }
        }
        public virtual decimal ColdCall_SB
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.SB, Source.PositionCouldColdCall?.SB);
            }
        }
        public virtual decimal ColdCall_BB
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.BB, Source.PositionCouldColdCall?.BB);
            }
        }

        public virtual StatDto ColdCall_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ColdCall_EP,
                    Occured = Source.PositionDidColdCall?.EP ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.EP ?? 0
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
                    Occured = Source.PositionDidColdCall?.MP ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.MP ?? 0
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
                    Occured = Source.PositionDidColdCall?.CO ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.CO ?? 0
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
                    Occured = Source.PositionDidColdCall?.BN ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.BN ?? 0
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
                    Occured = Source.PositionDidColdCall?.SB ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.SB ?? 0
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
                    Occured = Source.PositionDidColdCall?.BB ?? 0,
                    CouldOccured = Source.PositionCouldColdCall?.BB ?? 0
                };
            }
        }

        #endregion

        #region 3Bet

        public virtual decimal ThreeBet_EP
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.EP, Source.PositionCouldColdCall?.EP);
            }
        }
        public virtual decimal ThreeBet_MP
        {
            get { return GetPercentage(Source.PositionDidThreeBet?.MP, Source.PositionCouldThreeBet?.MP); }
        }
        public virtual decimal ThreeBet_CO
        {
            get { return GetPercentage(Source.PositionDidThreeBet?.CO, Source.PositionCouldThreeBet?.CO); }
        }
        public virtual decimal ThreeBet_BN
        {
            get { return GetPercentage(Source.PositionDidThreeBet?.BN, Source.PositionCouldThreeBet?.BN); }
        }
        public virtual decimal ThreeBet_SB
        {
            get { return GetPercentage(Source.PositionDidThreeBet?.SB, Source.PositionCouldThreeBet?.SB); }
        }
        public virtual decimal ThreeBet_BB
        {
            get { return GetPercentage(Source.PositionDidThreeBet?.BB, Source.PositionCouldThreeBet?.BB); }
        }

        public virtual StatDto ThreeBet_EPObject
        {
            get
            {
                return new StatDto
                {
                    Value = ThreeBet_EP,
                    Occured = Source.PositionDidThreeBet?.EP ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.EP ?? 0
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
                    Occured = Source.PositionDidThreeBet?.MP ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.MP ?? 0
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
                    Occured = Source.PositionDidThreeBet?.CO ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.CO ?? 0
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
                    Occured = Source.PositionDidThreeBet?.BN ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.BN ?? 0
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
                    Occured = Source.PositionDidThreeBet?.SB ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.SB ?? 0
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
                    Occured = Source.PositionDidThreeBet?.BB ?? 0,
                    CouldOccured = Source.PositionCouldThreeBet?.BB ?? 0
                };
            }
        }

        #endregion

        #endregion

        #region CardsLists

        public virtual IEnumerable<string> ThreeBetCardsList
        {
            get
            {
                return Statistcs.SingleOrDefault(x => x.ThreeBetCardsList != null)?.ThreeBetCardsList;
            }
        }

        #endregion

        protected decimal GetPercentage(decimal? actual, decimal? possible)
        {
            if (TotalHands == 0)
                return 0;

            if (!possible.HasValue || !actual.HasValue || possible == 0)
                return 0;

            return (actual.Value / possible.Value) * 100;
        }

    }
}
