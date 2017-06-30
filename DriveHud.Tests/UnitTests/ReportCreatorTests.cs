//-----------------------------------------------------------------------
// <copyright file="ReportCreatorTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Common.Log;
using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Common.Linq;
using DriveHUD.Entities;
using Model.Reports;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class ReportCreatorTests : BaseIndicatorsTests
    {
        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
            LoadTestPlayerStatistic();
        }

        private Dictionary<string, IList<Playerstatistic>> playerStatisticDictionary;

        [Test]
        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestSessionReportCreator(string file)
        {
            using (var perfScope = new PerformanceMonitor("ReportCreatorTests.TestSessionReportCreator"))
            {
                var playerStatistic = playerStatisticDictionary[file];

                var sessionsReportCreator = new SessionsReportCreator();

                var report = sessionsReportCreator
                    .Create(playerStatistic)
                    .OrderBy(x => x.SessionStart)
                    .ToArray();

                var expectedData = SessionExpectedDataSet[file]
                    .OrderBy(x => x.SessionStart)
                    .ToArray();

                Assert.That(report.Length, Is.EqualTo(expectedData.Length), "Results length doesn't match expected");

                Assert.Multiple(() =>
                {
                    var tolerance = 0.01m;

                    for (var i = 0; i < report.Length; i++)
                    {
                        Console.WriteLine(report[i]);

                        Assert.That(report[i].SessionStart, Is.EqualTo(expectedData[i].SessionStart), $"SessionStart[{i}]");
                        Assert.That(report[i].StatisticsCount, Is.EqualTo(expectedData[i].StatisticsCount), $"StatisticsCount[{i}]");
                        Assert.That(report[i].BB, Is.EqualTo(expectedData[i].BB).Within(tolerance), $"BB[{i}]");
                        Assert.That(report[i].SessionLength, Is.EqualTo(expectedData[i].SessionLength), $"SessionLength[{i}]");
                        Assert.That(report[i].VPIP, Is.EqualTo(expectedData[i].VPIP).Within(tolerance), $"VPIP[{i}]");
                        Assert.That(report[i].PFR, Is.EqualTo(expectedData[i].PFR).Within(tolerance), $"PFR[{i}]");
                        Assert.That(report[i].ThreeBet, Is.EqualTo(expectedData[i].ThreeBet).Within(tolerance), $"ThreeBet[{i}]");
                        Assert.That(report[i].TotalWon, Is.EqualTo(expectedData[i].TotalWon).Within(tolerance), $"TotalWon[{i}]");
                        Assert.That(report[i].WSSD, Is.EqualTo(expectedData[i].WSSD).Within(tolerance), $"WSSD[{i}]");
                        Assert.That(report[i].WWSF, Is.EqualTo(expectedData[i].WWSF).Within(tolerance), $"WWSF[{i}]");
                        Assert.That(report[i].WTSD, Is.EqualTo(expectedData[i].WTSD).Within(tolerance), $"WTSD[{i}]");
                    }
                });
            }
        }

        /// <summary>
        /// Loads test player statistics data
        /// </summary>
        private void LoadTestPlayerStatistic()
        {
            using (var perfScope = new PerformanceMonitor("ReportCreatorTests.LoadTestPlayerStatistic"))
            {
                playerStatisticDictionary = new Dictionary<string, IList<Playerstatistic>>();

                PlayerStatisticsFiles.ForEach(file =>
                {
                    playerStatisticDictionary.Add(file, DataServiceHelper.GetPlayerStatisticFromFile(file));
                });
            }
        }

        private readonly string[] PlayerStatisticsFiles = new[]
        {
            @"..\..\UnitTests\TestData\testdata.stat"
        };

        #region Session report expected data

        private readonly Dictionary<string, ExpectedIndicatorObject[]> SessionExpectedDataSet = new Dictionary<string, ExpectedIndicatorObject[]>
        {
            [@"..\..\UnitTests\TestData\testdata.stat"] = new[]
            {
                // 0
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 3, 14, 35, 42),
                    StatisticsCount = 1069,
                    BB = 4.61m,
                    SessionLength = "2:26",
                    VPIP = 23.85m,
                    PFR = 22.83m,
                    ThreeBet = 7.67m,
                    TotalWon = 98.65m,
                    WSSD = 66.67m,
                    WWSF = 42.42m,
                    WTSD = 22.73m
                },
                // 1
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 4, 0, 18, 25),
                    StatisticsCount = 646,
                    BB = -18.08m,
                    SessionLength = "1:41",
                    VPIP = 22.6m,
                    PFR = 21.67m,
                    ThreeBet = 8.76m,
                    TotalWon = -233.6m,
                    WSSD = 54.55m,
                    WWSF = 42.37m,
                    WTSD = 18.64m
                },
                // 2
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 4, 4, 18, 39),
                    StatisticsCount = 180,
                    BB = 28.24m,
                    SessionLength = "0:22",
                    VPIP = 27.22m,
                    PFR = 26.67m,
                    ThreeBet = 12.28m,
                    TotalWon = 101.65m,
                    WSSD = 75m,
                    WWSF = 31.82m,
                    WTSD = 18.18m
                },
                // 3
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 4, 12, 56, 48),
                    StatisticsCount = 705,
                    BB = 10.29m,
                    SessionLength = "3:31",
                    VPIP = 23.26m,
                    PFR = 22.7m,
                    ThreeBet = 11.21m,
                    TotalWon = 145.05m,
                    WSSD = 48.28m,
                    WWSF = 42.11m,
                    WTSD = 38.16m
                },
                // 4
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 4, 19, 58, 36),
                    StatisticsCount = 163,
                    BB = 26.38m,
                    SessionLength = "1:37",
                    VPIP = 22.7m,
                    PFR = 21.47m,
                    ThreeBet = 17.31m,
                    TotalWon = 86m,
                    WSSD = 66.67m,
                    WWSF = 42.86m,
                    WTSD = 42.86m
                },
                // 5
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 5, 3, 27, 4),
                    StatisticsCount = 456,
                    BB = -16.94m,
                    SessionLength = "1:25",
                    VPIP = 22.37m,
                    PFR = 21.27m,
                    ThreeBet = 8.67m,
                    TotalWon = -154.45m,
                    WSSD = 54.54m,
                    WWSF = 30.43m,
                    WTSD = 23.91m
                },
                // 6
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 5, 13, 48, 58),
                    StatisticsCount = 555,
                    BB = 9.61m,
                    SessionLength = "0:48",
                    VPIP = 26.49m,
                    PFR = 25.23m,
                    ThreeBet = 11.8m,
                    TotalWon = 106.7m,
                    WSSD = 50m,
                    WWSF = 35.71m,
                    WTSD = 25m
                },
                // 7
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 21, 4, 0, 12),
                    StatisticsCount = 74,
                    BB = 23.38m,
                    SessionLength = "0:19",
                    VPIP = 10.81m,
                    PFR = 10.81m,
                    ThreeBet = 6.45m,
                    TotalWon = 34.6m,
                    WSSD = 0m,
                    WWSF = 50m,
                    WTSD = 0m
                },
                // 8
                new ExpectedIndicatorObject
                {
                    SessionStart = new DateTime(2010, 7, 23, 4, 56, 55),
                    StatisticsCount = 10,
                    BB = 20m,
                    SessionLength = "0:02",
                    VPIP = 20m,
                    PFR = 20m,
                    ThreeBet = 0m,
                    TotalWon = 4m,
                    WSSD = 0m,
                    WWSF = 0m,
                    WTSD = 0m
                }
            }
        };

        #endregion

        #region Test objects

        private class ExpectedIndicatorObject
        {
            public DateTime? SessionStart { get; set; }

            public int StatisticsCount { get; set; }

            public decimal BB { get; set; }

            public string SessionLength { get; set; }

            public decimal VPIP { get; set; }

            public decimal PFR { get; set; }

            public decimal ThreeBet { get; set; }

            public decimal TotalWon { get; set; }

            public decimal WSSD { get; set; }

            public decimal WWSF { get; set; }

            public decimal WTSD { get; set; }
        }

        #endregion
    }
}