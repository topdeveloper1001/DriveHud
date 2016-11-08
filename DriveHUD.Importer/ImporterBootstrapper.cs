﻿//-----------------------------------------------------------------------
// <copyright file="ImporterBootstrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Builders.iPoker;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.PokerStars;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using System;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Importer bootstrapper to allow us to make all interfaces and all classes internal
    /// </summary>
    public static class ImporterBootstrapper
    {
        public static void ConfigureImporter(IUnityContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.RegisterType<IBovadaCatcher, BovadaCatcher>();
            container.RegisterType<IBovadaDataManager, BovadaDataManager>();
            container.RegisterType<IBovadaImporter, BovadaImporter>();
            container.RegisterType<IBetOnlineCatcher, BetOnlineCatcher>();
            container.RegisterType<IBetOnlineDataManager, BetOnlineDataManager>();
            container.RegisterType<IBetOnlineImporter, BetOnlineImporter>();
            container.RegisterType<IBetOnlineTableService, BetOnlineTableService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBetOnlineTournamentImporter, BetOnlineTournamentImporter>();
            container.RegisterType<IBetOnlineTournamentManager, BetOnlineTournamentManager>();
            container.RegisterType<IBetOnlineXmlConverter, BetOnlineXmlToIPokerXmlConverter>();
            container.RegisterType<IPokerStarsImporter, PokerStarsImporter>();
            container.RegisterType<ICardsConverter, PokerCardsConverter>();
            container.RegisterType<ITournamentsCacheService, TournamentsCacheService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImporterService, ImporterService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPokerClientEncryptedLogger, PokerClientLogger>();
            container.RegisterType<IImporterSessionCacheService, ImporterSessionCacheService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileImporter, FileImporter>();
            container.RegisterType<IFileImporterLogger, FileImporterLogger>();

            // Loggers
            container.RegisterType<IPokerClientEncryptedLogger, PokerClientLogger>(LogServices.Base.ToString());
            container.RegisterType<IPokerClientEncryptedLogger, BetOnlineTournamentLogger>(LogServices.BetOnlineTournament.ToString());
        }

        public static void ConfigureImporterService()
        {
            var importerService = ServiceLocator.Current.GetInstance<IImporterService>() as IImporterInternalService;

            if (importerService == null)
            {
                throw new InvalidCastException("Importers could not be registered");
            }

            importerService.Register<IBovadaCatcher>();
            importerService.Register<IBovadaImporter>();
            importerService.Register<IBetOnlineCatcher>();
            importerService.Register<IBetOnlineImporter>();
            importerService.Register<IBetOnlineTournamentImporter>();
            importerService.Register<IPokerStarsImporter>();
        }
    }
}