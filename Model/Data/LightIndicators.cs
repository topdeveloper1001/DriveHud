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
using System;
using System.Collections.Generic;

namespace Model.Data
{
    public class LightIndicators : Indicators
    {
        private int statisticCount;
        private decimal netWon;
        private decimal bigBlind;
        private decimal netWonByBigBlind;
        private DateTime sessionStartTime = DateTime.MaxValue;
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
                return Math.Round(GetDevisionResult(netWonByBigBlind, totalhands), 2);
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

        #endregion

        #region overridden methods

        public override void AddStatistic(Playerstatistic statistic)
        {
            statistic.CalculatePositionalStats();

            Source += statistic;

            statisticCount++;
            netWon += statistic.NetWon;
            bigBlind += statistic.BigBlind;
            netWonByBigBlind += GetDevisionResult(statistic.NetWon, statistic.BigBlind);

            if (sessionStartTime > statistic.Time)
            {
                sessionStartTime = statistic.Time;
            }

            if (sessionEndTime < statistic.Time)
            {
                sessionEndTime = statistic.Time;
            }
        }

        #endregion

        #region Help methods

        protected decimal GetPercentage(decimal? actual, decimal? possible)
        {
            if (TotalHands == 0)
                return 0;

            if (!possible.HasValue || !actual.HasValue || possible == 0)
                return 0;

            return (actual.Value / possible.Value) * 100;
        }

        #endregion
    }
}