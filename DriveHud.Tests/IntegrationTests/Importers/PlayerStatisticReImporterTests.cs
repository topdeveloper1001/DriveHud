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
                Assert.That(actual.TurnBet, Is.EqualTo(expected.TurnBet), nameof(HudLightIndicators.TurnBet));
                Assert.That(actual.FlopBet, Is.EqualTo(expected.FlopBet), nameof(HudLightIndicators.FlopBet));
                Assert.That(actual.RiverCBet, Is.EqualTo(expected.RiverCBet), nameof(HudLightIndicators.RiverCBet));
                Assert.That(actual.FoldFlop, Is.EqualTo(expected.FoldFlop), nameof(HudLightIndicators.FoldFlop));
                Assert.That(actual.RaiseLimpers, Is.EqualTo(expected.RaiseLimpers), nameof(HudLightIndicators.RaiseLimpers));
                Assert.That(actual.RaiseLimpersInMP, Is.EqualTo(expected.RaiseLimpersInMP), nameof(HudLightIndicators.RaiseLimpersInMP));
                Assert.That(actual.RaiseLimpersInCO, Is.EqualTo(expected.RaiseLimpersInCO), nameof(HudLightIndicators.RaiseLimpersInCO));
                Assert.That(actual.RaiseLimpersInBN, Is.EqualTo(expected.RaiseLimpersInBN), nameof(HudLightIndicators.RaiseLimpersInBN));
                Assert.That(actual.RaiseLimpersInSB, Is.EqualTo(expected.RaiseLimpersInSB), nameof(HudLightIndicators.RaiseLimpersInSB));
                Assert.That(actual.RaiseLimpersInBB, Is.EqualTo(expected.RaiseLimpersInBB), nameof(HudLightIndicators.RaiseLimpersInBB));
                Assert.That(actual.ThreeBetMPvsEP, Is.EqualTo(expected.ThreeBetMPvsEP), nameof(HudLightIndicators.ThreeBetMPvsEP));
                Assert.That(actual.ThreeBetCOvsEP, Is.EqualTo(expected.ThreeBetCOvsEP), nameof(HudLightIndicators.ThreeBetCOvsEP));
                Assert.That(actual.ThreeBetCOvsMP, Is.EqualTo(expected.ThreeBetCOvsMP), nameof(HudLightIndicators.ThreeBetCOvsMP));
                Assert.That(actual.ThreeBetBTNvsEP, Is.EqualTo(expected.ThreeBetBTNvsEP), nameof(HudLightIndicators.ThreeBetBTNvsEP));
                Assert.That(actual.ThreeBetBTNvsMP, Is.EqualTo(expected.ThreeBetBTNvsMP), nameof(HudLightIndicators.ThreeBetBTNvsMP));
                Assert.That(actual.ThreeBetBTNvsCO, Is.EqualTo(expected.ThreeBetBTNvsCO), nameof(HudLightIndicators.ThreeBetBTNvsCO));
                Assert.That(actual.ThreeBetSBvsEP, Is.EqualTo(expected.ThreeBetSBvsEP), nameof(HudLightIndicators.ThreeBetSBvsEP));
                Assert.That(actual.ThreeBetSBvsMP, Is.EqualTo(expected.ThreeBetSBvsMP), nameof(HudLightIndicators.ThreeBetSBvsMP));
                Assert.That(actual.ThreeBetSBvsCO, Is.EqualTo(expected.ThreeBetSBvsCO), nameof(HudLightIndicators.ThreeBetSBvsCO));
                Assert.That(actual.ThreeBetSBvsBTN, Is.EqualTo(expected.ThreeBetSBvsBTN), nameof(HudLightIndicators.ThreeBetSBvsBTN));
                Assert.That(actual.ThreeBetBBvsEP, Is.EqualTo(expected.ThreeBetBBvsEP), nameof(HudLightIndicators.ThreeBetBBvsEP));
                Assert.That(actual.ThreeBetBBvsMP, Is.EqualTo(expected.ThreeBetBBvsMP), nameof(HudLightIndicators.ThreeBetBBvsMP));
                Assert.That(actual.ThreeBetBBvsCO, Is.EqualTo(expected.ThreeBetBBvsCO), nameof(HudLightIndicators.ThreeBetBBvsCO));
                Assert.That(actual.ThreeBetBBvsBTN, Is.EqualTo(expected.ThreeBetBBvsBTN), nameof(HudLightIndicators.ThreeBetBBvsBTN));
                Assert.That(actual.ThreeBetBBvsSB, Is.EqualTo(expected.ThreeBetBBvsSB), nameof(HudLightIndicators.ThreeBetBBvsSB));
                Assert.That(actual.FoldTo3BetInEPvs3BetMP, Is.EqualTo(expected.FoldTo3BetInEPvs3BetMP), nameof(HudLightIndicators.FoldTo3BetInEPvs3BetMP));
                Assert.That(actual.FoldTo3BetInEPvs3BetCO, Is.EqualTo(expected.FoldTo3BetInEPvs3BetCO), nameof(HudLightIndicators.FoldTo3BetInEPvs3BetCO));
                Assert.That(actual.FoldTo3BetInEPvs3BetBTN, Is.EqualTo(expected.FoldTo3BetInEPvs3BetBTN), nameof(HudLightIndicators.FoldTo3BetInEPvs3BetBTN));
                Assert.That(actual.FoldTo3BetInEPvs3BetSB, Is.EqualTo(expected.FoldTo3BetInEPvs3BetSB), nameof(HudLightIndicators.FoldTo3BetInEPvs3BetSB));
                Assert.That(actual.FoldTo3BetInEPvs3BetBB, Is.EqualTo(expected.FoldTo3BetInEPvs3BetBB), nameof(HudLightIndicators.FoldTo3BetInEPvs3BetBB));
                Assert.That(actual.FoldTo3BetInMPvs3BetCO, Is.EqualTo(expected.FoldTo3BetInMPvs3BetCO), nameof(HudLightIndicators.FoldTo3BetInMPvs3BetCO));
                Assert.That(actual.FoldTo3BetInMPvs3BetBTN, Is.EqualTo(expected.FoldTo3BetInMPvs3BetBTN), nameof(HudLightIndicators.FoldTo3BetInMPvs3BetBTN));
                Assert.That(actual.FoldTo3BetInMPvs3BetSB, Is.EqualTo(expected.FoldTo3BetInMPvs3BetSB), nameof(HudLightIndicators.FoldTo3BetInMPvs3BetSB));
                Assert.That(actual.FoldTo3BetInMPvs3BetBB, Is.EqualTo(expected.FoldTo3BetInMPvs3BetBB), nameof(HudLightIndicators.FoldTo3BetInMPvs3BetBB));
                Assert.That(actual.FoldTo3BetInCOvs3BetBTN, Is.EqualTo(expected.FoldTo3BetInCOvs3BetBTN), nameof(HudLightIndicators.FoldTo3BetInCOvs3BetBTN));
                Assert.That(actual.FoldTo3BetInCOvs3BetSB, Is.EqualTo(expected.FoldTo3BetInCOvs3BetSB), nameof(HudLightIndicators.FoldTo3BetInCOvs3BetSB));
                Assert.That(actual.FoldTo3BetInCOvs3BetBB, Is.EqualTo(expected.FoldTo3BetInCOvs3BetBB), nameof(HudLightIndicators.FoldTo3BetInCOvs3BetBB));
                Assert.That(actual.FoldTo3BetInBTNvs3BetSB, Is.EqualTo(expected.FoldTo3BetInBTNvs3BetSB), nameof(HudLightIndicators.FoldTo3BetInBTNvs3BetSB));
                Assert.That(actual.FoldTo3BetInBTNvs3BetBB, Is.EqualTo(expected.FoldTo3BetInBTNvs3BetBB), nameof(HudLightIndicators.FoldTo3BetInBTNvs3BetBB));
                Assert.That(actual.FoldToRiverRaise, Is.EqualTo(expected.FoldToRiverRaise), nameof(HudLightIndicators.FoldToRiverRaise));
                Assert.That(actual.CheckRaiseFlopAsPFR, Is.EqualTo(expected.CheckRaiseFlopAsPFR), nameof(HudLightIndicators.CheckRaiseFlopAsPFR));
                Assert.That(actual.ProbeBetTurn, Is.EqualTo(expected.ProbeBetTurn), nameof(HudLightIndicators.ProbeBetTurn));
                Assert.That(actual.ProbeBetRiver, Is.EqualTo(expected.ProbeBetRiver), nameof(HudLightIndicators.ProbeBetRiver));
                Assert.That(actual.FloatFlopThenBetTurn, Is.EqualTo(expected.FloatFlopThenBetTurn), nameof(HudLightIndicators.FloatFlopThenBetTurn));
                Assert.That(actual.FoldBBvsSBSteal, Is.EqualTo(expected.FoldBBvsSBSteal), nameof(HudLightIndicators.FoldBBvsSBSteal));
                Assert.That(actual.BetTurnWhenCheckedToSRP, Is.EqualTo(expected.BetTurnWhenCheckedToSRP), nameof(HudLightIndicators.BetTurnWhenCheckedToSRP));
                Assert.That(actual.BetRiverWhenCheckedToSRP, Is.EqualTo(expected.BetRiverWhenCheckedToSRP), nameof(HudLightIndicators.BetRiverWhenCheckedToSRP));
                Assert.That(actual.BetFlopWhenCheckedToIn3BetPot, Is.EqualTo(expected.BetFlopWhenCheckedToIn3BetPot), nameof(HudLightIndicators.BetFlopWhenCheckedToIn3BetPot));
                Assert.That(actual.BetTurnWhenCheckedToIn3BetPot, Is.EqualTo(expected.BetTurnWhenCheckedToIn3BetPot), nameof(HudLightIndicators.BetTurnWhenCheckedToIn3BetPot));
                Assert.That(actual.BetRiverWhenCheckedToIn3BetPot, Is.EqualTo(expected.BetRiverWhenCheckedToIn3BetPot), nameof(HudLightIndicators.BetRiverWhenCheckedToIn3BetPot));
                Assert.That(actual.ColdCall3BetInBB, Is.EqualTo(expected.ColdCall3BetInBB), nameof(HudLightIndicators.ColdCall3BetInBB));
                Assert.That(actual.ColdCall3BetInSB, Is.EqualTo(expected.ColdCall3BetInSB), nameof(HudLightIndicators.ColdCall3BetInSB));
                Assert.That(actual.ColdCall3BetInMP, Is.EqualTo(expected.ColdCall3BetInMP), nameof(HudLightIndicators.ColdCall3BetInMP));
                Assert.That(actual.ColdCall3BetInCO, Is.EqualTo(expected.ColdCall3BetInCO), nameof(HudLightIndicators.ColdCall3BetInCO));
                Assert.That(actual.ColdCall3BetInBTN, Is.EqualTo(expected.ColdCall3BetInBTN), nameof(HudLightIndicators.ColdCall3BetInBTN));
                Assert.That(actual.ColdCall4BetInBB, Is.EqualTo(expected.ColdCall4BetInBB), nameof(HudLightIndicators.ColdCall4BetInBB));
                Assert.That(actual.ColdCall4BetInSB, Is.EqualTo(expected.ColdCall4BetInSB), nameof(HudLightIndicators.ColdCall4BetInSB));
                Assert.That(actual.ColdCall4BetInMP, Is.EqualTo(expected.ColdCall4BetInMP), nameof(HudLightIndicators.ColdCall4BetInMP));
                Assert.That(actual.ColdCall4BetInCO, Is.EqualTo(expected.ColdCall4BetInCO), nameof(HudLightIndicators.ColdCall4BetInCO));
                Assert.That(actual.ColdCall4BetInBTN, Is.EqualTo(expected.ColdCall4BetInBTN), nameof(HudLightIndicators.ColdCall4BetInBTN));
                Assert.That(actual.DoubleBarrelSRP, Is.EqualTo(expected.DoubleBarrelSRP), nameof(HudLightIndicators.DoubleBarrelSRP));
                Assert.That(actual.DoubleBarrel3BetPot, Is.EqualTo(expected.DoubleBarrel3BetPot), nameof(HudLightIndicators.DoubleBarrel3BetPot));
                Assert.That(actual.TripleBarrelSRP, Is.EqualTo(expected.TripleBarrelSRP), nameof(HudLightIndicators.TripleBarrelSRP));
                Assert.That(actual.TripleBarrel3BetPot, Is.EqualTo(expected.TripleBarrel3BetPot), nameof(HudLightIndicators.TripleBarrel3BetPot));
                Assert.That(actual.CBetThenFoldFlopSRP, Is.EqualTo(expected.CBetThenFoldFlopSRP), nameof(HudLightIndicators.CBetThenFoldFlopSRP));
                Assert.That(actual.FoldToProbeBetTurn, Is.EqualTo(expected.FoldToProbeBetTurn), nameof(HudLightIndicators.FoldToProbeBetTurn));
                Assert.That(actual.FoldToProbeBetRiver, Is.EqualTo(expected.FoldToProbeBetRiver), nameof(HudLightIndicators.FoldToProbeBetRiver));
                Assert.That(actual.CheckFlopAsPFRAndFoldToTurnBetIPSRP, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToTurnBetIPSRP), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToTurnBetIPSRP));
                Assert.That(actual.CheckFlopAsPFRAndFoldToTurnBetOOPSRP, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToTurnBetOOPSRP), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToTurnBetOOPSRP));
                Assert.That(actual.CheckFlopAsPFRAndFoldToRiverBetIPSRP, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToRiverBetIPSRP), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToRiverBetIPSRP));
                Assert.That(actual.CheckFlopAsPFRAndFoldToRiverBetOOPSRP, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToRiverBetOOPSRP), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToRiverBetOOPSRP));
                Assert.That(actual.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToTurnBetIP3BetPot));
                Assert.That(actual.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToTurnBetOOP3BetPot));
                Assert.That(actual.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToRiverBetIP3BetPot));
                Assert.That(actual.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot, Is.EqualTo(expected.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot), nameof(HudLightIndicators.CheckFlopAsPFRAndFoldToRiverBetOOP3BetPot));
                Assert.That(actual.FoldToTripleBarrelSRP, Is.EqualTo(expected.FoldToTripleBarrelSRP), nameof(HudLightIndicators.FoldToTripleBarrelSRP));
                Assert.That(actual.FoldToTripleBarrel3BetPot, Is.EqualTo(expected.FoldToTripleBarrel3BetPot), nameof(HudLightIndicators.FoldToTripleBarrel3BetPot));
                Assert.That(actual.FoldToTripleBarrel4BetPot, Is.EqualTo(expected.FoldToTripleBarrel4BetPot), nameof(HudLightIndicators.FoldToTripleBarrel4BetPot));
                Assert.That(actual.FoldToDoubleBarrelSRP, Is.EqualTo(expected.FoldToDoubleBarrelSRP), nameof(HudLightIndicators.FoldToDoubleBarrelSRP));
                Assert.That(actual.FoldToDoubleBarrel4BetPot, Is.EqualTo(expected.FoldToDoubleBarrel4BetPot), nameof(HudLightIndicators.FoldToDoubleBarrel4BetPot));
                Assert.That(actual.FoldToCBetSRP, Is.EqualTo(expected.FoldToCBetSRP), nameof(HudLightIndicators.FoldToCBetSRP));
                Assert.That(actual.SBOpenShove1to8bbUOPot, Is.EqualTo(expected.SBOpenShove1to8bbUOPot), nameof(HudLightIndicators.SBOpenShove1to8bbUOPot));
                Assert.That(actual.SBOpenShove9to14bbUOPot, Is.EqualTo(expected.SBOpenShove9to14bbUOPot), nameof(HudLightIndicators.SBOpenShove9to14bbUOPot));
                Assert.That(actual.SBOpenShove15to25bbUOPot, Is.EqualTo(expected.SBOpenShove15to25bbUOPot), nameof(HudLightIndicators.SBOpenShove15to25bbUOPot));
                Assert.That(actual.SBOpenShove26to50bbUOPot, Is.EqualTo(expected.SBOpenShove26to50bbUOPot), nameof(HudLightIndicators.SBOpenShove26to50bbUOPot));
                Assert.That(actual.SBOpenShove51plusbbUOPot, Is.EqualTo(expected.SBOpenShove51plusbbUOPot), nameof(HudLightIndicators.SBOpenShove51plusbbUOPot));
                Assert.That(actual.BTNOpenShove1to8bbUOPot, Is.EqualTo(expected.BTNOpenShove1to8bbUOPot), nameof(HudLightIndicators.BTNOpenShove1to8bbUOPot));
                Assert.That(actual.BTNOpenShove9to14bbUOPot, Is.EqualTo(expected.BTNOpenShove9to14bbUOPot), nameof(HudLightIndicators.BTNOpenShove9to14bbUOPot));
                Assert.That(actual.BTNOpenShove15to25bbUOPot, Is.EqualTo(expected.BTNOpenShove15to25bbUOPot), nameof(HudLightIndicators.BTNOpenShove15to25bbUOPot));
                Assert.That(actual.BTNOpenShove26to50bbUOPot, Is.EqualTo(expected.BTNOpenShove26to50bbUOPot), nameof(HudLightIndicators.BTNOpenShove26to50bbUOPot));
                Assert.That(actual.BTNOpenShove51plusbbUOPot, Is.EqualTo(expected.BTNOpenShove51plusbbUOPot), nameof(HudLightIndicators.BTNOpenShove51plusbbUOPot));
                Assert.That(actual.COOpenShove1to8bbUOPot, Is.EqualTo(expected.COOpenShove1to8bbUOPot), nameof(HudLightIndicators.COOpenShove1to8bbUOPot));
                Assert.That(actual.COOpenShove9to14bbUOPot, Is.EqualTo(expected.COOpenShove9to14bbUOPot), nameof(HudLightIndicators.COOpenShove9to14bbUOPot));
                Assert.That(actual.COOpenShove15to25bbUOPot, Is.EqualTo(expected.COOpenShove15to25bbUOPot), nameof(HudLightIndicators.COOpenShove15to25bbUOPot));
                Assert.That(actual.COOpenShove26to50bbUOPot, Is.EqualTo(expected.COOpenShove26to50bbUOPot), nameof(HudLightIndicators.COOpenShove26to50bbUOPot));
                Assert.That(actual.COOpenShove51plusbbUOPot, Is.EqualTo(expected.COOpenShove51plusbbUOPot), nameof(HudLightIndicators.COOpenShove51plusbbUOPot));
                Assert.That(actual.MPOpenShove1to8bbUOPot, Is.EqualTo(expected.MPOpenShove1to8bbUOPot), nameof(HudLightIndicators.MPOpenShove1to8bbUOPot));
                Assert.That(actual.MPOpenShove9to14bbUOPot, Is.EqualTo(expected.MPOpenShove9to14bbUOPot), nameof(HudLightIndicators.MPOpenShove9to14bbUOPot));
                Assert.That(actual.MPOpenShove15to25bbUOPot, Is.EqualTo(expected.MPOpenShove15to25bbUOPot), nameof(HudLightIndicators.MPOpenShove15to25bbUOPot));
                Assert.That(actual.MPOpenShove26to50bbUOPot, Is.EqualTo(expected.MPOpenShove26to50bbUOPot), nameof(HudLightIndicators.MPOpenShove26to50bbUOPot));
                Assert.That(actual.MPOpenShove51plusbbUOPot, Is.EqualTo(expected.MPOpenShove51plusbbUOPot), nameof(HudLightIndicators.MPOpenShove51plusbbUOPot));
                Assert.That(actual.EPOpenShove1to8bbUOPot, Is.EqualTo(expected.EPOpenShove1to8bbUOPot), nameof(HudLightIndicators.EPOpenShove1to8bbUOPot));
                Assert.That(actual.EPOpenShove9to14bbUOPot, Is.EqualTo(expected.EPOpenShove9to14bbUOPot), nameof(HudLightIndicators.EPOpenShove9to14bbUOPot));
                Assert.That(actual.EPOpenShove15to25bbUOPot, Is.EqualTo(expected.EPOpenShove15to25bbUOPot), nameof(HudLightIndicators.EPOpenShove15to25bbUOPot));
                Assert.That(actual.EPOpenShove26to50bbUOPot, Is.EqualTo(expected.EPOpenShove26to50bbUOPot), nameof(HudLightIndicators.EPOpenShove26to50bbUOPot));
                Assert.That(actual.EPOpenShove51plusbbUOPot, Is.EqualTo(expected.EPOpenShove51plusbbUOPot), nameof(HudLightIndicators.EPOpenShove51plusbbUOPot));
                Assert.That(actual.LimpEPFoldToPFR, Is.EqualTo(expected.LimpEPFoldToPFR), nameof(HudLightIndicators.LimpEPFoldToPFR));
                Assert.That(actual.LimpMPFoldToPFR, Is.EqualTo(expected.LimpMPFoldToPFR), nameof(HudLightIndicators.LimpMPFoldToPFR));
                Assert.That(actual.LimpCOFoldToPFR, Is.EqualTo(expected.LimpCOFoldToPFR), nameof(HudLightIndicators.LimpCOFoldToPFR));
                Assert.That(actual.LimpBTNFoldToPFR, Is.EqualTo(expected.LimpBTNFoldToPFR), nameof(HudLightIndicators.LimpBTNFoldToPFR));
                Assert.That(actual.LimpSBFoldToPFR, Is.EqualTo(expected.LimpSBFoldToPFR), nameof(HudLightIndicators.LimpSBFoldToPFR));
                Assert.That(actual.SBShoveOverLimpers1to8bb, Is.EqualTo(expected.SBShoveOverLimpers1to8bb), nameof(HudLightIndicators.SBShoveOverLimpers1to8bb));
                Assert.That(actual.SBShoveOverLimpers9to14bb, Is.EqualTo(expected.SBShoveOverLimpers9to14bb), nameof(HudLightIndicators.SBShoveOverLimpers9to14bb));
                Assert.That(actual.SBShoveOverLimpers15to25bb, Is.EqualTo(expected.SBShoveOverLimpers15to25bb), nameof(HudLightIndicators.SBShoveOverLimpers15to25bb));
                Assert.That(actual.SBShoveOverLimpers26to50bb, Is.EqualTo(expected.SBShoveOverLimpers26to50bb), nameof(HudLightIndicators.SBShoveOverLimpers26to50bb));
                Assert.That(actual.SBShoveOverLimpers51plusbb, Is.EqualTo(expected.SBShoveOverLimpers51plusbb), nameof(HudLightIndicators.SBShoveOverLimpers51plusbb));
                Assert.That(actual.BTNShoveOverLimpers1to8bb, Is.EqualTo(expected.BTNShoveOverLimpers1to8bb), nameof(HudLightIndicators.BTNShoveOverLimpers1to8bb));
                Assert.That(actual.BTNShoveOverLimpers9to14bb, Is.EqualTo(expected.BTNShoveOverLimpers9to14bb), nameof(HudLightIndicators.BTNShoveOverLimpers9to14bb));
                Assert.That(actual.BTNShoveOverLimpers15to25bb, Is.EqualTo(expected.BTNShoveOverLimpers15to25bb), nameof(HudLightIndicators.BTNShoveOverLimpers15to25bb));
                Assert.That(actual.BTNShoveOverLimpers26to50bb, Is.EqualTo(expected.BTNShoveOverLimpers26to50bb), nameof(HudLightIndicators.BTNShoveOverLimpers26to50bb));
                Assert.That(actual.BTNShoveOverLimpers51plusbb, Is.EqualTo(expected.BTNShoveOverLimpers51plusbb), nameof(HudLightIndicators.BTNShoveOverLimpers51plusbb));
                Assert.That(actual.COShoveOverLimpers1to8bb, Is.EqualTo(expected.COShoveOverLimpers1to8bb), nameof(HudLightIndicators.COShoveOverLimpers1to8bb));
                Assert.That(actual.COShoveOverLimpers9to14bb, Is.EqualTo(expected.COShoveOverLimpers9to14bb), nameof(HudLightIndicators.COShoveOverLimpers9to14bb));
                Assert.That(actual.COShoveOverLimpers15to25bb, Is.EqualTo(expected.COShoveOverLimpers15to25bb), nameof(HudLightIndicators.COShoveOverLimpers15to25bb));
                Assert.That(actual.COShoveOverLimpers26to50bb, Is.EqualTo(expected.COShoveOverLimpers26to50bb), nameof(HudLightIndicators.COShoveOverLimpers26to50bb));
                Assert.That(actual.COShoveOverLimpers51plusbb, Is.EqualTo(expected.COShoveOverLimpers51plusbb), nameof(HudLightIndicators.COShoveOverLimpers51plusbb));
                Assert.That(actual.MPShoveOverLimpers1to8bb, Is.EqualTo(expected.MPShoveOverLimpers1to8bb), nameof(HudLightIndicators.MPShoveOverLimpers1to8bb));
                Assert.That(actual.MPShoveOverLimpers9to14bb, Is.EqualTo(expected.MPShoveOverLimpers9to14bb), nameof(HudLightIndicators.MPShoveOverLimpers9to14bb));
                Assert.That(actual.MPShoveOverLimpers15to25bb, Is.EqualTo(expected.MPShoveOverLimpers15to25bb), nameof(HudLightIndicators.MPShoveOverLimpers15to25bb));
                Assert.That(actual.MPShoveOverLimpers26to50bb, Is.EqualTo(expected.MPShoveOverLimpers26to50bb), nameof(HudLightIndicators.MPShoveOverLimpers26to50bb));
                Assert.That(actual.MPShoveOverLimpers51plusbb, Is.EqualTo(expected.MPShoveOverLimpers51plusbb), nameof(HudLightIndicators.MPShoveOverLimpers51plusbb));
                Assert.That(actual.EPShoveOverLimpers1to8bb, Is.EqualTo(expected.EPShoveOverLimpers1to8bb), nameof(HudLightIndicators.EPShoveOverLimpers1to8bb));
                Assert.That(actual.EPShoveOverLimpers9to14bb, Is.EqualTo(expected.EPShoveOverLimpers9to14bb), nameof(HudLightIndicators.EPShoveOverLimpers9to14bb));
                Assert.That(actual.EPShoveOverLimpers15to25bb, Is.EqualTo(expected.EPShoveOverLimpers15to25bb), nameof(HudLightIndicators.EPShoveOverLimpers15to25bb));
                Assert.That(actual.EPShoveOverLimpers26to50bb, Is.EqualTo(expected.EPShoveOverLimpers26to50bb), nameof(HudLightIndicators.EPShoveOverLimpers26to50bb));
                Assert.That(actual.EPShoveOverLimpers51plusbb, Is.EqualTo(expected.EPShoveOverLimpers51plusbb), nameof(HudLightIndicators.EPShoveOverLimpers51plusbb));
                Assert.That(actual.OpenMinraise, Is.EqualTo(expected.OpenMinraise), nameof(HudLightIndicators.OpenMinraise));
                Assert.That(actual.EPOpenMinraiseUOPFR, Is.EqualTo(expected.EPOpenMinraiseUOPFR), nameof(HudLightIndicators.EPOpenMinraiseUOPFR));
                Assert.That(actual.MPOpenMinraiseUOPFR, Is.EqualTo(expected.MPOpenMinraiseUOPFR), nameof(HudLightIndicators.MPOpenMinraiseUOPFR));
                Assert.That(actual.COOpenMinraiseUOPFR, Is.EqualTo(expected.COOpenMinraiseUOPFR), nameof(HudLightIndicators.COOpenMinraiseUOPFR));
                Assert.That(actual.BTNOpenMinraiseUOPFR, Is.EqualTo(expected.BTNOpenMinraiseUOPFR), nameof(HudLightIndicators.BTNOpenMinraiseUOPFR));
                Assert.That(actual.SBOpenMinraiseUOPFR, Is.EqualTo(expected.SBOpenMinraiseUOPFR), nameof(HudLightIndicators.SBOpenMinraiseUOPFR));
                Assert.That(actual.BBOpenMinraiseUOPFR, Is.EqualTo(expected.BBOpenMinraiseUOPFR), nameof(HudLightIndicators.BBOpenMinraiseUOPFR));
                Assert.That(actual.SqueezeEP, Is.EqualTo(expected.SqueezeEP), nameof(HudLightIndicators.SqueezeEP));
                Assert.That(actual.SqueezeMP, Is.EqualTo(expected.SqueezeMP), nameof(HudLightIndicators.SqueezeMP));
                Assert.That(actual.SqueezeCO, Is.EqualTo(expected.SqueezeCO), nameof(HudLightIndicators.SqueezeCO));
                Assert.That(actual.SqueezeBTN, Is.EqualTo(expected.SqueezeBTN), nameof(HudLightIndicators.SqueezeBTN));
                Assert.That(actual.SqueezeSB, Is.EqualTo(expected.SqueezeSB), nameof(HudLightIndicators.SqueezeSB));
                Assert.That(actual.SqueezeBB, Is.EqualTo(expected.SqueezeBB), nameof(HudLightIndicators.SqueezeBB));
                Assert.That(actual.SqueezeBBVsBTNPFR, Is.EqualTo(expected.SqueezeBBVsBTNPFR), nameof(HudLightIndicators.SqueezeBBVsBTNPFR));
                Assert.That(actual.SqueezeBBVsCOPFR, Is.EqualTo(expected.SqueezeBBVsCOPFR), nameof(HudLightIndicators.SqueezeBBVsCOPFR));
                Assert.That(actual.SqueezeBBVsMPPFR, Is.EqualTo(expected.SqueezeBBVsMPPFR), nameof(HudLightIndicators.SqueezeBBVsMPPFR));
                Assert.That(actual.SqueezeBBVsEPPFR, Is.EqualTo(expected.SqueezeBBVsEPPFR), nameof(HudLightIndicators.SqueezeBBVsEPPFR));
                Assert.That(actual.SqueezeSBVsCOPFR, Is.EqualTo(expected.SqueezeSBVsCOPFR), nameof(HudLightIndicators.SqueezeSBVsCOPFR));
                Assert.That(actual.SqueezeSBVsMPPFR, Is.EqualTo(expected.SqueezeSBVsMPPFR), nameof(HudLightIndicators.SqueezeSBVsMPPFR));
                Assert.That(actual.SqueezeSBVsEPPFR, Is.EqualTo(expected.SqueezeSBVsEPPFR), nameof(HudLightIndicators.SqueezeSBVsEPPFR));
                Assert.That(actual.SqueezeBTNVsMPPFR, Is.EqualTo(expected.SqueezeBTNVsMPPFR), nameof(HudLightIndicators.SqueezeBTNVsMPPFR));
                Assert.That(actual.SqueezeBTNVsEPPFR, Is.EqualTo(expected.SqueezeBTNVsEPPFR), nameof(HudLightIndicators.SqueezeBTNVsEPPFR));
                Assert.That(actual.SqueezeCOVsMPPFR, Is.EqualTo(expected.SqueezeCOVsMPPFR), nameof(HudLightIndicators.SqueezeCOVsMPPFR));
                Assert.That(actual.SqueezeCOVsEPPFR, Is.EqualTo(expected.SqueezeCOVsEPPFR), nameof(HudLightIndicators.SqueezeCOVsEPPFR));
                Assert.That(actual.SqueezeMPVsEPPFR, Is.EqualTo(expected.SqueezeMPVsEPPFR), nameof(HudLightIndicators.SqueezeMPVsEPPFR));
                Assert.That(actual.SqueezeEPVsEPPFR, Is.EqualTo(expected.SqueezeEPVsEPPFR), nameof(HudLightIndicators.SqueezeEPVsEPPFR));
                Assert.That(actual.FoldToSqueezeAsColdCaller, Is.EqualTo(expected.FoldToSqueezeAsColdCaller), nameof(HudLightIndicators.FoldToSqueezeAsColdCaller));
                Assert.That(actual.FourBetVsBlind3Bet, Is.EqualTo(expected.FourBetVsBlind3Bet), nameof(HudLightIndicators.FourBetVsBlind3Bet));
                Assert.That(actual.BTNReStealVsCOSteal, Is.EqualTo(expected.BTNReStealVsCOSteal), nameof(HudLightIndicators.BTNReStealVsCOSteal));
                Assert.That(actual.BTNDefendVsCOSteal, Is.EqualTo(expected.BTNDefendVsCOSteal), nameof(HudLightIndicators.BTNDefendVsCOSteal));
                Assert.That(actual.FoldToStealInSB, Is.EqualTo(expected.FoldToStealInSB), nameof(HudLightIndicators.FoldToStealInSB));
                Assert.That(actual.FoldToStealInBB, Is.EqualTo(expected.FoldToStealInBB), nameof(HudLightIndicators.FoldToStealInBB));
                Assert.That(actual.CalledStealInSB, Is.EqualTo(expected.CalledStealInSB), nameof(HudLightIndicators.CalledStealInSB));
                Assert.That(actual.CalledStealInBB, Is.EqualTo(expected.CalledStealInBB), nameof(HudLightIndicators.CalledStealInBB));
                Assert.That(actual.FoldToBTNStealInSB, Is.EqualTo(expected.FoldToBTNStealInSB), nameof(HudLightIndicators.FoldToBTNStealInSB));
                Assert.That(actual.FoldToBTNStealInBB, Is.EqualTo(expected.FoldToBTNStealInBB), nameof(HudLightIndicators.FoldToBTNStealInBB));
                Assert.That(actual.FoldToCOStealInSB, Is.EqualTo(expected.FoldToCOStealInSB), nameof(HudLightIndicators.FoldToCOStealInSB));
                Assert.That(actual.FoldToCOStealInBB, Is.EqualTo(expected.FoldToCOStealInBB), nameof(HudLightIndicators.FoldToCOStealInBB));
                Assert.That(actual.CalledBTNStealInSB, Is.EqualTo(expected.CalledBTNStealInSB), nameof(HudLightIndicators.CalledBTNStealInSB));
                Assert.That(actual.CalledBTNStealInBB, Is.EqualTo(expected.CalledBTNStealInBB), nameof(HudLightIndicators.CalledBTNStealInBB));
                Assert.That(actual.CalledCOStealInSB, Is.EqualTo(expected.CalledCOStealInSB), nameof(HudLightIndicators.CalledCOStealInSB));
                Assert.That(actual.CalledCOStealInBB, Is.EqualTo(expected.CalledCOStealInBB), nameof(HudLightIndicators.CalledCOStealInBB));
                Assert.That(actual.OvercallBTNStealInBB, Is.EqualTo(expected.OvercallBTNStealInBB), nameof(HudLightIndicators.OvercallBTNStealInBB));
                Assert.That(actual.WTSDAsPFR, Is.EqualTo(expected.WTSDAsPFR), nameof(HudLightIndicators.WTSDAsPFR));
                Assert.That(actual.WTSDAs4Bettor, Is.EqualTo(expected.WTSDAs4Bettor), nameof(HudLightIndicators.WTSDAs4Bettor));
                Assert.That(actual.Call4BetIP, Is.EqualTo(expected.Call4BetIP), nameof(HudLightIndicators.Call4BetIP));
                Assert.That(actual.Call4BetOOP, Is.EqualTo(expected.Call4BetOOP), nameof(HudLightIndicators.Call4BetOOP));
                Assert.That(actual.Call4BetEP, Is.EqualTo(expected.Call4BetEP), nameof(HudLightIndicators.Call4BetEP));
                Assert.That(actual.Call4BetMP, Is.EqualTo(expected.Call4BetMP), nameof(HudLightIndicators.Call4BetMP));
                Assert.That(actual.Call4BetCO, Is.EqualTo(expected.Call4BetCO), nameof(HudLightIndicators.Call4BetCO));
                Assert.That(actual.Call4BetBTN, Is.EqualTo(expected.Call4BetBTN), nameof(HudLightIndicators.Call4BetBTN));
                Assert.That(actual.Call4BetSB, Is.EqualTo(expected.Call4BetSB), nameof(HudLightIndicators.Call4BetSB));
                Assert.That(actual.Call4BetBB, Is.EqualTo(expected.Call4BetBB), nameof(HudLightIndicators.Call4BetBB));
                Assert.That(actual.TotalOverCallSRP, Is.EqualTo(expected.TotalOverCallSRP), nameof(HudLightIndicators.TotalOverCallSRP));
                Assert.That(actual.LimpedPotFlopStealIP, Is.EqualTo(expected.LimpedPotFlopStealIP), nameof(HudLightIndicators.LimpedPotFlopStealIP));
                Assert.That(actual.FlopCheckCall, Is.EqualTo(expected.FlopCheckCall), nameof(HudLightIndicators.FlopCheckCall));
                Assert.That(actual.CallFlopFoldTurn, Is.EqualTo(expected.CallFlopFoldTurn), nameof(HudLightIndicators.CallFlopFoldTurn));
                Assert.That(actual.RiverFoldInSRP, Is.EqualTo(expected.RiverFoldInSRP), nameof(HudLightIndicators.RiverFoldInSRP));
                Assert.That(actual.RiverFoldIn3Bet, Is.EqualTo(expected.RiverFoldIn3Bet), nameof(HudLightIndicators.RiverFoldIn3Bet));
                Assert.That(actual.RiverFoldIn4Bet, Is.EqualTo(expected.RiverFoldIn4Bet), nameof(HudLightIndicators.RiverFoldIn4Bet));
                Assert.That(actual.DelayedTurnCBetInSRP, Is.EqualTo(expected.DelayedTurnCBetInSRP), nameof(HudLightIndicators.DelayedTurnCBetInSRP));
                Assert.That(actual.DelayedTurnCBetIn4BetPot, Is.EqualTo(expected.DelayedTurnCBetIn4BetPot), nameof(HudLightIndicators.DelayedTurnCBetIn4BetPot));
                Assert.That(actual.CheckRaiseFlopAsPFRInSRP, Is.EqualTo(expected.CheckRaiseFlopAsPFRInSRP), nameof(HudLightIndicators.CheckRaiseFlopAsPFRInSRP));
                Assert.That(actual.CheckRaiseFlopAsPFRIn3BetPot, Is.EqualTo(expected.CheckRaiseFlopAsPFRIn3BetPot), nameof(HudLightIndicators.CheckRaiseFlopAsPFRIn3BetPot));
                Assert.That(actual.OpenLimpEP, Is.EqualTo(expected.OpenLimpEP), nameof(HudLightIndicators.OpenLimpEP));
                Assert.That(actual.OpenLimpMP, Is.EqualTo(expected.OpenLimpMP), nameof(HudLightIndicators.OpenLimpMP));
                Assert.That(actual.OpenLimpCO, Is.EqualTo(expected.OpenLimpCO), nameof(HudLightIndicators.OpenLimpCO));
                Assert.That(actual.OpenLimpBTN, Is.EqualTo(expected.OpenLimpBTN), nameof(HudLightIndicators.OpenLimpBTN));
                Assert.That(actual.OpenLimpSB, Is.EqualTo(expected.OpenLimpSB), nameof(HudLightIndicators.OpenLimpSB));
                Assert.That(actual.CheckInStraddle, Is.EqualTo(expected.CheckInStraddle), nameof(HudLightIndicators.CheckInStraddle));
                Assert.That(actual.PFRInStraddle, Is.EqualTo(expected.PFRInStraddle), nameof(HudLightIndicators.PFRInStraddle));
                Assert.That(actual.ThreeBetInStraddle, Is.EqualTo(expected.ThreeBetInStraddle), nameof(HudLightIndicators.ThreeBetInStraddle));
                Assert.That(actual.FourBetInStraddle, Is.EqualTo(expected.FourBetInStraddle), nameof(HudLightIndicators.FourBetInStraddle));
                Assert.That(actual.FoldInStraddle, Is.EqualTo(expected.FoldInStraddle), nameof(HudLightIndicators.FoldInStraddle));
                Assert.That(actual.WTSDInStraddle, Is.EqualTo(expected.WTSDInStraddle), nameof(HudLightIndicators.WTSDInStraddle));
                Assert.That(actual.SkipFlopCBetInSRPandCheckFoldFlopOOP, Is.EqualTo(expected.SkipFlopCBetInSRPandCheckFoldFlopOOP), nameof(HudLightIndicators.SkipFlopCBetInSRPandCheckFoldFlopOOP));                
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
 