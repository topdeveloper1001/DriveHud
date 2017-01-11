using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Model.Importer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Model.Data
{
    /// <summary>
    /// This entity holds all high level indicators. These indicators are based on PlayerStatistic fields
    /// </summary>
    public class Indicators : INotifyPropertyChanged, IComparable
    {
        public Indicators()
        {
            Source = new Playerstatistic();
            Statistics = new List<Playerstatistic>();
        }

        public Indicators(IEnumerable<Playerstatistic> playerStatistic) : this()
        {
            playerStatistic.ForEach(x => AddStatistic(x));
        }

        public Playerstatistic Source { get; set; }

        public List<Playerstatistic> Statistics { get; private set; }

        protected decimal GetPercentage(decimal actual, decimal possible)
        {
            if (TotalHands == 0)
                return 0;

            if (possible == 0)
                return 0;

            return (actual / possible) * 100;
        }

        protected decimal GetDevisionResult(decimal actual, decimal possible)
        {
            if (TotalHands == 0)
                return 0;

            if (possible == 0)
                return 0;

            return (actual / possible);
        }

        public virtual decimal BB
        {
            get
            {
                // to prevent errors - collection was modified
                var statistic = Statistics.ToArray();

                decimal totalhands = statistic.Count() / (decimal)100;
                decimal netwon = statistic.Sum(x => GetDevisionResult(x.NetWon, x.BigBlind));

                return Math.Round(GetDevisionResult(netwon, totalhands), 2);
            }
        }

        public virtual decimal VPIP
        {
            get
            {
                return GetPercentage(Source.Vpiphands, TotalHands);
            }
        }

        public virtual decimal PFR
        {
            get
            {
                return GetPercentage(Source.Pfrhands, TotalHands);
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
                return GetPercentage(Source.Didfourbet, Source.Couldfourbet);
            }
        }

        public virtual decimal FourBetRange
        {
            get
            {
                return (PFR * FourBet) / 100;
            }
        }

        public virtual decimal ThreeBetCall
        {
            get
            {
                return GetPercentage(Source.Calledthreebetpreflop, Source.Facedthreebetpreflop);
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
                return GetPercentage(Source.Foldedtothreebetpreflop, Source.Facedthreebetpreflop);
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

        public virtual decimal TotalHands
        {
            get { return Source.Totalhands; }
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

        public virtual string SessionStart
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return "";
                }
                return StringFormatter.GetDateTimeString(Statistics.Min(x => x.Time));
            }
        }

        public virtual string SessionLength
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return "";
                }
                var length = Statistics.Max(x => x.Time) - Statistics.Min(x => x.Time);
                return String.Format("{0}:{1:00}", (int)length.TotalHours, (int)length.Minutes);
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
                return GetPercentage(Source.Didsqueeze, TotalHands);
            }
        }

        public virtual decimal CheckRaise
        {
            get
            {
                return GetPercentage(Source.DidCheckRaise, TotalHands);
            }
        }

        public virtual decimal FlopCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidFlopCheckRaise, Source.Sawflop);
            }
        }

        public virtual decimal TurnCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidTurnCheckRaise, Source.SawTurn);
            }
        }

        public virtual decimal RiverCheckRaise
        {
            get
            {
                return GetPercentage(Source.DidRiverCheckRaise, Source.SawRiver);
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

        public virtual decimal ThreeBetInBB
        {
            get
            {
                return GetPercentage(Source.DidThreeBetInBb, Source.Couldthreebet);
            }
        }

        public virtual decimal ThreeBetInBTN
        {
            get
            {
                return GetPercentage(Source.DidThreeBetInBtn, Source.Couldthreebet);
            }
        }

        public virtual decimal ThreeBetInCO
        {
            get
            {
                return GetPercentage(Source.DidThreeBetInCo, Source.Couldthreebet);
            }
        }

        public virtual decimal ThreeBetInMP
        {
            get
            {
                return GetPercentage(Source.DidThreeBetInMp, Source.Couldthreebet);
            }
        }

        public virtual decimal ThreeBetInSB
        {
            get
            {
                return GetPercentage(Source.DidThreeBetInSb, Source.Couldthreebet);
            }
        }

        public virtual decimal FourBetInBB
        {
            get
            {
                return GetPercentage(Source.DidFourBetInBb, Source.Couldfourbet);
            }
        }

        public virtual decimal FourBetInBTN
        {
            get
            {
                return GetPercentage(Source.DidFourBetInBtn, Source.Couldfourbet);
            }
        }

        public virtual decimal FourBetInCO
        {
            get
            {
                return GetPercentage(Source.DidFourBetInCo, Source.Couldfourbet);
            }
        }

        public virtual decimal FourBetInMP
        {
            get
            {
                return GetPercentage(Source.DidFourBetInMp, Source.Couldfourbet);
            }
        }

        public virtual decimal FourBetInSB
        {
            get
            {
                return GetPercentage(Source.DidFourBetInSb, Source.Couldfourbet);
            }
        }

        public virtual decimal ColdCallInBB
        {
            get
            {
                return GetPercentage(Source.DidColdCallInBb, Source.Couldcoldcall);
            }
        }

        public virtual decimal ColdCallInBTN
        {
            get
            {
                return GetPercentage(Source.DidColdCallInBtn, Source.Couldcoldcall);
            }
        }

        public virtual decimal ColdCallInCO
        {
            get
            {
                return GetPercentage(Source.DidColdCallInCo, Source.Couldcoldcall);
            }
        }

        public virtual decimal ColdCallInMP
        {
            get
            {
                return GetPercentage(Source.DidColdCallInMp, Source.Couldcoldcall);
            }
        }

        public virtual decimal ColdCallInSB
        {
            get
            {
                return GetPercentage(Source.DidColdCallInSb, Source.Couldcoldcall);
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

        #region Unopened Pot

        public virtual decimal UO_PFR_EP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_EP,
                    Statistics.Count(x => x.PositionString == "EP" && PlayerStatisticCalculator.IsUnopened(x)));
            }
        }
        public virtual decimal UO_PFR_MP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_MP,
                    Statistics.Count(x => x.PositionString == "MP" && PlayerStatisticCalculator.IsUnopened(x)));
            }
        }
        public virtual decimal UO_PFR_CO
        {
            get
            {
                return GetPercentage(Source.UO_PFR_CO,
                    Statistics.Count(x => x.PositionString == "CO" && PlayerStatisticCalculator.IsUnopened(x)));
            }
        }
        public virtual decimal UO_PFR_BN
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BN,
                    Statistics.Count(x => x.PositionString == "BTN" && PlayerStatisticCalculator.IsUnopened(x)));
            }
        }
        public virtual decimal UO_PFR_SB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_SB,
                    Statistics.Count(x => x.PositionString == "SB" && PlayerStatisticCalculator.IsUnopened(x)));
            }
        }
        public virtual decimal UO_PFR_BB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BB,
                    Statistics.Count(x => x.PositionString == "BB" && PlayerStatisticCalculator.IsUnopened(x)));
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
        public virtual decimal BetFoldFlopPfrRaiser => GetPercentage(Source.BetFoldFlopPfrRaiser, Source.Pfrhands);
        public virtual decimal BetFlopCalled3BetPreflopIp => GetPercentage(Source.BetFlopCalled3BetPreflopIp, TotalHands);
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
            get { return ((EnumPokerSites)Source.PokersiteId).ToString(); }
        }

        public virtual string HourOfHand
        {
            get
            {
                if (Statistics == null || !Statistics.Any())
                {
                    return "";
                }
                var sessionStartHour = Converter.ToLocalizedDateTime(Statistics.Min(x => x.Time)).Hour;
                var sessionEndHour = Converter.ToLocalizedDateTime(Statistics.Max(x => x.Time)).Hour;
                return String.Format("{0}:00 - {1}:59", sessionStartHour, sessionEndHour);
            }
        }

        public virtual void UpdateSource(Playerstatistic statistic)
        {
            Source = statistic;
        }

        public virtual void UpdateSource(IList<Playerstatistic> statistics)
        {
            foreach (var statistic in statistics)
                AddStatistic(statistic);
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

        // required to avoid binding leaks
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
