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

        [ProtoMember(110)]
        protected int probeBetTurn;

        public override decimal ProbeBetTurn => GetPercentage(probeBetTurn, Source.CouldProbeBetTurn);

        [ProtoMember(111)]
        protected int probeBetRiver;

        public override decimal ProbeBetRiver => GetPercentage(probeBetRiver, Source.CouldProbeBetRiver);

        [ProtoMember(112)]
        protected int floatFlopThenBetTurn;

        [ProtoMember(113)]
        protected int couldFloatFlopThenBetTurn;

        public override decimal FloatFlopThenBetTurn => GetPercentage(floatFlopThenBetTurn, couldFloatFlopThenBetTurn);

        [ProtoMember(114)]
        protected int foldBBvsSBSteal;

        [ProtoMember(115)]
        protected int couldFoldBBvsSBSteal;

        public override decimal FoldBBvsSBSteal => GetPercentage(foldBBvsSBSteal, couldFoldBBvsSBSteal);

        [ProtoMember(116)]
        protected int betTurnWhenCheckedToSRP;

        [ProtoMember(117)]
        protected int couldBetTurnWhenCheckedToSRP;

        public override decimal BetTurnWhenCheckedToSRP => GetPercentage(betTurnWhenCheckedToSRP, couldBetTurnWhenCheckedToSRP);

        [ProtoMember(118)]
        protected int betRiverWhenCheckedToSRP;

        [ProtoMember(119)]
        protected int couldBetRiverWhenCheckedToSRP;

        public override decimal BetRiverWhenCheckedToSRP => GetPercentage(betRiverWhenCheckedToSRP, couldBetRiverWhenCheckedToSRP);

        [ProtoMember(126)]
        protected int doubleBarrelSRP;

        [ProtoMember(127)]
        protected int couldDoubleBarrelSRP;

        public override decimal DoubleBarrelSRP => GetPercentage(doubleBarrelSRP, couldDoubleBarrelSRP);

        [ProtoMember(128)]
        protected int doubleBarrel3BetPot;

        [ProtoMember(129)]
        protected int couldDoubleBarrel3BetPot;

        public override decimal DoubleBarrel3BetPot => GetPercentage(doubleBarrel3BetPot, couldDoubleBarrel3BetPot);

        [ProtoMember(130)]
        protected int tripleBarrelSRP;

        [ProtoMember(131)]
        protected int couldTripleBarrelSRP;

        public override decimal TripleBarrelSRP => GetPercentage(tripleBarrelSRP, couldTripleBarrelSRP);

        [ProtoMember(132)]
        protected int tripleBarrel3BetPot;

        [ProtoMember(133)]
        protected int couldTripleBarrel3BetPot;

        public override decimal TripleBarrel3BetPot => GetPercentage(tripleBarrel3BetPot, couldTripleBarrel3BetPot);

        [ProtoMember(134)]
        protected int cBetThenFoldFlopSRP;

        [ProtoMember(135)]
        protected int couldCBetThenFoldFlopSRP;

        public override decimal CBetThenFoldFlopSRP => GetPercentage(cBetThenFoldFlopSRP, couldCBetThenFoldFlopSRP);

        [ProtoMember(136)]
        protected int foldedToProbeBetTurn;

        [ProtoMember(137)]
        protected int facedProbeBetTurn;

        public override decimal FoldToProbeBetTurn => GetPercentage(foldedToProbeBetTurn, facedProbeBetTurn);

        [ProtoMember(138)]
        protected int foldedToProbeBetRiver;

        [ProtoMember(139)]
        protected int facedProbeBetRiver;

        public override decimal FoldToProbeBetRiver => GetPercentage(foldedToProbeBetRiver, facedProbeBetRiver);

        #endregion

        #region Bet When Checked to in 3Bet Pot

        [ProtoMember(120)]
        protected int betFlopWhenCheckedToIn3BetPot;

        [ProtoMember(121)]
        protected int couldBetFlopWhenCheckedToIn3BetPot;

        public override decimal BetFlopWhenCheckedToIn3BetPot => GetPercentage(betFlopWhenCheckedToIn3BetPot, couldBetFlopWhenCheckedToIn3BetPot);

        [ProtoMember(122)]
        protected int betTurnWhenCheckedToIn3BetPot;

        [ProtoMember(123)]
        protected int couldBetTurnWhenCheckedToIn3BetPot;

        public override decimal BetTurnWhenCheckedToIn3BetPot => GetPercentage(betTurnWhenCheckedToIn3BetPot, couldBetTurnWhenCheckedToIn3BetPot);

        [ProtoMember(124)]
        protected int betRiverWhenCheckedToIn3BetPot;

        [ProtoMember(125)]
        protected int couldBetRiverWhenCheckedToIn3BetPot;

        public override decimal BetRiverWhenCheckedToIn3BetPot => GetPercentage(betRiverWhenCheckedToIn3BetPot, couldBetRiverWhenCheckedToIn3BetPot);

        private void AddBetWhenCheckedToIn3BetPotStatistic(Playerstatistic statistic)
        {
            betFlopWhenCheckedToIn3BetPot += statistic.BetFlopWhenCheckedToIn3BetPot;
            couldBetFlopWhenCheckedToIn3BetPot += statistic.CouldBetFlopWhenCheckedToIn3BetPot;
            betTurnWhenCheckedToIn3BetPot += statistic.BetTurnWhenCheckedToIn3BetPot;
            couldBetTurnWhenCheckedToIn3BetPot += statistic.CouldBetTurnWhenCheckedToIn3BetPot;
            betRiverWhenCheckedToIn3BetPot += statistic.BetRiverWhenCheckedToIn3BetPot;
            couldBetRiverWhenCheckedToIn3BetPot += statistic.CouldBetRiverWhenCheckedToIn3BetPot;
        }

        private void CleanBetWhenCheckedToIn3BetPot()
        {
            betFlopWhenCheckedToIn3BetPot = 0;
            couldBetFlopWhenCheckedToIn3BetPot = 0;
            betTurnWhenCheckedToIn3BetPot = 0;
            couldBetTurnWhenCheckedToIn3BetPot = 0;
            betRiverWhenCheckedToIn3BetPot = 0;
            couldBetRiverWhenCheckedToIn3BetPot = 0;
        }

        private void AddBetWhenCheckedToIn3BetPotIndicator(LightIndicators indicator)
        {
            betFlopWhenCheckedToIn3BetPot += indicator.betFlopWhenCheckedToIn3BetPot;
            couldBetFlopWhenCheckedToIn3BetPot += indicator.couldBetFlopWhenCheckedToIn3BetPot;
            betTurnWhenCheckedToIn3BetPot += indicator.betTurnWhenCheckedToIn3BetPot;
            couldBetTurnWhenCheckedToIn3BetPot += indicator.couldBetTurnWhenCheckedToIn3BetPot;
            betRiverWhenCheckedToIn3BetPot += indicator.betRiverWhenCheckedToIn3BetPot;
            couldBetRiverWhenCheckedToIn3BetPot += indicator.couldBetRiverWhenCheckedToIn3BetPot;
        }

        #endregion

        #region Check Flop as PFR and Fold to Turn Bet SRP

        [ProtoMember(140)]
        protected int checkFlopAsPFRAndFoldToTurnBetIPSRP;

        [ProtoMember(141)]
        protected int couldCheckFlopAsPFRAndFoldToTurnBetIPSRP;

        public override decimal CheckFlopAsPFRAndFoldToTurnBetIPSRP => GetPercentage(checkFlopAsPFRAndFoldToTurnBetIPSRP, couldCheckFlopAsPFRAndFoldToTurnBetIPSRP);

        [ProtoMember(142)]
        protected int checkFlopAsPFRAndFoldToTurnBetOOPSRP;

        [ProtoMember(143)]
        protected int couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP;

        public override decimal CheckFlopAsPFRAndFoldToTurnBetOOPSRP => GetPercentage(checkFlopAsPFRAndFoldToTurnBetOOPSRP, couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP);

        [ProtoMember(144)]
        protected int checkFlopAsPFRAndFoldToRiverBetIPSRP;

        [ProtoMember(145)]
        protected int couldCheckFlopAsPFRAndFoldToRiverBetIPSRP;

        public override decimal CheckFlopAsPFRAndFoldToRiverBetIPSRP => GetPercentage(checkFlopAsPFRAndFoldToRiverBetIPSRP, couldCheckFlopAsPFRAndFoldToRiverBetIPSRP);

        [ProtoMember(146)]
        protected int checkFlopAsPFRAndFoldToRiverBetOOPSRP;

        [ProtoMember(147)]
        protected int couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP;

        public override decimal CheckFlopAsPFRAndFoldToRiverBetOOPSRP => GetPercentage(checkFlopAsPFRAndFoldToRiverBetOOPSRP, couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP);

        private void AddCheckFlopAsPFRAndFoldToTurnBetSRPStatistic(Playerstatistic statistic)
        {
            checkFlopAsPFRAndFoldToTurnBetIPSRP += statistic.CheckFlopAsPFRAndFoldToTurnBetIPSRP;
            couldCheckFlopAsPFRAndFoldToTurnBetIPSRP += statistic.CouldCheckFlopAsPFRAndFoldToTurnBetIPSRP;
            checkFlopAsPFRAndFoldToTurnBetOOPSRP += statistic.CheckFlopAsPFRAndFoldToTurnBetOOPSRP;
            couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP += statistic.CouldCheckFlopAsPFRAndFoldToTurnBetOOPSRP;
            checkFlopAsPFRAndFoldToRiverBetIPSRP += statistic.CheckFlopAsPFRAndFoldToRiverBetIPSRP;
            couldCheckFlopAsPFRAndFoldToRiverBetIPSRP += statistic.CouldCheckFlopAsPFRAndFoldToRiverBetIPSRP;
            checkFlopAsPFRAndFoldToRiverBetOOPSRP += statistic.CheckFlopAsPFRAndFoldToRiverBetOOPSRP;
            couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP += statistic.CouldCheckFlopAsPFRAndFoldToRiverBetOOPSRP;
        }

        private void CleanCheckFlopAsPFRAndFoldToTurnBetSRP()
        {
            checkFlopAsPFRAndFoldToTurnBetIPSRP = 0;
            couldCheckFlopAsPFRAndFoldToTurnBetIPSRP = 0;
            checkFlopAsPFRAndFoldToTurnBetOOPSRP = 0;
            couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP = 0;
            checkFlopAsPFRAndFoldToRiverBetIPSRP = 0;
            couldCheckFlopAsPFRAndFoldToRiverBetIPSRP = 0;
            checkFlopAsPFRAndFoldToRiverBetOOPSRP = 0;
            couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP = 0;
        }

        private void AddCheckFlopAsPFRAndFoldToTurnBetSRPIndicator(LightIndicators indicator)
        {
            checkFlopAsPFRAndFoldToTurnBetIPSRP += indicator.checkFlopAsPFRAndFoldToTurnBetIPSRP;
            couldCheckFlopAsPFRAndFoldToTurnBetIPSRP += indicator.couldCheckFlopAsPFRAndFoldToTurnBetIPSRP;
            checkFlopAsPFRAndFoldToTurnBetOOPSRP += indicator.checkFlopAsPFRAndFoldToTurnBetOOPSRP;
            couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP += indicator.couldCheckFlopAsPFRAndFoldToTurnBetOOPSRP;
            checkFlopAsPFRAndFoldToRiverBetIPSRP += indicator.checkFlopAsPFRAndFoldToRiverBetIPSRP;
            couldCheckFlopAsPFRAndFoldToRiverBetIPSRP += indicator.couldCheckFlopAsPFRAndFoldToRiverBetIPSRP;
            checkFlopAsPFRAndFoldToRiverBetOOPSRP += indicator.checkFlopAsPFRAndFoldToRiverBetOOPSRP;
            couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP += indicator.couldCheckFlopAsPFRAndFoldToRiverBetOOPSRP;
        }

        #endregion

        #region Check Flop as PFR and Fold to Turn/River Bet in 3-Bet Pot

        [ProtoMember(148)]
        protected int checkFlopAsPFRAndFoldToTurnBetIP3BetPot;

        [ProtoMember(149)]
        protected int couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot;

        public override decimal CheckFlopAsPFRAndFoldToTurnBetIP3BetPot => GetPercentage(checkFlopAsPFRAndFoldToTurnBetIP3BetPot, couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot);

        [ProtoMember(150)]
        protected int checkFlopAsPFRAndFoldToTurnBetOOP3BetPot;

        [ProtoMember(151)]
        protected int couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot;

        public override decimal CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot => GetPercentage(checkFlopAsPFRAndFoldToTurnBetOOP3BetPot, couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot);

        [ProtoMember(152)]
        protected int checkFlopAsPFRAndFoldToRiverBetIP3BetPot;

        [ProtoMember(153)]
        protected int couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot;

        public override decimal CheckFlopAsPFRAndFoldToRiverBetIP3BetPot => GetPercentage(checkFlopAsPFRAndFoldToRiverBetIP3BetPot, couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot);

        [ProtoMember(154)]
        protected int checkFlopAsPFRAndFoldToRiverBetOOP3BetPot;

        [ProtoMember(155)]
        protected int couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot;

        public override decimal CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot => GetPercentage(checkFlopAsPFRAndFoldToRiverBetOOP3BetPot, couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot);

        private void AddCheckFlopAsPFRAndFoldToTurnBet3BetPotStatistic(Playerstatistic statistic)
        {
            checkFlopAsPFRAndFoldToTurnBetIP3BetPot += statistic.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot;
            couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot += statistic.CouldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot;
            checkFlopAsPFRAndFoldToTurnBetOOP3BetPot += statistic.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot;
            couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot += statistic.CouldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot;
            checkFlopAsPFRAndFoldToRiverBetIP3BetPot += statistic.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot;
            couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot += statistic.CouldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot;
            checkFlopAsPFRAndFoldToRiverBetOOP3BetPot += statistic.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot;
            couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot += statistic.CouldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot;
        }

        private void CleanCheckFlopAsPFRAndFoldToTurnBet3BetPot()
        {
            checkFlopAsPFRAndFoldToTurnBetIP3BetPot = 0;
            couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot = 0;
            checkFlopAsPFRAndFoldToTurnBetOOP3BetPot = 0;
            couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot = 0;
            checkFlopAsPFRAndFoldToRiverBetIP3BetPot = 0;
            couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot = 0;
            checkFlopAsPFRAndFoldToRiverBetOOP3BetPot = 0;
            couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot = 0;
        }

        private void AddCheckFlopAsPFRAndFoldToTurnBet3BetPotIndicator(LightIndicators indicator)
        {
            checkFlopAsPFRAndFoldToTurnBetIP3BetPot += indicator.checkFlopAsPFRAndFoldToTurnBetIP3BetPot;
            couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot += indicator.couldCheckFlopAsPFRAndFoldToTurnBetIP3BetPot;
            checkFlopAsPFRAndFoldToTurnBetOOP3BetPot += indicator.checkFlopAsPFRAndFoldToTurnBetOOP3BetPot;
            couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot += indicator.couldCheckFlopAsPFRAndFoldToTurnBetOOP3BetPot;
            checkFlopAsPFRAndFoldToRiverBetIP3BetPot += indicator.checkFlopAsPFRAndFoldToRiverBetIP3BetPot;
            couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot += indicator.couldCheckFlopAsPFRAndFoldToRiverBetIP3BetPot;
            checkFlopAsPFRAndFoldToRiverBetOOP3BetPot += indicator.checkFlopAsPFRAndFoldToRiverBetOOP3BetPot;
            couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot += indicator.couldCheckFlopAsPFRAndFoldToRiverBetOOP3BetPot;
        }

        #endregion

        #region Fold to continuation bets in SRP/3Bet/4Bet

        [ProtoMember(156)]
        protected int foldToTripleBarrelSRP;

        [ProtoMember(157)]
        protected int facingTripleBarrelSRP;

        public override decimal FoldToTripleBarrelSRP => GetPercentage(foldToTripleBarrelSRP, facingTripleBarrelSRP);

        [ProtoMember(158)]
        protected int foldToTripleBarrel3BetPot;

        [ProtoMember(159)]
        protected int facingTripleBarrel3BetPot;

        public override decimal FoldToTripleBarrel3BetPot => GetPercentage(foldToTripleBarrel3BetPot, facingTripleBarrel3BetPot);

        [ProtoMember(160)]
        protected int foldToTripleBarrel4BetPot;

        [ProtoMember(161)]
        protected int facingTripleBarrel4BetPot;

        public override decimal FoldToTripleBarrel4BetPot => GetPercentage(foldToTripleBarrel4BetPot, facingTripleBarrel4BetPot);

        [ProtoMember(162)]
        protected int foldToDoubleBarrelSRP;

        [ProtoMember(163)]
        protected int facingDoubleBarrelSRP;

        public override decimal FoldToDoubleBarrelSRP => GetPercentage(foldToDoubleBarrelSRP, facingDoubleBarrelSRP);

        [ProtoMember(164)]
        protected int foldToDoubleBarrel4BetPot;

        [ProtoMember(165)]
        protected int facingDoubleBarrel4BetPot;

        public override decimal FoldToDoubleBarrel4BetPot => GetPercentage(foldToDoubleBarrel4BetPot, facingDoubleBarrel4BetPot);

        [ProtoMember(166)]
        protected int foldToCBetSRP;

        [ProtoMember(167)]
        protected int facingCBetSRP;

        public override decimal FoldToCBetSRP => GetPercentage(foldToCBetSRP, facingCBetSRP);

        private void AddFoldToContinuationBetsInPotsStatistic(Playerstatistic statistic)
        {
            foldToTripleBarrelSRP += statistic.FoldToTripleBarrelSRP;
            facingTripleBarrelSRP += statistic.FacingTripleBarrelSRP;

            foldToTripleBarrel3BetPot += statistic.FoldToTripleBarrel3BetPot;
            facingTripleBarrel3BetPot += statistic.FacingTripleBarrel3BetPot;

            foldToTripleBarrel4BetPot += statistic.FoldToTripleBarrel4BetPot;
            facingTripleBarrel4BetPot += statistic.FacingTripleBarrel4BetPot;

            foldToDoubleBarrelSRP += statistic.FoldToDoubleBarrelSRP;
            facingDoubleBarrelSRP += statistic.FacingDoubleBarrelSRP;

            foldToDoubleBarrel4BetPot += statistic.FoldToDoubleBarrel4BetPot;
            facingDoubleBarrel4BetPot += statistic.FacingDoubleBarrel4BetPot;

            foldToCBetSRP += statistic.FoldToCBetSRP;
            facingCBetSRP += statistic.FacingCBetSRP;
        }

        private void CleanFoldToContinuationBetsInPots()
        {
            foldToTripleBarrelSRP = 0;
            facingTripleBarrelSRP = 0;

            foldToTripleBarrel3BetPot = 0;
            facingTripleBarrel3BetPot = 0;

            foldToTripleBarrel4BetPot = 0;
            facingTripleBarrel4BetPot = 0;

            foldToDoubleBarrelSRP = 0;
            facingDoubleBarrelSRP = 0;

            foldToDoubleBarrel4BetPot = 0;
            facingDoubleBarrel4BetPot = 0;

            foldToCBetSRP = 0;
            facingCBetSRP = 0;
        }

        private void AddFoldToContinuationBetsInPotsIndicator(LightIndicators indicator)
        {
            foldToTripleBarrelSRP += indicator.foldToTripleBarrelSRP;
            facingTripleBarrelSRP += indicator.facingTripleBarrelSRP;

            foldToTripleBarrel3BetPot += indicator.foldToTripleBarrel3BetPot;
            facingTripleBarrel3BetPot += indicator.facingTripleBarrel3BetPot;

            foldToTripleBarrel4BetPot += indicator.foldToTripleBarrel4BetPot;
            facingTripleBarrel4BetPot += indicator.facingTripleBarrel4BetPot;

            foldToDoubleBarrelSRP += indicator.foldToDoubleBarrelSRP;
            facingDoubleBarrelSRP += indicator.facingDoubleBarrelSRP;

            foldToDoubleBarrel4BetPot += indicator.foldToDoubleBarrel4BetPot;
            facingDoubleBarrel4BetPot += indicator.facingDoubleBarrel4BetPot;

            foldToCBetSRP += indicator.foldToCBetSRP;
            facingCBetSRP += indicator.facingCBetSRP;
        }

        #endregion

        #region Open Shove UO Pot positional

        [ProtoMember(168)]
        protected int sbOpenShoveUOPot;

        [ProtoMember(169)]
        protected int sbOpenShove1to8bbUOPot;

        public override decimal SBOpenShove1to8bbUOPot => GetPercentage(sbOpenShove1to8bbUOPot, sbOpenShoveUOPot);

        [ProtoMember(170)]
        protected int sbOpenShove9to14bbUOPot;

        public override decimal SBOpenShove9to14bbUOPot => GetPercentage(sbOpenShove9to14bbUOPot, sbOpenShoveUOPot);

        [ProtoMember(171)]
        protected int sbOpenShove15to25bbUOPot;

        public override decimal SBOpenShove15to25bbUOPot => GetPercentage(sbOpenShove15to25bbUOPot, sbOpenShoveUOPot);

        [ProtoMember(172)]
        protected int sbOpenShove26to50bbUOPot;

        public override decimal SBOpenShove26to50bbUOPot => GetPercentage(sbOpenShove26to50bbUOPot, sbOpenShoveUOPot);

        [ProtoMember(173)]
        protected int sbOpenShove51plusbbUOPot;

        public override decimal SBOpenShove51plusbbUOPot => GetPercentage(sbOpenShove51plusbbUOPot, sbOpenShoveUOPot);

        [ProtoMember(174)]
        protected int btnOpenShoveUOPot;

        [ProtoMember(175)]
        protected int btnOpenShove1to8bbUOPot;

        public override decimal BTNOpenShove1to8bbUOPot => GetPercentage(btnOpenShove1to8bbUOPot, btnOpenShoveUOPot);

        [ProtoMember(175)]
        protected int btnOpenShove9to14bbUOPot;

        public override decimal BTNOpenShove9to14bbUOPot => GetPercentage(btnOpenShove9to14bbUOPot, btnOpenShoveUOPot);

        [ProtoMember(176)]
        protected int btnOpenShove15to25bbUOPot;

        public override decimal BTNOpenShove15to25bbUOPot => GetPercentage(btnOpenShove15to25bbUOPot, btnOpenShoveUOPot);

        [ProtoMember(177)]
        protected int btnOpenShove26to50bbUOPot;

        public override decimal BTNOpenShove26to50bbUOPot => GetPercentage(btnOpenShove26to50bbUOPot, btnOpenShoveUOPot);

        [ProtoMember(178)]
        protected int btnOpenShove51plusbbUOPot;

        public override decimal BTNOpenShove51plusbbUOPot => GetPercentage(btnOpenShove51plusbbUOPot, btnOpenShoveUOPot);

        [ProtoMember(179)]
        protected int coOpenShoveUOPot;

        [ProtoMember(180)]
        protected int coOpenShove1to8bbUOPot;

        public override decimal COOpenShove1to8bbUOPot => GetPercentage(coOpenShove1to8bbUOPot, coOpenShoveUOPot);

        [ProtoMember(181)]
        protected int coOpenShove9to14bbUOPot;

        public override decimal COOpenShove9to14bbUOPot => GetPercentage(coOpenShove9to14bbUOPot, coOpenShoveUOPot);

        [ProtoMember(182)]
        protected int coOpenShove15to25bbUOPot;

        public override decimal COOpenShove15to25bbUOPot => GetPercentage(coOpenShove15to25bbUOPot, coOpenShoveUOPot);

        [ProtoMember(183)]
        protected int coOpenShove26to50bbUOPot;

        public override decimal COOpenShove26to50bbUOPot => GetPercentage(coOpenShove26to50bbUOPot, coOpenShoveUOPot);

        [ProtoMember(184)]
        protected int coOpenShove51plusbbUOPot;

        public override decimal COOpenShove51plusbbUOPot => GetPercentage(coOpenShove51plusbbUOPot, coOpenShoveUOPot);

        [ProtoMember(185)]
        protected int mpOpenShoveUOPot;

        [ProtoMember(186)]
        protected int mpOpenShove1to8bbUOPot;

        public override decimal MPOpenShove1to8bbUOPot => GetPercentage(mpOpenShove1to8bbUOPot, mpOpenShoveUOPot);

        [ProtoMember(187)]
        protected int mpOpenShove9to14bbUOPot;

        public override decimal MPOpenShove9to14bbUOPot => GetPercentage(mpOpenShove9to14bbUOPot, mpOpenShoveUOPot);

        [ProtoMember(188)]
        protected int mpOpenShove15to25bbUOPot;

        public override decimal MPOpenShove15to25bbUOPot => GetPercentage(mpOpenShove15to25bbUOPot, mpOpenShoveUOPot);

        [ProtoMember(189)]
        protected int mpOpenShove26to50bbUOPot;

        public override decimal MPOpenShove26to50bbUOPot => GetPercentage(mpOpenShove26to50bbUOPot, mpOpenShoveUOPot);

        [ProtoMember(190)]
        protected int mpOpenShove51plusbbUOPot;

        public override decimal MPOpenShove51plusbbUOPot => GetPercentage(mpOpenShove51plusbbUOPot, mpOpenShoveUOPot);

        [ProtoMember(191)]
        protected int epOpenShoveUOPot;

        [ProtoMember(192)]
        protected int epOpenShove1to8bbUOPot;

        public override decimal EPOpenShove1to8bbUOPot => GetPercentage(epOpenShove1to8bbUOPot, epOpenShoveUOPot);

        [ProtoMember(193)]
        protected int epOpenShove9to14bbUOPot;

        public override decimal EPOpenShove9to14bbUOPot => GetPercentage(epOpenShove9to14bbUOPot, epOpenShoveUOPot);

        [ProtoMember(194)]
        protected int epOpenShove15to25bbUOPot;

        public override decimal EPOpenShove15to25bbUOPot => GetPercentage(epOpenShove15to25bbUOPot, epOpenShoveUOPot);

        [ProtoMember(195)]
        protected int epOpenShove26to50bbUOPot;

        public override decimal EPOpenShove26to50bbUOPot => GetPercentage(epOpenShove26to50bbUOPot, epOpenShoveUOPot);

        [ProtoMember(196)]
        protected int epOpenShove51plusbbUOPot;

        public override decimal EPOpenShove51plusbbUOPot => GetPercentage(epOpenShove51plusbbUOPot, epOpenShoveUOPot);

        private void AddOpenShoveUOPotStatistic(Playerstatistic statistic)
        {
            sbOpenShoveUOPot += statistic.SBOpenShoveUOPot;
            sbOpenShove1to8bbUOPot += statistic.SBOpenShove1to8bbUOPot;
            sbOpenShove9to14bbUOPot += statistic.SBOpenShove9to14bbUOPot;
            sbOpenShove15to25bbUOPot += statistic.SBOpenShove15to25bbUOPot;
            sbOpenShove26to50bbUOPot += statistic.SBOpenShove26to50bbUOPot;
            sbOpenShove51plusbbUOPot += statistic.SBOpenShove51plusbbUOPot;

            btnOpenShoveUOPot += statistic.BTNOpenShoveUOPot;
            btnOpenShove1to8bbUOPot += statistic.BTNOpenShove1to8bbUOPot;
            btnOpenShove9to14bbUOPot += statistic.BTNOpenShove9to14bbUOPot;
            btnOpenShove15to25bbUOPot += statistic.BTNOpenShove15to25bbUOPot;
            btnOpenShove26to50bbUOPot += statistic.BTNOpenShove26to50bbUOPot;
            btnOpenShove51plusbbUOPot += statistic.BTNOpenShove51plusbbUOPot;

            coOpenShoveUOPot += statistic.COOpenShoveUOPot;
            coOpenShove1to8bbUOPot += statistic.COOpenShove1to8bbUOPot;
            coOpenShove9to14bbUOPot += statistic.COOpenShove9to14bbUOPot;
            coOpenShove15to25bbUOPot += statistic.COOpenShove15to25bbUOPot;
            coOpenShove26to50bbUOPot += statistic.COOpenShove26to50bbUOPot;
            coOpenShove51plusbbUOPot += statistic.COOpenShove51plusbbUOPot;

            mpOpenShoveUOPot += statistic.MPOpenShoveUOPot;
            mpOpenShove1to8bbUOPot += statistic.MPOpenShove1to8bbUOPot;
            mpOpenShove9to14bbUOPot += statistic.MPOpenShove9to14bbUOPot;
            mpOpenShove15to25bbUOPot += statistic.MPOpenShove15to25bbUOPot;
            mpOpenShove26to50bbUOPot += statistic.MPOpenShove26to50bbUOPot;
            mpOpenShove51plusbbUOPot += statistic.MPOpenShove51plusbbUOPot;

            epOpenShoveUOPot += statistic.EPOpenShoveUOPot;
            epOpenShove1to8bbUOPot += statistic.EPOpenShove1to8bbUOPot;
            epOpenShove9to14bbUOPot += statistic.EPOpenShove9to14bbUOPot;
            epOpenShove15to25bbUOPot += statistic.EPOpenShove15to25bbUOPot;
            epOpenShove26to50bbUOPot += statistic.EPOpenShove26to50bbUOPot;
            epOpenShove51plusbbUOPot += statistic.EPOpenShove51plusbbUOPot;
        }

        private void CleanOpenShoveUOPotStatistic()
        {
            sbOpenShoveUOPot = 0;
            sbOpenShove1to8bbUOPot = 0;
            sbOpenShove9to14bbUOPot = 0;
            sbOpenShove15to25bbUOPot = 0;
            sbOpenShove26to50bbUOPot = 0;
            sbOpenShove51plusbbUOPot = 0;

            btnOpenShoveUOPot = 0;
            btnOpenShove1to8bbUOPot = 0;
            btnOpenShove9to14bbUOPot = 0;
            btnOpenShove15to25bbUOPot = 0;
            btnOpenShove26to50bbUOPot = 0;
            btnOpenShove51plusbbUOPot = 0;

            coOpenShoveUOPot = 0;
            coOpenShove1to8bbUOPot = 0;
            coOpenShove9to14bbUOPot = 0;
            coOpenShove15to25bbUOPot = 0;
            coOpenShove26to50bbUOPot = 0;
            coOpenShove51plusbbUOPot = 0;

            mpOpenShoveUOPot = 0;
            mpOpenShove1to8bbUOPot = 0;
            mpOpenShove9to14bbUOPot = 0;
            mpOpenShove15to25bbUOPot = 0;
            mpOpenShove26to50bbUOPot = 0;
            mpOpenShove51plusbbUOPot = 0;

            epOpenShoveUOPot = 0;
            epOpenShove1to8bbUOPot = 0;
            epOpenShove9to14bbUOPot = 0;
            epOpenShove15to25bbUOPot = 0;
            epOpenShove26to50bbUOPot = 0;
            epOpenShove51plusbbUOPot = 0;
        }

        private void AddOpenShoveUOPotIndicators(LightIndicators indicator)
        {
            sbOpenShoveUOPot += indicator.sbOpenShoveUOPot;
            sbOpenShove1to8bbUOPot += indicator.sbOpenShove1to8bbUOPot;
            sbOpenShove9to14bbUOPot += indicator.sbOpenShove9to14bbUOPot;
            sbOpenShove15to25bbUOPot += indicator.sbOpenShove15to25bbUOPot;
            sbOpenShove26to50bbUOPot += indicator.sbOpenShove26to50bbUOPot;
            sbOpenShove51plusbbUOPot += indicator.sbOpenShove51plusbbUOPot;

            btnOpenShoveUOPot += indicator.btnOpenShoveUOPot;
            btnOpenShove1to8bbUOPot += indicator.btnOpenShove1to8bbUOPot;
            btnOpenShove9to14bbUOPot += indicator.btnOpenShove9to14bbUOPot;
            btnOpenShove15to25bbUOPot += indicator.btnOpenShove15to25bbUOPot;
            btnOpenShove26to50bbUOPot += indicator.btnOpenShove26to50bbUOPot;
            btnOpenShove51plusbbUOPot += indicator.btnOpenShove51plusbbUOPot;

            coOpenShoveUOPot += indicator.coOpenShoveUOPot;
            coOpenShove1to8bbUOPot += indicator.coOpenShove1to8bbUOPot;
            coOpenShove9to14bbUOPot += indicator.coOpenShove9to14bbUOPot;
            coOpenShove15to25bbUOPot += indicator.coOpenShove15to25bbUOPot;
            coOpenShove26to50bbUOPot += indicator.coOpenShove26to50bbUOPot;
            coOpenShove51plusbbUOPot += indicator.coOpenShove51plusbbUOPot;

            mpOpenShoveUOPot += indicator.mpOpenShoveUOPot;
            mpOpenShove1to8bbUOPot += indicator.mpOpenShove1to8bbUOPot;
            mpOpenShove9to14bbUOPot += indicator.mpOpenShove9to14bbUOPot;
            mpOpenShove15to25bbUOPot += indicator.mpOpenShove15to25bbUOPot;
            mpOpenShove26to50bbUOPot += indicator.mpOpenShove26to50bbUOPot;
            mpOpenShove51plusbbUOPot += indicator.mpOpenShove51plusbbUOPot;

            epOpenShoveUOPot += indicator.epOpenShoveUOPot;
            epOpenShove1to8bbUOPot += indicator.epOpenShove1to8bbUOPot;
            epOpenShove9to14bbUOPot += indicator.epOpenShove9to14bbUOPot;
            epOpenShove15to25bbUOPot += indicator.epOpenShove15to25bbUOPot;
            epOpenShove26to50bbUOPot += indicator.epOpenShove26to50bbUOPot;
            epOpenShove51plusbbUOPot += indicator.epOpenShove51plusbbUOPot;
        }

        #endregion

        #region Limp Positional & Fold to PFR%

        [ProtoMember(197)]
        protected int limpEPFoldToPFR;

        [ProtoMember(198)]
        protected int limpEPFacedPFR;

        public override decimal LimpEPFoldToPFR => GetPercentage(limpEPFoldToPFR, limpEPFacedPFR);

        [ProtoMember(199)]
        protected int limpMPFoldToPFR;

        [ProtoMember(200)]
        protected int limpMPFacedPFR;

        public override decimal LimpMPFoldToPFR => GetPercentage(limpMPFoldToPFR, limpMPFacedPFR);

        [ProtoMember(201)]
        protected int limpCOFoldToPFR;

        [ProtoMember(202)]
        protected int limpCOFacedPFR;

        public override decimal LimpCOFoldToPFR => GetPercentage(limpCOFoldToPFR, limpCOFacedPFR);

        [ProtoMember(203)]
        protected int limpBTNFoldToPFR;

        [ProtoMember(204)]
        protected int limpBTNFacedPFR;

        public override decimal LimpBTNFoldToPFR => GetPercentage(limpBTNFoldToPFR, limpBTNFacedPFR);

        [ProtoMember(205)]
        protected int limpSBFoldToPFR;

        [ProtoMember(206)]
        protected int limpSBFacedPFR;

        public override decimal LimpSBFoldToPFR => GetPercentage(limpSBFoldToPFR, limpSBFacedPFR);

        private void AddLimpPositionFoldToPFRStatistic(Playerstatistic statistic)
        {
            limpEPFoldToPFR += statistic.LimpEPFoldToPFR;
            limpEPFacedPFR += statistic.LimpEPFacedPFR;
            limpMPFoldToPFR += statistic.LimpMPFoldToPFR;
            limpMPFacedPFR += statistic.LimpMPFacedPFR;
            limpCOFoldToPFR += statistic.LimpCOFoldToPFR;
            limpCOFacedPFR += statistic.LimpCOFacedPFR;
            limpBTNFoldToPFR += statistic.LimpBTNFoldToPFR;
            limpBTNFacedPFR += statistic.LimpBTNFacedPFR;
            limpSBFoldToPFR += statistic.LimpSBFoldToPFR;
            limpSBFacedPFR += statistic.LimpSBFacedPFR;
        }

        private void CleanLimpPositionFoldToPFRStatistic()
        {
            limpEPFoldToPFR = 0;
            limpEPFacedPFR = 0;
            limpMPFoldToPFR = 0;
            limpMPFacedPFR = 0;
            limpCOFoldToPFR = 0;
            limpCOFacedPFR = 0;
            limpBTNFoldToPFR = 0;
            limpBTNFacedPFR = 0;
            limpSBFoldToPFR = 0;
            limpSBFacedPFR = 0;
        }

        private void AddLimpPositionFoldToPFRIndicators(LightIndicators indicator)
        {
            limpEPFoldToPFR += indicator.limpEPFoldToPFR;
            limpEPFacedPFR += indicator.limpEPFacedPFR;
            limpMPFoldToPFR += indicator.limpMPFoldToPFR;
            limpMPFacedPFR += indicator.limpMPFacedPFR;
            limpCOFoldToPFR += indicator.limpCOFoldToPFR;
            limpCOFacedPFR += indicator.limpCOFacedPFR;
            limpBTNFoldToPFR += indicator.limpBTNFoldToPFR;
            limpBTNFacedPFR += indicator.limpBTNFacedPFR;
            limpSBFoldToPFR += indicator.limpSBFoldToPFR;
            limpSBFacedPFR += indicator.limpSBFacedPFR;
        }

        #endregion

        #region Shoves over limpers positional

        [ProtoMember(207)]
        protected int sbShoveOverLimpers;

        [ProtoMember(208)]
        protected int sbShoveOverLimpers1to8bb;

        public override decimal SBShoveOverLimpers1to8bb => GetPercentage(sbShoveOverLimpers1to8bb, sbShoveOverLimpers);

        [ProtoMember(209)]
        protected int sbShoveOverLimpers9to14bb;

        public override decimal SBShoveOverLimpers9to14bb => GetPercentage(sbShoveOverLimpers9to14bb, sbShoveOverLimpers);

        [ProtoMember(210)]
        protected int sbShoveOverLimpers15to25bb;

        public override decimal SBShoveOverLimpers15to25bb => GetPercentage(sbShoveOverLimpers15to25bb, sbShoveOverLimpers);

        [ProtoMember(211)]
        protected int sbShoveOverLimpers26to50bb;

        public override decimal SBShoveOverLimpers26to50bb => GetPercentage(sbShoveOverLimpers26to50bb, sbShoveOverLimpers);

        [ProtoMember(212)]
        protected int sbShoveOverLimpers51plusbb;

        public override decimal SBShoveOverLimpers51plusbb => GetPercentage(sbShoveOverLimpers51plusbb, sbShoveOverLimpers);

        [ProtoMember(213)]
        protected int btnShoveOverLimpers;

        [ProtoMember(214)]
        protected int btnShoveOverLimpers1to8bb;

        public override decimal BTNShoveOverLimpers1to8bb => GetPercentage(btnShoveOverLimpers1to8bb, btnShoveOverLimpers);

        [ProtoMember(215)]
        protected int btnShoveOverLimpers9to14bb;

        public override decimal BTNShoveOverLimpers9to14bb => GetPercentage(btnShoveOverLimpers9to14bb, btnShoveOverLimpers);

        [ProtoMember(216)]
        protected int btnShoveOverLimpers15to25bb;

        public override decimal BTNShoveOverLimpers15to25bb => GetPercentage(btnShoveOverLimpers15to25bb, btnShoveOverLimpers);

        [ProtoMember(217)]
        protected int btnShoveOverLimpers26to50bb;

        public override decimal BTNShoveOverLimpers26to50bb => GetPercentage(btnShoveOverLimpers26to50bb, btnShoveOverLimpers);

        [ProtoMember(218)]
        protected int btnShoveOverLimpers51plusbb;

        public override decimal BTNShoveOverLimpers51plusbb => GetPercentage(btnShoveOverLimpers51plusbb, btnShoveOverLimpers);

        [ProtoMember(219)]
        protected int coShoveOverLimpers;

        [ProtoMember(220)]
        protected int coShoveOverLimpers1to8bb;

        public override decimal COShoveOverLimpers1to8bb => GetPercentage(coShoveOverLimpers1to8bb, coShoveOverLimpers);

        [ProtoMember(221)]
        protected int coShoveOverLimpers9to14bb;

        public override decimal COShoveOverLimpers9to14bb => GetPercentage(coShoveOverLimpers9to14bb, coShoveOverLimpers);

        [ProtoMember(222)]
        protected int coShoveOverLimpers15to25bb;

        public override decimal COShoveOverLimpers15to25bb => GetPercentage(coShoveOverLimpers15to25bb, coShoveOverLimpers);

        [ProtoMember(223)]
        protected int coShoveOverLimpers26to50bb;

        public override decimal COShoveOverLimpers26to50bb => GetPercentage(coShoveOverLimpers26to50bb, coShoveOverLimpers);

        [ProtoMember(224)]
        protected int coShoveOverLimpers51plusbb;

        public override decimal COShoveOverLimpers51plusbb => GetPercentage(coShoveOverLimpers51plusbb, coShoveOverLimpers);

        [ProtoMember(225)]
        protected int mpShoveOverLimpers;

        [ProtoMember(226)]
        protected int mpShoveOverLimpers1to8bb;

        public override decimal MPShoveOverLimpers1to8bb => GetPercentage(mpShoveOverLimpers1to8bb, mpShoveOverLimpers);

        [ProtoMember(227)]
        protected int mpShoveOverLimpers9to14bb;

        public override decimal MPShoveOverLimpers9to14bb => GetPercentage(mpShoveOverLimpers9to14bb, mpShoveOverLimpers);

        [ProtoMember(228)]
        protected int mpShoveOverLimpers15to25bb;

        public override decimal MPShoveOverLimpers15to25bb => GetPercentage(mpShoveOverLimpers15to25bb, mpShoveOverLimpers);

        [ProtoMember(229)]
        protected int mpShoveOverLimpers26to50bb;

        public override decimal MPShoveOverLimpers26to50bb => GetPercentage(mpShoveOverLimpers26to50bb, mpShoveOverLimpers);

        [ProtoMember(230)]
        protected int mpShoveOverLimpers51plusbb;

        public override decimal MPShoveOverLimpers51plusbb => GetPercentage(mpShoveOverLimpers51plusbb, mpShoveOverLimpers);

        [ProtoMember(231)]
        protected int epShoveOverLimpers;

        [ProtoMember(232)]
        protected int epShoveOverLimpers1to8bb;

        public override decimal EPShoveOverLimpers1to8bb => GetPercentage(epShoveOverLimpers1to8bb, epShoveOverLimpers);

        [ProtoMember(233)]
        protected int epShoveOverLimpers9to14bb;

        public override decimal EPShoveOverLimpers9to14bb => GetPercentage(epShoveOverLimpers9to14bb, epShoveOverLimpers);

        [ProtoMember(234)]
        protected int epShoveOverLimpers15to25bb;

        public override decimal EPShoveOverLimpers15to25bb => GetPercentage(epShoveOverLimpers15to25bb, epShoveOverLimpers);

        [ProtoMember(235)]
        protected int epShoveOverLimpers26to50bb;

        public override decimal EPShoveOverLimpers26to50bb => GetPercentage(epShoveOverLimpers26to50bb, epShoveOverLimpers);

        [ProtoMember(236)]
        protected int epShoveOverLimpers51plusbb;

        public override decimal EPShoveOverLimpers51plusbb => GetPercentage(epShoveOverLimpers51plusbb, epShoveOverLimpers);

        private void AddShoveOverLimpersStatistic(Playerstatistic statistic)
        {
            sbShoveOverLimpers += statistic.SBShoveOverLimpers;
            sbShoveOverLimpers1to8bb += statistic.SBShoveOverLimpers1to8bb;
            sbShoveOverLimpers9to14bb += statistic.SBShoveOverLimpers9to14bb;
            sbShoveOverLimpers15to25bb += statistic.SBShoveOverLimpers15to25bb;
            sbShoveOverLimpers26to50bb += statistic.SBShoveOverLimpers26to50bb;
            sbShoveOverLimpers51plusbb += statistic.SBShoveOverLimpers51plusbb;

            btnShoveOverLimpers += statistic.BTNShoveOverLimpers;
            btnShoveOverLimpers1to8bb += statistic.BTNShoveOverLimpers1to8bb;
            btnShoveOverLimpers9to14bb += statistic.BTNShoveOverLimpers9to14bb;
            btnShoveOverLimpers15to25bb += statistic.BTNShoveOverLimpers15to25bb;
            btnShoveOverLimpers26to50bb += statistic.BTNShoveOverLimpers26to50bb;
            btnShoveOverLimpers51plusbb += statistic.BTNShoveOverLimpers51plusbb;

            coShoveOverLimpers += statistic.COShoveOverLimpers;
            coShoveOverLimpers1to8bb += statistic.COShoveOverLimpers1to8bb;
            coShoveOverLimpers9to14bb += statistic.COShoveOverLimpers9to14bb;
            coShoveOverLimpers15to25bb += statistic.COShoveOverLimpers15to25bb;
            coShoveOverLimpers26to50bb += statistic.COShoveOverLimpers26to50bb;
            coShoveOverLimpers51plusbb += statistic.COShoveOverLimpers51plusbb;

            mpShoveOverLimpers += statistic.MPShoveOverLimpers;
            mpShoveOverLimpers1to8bb += statistic.MPShoveOverLimpers1to8bb;
            mpShoveOverLimpers9to14bb += statistic.MPShoveOverLimpers9to14bb;
            mpShoveOverLimpers15to25bb += statistic.MPShoveOverLimpers15to25bb;
            mpShoveOverLimpers26to50bb += statistic.MPShoveOverLimpers26to50bb;
            mpShoveOverLimpers51plusbb += statistic.MPShoveOverLimpers51plusbb;

            epShoveOverLimpers += statistic.EPShoveOverLimpers;
            epShoveOverLimpers1to8bb += statistic.EPShoveOverLimpers1to8bb;
            epShoveOverLimpers9to14bb += statistic.EPShoveOverLimpers9to14bb;
            epShoveOverLimpers15to25bb += statistic.EPShoveOverLimpers15to25bb;
            epShoveOverLimpers26to50bb += statistic.EPShoveOverLimpers26to50bb;
            epShoveOverLimpers51plusbb += statistic.EPShoveOverLimpers51plusbb;
        }

        private void CleanShoveOverLimpersStatistic()
        {
            sbShoveOverLimpers = 0;
            sbShoveOverLimpers1to8bb = 0;
            sbShoveOverLimpers9to14bb = 0;
            sbShoveOverLimpers15to25bb = 0;
            sbShoveOverLimpers26to50bb = 0;
            sbShoveOverLimpers51plusbb = 0;

            btnShoveOverLimpers = 0;
            btnShoveOverLimpers1to8bb = 0;
            btnShoveOverLimpers9to14bb = 0;
            btnShoveOverLimpers15to25bb = 0;
            btnShoveOverLimpers26to50bb = 0;
            btnShoveOverLimpers51plusbb = 0;

            coShoveOverLimpers = 0;
            coShoveOverLimpers1to8bb = 0;
            coShoveOverLimpers9to14bb = 0;
            coShoveOverLimpers15to25bb = 0;
            coShoveOverLimpers26to50bb = 0;
            coShoveOverLimpers51plusbb = 0;

            mpShoveOverLimpers = 0;
            mpShoveOverLimpers1to8bb = 0;
            mpShoveOverLimpers9to14bb = 0;
            mpShoveOverLimpers15to25bb = 0;
            mpShoveOverLimpers26to50bb = 0;
            mpShoveOverLimpers51plusbb = 0;

            epShoveOverLimpers = 0;
            epShoveOverLimpers1to8bb = 0;
            epShoveOverLimpers9to14bb = 0;
            epShoveOverLimpers15to25bb = 0;
            epShoveOverLimpers26to50bb = 0;
            epShoveOverLimpers51plusbb = 0;
        }

        private void AddShoveOverLimpersIndicators(LightIndicators indicator)
        {
            sbShoveOverLimpers += indicator.sbShoveOverLimpers;
            sbShoveOverLimpers1to8bb += indicator.sbShoveOverLimpers1to8bb;
            sbShoveOverLimpers9to14bb += indicator.sbShoveOverLimpers9to14bb;
            sbShoveOverLimpers15to25bb += indicator.sbShoveOverLimpers15to25bb;
            sbShoveOverLimpers26to50bb += indicator.sbShoveOverLimpers26to50bb;
            sbShoveOverLimpers51plusbb += indicator.sbShoveOverLimpers51plusbb;

            btnShoveOverLimpers += indicator.btnShoveOverLimpers;
            btnShoveOverLimpers1to8bb += indicator.btnShoveOverLimpers1to8bb;
            btnShoveOverLimpers9to14bb += indicator.btnShoveOverLimpers9to14bb;
            btnShoveOverLimpers15to25bb += indicator.btnShoveOverLimpers15to25bb;
            btnShoveOverLimpers26to50bb += indicator.btnShoveOverLimpers26to50bb;
            btnShoveOverLimpers51plusbb += indicator.btnShoveOverLimpers51plusbb;

            coShoveOverLimpers += indicator.coShoveOverLimpers;
            coShoveOverLimpers1to8bb += indicator.coShoveOverLimpers1to8bb;
            coShoveOverLimpers9to14bb += indicator.coShoveOverLimpers9to14bb;
            coShoveOverLimpers15to25bb += indicator.coShoveOverLimpers15to25bb;
            coShoveOverLimpers26to50bb += indicator.coShoveOverLimpers26to50bb;
            coShoveOverLimpers51plusbb += indicator.coShoveOverLimpers51plusbb;

            mpShoveOverLimpers += indicator.mpShoveOverLimpers;
            mpShoveOverLimpers1to8bb += indicator.mpShoveOverLimpers1to8bb;
            mpShoveOverLimpers9to14bb += indicator.mpShoveOverLimpers9to14bb;
            mpShoveOverLimpers15to25bb += indicator.mpShoveOverLimpers15to25bb;
            mpShoveOverLimpers26to50bb += indicator.mpShoveOverLimpers26to50bb;
            mpShoveOverLimpers51plusbb += indicator.mpShoveOverLimpers51plusbb;

            epShoveOverLimpers += indicator.epShoveOverLimpers;
            epShoveOverLimpers1to8bb += indicator.epShoveOverLimpers1to8bb;
            epShoveOverLimpers9to14bb += indicator.epShoveOverLimpers9to14bb;
            epShoveOverLimpers15to25bb += indicator.epShoveOverLimpers15to25bb;
            epShoveOverLimpers26to50bb += indicator.epShoveOverLimpers26to50bb;
            epShoveOverLimpers51plusbb += indicator.epShoveOverLimpers51plusbb;
        }

        #endregion

        #region Open minraise

        [ProtoMember(237)]
        protected int openMinraise;

        public override decimal OpenMinraise => GetPercentage(openMinraise, TotalHands - NumberOfWalks);

        #endregion

        #region Squeeze vs PFR

        [ProtoMember(238)]
        protected int didSqueezeBBVsBTNPFR;

        [ProtoMember(239)]
        protected int couldSqueezeBBVsBTNPFR;

        public override decimal SqueezeBBVsBTNPFR => GetPercentage(didSqueezeBBVsBTNPFR, couldSqueezeBBVsBTNPFR);

        [ProtoMember(240)]
        protected int didSqueezeBBVsCOPFR;

        [ProtoMember(241)]
        protected int couldSqueezeBBVsCOPFR;

        public override decimal SqueezeBBVsCOPFR => GetPercentage(didSqueezeBBVsCOPFR, couldSqueezeBBVsCOPFR);

        [ProtoMember(242)]
        protected int didSqueezeBBVsMPPFR;

        [ProtoMember(243)]
        protected int couldSqueezeBBVsMPPFR;

        public override decimal SqueezeBBVsMPPFR => GetPercentage(didSqueezeBBVsMPPFR, couldSqueezeBBVsMPPFR);

        [ProtoMember(244)]
        protected int didSqueezeBBVsEPPFR;

        [ProtoMember(245)]
        protected int couldSqueezeBBVsEPPFR;

        public override decimal SqueezeBBVsEPPFR => GetPercentage(didSqueezeBBVsEPPFR, couldSqueezeBBVsEPPFR);

        [ProtoMember(246)]
        protected int didSqueezeSBVsCOPFR;

        [ProtoMember(247)]
        protected int couldSqueezeSBVsCOPFR;

        public override decimal SqueezeSBVsCOPFR => GetPercentage(didSqueezeSBVsCOPFR, couldSqueezeSBVsCOPFR);

        [ProtoMember(248)]
        protected int didSqueezeSBVsMPPFR;

        [ProtoMember(249)]
        protected int couldSqueezeSBVsMPPFR;

        public override decimal SqueezeSBVsMPPFR => GetPercentage(didSqueezeSBVsMPPFR, couldSqueezeSBVsMPPFR);

        [ProtoMember(250)]
        protected int didSqueezeSBVsEPPFR;

        [ProtoMember(251)]
        protected int couldSqueezeSBVsEPPFR;

        public override decimal SqueezeSBVsEPPFR => GetPercentage(didSqueezeSBVsEPPFR, couldSqueezeSBVsEPPFR);

        [ProtoMember(252)]
        protected int didSqueezeBTNVsMPPFR;

        [ProtoMember(253)]
        protected int couldSqueezeBTNVsMPPFR;

        public override decimal SqueezeBTNVsMPPFR => GetPercentage(didSqueezeBTNVsMPPFR, couldSqueezeBTNVsMPPFR);

        [ProtoMember(254)]
        protected int didSqueezeBTNVsEPPFR;

        [ProtoMember(255)]
        protected int couldSqueezeBTNVsEPPFR;

        public override decimal SqueezeBTNVsEPPFR => GetPercentage(didSqueezeBTNVsEPPFR, couldSqueezeBTNVsEPPFR);

        [ProtoMember(256)]
        protected int didSqueezeCOVsMPPFR;

        [ProtoMember(257)]
        protected int couldSqueezeCOVsMPPFR;

        public override decimal SqueezeCOVsMPPFR => GetPercentage(didSqueezeCOVsMPPFR, couldSqueezeCOVsMPPFR);

        [ProtoMember(258)]
        protected int didSqueezeCOVsEPPFR;

        [ProtoMember(259)]
        protected int couldSqueezeCOVsEPPFR;

        public override decimal SqueezeCOVsEPPFR => GetPercentage(didSqueezeCOVsEPPFR, couldSqueezeCOVsEPPFR);

        [ProtoMember(260)]
        protected int didSqueezeMPVsEPPFR;

        [ProtoMember(261)]
        protected int couldSqueezeMPVsEPPFR;

        public override decimal SqueezeMPVsEPPFR => GetPercentage(didSqueezeMPVsEPPFR, couldSqueezeMPVsEPPFR);

        [ProtoMember(262)]
        protected int didSqueezeEPVsEPPFR;

        [ProtoMember(263)]
        protected int couldSqueezeEPVsEPPFR;

        public override decimal SqueezeEPVsEPPFR => GetPercentage(didSqueezeEPVsEPPFR, couldSqueezeEPVsEPPFR);

        private void AddSqueezeVsPFRStatistic(Playerstatistic statistic)
        {
            didSqueezeBBVsBTNPFR += statistic.DidSqueezeBBVsBTNPFR;
            couldSqueezeBBVsBTNPFR += statistic.CouldSqueezeBBVsBTNPFR;

            didSqueezeBBVsCOPFR += statistic.DidSqueezeBBVsCOPFR;
            couldSqueezeBBVsCOPFR += statistic.CouldSqueezeBBVsCOPFR;

            didSqueezeBBVsMPPFR += statistic.DidSqueezeBBVsMPPFR;
            couldSqueezeBBVsMPPFR += statistic.CouldSqueezeBBVsMPPFR;

            didSqueezeBBVsEPPFR += statistic.DidSqueezeBBVsEPPFR;
            couldSqueezeBBVsEPPFR += statistic.CouldSqueezeBBVsEPPFR;

            didSqueezeSBVsCOPFR += statistic.DidSqueezeSBVsCOPFR;
            couldSqueezeSBVsCOPFR += statistic.CouldSqueezeSBVsCOPFR;

            didSqueezeSBVsMPPFR += statistic.DidSqueezeSBVsMPPFR;
            couldSqueezeSBVsMPPFR += statistic.CouldSqueezeSBVsMPPFR;

            didSqueezeSBVsEPPFR += statistic.DidSqueezeSBVsEPPFR;
            couldSqueezeSBVsEPPFR += statistic.CouldSqueezeSBVsEPPFR;

            didSqueezeBTNVsMPPFR += statistic.DidSqueezeBTNVsMPPFR;
            couldSqueezeBTNVsMPPFR += statistic.CouldSqueezeBTNVsMPPFR;

            didSqueezeBTNVsEPPFR += statistic.DidSqueezeBTNVsEPPFR;
            couldSqueezeBTNVsEPPFR += statistic.CouldSqueezeBTNVsEPPFR;

            didSqueezeCOVsMPPFR += statistic.DidSqueezeCOVsMPPFR;
            couldSqueezeCOVsMPPFR += statistic.CouldSqueezeCOVsMPPFR;

            didSqueezeCOVsEPPFR += statistic.DidSqueezeCOVsEPPFR;
            couldSqueezeCOVsEPPFR += statistic.CouldSqueezeCOVsEPPFR;

            didSqueezeMPVsEPPFR += statistic.DidSqueezeMPVsEPPFR;
            couldSqueezeMPVsEPPFR += statistic.CouldSqueezeMPVsEPPFR;

            didSqueezeEPVsEPPFR += statistic.DidSqueezeEPVsEPPFR;
            couldSqueezeEPVsEPPFR += statistic.CouldSqueezeEPVsEPPFR;
        }

        private void CleanSqueezeVsPFRStatistic()
        {
            didSqueezeBBVsBTNPFR = 0;
            couldSqueezeBBVsBTNPFR = 0;

            didSqueezeBBVsCOPFR = 0;
            couldSqueezeBBVsCOPFR = 0;

            didSqueezeBBVsMPPFR = 0;
            couldSqueezeBBVsMPPFR = 0;

            didSqueezeBBVsEPPFR = 0;
            couldSqueezeBBVsEPPFR = 0;

            didSqueezeSBVsCOPFR = 0;
            couldSqueezeSBVsCOPFR = 0;

            didSqueezeSBVsMPPFR = 0;
            couldSqueezeSBVsMPPFR = 0;

            didSqueezeSBVsEPPFR = 0;
            couldSqueezeSBVsEPPFR = 0;

            didSqueezeBTNVsMPPFR = 0;
            couldSqueezeBTNVsMPPFR = 0;

            didSqueezeBTNVsEPPFR = 0;
            couldSqueezeBTNVsEPPFR = 0;

            didSqueezeCOVsMPPFR = 0;
            couldSqueezeCOVsMPPFR = 0;

            didSqueezeCOVsEPPFR = 0;
            couldSqueezeCOVsEPPFR = 0;

            didSqueezeMPVsEPPFR = 0;
            couldSqueezeMPVsEPPFR = 0;

            didSqueezeEPVsEPPFR = 0;
            couldSqueezeEPVsEPPFR = 0;
        }

        private void AddSqueezeVsPFRIndicators(LightIndicators indicator)
        {
            didSqueezeBBVsBTNPFR += indicator.didSqueezeBBVsBTNPFR;
            couldSqueezeBBVsBTNPFR += indicator.couldSqueezeBBVsBTNPFR;

            didSqueezeBBVsCOPFR += indicator.didSqueezeBBVsCOPFR;
            couldSqueezeBBVsCOPFR += indicator.couldSqueezeBBVsCOPFR;

            didSqueezeBBVsMPPFR += indicator.didSqueezeBBVsMPPFR;
            couldSqueezeBBVsMPPFR += indicator.couldSqueezeBBVsMPPFR;

            didSqueezeBBVsEPPFR += indicator.didSqueezeBBVsEPPFR;
            couldSqueezeBBVsEPPFR += indicator.couldSqueezeBBVsEPPFR;

            didSqueezeSBVsCOPFR += indicator.didSqueezeSBVsCOPFR;
            couldSqueezeSBVsCOPFR += indicator.couldSqueezeSBVsCOPFR;

            didSqueezeSBVsMPPFR += indicator.didSqueezeSBVsMPPFR;
            couldSqueezeSBVsMPPFR += indicator.couldSqueezeSBVsMPPFR;

            didSqueezeSBVsEPPFR += indicator.didSqueezeSBVsEPPFR;
            couldSqueezeSBVsEPPFR += indicator.couldSqueezeSBVsEPPFR;

            didSqueezeBTNVsMPPFR += indicator.didSqueezeBTNVsMPPFR;
            couldSqueezeBTNVsMPPFR += indicator.couldSqueezeBTNVsMPPFR;

            didSqueezeBTNVsEPPFR += indicator.didSqueezeBTNVsEPPFR;
            couldSqueezeBTNVsEPPFR += indicator.couldSqueezeBTNVsEPPFR;

            didSqueezeCOVsMPPFR += indicator.didSqueezeCOVsMPPFR;
            couldSqueezeCOVsMPPFR += indicator.couldSqueezeCOVsMPPFR;

            didSqueezeCOVsEPPFR += indicator.didSqueezeCOVsEPPFR;
            couldSqueezeCOVsEPPFR += indicator.couldSqueezeCOVsEPPFR;

            didSqueezeMPVsEPPFR += indicator.didSqueezeMPVsEPPFR;
            couldSqueezeMPVsEPPFR += indicator.couldSqueezeMPVsEPPFR;

            didSqueezeEPVsEPPFR += indicator.didSqueezeEPVsEPPFR;
            couldSqueezeEPVsEPPFR += indicator.couldSqueezeEPVsEPPFR;
        }

        #endregion

        #region Fold to Squeeze as Cold Caller

        [ProtoMember(264)]
        protected int foldToSqueezeAsColdCaller;

        [ProtoMember(265)]
        protected int facedSqueezeAsColdCaller;

        public override decimal FoldToSqueezeAsColdCaller => GetPercentage(foldToSqueezeAsColdCaller, facedSqueezeAsColdCaller);

        #endregion

        #region 4-Bet vs Blind 3-Bet%

        [ProtoMember(266)]
        protected int did4BetVsBlind3Bet;

        [ProtoMember(267)]
        protected int could4BetVsBlind3Bet;

        public override decimal FourBetVsBlind3Bet => GetPercentage(did4BetVsBlind3Bet, could4BetVsBlind3Bet);

        #endregion

        #region BTN Re/Def vs CO Steal

        [ProtoMember(268)]
        protected int btnReStealVsCOSteal;

        [ProtoMember(269)]
        protected int btnFacedCOSteal;

        [ProtoMember(270)]
        protected int btnDefendVsCOSteal;

        public override decimal BTNReStealVsCOSteal => GetPercentage(btnReStealVsCOSteal, btnFacedCOSteal);

        public override decimal BTNDefendVsCOSteal => GetPercentage(btnDefendVsCOSteal, btnFacedCOSteal);

        #endregion

        #region Positional Call & Fold to Steal

        [ProtoMember(271)]
        protected int foldToStealInSB;

        [ProtoMember(272)]
        protected int facedStealInSB;

        public override decimal FoldToStealInSB => GetPercentage(foldToStealInSB, facedStealInSB);

        [ProtoMember(273)]
        protected int foldToStealInBB;

        [ProtoMember(274)]
        protected int facedStealInBB;

        public override decimal FoldToStealInBB => GetPercentage(foldToStealInBB, facedStealInBB);

        [ProtoMember(275)]
        protected int calledStealInSB;

        public override decimal CalledStealInSB => GetPercentage(calledStealInSB, facedStealInSB);

        [ProtoMember(276)]
        protected int calledStealInBB;

        public override decimal CalledStealInBB => GetPercentage(calledStealInBB, facedStealInBB);

        [ProtoMember(277)]
        protected int foldToBTNStealInSB;

        [ProtoMember(278)]
        protected int facedBTNStealInSB;

        public override decimal FoldToBTNStealInSB => GetPercentage(foldToBTNStealInSB, facedBTNStealInSB);

        [ProtoMember(279)]
        protected int foldToBTNStealInBB;

        [ProtoMember(280)]
        protected int facedBTNStealInBB;

        public override decimal FoldToBTNStealInBB => GetPercentage(foldToBTNStealInBB, facedBTNStealInBB);

        [ProtoMember(281)]
        protected int foldToCOStealInSB;

        [ProtoMember(282)]
        protected int facedCOStealInSB;

        public override decimal FoldToCOStealInSB => GetPercentage(foldToCOStealInSB, facedCOStealInSB);

        [ProtoMember(283)]
        protected int foldToCOStealInBB;

        [ProtoMember(284)]
        protected int facedCOStealInBB;

        public override decimal FoldToCOStealInBB => GetPercentage(foldToCOStealInBB, facedCOStealInBB);

        [ProtoMember(285)]
        protected int calledBTNStealInSB;

        public override decimal CalledBTNStealInSB => GetPercentage(calledBTNStealInSB, facedBTNStealInSB);

        [ProtoMember(286)]
        protected int calledBTNStealInBB;

        public override decimal CalledBTNStealInBB => GetPercentage(calledBTNStealInBB, facedBTNStealInBB);

        [ProtoMember(287)]
        protected int calledCOStealInSB;

        public override decimal CalledCOStealInSB => GetPercentage(calledCOStealInSB, facedCOStealInSB);

        [ProtoMember(288)]
        protected int calledCOStealInBB;

        public override decimal CalledCOStealInBB => GetPercentage(calledCOStealInBB, facedCOStealInBB);

        private void AddPositionalCallFoldToStealStatistic(Playerstatistic statistic)
        {
            foldToStealInSB += statistic.FoldToStealInSB;
            facedStealInSB += statistic.FacedStealInSB;
            foldToStealInBB += statistic.FoldToStealInBB;
            facedStealInBB += statistic.FacedStealInBB;
            calledStealInSB += statistic.CalledStealInSB;
            calledStealInBB += statistic.CalledStealInBB;
            foldToBTNStealInSB += statistic.FoldToBTNStealInSB;
            facedBTNStealInSB += statistic.FacedBTNStealInSB;
            foldToBTNStealInBB += statistic.FoldToBTNStealInBB;
            facedBTNStealInBB += statistic.FacedBTNStealInBB;
            foldToCOStealInSB += statistic.FoldToCOStealInSB;
            facedCOStealInSB += statistic.FacedCOStealInSB;
            foldToCOStealInBB += statistic.FoldToCOStealInBB;
            facedCOStealInBB += statistic.FacedCOStealInBB;
            calledBTNStealInSB += statistic.CalledBTNStealInSB;
            calledBTNStealInBB += statistic.CalledBTNStealInBB;
            calledCOStealInSB += statistic.CalledCOStealInSB;
            calledCOStealInBB += statistic.CalledCOStealInBB;
        }

        private void CleanPositionalCallFoldToStealStatistic()
        {
            foldToStealInSB = 0;
            facedStealInSB = 0;
            foldToStealInBB = 0;
            facedStealInBB = 0;
            calledStealInSB = 0;
            calledStealInBB = 0;
            foldToBTNStealInSB = 0;
            facedBTNStealInSB = 0;
            foldToBTNStealInBB = 0;
            facedBTNStealInBB = 0;
            foldToCOStealInSB = 0;
            facedCOStealInSB = 0;
            foldToCOStealInBB = 0;
            facedCOStealInBB = 0;
            calledBTNStealInSB = 0;
            calledBTNStealInBB = 0;
            calledCOStealInSB = 0;
            calledCOStealInBB = 0;
        }

        private void AddPositionalCallFoldToStealIndicators(LightIndicators indicator)
        {
            foldToStealInSB += indicator.foldToStealInSB;
            facedStealInSB += indicator.facedStealInSB;
            foldToStealInBB += indicator.foldToStealInBB;
            facedStealInBB += indicator.facedStealInBB;
            calledStealInSB += indicator.calledStealInSB;
            calledStealInBB += indicator.calledStealInBB;
            foldToBTNStealInSB += indicator.foldToBTNStealInSB;
            facedBTNStealInSB += indicator.facedBTNStealInSB;
            foldToBTNStealInBB += indicator.foldToBTNStealInBB;
            facedBTNStealInBB += indicator.facedBTNStealInBB;
            foldToCOStealInSB += indicator.foldToCOStealInSB;
            facedCOStealInSB += indicator.facedCOStealInSB;
            foldToCOStealInBB += indicator.foldToCOStealInBB;
            facedCOStealInBB += indicator.facedCOStealInBB;
            calledBTNStealInSB += indicator.calledBTNStealInSB;
            calledBTNStealInBB += indicator.calledBTNStealInBB;
            calledCOStealInSB += indicator.calledCOStealInSB;
            calledCOStealInBB += indicator.calledCOStealInBB;
        }

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

            probeBetTurn += statistic.ProbeBetTurn;
            probeBetRiver += statistic.ProbeBetRiver;

            floatFlopThenBetTurn += statistic.FloatFlopThenBetTurn;
            couldFloatFlopThenBetTurn += statistic.CouldFloatFlopThenBetTurn;

            foldBBvsSBSteal += statistic.FoldBBvsSBSteal;
            couldFoldBBvsSBSteal += statistic.CouldFoldBBvsSBSteal;

            betTurnWhenCheckedToSRP += statistic.BetTurnWhenCheckedToSRP;
            couldBetTurnWhenCheckedToSRP += statistic.CouldBetTurnWhenCheckedToSRP;
            betRiverWhenCheckedToSRP += statistic.BetRiverWhenCheckedToSRP;
            couldBetRiverWhenCheckedToSRP += statistic.CouldBetRiverWhenCheckedToSRP;

            doubleBarrelSRP += statistic.DoubleBarrelSRP;
            couldDoubleBarrelSRP += statistic.CouldDoubleBarrelSRP;
            doubleBarrel3BetPot += statistic.DoubleBarrel3BetPot;
            couldDoubleBarrel3BetPot += statistic.CouldDoubleBarrel3BetPot;

            tripleBarrelSRP += statistic.TripleBarrelSRP;
            couldTripleBarrelSRP += statistic.CouldTripleBarrelSRP;
            tripleBarrel3BetPot += statistic.TripleBarrel3BetPot;
            couldTripleBarrel3BetPot += statistic.CouldTripleBarrel3BetPot;

            cBetThenFoldFlopSRP += statistic.CBetThenFoldFlopSRP;
            couldCBetThenFoldFlopSRP += statistic.CouldCBetThenFoldFlopSRP;

            foldedToProbeBetTurn += statistic.FoldedToProbeBetTurn;
            facedProbeBetTurn += statistic.FacedProbeBetTurn;
            foldedToProbeBetRiver += statistic.FoldedToProbeBetRiver;
            facedProbeBetRiver += statistic.FacedProbeBetRiver;

            openMinraise += statistic.OpenMinraise;

            foldToSqueezeAsColdCaller += statistic.FoldToSqueezeAsColdCaller;
            facedSqueezeAsColdCaller += statistic.FacedSqueezeAsColdCaller;

            did4BetVsBlind3Bet += statistic.Did4BetVsBlind3Bet;
            could4BetVsBlind3Bet += statistic.Could4BetVsBlind3Bet;

            btnReStealVsCOSteal += statistic.BTNReStealVsCOSteal;
            btnDefendVsCOSteal += statistic.BTNDefendVsCOSteal;
            btnFacedCOSteal += statistic.BTNFacedCOSteal;

            Add3BetVsRaiserInPosStatistic(statistic);
            AddFoldTo3BetInPosVs3BetPosStatistic(statistic);
            AddBetWhenCheckedToIn3BetPotStatistic(statistic);
            AddCheckFlopAsPFRAndFoldToTurnBetSRPStatistic(statistic);
            AddCheckFlopAsPFRAndFoldToTurnBet3BetPotStatistic(statistic);
            AddFoldToContinuationBetsInPotsStatistic(statistic);
            AddOpenShoveUOPotStatistic(statistic);
            AddLimpPositionFoldToPFRStatistic(statistic);
            AddShoveOverLimpersStatistic(statistic);
            AddSqueezeVsPFRStatistic(statistic);
            AddPositionalCallFoldToStealStatistic(statistic);
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

            probeBetTurn = 0;
            probeBetRiver = 0;

            floatFlopThenBetTurn = 0;
            couldFloatFlopThenBetTurn = 0;

            foldBBvsSBSteal = 0;
            couldFoldBBvsSBSteal = 0;

            betTurnWhenCheckedToSRP = 0;
            couldBetTurnWhenCheckedToSRP = 0;
            betRiverWhenCheckedToSRP = 0;
            couldBetRiverWhenCheckedToSRP = 0;

            doubleBarrelSRP = 0;
            couldDoubleBarrelSRP = 0;
            doubleBarrel3BetPot = 0;
            couldDoubleBarrel3BetPot = 0;

            tripleBarrelSRP = 0;
            couldTripleBarrelSRP = 0;
            tripleBarrel3BetPot = 0;
            couldTripleBarrel3BetPot = 0;

            cBetThenFoldFlopSRP = 0;
            couldCBetThenFoldFlopSRP = 0;

            foldedToProbeBetTurn = 0;
            facedProbeBetTurn = 0;
            foldedToProbeBetRiver = 0;
            facedProbeBetRiver = 0;

            openMinraise = 0;

            foldToSqueezeAsColdCaller = 0;
            facedSqueezeAsColdCaller = 0;

            did4BetVsBlind3Bet = 0;
            could4BetVsBlind3Bet = 0;

            btnReStealVsCOSteal = 0;
            btnDefendVsCOSteal = 0;
            btnFacedCOSteal = 0;

            Clean3BetVsRaiserInPos();
            CleanFoldTo3BetInPosVs3BetPos();
            CleanBetWhenCheckedToIn3BetPot();
            CleanCheckFlopAsPFRAndFoldToTurnBetSRP();
            CleanCheckFlopAsPFRAndFoldToTurnBet3BetPot();
            CleanFoldToContinuationBetsInPots();
            CleanOpenShoveUOPotStatistic();
            CleanLimpPositionFoldToPFRStatistic();
            CleanShoveOverLimpersStatistic();
            CleanSqueezeVsPFRStatistic();
            CleanPositionalCallFoldToStealStatistic();
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
            positionDidColdCallThreeBet?.Add(indicator.positionDidColdCallThreeBet);
            positionCouldColdCallThreeBet?.Add(indicator.positionCouldColdCallThreeBet);
            positionDidColdCallFourBet?.Add(indicator.positionDidColdCallFourBet);
            positionCouldColdCallFourBet?.Add(indicator.positionCouldColdCallFourBet);
            positionOpenMinraiseUOPFR?.Add(indicator.positionOpenMinraiseUOPFR);
            positionDidSqueeze?.Add(indicator.positionDidSqueeze);
            positionCouldSqueeze?.Add(indicator.positionCouldSqueeze);

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

            probeBetTurn += indicator.probeBetTurn;
            probeBetRiver += indicator.probeBetRiver;

            floatFlopThenBetTurn += indicator.floatFlopThenBetTurn;
            couldFloatFlopThenBetTurn += indicator.couldFloatFlopThenBetTurn;

            foldBBvsSBSteal += indicator.foldBBvsSBSteal;
            couldFoldBBvsSBSteal += indicator.couldFoldBBvsSBSteal;

            betTurnWhenCheckedToSRP += indicator.betTurnWhenCheckedToSRP;
            couldBetTurnWhenCheckedToSRP += indicator.couldBetTurnWhenCheckedToSRP;
            betRiverWhenCheckedToSRP += indicator.betRiverWhenCheckedToSRP;
            couldBetRiverWhenCheckedToSRP += indicator.couldBetRiverWhenCheckedToSRP;

            doubleBarrelSRP += indicator.doubleBarrelSRP;
            couldDoubleBarrelSRP += indicator.couldDoubleBarrelSRP;
            doubleBarrel3BetPot += indicator.doubleBarrel3BetPot;
            couldDoubleBarrel3BetPot += indicator.couldDoubleBarrel3BetPot;

            tripleBarrelSRP += indicator.tripleBarrelSRP;
            couldTripleBarrelSRP += indicator.couldTripleBarrelSRP;
            tripleBarrel3BetPot += indicator.tripleBarrel3BetPot;
            couldTripleBarrel3BetPot += indicator.couldTripleBarrel3BetPot;

            cBetThenFoldFlopSRP += indicator.cBetThenFoldFlopSRP;
            couldCBetThenFoldFlopSRP += indicator.couldCBetThenFoldFlopSRP;

            foldedToProbeBetTurn += indicator.foldedToProbeBetTurn;
            facedProbeBetTurn += indicator.facedProbeBetTurn;
            foldedToProbeBetRiver += indicator.foldedToProbeBetRiver;
            facedProbeBetRiver += indicator.facedProbeBetRiver;

            openMinraise += indicator.openMinraise;

            foldToSqueezeAsColdCaller += indicator.foldToSqueezeAsColdCaller;
            facedSqueezeAsColdCaller += indicator.facedSqueezeAsColdCaller;

            did4BetVsBlind3Bet += indicator.did4BetVsBlind3Bet;
            could4BetVsBlind3Bet += indicator.could4BetVsBlind3Bet;

            btnReStealVsCOSteal += indicator.btnReStealVsCOSteal;
            btnDefendVsCOSteal += indicator.btnDefendVsCOSteal;
            btnFacedCOSteal += indicator.btnFacedCOSteal;

            Add3BetVsRaiserInPosIndicator(indicator);
            AddFoldTo3BetInPosVs3BetPosIndicator(indicator);
            AddBetWhenCheckedToIn3BetPotIndicator(indicator);
            AddCheckFlopAsPFRAndFoldToTurnBetSRPIndicator(indicator);
            AddCheckFlopAsPFRAndFoldToTurnBet3BetPotIndicator(indicator);
            AddFoldToContinuationBetsInPotsIndicator(indicator);
            AddOpenShoveUOPotIndicators(indicator);
            AddLimpPositionFoldToPFRIndicators(indicator);
            AddShoveOverLimpersIndicators(indicator);
            AddSqueezeVsPFRIndicators(indicator);
            AddPositionalCallFoldToStealIndicators(indicator);
        }

        public override int CompareTo(object obj)
        {
            if (!(obj is LightIndicators objIndicator))
            {
                return 1;
            }

            return objIndicator.gameNumberMax.CompareTo(gameNumberMax);
        }

        #endregion       
    }
}