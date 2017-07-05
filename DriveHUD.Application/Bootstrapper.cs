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
using DriveHUD.API;
using DriveHUD.Application.Bootstrappers;
using DriveHUD.Application.HudServices;
using DriveHUD.Application.Licensing;
using DriveHUD.Application.Migrations;
using DriveHUD.Application.MigrationService;
using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Application.Security;
using DriveHUD.Application.Surrogates;
using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.TableConfigurators.SiteSettingTableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Common.Log;
using DriveHUD.Common.Security;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
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
using ProtoBuf.Meta;
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

        private bool isLicenseValid;

        protected override DependencyObject CreateShell()
        {
            return ServiceLocator.Current.GetInstance<MainWindow>();
        }

        protected override void ConfigureModuleCatalog()
        {
            RegisterRuntimeTypeModelTypes();

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

            ImporterBootstrapper.ConfigureImporterService();

            var sqliteBootstrapper = ServiceLocator.Current.GetInstance<ISQLiteBootstrapper>();
            sqliteBootstrapper.InitializeDatabase();        

            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            LogProvider.Log.Info($"Screen: {Utils.GetScreenResolution()}");
            LogProvider.Log.Info($"Dpi: {Utils.GetCurrentDpi()}");

            try
            {
                if (IsUninstall())
                {
                    LogProvider.Log.Info(this, "Uninstalling all user's data...");
                    DataRemoverViewModel dr = new DataRemoverViewModel();
                    dr.UninstallCommand.Execute(null);
                }
                else
                {
                    mainWindowViewModel = new MainWindowViewModel(SynchronizationContext.Current);
                    ((RadWindow)this.Shell).DataContext = mainWindowViewModel;

                    ((RadWindow)this.Shell).Activated += MainWindow_Activated;

                    ((RadWindow)this.Shell).IsTopmost = true;
                    ((RadWindow)this.Shell).Show();
                    ((RadWindow)this.Shell).IsTopmost = false;

                    App.SplashScreen.CloseSplashScreen();

                    if (App.IsUpdateAvailable)
                    {
                        mainWindowViewModel.IsActive = true;
                        mainWindowViewModel.ShowUpdate();
                        mainWindowViewModel.IsActive = false;
                    }

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

                    mainWindowViewModel.StartHudCommand.Execute(null);

                    ServiceLocator.Current.GetInstance<ISiteConfigurationService>().ValidateSiteConfigurations();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        private bool IsUninstall()
        {
            string[] args = Environment.GetCommandLineArgs();
            foreach (string arg in args.Skip(1))
            {
                LogProvider.Log.Info(this, string.Format("Argument found {0}", arg));

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

        public static void RegisterRuntimeTypeModelTypes()
        {
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            Container.RegisterType<SingletonStorageModel>(new ContainerControlledLifetimeManager());

            RegisterTypeIfMissing(typeof(ISQLiteBootstrapper), typeof(SQLiteBootstrapper), false);
            RegisterTypeIfMissing(typeof(IDataService), typeof(DataService), true);
            RegisterTypeIfMissing(typeof(ISiteConfigurationService), typeof(SiteConfigurationService), true);
            RegisterTypeIfMissing(typeof(IHandHistoryParserFactory), typeof(HandHistoryParserFactoryImpl), false);
            RegisterTypeIfMissing(typeof(ILicenseService), typeof(LicenseService), true);
            RegisterTypeIfMissing(typeof(IHudElementViewModelCreator), typeof(HudElementViewModelCreator), false);
            RegisterTypeIfMissing(typeof(IHudLayoutsService), typeof(HudLayoutsService), true);
            RegisterTypeIfMissing(typeof(IReplayerTableConfigurator), typeof(ReplayerTableConfigurator), false);
            RegisterTypeIfMissing(typeof(IReplayerService), typeof(ReplayerService), true);
            RegisterTypeIfMissing(typeof(IPlayerStatisticCalculator), typeof(PlayerStatisticCalculator), false);
            RegisterTypeIfMissing(typeof(ISessionService), typeof(SessionService), true);
            RegisterTypeIfMissing(typeof(IHudTransmitter), typeof(HudTransmitter), true);
            RegisterTypeIfMissing(typeof(ITopPlayersService), typeof(TopPlayersService), true);
            RegisterTypeIfMissing(typeof(ILayoutMigrator), typeof(LayoutMigrator), false);

            // Migration
            Container.RegisterType<IMigrationService, SQLiteMigrationService>(DatabaseType.SQLite.ToString());
            Container.RegisterType<IMigrationService, PostgresMigrationService>(DatabaseType.PostgreSQL.ToString());

            // Filters Save/Load service
            Container.RegisterType<IFilterDataService, FilterDataService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));
            Container.RegisterType<IFilterModelManagerService, MainFilterModelManagerService>(FilterServices.Main.ToString(), new ContainerControlledLifetimeManager());
            Container.RegisterType<IFilterModelManagerService, StickersFilterModelManagerService>(FilterServices.Stickers.ToString(), new ContainerControlledLifetimeManager());

            // Sites configurations
            Container.RegisterType<ISiteConfiguration, BovadaConfiguration>(EnumPokerSites.Ignition.ToString());
            Container.RegisterType<ISiteConfiguration, BetOnlineConfiguration>(EnumPokerSites.BetOnline.ToString());
            Container.RegisterType<ISiteConfiguration, TigerGamingConfiguration>(EnumPokerSites.TigerGaming.ToString());
            Container.RegisterType<ISiteConfiguration, SportsBettingConfiguration>(EnumPokerSites.SportsBetting.ToString());
            Container.RegisterType<ISiteConfiguration, PokerStarsConfiguration>(EnumPokerSites.PokerStars.ToString());
            Container.RegisterType<ISiteConfiguration, Poker888Configuration>(EnumPokerSites.Poker888.ToString());
            Container.RegisterType<ISiteConfiguration, AmericasCardroomConfiguration>(EnumPokerSites.AmericasCardroom.ToString());
            Container.RegisterType<ISiteConfiguration, BlackChipPokerConfiguration>(EnumPokerSites.BlackChipPoker.ToString());
            Container.RegisterType<ISiteConfiguration, TruePokerConfiguration>(EnumPokerSites.TruePoker.ToString());
            Container.RegisterType<ISiteConfiguration, YaPokerConfiguration>(EnumPokerSites.YaPoker.ToString());

            // HUD designer 
            Container.RegisterType<IHudToolFactory, HudToolFactory>();

            // HUD panel services
            UnityServicesBootstrapper.ConfigureContainer(Container);

            // Model services
            ModelBootstrapper.ConfigureContainer(Container);

            // API services
            APIServicesBootstrapper.ConfigureContainer(Container);

            // Licenses
            Container.RegisterType<ILicenseManager, DHTReg>(LicenseType.Trial.ToString());
            Container.RegisterType<ILicenseManager, DHHReg>(LicenseType.Holdem.ToString());
            Container.RegisterType<ILicenseManager, DHOReg>(LicenseType.Omaha.ToString());
            Container.RegisterType<ILicenseManager, DHCReg>(LicenseType.Combo.ToString());

            //Settings
            Container.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));

            // Settings Table Configurators
            Container.RegisterType<ISiteSettingTableConfigurator, BovadaSiteSettingTableConfigurator>(EnumPokerSites.Ignition.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.BetOnline.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.TigerGaming.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, CommonSiteSettingTableConfigurator>(EnumPokerSites.SportsBetting.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, PokerStarsSiteSettingTableConfigurator>(EnumPokerSites.PokerStars.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, Poker888SiteSettingTableConfigurator>(EnumPokerSites.Poker888.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, WinningPokerNetworkSiteSettingTableConfigurator>(EnumPokerSites.AmericasCardroom.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, WinningPokerNetworkSiteSettingTableConfigurator>(EnumPokerSites.BlackChipPoker.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, WinningPokerNetworkSiteSettingTableConfigurator>(EnumPokerSites.TruePoker.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, WinningPokerNetworkSiteSettingTableConfigurator>(EnumPokerSites.YaPoker.ToString());

            ImporterBootstrapper.ConfigureImporter(Container);
        }
    }
}