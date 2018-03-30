//-----------------------------------------------------------------------
// <copyright file="BaseReportCreator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Model.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Reports
{
    public abstract class BaseReportCreator<T> : IReportCreator
       where T : ReportIndicators
    {
        public abstract bool IsTournament { get; }

        public virtual ObservableCollection<ReportIndicators> Create(List<Playerstatistic> statistics, bool forceRefresh = false)
        {
            using (var pf = new PerformanceMonitor($"{GetType()}.{nameof(Create)}"))
            {
                var report = new ObservableCollection<ReportIndicators>();

                if (statistics == null || !statistics.Any())
                {
                    return report;
                }

                var chunkSize = GetChunkSize(statistics);

                var splittedStatistic = new Queue<List<Playerstatistic>>(statistics.ToList().SplitList(chunkSize));

                var runningTasks = new List<Task>();

                var chunkedIndicators = new BlockingCollection<T>();

                while (splittedStatistic.Count > 0)
                {
                    while (runningTasks.Count < maxThreads && splittedStatistic.Count > 0)
                    {
                        var statisticChunk = splittedStatistic.Dequeue();

                        runningTasks.Add(Task.Run(() => ProcessChunkedStatistic(statisticChunk, chunkedIndicators)));
                    }

                    var completedTask = Task.WhenAny(runningTasks).Result;

                    runningTasks.Remove(completedTask);
                }

                Task.WhenAll(runningTasks).Wait();

                report.AddRange(OrderResult(CombineChunkedIndicators(chunkedIndicators)));

                return report;
            }
        }

        private static int maxThreads = Environment.ProcessorCount + 1;

        private const int unchunkedSize = 10000;

        protected virtual int GetChunkSize(List<Playerstatistic> statistics)
        {
            return statistics.Count < unchunkedSize ?
                unchunkedSize :
                (statistics.Count + maxThreads - 1) / maxThreads;
        }

        protected abstract List<T> CombineChunkedIndicators(BlockingCollection<T> chunkedIndicators);

        protected abstract void ProcessChunkedStatistic(List<Playerstatistic> statistics, BlockingCollection<T> chunkedIndicators);

        protected virtual IEnumerable<T> OrderResult(IEnumerable<T> reports)
        {
            return reports;
        }
    }
}