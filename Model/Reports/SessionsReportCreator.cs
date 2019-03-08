//-----------------------------------------------------------------------
// <copyright file="SessionsReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Model.Reports
{
    public class SessionsReportCreator : CashBaseReportCreator<SessionReportIndicators>
    {
        private static TimeSpan sessionInterval = TimeSpan.FromMinutes(30);

        protected override List<SessionReportIndicators> CombineChunkedIndicators(BlockingCollection<SessionReportIndicators> chunkedIndicators, CancellationToken cancellationToken)
        {           
            var reports = new List<SessionReportIndicators>();

            SessionReportIndicators report = null;

            foreach (var chunkedIndicator in chunkedIndicators.OrderByDescending(x => x.SessionEnd))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return reports;
                }

                if (report != null && chunkedIndicator.SessionEnd.HasValue &&
                    Utils.IsDateInDateRange(chunkedIndicator.SessionEnd.Value, report.SessionStart, report.SessionEnd, sessionInterval))
                {
                    report.AddIndicator(chunkedIndicator);

                    continue;
                }

                report = chunkedIndicator;
                reports.Add(report);
            }

            return reports;
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<SessionReportIndicators> chunkedIndicators, CancellationToken cancellationToken)
        {
            SessionReportIndicators report = null;

            foreach (var playerstatistic in statistics.OrderByDescending(x => x.Time).ToArray())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (report != null &&
                    Utils.IsDateInDateRange(playerstatistic.Time, report.SessionStart, report.SessionEnd, sessionInterval))
                {
                    if (!report.GameTypes.Contains(playerstatistic.GameType))
                    {
                        report.GameTypes.Add(playerstatistic.GameType);
                    }

                    report.AddStatistic(playerstatistic);
                    continue;
                }

                report = new SessionReportIndicators();
                report.AddStatistic(playerstatistic);
                report.GameTypes.Add(playerstatistic.GameType);

                chunkedIndicators.Add(report);
            }
        }

        protected override IEnumerable<SessionReportIndicators> OrderResult(IEnumerable<SessionReportIndicators> reports)
        {
            return reports.OrderByDescending(x => x.SessionStart);
        }
    }
}