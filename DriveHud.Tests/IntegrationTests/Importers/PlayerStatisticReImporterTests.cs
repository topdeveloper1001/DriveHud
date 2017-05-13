//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticReImporterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Common.Extensions;
using DriveHUD.Entities;
using DriveHUD.Importers;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Data;
using Model.Interfaces;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    [TestFixture]
    class PlayerStatisticReImporterTests : BaseDatabaseTest
    {
        private static string databaseFile = "drivehud-reimporter.db";
        private const string playerStatisticFolder = "Database";
        private const string playerStatisticTempFolder = "Database-temp";
        private const string playerStatisticBackup = "Database-backup";
        private const string playerStatisticOld = "Database-old";

        private static string databaseZipFile;
        private static string playerStatisticZipFile;

        protected override void InitializeDatabase()
        {
        }

        /// <summary>
        /// Initializes environment for test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            using (var perfScope = new PerformanceMonitor("PlayerStatisticReImporterTests.SetUp"))
            {
                Initalize();
                Clear();
            }
        }

        /// <summary>
        /// Clears environment after tests
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Clear();
        }

        private void Clear()
        {
            using (var perfScope = new PerformanceMonitor("PlayerStatisticReImporterTests.Clear"))
            {
                if (File.Exists(databaseFile))
                {
                    File.Delete(databaseFile);
                }

                if (Directory.Exists(playerStatisticFolder))
                {
                    Directory.Delete(playerStatisticFolder, true);
                }

                if (Directory.Exists(playerStatisticTempFolder))
                {
                    Directory.Delete(playerStatisticTempFolder, true);
                }

                if (Directory.Exists(playerStatisticBackup))
                {
                    Directory.Delete(playerStatisticBackup, true);
                }
            }
        }

        [Test]
        public void Test()
        {
            databaseFile = @"c:\Users\Freeman\AppData\Roaming\DriveHUD\drivehud.db";

            var dataService = ServiceLocator.Current.GetInstance<IDataService>();

            var stats = dataService.GetPlayerStatisticFromFile("mhdishere", 7);

            var tournamentStats = stats.Where(x => x.Time >= DateTime.Now.FirstDayOfMonth() && x.IsTourney).ToArray();

            Assert.That(tournamentStats.Length, Is.GreaterThan(0));

            databaseFile = "drivehud-importer.db";
        }

        //[Test]      
        //[TestCase("drivehud.zip", "Database.zip", "DURKADURDUR", EnumPokerSites.PokerStars)]
        public void ReImportTest(string databaseZipFile, string playerStatisticZipFile, string playerName, EnumPokerSites pokerSite)
        {
            PlayerStatisticReImporterTests.databaseZipFile = databaseZipFile;
            PlayerStatisticReImporterTests.playerStatisticZipFile = playerStatisticZipFile;

            UnzipTestData();

            using (var perfScope = new PerformanceMonitor("ReImportTest.ReImport"))
            {
                var playerStatisticReImporter = new PlayerStatisticTestReImporter();
                playerStatisticReImporter.ReImport();
            }

            using (var assertsPerfScope = new PerformanceMonitor("ReImportTest.Asserts"))
            {
                var expectedIndicators = GetIndicators(playerName, pokerSite, playerStatisticBackup);

                GC.Collect();

                var actualIndicators = GetIndicators(playerName, pokerSite, playerStatisticFolder);

                GC.Collect();

                AssertThatIndicatorsAreEqual(actualIndicators, expectedIndicators);
            }
        }

        /// <summary>
        /// Gets indicators for the specified player, poker site and player statistic folder
        /// </summary>
        /// <param name="playerName"></param>
        /// <param name="pokerSite"></param>
        /// <param name="playerStatisticFolder"></param>
        /// <returns></returns>
        private HudLightIndicators GetIndicators(string playerName, EnumPokerSites pokerSite, string playerStatisticFolder)
        {
            var dataService = ServiceLocator.Current.GetInstance<IDataService>();
            dataService.SetPlayerStatisticPath(playerStatisticFolder);

            var playerStatistic = dataService.GetPlayerStatisticFromFile(playerName, (short)pokerSite);

            var indicators = new HudLightIndicators(playerStatistic);

            return indicators;
        }

        /// <summary>
        /// Asserts that the specified indicators are equal
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        private void AssertThatIndicatorsAreEqual(HudLightIndicators actual, HudLightIndicators expected)
        {
            Assert.IsNotNull(actual, "Actual indicator must be not null");
            Assert.IsNotNull(expected, "Expected indicator must be not null");

            Assert.That(actual.BB, Is.EqualTo(expected.BB));
            Assert.That(actual.VPIP, Is.EqualTo(expected.VPIP));
            Assert.That(actual.PFR, Is.EqualTo(expected.PFR));
            Assert.That(actual.ThreeBet, Is.EqualTo(expected.ThreeBet));
            Assert.That(actual.ThreeBetIP, Is.EqualTo(expected.ThreeBetIP));
            Assert.That(actual.ThreeBetOOP, Is.EqualTo(expected.ThreeBetOOP));
            Assert.That(actual.FourBet, Is.EqualTo(expected.FourBet));
            Assert.That(actual.ThreeBetCall, Is.EqualTo(expected.ThreeBetCall));
            Assert.That(actual.FlopCBet, Is.EqualTo(expected.FlopCBet));
            Assert.That(actual.TurnCBet, Is.EqualTo(expected.TurnCBet));
            Assert.That(actual.FlopCBetInThreeBetPot, Is.EqualTo(expected.FlopCBetInThreeBetPot));
            Assert.That(actual.FlopCBetInFourBetPot, Is.EqualTo(expected.FlopCBetInFourBetPot));
            Assert.That(actual.FlopCBetVsOneOpp, Is.EqualTo(expected.FlopCBetVsOneOpp));
            Assert.That(actual.FlopCBetVsTwoOpp, Is.EqualTo(expected.FlopCBetVsTwoOpp));
            Assert.That(actual.FlopCBetMW, Is.EqualTo(expected.FlopCBetMW));
            Assert.That(actual.FlopCBetMonotone, Is.EqualTo(expected.FlopCBetMonotone));
            Assert.That(actual.FlopCBetRag, Is.EqualTo(expected.FlopCBetRag));
            Assert.That(actual.FoldFlopCBetFromThreeBetPot, Is.EqualTo(expected.FoldFlopCBetFromThreeBetPot));
            Assert.That(actual.FoldFlopCBetFromFourBetPot, Is.EqualTo(expected.FoldFlopCBetFromFourBetPot));
            Assert.That(actual.Steal, Is.EqualTo(expected.Steal));
            Assert.That(actual.FoldToThreeBet, Is.EqualTo(expected.FoldToThreeBet));
            Assert.That(actual.BlindsReraiseSteal, Is.EqualTo(expected.BlindsReraiseSteal));
            Assert.That(actual.WSSD, Is.EqualTo(expected.WSSD));
            Assert.That(actual.WWSF, Is.EqualTo(expected.WWSF));
            Assert.That(actual.WTSD, Is.EqualTo(expected.WTSD));
            Assert.That(actual.Agg, Is.EqualTo(expected.Agg));
            Assert.That(actual.AggPr, Is.EqualTo(expected.AggPr));
            Assert.That(actual.TrueAggression, Is.EqualTo(expected.TrueAggression));
            Assert.That(actual.WSWSF, Is.EqualTo(expected.WSWSF));
            Assert.That(actual.TotalHands, Is.EqualTo(expected.TotalHands));
            Assert.That(actual.TotalWon, Is.EqualTo(expected.TotalWon));
            Assert.That(actual.ColdCall, Is.EqualTo(expected.ColdCall));
            Assert.That(actual.FlopAgg, Is.EqualTo(expected.FlopAgg));
            Assert.That(actual.TurnAgg, Is.EqualTo(expected.TurnAgg));
            Assert.That(actual.RiverAgg, Is.EqualTo(expected.RiverAgg));
            Assert.That(actual.FoldCBet, Is.EqualTo(expected.FoldCBet));
            Assert.That(actual.RaiseCBet, Is.EqualTo(expected.RaiseCBet));
            Assert.That(actual.FoldToFourBet, Is.EqualTo(expected.FoldToFourBet));
            Assert.That(actual.Squeeze, Is.EqualTo(expected.Squeeze));
            Assert.That(actual.CheckRaise, Is.EqualTo(expected.CheckRaise));
            Assert.That(actual.FlopCheckRaise, Is.EqualTo(expected.FlopCheckRaise));
            Assert.That(actual.TurnCheckRaise, Is.EqualTo(expected.TurnCheckRaise));
            Assert.That(actual.RiverCheckRaise, Is.EqualTo(expected.RiverCheckRaise));
            Assert.That(actual.FloatFlop, Is.EqualTo(expected.FloatFlop));
            Assert.That(actual.RaiseFlop, Is.EqualTo(expected.RaiseFlop));
            Assert.That(actual.RaiseTurn, Is.EqualTo(expected.RaiseTurn));
            Assert.That(actual.RaiseRiver, Is.EqualTo(expected.RaiseRiver));
            Assert.That(actual.RaiseFrequencyFactor, Is.EqualTo(expected.RaiseFrequencyFactor));
            Assert.That(actual.TurnSeen, Is.EqualTo(expected.TurnSeen));
            Assert.That(actual.RiverSeen, Is.EqualTo(expected.RiverSeen));
            Assert.That(actual.ThreeBetVsSteal, Is.EqualTo(expected.ThreeBetVsSteal));
            Assert.That(actual.CheckRiverOnBXLine, Is.EqualTo(expected.CheckRiverOnBXLine));
            Assert.That(actual.CBetIP, Is.EqualTo(expected.CBetIP));
            Assert.That(actual.CBetOOP, Is.EqualTo(expected.CBetOOP));
            Assert.That(actual.ThreeBetInBB, Is.EqualTo(expected.ThreeBetInBB));
            Assert.That(actual.ThreeBetInBTN, Is.EqualTo(expected.ThreeBetInBTN));
            Assert.That(actual.ThreeBetInCO, Is.EqualTo(expected.ThreeBetInCO));
            Assert.That(actual.ThreeBetInMP, Is.EqualTo(expected.ThreeBetInMP));
            Assert.That(actual.ThreeBetInSB, Is.EqualTo(expected.ThreeBetInSB));
            Assert.That(actual.FourBetInBB, Is.EqualTo(expected.FourBetInBB));
            Assert.That(actual.FourBetInBTN, Is.EqualTo(expected.FourBetInBTN));
            Assert.That(actual.FourBetInCO, Is.EqualTo(expected.FourBetInCO));
            Assert.That(actual.FourBetInMP, Is.EqualTo(expected.FourBetInMP));
            Assert.That(actual.FourBetInSB, Is.EqualTo(expected.FourBetInSB));
            Assert.That(actual.ColdCallInBB, Is.EqualTo(expected.ColdCallInBB));
            Assert.That(actual.ColdCallInBTN, Is.EqualTo(expected.ColdCallInBTN));
            Assert.That(actual.ColdCallInCO, Is.EqualTo(expected.ColdCallInCO));
            Assert.That(actual.ColdCallInMP, Is.EqualTo(expected.ColdCallInMP));
            Assert.That(actual.ColdCallInSB, Is.EqualTo(expected.ColdCallInSB));
            Assert.That(actual.DonkBet, Is.EqualTo(expected.DonkBet));
            Assert.That(actual.DidDelayedTurnCBet, Is.EqualTo(expected.DidDelayedTurnCBet));
            Assert.That(actual.MRatio, Is.EqualTo(expected.MRatio));
            Assert.That(actual.StackInBBs, Is.EqualTo(expected.StackInBBs));
            Assert.That(actual.UO_PFR_EP, Is.EqualTo(expected.UO_PFR_EP));
            Assert.That(actual.UO_PFR_MP, Is.EqualTo(expected.UO_PFR_MP));
            Assert.That(actual.UO_PFR_CO, Is.EqualTo(expected.UO_PFR_CO));
            Assert.That(actual.UO_PFR_BN, Is.EqualTo(expected.UO_PFR_BN));
            Assert.That(actual.UO_PFR_SB, Is.EqualTo(expected.UO_PFR_SB));
            Assert.That(actual.UO_PFR_BB, Is.EqualTo(expected.UO_PFR_BB));
            Assert.That(actual.PFRInEP, Is.EqualTo(expected.PFRInEP));
            Assert.That(actual.PFRInMP, Is.EqualTo(expected.PFRInMP));
            Assert.That(actual.PFRInCO, Is.EqualTo(expected.PFRInCO));
            Assert.That(actual.PFRInBTN, Is.EqualTo(expected.PFRInBTN));
            Assert.That(actual.PFRInBB, Is.EqualTo(expected.PFRInBB));
            Assert.That(actual.PFRInSB, Is.EqualTo(expected.PFRInSB));
            Assert.That(actual.DidLimp, Is.EqualTo(expected.DidLimp));
            Assert.That(actual.DidLimpCall, Is.EqualTo(expected.DidLimpCall));
            Assert.That(actual.DidLimpFold, Is.EqualTo(expected.DidLimpFold));
            Assert.That(actual.DidLimpReraise, Is.EqualTo(expected.DidLimpReraise));
            Assert.That(actual.CheckFoldFlopPfrOop, Is.EqualTo(expected.CheckFoldFlopPfrOop));
            Assert.That(actual.CheckFoldFlop3BetOop, Is.EqualTo(expected.CheckFoldFlop3BetOop));
            Assert.That(actual.BetFoldFlopPfrRaiser, Is.EqualTo(expected.BetFoldFlopPfrRaiser));
            Assert.That(actual.BetFlopCalled3BetPreflopIp, Is.EqualTo(expected.BetFlopCalled3BetPreflopIp));
            Assert.That(actual.BTNDefendCORaise, Is.EqualTo(expected.BTNDefendCORaise));
            Assert.That(actual.EVDiff, Is.EqualTo(expected.EVDiff));
            Assert.That(actual.EVBB, Is.EqualTo(expected.EVBB));
            Assert.That(actual.BetWhenCheckedTo, Is.EqualTo(expected.BetWhenCheckedTo));
            Assert.That(actual.FoldToFlopRaise, Is.EqualTo(expected.FoldToFlopRaise));
            Assert.That(actual.FoldToTurnRaise, Is.EqualTo(expected.FoldToTurnRaise));
            Assert.That(actual.FoldToRiverCBet, Is.EqualTo(expected.FoldToRiverCBet));
            Assert.That(actual.FoldToSqueez, Is.EqualTo(expected.FoldToSqueez));
            Assert.That(actual.VPIP_EP, Is.EqualTo(expected.VPIP_EP));
            Assert.That(actual.VPIP_MP, Is.EqualTo(expected.VPIP_MP));
            Assert.That(actual.VPIP_CO, Is.EqualTo(expected.VPIP_CO));
            Assert.That(actual.VPIP_BN, Is.EqualTo(expected.VPIP_BN));
            Assert.That(actual.VPIP_SB, Is.EqualTo(expected.VPIP_SB));
            Assert.That(actual.VPIP_BB, Is.EqualTo(expected.VPIP_BB));
            Assert.That(actual.ColdCall_EP, Is.EqualTo(expected.ColdCall_EP));
            Assert.That(actual.ColdCall_MP, Is.EqualTo(expected.ColdCall_MP));
            Assert.That(actual.ColdCall_CO, Is.EqualTo(expected.ColdCall_CO));
            Assert.That(actual.ColdCall_BN, Is.EqualTo(expected.ColdCall_BN));
            Assert.That(actual.ColdCall_SB, Is.EqualTo(expected.ColdCall_SB));
            Assert.That(actual.ColdCall_BB, Is.EqualTo(expected.ColdCall_BB));
            Assert.That(actual.ThreeBet_EP, Is.EqualTo(expected.ThreeBet_EP));
            Assert.That(actual.ThreeBet_MP, Is.EqualTo(expected.ThreeBet_MP));
            Assert.That(actual.ThreeBet_CO, Is.EqualTo(expected.ThreeBet_CO));
            Assert.That(actual.ThreeBet_BN, Is.EqualTo(expected.ThreeBet_BN));
            Assert.That(actual.ThreeBet_SB, Is.EqualTo(expected.ThreeBet_SB));
            Assert.That(actual.ThreeBet_BB, Is.EqualTo(expected.ThreeBet_BB));
        }

        private void UnzipTestData()
        {
            using (var perfScope = new PerformanceMonitor("ReImportTest.UnzipTestData"))
            {
                ZipFile.ExtractToDirectory(GetDataBaseZipPath(), "./");
                ZipFile.ExtractToDirectory(GetPlayerStatisticZipPath(), "./");
            }
        }

        protected override void InitializeDataService(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IDataService, DataService>(new ContainerControlledLifetimeManager());
        }

        protected override void InitializeSessionFactoryService(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<ISessionFactoryService, ReImporterSessionFactoryTestService>(new ContainerControlledLifetimeManager());
        }

        /// <summary>
        /// Gets the path to zip archive with database
        /// </summary>
        /// <returns>The path to zip archive with database</returns>
        private string GetDataBaseZipPath()
        {
            var path = Path.Combine(TestDataFolder, "ReImporterTestData", databaseZipFile);
            return path;
        }

        /// <summary>
        /// Gets the path to zip archive with player statistic
        /// </summary>
        /// <returns>The path to zip archive with player statistic</returns>
        private string GetPlayerStatisticZipPath()
        {
            var path = Path.Combine(TestDataFolder, "ReImporterTestData", playerStatisticZipFile);
            return path;
        }

        /// <summary>
        /// Determines connection to test database
        /// </summary>
        private class ReImporterSessionFactoryTestService : SessionFactoryTestService
        {
            public override string GetConnectionString()
            {
                return $"Data Source={databaseFile};Version=3;foreign keys=true;";
            }
        }

        private class PlayerStatisticTestReImporter : PlayerStatisticReImporter
        {
            public PlayerStatisticTestReImporter() : base(playerStatisticFolder,
                playerStatisticTempFolder, playerStatisticBackup, playerStatisticOld)
            {
            }
        }
    }
}