﻿//-----------------------------------------------------------------------
// <copyright file="BaseDatabaseTest.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Licensing;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.Builders.iPoker;
using DriveHUD.Importers.Loggers;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.Unity;
using Model;
using Model.Enums;
using Model.Interfaces;
using Model.Reports;
using Model.Solvers;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.IntegrationTests.Base
{
    class BaseDatabaseTest : BaseIntegrationTest
    {
        private const string testDataFolder = @"..\..\IntegrationTests\Importers\TestData";

        protected override string TestDataFolder
        {
            get
            {
                return testDataFolder;
            }
        }

        protected override void Initalize()
        {
            base.Initalize();
            InitializeDatabase();
        }

        protected override void InitializeContainer(UnityContainer unityContainer)
        {
            InitializeImporterSessionCacheService(unityContainer);
            InitializeDataService(unityContainer);
            InitializePlayerStatisticRepository(unityContainer);
            InitializeSessionFactoryService(unityContainer);
            InitializeHandHistoryParserFactory(unityContainer);
            InitializePlayerStatisticCalculator(unityContainer);
            InitializeFileImporterLogger(unityContainer);
            InitializeLicenseService(unityContainer);
            InitializeSessionService(unityContainer);
            InitializeEventAggregator(unityContainer);
            InitializeOpponentReportService(unityContainer);
            InitializeEquityResolver(unityContainer);
            InitializeReportStatusService(unityContainer);
            InitializeResources();
        }

        protected virtual void InitializeDatabase()
        {
            RemoveDatabase();
            CopyDatabase();
        }

        protected virtual void CopyDatabase()
        {
            var resourcesAssembly = typeof(ResourceRegistrator).Assembly;

            using (var stream = resourcesAssembly.GetManifestResourceStream(DatabaseHelper.DatabaseResource))
            {
                using (var fs = File.OpenWrite(DatabaseHelper.DatabaseFile))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fs);
                }
            }
        }

        protected virtual void RemoveDatabase()
        {
            if (File.Exists(DatabaseHelper.DatabaseFile))
            {
                File.Delete(DatabaseHelper.DatabaseFile);
            }
        }

        protected void CleanUpDatabase()
        {
            using (var perfScope = new PerformanceMonitor("CleanUpDatabase"))
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        // order is important because of foreign keys
                        var tablesToCleanUp = new string[]
                        {
                            "Tournaments",
                            "HandNotes",
                            "PlayerGameInfo",
                            "HandsPlayers",
                            "PlayerNotes",
                            "HandHistories",
                            "GameInfo",
                            "Players",
                        };

                        tablesToCleanUp.ForEach(x => session.CreateSQLQuery($"DELETE FROM \"{x}\"").ExecuteUpdate());

                        transaction.Commit();
                    }
                }
            }
        }

        /// <summary>        
        /// Fills the database with the data from the specified hand history file
        /// </summary>        
        /// <param name="fileName">File with hh</param>
        /// <param name="pokerSite">Site</param>
        /// <returns>The result of importing</returns>
        protected virtual IEnumerable<ParsingResult> FillDatabaseFromSingleFile(string fileName, EnumPokerSites pokerSite)
        {
            return FillDatabaseFromSingleFile(fileName, pokerSite, new BasicFileTestImporter());
        }

        /// <summary>        
        /// Fills the database with the data from the specified hand history file
        /// </summary>        
        /// <param name="fileName">File with hh</param>
        /// <param name="pokerSite">Site</param>
        /// <param name="fileTestImporter">Importer</param>
        /// <returns>The result of importing</returns>
        protected virtual IEnumerable<ParsingResult> FillDatabaseFromSingleFile(string fileName, EnumPokerSites pokerSite, IFileTestImporter fileTestImporter)
        {
            using (var perfScope = new PerformanceMonitor("FillDatabaseFromSingleFile"))
            {
                var fileImporter = new TestFileImporter();

                var handHistoryFileFullName = Path.Combine(TestDataFolder, fileName);

                var handHistoryFileInfo = new FileInfo(handHistoryFileFullName);

                Assert.That(handHistoryFileInfo.Exists, $"{handHistoryFileFullName} doesn't exists. Please check.");

                var handHistoryText = File.ReadAllText(handHistoryFileInfo.FullName);

                var gameInfo = new GameInfo
                {
                    PokerSite = pokerSite,
                    FileName = handHistoryFileInfo.FullName
                };

                return fileTestImporter.Import(fileImporter, handHistoryText, gameInfo);
            }
        }

        #region Initializers

        protected virtual void InitializeImporterSessionCacheService(UnityContainer unityContainer)
        {
            var importerSessionCacheService = Substitute.For<IImporterSessionCacheService>();
            unityContainer.RegisterInstance(importerSessionCacheService);
        }

        protected virtual void InitializeDataService(UnityContainer unityContainer)
        {
            var dataService = Substitute.For<IDataService>();
            unityContainer.RegisterInstance(dataService);
        }

        protected virtual void InitializePlayerStatisticRepository(UnityContainer unityContainer)
        {
            var playerStatisticRepository = Substitute.For<IPlayerStatisticRepository>();
            unityContainer.RegisterInstance(playerStatisticRepository);
        }

        protected virtual void InitializeSessionFactoryService(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<ISessionFactoryService, SessionFactoryTestService>(new ContainerControlledLifetimeManager());
        }

        protected virtual void InitializeHandHistoryParserFactory(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IHandHistoryParserFactory, HandHistoryParserFactoryImpl>();
        }

        protected virtual void InitializePlayerStatisticCalculator(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IPlayerStatisticCalculator, PlayerStatisticCalculator>(PlayerStatisticCalculatorServiceNames.Base);
            unityContainer.RegisterType<IPlayerStatisticCalculator, PKShortPlayerStatisticCalculator>(PlayerStatisticCalculatorServiceNames.PKShortDeck);
        }

        protected virtual void InitializeFileImporterLogger(UnityContainer unityContainer)
        {
            var logger = Substitute.For<IFileImporterLogger>();
            unityContainer.RegisterInstance(logger);
        }

        protected virtual void InitializeLicenseService(UnityContainer unityContainer)
        {
            var licenceService = Substitute.For<ILicenseService>();
            unityContainer.RegisterInstance(licenceService);
        }

        protected virtual void InitializeSessionService(UnityContainer unityContainer)
        {
            var sessionService = Substitute.For<ISessionService>();
            unityContainer.RegisterInstance(sessionService);

            var userSession = Substitute.For<IUserSession>();
            userSession.IsMatch(Arg.Any<GameMatchInfo>()).Returns(true);

            sessionService.GetUserSession().Returns(userSession);
        }

        protected virtual void InitializeEventAggregator(UnityContainer unityContainer)
        {
            var eventAggregator = Substitute.For<IEventAggregator>();
            eventAggregator.GetEvent<PlayersAddedEvent>().ReturnsForAnyArgs(new PlayersAddedEvent());
            unityContainer.RegisterInstance(eventAggregator);
        }

        protected virtual void InitializeOpponentReportService(UnityContainer unityContainer)
        {
            var opponentReportService = Substitute.For<IOpponentReportService>();
            unityContainer.RegisterInstance(opponentReportService);
        }

        protected virtual void InitializeReportStatusService(UnityContainer unityContainer)
        {
            var reportStatusService = Substitute.For<IReportStatusService>();
            unityContainer.RegisterInstance(reportStatusService);
        }

        protected virtual void InitializeResources()
        {
            ResourceRegistrator.Initialization();
        }

        protected virtual void InitializeEquityResolver(UnityContainer unityContainer)
        {
            unityContainer.RegisterType<IEquitySolver, EquitySolver>();
            unityContainer.RegisterType<IPokerEvaluator, HoldemEvaluator>(GeneralGameTypeEnum.Holdem.ToString());
            unityContainer.RegisterType<IPokerEvaluator, OmahaEvaluator>(GeneralGameTypeEnum.Omaha.ToString());
            unityContainer.RegisterType<IPokerEvaluator, OmahaHiLoEvaluator>(GeneralGameTypeEnum.OmahaHiLo.ToString());
        }

        #endregion

        private class TestFileImporter : FileImporter
        {
            protected override void StorePlayerStatistic(Playerstatistic playerStat, bool isAsync)
            {
                base.StorePlayerStatistic(playerStat, false);
            }
        }
    }
}