//-----------------------------------------------------------------------
// <copyright file="GroupingReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker CashGroupingReportCreator. All Rights Reserved.
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
using System.Threading;

namespace Model.Reports
{
    public abstract class GroupingReportCreator<T, GroupType> : BaseReportCreator<T>
         where T : ReportIndicators
    {
        protected abstract GroupType GroupBy(Playerstatistic statistic);

        protected abstract GroupType GroupBy(T indicator);

        protected override List<T> CombineChunkedIndicators(BlockingCollection<T> chunkedIndicators, CancellationToken cancellationToken)
        {
            var reports = new List<T>();

            foreach (var chunkedIndicatorsByCards in chunkedIndicators.GroupBy(GroupBy))
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var report = chunkedIndicatorsByCards.First();

                chunkedIndicatorsByCards.Skip(1).ForEach(r =>
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    report.AddIndicator(r);
                });

                reports.Add(report);
            }

            return reports;
        }

        protected override void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<T> chunkedIndicators, CancellationToken cancellationToken)
        {
            foreach (var statisticsByCards in statistics.GroupBy(GroupBy).ToArray())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                var report = CreateIndicator(statisticsByCards.Key);

                chunkedIndicators.Add(report);

                foreach (var playerstatistic in statisticsByCards)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    report.AddStatistic(playerstatistic);
                }
            }
        }

        protected abstract T CreateIndicator(GroupType groupKey);
    }
}