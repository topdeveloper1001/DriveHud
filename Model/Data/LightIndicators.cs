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
using ProtoBuf;
using System;
using System.Collections.Generic;

namespace Model.Data
{
    [ProtoContract]
    [ProtoInclude(300, typeof(ReportIndicators))]
    public class LightIndicators : Indicators
    {
        [ProtoMember(1)]
        protected int statisticCount;

        [ProtoMember(2)]
        protected decimal netWon;

        [ProtoMember(3)]
        protected decimal bigBlind;

        [ProtoMember(4)]
        protected decimal netWonByBigBlind;

        [ProtoMember(5)]
        protected decimal evInBB;

        [ProtoMember(6)]
        protected DateTime sessionStartTime = DateTime.MaxValue;

        [ProtoMember(7)]
        protected DateTime sessionEndTime = DateTime.MinValue;

        [ProtoMember(43)]
        protected long gameNumberMax;

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

        public override decimal EVBB
        {
            get
            {
                var totalhands = statisticCount / 100m;
                return Math.Round(GetDivisionResult(evInBB, totalhands), 2);
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
                return GetPercentage(positionVPIP?.EP, positionTotal?.EP);
            }
        }

        public override decimal VPIP_MP
        {
            get
            {
                return GetPercentage(positionVPIP?.MP, positionTotal?.MP);
            }
        }

        public override decimal VPIP_CO
        {
            get
            {
                return GetPercentage(positionVPIP?.CO, positionTotal?.CO);
            }
        }

        public override decimal VPIP_BN
        {
            get
            {
                return GetPercentage(positionVPIP?.BN, positionTotal?.BN);
            }
        }

        public override decimal VPIP_SB
        {
            get
            {
                return GetPercentage(positionVPIP?.SB, positionTotal?.SB);
            }
        }

        public override decimal VPIP_BB
        {
            get
            {
                return GetPercentage(positionVPIP?.BB, positionTotal?.BB);
            }
        }

        #endregion

        #region Positional PFR

        public override decimal UO_PFR_EP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_EP, positionUnoppened?.EP);
            }
        }

        public override decimal UO_PFR_MP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_MP, positionUnoppened?.MP);
            }
        }

        public override decimal UO_PFR_CO
        {
            get
            {
                return GetPercentage(Source.UO_PFR_CO, positionUnoppened?.CO);
            }
        }

        public override decimal UO_PFR_BN
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BN, positionUnoppened?.BN);
            }
        }

        public override decimal UO_PFR_SB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_SB, positionUnoppened?.SB);
            }
        }

        public override decimal UO_PFR_BB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BB, positionUnoppened?.BB);
            }
        }

        #endregion

        #region Positional 3-Bet

        public override decimal ThreeBet_EP
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.EP, positionCouldColdCall?.EP);
            }
        }

        public override decimal ThreeBet_MP
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.MP, positionCouldThreeBet?.MP);
            }
        }

        public override decimal ThreeBet_CO
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.CO, positionCouldThreeBet?.CO);
            }
        }

        public override decimal ThreeBet_BN
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.BN, positionCouldThreeBet?.BN);
            }
        }

        public override decimal ThreeBet_SB
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.SB, positionCouldThreeBet?.SB);
            }
        }

        public override decimal ThreeBet_BB
        {
            get
            {
                return GetPercentage(positionDidThreeBet?.BB, positionCouldThreeBet?.BB);
            }
        }

        #endregion

        #region Positional 4-Bet

        public override decimal FourBetInBB
        {
            get
            {
                return GetPercentage(positionDidFourBet?.BB, positionCouldFourBet?.BB);
            }
        }

        public override decimal FourBetInBTN
        {
            get
            {
                return GetPercentage(positionDidFourBet?.BN, positionCouldFourBet?.BN);
            }
        }

        public override decimal FourBetInCO
        {
            get
            {
                return GetPercentage(positionDidFourBet?.CO, positionCouldFourBet?.CO);
            }
        }

        public override decimal FourBetInMP
        {
            get
            {
                return GetPercentage(positionDidFourBet?.MP, positionCouldFourBet?.MP);
            }
        }

        public override decimal FourBetInEP
        {
            get
            {
                return GetPercentage(positionDidFourBet?.EP, positionCouldFourBet?.EP);
            }
        }

        public override decimal FourBetInSB
        {
            get
            {
                return GetPercentage(positionDidFourBet?.SB, positionCouldFourBet?.SB);
            }
        }

        #endregion

        #region Positional Cold call

        public override decimal ColdCall_EP
        {
            get
            {
                return GetPercentage(positionDidColdCall?.EP, positionCouldColdCall?.EP);
            }
        }
        public override decimal ColdCall_MP
        {
            get
            {
                return GetPercentage(positionDidColdCall?.MP, positionCouldColdCall?.MP);
            }
        }
        public override decimal ColdCall_CO
        {
            get
            {
                return GetPercentage(positionDidColdCall?.CO, positionCouldColdCall?.CO);
            }
        }
        public override decimal ColdCall_BN
        {
            get
            {
                return GetPercentage(positionDidColdCall?.BN, positionCouldColdCall?.BN);
            }
        }
        public override decimal ColdCall_SB
        {
            get
            {
                return GetPercentage(positionDidColdCall?.SB, positionCouldColdCall?.SB);
            }
        }
        public override decimal ColdCall_BB
        {
            get
            {
                return GetPercentage(positionDidColdCall?.BB, positionCouldColdCall?.BB);
            }
        }

        #endregion

        #region IP/OOP based stats

        [ProtoMember(8)]
        protected int didDelayedTurnCBetIP;

        [ProtoMember(9)]
        protected int couldDelayedTurnCBetIP;

        public override decimal DelayedTurnCBetIP
        {
            get
            {
                return GetPercentage(didDelayedTurnCBetIP, couldDelayedTurnCBetIP);
            }
        }

        [ProtoMember(10)]
        protected int didDelayedTurnCBetOOP;

        [ProtoMember(11)]
        protected int couldDelayedTurnCBetOOP;

        public override decimal DelayedTurnCBetOOP
        {
            get
            {
                return GetPercentage(didDelayedTurnCBetOOP, couldDelayedTurnCBetOOP);
            }
        }

        #endregion

        #region Bet stats

        public override decimal TurnBet
        {
            get
            {
                return GetPercentage(didTurnBet, Source.CouldTurnBet);
            }
        }

        public override decimal FlopBet
        {
            get
            {
                return GetPercentage(didFlopBet, Source.CouldFlopBet);
            }
        }

        #endregion

        #region Calculated stats

        [ProtoMember(12)]
        protected int checkRaisedFlopCBet;

        [ProtoMember(13)]
        protected int couldCheckRaiseFlopCBet;

        public override decimal CheckRaisedFlopCBet
        {
            get
            {
                return GetPercentage(checkRaisedFlopCBet, couldCheckRaiseFlopCBet);
            }
        }

        [ProtoMember(14)]
        protected int foldToTurnCBetIn3BetPot;

        [ProtoMember(15)]
        protected int facedToTurnCBetIn3BetPot;

        public override decimal FoldToTurnCBetIn3BetPot
        {
            get
            {
                return GetPercentage(foldToTurnCBetIn3BetPot, facedToTurnCBetIn3BetPot);
            }
        }

        [ProtoMember(16)]
        protected int raisedFlopCBetIn3BetPot;

        [ProtoMember(17)]
        protected int couldRaiseFlopCBetIn3BetPot;

        public override decimal RaiseFlopCBetIn3BetPot
        {
            get
            {
                return GetPercentage(raisedFlopCBetIn3BetPot, couldRaiseFlopCBetIn3BetPot);
            }
        }

        [ProtoMember(18)]
        protected int didFlopBet;

        [ProtoMember(19)]
        protected int didTurnBet;

        #region FlopBetSize stats

        [ProtoMember(20)]
        protected int flopBetSizeOneHalfOrLess;

        public override decimal FlopBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneHalfOrLess, didFlopBet);
            }
        }

        [ProtoMember(21)]
        protected int flopBetSizeOneQuarterOrLess;

        public override decimal FlopBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneQuarterOrLess, didFlopBet);
            }
        }

        [ProtoMember(22)]
        protected int flopBetSizeTwoThirdsOrLess;

        public override decimal FlopBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeTwoThirdsOrLess, didFlopBet);
            }
        }

        [ProtoMember(23)]
        protected int flopBetSizeThreeQuartersOrLess;

        public override decimal FlopBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeThreeQuartersOrLess, didFlopBet);
            }
        }

        [ProtoMember(24)]
        protected int flopBetSizeOneOrLess;

        public override decimal FlopBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(flopBetSizeOneOrLess, didFlopBet);
            }
        }

        [ProtoMember(25)]
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

        [ProtoMember(26)]
        protected int turnBetSizeOneHalfOrLess;

        public override decimal TurnBetSizeOneHalfOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneHalfOrLess, didTurnBet);
            }
        }

        [ProtoMember(27)]
        protected int turnBetSizeOneQuarterOrLess;

        public override decimal TurnBetSizeOneQuarterOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneQuarterOrLess, didTurnBet);
            }
        }

        [ProtoMember(28)]
        protected int turnBetSizeOneThirdOrLess;

        public override decimal TurnBetSizeOneThirdOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneThirdOrLess, didTurnBet);
            }
        }

        [ProtoMember(29)]
        protected int turnBetSizeTwoThirdsOrLess;

        public override decimal TurnBetSizeTwoThirdsOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeTwoThirdsOrLess, didTurnBet);
            }
        }

        [ProtoMember(30)]
        protected int turnBetSizeThreeQuartersOrLess;

        public override decimal TurnBetSizeThreeQuartersOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeThreeQuartersOrLess, didTurnBet);
            }
        }

        [ProtoMember(31)]
        protected int turnBetSizeOneOrLess;

        public override decimal TurnBetSizeOneOrLess
        {
            get
            {
                return GetPercentage(turnBetSizeOneOrLess, didTurnBet);
            }
        }

        [ProtoMember(32)]
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

        [ProtoMember(33)]
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

        [ProtoMember(34)]
        protected int wtsdAfterCalling3Bet;

        [ProtoMember(35)]
        protected int wtsdAfterCalling3BetOpportunity;

        public override decimal WTSDAfterCalling3Bet
        {
            get
            {
                return GetPercentage(wtsdAfterCalling3Bet, wtsdAfterCalling3BetOpportunity);
            }
        }

        [ProtoMember(36)]
        protected int wtsdAfterCallingPfr;

        [ProtoMember(37)]
        protected int wtsdAfterCallingPfrOpportunity;

        public override decimal WTSDAfterCallingPfr
        {
            get
            {
                return GetPercentage(wtsdAfterCallingPfr, wtsdAfterCallingPfrOpportunity);
            }
        }

        [ProtoMember(38)]
        protected int wtsdAfterNotCBettingFlopAsPfr;

        [ProtoMember(39)]
        protected int wtsdAfterNotCBettingFlopAsPfrOpportunity;

        public override decimal WTSDAfterNotCBettingFlopAsPfr
        {
            get
            {
                return GetPercentage(wtsdAfterNotCBettingFlopAsPfr, wtsdAfterNotCBettingFlopAsPfrOpportunity);
            }
        }

        [ProtoMember(40)]
        protected int wtsdAfterSeeingTurn;

        public override decimal WTSDAfterSeeingTurn
        {
            get
            {
                return GetPercentage(wtsdAfterSeeingTurn, Source.SawTurn);
            }
        }

        [ProtoMember(41)]
        protected int wtsdAsPF3Bettor;

        [ProtoMember(42)]
        protected int wtsdAsPF3BettorOpportunity;

        public override decimal WTSDAsPF3Bettor
        {
            get
            {
                return GetPercentage(wtsdAsPF3Bettor, wtsdAsPF3BettorOpportunity);
            }
        }

        #endregion

        [ProtoMember(44)]
        protected int did4Bet;

        [ProtoMember(45)]
        protected int could4Bet;

        public override decimal FourBet
        {
            get
            {
                return GetPercentage(did4Bet, could4Bet);
            }
        }

        [ProtoMember(46)]
        protected int faced3Bet;

        public override decimal ThreeBetCall
        {
            get
            {
                return GetPercentage(Source.Calledthreebetpreflop, faced3Bet);
            }
        }

        [ProtoMember(47)]
        protected int foldedTo3Bet;

        public override decimal FoldToThreeBet
        {
            get
            {
                return GetPercentage(foldedTo3Bet, faced3Bet);
            }
        }

        #region Raise Limpers

        [ProtoMember(48)]
        protected int raisedLimpers;

        [ProtoMember(49)]
        protected int couldRaiseLimpers;

        public override decimal RaiseLimpers
        {
            get
            {
                return GetPercentage(raisedLimpers, couldRaiseLimpers);
            }
        }




        #endregion

        #region 3-Bet vs Pos stats

        [ProtoMember(50)]
        protected int threeBetMPvsEP;

        [ProtoMember(51)]
        protected int couldThreeBetMPvsEP;

        public override decimal ThreeBetMPvsEP => GetPercentage(threeBetMPvsEP, couldThreeBetMPvsEP);

        [ProtoMember(52)]
        protected int threeBetCOvsEP;

        [ProtoMember(53)]
        protected int couldThreeBetCOvsEP;

        public override decimal ThreeBetCOvsEP => GetPercentage(threeBetCOvsEP, couldThreeBetCOvsEP);

        [ProtoMember(54)]
        protected int threeBetCOvsMP;

        [ProtoMember(55)]
        protected int couldThreeBetCOvsMP;

        public override decimal ThreeBetCOvsMP => GetPercentage(threeBetCOvsMP, couldThreeBetCOvsMP);

        [ProtoMember(56)]
        protected int threeBetBTNvsEP;

        [ProtoMember(57)]
        protected int couldThreeBetBTNvsEP;

        public override decimal ThreeBetBTNvsEP => GetPercentage(threeBetBTNvsEP, couldThreeBetBTNvsEP);

        [ProtoMember(58)]
        protected int threeBetBTNvsMP;

        [ProtoMember(59)]
        protected int couldThreeBetBTNvsMP;

        public override decimal ThreeBetBTNvsMP => GetPercentage(threeBetBTNvsMP, couldThreeBetBTNvsMP);

        [ProtoMember(60)]
        protected int threeBetBTNvsCO;

        [ProtoMember(61)]
        protected int couldThreeBetBTNvsCO;

        public override decimal ThreeBetBTNvsCO => GetPercentage(threeBetBTNvsCO, couldThreeBetBTNvsCO);

        [ProtoMember(62)]
        protected int threeBetSBvsEP;

        [ProtoMember(63)]
        protected int couldThreeBetSBvsEP;

        public override decimal ThreeBetSBvsEP => GetPercentage(threeBetSBvsEP, couldThreeBetSBvsEP);

        [ProtoMember(64)]
        protected int threeBetSBvsMP;

        [ProtoMember(65)]
        protected int couldThreeBetSBvsMP;

        public override decimal ThreeBetSBvsMP => GetPercentage(threeBetSBvsMP, couldThreeBetSBvsMP);

        [ProtoMember(66)]
        protected int threeBetSBvsCO;

        [ProtoMember(67)]
        protected int couldThreeBetSBvsCO;

        public override decimal ThreeBetSBvsCO => GetPercentage(threeBetSBvsCO, couldThreeBetSBvsCO);

        [ProtoMember(68)]
        protected int threeBetSBvsBTN;

        [ProtoMember(69)]
        protected int couldThreeBetSBvsBTN;

        public override decimal ThreeBetSBvsBTN => GetPercentage(threeBetSBvsBTN, couldThreeBetSBvsBTN);

        [ProtoMember(70)]
        protected int threeBetBBvsEP;

        [ProtoMember(71)]
        protected int couldThreeBetBBvsEP;

        public override decimal ThreeBetBBvsEP => GetPercentage(threeBetBBvsEP, couldThreeBetBBvsEP);

        [ProtoMember(72)]
        protected int threeBetBBvsMP;

        [ProtoMember(73)]
        protected int couldThreeBetBBvsMP;

        public override decimal ThreeBetBBvsMP => GetPercentage(threeBetBBvsMP, couldThreeBetBBvsMP);

        [ProtoMember(74)]
        protected int threeBetBBvsCO;

        [ProtoMember(75)]
        protected int couldThreeBetBBvsCO;

        public override decimal ThreeBetBBvsCO => GetPercentage(threeBetBBvsCO, couldThreeBetBBvsCO);

        [ProtoMember(76)]
        protected int threeBetBBvsBTN;

        [ProtoMember(77)]
        protected int couldThreeBetBBvsBTN;

        public override decimal ThreeBetBBvsBTN => GetPercentage(threeBetBBvsBTN, couldThreeBetBBvsBTN);

        [ProtoMember(78)]
        protected int threeBetBBvsSB;

        [ProtoMember(79)]
        protected int couldThreeBetBBvsSB;

        public override decimal ThreeBetBBvsSB => GetPercentage(threeBetBBvsSB, couldThreeBetBBvsSB);

        private void Add3BetVsRaiserInPosStatistic(Playerstatistic statistic)
        {
            threeBetMPvsEP += statistic.ThreeBetMPvsEP;
            couldThreeBetMPvsEP += statistic.CouldThreeBetMPvsEP;

            threeBetCOvsEP += statistic.ThreeBetCOvsEP;
            couldThreeBetCOvsEP += statistic.CouldThreeBetCOvsEP;

            threeBetCOvsMP += statistic.ThreeBetCOvsMP;
            couldThreeBetCOvsMP += statistic.CouldThreeBetCOvsMP;

            threeBetBTNvsEP += statistic.ThreeBetBTNvsEP;
            couldThreeBetBTNvsEP += statistic.CouldThreeBetBTNvsEP;

            threeBetBTNvsMP += statistic.ThreeBetBTNvsMP;
            couldThreeBetBTNvsMP += statistic.CouldThreeBetBTNvsMP;

            threeBetBTNvsCO += statistic.ThreeBetBTNvsCO;
            couldThreeBetBTNvsCO += statistic.CouldThreeBetBTNvsCO;

            threeBetSBvsEP += statistic.ThreeBetSBvsEP;
            couldThreeBetSBvsEP += statistic.CouldThreeBetSBvsEP;

            threeBetSBvsMP += statistic.ThreeBetSBvsMP;
            couldThreeBetSBvsMP += statistic.CouldThreeBetSBvsMP;

            threeBetSBvsCO += statistic.ThreeBetSBvsCO;
            couldThreeBetSBvsCO += statistic.CouldThreeBetSBvsCO;

            threeBetSBvsBTN += statistic.ThreeBetSBvsBTN;
            couldThreeBetSBvsBTN += statistic.CouldThreeBetSBvsBTN;

            threeBetBBvsEP += statistic.ThreeBetBBvsEP;
            couldThreeBetBBvsEP += statistic.CouldThreeBetBBvsEP;

            threeBetBBvsMP += statistic.ThreeBetBBvsMP;
            couldThreeBetBBvsMP += statistic.CouldThreeBetBBvsMP;

            threeBetBBvsCO += statistic.ThreeBetBBvsCO;
            couldThreeBetBBvsCO += statistic.CouldThreeBetBBvsCO;

            threeBetBBvsBTN += statistic.ThreeBetBBvsBTN;
            couldThreeBetBBvsBTN += statistic.CouldThreeBetBBvsBTN;

            threeBetBBvsSB += statistic.ThreeBetBBvsSB;
            couldThreeBetBBvsSB += statistic.CouldThreeBetBBvsSB;
        }

        private void Clean3BetVsRaiserInPos()
        {
            threeBetMPvsEP = 0;
            couldThreeBetMPvsEP = 0;

            threeBetCOvsEP = 0;
            couldThreeBetCOvsEP = 0;

            threeBetCOvsMP = 0;
            couldThreeBetCOvsMP = 0;

            threeBetBTNvsEP = 0;
            couldThreeBetBTNvsEP = 0;

            threeBetBTNvsMP = 0;
            couldThreeBetBTNvsMP = 0;

            threeBetBTNvsCO = 0;
            couldThreeBetBTNvsCO = 0;

            threeBetSBvsEP = 0;
            couldThreeBetSBvsEP = 0;

            threeBetSBvsMP = 0;
            couldThreeBetSBvsMP = 0;

            threeBetSBvsCO = 0;
            couldThreeBetSBvsCO = 0;

            threeBetSBvsBTN = 0;
            couldThreeBetSBvsBTN = 0;

            threeBetBBvsEP = 0;
            couldThreeBetBBvsEP = 0;

            threeBetBBvsMP = 0;
            couldThreeBetBBvsMP = 0;

            threeBetBBvsCO = 0;
            couldThreeBetBBvsCO = 0;

            threeBetBBvsBTN = 0;
            couldThreeBetBBvsBTN = 0;

            threeBetBBvsSB = 0;
            couldThreeBetBBvsSB = 0;
        }

        private void Add3BetVsRaiserInPosIndicator(LightIndicators indicator)
        {
            threeBetMPvsEP += indicator.threeBetMPvsEP;
            couldThreeBetMPvsEP += indicator.couldThreeBetMPvsEP;

            threeBetCOvsEP += indicator.threeBetCOvsEP;
            couldThreeBetCOvsEP += indicator.couldThreeBetCOvsEP;

            threeBetCOvsMP += indicator.threeBetCOvsMP;
            couldThreeBetCOvsMP += indicator.couldThreeBetCOvsMP;

            threeBetBTNvsEP += indicator.threeBetBTNvsEP;
            couldThreeBetBTNvsEP += indicator.couldThreeBetBTNvsEP;

            threeBetBTNvsMP += indicator.threeBetBTNvsMP;
            couldThreeBetBTNvsMP += indicator.couldThreeBetBTNvsMP;

            threeBetBTNvsCO += indicator.threeBetBTNvsCO;
            couldThreeBetBTNvsCO += indicator.couldThreeBetBTNvsCO;

            threeBetSBvsEP += indicator.threeBetSBvsEP;
            couldThreeBetSBvsEP += indicator.couldThreeBetSBvsEP;

            threeBetSBvsMP += indicator.threeBetSBvsMP;
            couldThreeBetSBvsMP += indicator.couldThreeBetSBvsMP;

            threeBetSBvsCO += indicator.threeBetSBvsCO;
            couldThreeBetSBvsCO += indicator.couldThreeBetSBvsCO;

            threeBetSBvsBTN += indicator.threeBetSBvsBTN;
            couldThreeBetSBvsBTN += indicator.couldThreeBetSBvsBTN;

            threeBetBBvsEP += indicator.threeBetBBvsEP;
            couldThreeBetBBvsEP += indicator.couldThreeBetBBvsEP;

            threeBetBBvsMP += indicator.threeBetBBvsMP;
            couldThreeBetBBvsMP += indicator.couldThreeBetBBvsMP;

            threeBetBBvsCO += indicator.threeBetBBvsCO;
            couldThreeBetBBvsCO += indicator.couldThreeBetBBvsCO;

            threeBetBBvsBTN += indicator.threeBetBBvsBTN;
            couldThreeBetBBvsBTN += indicator.couldThreeBetBBvsBTN;

            threeBetBBvsSB += indicator.threeBetBBvsSB;
            couldThreeBetBBvsSB += indicator.couldThreeBetBBvsSB;
        }

        #endregion

        #region Fold to 3-Bet in Pos vs 3-bet Pos

        [ProtoMember(80)]
        protected int foldTo3BetInEPvs3BetMP;

        [ProtoMember(81)]
        protected int couldFoldTo3BetInEPvs3BetMP;

        public override decimal FoldTo3BetInEPvs3BetMP => GetPercentage(foldTo3BetInEPvs3BetMP, couldFoldTo3BetInEPvs3BetMP);

        [ProtoMember(82)]
        protected int foldTo3BetInEPvs3BetCO;

        [ProtoMember(83)]
        protected int couldFoldTo3BetInEPvs3BetCO;

        public override decimal FoldTo3BetInEPvs3BetCO => GetPercentage(foldTo3BetInEPvs3BetCO, couldFoldTo3BetInEPvs3BetCO);

        [ProtoMember(84)]
        protected int foldTo3BetInEPvs3BetBTN;

        [ProtoMember(85)]
        protected int couldFoldTo3BetInEPvs3BetBTN;

        public override decimal FoldTo3BetInEPvs3BetBTN => GetPercentage(foldTo3BetInEPvs3BetBTN, couldFoldTo3BetInEPvs3BetBTN);

        [ProtoMember(86)]
        protected int foldTo3BetInEPvs3BetSB;

        [ProtoMember(87)]
        protected int couldFoldTo3BetInEPvs3BetSB;

        public override decimal FoldTo3BetInEPvs3BetSB => GetPercentage(foldTo3BetInEPvs3BetSB, couldFoldTo3BetInEPvs3BetSB);

        [ProtoMember(88)]
        protected int foldTo3BetInEPvs3BetBB;

        [ProtoMember(89)]
        protected int couldFoldTo3BetInEPvs3BetBB;

        public override decimal FoldTo3BetInEPvs3BetBB => GetPercentage(foldTo3BetInEPvs3BetBB, couldFoldTo3BetInEPvs3BetBB);

        [ProtoMember(90)]
        protected int foldTo3BetInMPvs3BetCO;

        [ProtoMember(91)]
        protected int couldFoldTo3BetInMPvs3BetCO;

        public override decimal FoldTo3BetInMPvs3BetCO => GetPercentage(foldTo3BetInMPvs3BetCO, couldFoldTo3BetInMPvs3BetCO);

        [ProtoMember(92)]
        protected int foldTo3BetInMPvs3BetBTN;

        [ProtoMember(93)]
        protected int couldFoldTo3BetInMPvs3BetBTN;

        public override decimal FoldTo3BetInMPvs3BetBTN => GetPercentage(foldTo3BetInMPvs3BetBTN, couldFoldTo3BetInMPvs3BetBTN);

        [ProtoMember(94)]
        protected int foldTo3BetInMPvs3BetSB;

        [ProtoMember(95)]
        protected int couldFoldTo3BetInMPvs3BetSB;

        public override decimal FoldTo3BetInMPvs3BetSB => GetPercentage(foldTo3BetInMPvs3BetSB, couldFoldTo3BetInMPvs3BetSB);

        [ProtoMember(96)]
        protected int foldTo3BetInMPvs3BetBB;

        [ProtoMember(97)]
        protected int couldFoldTo3BetInMPvs3BetBB;

        public override decimal FoldTo3BetInMPvs3BetBB => GetPercentage(foldTo3BetInMPvs3BetBB, couldFoldTo3BetInMPvs3BetBB);

        [ProtoMember(98)]
        protected int foldTo3BetInCOvs3BetBTN;

        [ProtoMember(99)]
        protected int couldFoldTo3BetInCOvs3BetBTN;

        public override decimal FoldTo3BetInCOvs3BetBTN => GetPercentage(foldTo3BetInCOvs3BetBTN, couldFoldTo3BetInCOvs3BetBTN);

        [ProtoMember(100)]
        protected int foldTo3BetInCOvs3BetSB;

        [ProtoMember(101)]
        protected int couldFoldTo3BetInCOvs3BetSB;

        public override decimal FoldTo3BetInCOvs3BetSB => GetPercentage(foldTo3BetInCOvs3BetSB, couldFoldTo3BetInCOvs3BetSB);

        [ProtoMember(102)]
        protected int foldTo3BetInCOvs3BetBB;

        [ProtoMember(103)]
        protected int couldFoldTo3BetInCOvs3BetBB;

        public override decimal FoldTo3BetInCOvs3BetBB => GetPercentage(foldTo3BetInCOvs3BetBB, couldFoldTo3BetInCOvs3BetBB);

        [ProtoMember(104)]
        protected int foldTo3BetInBTNvs3BetSB;

        [ProtoMember(105)]
        protected int couldFoldTo3BetInBTNvs3BetSB;

        public override decimal FoldTo3BetInBTNvs3BetSB => GetPercentage(foldTo3BetInBTNvs3BetSB, couldFoldTo3BetInBTNvs3BetSB);

        [ProtoMember(106)]
        protected int foldTo3BetInBTNvs3BetBB;

        [ProtoMember(107)]
        protected int couldFoldTo3BetInBTNvs3BetBB;

        public override decimal FoldTo3BetInBTNvs3BetBB => GetPercentage(foldTo3BetInBTNvs3BetBB, couldFoldTo3BetInBTNvs3BetBB);

        private void AddFoldTo3BetInPosVs3BetPosStatistic(Playerstatistic statistic)
        {
            foldTo3BetInEPvs3BetMP += statistic.FoldTo3BetInEPvs3BetMP;
            couldFoldTo3BetInEPvs3BetMP += statistic.CouldFoldTo3BetInEPvs3BetMP;

            foldTo3BetInEPvs3BetCO += statistic.FoldTo3BetInEPvs3BetCO;
            couldFoldTo3BetInEPvs3BetCO += statistic.CouldFoldTo3BetInEPvs3BetCO;

            foldTo3BetInEPvs3BetBTN += statistic.FoldTo3BetInEPvs3BetBTN;
            couldFoldTo3BetInEPvs3BetBTN += statistic.CouldFoldTo3BetInEPvs3BetBTN;

            foldTo3BetInEPvs3BetSB += statistic.FoldTo3BetInEPvs3BetSB;
            couldFoldTo3BetInEPvs3BetSB += statistic.CouldFoldTo3BetInEPvs3BetSB;

            foldTo3BetInEPvs3BetBB += statistic.FoldTo3BetInEPvs3BetBB;
            couldFoldTo3BetInEPvs3BetBB += statistic.CouldFoldTo3BetInEPvs3BetBB;

            foldTo3BetInMPvs3BetCO += statistic.FoldTo3BetInMPvs3BetCO;
            couldFoldTo3BetInMPvs3BetCO += statistic.CouldFoldTo3BetInMPvs3BetCO;

            foldTo3BetInMPvs3BetBTN += statistic.FoldTo3BetInMPvs3BetBTN;
            couldFoldTo3BetInMPvs3BetBTN += statistic.CouldFoldTo3BetInMPvs3BetBTN;

            foldTo3BetInMPvs3BetSB += statistic.FoldTo3BetInMPvs3BetSB;
            couldFoldTo3BetInMPvs3BetSB += statistic.CouldFoldTo3BetInMPvs3BetSB;

            foldTo3BetInMPvs3BetBB += statistic.FoldTo3BetInMPvs3BetBB;
            couldFoldTo3BetInMPvs3BetBB += statistic.CouldFoldTo3BetInMPvs3BetBB;

            foldTo3BetInCOvs3BetBTN += statistic.FoldTo3BetInCOvs3BetBTN;
            couldFoldTo3BetInCOvs3BetBTN += statistic.CouldFoldTo3BetInCOvs3BetBTN;

            foldTo3BetInCOvs3BetSB += statistic.FoldTo3BetInCOvs3BetSB;
            couldFoldTo3BetInCOvs3BetSB += statistic.CouldFoldTo3BetInCOvs3BetSB;

            foldTo3BetInCOvs3BetBB += statistic.FoldTo3BetInCOvs3BetBB;
            couldFoldTo3BetInCOvs3BetBB += statistic.CouldFoldTo3BetInCOvs3BetBB;

            foldTo3BetInBTNvs3BetSB += statistic.FoldTo3BetInBTNvs3BetSB;
            couldFoldTo3BetInBTNvs3BetSB += statistic.CouldFoldTo3BetInBTNvs3BetSB;

            foldTo3BetInBTNvs3BetBB += statistic.FoldTo3BetInBTNvs3BetBB;
            couldFoldTo3BetInBTNvs3BetBB += statistic.CouldFoldTo3BetInBTNvs3BetBB;
        }

        private void CleanFoldTo3BetInPosVs3BetPos()
        {
            foldTo3BetInEPvs3BetMP = 0;
            couldFoldTo3BetInEPvs3BetMP = 0;

            foldTo3BetInEPvs3BetCO = 0;
            couldFoldTo3BetInEPvs3BetCO = 0;

            foldTo3BetInEPvs3BetBTN = 0;
            couldFoldTo3BetInEPvs3BetBTN = 0;

            foldTo3BetInEPvs3BetSB = 0;
            couldFoldTo3BetInEPvs3BetSB = 0;

            foldTo3BetInEPvs3BetBB = 0;
            couldFoldTo3BetInEPvs3BetBB = 0;

            foldTo3BetInMPvs3BetCO = 0;
            couldFoldTo3BetInMPvs3BetCO = 0;

            foldTo3BetInMPvs3BetBTN = 0;
            couldFoldTo3BetInMPvs3BetBTN = 0;

            foldTo3BetInMPvs3BetSB = 0;
            couldFoldTo3BetInMPvs3BetSB = 0;

            foldTo3BetInMPvs3BetBB = 0;
            couldFoldTo3BetInMPvs3BetBB = 0;

            foldTo3BetInCOvs3BetBTN = 0;
            couldFoldTo3BetInCOvs3BetBTN = 0;

            foldTo3BetInCOvs3BetSB = 0;
            couldFoldTo3BetInCOvs3BetSB = 0;

            foldTo3BetInCOvs3BetBB = 0;
            couldFoldTo3BetInCOvs3BetBB = 0;

            foldTo3BetInBTNvs3BetSB = 0;
            couldFoldTo3BetInBTNvs3BetSB = 0;

            foldTo3BetInBTNvs3BetBB = 0;
            couldFoldTo3BetInBTNvs3BetBB = 0;
        }

        private void AddFoldTo3BetInPosVs3BetPosIndicator(LightIndicators indicator)
        {
            foldTo3BetInEPvs3BetMP += indicator.foldTo3BetInEPvs3BetMP;
            couldFoldTo3BetInEPvs3BetMP += indicator.couldFoldTo3BetInEPvs3BetMP;

            foldTo3BetInEPvs3BetCO += indicator.foldTo3BetInEPvs3BetCO;
            couldFoldTo3BetInEPvs3BetCO += indicator.couldFoldTo3BetInEPvs3BetCO;

            foldTo3BetInEPvs3BetBTN += indicator.foldTo3BetInEPvs3BetBTN;
            couldFoldTo3BetInEPvs3BetBTN += indicator.couldFoldTo3BetInEPvs3BetBTN;

            foldTo3BetInEPvs3BetSB += indicator.foldTo3BetInEPvs3BetSB;
            couldFoldTo3BetInEPvs3BetSB += indicator.couldFoldTo3BetInEPvs3BetSB;

            foldTo3BetInEPvs3BetBB += indicator.foldTo3BetInEPvs3BetBB;
            couldFoldTo3BetInEPvs3BetBB += indicator.couldFoldTo3BetInEPvs3BetBB;

            foldTo3BetInMPvs3BetCO += indicator.foldTo3BetInMPvs3BetCO;
            couldFoldTo3BetInMPvs3BetCO += indicator.couldFoldTo3BetInMPvs3BetCO;

            foldTo3BetInMPvs3BetBTN += indicator.foldTo3BetInMPvs3BetBTN;
            couldFoldTo3BetInMPvs3BetBTN += indicator.couldFoldTo3BetInMPvs3BetBTN;

            foldTo3BetInMPvs3BetSB += indicator.foldTo3BetInMPvs3BetSB;
            couldFoldTo3BetInMPvs3BetSB += indicator.couldFoldTo3BetInMPvs3BetSB;

            foldTo3BetInMPvs3BetBB += indicator.foldTo3BetInMPvs3BetBB;
            couldFoldTo3BetInMPvs3BetBB += indicator.couldFoldTo3BetInMPvs3BetBB;

            foldTo3BetInCOvs3BetBTN += indicator.foldTo3BetInCOvs3BetBTN;
            couldFoldTo3BetInCOvs3BetBTN += indicator.couldFoldTo3BetInCOvs3BetBTN;

            foldTo3BetInCOvs3BetSB += indicator.foldTo3BetInCOvs3BetSB;
            couldFoldTo3BetInCOvs3BetSB += indicator.couldFoldTo3BetInCOvs3BetSB;

            foldTo3BetInCOvs3BetBB += indicator.foldTo3BetInCOvs3BetBB;
            couldFoldTo3BetInCOvs3BetBB += indicator.couldFoldTo3BetInCOvs3BetBB;

            foldTo3BetInBTNvs3BetSB += indicator.foldTo3BetInBTNvs3BetSB;
            couldFoldTo3BetInBTNvs3BetSB += indicator.couldFoldTo3BetInBTNvs3BetSB;

            foldTo3BetInBTNvs3BetBB += indicator.foldTo3BetInBTNvs3BetBB;
            couldFoldTo3BetInBTNvs3BetBB += indicator.couldFoldTo3BetInBTNvs3BetBB;
        }

        #endregion

        [ProtoMember(108)]
        protected int checkRaiseFlopAsPFR;

        [ProtoMember(109)]
        protected int couldCheckRaiseFlopAsPFR;

        public override decimal CheckRaiseFlopAsPFR => GetPercentage(checkRaiseFlopAsPFR, couldCheckRaiseFlopAsPFR);

        #endregion

        #endregion

        #region overridden methods

        public override void AddStatistic(Playerstatistic statistic)
        {
            Source += statistic;
            UpdatePositionalStats(statistic);

            if (gameNumberMax < statistic.GameNumber)
            {
                gameNumberMax = statistic.GameNumber;
            }

            statisticCount++;
            netWon += statistic.NetWon;
            bigBlind += statistic.BigBlind;
            netWonByBigBlind += GetDivisionResult(statistic.NetWon, statistic.BigBlind);
            evInBB += GetDivisionResult(statistic.NetWon + statistic.EVDiff, statistic.BigBlind);

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

            did4Bet += statistic.DidfourbetpreflopVirtual;
            could4Bet += statistic.CouldfourbetpreflopVirtual;

            faced3Bet += statistic.FacedthreebetpreflopVirtual;
            foldedTo3Bet += statistic.FoldedtothreebetpreflopVirtual;

            raisedLimpers += statistic.IsRaisedLimpers;
            couldRaiseLimpers += statistic.CouldRaiseLimpers;

            checkRaiseFlopAsPFR += statistic.CheckRaiseFlopAsPFR;
            couldCheckRaiseFlopAsPFR += statistic.CouldCheckRaiseFlopAsPFR;
        }

        public override void Clean()
        {
            base.Clean();

            gameNumberMax = 0;
            statisticCount = 0;
            netWon = 0;
            bigBlind = 0;
            netWonByBigBlind = 0;
            evInBB = 0;
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

            did4Bet = 0;
            could4Bet = 0;
            faced3Bet = 0;
            foldedTo3Bet = 0;

            raisedLimpers = 0;
            couldRaiseLimpers = 0;

            checkRaiseFlopAsPFR = 0;
            couldCheckRaiseFlopAsPFR = 0;

            Clean3BetVsRaiserInPos();
        }

        public virtual void AddIndicator(LightIndicators indicator)
        {
            Source += indicator.Source;

            positionTotal?.Add(indicator.positionTotal);
            positionUnoppened?.Add(indicator.positionUnoppened);
            positionVPIP?.Add(indicator.positionVPIP);
            positionDidColdCall?.Add(indicator.positionDidColdCall);
            positionCouldColdCall?.Add(indicator.positionCouldColdCall);
            positionDidThreeBet?.Add(indicator.positionDidThreeBet);
            positionCouldThreeBet?.Add(indicator.positionCouldThreeBet);
            positionDidFourBet?.Add(indicator.positionDidFourBet);
            positionCouldFourBet?.Add(indicator.positionCouldFourBet);
            positionLimpMade?.Add(indicator.positionLimpMade);
            positionLimpPossible?.Add(indicator.positionLimpPossible);
            positionRaiseLimpers?.Add(indicator.positionRaiseLimpers);
            positionCouldRaiseLimpers?.Add(indicator.positionCouldRaiseLimpers);

            if (gameNumberMax < indicator.gameNumberMax)
            {
                gameNumberMax = indicator.gameNumberMax;
            }

            statisticCount += indicator.statisticCount;
            netWon += indicator.netWon;
            bigBlind += indicator.bigBlind;
            netWonByBigBlind += indicator.netWonByBigBlind;

            evInBB += indicator.evInBB;

            if (sessionStartTime > indicator.sessionStartTime)
            {
                sessionStartTime = indicator.sessionStartTime;
            }

            if (sessionEndTime < indicator.sessionEndTime)
            {
                sessionEndTime = indicator.sessionEndTime;
            }

            didDelayedTurnCBetIP += indicator.didDelayedTurnCBetIP;
            couldDelayedTurnCBetIP += indicator.couldDelayedTurnCBetIP;
            didDelayedTurnCBetOOP += indicator.didDelayedTurnCBetOOP;
            couldDelayedTurnCBetOOP += indicator.couldDelayedTurnCBetOOP;
            checkRaisedFlopCBet += indicator.checkRaisedFlopCBet;
            couldCheckRaiseFlopCBet += indicator.couldCheckRaiseFlopCBet;

            didFlopBet += indicator.didFlopBet;
            flopBetSizeMoreThanOne += indicator.flopBetSizeMoreThanOne;
            flopBetSizeOneHalfOrLess += indicator.flopBetSizeOneHalfOrLess;
            flopBetSizeOneQuarterOrLess += indicator.flopBetSizeOneQuarterOrLess;
            flopBetSizeTwoThirdsOrLess += indicator.flopBetSizeTwoThirdsOrLess;
            flopBetSizeThreeQuartersOrLess += indicator.flopBetSizeThreeQuartersOrLess;
            flopBetSizeOneOrLess += indicator.flopBetSizeOneOrLess;

            didTurnBet += indicator.didTurnBet;
            turnBetSizeMoreThanOne += indicator.turnBetSizeMoreThanOne;
            turnBetSizeOneHalfOrLess += indicator.turnBetSizeOneHalfOrLess;
            turnBetSizeOneQuarterOrLess += indicator.turnBetSizeOneQuarterOrLess;
            turnBetSizeOneThirdOrLess += indicator.turnBetSizeOneThirdOrLess;
            turnBetSizeTwoThirdsOrLess += indicator.turnBetSizeTwoThirdsOrLess;
            turnBetSizeThreeQuartersOrLess += indicator.turnBetSizeThreeQuartersOrLess;
            turnBetSizeOneOrLess += indicator.turnBetSizeOneOrLess;

            riverBetSizeMoreThanOne += indicator.riverBetSizeMoreThanOne;

            wtsdAfterCalling3Bet += indicator.wtsdAfterCalling3Bet;
            wtsdAfterCalling3BetOpportunity += indicator.wtsdAfterCalling3BetOpportunity;
            wtsdAfterCallingPfr += indicator.wtsdAfterCallingPfr;
            wtsdAfterCallingPfrOpportunity += indicator.wtsdAfterCallingPfrOpportunity;
            wtsdAfterNotCBettingFlopAsPfr += indicator.wtsdAfterNotCBettingFlopAsPfr;
            wtsdAfterNotCBettingFlopAsPfrOpportunity += indicator.wtsdAfterNotCBettingFlopAsPfrOpportunity;
            wtsdAfterSeeingTurn += indicator.wtsdAfterSeeingTurn;
            wtsdAsPF3Bettor += indicator.wtsdAsPF3Bettor;
            wtsdAsPF3BettorOpportunity += indicator.wtsdAsPF3BettorOpportunity;

            foldToTurnCBetIn3BetPot += indicator.foldToTurnCBetIn3BetPot;
            facedToTurnCBetIn3BetPot += indicator.facedToTurnCBetIn3BetPot;

            raisedFlopCBetIn3BetPot += indicator.raisedFlopCBetIn3BetPot;
            couldRaiseFlopCBetIn3BetPot += indicator.couldRaiseFlopCBetIn3BetPot;

            did4Bet += indicator.did4Bet;
            could4Bet += indicator.could4Bet;

            faced3Bet += indicator.faced3Bet;
            foldedTo3Bet += indicator.foldedTo3Bet;

            raisedLimpers += indicator.raisedLimpers;
            couldRaiseLimpers += indicator.couldRaiseLimpers;

            checkRaiseFlopAsPFR += indicator.checkRaiseFlopAsPFR;
            couldCheckRaiseFlopAsPFR += indicator.couldCheckRaiseFlopAsPFR;

            Add3BetVsRaiserInPosIndicator(indicator);
        }

        public override int CompareTo(object obj)
        {
            var objIndicator = obj as LightIndicators;

            if (objIndicator == null)
            {
                return 1;
            }

            return objIndicator.gameNumberMax.CompareTo(gameNumberMax);
        }

        #endregion       
    }
}