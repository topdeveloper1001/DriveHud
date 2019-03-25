//-----------------------------------------------------------------------
// <copyright file="ImporterBootstrapper.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Importers.Adda52;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Builders.iPoker;
using DriveHUD.Importers.ExternalImporter;
using DriveHUD.Importers.Horizon;
using DriveHUD.Importers.IPoker;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.Pacific888;
using DriveHUD.Importers.PartyPoker;
using DriveHUD.Importers.PokerBaazi;
using DriveHUD.Importers.PokerKing;
using DriveHUD.Importers.PokerKing.Model;
using DriveHUD.Importers.PokerMaster;
using DriveHUD.Importers.PokerMaster.Model;
using DriveHUD.Importers.PokerStars;
using DriveHUD.Importers.ProxyBase;
using DriveHUD.Importers.Winamax;
using DriveHUD.Importers.WinningPokerNetwork;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Enums;
using Model.Export;
using Model.Solvers;
using System;
using System.Collections.Generic;
using System.Reflection;

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
            container.RegisterType<IPokerStarsZoomImporter, PokerStarsZoomImporter>();
            container.RegisterType<IPokerStarsZoomDataManager, PokerStarsZoomDataManager>();
            container.RegisterType<IAmericasCardroomImporter, AmericasCardroomImporter>();
            container.RegisterType<IBlackChipPokerImporter, BlackChipPokerImporter>();
            container.RegisterType<ITruePokerImporter, TruePokerImporter>();
            container.RegisterType<IYaPokerImporter, YaPokerImporter>();
            container.RegisterType<IWPNImporter, WPNImporter>();
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
            container.RegisterType<ITcpImporter, TcpImporter>();
            container.RegisterType<IHandBuilder, HandBuilder>();
            container.RegisterType<IPKHandBuilder, PKHandBuilder>();
            container.RegisterType<INetworkConnectionsService, NetworkConnectionsService>();
            container.RegisterType<IEmulatorService, EmulatorService>();
            container.RegisterType<IPacketManager<PokerMasterPackage>, PokerMasterPacketManager>();
            container.RegisterType<IPacketManager<PokerKingPackage>, PokerKingPacketManager>();
            container.RegisterType<IPacketManager<Adda52Package>, Adda52PacketManager>();
            container.RegisterType<IPKImporter, PKImporter>();
            container.RegisterType<IPackageBuilder<PokerMasterPackage>, PokerMasterPackageBuilder>();
            container.RegisterType<IPackageBuilder<PokerKingPackage>, PokerKingPackageBuilder>();
            container.RegisterType<IPackageBuilder<Adda52Package>, Adda52PackageBuilder>();
            container.RegisterType<IHorizonImporter, HorizonImporter>();
            container.RegisterType<IWinamaxImporter, WinamaxImporter>();
            container.RegisterType<IPokerEvaluator, HoldemEvaluator>(GeneralGameTypeEnum.Holdem.ToString());
            container.RegisterType<IPokerEvaluator, OmahaEvaluator>(GeneralGameTypeEnum.Omaha.ToString());
            container.RegisterType<IPokerEvaluator, OmahaHiLoEvaluator>(GeneralGameTypeEnum.OmahaHiLo.ToString());
            container.RegisterType<IProxyImporter, ProxyImporter>();
            container.RegisterType<IAdda52Importer, Adda52Importer>();
            container.RegisterType<IAdda52HandBuilder, Adda52HandBuilder>();
            container.RegisterType<IAdda52TableService, Adda52TableService>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPokerBaaziCatcher, PokerBaaziCatcher>();
            container.RegisterType<IPokerBaaziDataImporter, PokerBaaziDataImporter>();
            container.RegisterType<IPokerBaaziDataManager, PokerBaaziDataManager>();
            container.RegisterType<IPokerBaaziImporter, PokerBaaziImporter>();
            container.RegisterType<IPokerBaaziHandBuilder, PokerBaaziHandBuilder>();

            // Loggers
            container.RegisterType<IPokerClientEncryptedLogger, PokerClientLogger>(LogServices.Base.ToString());
            container.RegisterType<IPokerClientEncryptedLogger, BetOnlineTournamentLogger>(LogServices.BetOnlineTournament.ToString());

            container.RegisterType<IHandHistoryToIPokerConverter, HandHistoryToIPokerConverter>();
            container.RegisterType<IZoneHandAdjuster, ZoneHandAdjuster>();

            LoadLibrariesOnDemand();
        }

        public static void ConfigureImporterService()
        {
            if (!(ServiceLocator.Current.GetInstance<IImporterService>() is IImporterInternalService importerService))
            {
                throw new InvalidCastException("Importers could not be registered");
            }

            importerService.Register<IIgnitionCatcher>();
            importerService.Register<IIgnitionImporter>();
            importerService.Register<IIgnitionInfoImporter>();
            importerService.Register<IBetOnlineCatcher>();
            importerService.Register<IBetOnlineImporter>();
            importerService.Register<IBetOnlineTournamentImporter>();
            importerService.Register<IBetOnlineTableService>();
            importerService.Register<IPokerStarsImporter>();
            importerService.Register<IPokerStarsZoomImporter>();
            importerService.Register<IAmericasCardroomImporter>();
            importerService.Register<IBlackChipPokerImporter>();
            importerService.Register<ITruePokerImporter>();
            importerService.Register<IYaPokerImporter>();
            importerService.Register<IWPNImporter>();
            importerService.Register<IPacific888Importer>();
            importerService.Register<IPartyPokerImporter>();
            importerService.Register<IIPokerImporter>();
            importerService.Register<IExternalImporter>();
            importerService.Register<IPKImporter>();
            importerService.Register<ITcpImporter>();
            importerService.Register<IHorizonImporter>();
            importerService.Register<IWinamaxImporter>();
            importerService.Register<IAdda52Importer>();
            importerService.Register<IProxyImporter>();
            importerService.Register<IAdda52TableService>();
            importerService.Register<IPokerBaaziCatcher>();
            importerService.Register<IPokerBaaziDataImporter>();
            importerService.Register<IPokerBaaziImporter>();

            var tcpImporter = importerService.GetImporter<ITcpImporter>();
            tcpImporter.RegisterImporter<IPKImporter>();

            var proxyImporter = importerService.GetImporter<IProxyImporter>();
            proxyImporter.RegisterImporter<IAdda52Importer>();
        }

        private static Dictionary<string, bool> loadedAssemblies = new Dictionary<string, bool>();

        private static void LoadLibrariesOnDemand()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (s, e) =>
            {
                if (loadedAssemblies.ContainsKey(e.Name))
                {
                    return null;
                }

                try
                {
                    if (e.Name.ContainsIgnoreCase("SmartFox2X"))
                    {
                        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DriveHUD.Importers.Adda52.Libs.SmartFox2X.dll"))
                        {
                            byte[] bytes = new byte[(int)stream.Length];
                            stream.Read(bytes, 0, (int)stream.Length);

                            var assembly = Assembly.Load(bytes);

                            loadedAssemblies[e.Name] = true;

                            return assembly;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(typeof(ImporterBootstrapper), $"Failed to resolve '{e.Name}' assembly dependencies.", ex);
                }

                return null;
            };
        }
    }
}