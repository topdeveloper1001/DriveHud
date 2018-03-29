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
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Data;
using Model.Interfaces;
using NUnit.Framework;
using System;
using System.IO;
using System.IO.Compression;

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
        [TestCase("drivehud.zip", "Database.zip", "DURKADURDUR", EnumPokerSites.PokerStars)]
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
            var playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
            playerStatisticRepository.SetPlayerStatisticPath(playerStatisticFolder);

            var playerStatistic = playerStatisticRepository.GetPlayerStatistic(playerName, (short)pokerSite);

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

            Assert.Multiple(() =>
            {
                Assert.That(actual.BB, Is.EqualTo(expected.BB), nameof(HudLightIndicators.BB));
                Assert.That(actual.VPIP, Is.EqualTo(expected.VPIP), nameof(HudLightIndicators.VPIP));
                Assert.That(actual.PFR, Is.EqualTo(expected.PFR), nameof(HudLightIndicators.PFR));
                Assert.That(actual.ThreeBet, Is.EqualTo(expected.ThreeBet), nameof(HudLightIndicators.ThreeBet));
                Assert.That(actual.ThreeBetIP, Is.EqualTo(expected.ThreeBetIP), nameof(HudLightIndicators.ThreeBetIP));
                Assert.That(actual.ThreeBetOOP, Is.EqualTo(expected.ThreeBetOOP), nameof(HudLightIndicators.ThreeBetOOP));
                Assert.That(actual.FourBet, Is.EqualTo(expected.FourBet), nameof(HudLightIndicators.FourBet));
                Assert.That(actual.ThreeBetCall, Is.EqualTo(expected.ThreeBetCall), nameof(HudLightIndicators.ThreeBetCall));
                Assert.That(actual.FlopCBet, Is.EqualTo(expected.FlopCBet), nameof(HudLightIndicators.FlopCBet));
                Assert.That(actual.TurnCBet, Is.EqualTo(expected.TurnCBet), nameof(HudLightIndicators.TurnCBet));
                Assert.That(actual.FoldToTurnCBet, Is.EqualTo(expected.FoldToTurnCBet), nameof(HudLightIndicators.FoldToTurnCBet));
                Assert.That(actual.FlopCBetInThreeBetPot, Is.EqualTo(expected.FlopCBetInThreeBetPot), nameof(HudLightIndicators.FlopCBetInThreeBetPot));
                Assert.That(actual.FlopCBetInFourBetPot, Is.EqualTo(expected.FlopCBetInFourBetPot), nameof(HudLightIndicators.FlopCBetInFourBetPot));
                Assert.That(actual.FlopCBetVsOneOpp, Is.EqualTo(expected.FlopCBetVsOneOpp), nameof(HudLightIndicators.FlopCBetVsOneOpp));
                Assert.That(actual.FlopCBetVsTwoOpp, Is.EqualTo(expected.FlopCBetVsTwoOpp), nameof(HudLightIndicators.FlopCBetVsTwoOpp));
                Assert.That(actual.FlopCBetMW, Is.EqualTo(expected.FlopCBetMW), nameof(HudLightIndicators.FlopCBetMW));
                Assert.That(actual.FlopCBetMonotone, Is.EqualTo(expected.FlopCBetMonotone), nameof(HudLightIndicators.FlopCBetMonotone));
                Assert.That(actual.FlopCBetRag, Is.EqualTo(expected.FlopCBetRag), nameof(HudLightIndicators.FlopCBetRag));
                Assert.That(actual.FoldFlopCBetFromThreeBetPot, Is.EqualTo(expected.FoldFlopCBetFromThreeBetPot), nameof(HudLightIndicators.FoldFlopCBetFromThreeBetPot));
                Assert.That(actual.FoldFlopCBetFromFourBetPot, Is.EqualTo(expected.FoldFlopCBetFromFourBetPot), nameof(HudLightIndicators.FoldFlopCBetFromFourBetPot));
                Assert.That(actual.Steal, Is.EqualTo(expected.Steal), nameof(HudLightIndicators.Steal));
                Assert.That(actual.FoldToThreeBet, Is.EqualTo(expected.FoldToThreeBet), nameof(HudLightIndicators.FoldToThreeBet));
                Assert.That(actual.BlindsReraiseSteal, Is.EqualTo(expected.BlindsReraiseSteal), nameof(HudLightIndicators.BlindsReraiseSteal));
                Assert.That(actual.WSSD, Is.EqualTo(expected.WSSD), nameof(HudLightIndicators.WSSD));
                Assert.That(actual.WWSF, Is.EqualTo(expected.WWSF), nameof(HudLightIndicators.WWSF));
                Assert.That(actual.WTSD, Is.EqualTo(expected.WTSD), nameof(HudLightIndicators.WTSD));
                Assert.That(actual.Agg, Is.EqualTo(expected.Agg), nameof(HudLightIndicators.Agg));
                Assert.That(actual.AggPr, Is.EqualTo(expected.AggPr), nameof(HudLightIndicators.AggPr));
                Assert.That(actual.TrueAggression, Is.EqualTo(expected.TrueAggression), nameof(HudLightIndicators.TrueAggression));
                Assert.That(actual.WSWSF, Is.EqualTo(expected.WSWSF), nameof(HudLightIndicators.WSWSF));
                Assert.That(actual.TotalHands, Is.EqualTo(expected.TotalHands), nameof(HudLightIndicators.TotalHands));
                Assert.That(actual.TotalWon, Is.EqualTo(expected.TotalWon), nameof(HudLightIndicators.TotalWon));
                Assert.That(actual.ColdCall, Is.EqualTo(expected.ColdCall), nameof(HudLightIndicators.ColdCall));
                Assert.That(actual.FlopAgg, Is.EqualTo(expected.FlopAgg), nameof(HudLightIndicators.FlopAgg));
                Assert.That(actual.TurnAgg, Is.EqualTo(expected.TurnAgg), nameof(HudLightIndicators.TurnAgg));
                Assert.That(actual.RiverAgg, Is.EqualTo(expected.RiverAgg), nameof(HudLightIndicators.RiverAgg));
                Assert.That(actual.FoldCBet, Is.EqualTo(expected.FoldCBet), nameof(HudLightIndicators.FoldCBet));
                Assert.That(actual.RaiseCBet, Is.EqualTo(expected.RaiseCBet), nameof(HudLightIndicators.RaiseCBet));
                Assert.That(actual.FoldToFourBet, Is.EqualTo(expected.FoldToFourBet), nameof(HudLightIndicators.FoldToFourBet));
                Assert.That(actual.Squeeze, Is.EqualTo(expected.Squeeze), nameof(HudLightIndicators.Squeeze));
                Assert.That(actual.CheckRaise, Is.EqualTo(expected.CheckRaise), nameof(HudLightIndicators.CheckRaise));
                Assert.That(actual.FlopCheckRaise, Is.EqualTo(expected.FlopCheckRaise), nameof(HudLightIndicators.FlopCheckRaise));
                Assert.That(actual.TurnCheckRaise, Is.EqualTo(expected.TurnCheckRaise), nameof(HudLightIndicators.TurnCheckRaise));
                Assert.That(actual.RiverCheckRaise, Is.EqualTo(expected.RiverCheckRaise), nameof(HudLightIndicators.RiverCheckRaise));
                Assert.That(actual.FloatFlop, Is.EqualTo(expected.FloatFlop), nameof(HudLightIndicators.FloatFlop));
                Assert.That(actual.RaiseFlop, Is.EqualTo(expected.RaiseFlop), nameof(HudLightIndicators.RaiseFlop));
                Assert.That(actual.RaiseTurn, Is.EqualTo(expected.RaiseTurn), nameof(HudLightIndicators.RaiseTurn));
                Assert.That(actual.RaiseRiver, Is.EqualTo(expected.RaiseRiver), nameof(HudLightIndicators.RaiseRiver));
                Assert.That(actual.RaiseFrequencyFactor, Is.EqualTo(expected.RaiseFrequencyFactor), nameof(HudLightIndicators.RaiseFrequencyFactor));
                Assert.That(actual.TurnSeen, Is.EqualTo(expected.TurnSeen), nameof(HudLightIndicators.TurnSeen));
                Assert.That(actual.RiverSeen, Is.EqualTo(expected.RiverSeen), nameof(HudLightIndicators.RiverSeen));
                Assert.That(actual.ThreeBetVsSteal, Is.EqualTo(expected.ThreeBetVsSteal), nameof(HudLightIndicators.ThreeBetVsSteal));
                Assert.That(actual.CheckRiverOnBXLine, Is.EqualTo(expected.CheckRiverOnBXLine), nameof(HudLightIndicators.CheckRiverOnBXLine));
                Assert.That(actual.CBetIP, Is.EqualTo(expected.CBetIP), nameof(HudLightIndicators.CBetIP));
                Assert.That(actual.CBetOOP, Is.EqualTo(expected.CBetOOP), nameof(HudLightIndicators.CBetOOP));
                Assert.That(actual.ThreeBet_BB, Is.EqualTo(expected.ThreeBet_BB), nameof(HudLightIndicators.ThreeBet_BB));
                Assert.That(actual.ThreeBet_BN, Is.EqualTo(expected.ThreeBet_BN), nameof(HudLightIndicators.ThreeBet_BN));
                Assert.That(actual.ThreeBet_CO, Is.EqualTo(expected.ThreeBet_CO), nameof(HudLightIndicators.ThreeBet_CO));
                Assert.That(actual.ThreeBet_EP, Is.EqualTo(expected.ThreeBet_EP), nameof(HudLightIndicators.ThreeBet_EP));
                Assert.That(actual.ThreeBet_MP, Is.EqualTo(expected.ThreeBet_MP), nameof(HudLightIndicators.ThreeBet_MP));
                Assert.That(actual.ThreeBet_SB, Is.EqualTo(expected.ThreeBet_SB), nameof(HudLightIndicators.ThreeBet_SB));
                Assert.That(actual.FourBetInBB, Is.EqualTo(expected.FourBetInBB), nameof(HudLightIndicators.FourBetInBB));
                Assert.That(actual.FourBetInBTN, Is.EqualTo(expected.FourBetInBTN), nameof(HudLightIndicators.FourBetInBTN));
                Assert.That(actual.FourBetInCO, Is.EqualTo(expected.FourBetInCO), nameof(HudLightIndicators.FourBetInCO));
                Assert.That(actual.FourBetInMP, Is.EqualTo(expected.FourBetInMP), nameof(HudLightIndicators.FourBetInMP));
                Assert.That(actual.FourBetInSB, Is.EqualTo(expected.FourBetInSB), nameof(HudLightIndicators.FourBetInSB));
                Assert.That(actual.ColdCall_BB, Is.EqualTo(expected.ColdCall_BB), nameof(HudLightIndicators.ColdCall_BB));
                Assert.That(actual.ColdCall_BN, Is.EqualTo(expected.ColdCall_BN), nameof(HudLightIndicators.ColdCall_BN));
                Assert.That(actual.ColdCall_CO, Is.EqualTo(expected.ColdCall_CO), nameof(HudLightIndicators.ColdCall_CO));
                Assert.That(actual.ColdCall_EP, Is.EqualTo(expected.ColdCall_EP), nameof(HudLightIndicators.ColdCall_EP));
                Assert.That(actual.ColdCall_MP, Is.EqualTo(expected.ColdCall_MP), nameof(HudLightIndicators.ColdCall_MP));
                Assert.That(actual.ColdCall_SB, Is.EqualTo(expected.ColdCall_SB), nameof(HudLightIndicators.ColdCall_SB));
                Assert.That(actual.DonkBet, Is.EqualTo(expected.DonkBet), nameof(HudLightIndicators.DonkBet));
                Assert.That(actual.DidDelayedTurnCBet, Is.EqualTo(expected.DidDelayedTurnCBet), nameof(HudLightIndicators.DidDelayedTurnCBet));
                Assert.That(actual.UO_PFR_EP, Is.EqualTo(expected.UO_PFR_EP), nameof(HudLightIndicators.UO_PFR_EP));
                Assert.That(actual.UO_PFR_MP, Is.EqualTo(expected.UO_PFR_MP), nameof(HudLightIndicators.UO_PFR_MP));
                Assert.That(actual.UO_PFR_CO, Is.EqualTo(expected.UO_PFR_CO), nameof(HudLightIndicators.UO_PFR_CO));
                Assert.That(actual.UO_PFR_BN, Is.EqualTo(expected.UO_PFR_BN), nameof(HudLightIndicators.UO_PFR_BN));
                Assert.That(actual.UO_PFR_SB, Is.EqualTo(expected.UO_PFR_SB), nameof(HudLightIndicators.UO_PFR_SB));
                Assert.That(actual.UO_PFR_BB, Is.EqualTo(expected.UO_PFR_BB), nameof(HudLightIndicators.UO_PFR_BB));
                Assert.That(actual.PFRInEP, Is.EqualTo(expected.PFRInEP), nameof(HudLightIndicators.PFRInEP));
                Assert.That(actual.PFRInMP, Is.EqualTo(expected.PFRInMP), nameof(HudLightIndicators.PFRInMP));
                Assert.That(actual.PFRInCO, Is.EqualTo(expected.PFRInCO), nameof(HudLightIndicators.PFRInCO));
                Assert.That(actual.PFRInBTN, Is.EqualTo(expected.PFRInBTN), nameof(HudLightIndicators.PFRInBTN));
                Assert.That(actual.PFRInBB, Is.EqualTo(expected.PFRInBB), nameof(HudLightIndicators.PFRInBB));
                Assert.That(actual.PFRInSB, Is.EqualTo(expected.PFRInSB), nameof(HudLightIndicators.PFRInSB));
                Assert.That(actual.DidLimp, Is.EqualTo(expected.DidLimp), nameof(HudLightIndicators.DidLimp));
                Assert.That(actual.DidLimpCall, Is.EqualTo(expected.DidLimpCall), nameof(HudLightIndicators.DidLimpCall));
                Assert.That(actual.DidLimpFold, Is.EqualTo(expected.DidLimpFold), nameof(HudLightIndicators.DidLimpFold));
                Assert.That(actual.DidLimpReraise, Is.EqualTo(expected.DidLimpReraise), nameof(HudLightIndicators.DidLimpReraise));
                Assert.That(actual.CheckFoldFlopPfrOop, Is.EqualTo(expected.CheckFoldFlopPfrOop), nameof(HudLightIndicators.CheckFoldFlopPfrOop));
                Assert.That(actual.CheckFoldFlop3BetOop, Is.EqualTo(expected.CheckFoldFlop3BetOop), nameof(HudLightIndicators.CheckFoldFlop3BetOop));
                Assert.That(actual.BetFoldFlopPfrRaiser, Is.EqualTo(expected.BetFoldFlopPfrRaiser), nameof(HudLightIndicators.BetFoldFlopPfrRaiser));
                Assert.That(actual.BetFlopCalled3BetPreflopIp, Is.EqualTo(expected.BetFlopCalled3BetPreflopIp), nameof(HudLightIndicators.BetFlopCalled3BetPreflopIp));
                Assert.That(actual.BTNDefendCORaise, Is.EqualTo(expected.BTNDefendCORaise), nameof(HudLightIndicators.BTNDefendCORaise));
                Assert.That(actual.EVDiff, Is.EqualTo(expected.EVDiff), nameof(HudLightIndicators.EVDiff));
                Assert.That(actual.EVBB, Is.EqualTo(expected.EVBB), nameof(HudLightIndicators.EVBB));
                Assert.That(actual.BetWhenCheckedTo, Is.EqualTo(expected.BetWhenCheckedTo), nameof(HudLightIndicators.BetWhenCheckedTo));
                Assert.That(actual.FoldToFlopRaise, Is.EqualTo(expected.FoldToFlopRaise), nameof(HudLightIndicators.FoldToFlopRaise));
                Assert.That(actual.FoldToTurnRaise, Is.EqualTo(expected.FoldToTurnRaise), nameof(HudLightIndicators.FoldToTurnRaise));
                Assert.That(actual.FoldToRiverCBet, Is.EqualTo(expected.FoldToRiverCBet), nameof(HudLightIndicators.FoldToRiverCBet));
                Assert.That(actual.FoldToSqueez, Is.EqualTo(expected.FoldToSqueez), nameof(HudLightIndicators.FoldToSqueez));
                Assert.That(actual.VPIP_EP, Is.EqualTo(expected.VPIP_EP), nameof(HudLightIndicators.VPIP_EP));
                Assert.That(actual.VPIP_MP, Is.EqualTo(expected.VPIP_MP), nameof(HudLightIndicators.VPIP_MP));
                Assert.That(actual.VPIP_CO, Is.EqualTo(expected.VPIP_CO), nameof(HudLightIndicators.VPIP_CO));
                Assert.That(actual.VPIP_BN, Is.EqualTo(expected.VPIP_BN), nameof(HudLightIndicators.VPIP_BN));
                Assert.That(actual.VPIP_SB, Is.EqualTo(expected.VPIP_SB), nameof(HudLightIndicators.VPIP_SB));
                Assert.That(actual.VPIP_BB, Is.EqualTo(expected.VPIP_BB), nameof(HudLightIndicators.VPIP_BB));
                Assert.That(actual.ColdCall_EP, Is.EqualTo(expected.ColdCall_EP), nameof(HudLightIndicators.ColdCall_EP));
                Assert.That(actual.ColdCall_MP, Is.EqualTo(expected.ColdCall_MP), nameof(HudLightIndicators.ColdCall_MP));
                Assert.That(actual.ColdCall_CO, Is.EqualTo(expected.ColdCall_CO), nameof(HudLightIndicators.ColdCall_CO));
                Assert.That(actual.ColdCall_BN, Is.EqualTo(expected.ColdCall_BN), nameof(HudLightIndicators.ColdCall_BN));
                Assert.That(actual.ColdCall_SB, Is.EqualTo(expected.ColdCall_SB), nameof(HudLightIndicators.ColdCall_SB));
                Assert.That(actual.ColdCall_BB, Is.EqualTo(expected.ColdCall_BB), nameof(HudLightIndicators.ColdCall_BB));
                Assert.That(actual.ThreeBet_EP, Is.EqualTo(expected.ThreeBet_EP), nameof(HudLightIndicators.ThreeBet_EP));
                Assert.That(actual.ThreeBet_MP, Is.EqualTo(expected.ThreeBet_MP), nameof(HudLightIndicators.ThreeBet_MP));
                Assert.That(actual.ThreeBet_CO, Is.EqualTo(expected.ThreeBet_CO), nameof(HudLightIndicators.ThreeBet_CO));
                Assert.That(actual.ThreeBet_BN, Is.EqualTo(expected.ThreeBet_BN), nameof(HudLightIndicators.ThreeBet_BN));
                Assert.That(actual.ThreeBet_SB, Is.EqualTo(expected.ThreeBet_SB), nameof(HudLightIndicators.ThreeBet_SB));
                Assert.That(actual.ThreeBet_BB, Is.EqualTo(expected.ThreeBet_BB), nameof(HudLightIndicators.ThreeBet_BB));
                Assert.That(actual.StdDev, Is.EqualTo(expected.StdDev), nameof(HudLightIndicators.StdDev));
                Assert.That(actual.StdDevBB, Is.EqualTo(expected.StdDevBB), nameof(HudLightIndicators.StdDevBB));
                Assert.That(actual.StdDevBBPer100Hands, Is.EqualTo(expected.StdDevBBPer100Hands), nameof(HudLightIndicators.StdDevBBPer100Hands));
                Assert.That(actual.NetWonPerHour, Is.EqualTo(expected.NetWonPerHour), nameof(HudLightIndicators.NetWonPerHour));
                Assert.That(actual.FoldToFlopCheckRaise, Is.EqualTo(expected.FoldToFlopCheckRaise), nameof(HudLightIndicators.FoldToFlopCheckRaise));
                Assert.That(actual.FoldToTurnCheckRaise, Is.EqualTo(expected.FoldToTurnCheckRaise), nameof(HudLightIndicators.FoldToTurnCheckRaise));
                Assert.That(actual.FoldToRiverCheckRaise, Is.EqualTo(expected.FoldToRiverCheckRaise), nameof(HudLightIndicators.FoldToRiverCheckRaise));
                Assert.That(actual.CalledTurnCheckRaise, Is.EqualTo(expected.CalledTurnCheckRaise), nameof(HudLightIndicators.CalledTurnCheckRaise));
                Assert.That(actual.CheckRiverAfterBBLine, Is.EqualTo(expected.CheckRiverAfterBBLine), nameof(HudLightIndicators.CheckRiverAfterBBLine));
                Assert.That(actual.BetRiverOnBXLine, Is.EqualTo(expected.BetRiverOnBXLine), nameof(HudLightIndicators.BetRiverOnBXLine));
                Assert.That(actual.CallFlopCBetIP, Is.EqualTo(expected.CallFlopCBetIP), nameof(HudLightIndicators.CallFlopCBetIP));
                Assert.That(actual.CallFlopCBetOOP, Is.EqualTo(expected.CallFlopCBetOOP), nameof(HudLightIndicators.CallFlopCBetOOP));
                Assert.That(actual.FoldToFlopCBetIP, Is.EqualTo(expected.FoldToFlopCBetIP), nameof(HudLightIndicators.FoldToFlopCBetIP));
                Assert.That(actual.FoldToFlopCBetOOP, Is.EqualTo(expected.FoldToFlopCBetOOP), nameof(HudLightIndicators.FoldToFlopCBetOOP));
                Assert.That(actual.CallRiverRaise, Is.EqualTo(expected.CallRiverRaise), nameof(HudLightIndicators.CallRiverRaise));
                Assert.That(actual.RiverBet, Is.EqualTo(expected.RiverBet), nameof(HudLightIndicators.RiverBet));
                Assert.That(actual.DelayedTurnCBetIP, Is.EqualTo(expected.DelayedTurnCBetIP), nameof(HudLightIndicators.DelayedTurnCBetIP));
                Assert.That(actual.DelayedTurnCBetOOP, Is.EqualTo(expected.DelayedTurnCBetOOP), nameof(HudLightIndicators.DelayedTurnCBetOOP));
                Assert.That(actual.FiveBet, Is.EqualTo(expected.FiveBet), nameof(HudLightIndicators.FiveBet));
                Assert.That(actual.CalledCheckRaiseVsFlopCBet, Is.EqualTo(expected.CalledCheckRaiseVsFlopCBet), nameof(HudLightIndicators.CalledCheckRaiseVsFlopCBet));
                Assert.That(actual.FoldedCheckRaiseVsFlopCBet, Is.EqualTo(expected.FoldedCheckRaiseVsFlopCBet), nameof(HudLightIndicators.FoldedCheckRaiseVsFlopCBet));
                Assert.That(actual.CheckFlopAsPFRAndXCOnTurnOOP, Is.EqualTo(expected.CheckFlopAsPFRAndXCOnTurnOOP), nameof(HudLightIndicators.CheckFlopAsPFRAndXCOnTurnOOP));
                Assert.That(actual.CheckFlopAsPFRAndXFOnTurnOOP, Is.EqualTo(expected.CheckFlopAsPFRAndXFOnTurnOOP), nameof(HudLightIndicators.CheckFlopAsPFRAndXFOnTurnOOP));
                Assert.That(actual.CheckFlopAsPFRAndCallOnTurn, Is.EqualTo(expected.CheckFlopAsPFRAndCallOnTurn), nameof(HudLightIndicators.CheckFlopAsPFRAndCallOnTurn));
                Assert.That(actual.CheckFlopAsPFRAndFoldOnTurn, Is.EqualTo(expected.CheckFlopAsPFRAndFoldOnTurn), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldOnTurn));
                Assert.That(actual.CheckFlopAsPFRAndRaiseOnTurn, Is.EqualTo(expected.CheckFlopAsPFRAndRaiseOnTurn), nameof(HudLightIndicators.CheckFlopAsPFRAndRaiseOnTurn));
                Assert.That(actual.CheckRaisedFlopCBet, Is.EqualTo(expected.CheckRaisedFlopCBet), nameof(HudLightIndicators.CheckRaisedFlopCBet));
                Assert.That(actual.FlopBetSizeOneHalfOrLess, Is.EqualTo(expected.FlopBetSizeOneHalfOrLess), nameof(HudLightIndicators.FlopBetSizeOneHalfOrLess));
                Assert.That(actual.FlopBetSizeOneQuarterOrLess, Is.EqualTo(expected.FlopBetSizeOneQuarterOrLess), nameof(HudLightIndicators.FlopBetSizeOneQuarterOrLess));
                Assert.That(actual.FlopBetSizeTwoThirdsOrLess, Is.EqualTo(expected.FlopBetSizeTwoThirdsOrLess), nameof(HudLightIndicators.FlopBetSizeTwoThirdsOrLess));
                Assert.That(actual.FlopBetSizeThreeQuartersOrLess, Is.EqualTo(expected.FlopBetSizeThreeQuartersOrLess), nameof(HudLightIndicators.FlopBetSizeThreeQuartersOrLess));
                Assert.That(actual.FlopBetSizeOneOrLess, Is.EqualTo(expected.FlopBetSizeOneOrLess), nameof(HudLightIndicators.FlopBetSizeOneOrLess));
                Assert.That(actual.FlopBetSizeMoreThanOne, Is.EqualTo(expected.FlopBetSizeMoreThanOne), nameof(HudLightIndicators.FlopBetSizeMoreThanOne));
                Assert.That(actual.TurnBetSizeOneHalfOrLess, Is.EqualTo(expected.TurnBetSizeOneHalfOrLess), nameof(HudLightIndicators.TurnBetSizeOneHalfOrLess));
                Assert.That(actual.TurnBetSizeOneQuarterOrLess, Is.EqualTo(expected.TurnBetSizeOneQuarterOrLess), nameof(HudLightIndicators.TurnBetSizeOneQuarterOrLess));
                Assert.That(actual.TurnBetSizeOneThirdOrLess, Is.EqualTo(expected.TurnBetSizeOneThirdOrLess), nameof(HudLightIndicators.TurnBetSizeOneThirdOrLess));
                Assert.That(actual.TurnBetSizeTwoThirdsOrLess, Is.EqualTo(expected.TurnBetSizeTwoThirdsOrLess), nameof(HudLightIndicators.TurnBetSizeTwoThirdsOrLess));
                Assert.That(actual.TurnBetSizeThreeQuartersOrLess, Is.EqualTo(expected.TurnBetSizeThreeQuartersOrLess), nameof(HudLightIndicators.TurnBetSizeThreeQuartersOrLess));
                Assert.That(actual.TurnBetSizeOneOrLess, Is.EqualTo(expected.TurnBetSizeOneOrLess), nameof(HudLightIndicators.TurnBetSizeOneOrLess));
                Assert.That(actual.TurnBetSizeMoreThanOne, Is.EqualTo(expected.TurnBetSizeMoreThanOne), nameof(HudLightIndicators.TurnBetSizeMoreThanOne));
                Assert.That(actual.WTSDAfterCalling3Bet, Is.EqualTo(expected.WTSDAfterCalling3Bet), nameof(HudLightIndicators.WTSDAfterCalling3Bet));
                Assert.That(actual.WTSDAfterCallingPfr, Is.EqualTo(expected.WTSDAfterCallingPfr), nameof(HudLightIndicators.WTSDAfterCallingPfr));
                Assert.That(actual.WTSDAfterNotCBettingFlopAsPfr, Is.EqualTo(expected.WTSDAfterNotCBettingFlopAsPfr), nameof(HudLightIndicators.WTSDAfterNotCBettingFlopAsPfr));
                Assert.That(actual.WTSDAfterSeeingTurn, Is.EqualTo(expected.WTSDAfterSeeingTurn), nameof(HudLightIndicators.WTSDAfterSeeingTurn));
                Assert.That(actual.WTSDAsPF3Bettor, Is.EqualTo(expected.WTSDAsPF3Bettor), nameof(HudLightIndicators.WTSDAsPF3Bettor));
                Assert.That(actual.DelayedTurnCBetIn3BetPot, Is.EqualTo(expected.DelayedTurnCBetIn3BetPot), nameof(HudLightIndicators.DelayedTurnCBetIn3BetPot));
                Assert.That(actual.FoldToTurnCBetIn3BetPot, Is.EqualTo(expected.FoldToTurnCBetIn3BetPot), nameof(HudLightIndicators.FoldToTurnCBetIn3BetPot));
                Assert.That(actual.FlopCheckBehind, Is.EqualTo(expected.FlopCheckBehind), nameof(HudLightIndicators.FlopCheckBehind));
                Assert.That(actual.FoldToDonkBet, Is.EqualTo(expected.FoldToDonkBet), nameof(HudLightIndicators.FoldToDonkBet));
                Assert.That(actual.FoldTurn, Is.EqualTo(expected.FoldTurn), nameof(HudLightIndicators.FoldTurn));
                Assert.That(actual.RiverCheckCall, Is.EqualTo(expected.RiverCheckCall), nameof(HudLightIndicators.RiverCheckCall));
                Assert.That(actual.RiverCheckFold, Is.EqualTo(expected.RiverCheckFold), nameof(HudLightIndicators.RiverCheckFold));
                Assert.That(actual.RiverBetSizeMoreThanOne, Is.EqualTo(expected.RiverBetSizeMoreThanOne), nameof(HudLightIndicators.RiverBetSizeMoreThanOne));
                Assert.That(actual.RiverCallEffeciency, Is.EqualTo(expected.RiverCallEffeciency), nameof(HudLightIndicators.RiverCallEffeciency));
                Assert.That(actual.FoldToFiveBet, Is.EqualTo(expected.FoldToFiveBet), nameof(HudLightIndicators.FoldToFiveBet));
                Assert.That(actual.TurnAF, Is.EqualTo(expected.TurnAF), nameof(HudLightIndicators.TurnAF));
                Assert.That(actual.ShovedFlopAfter4Bet, Is.EqualTo(expected.ShovedFlopAfter4Bet), nameof(HudLightIndicators.ShovedFlopAfter4Bet));
                Assert.That(actual.RaiseFlopCBetIn3BetPot, Is.EqualTo(expected.RaiseFlopCBetIn3BetPot), nameof(HudLightIndicators.RaiseFlopCBetIn3BetPot));
                Assert.That(actual.FoldToThreeBetIP, Is.EqualTo(expected.FoldToThreeBetIP), nameof(HudLightIndicators.FoldToThreeBetIP));
                Assert.That(actual.FoldToThreeBetOOP, Is.EqualTo(expected.FoldToThreeBetOOP), nameof(HudLightIndicators.FoldToThreeBetOOP));
                Assert.That(actual.BetFlopWhenCheckedToSRP, Is.EqualTo(expected.BetFlopWhenCheckedToSRP), nameof(HudLightIndicators.BetFlopWhenCheckedToSRP));
                // add new stats here            
            });
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

        protected override void InitializePlayerStatisticRepository(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IPlayerStatisticRepository, PlayerStatisticRepository>(new ContainerControlledLifetimeManager());
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