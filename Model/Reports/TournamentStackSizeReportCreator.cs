//-----------------------------------------------------------------------
// <copyright file="TournamentStackSizeReportCreator.cs" company="Ace Poker Solutions">
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
using Model.Data;
using Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Model.Reports
{
    public class TournamentStackSizeReportCreator : TournamentGroupingReportCreator<MRatioReportRecord, EnumMRatio>
    {
        protected override MRatioReportRecord CreateIndicator(EnumMRatio groupKey)
        {
            var report = new MRatioReportRecord
            {
                MRatioZone = groupKey
            };

            return report;
        }

        protected override EnumMRatio GroupBy(Playerstatistic statistic)
        {
            return GetMRatio(statistic);
        }

        protected override EnumMRatio GroupBy(MRatioReportRecord indicator)
        {
            return indicator.MRatioZone;
        }

        protected override IEnumerable<MRatioReportRecord> OrderResult(IEnumerable<MRatioReportRecord> reports)
        {
            return reports.OrderBy(x => x.MRatioZone);
        }

        private EnumMRatio GetMRatio(Playerstatistic stat)
        {
            var mRatioValue = PlayerStatisticCalculator.CalculateMRatio(stat);

            EnumMRatio mRatio;

            if (mRatioValue <= 5)
            {
                mRatio = EnumMRatio.RedZone;
            }
            else if (mRatioValue < 10)
            {
                mRatio = EnumMRatio.OrangeZone;
            }
            else if (mRatioValue < 20)
            {
                mRatio = EnumMRatio.YellowZone;
            }
            else if (mRatioValue < 40)
            {
                mRatio = EnumMRatio.GreenZone;
            }
            else if (mRatioValue < 60)
            {
                mRatio = EnumMRatio.BlueZone;
            }
            else
            {
                mRatio = EnumMRatio.PurpleZone;
            }

            return mRatio;
        }
    }
}