﻿//-----------------------------------------------------------------------
// <copyright file="FileImporterTests.cs" company="Ace Poker Solutions">
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
using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Entities;
using Model;
using NHibernate.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;

namespace DriveHud.Tests.IntegrationTests.Importers
{
    /// <summary>
    /// Importer integration tests
    /// </summary>
    [TestFixture]
    class FileImporterTests : BaseDatabaseTest
    {
        /// <summary>
        /// Initialize environment for test
        /// </summary>
        [OneTimeSetUp]
        public void SetUp()
        {
            Initalize();
            FillDatabase();
        }

        /// <summary>
        /// Freeing resources
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            RemoveDatabase();
        }

        [Test]
        [TestCase("6995792", 9)]
        [TestCase("8034884", 9)]
        [TestCase("8043170", 9)]
        [TestCase("5944035303", 6)]
        [TestCase("5569123611", 10)]
        public void TournamentsAreImportedForEachPlayer(string tournamentNumber, int expectedCount)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsAreImportedForEachPlayer"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournaments = session.Query<Tournaments>()
                        .Where(x => x.Tourneynumber == tournamentNumber)
                        .ToList();

                    Assert.That(tournaments.Count, Is.EqualTo(expectedCount));
                }
            }
        }

        [Test]
        [TestCase("6995792", "Peon_84", 7)]
        [TestCase("8034884", "Peon_84", 3)]
        [TestCase("8043170", "Peon_84", 7)]
        [TestCase("6995792", "pkslcd13", 9)]
        [TestCase("6995792", "Maschris", 8)]
        [TestCase("5944035303", "WhiteRiderT", 1)]
        [TestCase("5944035303", "BOLL1X", 3)]
        [TestCase("5944035303", "AntoniAG9", 2)]
        [TestCase("5944035303", "Martinoq", 4)]
        [TestCase("5944035303", "FotisTheGreek", 6)]
        [TestCase("5569123611", "WhiteRiderT", 2)]
        [TestCase("5569123611", "ThaGhostGoose", 1)]
        [TestCase("5569123611", "yako70", 3)]
        [TestCase("1705825174", "Peon347", 2560)]
        public void TournamentsFinishPositionIsImported(string tournamentNumber, string playerName, int expectedFinishPosition)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsPlacesAreImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Finishposition, Is.EqualTo(expectedFinishPosition),
                        $"Tournament finish position doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
                }
            }
        }

        [Test]
        [TestCase("5569123611", "ThaGhostGoose", 100)]
        [TestCase("5569123611", "WhiteRiderT", 60)]
        [TestCase("5569123611", "yako70", 40)]
        [TestCase("5944035303", "WhiteRiderT", 42)]
        [TestCase("5944035303", "BOLL1X", 0)]
        [TestCase("5944035303", "AntoniAG9", 18)]
        public void TournamentsWinIsImported(string tournamentNumber, string playerName, int winningInCents)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsWinIsImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Winningsincents, Is.EqualTo(winningInCents),
                        $"Tournament win amount doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
                }
            }
        }

        [Test]
        [TestCase("6995792", "Peon_84", 150)]
        [TestCase("8034884", "Peon_84", 500)]
        [TestCase("8043170", "Peon_84", 500)]
        [TestCase("5944035303", "BOLL1X", 10)]
        [TestCase("5569123611", "yako70", 20)]
        [TestCase("1705825174", "Peon347", 0)]
        public void TournamentsBuyinIsImported(string tournamentNumber, string playerName, int buyInInCents)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsBuyinIsImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Buyinincents, Is.EqualTo(buyInInCents),
                        $"Tournament buyin amount doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
                }
            }
        }

        [Test]
        [TestCase("6995792", "Peon_84", 15)]
        [TestCase("8034884", "Peon_84", 50)]
        [TestCase("8043170", "Peon_84", 50)]
        [TestCase("5944035303", "BOLL1X", 2)]
        [TestCase("5569123611", "yako70", 4)]
        [TestCase("1705825174", "Peon347", 0)]
        public void TournamentsRakeIsImported(string tournamentNumber, string playerName, int rakeInInCents)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsRakeIsImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Rakeincents, Is.EqualTo(rakeInInCents),
                        $"Tournament rake amount doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
                }
            }
        }

        [Test]
        [TestCase("6995792", "Peon_84", 1500)]
        [TestCase("8034884", "Peon_84", 1500)]
        [TestCase("8043170", "Peon_84", 1500)]
        [TestCase("5944035303", "BOLL1X", 1500)]
        [TestCase("5569123611", "yako70", 1500)]
        [TestCase("1705825174", "Peon347", 1000)]
        public void TournamentsStartingStackSizeIsImported(string tournamentNumber, string playerName, int startingStackSizeInChips)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsStartingStackSizeIsImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Startingstacksizeinchips, Is.EqualTo(startingStackSizeInChips),
                        $"Tournament starting stack size doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
                }
            }
        }

        [Test, Order(1)]
        public void ImporterDoesntThrowExceptions()
        {
            customLogger.DidNotReceive()
                      .Log(Arg.Any<Type>(), Arg.Any<object>(), Arg.Any<Exception>(),
                              Arg.Is<LogMessageType>(x => x == LogMessageType.Error));
        }

        /// <summary>
        /// Fills database with data from test hand history files
        /// </summary>
        protected virtual void FillDatabase()
        {
            using (var perfScope = new PerformanceMonitor("FillDatabase"))
            {
                var progress = Substitute.For<IDHProgress>();

                foreach (var testCase in TestCaseDataSet)
                {
                    FillDatabaseFromSingleFile(testCase.Item1, testCase.Item2);
                }
            }
        }

        /// <summary>
        /// Set of hh files to fill DB, summaries must follow normal hh, summary data must be added for WPN hh
        /// </summary>
        private Tuple<string, EnumPokerSites>[] TestCaseDataSet = new Tuple<string, EnumPokerSites>[]
        {
            Tuple.Create(@"WinningPokerNetwork\HH20170216 T6995792-G39795657.txt", EnumPokerSites.Unknown),
            Tuple.Create(@"iPoker\NLH-6-max-5944035303.xml", EnumPokerSites.BetOnline),
            Tuple.Create(@"iPoker\NLH-9-max-5569123611.xml", EnumPokerSites.BetOnline),
            Tuple.Create(@"PokerStars\HH20161206 T1705825174 No Limit Hold'em Freeroll.txt", EnumPokerSites.Unknown),
            Tuple.Create(@"PokerStars\TS20161206 T1705825174 No Limit Hold'em Freeroll.txt", EnumPokerSites.Unknown),
            Tuple.Create(@"WinningPokerNetwork\20170507_20170511_Sng2HH.txt", EnumPokerSites.Unknown)
        };
    }
}