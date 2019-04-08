//-----------------------------------------------------------------------
// <copyright file="Indicators.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Importer;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Model.Data
{
    /// <summary>
    /// This entity holds all high level indicators. These indicators are based on PlayerStatistic fields
    /// </summary>
    [ProtoContract]
    [ProtoInclude(200, typeof(LightIndicators))]
    public abstract class Indicators : INotifyPropertyChanged, IComparable
    {
#pragma warning disable 0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 0067

        /// <summary>
        /// Initializes a new instance of <see cref="Indicators"/>
        /// </summary>
        public Indicators()
        {
            Source = new Playerstatistic();
            Statistics = new List<Playerstatistic>();
        }

        /// <summary>
        /// Initializes a new instance of <see cref="Indicators"/>
        /// </summary>
        public Indicators(IEnumerable<Playerstatistic> playerStatistic) : this()
        {
            playerStatistic.ForEach(x => AddStatistic(x));
        }

        [ProtoMember(1)]
        public Playerstatistic Source { get; set; }

        public List<Playerstatistic> Statistics { get; private set; }

        public virtual int StatisticsCount
        {
            get
            {
                return Statistics != null ? Statistics.Count : 0;
            }
        }

        public virtual List<DateTime> StatisticsTimeCollection
        {
            get
            {
                return Statistics != null ? Statistics.Select(x => x.Time).ToList() : new List<DateTime>();
            }
        }

        public virtual decimal BB
        {
            get
            {
                // to prevent errors - collection was modified
                var statistic = Statistics.ToArray();

                decimal totalhands = statistic.Count() / (decimal)100;
                decimal netwon = statistic.Sum(x => GetDivisionResult(x.NetWon, x.BigBlind));

                return Math.Round(GetDivisionResult(netwon, totalhands), 2);
            }
        }

        public virtual decimal VPIP
        {
            get
            {
                return GetPercentage(Source.Vpiphands, TotalHands - Source.NumberOfWalks);
            }
        }

        public virtual decimal PFR
        {
            get
            {
                return GetPercentage(Source.Pfrhands, TotalHands - Source.NumberOfWalks);
            }
        }

        public virtual decimal ThreeBet
        {
            get
            {
                return GetPercentage(Source.Didthreebet, Source.Couldthreebet);
            }
        }

        public virtual decimal ThreeBetIP
        {
            get
            {
                return GetPercentage(Source.DidThreeBetIp, Source.CouldThreeBetIp);
            }
        }

        public virtual decimal ThreeBetOOP
        {
            get
            {
                return GetPercentage(Source.DidThreeBetOop, Source.CouldThreeBetOop);
            }
        }

        public virtual decimal FourBet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.DidfourbetpreflopVirtual), Statistics.Sum(x => x.CouldfourbetpreflopVirtual));
            }
        }

        public virtual decimal FourBetRange
        {
            get
            {
                return (PFR * FourBet) / 100;
            }
        }

        public virtual decimal FiveBet
        {
            get
            {
                return GetPercentage(Source.Did5Bet, Source.Could5Bet);
            }
        }

        public virtual decimal FoldToFiveBet
        {
            get
            {
                return GetPercentage(Source.FoldedTo5Bet, Source.Faced5Bet);
            }
        }

        public virtual decimal ThreeBetCall
        {
            get
            {
                return GetPercentage(Source.Calledthreebetpreflop, Statistics.Sum(x => x.FacedthreebetpreflopVirtual));
            }
        }

        public virtual decimal FlopCBet
        {
            get
            {
                return GetPercentage(Source.Flopcontinuationbetmade, Source.Flopcontinuationbetpossible);
            }
        }

        public virtual decimal TurnCBet
        {
            get
            {
                return GetPercentage(Source.Turncontinuationbetmade, Source.Turncontinuationbetpossible);
            }
        }

        public virtual decimal FoldToTurnCBet
        {
            get
            {
                return GetPercentage(Source.Foldedtoturncontinuationbet, Source.Facingturncontinuationbet);
            }
        }

        public virtual decimal RiverCBet
        {
            get
            {
                return GetPercentage(Source.Rivercontinuationbetmade, Source.Rivercontinuationbetpossible);
            }
        }

        public virtual decimal FlopCBetInThreeBetPot
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetInThreeBetPotMade, Source.FlopContinuationBetInThreeBetPotPossible);
            }
        }

        public virtual decimal FlopCBetInFourBetPot
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetInFourBetPotMade, Source.FlopContinuationBetInFourBetPotPossible);
            }
        }

        public virtual decimal FlopCBetVsOneOpp
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetVsOneOpponentMade, Source.FlopContinuationBetVsOneOpponentPossible);
            }
        }

        public virtual decimal FlopCBetVsTwoOpp
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetVsTwoOpponentsMade, Source.FlopContinuationBetVsTwoOpponentsPossible);
            }
        }

        public virtual decimal FlopCBetMW
        {
            get
            {
                return GetPercentage(Source.MultiWayFlopContinuationBetMade, Source.MultiWayFlopContinuationBetPossible);
            }
        }

        public virtual decimal FlopCBetMonotone
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetMonotonePotMade, Source.FlopContinuationBetMonotonePotPossible);
            }
        }

        public virtual decimal FlopCBetRag
        {
            get
            {
                return GetPercentage(Source.FlopContinuationBetRagPotMade, Source.FlopContinuationBetRagPotPossible);
            }
        }

        public virtual decimal FoldFlopCBetFromThreeBetPot
        {
            get
            {
                return GetPercentage(Source.FoldedToFlopContinuationBetFromThreeBetPot, Source.FacingFlopContinuationBetFromThreeBetPot);
            }
        }

        public virtual decimal FoldFlopCBetFromFourBetPot
        {
            get
            {
                return GetPercentage(Source.FoldedToFlopContinuationBetFromFourBetPot, Source.FacingFlopContinuationBetFromFourBetPot);
            }
        }

        public virtual decimal Steal
        {
            get
            {
                return GetPercentage(Source.StealMade, Source.StealPossible);
            }
        }

        public virtual decimal FoldToThreeBet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FoldedtothreebetpreflopVirtual), Statistics.Sum(x => x.FacedthreebetpreflopVirtual));
            }
        }

        public virtual decimal BlindsReraiseSteal
        {
            get
            {
                var possible = Source.Bigblindstealfaced + Source.Smallblindstealfaced;
                var made = Source.Bigblindstealreraised + Source.Smallblindstealreraised;
                return GetPercentage(made, possible);
            }
        }

        public virtual decimal BlindsFoldSteal
        {
            get
            {
                var possible = Source.Bigblindstealfaced + Source.Smallblindstealfaced;
                var made = Source.Bigblindstealfolded + Source.Smallblindstealfolded;
                return GetPercentage(made, possible);
            }
        }

        public virtual decimal WSSD
        {
            get
            {
                return GetPercentage(Source.Wonshowdown, Source.Sawshowdown);
            }
        }

        public virtual decimal WWSF
        {
            get
            {
                return GetPercentage(Source.Wonhandwhensawflop, Source.Sawflop);
            }
        }

        public virtual decimal WTSD
        {
            get
            {
                return GetPercentage(Source.Sawshowdown, Source.Sawflop);
            }
        }

        public virtual decimal Agg
        {
            get
            {
                if (Source.Totalcalls > 0)
                {
                    return (Convert.ToDecimal(Source.Totalbets) / Convert.ToDecimal(Source.Totalcalls));
                }
                else
                {
                    return Source.Totalbets * 2m;
                }
            }
        }

        public virtual decimal TurnAF
        {
            get
            {
                if (Source.TotalcallsTurn > 0)
                {
                    return (Convert.ToDecimal(Source.TotalbetsTurn) / Convert.ToDecimal(Source.TotalcallsTurn));
                }
                else
                {
                    return Source.TotalbetsTurn * 2m;
                }
            }
        }

        public virtual decimal AggPr
        {
            get
            {
                return GetPercentage(Source.Totalbets, Source.Totalpostflopstreetsplayed);
            }
        }

        public virtual decimal TrueAggression
        {
            get
            {
                return GetPercentage(Source.TotalAggressiveBets, Source.Totalpostflopstreetsplayed - Source.Flopcontinuationbetmade);
            }
        }

        public virtual decimal WSWSF
        {
            get
            {
                return GetPercentage(Source.Wonhandwhensawflop, Source.Sawflop);
            }
        }

        public virtual decimal CalledCheckRaiseVsFlopCBet
        {
            get
            {
                return GetPercentage(Source.CalledCheckRaiseVsFlopCBet, Source.FacedCheckRaiseVsFlopCBet);
            }
        }

        public virtual decimal FoldedCheckRaiseVsFlopCBet
        {
            get
            {
                return GetPercentage(Source.FoldedCheckRaiseVsFlopCBet, Source.FacedCheckRaiseVsFlopCBet);
            }
        }

        public virtual decimal CheckFlopAsPFRAndXCOnTurnOOP
        {
            get
            {
                return GetPercentage(Source.CheckedCalledTurnWhenCheckedFlopAsPfr, Source.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP);
            }
        }

        public virtual decimal CheckFlopAsPFRAndXFOnTurnOOP
        {
            get
            {
                return GetPercentage(Source.CheckedFoldedToTurnWhenCheckedFlopAsPfr, Source.FacedTurnBetAfterCheckWhenCheckedFlopAsPfrOOP);
            }
        }

        public virtual decimal CheckFlopAsPFRAndCallOnTurn
        {
            get
            {
                return GetPercentage(Source.CalledTurnBetWhenCheckedFlopAsPfr, Source.FacedTurnBetWhenCheckedFlopAsPfr);
            }
        }

        public virtual decimal CheckFlopAsPFRAndFoldOnTurn
        {
            get
            {
                return GetPercentage(Source.FoldedToTurnBetWhenCheckedFlopAsPfr, Source.FacedTurnBetWhenCheckedFlopAsPfr);
            }
        }

        public virtual decimal CheckFlopAsPFRAndRaiseOnTurn
        {
            get
            {
                return GetPercentage(Source.RaisedTurnBetWhenCheckedFlopAsPfr, Source.FacedTurnBetWhenCheckedFlopAsPfr);
            }
        }

        public virtual decimal TotalHands
        {
            get { return Source.Totalhands; }
        }

        public virtual decimal NumberOfWalks
        {
            get { return Source.NumberOfWalks; }
        }

        public virtual decimal TotalWon
        {
            get { return Source.NetWon; }
        }

        public virtual decimal AveragePot
        {
            get
            {
                return Source.Totalhands != 0 ? Source.TotalPot / Source.Totalhands : 0;
            }
        }

        #region Session

        public virtual DateTime? SessionStart
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return null;
                }

                return Statistics.Min(x => x.Time);
            }
        }

        public virtual string SessionLength
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return string.Empty;
                }

                var length = Statistics.Max(x => x.Time) - Statistics.Min(x => x.Time);
                return string.Format("{0}:{1:00}", (int)length.TotalHours, length.Minutes);
            }
        }

        #endregion

        public virtual decimal ColdCall
        {
            get
            {
                return GetPercentage(Source.Didcoldcall, Source.Couldcoldcall);
            }
        }

        public virtual decimal FlopAgg
        {
            get
            {
                return GetPercentage(Source.TotalbetsFlop, Source.FlopAggPossible);
            }
        }

        public virtual decimal TurnAgg
        {
            get
            {
                return GetPercentage(Source.TotalbetsTurn, Source.TurnAggPossible);
            }
        }

        public virtual decimal RiverAgg
        {
            get
            {
                return GetPercentage(Source.TotalbetsRiver, Source.RiverAggPossible);
            }
        }

        public virtual decimal FoldCBet
        {
            get
            {
                return GetPercentage(Source.Foldedtoflopcontinuationbet, Source.Facingflopcontinuationbet);
            }
        }

        public virtual decimal RaiseCBet
        {
            get
            {
                return GetPercentage(Source.Raisedflopcontinuationbet, Source.Facingflopcontinuationbet);
            }
        }

        public virtual decimal FoldToFourBet
        {
            get
            {
                return GetPercentage(Source.Foldedtofourbetpreflop, Source.Facedfourbetpreflop);
            }
        }

        public virtual decimal Squeeze
        {
            get
            {
                return GetPercentage(Source.Didsqueeze, Source.Couldsqueeze);
            }
        }

        public virtual decimal CheckRaise
        {
            get
            {
                var couldCheckRaise = Source.CouldFlopCheckRaise + Source.CouldTurnCheckRaise + Source.CouldRiverCheckRaise;

                if (couldCheckRaise == 0)
                {
                    couldCheckRaise = (int)TotalHands;
                }

                return GetPercentage(Source.DidFlopCheckRaise + Source.DidTurnCheckRaise + Source.DidRiverCheckRaise, couldCheckRaise);
            }
        }

        public virtual decimal FlopCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidFlopCheckRaise, Source.CouldFlopCheckRaise != 0 ? Source.CouldFlopCheckRaise : Source.Sawflop);
            }
        }

        public virtual decimal TurnCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidTurnCheckRaise, Source.CouldTurnCheckRaise != 0 ? Source.CouldTurnCheckRaise : Source.SawTurn);
            }
        }

        public virtual decimal RiverCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidRiverCheckRaise, Source.CouldRiverCheckRaise != 0 ? Source.CouldRiverCheckRaise : Source.SawRiver);
            }
        }

        public virtual decimal FloatFlop
        {
            get
            {
                return GetPercentage(Source.PlayedFloatFlop, Source.Facingflopcontinuationbet);
            }
        }

        public virtual decimal RaiseFlop
        {
            get
            {
                return GetPercentage(Source.DidRaiseFlop, Source.CouldRaiseFlop);
            }
        }

        public virtual decimal RaiseTurn
        {
            get
            {
                return GetPercentage(Source.DidRaiseTurn, Source.CouldRaiseTurn);
            }
        }

        public virtual decimal RaiseRiver
        {
            get
            {
                return GetPercentage(Source.DidRaiseRiver, Source.CouldRaiseRiver);
            }
        }

        public virtual decimal RaiseFrequencyFactor
        {
            get
            {
                return GetPercentage(Source.DidRaiseFlop + Source.DidRaiseTurn + Source.DidRaiseRiver,
                    Source.CouldRaiseFlop + Source.CouldRaiseTurn + Source.CouldRaiseRiver);
            }
        }

        public virtual decimal TurnSeen
        {
            get
            {
                return GetPercentage(Source.SawTurn, Source.WasTurn);
            }
        }

        public virtual decimal RiverSeen
        {
            get
            {
                return GetPercentage(Source.SawRiver, Source.WasRiver);
            }
        }

        public virtual decimal ThreeBetVsSteal
        {
            get
            {
                return GetPercentage(Source.DidThreeBetVsSteal, Source.CouldThreeBetVsSteal);
            }
        }

        public virtual decimal CheckRiverOnBXLine
        {
            get
            {
                return GetPercentage(Source.DidCheckRiverOnBXLine, Source.CouldCheckRiverOnBXLine);
            }
        }

        public virtual decimal CheckRiverAfterBBLine
        {
            get
            {
                return GetPercentage(Source.CheckedRiverAfterBBLine, Source.CouldCheckRiverAfterBBLine);
            }
        }

        public virtual decimal BetRiverOnBXLine
        {
            get
            {
                return GetPercentage(Source.DidBetRiverOnBXLine, Source.CouldBetRiverOnBXLine);
            }
        }

        public virtual decimal RiverBet
        {
            get
            {
                return GetPercentage(Source.DidRiverBet, Source.CouldRiverBet);
            }
        }

        public virtual decimal TurnBet
        {
            get
            {
                return GetPercentage(Source.DidTurnBet, Source.CouldTurnBet);
            }
        }

        public virtual decimal FlopBet
        {
            get
            {
                return GetPercentage(Source.DidFlopBet, Source.CouldFlopBet);
            }
        }

        public virtual decimal CBetIP
        {
            get
            {
                return GetPercentage(Source.Flopcontinuationipbetmade, Source.Flopcontinuationipbetpossible);
            }
        }

        public virtual decimal CBetOOP
        {
            get
            {
                return GetPercentage(Source.Flopcontinuationoopbetmade, Source.Flopcontinuationoopbetpossible);
            }
        }

        public virtual decimal ColdCallVsBtnOpen
        {
            get
            {
                return GetPercentage(Source.DidColdCallVsOpenRaiseBtn, Source.CouldColdCallVsOpenRaiseBtn);
            }
        }

        public virtual decimal ColdCallVsCoOpen
        {
            get
            {
                return GetPercentage(Source.DidColdCallVsOpenRaiseCo, Source.CouldColdCallVsOpenRaiseCo);
            }
        }

        public virtual decimal ColdCallVsSbOpen
        {
            get
            {
                return GetPercentage(Source.DidColdCallVsOpenRaiseSb, Source.CouldColdCallVsOpenRaiseSb);
            }
        }

        public virtual decimal DonkBet
        {
            get
            {
                return GetPercentage(Source.DidDonkBet, Source.CouldDonkBet);
            }
        }

        public virtual decimal DidDelayedTurnCBet
        {
            get
            {
                return GetPercentage(Source.DidDelayedTurnCBet, Source.CouldDelayedTurnCBet);
            }
        }

        public virtual decimal DelayedTurnCBetIP
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.DidDelayedTurnCBetIP), Statistics.Sum(x => x.CouldDelayedTurnCBetIP));
            }
        }

        public virtual decimal DelayedTurnCBetOOP
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.DidDelayedTurnCBetOOP), Statistics.Sum(x => x.CouldDelayedTurnCBetOOP));
            }
        }

        public virtual decimal DelayedTurnCBetIn3BetPot
        {
            get
            {
                return GetPercentage(Source.DidDelayedTurnCBetIn3BetPot, Source.CouldDelayedTurnCBetIn3BetPot);
            }
        }

        public virtual decimal MRatio
        {
            get
            {
                return Source.MRatio;
            }
        }

        public virtual decimal StackInBBs
        {
            get
            {
                return Source.StackInBBs;
            }
        }

        public virtual decimal ShovedFlopAfter4Bet
        {
            get
            {
                return GetPercentage(Source.ShovedFlopAfter4Bet, Source.CouldShoveFlopAfter4Bet);
            }
        }

        public virtual decimal RaiseFlopCBetIn3BetPot
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.RaisedFlopCBetIn3BetPot), Statistics.Sum(x => x.CouldRaiseFlopCBetIn3BetPot));
            }
        }

        public virtual decimal FoldToThreeBetIP
        {
            get
            {
                return GetPercentage(Source.FoldToThreeBetIP, Source.FacedThreeBetIP);
            }
        }

        public virtual decimal FoldToThreeBetOOP
        {
            get
            {
                return GetPercentage(Source.FoldToThreeBetOOP, Source.FacedThreeBetOOP);
            }
        }

        public virtual decimal BetFlopWhenCheckedToSRP
        {
            get
            {
                return GetPercentage(Source.BetFlopWhenCheckedToSRP, Source.CouldBetFlopWhenCheckedToSRP);
            }
        }

        #region Unopened PFR

        public virtual decimal UO_PFR_EP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_EP,
                    Statistics.Count(x => x.PositionString == "EP" && x.IsUnopened));
            }
        }
        public virtual decimal UO_PFR_MP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_MP,
                    Statistics.Count(x => x.PositionString == "MP" && x.IsUnopened));
            }
        }
        public virtual decimal UO_PFR_CO
        {
            get
            {
                return GetPercentage(Source.UO_PFR_CO,
                    Statistics.Count(x => x.PositionString == "CO" && x.IsUnopened));
            }
        }
        public virtual decimal UO_PFR_BN
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BN,
                    Statistics.Count(x => x.PositionString == "BTN" && x.IsUnopened));
            }
        }
        public virtual decimal UO_PFR_SB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_SB,
                    Statistics.Count(x => x.PositionString == "SB" && x.IsUnopened));
            }
        }
        public virtual decimal UO_PFR_BB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BB,
                    Statistics.Count(x => x.PositionString == "BB" && x.IsUnopened));
            }
        }

        public virtual decimal PFRInEP
        {
            get { return GetPercentage(Source.PfrInEp, TotalHands); }
        }

        public virtual decimal PFRInMP
        {
            get { return GetPercentage(Source.PfrInMp, TotalHands); }
        }

        public virtual decimal PFRInCO
        {
            get { return GetPercentage(Source.PfrInCo, TotalHands); }
        }

        public virtual decimal PFRInBTN
        {
            get { return GetPercentage(Source.PfrInBtn, TotalHands); }
        }

        public virtual decimal PFRInBB
        {
            get { return GetPercentage(Source.PfrInBb, TotalHands); }
        }

        public virtual decimal PFRInSB
        {
            get { return GetPercentage(Source.PfrInSb, TotalHands); }
        }

        #endregion

        #region Positional VPIP

        public virtual decimal VPIP_EP
        {
            get
            {
                return GetPositionalPercentage("EP", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        public virtual decimal VPIP_MP
        {
            get
            {
                return GetPositionalPercentage("MP", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        public virtual decimal VPIP_CO
        {
            get
            {
                return GetPositionalPercentage("CO", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        public virtual decimal VPIP_BN
        {
            get
            {
                return GetPositionalPercentage("BTN", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        public virtual decimal VPIP_SB
        {
            get
            {
                return GetPositionalPercentage("SB", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        public virtual decimal VPIP_BB
        {
            get
            {
                return GetPositionalPercentage("BB", x => x.Vpiphands, x => x.Totalhands);
            }
        }

        #endregion

        #region Positional 3-Bet

        public virtual decimal ThreeBet_EP
        {
            get
            {
                return GetPositionalPercentage("EP", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        public virtual decimal ThreeBet_MP
        {
            get
            {
                return GetPositionalPercentage("MP", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        public virtual decimal ThreeBet_CO
        {
            get
            {
                return GetPositionalPercentage("CO", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        public virtual decimal ThreeBet_BN
        {
            get
            {
                return GetPositionalPercentage("BTN", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        public virtual decimal ThreeBet_SB
        {
            get
            {
                return GetPositionalPercentage("SB", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        public virtual decimal ThreeBet_BB
        {
            get
            {
                return GetPositionalPercentage("BB", x => x.Didthreebet, x => x.Couldthreebet);
            }
        }

        #endregion

        #region Positional 4-Bet

        public virtual decimal FourBetInBB
        {
            get
            {
                return GetPercentage(positionDidFourBet?.BB, positionCouldFourBet?.BB);
            }
        }

        public virtual decimal FourBetInBTN
        {
            get
            {
                return GetPercentage(positionDidFourBet?.BN, positionCouldFourBet?.BN);
            }
        }

        public virtual decimal FourBetInCO
        {
            get
            {
                return GetPercentage(positionDidFourBet?.CO, positionCouldFourBet?.CO);
            }
        }

        public virtual decimal FourBetInMP
        {
            get
            {
                return GetPercentage(positionDidFourBet?.MP, positionCouldFourBet?.MP);
            }
        }

        public virtual decimal FourBetInEP
        {
            get
            {
                return GetPercentage(positionDidFourBet?.EP, positionCouldFourBet?.EP);
            }
        }

        public virtual decimal FourBetInSB
        {
            get
            {
                return GetPercentage(positionDidFourBet?.SB, positionCouldFourBet?.SB);
            }
        }

        #endregion

        #region Positional Cold call

        public virtual decimal ColdCall_EP
        {
            get
            {
                return GetPositionalPercentage("EP", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }
        public virtual decimal ColdCall_MP
        {
            get
            {
                return GetPositionalPercentage("MP", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }
        public virtual decimal ColdCall_CO
        {
            get
            {
                return GetPositionalPercentage("CO", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }
        public virtual decimal ColdCall_BN
        {
            get
            {
                return GetPositionalPercentage("BTN", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }
        public virtual decimal ColdCall_SB
        {
            get
            {
                return GetPositionalPercentage("SB", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }
        public virtual decimal ColdCall_BB
        {
            get
            {
                return GetPositionalPercentage("BB", x => x.Didcoldcall, x => x.Couldcoldcall);
            }
        }

        #endregion

        #region Positional Cold call 3-Bet

        public virtual decimal ColdCall3BetInBB => GetPercentage(positionDidColdCallThreeBet?.BB, positionCouldColdCallThreeBet?.BB);

        public virtual decimal ColdCall3BetInSB => GetPercentage(positionDidColdCallThreeBet?.SB, positionCouldColdCallThreeBet?.SB);

        public virtual decimal ColdCall3BetInMP => GetPercentage(positionDidColdCallThreeBet?.MP, positionCouldColdCallThreeBet?.MP);

        public virtual decimal ColdCall3BetInCO => GetPercentage(positionDidColdCallThreeBet?.CO, positionCouldColdCallThreeBet?.CO);

        public virtual decimal ColdCall3BetInBTN => GetPercentage(positionDidColdCallThreeBet?.BN, positionCouldColdCallThreeBet?.BN);

        #endregion

        #region Positional Cold call 4-Bet

        public virtual decimal ColdCall4BetInBB => GetPercentage(positionDidColdCallFourBet?.BB, positionCouldColdCallFourBet?.BB);

        public virtual decimal ColdCall4BetInSB => GetPercentage(positionDidColdCallFourBet?.SB, positionCouldColdCallFourBet?.SB);

        public virtual decimal ColdCall4BetInMP => GetPercentage(positionDidColdCallFourBet?.MP, positionCouldColdCallFourBet?.MP);

        public virtual decimal ColdCall4BetInCO => GetPercentage(positionDidColdCallFourBet?.CO, positionCouldColdCallFourBet?.CO);

        public virtual decimal ColdCall4BetInBTN => GetPercentage(positionDidColdCallFourBet?.BN, positionCouldColdCallFourBet?.BN);

        #endregion

        #region Positional Open minraise UO-PFR

        public virtual decimal OpenMinraise => GetPercentage(Statistics.Sum(x => x.OpenMinraise), TotalHands - Source.NumberOfWalks);

        public virtual decimal EPOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.EP, positionUnoppened?.EP);

        public virtual decimal MPOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.MP, positionUnoppened?.MP);

        public virtual decimal COOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.CO, positionUnoppened?.CO);

        public virtual decimal BTNOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.BN, positionUnoppened?.BN);

        public virtual decimal SBOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.SB, positionUnoppened?.SB);

        public virtual decimal BBOpenMinraiseUOPFR => GetPercentage(positionOpenMinraiseUOPFR?.BB, positionUnoppened?.BB);

        #endregion

        public virtual decimal TiltMeter
        {
            get
            {
                return GetPercentage(Source.TiltMeterPermanent + Source.TiltMeterTemporaryHistory.Sum(), 18);
            }
        }

        #region Limp

        public virtual decimal DidLimp
        {
            get { return GetPercentage(Source.LimpMade, Source.LimpPossible); }
        }

        public virtual decimal LimpEp
        {
            get { return GetPercentage(positionLimpMade?.EP, positionLimpPossible?.EP); }
        }

        public virtual decimal LimpMp
        {
            get { return GetPercentage(positionLimpMade?.MP, positionLimpPossible?.MP); }
        }

        public virtual decimal LimpCo
        {
            get { return GetPercentage(positionLimpMade?.CO, positionLimpPossible?.CO); }
        }

        public virtual decimal LimpBtn
        {
            get { return GetPercentage(positionLimpMade?.BN, positionLimpPossible?.BN); }
        }

        public virtual decimal LimpSb
        {
            get { return GetPercentage(positionLimpMade?.SB, positionLimpPossible?.SB); }
        }

        public virtual decimal DidLimpCall
        {
            get { return GetPercentage(Source.LimpCalled, Source.LimpFaced); }
        }

        public virtual decimal DidLimpFold
        {
            get { return GetPercentage(Source.LimpFolded, Source.LimpFaced); }
        }

        public virtual decimal DidLimpReraise
        {
            get { return GetPercentage(Source.LimpReraised, Source.LimpFaced); }
        }

        #endregion

        public virtual decimal CheckFoldFlopPfrOop => GetPercentage(Source.CheckFoldFlopPfrOop, Statistics.Sum(x => x.CouldCheckFoldFlopPfrOop));

        public virtual decimal CheckFoldFlop3BetOop => GetPercentage(Source.CheckFoldFlop3BetOop, Statistics.Sum(x => x.CouldCheckFoldFlop3BetOop));

        public virtual decimal BetFoldFlopPfrRaiser => GetPercentage(Source.BetFoldFlopPfrRaiser, Source.CouldBetFoldFlopPfrRaiser);

        public virtual decimal BetFlopCalled3BetPreflopIp => GetPercentage(Source.BetFlopCalled3BetPreflopIp, Source.CouldBetFlopCalled3BetPreflopIp);

        public virtual decimal BTNDefendCORaise
        {
            get
            {
                return GetPercentage(Source.Buttonstealdefended, Source.Buttonstealfaced);
            }
        }

        public virtual string GameType { get; set; }

        public virtual string PokerSite
        {
            get
            {
                return ((EnumPokerSites)Source.PokersiteId).ToString();
            }
        }

        public virtual string HourOfHand
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return string.Empty;
                }

                var sessionStartHour = Converter.ToLocalizedDateTime(Statistics.Min(x => x.Time)).Hour;
                var sessionEndHour = Converter.ToLocalizedDateTime(Statistics.Max(x => x.Time)).Hour;

                return string.Format("{0}:00 - {1}:59", sessionStartHour, sessionEndHour);
            }
        }

        public virtual decimal EVDiff
        {
            get
            {
                return Statistics.Sum(x => x.EVDiff);
            }
        }

        public virtual decimal EVBB
        {
            get
            {
                // to prevent errors - collection was modified
                var statistic = Statistics.ToArray();

                decimal totalhands = statistic.Count() / (decimal)100;
                decimal adjustedNetwon = statistic.Sum(x => GetDivisionResult(x.NetWon + x.EVDiff, x.BigBlind));

                return Math.Round(GetDivisionResult(adjustedNetwon, totalhands), 2);
            }
        }

        private decimal? stdDev;

        public virtual decimal StdDev
        {
            get
            {
                if (!stdDev.HasValue)
                {
                    CalculateStdDeviation();
                }

                return stdDev ?? 0m;
            }
        }

        private decimal? stdDevBB;

        public virtual decimal StdDevBB
        {
            get
            {
                if (!stdDevBB.HasValue)
                {
                    CalculateStdDeviation();
                }

                return stdDevBB ?? 0m;
            }
        }

        public virtual decimal StdDevBBPer100Hands
        {
            get
            {
                // formula is sqrt(100) * stddevBB
                return StdDevBB * 10;
            }
        }

        private decimal? netWonPerHour;

        public virtual decimal NetWonPerHour
        {
            get
            {
                if (!netWonPerHour.HasValue)
                {
                    netWonPerHour = CalculateNetWonPerHour();
                }

                return netWonPerHour.Value;
            }
        }

        public virtual decimal BetWhenCheckedTo
        {
            get
            {
                return GetPercentage(Source.DidBetWhenCheckedToFlop + Source.DidBetWhenCheckedToTurn + Source.DidBetWhenCheckedToRiver,
                    Source.CanBetWhenCheckedToFlop + Source.CanBetWhenCheckedToTurn + Source.CanBetWhenCheckedToRiver);
            }
        }

        public virtual decimal FoldToFlopCheckRaise
        {
            get
            {
                return GetPercentage(Source.FoldedToFlopCheckRaise, Source.FacedFlopCheckRaise);
            }
        }

        public virtual decimal FoldToFlopRaise
        {
            get
            {
                return GetPercentage(Source.FoldedFacedRaiseFlop, Source.FacedRaiseFlop);
            }
        }

        public virtual decimal FoldToTurnCheckRaise
        {
            get
            {
                return GetPercentage(Source.FoldedToTurnCheckRaise, Source.FacedTurnCheckRaise);
            }
        }

        public virtual decimal FoldToTurnRaise
        {
            get
            {
                return GetPercentage(Source.FoldedFacedRaiseTurn, Source.FacedRaiseTurn);
            }
        }

        public virtual decimal CalledTurnCheckRaise
        {
            get
            {
                return GetPercentage(Source.CalledTurnCheckRaise, Source.FacedTurnCheckRaise);
            }
        }

        public virtual decimal FoldToRiverCheckRaise
        {
            get
            {
                return GetPercentage(Source.FoldedToRiverCheckRaise, Source.FacedRiverCheckRaise);
            }
        }

        public virtual decimal FoldToRiverRaise
        {
            get
            {
                return GetPercentage(Source.FoldedFacedRaiseRiver, Source.FacedRaiseRiver);
            }
        }

        public virtual decimal FoldToRiverCBet
        {
            get
            {
                return GetPercentage(Source.Foldedtorivercontinuationbet, Source.Facingrivercontinuationbet);
            }
        }

        public virtual decimal FoldToSqueez
        {
            get
            {
                return GetPercentage(Source.FoldedFacedSqueez, Source.FacedSqueez);
            }
        }

        public virtual decimal NetWon
        {
            get
            {
                return Statistics.Sum(x => x.NetWon);
            }
        }

        public virtual decimal CallFlopCBetIP
        {
            get
            {
                return GetPercentage(Source.CalledflopcontinuationbetIP, Source.FacingflopcontinuationbetIP);
            }
        }

        public virtual decimal CallFlopCBetOOP
        {
            get
            {
                return GetPercentage(Source.CalledflopcontinuationbetOOP, Source.FacingflopcontinuationbetOOP);
            }
        }

        public virtual decimal FoldToFlopCBetIP
        {
            get
            {
                return GetPercentage(Source.FoldToFlopcontinuationbetIP, Source.FacingflopcontinuationbetIP);
            }
        }

        public virtual decimal FoldToFlopCBetOOP
        {
            get
            {
                return GetPercentage(Source.FoldToFlopcontinuationbetOOP, Source.FacingflopcontinuationbetOOP);
            }
        }

        public virtual decimal CallRiverRaise
        {
            get
            {
                return GetPercentage(Source.CalledFacedRaiseRiver, Source.FacedRaiseRiver);
            }
        }

        public virtual decimal CheckRaisedFlopCBet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.CheckRaisedFlopCBet), Statistics.Sum(x => x.CouldCheckRaiseFlopCBet));
            }
        }

        public virtual decimal FoldToTurnCBetIn3BetPot
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FoldToTurnCBetIn3BetPot), Statistics.Sum(x => x.FacedToTurnCBetIn3BetPot));
            }
        }

        public virtual decimal FlopCheckBehind
        {
            get
            {
                return GetPercentage(Source.DidFlopCheckBehind, Source.CouldFlopCheckBehind);
            }
        }

        public virtual decimal FoldToDonkBet
        {
            get
            {
                return GetPercentage(Source.FoldedToDonkBet, Source.FacedDonkBet);
            }
        }

        public virtual decimal FoldTurn
        {
            get
            {
                return GetPercentage(Source.FoldedTurn, Source.FacedBetOnTurn);
            }
        }

        public virtual decimal FoldFlop
        {
            get
            {
                return GetPercentage(Source.FoldedFlop, Source.FacedBetOnFlop);
            }
        }

        public virtual decimal RiverCheckCall
        {
            get
            {
                return GetPercentage(Source.CheckedCalledRiver, Source.CheckedThenFacedBetOnRiver);
            }
        }

        public virtual decimal RiverCheckFold
        {
            get
            {
                return GetPercentage(Source.CheckedFoldedRiver, Source.CheckedThenFacedBetOnRiver);
            }
        }

        public virtual decimal RiverCallEffeciency
        {
            get
            {
                return GetDivisionResult(Source.TotalWonAmountOnRiverCall, Source.TotalCallAmountOnRiver);
            }
        }

        #region FlopBetSize stats

        public virtual decimal FlopBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeOneHalfOrLess), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        public virtual decimal FlopBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeOneQuarterOrLess), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        public virtual decimal FlopBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeTwoThirdsOrLess), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        public virtual decimal FlopBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeThreeQuartersOrLess), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        public virtual decimal FlopBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeOneOrLess), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        public virtual decimal FlopBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.FlopBetSizeMoreThanOne), Statistics.Sum(x => x.DidFlopBet));
            }
        }

        #endregion

        #region TurnBetSize stats

        public virtual decimal TurnBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeOneHalfOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeOneQuarterOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeOneThirdOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeOneThirdOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeTwoThirdsOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeThreeQuartersOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeOneOrLess), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        public virtual decimal TurnBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.TurnBetSizeMoreThanOne), Statistics.Sum(x => x.DidTurnBet));
            }
        }

        #endregion

        #region RiverBetSize stats

        public virtual decimal RiverBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.RiverBetSizeMoreThanOne), Statistics.Sum(x => x.DidRiverBet));
            }
        }

        #endregion

        #region WTSD After stats

        public virtual decimal WTSDAfterCalling3Bet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.WTSDAfterCalling3Bet), Statistics.Sum(x => x.WTSDAfterCalling3BetOpportunity));
            }
        }

        public virtual decimal WTSDAfterCallingPfr
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.WTSDAfterCallingPfr), Statistics.Sum(x => x.WTSDAfterCallingPfrOpportunity));
            }
        }

        public virtual decimal WTSDAfterNotCBettingFlopAsPfr
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.WTSDAfterNotCBettingFlopAsPfr), Statistics.Sum(x => x.WTSDAfterNotCBettingFlopAsPfrOpportunity));
            }
        }

        public virtual decimal WTSDAfterSeeingTurn
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.WTSDAfterSeeingTurn), Statistics.Sum(x => x.SawTurn));
            }
        }

        public virtual decimal WTSDAsPF3Bettor
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.WTSDAsPF3Bettor), Statistics.Sum(x => x.WTSDAsPF3BettorOpportunity));
            }
        }

        #endregion

        #region Positional internal stats

        [ProtoMember(2)]
        protected PositionalStat positionUnoppened = new PositionalStat();

        [ProtoMember(3)]
        protected PositionalStat positionTotal = new PositionalStat();

        [ProtoMember(4)]
        protected PositionalStat positionVPIP = new PositionalStat();

        [ProtoMember(5)]
        protected PositionalStat positionDidColdCall = new PositionalStat();

        [ProtoMember(6)]
        protected PositionalStat positionCouldColdCall = new PositionalStat();

        [ProtoMember(7)]
        protected PositionalStat positionDidThreeBet = new PositionalStat();

        [ProtoMember(8)]
        protected PositionalStat positionCouldThreeBet = new PositionalStat();

        [ProtoMember(9)]
        protected PositionalStat positionDidFourBet = new PositionalStat();

        [ProtoMember(10)]
        protected PositionalStat positionCouldFourBet = new PositionalStat();

        [ProtoMember(11)]
        protected PositionalStat positionLimpMade = new PositionalStat();

        [ProtoMember(12)]
        protected PositionalStat positionLimpPossible = new PositionalStat();

        [ProtoMember(13)]
        protected PositionalStat positionDidColdCallThreeBet = new PositionalStat();

        [ProtoMember(14)]
        protected PositionalStat positionCouldColdCallThreeBet = new PositionalStat();

        [ProtoMember(15)]
        protected PositionalStat positionRaiseLimpers = new PositionalStat();

        [ProtoMember(16)]
        protected PositionalStat positionCouldRaiseLimpers = new PositionalStat();

        [ProtoMember(17)]
        protected PositionalStat positionDidColdCallFourBet = new PositionalStat();

        [ProtoMember(18)]
        protected PositionalStat positionCouldColdCallFourBet = new PositionalStat();

        [ProtoMember(19)]
        protected PositionalStat positionOpenMinraiseUOPFR = new PositionalStat();

        [ProtoMember(20)]
        protected PositionalStat positionDidSqueeze = new PositionalStat();

        [ProtoMember(21)]
        protected PositionalStat positionCouldSqueeze = new PositionalStat();

        [ProtoMember(22)]
        protected PositionalStat positionCall4Bet = new PositionalStat();

        [ProtoMember(23)]
        protected PositionalStat positionFaced4Bet = new PositionalStat();

        [ProtoMember(24)]
        protected PositionalStat positionDidOpenLimp = new PositionalStat();

        [ProtoMember(25)]
        protected PositionalStat positionCouldOpenLimp = new PositionalStat();


        [ProtoMember(26)]
        protected PositionalStat positionCall3Bet = new PositionalStat();

        [ProtoMember(27)]
        protected PositionalStat positionFaced3Bet = new PositionalStat();

        #endregion

        #region Raise Limpers

        public virtual decimal RaiseLimpers
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.IsRaisedLimpers), Statistics.Sum(x => x.CouldRaiseLimpers));
            }
        }

        public virtual decimal RaiseLimpersInMP
        {
            get
            {
                return GetPercentage(positionRaiseLimpers?.MP, positionCouldRaiseLimpers?.MP);
            }
        }

        public virtual decimal RaiseLimpersInCO
        {
            get
            {
                return GetPercentage(positionRaiseLimpers?.CO, positionCouldRaiseLimpers?.CO);
            }
        }

        public virtual decimal RaiseLimpersInBN
        {
            get
            {
                return GetPercentage(positionRaiseLimpers?.BN, positionCouldRaiseLimpers?.BN);
            }
        }

        public virtual decimal RaiseLimpersInSB
        {
            get
            {
                return GetPercentage(positionRaiseLimpers?.SB, positionCouldRaiseLimpers?.SB);
            }
        }

        public virtual decimal RaiseLimpersInBB
        {
            get
            {
                return GetPercentage(positionRaiseLimpers?.BB, positionCouldRaiseLimpers?.BB);
            }
        }

        #endregion

        #region 3-Bet vs Pos stats

        public abstract decimal ThreeBetMPvsEP { get; }

        public abstract decimal ThreeBetCOvsEP { get; }

        public abstract decimal ThreeBetCOvsMP { get; }

        public abstract decimal ThreeBetBTNvsEP { get; }

        public abstract decimal ThreeBetBTNvsMP { get; }

        public abstract decimal ThreeBetBTNvsCO { get; }

        public abstract decimal ThreeBetSBvsEP { get; }

        public abstract decimal ThreeBetSBvsMP { get; }

        public abstract decimal ThreeBetSBvsCO { get; }

        public abstract decimal ThreeBetSBvsBTN { get; }

        public abstract decimal ThreeBetBBvsEP { get; }

        public abstract decimal ThreeBetBBvsMP { get; }

        public abstract decimal ThreeBetBBvsCO { get; }

        public abstract decimal ThreeBetBBvsBTN { get; }

        public abstract decimal ThreeBetBBvsSB { get; }

        #endregion

        #region Fold to 3-Bet in Pos vs 3-bet Pos

        public abstract decimal FoldTo3BetInEPvs3BetMP { get; }

        public abstract decimal FoldTo3BetInEPvs3BetCO { get; }

        public abstract decimal FoldTo3BetInEPvs3BetBTN { get; }

        public abstract decimal FoldTo3BetInEPvs3BetSB { get; }

        public abstract decimal FoldTo3BetInEPvs3BetBB { get; }

        public abstract decimal FoldTo3BetInMPvs3BetCO { get; }

        public abstract decimal FoldTo3BetInMPvs3BetBTN { get; }

        public abstract decimal FoldTo3BetInMPvs3BetSB { get; }

        public abstract decimal FoldTo3BetInMPvs3BetBB { get; }

        public abstract decimal FoldTo3BetInCOvs3BetBTN { get; }

        public abstract decimal FoldTo3BetInCOvs3BetSB { get; }

        public abstract decimal FoldTo3BetInCOvs3BetBB { get; }

        public abstract decimal FoldTo3BetInBTNvs3BetSB { get; }

        public abstract decimal FoldTo3BetInBTNvs3BetBB { get; }

        #endregion

        #region Others stats

        public abstract decimal CheckRaiseFlopAsPFR { get; }

        public virtual decimal ProbeBetTurn => GetPercentage(Statistics.Sum(x => x.ProbeBetTurn), Source.CouldProbeBetTurn);

        public virtual decimal ProbeBetRiver => GetPercentage(Statistics.Sum(x => x.ProbeBetRiver), Source.CouldProbeBetRiver);

        public virtual decimal FloatFlopThenBetTurn => GetPercentage(Statistics.Sum(x => x.FloatFlopThenBetTurn), Statistics.Sum(x => x.CouldFloatFlopThenBetTurn));

        public virtual decimal FoldBBvsSBSteal => GetPercentage(Statistics.Sum(x => x.FoldBBvsSBSteal), Statistics.Sum(x => x.CouldFoldBBvsSBSteal));

        public virtual decimal BetTurnWhenCheckedToSRP => GetPercentage(Statistics.Sum(x => x.BetTurnWhenCheckedToSRP), Statistics.Sum(x => x.CouldBetTurnWhenCheckedToSRP));

        public virtual decimal BetRiverWhenCheckedToSRP => GetPercentage(Statistics.Sum(x => x.BetRiverWhenCheckedToSRP), Statistics.Sum(x => x.CouldBetRiverWhenCheckedToSRP));

        public virtual decimal DoubleBarrelSRP => GetPercentage(Statistics.Sum(x => x.DoubleBarrelSRP), Statistics.Sum(x => x.CouldDoubleBarrelSRP));

        public virtual decimal DoubleBarrel3BetPot => GetPercentage(Statistics.Sum(x => x.DoubleBarrel3BetPot), Statistics.Sum(x => x.CouldDoubleBarrel3BetPot));

        public virtual decimal TripleBarrelSRP => GetPercentage(Statistics.Sum(x => x.TripleBarrelSRP), Statistics.Sum(x => x.CouldTripleBarrelSRP));

        public virtual decimal TripleBarrel3BetPot => GetPercentage(Statistics.Sum(x => x.TripleBarrel3BetPot), Statistics.Sum(x => x.CouldTripleBarrel3BetPot));

        public virtual decimal CBetThenFoldFlopSRP => GetPercentage(Statistics.Sum(x => x.CBetThenFoldFlopSRP), Statistics.Sum(x => x.CouldCBetThenFoldFlopSRP));

        public virtual decimal FoldToProbeBetTurn => GetPercentage(Statistics.Sum(x => x.FoldedToProbeBetTurn), Statistics.Sum(x => x.FacedProbeBetTurn));

        public virtual decimal FoldToProbeBetRiver => GetPercentage(Statistics.Sum(x => x.FoldedToProbeBetRiver), Statistics.Sum(x => x.FacedProbeBetRiver));

        #endregion

        #region Bet When Checked to in 3Bet Pot

        public virtual decimal BetFlopWhenCheckedToIn3BetPot => GetPercentage(Statistics.Sum(x => x.BetFlopWhenCheckedToIn3BetPot), Statistics.Sum(x => x.CouldBetFlopWhenCheckedToIn3BetPot));

        public virtual decimal BetTurnWhenCheckedToIn3BetPot => GetPercentage(Statistics.Sum(x => x.BetTurnWhenCheckedToIn3BetPot), Statistics.Sum(x => x.CouldBetTurnWhenCheckedToIn3BetPot));

        public virtual decimal BetRiverWhenCheckedToIn3BetPot => GetPercentage(Statistics.Sum(x => x.BetRiverWhenCheckedToIn3BetPot), Statistics.Sum(x => x.CouldBetRiverWhenCheckedToIn3BetPot));

        #endregion

        #region Check Flop as PFR and Fold to Turn/River Bet SRP

        public virtual decimal CheckFlopAsPFRAndFoldToTurnBetIPSRP => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToTurnBetIPSRP), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP));

        public virtual decimal CheckFlopAsPFRAndFoldToTurnBetOOPSRP => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToTurnBetOOPSRP), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP));

        public virtual decimal CheckFlopAsPFRAndFoldToRiverBetIPSRP => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToRiverBetIPSRP), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP));

        public virtual decimal CheckFlopAsPFRAndFoldToRiverBetOOPSRP => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToRiverBetOOPSRP), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToRiverBetOOPSRP));

        #endregion

        #region Check Flop as PFR and Fold to Turn/River Bet in 3-Bet Pot

        public virtual decimal CheckFlopAsPFRAndFoldToTurnBetIP3BetPot => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot));

        public virtual decimal CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot));

        public virtual decimal CheckFlopAsPFRAndFoldToRiverBetIP3BetPot => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot));

        public virtual decimal CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot => GetPercentage(Statistics.Sum(x => x.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot), Statistics.Sum(x => x.CouldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot));

        #endregion

        #region Fold to continuation bets in SRP/3Bet/4Bet

        public virtual decimal FoldToTripleBarrelSRP => GetPercentage(Statistics.Sum(x => x.FoldToTripleBarrelSRP), Statistics.Sum(x => x.FacingTripleBarrelSRP));

        public virtual decimal FoldToTripleBarrel3BetPot => GetPercentage(Statistics.Sum(x => x.FoldToTripleBarrel3BetPot), Statistics.Sum(x => x.FacingTripleBarrel3BetPot));

        public virtual decimal FoldToTripleBarrel4BetPot => GetPercentage(Statistics.Sum(x => x.FoldToTripleBarrel4BetPot), Statistics.Sum(x => x.FacingTripleBarrel4BetPot));

        public virtual decimal FoldToDoubleBarrelSRP => GetPercentage(Statistics.Sum(x => x.FoldToDoubleBarrelSRP), Statistics.Sum(x => x.FacingDoubleBarrelSRP));

        public virtual decimal FoldToDoubleBarrel4BetPot => GetPercentage(Statistics.Sum(x => x.FoldToDoubleBarrel4BetPot), Statistics.Sum(x => x.FacingDoubleBarrel4BetPot));

        public virtual decimal FoldToCBetSRP => GetPercentage(Statistics.Sum(x => x.FoldToCBetSRP), Statistics.Sum(x => x.FacingCBetSRP));

        #endregion

        #region Open Shove UO Pot positional

        public virtual decimal SBOpenShove1to8bbUOPot => GetPercentage(Statistics.Sum(x => x.SBOpenShove1to8bbUOPot), Statistics.Sum(x => x.SBOpenShoveUOPot));

        public virtual decimal SBOpenShove9to14bbUOPot => GetPercentage(Statistics.Sum(x => x.SBOpenShove9to14bbUOPot), Statistics.Sum(x => x.SBOpenShoveUOPot));

        public virtual decimal SBOpenShove15to25bbUOPot => GetPercentage(Statistics.Sum(x => x.SBOpenShove15to25bbUOPot), Statistics.Sum(x => x.SBOpenShoveUOPot));

        public virtual decimal SBOpenShove26to50bbUOPot => GetPercentage(Statistics.Sum(x => x.SBOpenShove26to50bbUOPot), Statistics.Sum(x => x.SBOpenShoveUOPot));

        public virtual decimal SBOpenShove51plusbbUOPot => GetPercentage(Statistics.Sum(x => x.SBOpenShove51plusbbUOPot), Statistics.Sum(x => x.SBOpenShoveUOPot));

        public virtual decimal BTNOpenShove1to8bbUOPot => GetPercentage(Statistics.Sum(x => x.BTNOpenShove1to8bbUOPot), Statistics.Sum(x => x.BTNOpenShoveUOPot));

        public virtual decimal BTNOpenShove9to14bbUOPot => GetPercentage(Statistics.Sum(x => x.BTNOpenShove9to14bbUOPot), Statistics.Sum(x => x.BTNOpenShoveUOPot));

        public virtual decimal BTNOpenShove15to25bbUOPot => GetPercentage(Statistics.Sum(x => x.BTNOpenShove15to25bbUOPot), Statistics.Sum(x => x.BTNOpenShoveUOPot));

        public virtual decimal BTNOpenShove26to50bbUOPot => GetPercentage(Statistics.Sum(x => x.BTNOpenShove26to50bbUOPot), Statistics.Sum(x => x.BTNOpenShoveUOPot));

        public virtual decimal BTNOpenShove51plusbbUOPot => GetPercentage(Statistics.Sum(x => x.BTNOpenShove51plusbbUOPot), Statistics.Sum(x => x.BTNOpenShoveUOPot));

        public virtual decimal COOpenShove1to8bbUOPot => GetPercentage(Statistics.Sum(x => x.COOpenShove1to8bbUOPot), Statistics.Sum(x => x.COOpenShoveUOPot));

        public virtual decimal COOpenShove9to14bbUOPot => GetPercentage(Statistics.Sum(x => x.COOpenShove9to14bbUOPot), Statistics.Sum(x => x.COOpenShoveUOPot));

        public virtual decimal COOpenShove15to25bbUOPot => GetPercentage(Statistics.Sum(x => x.COOpenShove15to25bbUOPot), Statistics.Sum(x => x.COOpenShoveUOPot));

        public virtual decimal COOpenShove26to50bbUOPot => GetPercentage(Statistics.Sum(x => x.COOpenShove26to50bbUOPot), Statistics.Sum(x => x.COOpenShoveUOPot));

        public virtual decimal COOpenShove51plusbbUOPot => GetPercentage(Statistics.Sum(x => x.COOpenShove51plusbbUOPot), Statistics.Sum(x => x.COOpenShoveUOPot));

        public virtual decimal MPOpenShove1to8bbUOPot => GetPercentage(Statistics.Sum(x => x.MPOpenShove1to8bbUOPot), Statistics.Sum(x => x.MPOpenShoveUOPot));

        public virtual decimal MPOpenShove9to14bbUOPot => GetPercentage(Statistics.Sum(x => x.MPOpenShove9to14bbUOPot), Statistics.Sum(x => x.MPOpenShoveUOPot));

        public virtual decimal MPOpenShove15to25bbUOPot => GetPercentage(Statistics.Sum(x => x.MPOpenShove15to25bbUOPot), Statistics.Sum(x => x.MPOpenShoveUOPot));

        public virtual decimal MPOpenShove26to50bbUOPot => GetPercentage(Statistics.Sum(x => x.MPOpenShove26to50bbUOPot), Statistics.Sum(x => x.MPOpenShoveUOPot));

        public virtual decimal MPOpenShove51plusbbUOPot => GetPercentage(Statistics.Sum(x => x.MPOpenShove51plusbbUOPot), Statistics.Sum(x => x.MPOpenShoveUOPot));

        public virtual decimal EPOpenShove1to8bbUOPot => GetPercentage(Statistics.Sum(x => x.EPOpenShove1to8bbUOPot), Statistics.Sum(x => x.EPOpenShoveUOPot));

        public virtual decimal EPOpenShove9to14bbUOPot => GetPercentage(Statistics.Sum(x => x.EPOpenShove9to14bbUOPot), Statistics.Sum(x => x.EPOpenShoveUOPot));

        public virtual decimal EPOpenShove15to25bbUOPot => GetPercentage(Statistics.Sum(x => x.EPOpenShove15to25bbUOPot), Statistics.Sum(x => x.EPOpenShoveUOPot));

        public virtual decimal EPOpenShove26to50bbUOPot => GetPercentage(Statistics.Sum(x => x.EPOpenShove26to50bbUOPot), Statistics.Sum(x => x.EPOpenShoveUOPot));

        public virtual decimal EPOpenShove51plusbbUOPot => GetPercentage(Statistics.Sum(x => x.EPOpenShove51plusbbUOPot), Statistics.Sum(x => x.EPOpenShoveUOPot));

        #endregion

        #region Limp Positional & Fold to PFR%

        public virtual decimal LimpEPFoldToPFR => GetPercentage(Statistics.Sum(x => x.LimpEPFoldToPFR), Statistics.Sum(x => x.LimpEPFacedPFR));

        public virtual decimal LimpMPFoldToPFR => GetPercentage(Statistics.Sum(x => x.LimpMPFoldToPFR), Statistics.Sum(x => x.LimpMPFacedPFR));

        public virtual decimal LimpCOFoldToPFR => GetPercentage(Statistics.Sum(x => x.LimpCOFoldToPFR), Statistics.Sum(x => x.LimpCOFacedPFR));

        public virtual decimal LimpBTNFoldToPFR => GetPercentage(Statistics.Sum(x => x.LimpBTNFoldToPFR), Statistics.Sum(x => x.LimpBTNFacedPFR));

        public virtual decimal LimpSBFoldToPFR => GetPercentage(Statistics.Sum(x => x.LimpSBFoldToPFR), Statistics.Sum(x => x.LimpSBFacedPFR));

        #endregion

        #region Shoves over limpers positional

        public virtual decimal SBShoveOverLimpers1to8bb => GetPercentage(Statistics.Sum(x => x.SBShoveOverLimpers1to8bb), Statistics.Sum(x => x.SBShoveOverLimpers));

        public virtual decimal SBShoveOverLimpers9to14bb => GetPercentage(Statistics.Sum(x => x.SBShoveOverLimpers9to14bb), Statistics.Sum(x => x.SBShoveOverLimpers));

        public virtual decimal SBShoveOverLimpers15to25bb => GetPercentage(Statistics.Sum(x => x.SBShoveOverLimpers15to25bb), Statistics.Sum(x => x.SBShoveOverLimpers));

        public virtual decimal SBShoveOverLimpers26to50bb => GetPercentage(Statistics.Sum(x => x.SBShoveOverLimpers26to50bb), Statistics.Sum(x => x.SBShoveOverLimpers));

        public virtual decimal SBShoveOverLimpers51plusbb => GetPercentage(Statistics.Sum(x => x.SBShoveOverLimpers51plusbb), Statistics.Sum(x => x.SBShoveOverLimpers));

        public virtual decimal BTNShoveOverLimpers1to8bb => GetPercentage(Statistics.Sum(x => x.BTNShoveOverLimpers1to8bb), Statistics.Sum(x => x.BTNShoveOverLimpers));

        public virtual decimal BTNShoveOverLimpers9to14bb => GetPercentage(Statistics.Sum(x => x.BTNShoveOverLimpers9to14bb), Statistics.Sum(x => x.BTNShoveOverLimpers));

        public virtual decimal BTNShoveOverLimpers15to25bb => GetPercentage(Statistics.Sum(x => x.BTNShoveOverLimpers15to25bb), Statistics.Sum(x => x.BTNShoveOverLimpers));

        public virtual decimal BTNShoveOverLimpers26to50bb => GetPercentage(Statistics.Sum(x => x.BTNShoveOverLimpers26to50bb), Statistics.Sum(x => x.BTNShoveOverLimpers));

        public virtual decimal BTNShoveOverLimpers51plusbb => GetPercentage(Statistics.Sum(x => x.BTNShoveOverLimpers51plusbb), Statistics.Sum(x => x.BTNShoveOverLimpers));

        public virtual decimal COShoveOverLimpers1to8bb => GetPercentage(Statistics.Sum(x => x.COShoveOverLimpers1to8bb), Statistics.Sum(x => x.COShoveOverLimpers));

        public virtual decimal COShoveOverLimpers9to14bb => GetPercentage(Statistics.Sum(x => x.COShoveOverLimpers9to14bb), Statistics.Sum(x => x.COShoveOverLimpers));

        public virtual decimal COShoveOverLimpers15to25bb => GetPercentage(Statistics.Sum(x => x.COShoveOverLimpers15to25bb), Statistics.Sum(x => x.COShoveOverLimpers));

        public virtual decimal COShoveOverLimpers26to50bb => GetPercentage(Statistics.Sum(x => x.COShoveOverLimpers26to50bb), Statistics.Sum(x => x.COShoveOverLimpers));

        public virtual decimal COShoveOverLimpers51plusbb => GetPercentage(Statistics.Sum(x => x.COShoveOverLimpers51plusbb), Statistics.Sum(x => x.COShoveOverLimpers));

        public virtual decimal MPShoveOverLimpers1to8bb => GetPercentage(Statistics.Sum(x => x.MPShoveOverLimpers1to8bb), Statistics.Sum(x => x.MPShoveOverLimpers));

        public virtual decimal MPShoveOverLimpers9to14bb => GetPercentage(Statistics.Sum(x => x.MPShoveOverLimpers9to14bb), Statistics.Sum(x => x.MPShoveOverLimpers));

        public virtual decimal MPShoveOverLimpers15to25bb => GetPercentage(Statistics.Sum(x => x.MPShoveOverLimpers15to25bb), Statistics.Sum(x => x.MPShoveOverLimpers));

        public virtual decimal MPShoveOverLimpers26to50bb => GetPercentage(Statistics.Sum(x => x.MPShoveOverLimpers26to50bb), Statistics.Sum(x => x.MPShoveOverLimpers));

        public virtual decimal MPShoveOverLimpers51plusbb => GetPercentage(Statistics.Sum(x => x.MPShoveOverLimpers51plusbb), Statistics.Sum(x => x.MPShoveOverLimpers));

        public virtual decimal EPShoveOverLimpers1to8bb => GetPercentage(Statistics.Sum(x => x.EPShoveOverLimpers1to8bb), Statistics.Sum(x => x.EPShoveOverLimpers));

        public virtual decimal EPShoveOverLimpers9to14bb => GetPercentage(Statistics.Sum(x => x.EPShoveOverLimpers9to14bb), Statistics.Sum(x => x.EPShoveOverLimpers));

        public virtual decimal EPShoveOverLimpers15to25bb => GetPercentage(Statistics.Sum(x => x.EPShoveOverLimpers15to25bb), Statistics.Sum(x => x.EPShoveOverLimpers));

        public virtual decimal EPShoveOverLimpers26to50bb => GetPercentage(Statistics.Sum(x => x.EPShoveOverLimpers26to50bb), Statistics.Sum(x => x.EPShoveOverLimpers));

        public virtual decimal EPShoveOverLimpers51plusbb => GetPercentage(Statistics.Sum(x => x.EPShoveOverLimpers51plusbb), Statistics.Sum(x => x.EPShoveOverLimpers));

        #endregion

        #region Positional squeeze

        public virtual decimal SqueezeEP => GetPercentage(positionDidSqueeze?.EP, positionCouldSqueeze?.EP);

        public virtual decimal SqueezeMP => GetPercentage(positionDidSqueeze?.MP, positionCouldSqueeze?.MP);

        public virtual decimal SqueezeCO => GetPercentage(positionDidSqueeze?.CO, positionCouldSqueeze?.CO);

        public virtual decimal SqueezeBTN => GetPercentage(positionDidSqueeze?.BN, positionCouldSqueeze?.BN);

        public virtual decimal SqueezeSB => GetPercentage(positionDidSqueeze?.SB, positionCouldSqueeze?.SB);

        public virtual decimal SqueezeBB => GetPercentage(positionDidSqueeze?.BB, positionCouldSqueeze?.BB);

        #endregion;

        #region Squeeze vs PFR

        public virtual decimal SqueezeBBVsBTNPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBBVsBTNPFR), Statistics.Sum(x => x.CouldSqueezeBBVsBTNPFR));

        public virtual decimal SqueezeBBVsCOPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBBVsCOPFR), Statistics.Sum(x => x.CouldSqueezeBBVsCOPFR));

        public virtual decimal SqueezeBBVsMPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBBVsMPPFR), Statistics.Sum(x => x.CouldSqueezeBBVsMPPFR));

        public virtual decimal SqueezeBBVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBBVsEPPFR), Statistics.Sum(x => x.CouldSqueezeBBVsEPPFR));

        public virtual decimal SqueezeSBVsCOPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeSBVsCOPFR), Statistics.Sum(x => x.CouldSqueezeSBVsCOPFR));

        public virtual decimal SqueezeSBVsMPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeSBVsMPPFR), Statistics.Sum(x => x.CouldSqueezeSBVsMPPFR));

        public virtual decimal SqueezeSBVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeSBVsEPPFR), Statistics.Sum(x => x.CouldSqueezeSBVsEPPFR));

        public virtual decimal SqueezeBTNVsMPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBTNVsMPPFR), Statistics.Sum(x => x.CouldSqueezeBTNVsMPPFR));

        public virtual decimal SqueezeBTNVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeBTNVsEPPFR), Statistics.Sum(x => x.CouldSqueezeBTNVsEPPFR));

        public virtual decimal SqueezeCOVsMPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeCOVsMPPFR), Statistics.Sum(x => x.CouldSqueezeCOVsMPPFR));

        public virtual decimal SqueezeCOVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeCOVsEPPFR), Statistics.Sum(x => x.CouldSqueezeCOVsEPPFR));

        public virtual decimal SqueezeMPVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeMPVsEPPFR), Statistics.Sum(x => x.CouldSqueezeMPVsEPPFR));

        public virtual decimal SqueezeEPVsEPPFR => GetPercentage(Statistics.Sum(x => x.DidSqueezeEPVsEPPFR), Statistics.Sum(x => x.CouldSqueezeEPVsEPPFR));

        #endregion

        #region Fold to Squeeze as Cold Caller

        public virtual decimal FoldToSqueezeAsColdCaller => GetPercentage(Statistics.Sum(x => x.FoldToSqueezeAsColdCaller), Statistics.Sum(x => x.FacedSqueezeAsColdCaller));

        #endregion

        #region 4-Bet vs Blind 3-Bet%

        public virtual decimal FourBetVsBlind3Bet => GetPercentage(Statistics.Sum(x => x.Did4BetVsBlind3Bet), Statistics.Sum(x => x.Could4BetVsBlind3Bet));

        #endregion

        #region BTN Re/Def vs CO Steal

        public virtual decimal BTNReStealVsCOSteal => GetPercentage(Statistics.Sum(x => x.BTNReStealVsCOSteal), Statistics.Sum(x => x.BTNFacedCOSteal));

        public virtual decimal BTNDefendVsCOSteal => GetPercentage(Statistics.Sum(x => x.BTNDefendVsCOSteal), Statistics.Sum(x => x.BTNFacedCOSteal));

        #endregion

        #region Positional Call & Fold to Steal

        public virtual decimal FoldToStealInSB => GetPercentage(Statistics.Sum(x => x.FoldToStealInSB), Statistics.Sum(x => x.FacedStealInSB));

        public virtual decimal FoldToStealInBB => GetPercentage(Statistics.Sum(x => x.FoldToStealInBB), Statistics.Sum(x => x.FacedStealInBB));

        public virtual decimal CalledStealInSB => GetPercentage(Statistics.Sum(x => x.CalledStealInSB), Statistics.Sum(x => x.FacedStealInSB));

        public virtual decimal CalledStealInBB => GetPercentage(Statistics.Sum(x => x.CalledStealInBB), Statistics.Sum(x => x.FacedStealInBB));

        public virtual decimal FoldToBTNStealInSB => GetPercentage(Statistics.Sum(x => x.FoldToBTNStealInSB), Statistics.Sum(x => x.FacedBTNStealInSB));

        public virtual decimal FoldToBTNStealInBB => GetPercentage(Statistics.Sum(x => x.FoldToBTNStealInBB), Statistics.Sum(x => x.FacedBTNStealInBB));

        public virtual decimal FoldToCOStealInSB => GetPercentage(Statistics.Sum(x => x.FoldToCOStealInSB), Statistics.Sum(x => x.FacedCOStealInSB));

        public virtual decimal FoldToCOStealInBB => GetPercentage(Statistics.Sum(x => x.FoldToCOStealInBB), Statistics.Sum(x => x.FacedCOStealInBB));

        public virtual decimal CalledBTNStealInSB => GetPercentage(Statistics.Sum(x => x.CalledBTNStealInSB), Statistics.Sum(x => x.FacedBTNStealInSB));

        public virtual decimal CalledBTNStealInBB => GetPercentage(Statistics.Sum(x => x.CalledBTNStealInBB), Statistics.Sum(x => x.FacedBTNStealInBB));

        public virtual decimal CalledCOStealInSB => GetPercentage(Statistics.Sum(x => x.CalledCOStealInSB), Statistics.Sum(x => x.FacedCOStealInSB));

        public virtual decimal CalledCOStealInBB => GetPercentage(Statistics.Sum(x => x.CalledCOStealInBB), Statistics.Sum(x => x.FacedCOStealInBB));

        public virtual decimal OvercallBTNStealInBB => GetPercentage(Statistics.Sum(x => x.OvercallBTNStealInBB), Statistics.Sum(x => x.CouldOvercallBTNStealInBB));

        #endregion

        #region  WTSD as PFR/4-Bettor

        public virtual decimal WTSDAsPFR => GetPercentage(Statistics.Sum(x => x.WTSDAsPFR), Statistics.Sum(x => x.WTSDAsPFROpportunity));

        public virtual decimal WTSDAs4Bettor => GetPercentage(Statistics.Sum(x => x.WTSDAs4Bettor), Statistics.Sum(x => x.WTSDAs4BettorOpportunity));

        #endregion

        #region Call 4-Bet positional 

        public virtual decimal Call4Bet => GetPercentage(Source.Calledfourbetpreflop, Source.Facedfourbetpreflop);

        public virtual decimal Call4BetIP => GetPercentage(Statistics.Sum(x => x.Call4BetIP), Statistics.Sum(x => x.Faced4BetIP));

        public virtual decimal Call4BetOOP => GetPercentage(Statistics.Sum(x => x.Call4BetOOP), Statistics.Sum(x => x.Faced4BetOOP));

        public virtual decimal Call4BetEP => GetPercentage(positionCall4Bet?.EP, positionFaced4Bet?.EP);

        public virtual decimal Call4BetMP => GetPercentage(positionCall4Bet?.MP, positionFaced4Bet?.MP);

        public virtual decimal Call4BetCO => GetPercentage(positionCall4Bet?.CO, positionFaced4Bet?.CO);

        public virtual decimal Call4BetBTN => GetPercentage(positionCall4Bet?.BN, positionFaced4Bet?.BN);

        public virtual decimal Call4BetSB => GetPercentage(positionCall4Bet?.SB, positionFaced4Bet?.SB);

        public virtual decimal Call4BetBB => GetPercentage(positionCall4Bet?.BB, positionFaced4Bet?.BB);

        #endregion

        #region Total overcall SRP%

        public virtual decimal TotalOverCallSRP => GetPercentage(Statistics.Sum(x => x.TotalOverCallSRP), Statistics.Sum(x => x.CouldTotalOverCallSRP));

        #endregion

        #region Limped pot Flop Steal IP%

        public virtual decimal LimpedPotFlopStealIP => GetPercentage(Statistics.Sum(x => x.LimpedPotFlopStealIP), Statistics.Sum(x => x.CouldLimpedPotFlopStealIP));

        #endregion

        #region Flop-Check Call

        public virtual decimal FlopCheckCall => GetPercentage(Statistics.Sum(x => x.DidFlopCheckCall), Statistics.Sum(x => x.CouldFlopCheckCall));

        #endregion

        #region Call Flop & Fold Turn

        public virtual decimal CallFlopFoldTurn => GetPercentage(Statistics.Sum(x => x.DidCallFlopFoldTurn), Statistics.Sum(x => x.CouldCallFlopFoldTurn));

        #endregion

        #region River fold in SRP/3-Bet/4-Bet

        public virtual decimal RiverFoldInSRP => GetPercentage(Statistics.Sum(x => x.DidRiverFoldInSRP), Statistics.Sum(x => x.CouldRiverFoldInSRP));

        public virtual decimal RiverFoldIn3Bet => GetPercentage(Statistics.Sum(x => x.DidRiverFoldIn3Bet), Statistics.Sum(x => x.CouldRiverFoldIn3Bet));

        public virtual decimal RiverFoldIn4Bet => GetPercentage(Statistics.Sum(x => x.DidRiverFoldIn4Bet), Statistics.Sum(x => x.CouldRiverFoldIn4Bet));

        #endregion

        #region Delayed Turn C-Bet in SRP/4-Bet Pot%

        public virtual decimal DelayedTurnCBetInSRP => GetPercentage(Statistics.Sum(x => x.DidDelayedTurnCBetInSRP), Statistics.Sum(x => x.CouldDelayedTurnCBetInSRP));

        public virtual decimal DelayedTurnCBetIn4BetPot => GetPercentage(Statistics.Sum(x => x.DidDelayedTurnCBetIn4BetPot), Statistics.Sum(x => x.CouldDelayedTurnCBetIn4BetPot));

        #endregion

        #region Skip Flop C-Bet SRP & C/F Flop OOP%

        public virtual decimal SkipFlopCBetInSRPandCheckFoldFlopOOP => GetPercentage(Statistics.Sum(x => x.DidSkipFlopCBetInSRPandCheckFoldFlopOOP), Statistics.Sum(x => x.CouldSkipFlopCBetInSRPandCheckFoldFlopOOP));

        #endregion

        #region  Check-Raise Flop as PFR SRP/3-Bet pot

        public virtual decimal CheckRaiseFlopAsPFRInSRP => GetPercentage(Statistics.Sum(x => x.DidCheckRaiseFlopAsPFRInSRP), Statistics.Sum(x => x.CouldCheckRaiseFlopAsPFRInSRP));

        public virtual decimal CheckRaiseFlopAsPFRIn3BetPot => GetPercentage(Statistics.Sum(x => x.DidCheckRaiseFlopAsPFRIn3BetPot), Statistics.Sum(x => x.CouldCheckRaiseFlopAsPFRIn3BetPot));

        #endregion

        #region Open Limp positional

        public virtual decimal OpenLimpEP => GetPercentage(positionDidOpenLimp?.EP, positionCouldOpenLimp?.EP);

        public virtual decimal OpenLimpMP => GetPercentage(positionDidOpenLimp?.MP, positionCouldOpenLimp?.MP);

        public virtual decimal OpenLimpCO => GetPercentage(positionDidOpenLimp?.CO, positionCouldOpenLimp?.CO);

        public virtual decimal OpenLimpBTN => GetPercentage(positionDidOpenLimp?.BN, positionCouldOpenLimp?.BN);

        public virtual decimal OpenLimpSB => GetPercentage(positionDidOpenLimp?.SB, positionCouldOpenLimp?.SB);

        #endregion

        #region Straddle stats 

        public virtual decimal CheckInStraddle => GetPercentage(Statistics.Sum(x => x.DidCheckInStraddle), Statistics.Sum(x => x.CouldActInStraddle));

        public virtual decimal PFRInStraddle => GetPercentage(Statistics.Sum(x => x.DidPFRInStraddle), Statistics.Sum(x => x.CouldActInStraddle));

        public virtual decimal ThreeBetInStraddle => GetPercentage(Statistics.Sum(x => x.Did3BetInStraddle), Statistics.Sum(x => x.Could3BetInStraddle));

        public virtual decimal FourBetInStraddle => GetPercentage(Statistics.Sum(x => x.Did4BetInStraddle), Statistics.Sum(x => x.Could4BetInStraddle));

        public virtual decimal FoldInStraddle => GetPercentage(Statistics.Sum(x => x.DidFoldInStraddle), Statistics.Sum(x => x.CouldActInStraddle));

        public virtual decimal WTSDInStraddle => GetPercentage(Statistics.Sum(x => x.WTSDInStraddle), Statistics.Sum(x => x.WTSDOpportunityInStraddle));

        #endregion

        #region Fold to delayed Turn C-Bet

        public virtual decimal FoldedToDelayedCBet => GetPercentage(Statistics.Sum(x => x.FoldedToDelayedCBet), Source.FacedDelayedCBet);

        #endregion

        #region Double/Triple barrel

        public virtual decimal DoubleBarrel => GetPercentage(Statistics.Sum(x => x.DidDoubleBarrel), Statistics.Sum(x => x.CouldDoubleBarrel));

        public virtual decimal TripleBarrel => GetPercentage(Statistics.Sum(x => x.DidTripleBarrel), Statistics.Sum(x => x.CouldTripleBarrel));

        #endregion

        #region Cold Call 3/4-Bet

        public virtual decimal ColdCallThreeBet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.DidColdCall3Bet), Statistics.Sum(x => x.CouldColdCall3Bet));
            }
        }

        public virtual decimal ColdCallFourBet
        {
            get
            {
                return GetPercentage(Statistics.Sum(x => x.DidColdCall4Bet), Statistics.Sum(x => x.CouldColdCall4Bet));
            }
        }

        #endregion

        #region Call 3-Bet

        public virtual decimal Call3BetIP => GetPercentage(Statistics.Sum(x => x.Call3BetIP), Statistics.Sum(x => x.Faced3BetIP));

        public virtual decimal Call3BetOOP => GetPercentage(Statistics.Sum(x => x.Call3BetOOP), Statistics.Sum(x => x.Faced3BetOOP));

        public virtual decimal Call3BetEP => GetPercentage(positionCall3Bet?.EP, positionFaced3Bet?.EP);

        public virtual decimal Call3BetMP => GetPercentage(positionCall3Bet?.MP, positionFaced3Bet?.MP);

        public virtual decimal Call3BetCO => GetPercentage(positionCall3Bet?.CO, positionFaced3Bet?.CO);

        public virtual decimal Call3BetBTN => GetPercentage(positionCall3Bet?.BN, positionFaced3Bet?.BN);

        public virtual decimal Call3BetSB => GetPercentage(positionCall3Bet?.SB, positionFaced3Bet?.SB);

        #endregion

        public virtual void UpdateSource(IList<Playerstatistic> statistics)
        {
            foreach (var statistic in statistics)
            {
                AddStatistic(statistic);
            }
        }

        public virtual void AddStatistic(Playerstatistic statistic)
        {
            Source += statistic;
            Statistics.Add(statistic);

            UpdatePositionalStats(statistic);
        }

        public virtual void Clean()
        {
            Source = new Playerstatistic();
            Statistics = new List<Playerstatistic>();
            ResetPositionalStats();
        }

        public virtual int CompareTo(object obj)
        {
            var objIndicator = obj as Indicators;

            if (objIndicator == null)
            {
                return 1;
            }

            var thisIndicatorValue = this.Statistics?.Max(x => x.GameNumber) ?? int.MinValue;
            var objIndicatorValue = objIndicator.Statistics?.Max(x => x.GameNumber) ?? int.MinValue;

            return thisIndicatorValue > objIndicatorValue ? 1
                : thisIndicatorValue < objIndicatorValue ? -1
                : 0;
        }

        protected virtual void UpdatePositionalStats(Playerstatistic statistic)
        {
            var unopened = statistic.IsUnopened && statistic.NumberOfWalks == 0 ? 1 : 0;

            positionTotal?.Add(statistic.Position, 1 - statistic.NumberOfWalks);
            positionUnoppened?.Add(statistic.Position, unopened);
            positionVPIP?.Add(statistic.Position, statistic.Vpiphands);
            positionDidColdCall?.Add(statistic.Position, statistic.Didcoldcall);
            positionCouldColdCall?.Add(statistic.Position, statistic.Couldcoldcall);
            positionDidThreeBet?.Add(statistic.Position, statistic.Didthreebet);
            positionCouldThreeBet?.Add(statistic.Position, statistic.Couldthreebet);
            positionDidFourBet?.Add(statistic.Position, statistic.DidfourbetpreflopVirtual);
            positionCouldFourBet?.Add(statistic.Position, statistic.CouldfourbetpreflopVirtual);
            positionLimpMade?.Add(statistic.Position, statistic.LimpMade);
            positionLimpPossible?.Add(statistic.Position, statistic.LimpPossible);
            positionRaiseLimpers?.Add(statistic.Position, statistic.IsRaisedLimpers);
            positionCouldRaiseLimpers?.Add(statistic.Position, statistic.CouldRaiseLimpers);
            positionDidColdCallThreeBet?.Add(statistic.Position, statistic.DidColdCall3Bet);
            positionCouldColdCallThreeBet?.Add(statistic.Position, statistic.CouldColdCall3Bet);
            positionDidColdCallFourBet?.Add(statistic.Position, statistic.DidColdCall4Bet);
            positionCouldColdCallFourBet?.Add(statistic.Position, statistic.CouldColdCall4Bet);
            positionOpenMinraiseUOPFR?.Add(statistic.Position, statistic.OpenMinraise);
            positionDidSqueeze?.Add(statistic.Position, statistic.Didsqueeze);
            positionCouldSqueeze?.Add(statistic.Position, statistic.Couldsqueeze);
            positionCall4Bet?.Add(statistic.Position, statistic.Calledfourbetpreflop);
            positionFaced4Bet?.Add(statistic.Position, statistic.Facedfourbetpreflop);
            positionDidOpenLimp?.Add(statistic.Position, statistic.DidOpenLimp);
            positionCouldOpenLimp?.Add(statistic.Position, statistic.CouldOpenLimp);
            positionCall3Bet?.Add(statistic.Position, statistic.Calledthreebetpreflop);
            positionFaced3Bet?.Add(statistic.Position, statistic.Facedthreebetpreflop);
        }

        protected virtual void ResetPositionalStats()
        {
            positionTotal?.Reset();
            positionUnoppened?.Reset();
            positionVPIP?.Reset();
            positionDidColdCall?.Reset();
            positionCouldColdCall?.Reset();
            positionDidThreeBet?.Reset();
            positionCouldThreeBet?.Reset();
            positionDidFourBet?.Reset();
            positionCouldFourBet?.Reset();
            positionLimpMade?.Reset();
            positionLimpPossible?.Reset();
            positionRaiseLimpers?.Reset();
            positionCouldRaiseLimpers?.Reset();
            positionDidColdCallThreeBet?.Reset();
            positionCouldColdCallThreeBet?.Reset();
            positionDidColdCallFourBet?.Reset();
            positionCouldColdCallFourBet?.Reset();
            positionOpenMinraiseUOPFR?.Reset();
            positionDidSqueeze?.Reset();
            positionCouldSqueeze?.Reset();
            positionCall4Bet?.Reset();
            positionFaced4Bet?.Reset();
            positionDidOpenLimp?.Reset();
            positionCouldOpenLimp?.Reset();
            positionCall3Bet?.Reset();
            positionFaced3Bet?.Reset();
        }

        #region Helpers

        protected virtual decimal GetPercentage(decimal? actual, decimal? possible)
        {
            if (TotalHands == 0)
            {
                return 0;
            }

            if (!possible.HasValue || !actual.HasValue || possible == 0)
            {
                return 0;
            }

            return (actual.Value / possible.Value) * 100;
        }

        protected virtual decimal GetPositionalPercentage(string position, Func<Playerstatistic, int> actualSelector, Func<Playerstatistic, int> possibleSelector)
        {
            if (Statistics == null)
            {
                return default(decimal);
            }

            var stats = Statistics.Where(x => x.PositionString == position).ToArray();
            return GetPercentage(stats.Sum(actualSelector), stats.Sum(possibleSelector));
        }

        protected decimal GetDivisionResult(decimal actual, decimal possible)
        {
            if (TotalHands == 0)
                return 0;

            if (possible == 0)
                return 0;

            return (actual / possible);
        }

        protected virtual void CalculateStdDeviation(NetWonDeviationItem[] netWonCollection)
        {
            var netWonMean = 0m;
            var netWonMeanInBB = 0m;

            // calculate mean values
            for (var i = 0; i < netWonCollection.Length; i++)
            {
                netWonMean += netWonCollection[i].NetWon;
                netWonMeanInBB += netWonCollection[i].NetWonInBB;
            }

            netWonMean /= netWonCollection.Length;
            netWonMeanInBB /= netWonCollection.Length;

            var netWonVariance = 0m;
            var netWonVarianceInBB = 0m;

            // calculate variances
            for (var i = 0; i < netWonCollection.Length; i++)
            {
                netWonVariance += (netWonCollection[i].NetWon - netWonMean) * (netWonCollection[i].NetWon - netWonMean);
                netWonVarianceInBB += (netWonCollection[i].NetWonInBB - netWonMeanInBB) * (netWonCollection[i].NetWonInBB - netWonMeanInBB);
            }

            netWonVariance /= netWonCollection.Length;
            netWonVarianceInBB /= netWonCollection.Length;

            stdDev = (decimal)Math.Sqrt((double)netWonVariance);
            stdDevBB = (decimal)Math.Sqrt((double)netWonVarianceInBB);
        }

        protected virtual void CalculateStdDeviation()
        {
            if (Statistics == null || Statistics.Count == 0)
            {
                return;
            }

            var statistic = Statistics.ToArray();

            var netWonCollection = statistic
                .Select(x => new NetWonDeviationItem
                {
                    NetWon = x.NetWon,
                    NetWonInBB = GetDivisionResult(x.NetWon, x.BigBlind)
                })
                .ToArray();

            CalculateStdDeviation(netWonCollection);
        }

        protected virtual decimal CalculateNetWonPerHour(DateTime[] orderedTime)
        {
            var totalMinutes = 0d;

            DateTime? sessionStart = null;
            DateTime? sessionEnd = null;

            foreach (var time in orderedTime)
            {
                if (Utils.IsDateInDateRange(time, sessionStart, sessionEnd, TimeSpan.FromMinutes(30)))
                {
                    if (!sessionStart.HasValue)
                    {
                        sessionStart = time;
                    }

                    sessionEnd = time;
                }
                else
                {
                    totalMinutes += (sessionEnd.Value - sessionStart.Value).TotalMinutes;

                    sessionStart = time;
                    sessionEnd = time;
                }
            }

            totalMinutes += (sessionEnd.Value - sessionStart.Value).TotalMinutes;

            if (totalMinutes == 0)
            {
                totalMinutes = 1;
            }

            var netWonPerHour = NetWon / (decimal)totalMinutes * 60;

            return netWonPerHour;
        }

        protected virtual decimal CalculateNetWonPerHour()
        {
            if (Statistics == null || Statistics.Count == 0)
            {
                return 0m;
            }

            var orderedTime = Statistics.OrderBy(x => x.Time).Select(x => x.Time).ToArray();

            var netWonPerHour = CalculateNetWonPerHour(orderedTime);

            return netWonPerHour;
        }

        protected class NetWonDeviationItem
        {
            public decimal NetWon { get; set; }

            public decimal NetWonInBB { get; set; }
        }

        #endregion        
    }
}