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

        public override decimal BB
        {
            get
            {
                var totalhands = statisticCount / 100m;
                return Math.Round(GetDevisionResult(netWonByBigBlind, totalhands), 2);
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

        private int UnopenedEPPositionCount;

        public override decimal UO_PFR_EP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_EP, UnopenedEPPositionCount);
            }
        }

        private int UnopenedMPPositionCount;

        public override decimal UO_PFR_MP
        {
            get
            {
                return GetPercentage(Source.UO_PFR_MP, UnopenedMPPositionCount);
            }
        }

        private int UnopenedCOPositionCount;

        public override decimal UO_PFR_CO
        {
            get
            {
                return GetPercentage(Source.UO_PFR_CO, UnopenedCOPositionCount);
            }
        }

        private int UnopenedBTNPositionCount;

        public override decimal UO_PFR_BN
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BN, UnopenedBTNPositionCount);
            }
        }

        private int UnopenedSBPositionCount;

        public override decimal UO_PFR_SB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_SB, UnopenedSBPositionCount);
            }
        }

        private int UnopenedBBPositionCount;

        public override decimal UO_PFR_BB
        {
            get
            {
                return GetPercentage(Source.UO_PFR_BB, UnopenedBBPositionCount);
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

        #endregion

        #region overridden methods

        public override void AddStatistic(Playerstatistic statistic)
        {
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

            if (PlayerStatisticCalculator.IsUnopened(statistic))
            {
                switch (statistic.PositionString)
                {
                    case "EP":
                        UnopenedEPPositionCount++;
                        break;
                    case "MP":
                        UnopenedMPPositionCount++;
                        break;
                    case "CO":
                        UnopenedCOPositionCount++;
                        break;
                    case "BTN":
                        UnopenedBTNPositionCount++;
                        break;
                    case "SB":
                        UnopenedSBPositionCount++;
                        break;
                    case "BB":
                        UnopenedBBPositionCount++;
                        break;
                    default:
                        break;
                }
            }
        }

        public override void UpdateSource(IList<Playerstatistic> statistics)
        {
        }

        #endregion
    }
}