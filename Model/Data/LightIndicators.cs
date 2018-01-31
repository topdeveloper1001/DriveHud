//-----------------------------------------------------------------------
// <copyright file="LightIndicators.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Model.Importer;
using Model.Reports;
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Model.Data
{
    [ProtoContract]
    [ProtoInclude(100, typeof(OpponentReportIndicators))]
    public class LightIndicators : Indicators
    {
        [ProtoMember(1)]
        private int statisticCount;

        [ProtoMember(2)]
        private decimal netWon;

        [ProtoMember(3)]
        private decimal bigBlind;

        [ProtoMember(4)]
        private decimal netWonByBigBlind;

        [ProtoMember(5)]
        private DateTime sessionStartTime = DateTime.MaxValue;

        [ProtoMember(6)]
        private DateTime sessionEndTime = DateTime.MinValue;

        public LightIndicators() : base()
        {
        }

        public LightIndicators(IEnumerable<Playerstatistic> playerStatistic) : base(playerStatistic)
        {
        }

        #region overridden properties

        public override int StatisticsCount
        {
            get
            {
                return statisticCount;
            }
        }

        public override decimal BB
        {
            get
            {
                var totalhands = statisticCount / 100m;
                return Math.Round(GetDivisionResult(netWonByBigBlind, totalhands), 2);
            }
        }

        public override decimal NetWon
        {
            get
            {
                return netWon;
            }
        }

        public override DateTime? SessionStart
        {
            get
            {
                if (statisticCount == 0)
                {
                    return null;
                }

                return sessionStartTime;
            }
        }

        public virtual DateTime? SessionEnd
        {
            get
            {
                if (statisticCount == 0)
                {
                    return null;
                }

                return sessionEndTime;
            }
        }

        public override string SessionLength
        {
            get
            {
                if (statisticCount == 0)
                {
                    return string.Empty;
                }

                var length = sessionEndTime - sessionStartTime;

                return string.Format("{0}:{1:00}", (int)length.TotalHours, length.Minutes);
            }
        }

        public override string HourOfHand
        {
            get
            {
                if (statisticCount == 0)
                {
                    return string.Empty;
                }

                var sessionStartHour = Converter.ToLocalizedDateTime(sessionStartTime).Hour;
                var sessionEndHour = Converter.ToLocalizedDateTime(sessionEndTime).Hour;

                return string.Format("{0}:00 - {1}:59", sessionStartHour, sessionEndHour);
            }
        }

        #region Positional VPIP

        public override decimal VPIP_EP
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.EP, Source.PositionTotal?.EP);
            }
        }

        public override decimal VPIP_MP
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.MP, Source.PositionTotal?.MP);
            }
        }

        public override decimal VPIP_CO
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.CO, Source.PositionTotal?.CO);
            }
        }

        public override decimal VPIP_BN
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.BN, Source.PositionTotal?.BN);
            }
        }

        public override decimal VPIP_SB
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.SB, Source.PositionTotal?.SB);
            }
        }

        public override decimal VPIP_BB
        {
            get
            {
                return GetPercentage(Source.PositionVPIP?.BB, Source.PositionTotal?.BB);
            }
        }

        #endregion

        #region Positional PFR

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

        #endregion

        #region Positional 3-Bet

        public override decimal ThreeBet_EP
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.EP, Source.PositionCouldColdCall?.EP);
            }
        }

        public override decimal ThreeBet_MP
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.MP, Source.PositionCouldThreeBet?.MP);
            }
        }

        public override decimal ThreeBet_CO
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.CO, Source.PositionCouldThreeBet?.CO);
            }
        }

        public override decimal ThreeBet_BN
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.BN, Source.PositionCouldThreeBet?.BN);
            }
        }

        public override decimal ThreeBet_SB
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.SB, Source.PositionCouldThreeBet?.SB);
            }
        }

        public override decimal ThreeBet_BB
        {
            get
            {
                return GetPercentage(Source.PositionDidThreeBet?.BB, Source.PositionCouldThreeBet?.BB);
            }
        }

        #endregion

        #region Positional 4-Bet

        public override decimal FourBetInBB
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.BB, Source.PositionCouldFourBet?.BB);
            }
        }

        public override decimal FourBetInBTN
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.BN, Source.PositionCouldFourBet?.BN);
            }
        }

        public override decimal FourBetInCO
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.CO, Source.PositionCouldFourBet?.CO);
            }
        }

        public override decimal FourBetInMP
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.MP, Source.PositionCouldFourBet?.MP);
            }
        }

        public override decimal FourBetInEP
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.EP, Source.PositionCouldFourBet?.EP);
            }
        }

        public override decimal FourBetInSB
        {
            get
            {
                return GetPercentage(Source.PositionDidFourBet?.SB, Source.PositionCouldFourBet?.SB);
            }
        }

        #endregion

        #region Positional Cold call

        public override decimal ColdCall_EP
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.EP, Source.PositionCouldColdCall?.EP);
            }
        }
        public override decimal ColdCall_MP
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.MP, Source.PositionCouldColdCall?.MP);
            }
        }
        public override decimal ColdCall_CO
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.CO, Source.PositionCouldColdCall?.CO);
            }
        }
        public override decimal ColdCall_BN
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.BN, Source.PositionCouldColdCall?.BN);
            }
        }
        public override decimal ColdCall_SB
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.SB, Source.PositionCouldColdCall?.SB);
            }
        }
        public override decimal ColdCall_BB
        {
            get
            {
                return GetPercentage(Source.PositionDidColdCall?.BB, Source.PositionCouldColdCall?.BB);
            }
        }

        #endregion

        #region IP/OOP based stats

        [ProtoMember(7)]
        protected int didDelayedTurnCBetIP;

        [ProtoMember(8)]
        protected int couldDelayedTurnCBetIP;

        public override decimal DelayedTurnCBetIP
        {
            get
            {
                return GetPercentage(didDelayedTurnCBetIP, couldDelayedTurnCBetIP);
            }
        }

        [ProtoMember(9)]
        protected int didDelayedTurnCBetOOP;

        [ProtoMember(10)]
        protected int couldDelayedTurnCBetOOP;

        public override decimal DelayedTurnCBetOOP
        {
            get
            {
                return GetPercentage(didDelayedTurnCBetOOP, couldDelayedTurnCBetOOP);
            }
        }

        #endregion

        #region Calculated stats

        [ProtoMember(11)]
        protected int checkRaisedFlopCBet;

        [ProtoMember(12)]
        protected int couldCheckRaiseFlopCBet;

        public override decimal CheckRaisedFlopCBet
        {
            get
            {
                return GetPercentage(checkRaisedFlopCBet, couldCheckRaiseFlopCBet);
            }
        }

        [ProtoMember(13)]
        protected int foldToTurnCBetIn3BetPot;

        [ProtoMember(14)]
        protected int facedToTurnCBetIn3BetPot;

        public override decimal FoldToTurnCBetIn3BetPot
        {
            get
            {
                return GetPercentage(foldToTurnCBetIn3BetPot, facedToTurnCBetIn3BetPot);
            }
        }

        [ProtoMember(15)]
        protected int raisedFlopCBetIn3BetPot;

        [ProtoMember(16)]
        protected int couldRaiseFlopCBetIn3BetPot;

        public override decimal RaiseFlopCBetIn3BetPot
        {
            get
            {
                return GetPercentage(raisedFlopCBetIn3BetPot, couldRaiseFlopCBetIn3BetPot);
            }
        }

        [ProtoMember(17)]
        protected int didFlopBet;

        [ProtoMember(18)]
        protected int didTurnBet;

        #region FlopBetSize stats

        [ProtoMember(19)]
        protected int flopBetSizeOneHalfOrLess;

        public override decimal FlopBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneHalfOrLess, didFlopBet);
            }
        }

        [ProtoMember(20)]
        protected int flopBetSizeOneQuarterOrLess;

        public override decimal FlopBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneQuarterOrLess, didFlopBet);
            }
        }

        [ProtoMember(21)]
        protected int flopBetSizeTwoThirdsOrLess;

        public override decimal FlopBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeTwoThirdsOrLess, didFlopBet);
            }
        }

        [ProtoMember(22)]
        protected int flopBetSizeThreeQuartersOrLess;

        public override decimal FlopBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeThreeQuartersOrLess, didFlopBet);
            }
        }

        [ProtoMember(23)]
        protected int flopBetSizeOneOrLess;

        public override decimal FlopBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneOrLess, didFlopBet);
            }
        }

        [ProtoMember(24)]
        protected int flopBetSizeMoreThanOne;

        public override decimal FlopBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(flopBetSizeMoreThanOne, didFlopBet);
            }
        }

        #endregion

        #region TurnBetSize stats

        [ProtoMember(25)]
        protected int turnBetSizeOneHalfOrLess;

        public override decimal TurnBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneHalfOrLess, didTurnBet);
            }
        }

        [ProtoMember(26)]
        protected int turnBetSizeOneQuarterOrLess;

        public override decimal TurnBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneQuarterOrLess, didTurnBet);
            }
        }

        [ProtoMember(27)]
        protected int turnBetSizeOneThirdOrLess;

        public override decimal TurnBetSizeOneThirdOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneThirdOrLess, didTurnBet);
            }
        }

        [ProtoMember(28)]
        protected int turnBetSizeTwoThirdsOrLess;

        public override decimal TurnBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeTwoThirdsOrLess, didTurnBet);
            }
        }

        [ProtoMember(29)]
        protected int turnBetSizeThreeQuartersOrLess;

        public override decimal TurnBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeThreeQuartersOrLess, didTurnBet);
            }
        }

        [ProtoMember(30)]
        protected int turnBetSizeOneOrLess;

        public override decimal TurnBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneOrLess, didTurnBet);
            }
        }

        [ProtoMember(31)]
        protected int turnBetSizeMoreThanOne;

        public override decimal TurnBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(turnBetSizeMoreThanOne, didTurnBet);
            }
        }

        #endregion

        #region RiverBetSize stats

        [ProtoMember(32)]
        protected int riverBetSizeMoreThanOne;

        public override decimal RiverBetSizeMoreThanOne
        {
            get
            {
                return GetPercentage(riverBetSizeMoreThanOne, Source.DidRiverBet);
            }
        }

        #endregion

        #region WTSD After stats

        [ProtoMember(33)]
        protected int wtsdAfterCalling3Bet;

        [ProtoMember(34)]
        protected int wtsdAfterCalling3BetOpportunity;

        public override decimal WTSDAfterCalling3Bet
        {
            get
            {
                return GetPercentage(wtsdAfterCalling3Bet, wtsdAfterCalling3BetOpportunity);
            }
        }

        [ProtoMember(35)]
        protected int wtsdAfterCallingPfr;

        [ProtoMember(36)]
        protected int wtsdAfterCallingPfrOpportunity;

        public override decimal WTSDAfterCallingPfr
        {
            get
            {
                return GetPercentage(wtsdAfterCallingPfr, wtsdAfterCallingPfrOpportunity);
            }
        }

        [ProtoMember(37)]
        protected int wtsdAfterNotCBettingFlopAsPfr;

        [ProtoMember(38)]
        protected int wtsdAfterNotCBettingFlopAsPfrOpportunity;

        public override decimal WTSDAfterNotCBettingFlopAsPfr
        {
            get
            {
                return GetPercentage(wtsdAfterNotCBettingFlopAsPfr, wtsdAfterNotCBettingFlopAsPfrOpportunity);
            }
        }

        [ProtoMember(39)]
        protected int wtsdAfterSeeingTurn;

        public override decimal WTSDAfterSeeingTurn
        {
            get
            {
                return GetPercentage(wtsdAfterSeeingTurn, Source.SawTurn);
            }
        }

        [ProtoMember(40)]
        protected int wtsdAsPF3Bettor;

        [ProtoMember(41)]
        protected int wtsdAsPF3BettorOpportunity;

        public override decimal WTSDAsPF3Bettor
        {
            get
            {
                return GetPercentage(wtsdAsPF3Bettor, wtsdAsPF3BettorOpportunity);
            }
        }

        #endregion

        #endregion

        #endregion

        #region overridden methods

        public override void AddStatistic(Playerstatistic statistic)
        {
            statistic.CalculatePositionalStats();

            Source += statistic;

            statisticCount++;
            netWon += statistic.NetWon;
            bigBlind += statistic.BigBlind;
            netWonByBigBlind += GetDivisionResult(statistic.NetWon, statistic.BigBlind);

            if (sessionStartTime > statistic.Time)
            {
                sessionStartTime = statistic.Time;
            }

            if (sessionEndTime < statistic.Time)
            {
                sessionEndTime = statistic.Time;
            }

            didDelayedTurnCBetIP += statistic.DidDelayedTurnCBetIP;
            couldDelayedTurnCBetIP += statistic.CouldDelayedTurnCBetIP;
            didDelayedTurnCBetOOP += statistic.DidDelayedTurnCBetOOP;
            couldDelayedTurnCBetOOP += statistic.CouldDelayedTurnCBetOOP;
            checkRaisedFlopCBet += statistic.CheckRaisedFlopCBet;
            couldCheckRaiseFlopCBet += statistic.CouldCheckRaiseFlopCBet;

            didFlopBet += statistic.DidFlopBet;
            flopBetSizeMoreThanOne += statistic.FlopBetSizeMoreThanOne;
            flopBetSizeOneHalfOrLess += statistic.FlopBetSizeOneHalfOrLess;
            flopBetSizeOneQuarterOrLess += statistic.FlopBetSizeOneQuarterOrLess;
            flopBetSizeTwoThirdsOrLess += statistic.FlopBetSizeTwoThirdsOrLess;
            flopBetSizeThreeQuartersOrLess += statistic.FlopBetSizeThreeQuartersOrLess;
            flopBetSizeOneOrLess += statistic.FlopBetSizeOneOrLess;

            didTurnBet += statistic.DidTurnBet;
            turnBetSizeMoreThanOne += statistic.TurnBetSizeMoreThanOne;
            turnBetSizeOneHalfOrLess += statistic.TurnBetSizeOneHalfOrLess;
            turnBetSizeOneQuarterOrLess += statistic.TurnBetSizeOneQuarterOrLess;
            turnBetSizeOneThirdOrLess += statistic.TurnBetSizeOneThirdOrLess;
            turnBetSizeTwoThirdsOrLess += statistic.TurnBetSizeTwoThirdsOrLess;
            turnBetSizeThreeQuartersOrLess += statistic.TurnBetSizeThreeQuartersOrLess;
            turnBetSizeOneOrLess += statistic.TurnBetSizeOneOrLess;

            riverBetSizeMoreThanOne += statistic.RiverBetSizeMoreThanOne;

            wtsdAfterCalling3Bet += statistic.WTSDAfterCalling3Bet;
            wtsdAfterCalling3BetOpportunity += statistic.WTSDAfterCalling3BetOpportunity;
            wtsdAfterCallingPfr += statistic.WTSDAfterCallingPfr;
            wtsdAfterCallingPfrOpportunity += statistic.WTSDAfterCallingPfrOpportunity;
            wtsdAfterNotCBettingFlopAsPfr += statistic.WTSDAfterNotCBettingFlopAsPfr;
            wtsdAfterNotCBettingFlopAsPfrOpportunity += statistic.WTSDAfterNotCBettingFlopAsPfrOpportunity;
            wtsdAfterSeeingTurn += statistic.WTSDAfterSeeingTurn;
            wtsdAsPF3Bettor += statistic.WTSDAsPF3Bettor;
            wtsdAsPF3BettorOpportunity += statistic.WTSDAsPF3BettorOpportunity;

            foldToTurnCBetIn3BetPot += statistic.FoldToTurnCBetIn3BetPot;
            facedToTurnCBetIn3BetPot += statistic.FacedToTurnCBetIn3BetPot;

            raisedFlopCBetIn3BetPot += statistic.RaisedFlopCBetIn3BetPot;
            couldRaiseFlopCBetIn3BetPot += statistic.CouldRaiseFlopCBetIn3BetPot;
        }

        public override void Clean()
        {
            base.Clean();

            statisticCount = 0;
            netWon = 0;
            bigBlind = 0;
            netWonByBigBlind = 0;
            sessionStartTime = DateTime.MaxValue;
            sessionEndTime = DateTime.MinValue;

            didDelayedTurnCBetIP = 0;
            couldDelayedTurnCBetIP = 0;
            didDelayedTurnCBetOOP = 0;
            couldDelayedTurnCBetOOP = 0;
            checkRaisedFlopCBet = 0;
            couldCheckRaiseFlopCBet = 0;

            didFlopBet = 0;
            flopBetSizeMoreThanOne = 0;
            flopBetSizeOneHalfOrLess = 0;
            flopBetSizeOneQuarterOrLess = 0;
            flopBetSizeTwoThirdsOrLess = 0;
            flopBetSizeThreeQuartersOrLess = 0;
            flopBetSizeOneOrLess = 0;

            didTurnBet = 0;
            turnBetSizeMoreThanOne = 0;
            turnBetSizeOneHalfOrLess = 0;
            turnBetSizeOneQuarterOrLess = 0;
            turnBetSizeOneThirdOrLess = 0;
            turnBetSizeTwoThirdsOrLess = 0;
            turnBetSizeThreeQuartersOrLess = 0;
            turnBetSizeOneOrLess = 0;

            riverBetSizeMoreThanOne = 0;

            wtsdAfterCalling3Bet = 0;
            wtsdAfterCalling3BetOpportunity = 0;
            wtsdAfterCallingPfr = 0;
            wtsdAfterCallingPfrOpportunity = 0;
            wtsdAfterNotCBettingFlopAsPfr = 0;
            wtsdAfterNotCBettingFlopAsPfrOpportunity = 0;
            wtsdAfterSeeingTurn = 0;
            wtsdAsPF3Bettor = 0;
            wtsdAsPF3BettorOpportunity = 0;

            foldToTurnCBetIn3BetPot = 0;
            facedToTurnCBetIn3BetPot = 0;

            raisedFlopCBetIn3BetPot = 0;
            couldRaiseFlopCBetIn3BetPot = 0;
        }

        #endregion

        #region Help methods

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

        #endregion
    }
}