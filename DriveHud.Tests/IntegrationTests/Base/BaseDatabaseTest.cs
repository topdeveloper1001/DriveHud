//-----------------------------------------------------------------------
// <copyright file="SessionFactoryService.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Importers;
using DriveHUD.Importers.Loggers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.IO;

namespace DriveHud.Tests.IntegrationTests.Base
{
    public class BaseDatabaseTest
    {
        protected IDHLog customLogger;

        protected virtual void Initalize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            InitializeDatabase();
            InitializeEnvironment();
            ConfigureCustomLogger();
        }

        protected virtual void InitializeEnvironment()
        {
            var unityContainer = new UnityContainer();

            InitializeImporterSessionCacheService(unityContainer);
            InitializeDataService(unityContainer);
            InitializeSessionFactoryService(unityContainer);
            InitializeHandHistoryParserFactory(unityContainer);
            InitializePlayerStatisticCalculator(unityContainer);
            InitializeFileImporterLogger(unityContainer);
            InitializeLicenseService(unityContainer);
            InitializeSessionService(unityContainer);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
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
                            "HandRecords",
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

        protected virtual void ConfigureCustomLogger()
        {
            customLogger = Substitute.For<IDHLog>();
            LogProvider.SetCustomLogger(customLogger);
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
            unityContainer.RegisterType<IPlayerStatisticCalculator, PlayerStatisticCalculator>();
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

        #endregion
    }
}