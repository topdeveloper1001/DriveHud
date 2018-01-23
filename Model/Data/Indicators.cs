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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Model.Data
{
    /// <summary>
    /// This entity holds all high level indicators. These indicators are based on PlayerStatistic fields
    /// </summary>
    public class Indicators : INotifyPropertyChanged, IComparable
    {
        public event PropertyChangedEventHandler PropertyChanged;

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
                return GetPercentage(Source.DidfourbetpreflopVirtualCounter, Source.CouldfourbetpreflopVirtualCounter);
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

        public virtual decimal ThreeBetCall
        {
            get
            {
                return GetPercentage(Source.Calledthreebetpreflop, Source.FacedthreebetpreflopVirtualCounter);
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
                return GetPercentage(Source.FoldedtothreebetpreflopVirtualCounter, Source.FacedthreebetpreflopVirtualCounter);
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

        public virtual decimal ColdCallThreeBet
        {
            get
            {
                return GetPercentage(Source.DidColdCallThreeBet, Source.CouldColdCallThreeBet);
            }
        }

        public virtual decimal ColdCallFourBet
        {
            get
            {
                return GetPercentage(Source.DidColdCallFourBet, Source.CouldColdCallFourBet);
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
                return GetPositionalPercentage("BB", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
            }
        }

        public virtual decimal FourBetInBTN
        {
            get
            {
                return GetPositionalPercentage("BTN", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
            }
        }

        public virtual decimal FourBetInCO
        {
            get
            {
                return GetPositionalPercentage("CO", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
            }
        }

        public virtual decimal FourBetInMP
        {
            get
            {
                return GetPositionalPercentage("MP", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
            }
        }

        public virtual decimal FourBetInEP
        {
            get
            {
                return GetPositionalPercentage("EP", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
            }
        }

        public virtual decimal FourBetInSB
        {
            get
            {
                return GetPositionalPercentage("SB", x => x.DidfourbetpreflopVirtualCounter, x => x.CouldfourbetpreflopVirtualCounter);
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
            get { return GetPercentage(Source.LimpEp, Source.LimpPossible); }
        }

        public virtual decimal LimpMp
        {
            get { return GetPercentage(Source.LimpMp, Source.LimpPossible); }
        }

        public virtual decimal LimpCo
        {
            get { return GetPercentage(Source.LimpCo, Source.LimpPossible); }
        }

        public virtual decimal LimpBtn
        {
            get { return GetPercentage(Source.LimpBtn, Source.LimpPossible); }
        }

        public virtual decimal LimpSb
        {
            get { return GetPercentage(Source.LimpSb, Source.LimpPossible); }
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

        public virtual decimal CheckFoldFlopPfrOop => GetPercentage(Source.CheckFoldFlopPfrOop, Source.PfrOop);

        public virtual decimal CheckFoldFlop3BetOop => GetPercentage(Source.CheckFoldFlop3BetOop, Source.DidThreeBetOop);

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

                return stdDev.HasValue ? stdDev.Value : 0m;
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

                return stdDevBB.HasValue ? stdDevBB.Value : 0m;
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

        public virtual void UpdateSource(Playerstatistic statistic)
        {
            Source = statistic;
        }

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
        }

        public virtual void Clean()
        {
            Source = new Playerstatistic();
            Statistics = new List<Playerstatistic>();
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

        #region Helpers

        protected virtual decimal GetPercentage(decimal actual, decimal possible)
        {
            if (TotalHands == 0)
            {
                return 0;
            }

            if (possible == 0)
            {
                return 0;
            }

            return (actual / possible) * 100;
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

        protected virtual void CalculateStdDeviation()
        {
            if (Statistics.Count == 0)
            {
                return;
            }

            var statistic = Statistics.ToArray();

            var netWonCollection = statistic
                .Select(x => new
                {
                    NetWon = x.NetWon,
                    NetWonInBB = GetDivisionResult(x.NetWon, x.BigBlind)
                })
                .ToArray();

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

        protected virtual decimal CalculateNetWonPerHour()
        {
            if (Statistics == null || Statistics.Count == 0)
            {
                return 0m;
            }

            var totalMinutes = 0d;

            DateTime? sessionStart = null;
            DateTime? sessionEnd = null;

            foreach (var playerstatistic in Statistics.OrderBy(x => x.Time).ToArray())
            {
                if (Utils.IsDateInDateRange(playerstatistic.Time, sessionStart, sessionEnd, TimeSpan.FromMinutes(30)))
                {
                    if (!sessionStart.HasValue)
                    {
                        sessionStart = playerstatistic.Time;
                    }

                    sessionEnd = playerstatistic.Time;
                }
                else
                {
                    totalMinutes += (sessionEnd.Value - sessionStart.Value).TotalMinutes;

                    sessionStart = playerstatistic.Time;
                    sessionEnd = playerstatistic.Time;
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

        #endregion        
    }
}