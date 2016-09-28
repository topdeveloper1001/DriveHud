//-----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DHCRegistration;
using DHHRegistration;
using DHORegistration;
using DriveHUD.Application.Licensing;
using DriveHUD.Application.Security;
using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Application.Views;
using DriveHUD.Common.Log;
using DriveHUD.Common.Security;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Builders.iPoker;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.PokerStars;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Enums;
using Model.Filters;
using Model.Interfaces;
using Model.Settings;
using Model.Site;
using Prism.Unity;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using Telerik.Windows.Controls;

namespace DriveHUD.Application
{
    public class Bootstrapper : UnityBootstrapper
    {
        private MainWindowViewModel mainWindowViewModel;

        private ConfigurePostgresqlServerViewModel configViewModel;
        private ConfigurePostgresqlServer configWindow;
        private bool isLicenseValid;

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            this.ModuleCatalog.AddModule(null);
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();

            // initialize and load configurations
            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configurationService.Initialize();

            var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();
            isLicenseValid = licenseService.Validate();

            var sessionService = ServiceLocator.Current.GetInstance<ISessionService>();
            sessionService.Initialize();

            var tournamentCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
            tournamentCacheService.Initialize();

            // register importers
            ConfigureImporters();

            configViewModel = new ConfigurePostgresqlServerViewModel();
            configViewModel.AfterConnectAction += ShowMainWindow;
            configViewModel.ConnectCommand.Execute(null);
        }

        private void ShowMainWindow()
        {
            if (ConfigurePostgresqlServerViewModel.IsConnected)
            {
                if (IsUninstall())
                {
                    LogProvider.Log.Debug(this, "Uninstalling all user's data...");
                    DataRemoverViewModel dr = new DataRemoverViewModel();
                    dr.UninstallCommand.Execute(null);
                }
                else
                {
                    mainWindowViewModel = new MainWindowViewModel(SynchronizationContext.Current);
                    ((RadWindow)this.Shell).DataContext = mainWindowViewModel;

                    ((RadWindow)this.Shell).Activated += MainWindow_Activated;

                    ((RadWindow)this.Shell).Show();

                    App.SplashScreen.CloseSplashScreen();

                    var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                    if (!isLicenseValid || licenseService.IsTrial || licenseService.IsExpiringSoon || licenseService.IsExpired)
                    {
                        var registrationViewModel = new RegistrationViewModel(false);
                        mainWindowViewModel.RegistrationViewRequest.Raise(registrationViewModel);
                        mainWindowViewModel.UpdateHeader();
                    }

                    if (!licenseService.IsRegistered)
                    {
#if !DEBUG
                         System.Windows.Application.Current.Shutdown();
#endif
                    }

                    mainWindowViewModel.IsTrial = licenseService.IsTrial;
                    mainWindowViewModel.IsUpgradable = licenseService.IsUpgradable;

                    mainWindowViewModel.IsActive = true;

                    mainWindowViewModel.StartHud(false);
                }
            }
            else
            {
                if (configWindow == null)
                {
                    configWindow = new ConfigurePostgresqlServer(configViewModel);
                }

                configWindow.Show();
                App.SplashScreen.CloseSplashScreen();
            }
        }

        private bool IsUninstall()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args.Skip(1))
            {
                LogProvider.Log.Debug(this, string.Format("Argument found {0}", arg));
                if (arg == "-uninstall")
                {
                    return true;
                }
            }
            return false;
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            foreach (var v in RadWindowManager.Current.GetWindows())
            {
                if (v.Tag != null && v.Tag.ToString() == "PopupWindow")
                {
                    v.BringIntoView();
                    v.BringToFront();
                    v.Focus();
                }
            }
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<SingletonStorageModel>(new ContainerControlledLifetimeManager());

            RegisterTypeIfMissing(typeof(ISessionService), typeof(SessionService), true);
            RegisterTypeIfMissing(typeof(IBovadaCatcher), typeof(BovadaCatcher), false);
            RegisterTypeIfMissing(typeof(IBovadaDataManager), typeof(BovadaDataManager), false);
            RegisterTypeIfMissing(typeof(IBovadaImporter), typeof(BovadaImporter), false);
            RegisterTypeIfMissing(typeof(IBetOnlineCatcher), typeof(BetOnlineCatcher), false);
            RegisterTypeIfMissing(typeof(IBetOnlineDataManager), typeof(BetOnlineDataManager), false);
            RegisterTypeIfMissing(typeof(IBetOnlineImporter), typeof(BetOnlineImporter), false);
            RegisterTypeIfMissing(typeof(IBetOnlineTableService), typeof(BetOnlineTableService), true);
            RegisterTypeIfMissing(typeof(IBetOnlineTournamentImporter), typeof(BetOnlineTournamentImporter), false);
            RegisterTypeIfMissing(typeof(IBetOnlineTournamentManager), typeof(BetOnlineTournamentManager), false);
            RegisterTypeIfMissing(typeof(IBetOnlineXmlConverter), typeof(BetOnlineXmlToIPokerXmlConverter), false);
            RegisterTypeIfMissing(typeof(IPokerStarsImporter), typeof(PokerStarsImporter), false);
            RegisterTypeIfMissing(typeof(ICardsConverter), typeof(PokerCardsConverter), false);
            RegisterTypeIfMissing(typeof(ITournamentsCacheService), typeof(TournamentsCacheService), true);
            RegisterTypeIfMissing(typeof(IDataService), typeof(DataService), true);
            RegisterTypeIfMissing(typeof(IImporterService), typeof(ImporterService), true);
            RegisterTypeIfMissing(typeof(IParser), typeof(BovadaParser), false);
            RegisterTypeIfMissing(typeof(IPokerClientEncryptedLogger), typeof(PokerClientLogger), false);
            RegisterTypeIfMissing(typeof(ISiteConfigurationService), typeof(SiteConfigurationService), true);
            RegisterTypeIfMissing(typeof(IFileImporter), typeof(FileImporter), false);
            RegisterTypeIfMissing(typeof(IHandHistoryParserFactory), typeof(HandHistoryParserFactoryImpl), false);
            RegisterTypeIfMissing(typeof(IImporterSessionCacheService), typeof(ImporterSessionCacheService), true);
            RegisterTypeIfMissing(typeof(ILicenseService), typeof(LicenseService), true);
            RegisterTypeIfMissing(typeof(IHudElementViewModelCreator), typeof(HudElementViewModelCreator), false);
            RegisterTypeIfMissing(typeof(IHudPanelService), typeof(HudPanelService), false);
            RegisterTypeIfMissing(typeof(IHudLayoutsService), typeof(HudLayoutsService), true);
            RegisterTypeIfMissing(typeof(IReplayerTableConfigurator), typeof(ReplayerTableConfigurator), false);
            RegisterTypeIfMissing(typeof(IPlayerStatisticCalculator), typeof(PlayerStatisticCalculator), false);
            RegisterTypeIfMissing(typeof(IFileImporterLogger), typeof(FileImporterLogger), false);

            // Loggers
            Container.RegisterType<IPokerClientEncryptedLogger, PokerClientLogger>(LogServices.Base.ToString());
            Container.RegisterType<IPokerClientEncryptedLogger, BetOnlineTournamentLogger>(LogServices.BetOnlineTournament.ToString());

            // Filters Save/Load service
            Container.RegisterType<IFilterDataService, FilterDataService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));
            Container.RegisterType<IFilterModelManagerService, FilterModelManagerService>(new ContainerControlledLifetimeManager());

            // Sites configurations
            Container.RegisterType<ISiteConfiguration, BovadaConfiguration>(EnumPokerSites.Bovada.ToString());
            Container.RegisterType<ISiteConfiguration, BetOnlineConfiguration>(EnumPokerSites.BetOnline.ToString());
            Container.RegisterType<ISiteConfiguration, TigerGamingConfiguration>(EnumPokerSites.TigerGaming.ToString());
            Container.RegisterType<ISiteConfiguration, SportsBettingConfiguration>(EnumPokerSites.SportsBetting.ToString());
            Container.RegisterType<ISiteConfiguration, PokerStarsConfiguration>(EnumPokerSites.PokerStars.ToString());

            // HUD table configurators

            // Bovada
            Container.RegisterType<ITableConfigurator, BovadaRichTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.Bovada, HudType.Default));
            Container.RegisterType<ITableConfigurator, BovadaTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.Bovada, HudType.Plain));

            // BetOnline
            Container.RegisterType<ITableConfigurator, CommonRichTableConfiguration>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.BetOnline, HudType.Default));
            Container.RegisterType<ITableConfigurator, CommonTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.BetOnline, HudType.Plain));
            Container.RegisterType<ITableConfigurator, CommonRichTableConfiguration>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.SportsBetting, HudType.Default));
            Container.RegisterType<ITableConfigurator, CommonTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.SportsBetting, HudType.Plain));
            Container.RegisterType<ITableConfigurator, CommonRichTableConfiguration>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.TigerGaming, HudType.Default));
            Container.RegisterType<ITableConfigurator, CommonTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.TigerGaming, HudType.Plain));

            // PokerStars
            Container.RegisterType<ITableConfigurator, CommonRichTableConfiguration>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.PokerStars, HudType.Default));
            Container.RegisterType<ITableConfigurator, PokerStarsTableConfigurator>(TableConfiguratorHelper.GetServiceName(EnumPokerSites.PokerStars, HudType.Plain));

            // HUD panel services
            Container.RegisterType<IHudPanelService, HudPanelService>();
            Container.RegisterType<IHudPanelService, BovadaHudPanelService>(EnumPokerSites.Bovada.ToString());
            Container.RegisterType<IHudPanelService, BovadaHudPanelService>(EnumPokerSites.Bodog.ToString());
            Container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.BetOnline.ToString());
            Container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.SportsBetting.ToString());
            Container.RegisterType<IHudPanelService, BetOnlineHudPanelService>(EnumPokerSites.TigerGaming.ToString());
            Container.RegisterType<IHudPanelService, PokerStarsHudPanelService>(EnumPokerSites.PokerStars.ToString());

            // Licenses
            Container.RegisterType<ILicenseManager, DHTReg>(LicenseType.Trial.ToString());
            Container.RegisterType<ILicenseManager, DHHReg>(LicenseType.Holdem.ToString());
            Container.RegisterType<ILicenseManager, DHOReg>(LicenseType.Omaha.ToString());
            Container.RegisterType<ILicenseManager, DHCReg>(LicenseType.Combo.ToString());

            //Settings
            Container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));

            // Settings Table Configurators
            Container.RegisterType<ISiteSettingTableConfigurator, BovadaSiteSettingTableConfigurator>(EnumPokerSites.Bovada.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.BetOnline.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.TigerGaming.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.SportsBetting.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, PokerStarsSiteSettingTableConfigurator>(EnumPokerSites.PokerStars.ToString());

        }

        private void ConfigureImporters()
        {
            var importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            importerService.Register<IBovadaCatcher>();
            importerService.Register<IBovadaImporter>();
            importerService.Register<IBetOnlineCatcher>();
            importerService.Register<IBetOnlineImporter>();
            importerService.Register<IBetOnlineTournamentImporter>();
            importerService.Register<IPokerStarsImporter>();
        }
    }
}