//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticPerformanceTest.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Model;
using Model.Data;
using Model.Enums;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class PlayerStatisticPerformanceTest
    {
        //private readonly string statisticPath = StringFormatter.GetPlayerStatisticDataFolderPath();
        private readonly string statisticPath = @"c:\Users\Freeman\AppData\Roaming\DriveHUD\Database-en\";

        // [TestCase(203)]
        public void Test(int userId)
        {
            ResourceRegistrator.Initialization();

            var hudIndicator = new HudIndicators(new List<Stat>());
            var advIndicator = new AdvancedIndicator();
            var advIndicator2 = new AdvancedIndicator();

            var cycles = 1;

            using (var pf = new PerformanceMonitor("Old way"))
            {
                for (var i = 0; i < cycles; i++)
                {
                    var repository = new PlayerStatisticRepository();
                    repository.SetPlayerStatisticPath(statisticPath);
                    repository.GetPlayerStatistic(userId).ForEach(x => hudIndicator.AddStatistic(x));
                }
            }

            //using (var pf = new PerformanceMonitor("New way"))
            //{
            //    for (var i = 0; i < cycles; i++)
            //    {
            //        var repository = new PlayerStatisticRepository();
            //        repository.SetPlayerStatisticPath(statisticPath);
            //        var stats = repository.GetPlayerStatistic(13078).ToList();

            //        Parallel.ForEach(stats, x => advIndicator.ProcessStatistic(x));
            //    }
            //}

            //using (var pf = new PerformanceMonitor("New way2"))
            //{
            //    for (var i = 0; i < cycles; i++)
            //    {
            //        var repository = new PlayerStatisticRepository();
            //        repository.SetPlayerStatisticPath(statisticPath);
            //        repository.GetPlayerStatistic(13078).ForEach(x => advIndicator2.ProcessStatistic(x));
            //    }
            //}
        }
    }

    class AdvancedIndicator
    {
        public List<BaseStat> Stats = new List<BaseStat>
        {
            new NetWonStat(),
             new NetWonStat(),
              new NetWonStat(),
               new NetWonStat(),
                new NetWonStat(),
                 new NetWonStat(),
                  new NetWonStat(),
                   new NetWonStat(),
                    new NetWonStat(),
                     new NetWonStat(),
            new TotalHandsStat(),
             new TotalHandsStat(),
              new TotalHandsStat(),
               new TotalHandsStat(),
                new TotalHandsStat(),
                 new TotalHandsStat(),
                  new TotalHandsStat(),
                   new TotalHandsStat(),
                    new TotalHandsStat(),
                     new TotalHandsStat(),

        };

        public void ProcessStatistic(Playerstatistic playerstatistic)
        {
            Stats.ForEach(x => x.ProcessStatistic(playerstatistic));
        }
    }

    abstract class BaseStat
    {
        public abstract object Value { get; }

        public abstract void ProcessStatistic(Playerstatistic playerstatistic);
    }

    class NetWonStat : BaseStat
    {
        private double netWon;

        public override object Value => netWon;

        public override void ProcessStatistic(Playerstatistic playerstatistic)
        {
            double totalWinning = 0;
            double netWon = 0;

            do
            {
                netWon = this.netWon;
                totalWinning = netWon + (double)playerstatistic.NetWon;
            }
            while (netWon != Interlocked.CompareExchange(ref this.netWon, totalWinning, netWon));
        }
    }

    class TotalHandsStat : BaseStat
    {
        private int totalHands;

        public override object Value => totalHands;

        public override void ProcessStatistic(Playerstatistic playerstatistic)
        {
            Interlocked.Increment(ref totalHands);
        }
    }
}