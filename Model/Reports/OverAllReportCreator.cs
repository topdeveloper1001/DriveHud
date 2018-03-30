//-----------------------------------------------------------------------
// <copyright file="OverAllReportCreator.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Model.Data;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Model.Reports
{
    /// <summary>
    /// Report will all basic indicators. No grouping
    /// </summary>
    public class OverAllReportCreator : CashBaseReportCreator<ReportIndicators>
    {
        protected override List<ReportIndicators> CombineChunkedIndicators(BlockingCollection<ReportIndicators> chunkedIndicators)
        {
            var report = chunkedIndicators.FirstOrDefault();

            if (report == null)
            {
                return new List<ReportIndicators>();
            }

            chunkedIndicators.Skip(1).ForEach(r => report.AddIndicator(r));

            return new List<ReportIndicators>
            {
                report
            };
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<ReportIndicators> chunkedIndicators)
        {
            var report = new ReportIndicators();

            chunkedIndicators.Add(report);

            foreach (var playerstatistic in statistics)
            {
                report.AddStatistic(playerstatistic);
            }
        }
    }    
}