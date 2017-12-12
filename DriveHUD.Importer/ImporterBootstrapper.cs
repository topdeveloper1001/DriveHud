//-----------------------------------------------------------------------
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
using DriveHUD.Importers.ExternalImporter;
using DriveHUD.Importers.IPoker;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.Pacific888;
using DriveHUD.Importers.PartyPoker;
using DriveHUD.Importers.PokerStars;
using DriveHUD.Importers.WinningPokerNetwork;
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

            container.RegisterType<IPipeManager, PipeManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IIgnitionCatcher, IgnitionCatcher>();
            container.RegisterType<IIgnitionImporter, IgnitionImporter>();
            container.RegisterType<IIgnitionInfoImporter, IgnitionInfoImporter>();
            container.RegisterType<IIgnitionDataManager, IgnitionDataManager>();
            container.RegisterType<IIgnitionInfoDataManager, IgnitionInfoDataManager>();
            container.RegisterType<IBetOnlineCatcher, BetOnlineCatcher>();
            container.RegisterType<IBetOnlineDataManager, BetOnlineDataManager>();
            container.RegisterType<IBetOnlineImporter, BetOnlineImporter>();
            container.RegisterType<IBetOnlineTableService, BetOnlineTableService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IBetOnlineTournamentImporter, BetOnlineTournamentImporter>();
            container.RegisterType<IBetOnlineTournamentManager, BetOnlineTournamentManager>();
            container.RegisterType<IBetOnlineXmlConverter, BetOnlineXmlToIPokerXmlConverter>();
            container.RegisterType<IPokerStarsImporter, PokerStarsImporter>();
            container.RegisterType<IPokerStarsZoomCatcher, PokerStarsZoomCatcher>();
            container.RegisterType<IPokerStarsZoomImporter, PokerStarsZoomImporter>();
            container.RegisterType<IPokerStarsZoomDataManager, PokerStarsZoomDataManager>();
            container.RegisterType<IAmericasCardroomImporter, AmericasCardroomImporter>();
            container.RegisterType<IBlackChipPokerImporter, BlackChipPokerImporter>();
            container.RegisterType<ITruePokerImporter, TruePokerImporter>();
            container.RegisterType<IYaPokerImporter, YaPokerImporter>();
            container.RegisterType<IPacific888Importer, Pacific888Importer>();
            container.RegisterType<IPartyPokerImporter, PartyPokerImporter>();
            container.RegisterType<ICardsConverter, PokerCardsConverter>();
            container.RegisterType<IIPokerImporter, IPokerImporter>();
            container.RegisterType<ITournamentsCacheService, TournamentsCacheService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IImporterService, ImporterService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPokerClientEncryptedLogger, PokerClientLogger>();
            container.RegisterType<IImporterSessionCacheService, ImporterSessionCacheService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IFileImporter, FileImporter>();
            container.RegisterType<IFileImporterLogger, FileImporterLogger>();
            container.RegisterType<IPlayerStatisticReImporter, PlayerStatisticReImporter>();
            container.RegisterType<IIgnitionWindowCache, IgnitionWindowCache>(new ContainerControlledLifetimeManager());
            container.RegisterType<IExternalImporter, ExternalImporter.ExternalImporter>();

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

            //importerService.Register<IIgnitionCatcher>();
            //importerService.Register<IIgnitionImporter>();
            //importerService.Register<IIgnitionInfoImporter>();
            //importerService.Register<IBetOnlineCatcher>();
            //importerService.Register<IBetOnlineImporter>();
            //importerService.Register<IBetOnlineTournamentImporter>();
            //importerService.Register<IBetOnlineTableService>();
            //importerService.Register<IPokerStarsImporter>();
            //importerService.Register<IPokerStarsZoomCatcher>();
            //importerService.Register<IPokerStarsZoomImporter>();
            //importerService.Register<IAmericasCardroomImporter>();
            //importerService.Register<IBlackChipPokerImporter>();
            //importerService.Register<ITruePokerImporter>();
            //importerService.Register<IYaPokerImporter>();
            //importerService.Register<IPacific888Importer>();
            //importerService.Register<IPartyPokerImporter>();
            //importerService.Register<IIPokerImporter>();
            importerService.Register<IExternalImporter>();
        }
    }
}