//-----------------------------------------------------------------------
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

using DriveHud.Tests.IntegrationTests.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Entities;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Builders.iPoker;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Settings;
using Model.Site;
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

        protected override void InitializeContainer(UnityContainer unityContainer)
        {
            base.InitializeContainer(unityContainer);

            unityContainer.RegisterType<ICardsConverter, PokerCardsConverter>();
            unityContainer.RegisterType<ITournamentsCacheService, TournamentsCacheService>();
            unityContainer.RegisterType<IBetOnlineTableService, BetOnlineTableServiceStub>();
            unityContainer.RegisterType<ISiteConfiguration, BovadaConfiguration>(EnumPokerSites.Ignition.ToString());
            unityContainer.RegisterType<ISiteConfiguration, BetOnlineConfiguration>(EnumPokerSites.BetOnline.ToString());
            unityContainer.RegisterType<ISiteConfiguration, TigerGamingConfiguration>(EnumPokerSites.TigerGaming.ToString());
            unityContainer.RegisterType<ISiteConfiguration, SportsBettingConfiguration>(EnumPokerSites.SportsBetting.ToString());
            unityContainer.RegisterType<ISiteConfiguration, PokerStarsConfiguration>(EnumPokerSites.PokerStars.ToString());
            unityContainer.RegisterType<ISiteConfiguration, Poker888Configuration>(EnumPokerSites.Poker888.ToString());
            unityContainer.RegisterType<ISiteConfiguration, AmericasCardroomConfiguration>(EnumPokerSites.AmericasCardroom.ToString());
            unityContainer.RegisterType<ISiteConfiguration, BlackChipPokerConfiguration>(EnumPokerSites.BlackChipPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, TruePokerConfiguration>(EnumPokerSites.TruePoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, YaPokerConfiguration>(EnumPokerSites.YaPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, PartyPokerConfiguration>(EnumPokerSites.PartyPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, IPokerConfiguration>(EnumPokerSites.IPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, HorizonConfiguration>(EnumPokerSites.Horizon.ToString());
            unityContainer.RegisterType<ISiteConfiguration, WinamaxConfiguration>(EnumPokerSites.Winamax.ToString());
            unityContainer.RegisterType<ISiteConfigurationService, SiteConfigurationService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));
        }

        protected override void Initalize()
        {
            base.Initalize();

            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configurationService.Initialize();
        }

        [Test]
        [TestCase("6995792", 9)]
        [TestCase("8034884", 9)]
        [TestCase("8043170", 9)]
        [TestCase("5944035303", 6)]
        [TestCase("5569123611", 10)]
        [TestCase("16612340", 3)]
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
        [TestCase("910226258", "Peon384", 1)]
        [TestCase("125460058", "Justfold88", 1)]
        [TestCase("102233028", "Peon84", 1)]
        [TestCase("8192538", "Granny_Annie", 2)]
        [TestCase("918178286", "Peon384", 1)]
        [TestCase("2450627306", "Peon84", 2)]
        [TestCase("2450627306", "koko55", 1)]
        [TestCase("16612340", "AlexTh", 1)]
        [TestCase("16612276", "AlexTh", 93)]        
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
        [TestCase("910226258", "Peon384", 18)]
        [TestCase("125460058", "Justfold88", 2000)]
        [TestCase("102233028", "Peon84", 505)]
        [TestCase("102233028", "Ironbark", 299)]
        [TestCase("101810121", "ace3d", 6000)]
        [TestCase("101810121", "Rjg4498", 0)]
        [TestCase("918178286", "Peon384", 36)]
        [TestCase("918178286", "anhanga", 21)]
        [TestCase("918178286", "CVETANKA71", 14)]
        [TestCase("16612340", "AlexTh", 66)]
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
        [TestCase("125460058", "Justfold88", 1000)]
        [TestCase("102233028", "Peon84", 134)]
        [TestCase("16612340", "AlexTh", 22)]
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
        [TestCase("125460058", "Justfold88", 50)]
        [TestCase("102233028", "Peon84", 16)]
        [TestCase("16612340", "AlexTh", 3)]
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
        [TestCase("16612340", "AlexTh", 500)]
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

        [Test]
        [TestCase("101810121", "ace3d", 3)]
        [TestCase("16612340", "AlexTh", 3)]
        public void TournamentsTableSizeIsImported(string tournamentNumber, string playerName, int tableSize)
        {
            using (var perfScope = new PerformanceMonitor("TournamentsTableSizeIsImported"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    var tournament = session.Query<Tournaments>()
                        .SingleOrDefault(x => x.Tourneynumber == tournamentNumber && x.Player.Playername == playerName);

                    Assert.IsNotNull(tournament, $"Tournament data for '{playerName}' has not been found in tournament '{tournamentNumber}'");

                    Assert.That(tournament.Tablesize, Is.EqualTo(tableSize),
                        $"Tournament table size doesn't match for '{playerName}' in tournament '{tournamentNumber}' [{(EnumPokerSites)tournament.SiteId}]");
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
                    if (testCase.Item3 == null)
                    {
                        FillDatabaseFromSingleFile(testCase.Item1, testCase.Item2);
                    }
                    else
                    {
                        FillDatabaseFromSingleFile(testCase.Item1, testCase.Item2, testCase.Item3);
                    }
                }
            }
        }

        /// <summary>
        /// Set of hh files to fill DB, summaries must follow normal hh, summary data must be added for WPN hh
        /// </summary>
        private Tuple<string, EnumPokerSites, IFileTestImporter>[] TestCaseDataSet = new Tuple<string, EnumPokerSites, IFileTestImporter>[]
        {
            Tuple.Create(@"BetOnline\SNG-4-max-playmoney.xml", EnumPokerSites.BetOnline, (IFileTestImporter)(new BetOnlineRawFileTestImporter())),
            Tuple.Create(@"WinningPokerNetwork\HH20170216 T6995792-G39795657.txt", EnumPokerSites.Unknown, (IFileTestImporter)null),
            Tuple.Create(@"WinningPokerNetwork\HH20180107 T8192538-G39795657.txt", EnumPokerSites.AmericasCardroom, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-6-max-5944035303.xml", EnumPokerSites.BetOnline, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-9-max-5569123611.xml", EnumPokerSites.BetOnline, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-6-max-DON-6732774762.xml", EnumPokerSites.IPoker, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-2-max-125460058.xml", EnumPokerSites.BetOnline, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-6-max-102233028.xml", EnumPokerSites.BetOnline, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-3-max-Windfall-101810121.xml", EnumPokerSites.BetOnline, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-Zone-many-players.xml", EnumPokerSites.Ignition, (IFileTestImporter)null),
            Tuple.Create(@"iPoker\NLH-9-max-6780120497.xml", EnumPokerSites.IPoker, (IFileTestImporter)null),
            Tuple.Create(@"PokerStars\HH20161206 T1705825174 No Limit Hold'em Freeroll.txt", EnumPokerSites.Unknown, (IFileTestImporter)null),
            Tuple.Create(@"PokerStars\TS20161206 T1705825174 No Limit Hold'em Freeroll.txt", EnumPokerSites.Unknown, (IFileTestImporter)null),
            Tuple.Create(@"WinningPokerNetwork\20170507_20170511_Sng2HH.txt", EnumPokerSites.Unknown, (IFileTestImporter)null),
            Tuple.Create(@"Horizon\HH20180420 The Colosseum - $100 GTD T16612276.txt", EnumPokerSites.Unknown, (IFileTestImporter)null),
            Tuple.Create(@"Horizon\HH20180420 Hyper Turbo NLH 3-max - $0.25 T16612340.txt", EnumPokerSites.Unknown, (IFileTestImporter)null)            
        };

        private class TestDataSet
        {
            public string File { get; set; }

            public EnumPokerSites Site { get; set; }
        }
    }
}