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

using DriveHUD.API;
using DriveHUD.Application.Bootstrappers;
using DriveHUD.Application.HudServices;
using DriveHUD.Application.Licensing;
using DriveHUD.Application.Migrations;
using DriveHUD.Application.MigrationService;
using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Application.Modules;
using DriveHUD.Application.Security;
using DriveHUD.Application.Services;
using DriveHUD.Application.Surrogates;
using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.TableConfigurators.SiteSettingTableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Graphs;
using DriveHUD.Application.ViewModels.Graphs.SeriesProviders;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Application.Views;
using DriveHUD.Application.Views.Graphs;
using DriveHUD.Application.Views.Hud;
using DriveHUD.Common.Log;
using DriveHUD.Common.Security;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Common.Wpf.Interactivity;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
using DriveHUD.ViewModels.Replayer;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Interfaces;
using Model.Settings;
using Model.Site;
using Prism.Unity;
using ProtoBuf.Meta;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
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

            var moduleService = ServiceLocator.Current.GetInstance<IModuleService>();
            moduleService.InitializeModules(Container);

            var sqliteBootstrapper = ServiceLocator.Current.GetInstance<ISQLiteBootstrapper>();
            sqliteBootstrapper.InitializeDatabase();

            ImporterBootstrapper.ConfigureImporterService();

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
                    var dataRemoverViewModel = new DataRemoverViewModel();
                    dataRemoverViewModel.UninstallCommand.Execute(null);
                }
                else
                {
                    var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                    var settingsModel = settingsService.GetSettings();

                    mainWindowViewModel = new MainWindowViewModel();

                    var mainRadWindow = (RadWindow)Shell;

                    mainRadWindow.DataContext = mainWindowViewModel;

                    if (settingsModel != null && settingsModel.GeneralSettings != null &&
                        settingsModel.GeneralSettings.RememberScreenPosition)
                    {
                        var positionsInfo = new WindowPositionsInfo
                        {
                            Width = mainWindowViewModel.WindowMinWidth,
                            Height = mainWindowViewModel.AppStartupHeight,
                            DisplaySettings = settingsModel?.GeneralSettings?.DisplaySettings,
                            StartupLocation = WindowStartupLocation.Manual
                        };

                        WindowPositionsService.SetPosition(mainRadWindow, positionsInfo);
                    }

                    mainRadWindow.IsTopmost = true;
                    mainRadWindow.Show();
                    mainRadWindow.IsTopmost = false;

                    App.SplashScreen.CloseSplashScreen();

                    var mainWindow = mainRadWindow.ParentOfType<Window>();

                    if (mainWindow != null && App.Current.MainWindow != mainWindow)
                    {
                        App.Current.MainWindow = mainWindow;
                    }

                    if (App.IsUpdateAvailable)
                    {
                        mainWindowViewModel.IsActive = true;
                        mainWindowViewModel.ShowUpdate();
                        mainWindowViewModel.IsActive = false;
                    }

                    var licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

                    if (!isLicenseValid || licenseService.IsTrial ||
                        (licenseService.IsRegistered && licenseService.IsExpiringSoon) ||
                        !licenseService.IsRegistered)
                    {
                        var registrationViewModel = new RegistrationViewModel(false);
                        mainWindowViewModel.RegistrationViewRequest.Raise(registrationViewModel);
                        mainWindowViewModel.UpdateHeader();
                    }

                    if (!licenseService.IsRegistered)
                    {
                        System.Windows.Application.Current.Shutdown();
                    }

                    mainWindowViewModel.IsTrial = licenseService.IsTrial;
                    mainWindowViewModel.IsUpgradable = licenseService.IsUpgradable;

                    mainWindowViewModel.IsActive = true;

                    if (settingsModel != null && settingsModel.GeneralSettings != null)
                    {
                        try
                        {
                            var validationResults = ServiceLocator.Current.GetInstance<ISiteConfigurationService>()
                                 .ValidateSiteConfigurations().ToArray();

                            var detectedSites = validationResults.Where(x => x.IsNew && x.IsDetected).ToArray();

                            if (detectedSites.Length > 0)
                            {
                                var sitesSetupViewModel = new SitesSetupViewModel(detectedSites);
                                mainWindowViewModel.SitesSetupViewRequest?.Raise(sitesSetupViewModel);
                            }

                            var incorrectlyConfiguredSites = validationResults
                                .Where(x => x.HasIssue && x.IsEnabled && settingsModel.GeneralSettings.RunSiteDetection)
                                .ToArray();

                            if (incorrectlyConfiguredSites.Length > 0)
                            {
                                var incorrectlyConfiguredSitesViewModel = new IncorrectlyConfiguredSitesViewModel(incorrectlyConfiguredSites);
                                mainWindowViewModel.IncorrectlyConfiguredSitesViewRequest?.Raise(incorrectlyConfiguredSitesViewModel);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error(this, "Site validation failed.", ex);
                        }
                    }
#warning disable
                //    mainWindowViewModel.StartHudCommand.Execute(null);
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
            RegisterTypeIfMissing(typeof(ILayoutMigrator), typeof(LayoutMigrator), false);
            RegisterTypeIfMissing(typeof(ITreatAsService), typeof(TreatAsService), true);
            RegisterTypeIfMissing(typeof(IModuleService), typeof(ModuleService), false);
            RegisterTypeIfMissing(typeof(IWindowController), typeof(WindowController), true);

            // Migration
            Container.RegisterType<IMigrationService, SQLiteMigrationService>(DatabaseType.SQLite.ToString());
            Container.RegisterType<IMigrationService, PostgresMigrationService>(DatabaseType.PostgreSQL.ToString());

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
            Container.RegisterType<ISiteConfiguration, IPokerConfiguration>(EnumPokerSites.IPoker.ToString());
            Container.RegisterType<ISiteConfiguration, PartyPokerConfiguration>(EnumPokerSites.PartyPoker.ToString());
            Container.RegisterType<ISiteConfiguration, HorizonConfiguration>(EnumPokerSites.Horizon.ToString());
            Container.RegisterType<ISiteConfiguration, WinamaxConfiguration>(EnumPokerSites.Winamax.ToString());

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
            Container.RegisterType<ICashGraphSettingsService, CashGraphSettingsService>(new ContainerControlledLifetimeManager());

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
            Container.RegisterType<ISiteSettingTableConfigurator, PartyPokerSiteSettingTableConfigurator>(EnumPokerSites.PartyPoker.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, IPokerSiteSettingTableConfigurator>(EnumPokerSites.IPoker.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, GGNSiteSettingTableConfigurator>(EnumPokerSites.GGN.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, HorizonSiteSettingTableConfigurator>(EnumPokerSites.Horizon.ToString());
            Container.RegisterType<ISiteSettingTableConfigurator, WinamaxSiteSettingTableConfigurator>(EnumPokerSites.Winamax.ToString());

            // Series providers
            Container.RegisterType<IGraphsProvider, GraphsProvider>();
            Container.RegisterType<IGraphSeriesProvider, WinningByMonthSeriesProvider>(SerieType.WinningsByMonth.ToString());
            Container.RegisterType<IGraphSeriesProvider, WinningByYearSeriesProvider>(SerieType.WinningsByYear.ToString());
            Container.RegisterType<IGraphSeriesProvider, MoneyWinByCashGameTypeSeriesProvider>(SerieType.MoneyWonByCashGameType.ToString());
            Container.RegisterType<IGraphSeriesProvider, MoneyWinByTournamentGameTypeSeriesProvider>(SerieType.MoneyWonByTournamentGameType.ToString());
            Container.RegisterType<IGraphSeriesProvider, EVDiffToRealizedEvByMonthSeriesProvider>(SerieType.EVDiffToRealizedEVByMonth.ToString());
            Container.RegisterType<IGraphSeriesProvider, Top20BiggestLosingHandsSeriesProvider>(SerieType.Top20BiggestLosingHands.ToString());
            Container.RegisterType<IGraphSeriesProvider, Top20BiggestWinningHandsSeriesProvider>(SerieType.Top20BiggestWinningHands.ToString());
            Container.RegisterType<IGraphSeriesProvider, MoneyWonByPositionSeriesProvider>(SerieType.MoneyWonByPosition.ToString());
            Container.RegisterType<IGraphSeriesProvider, BB100ByTimeOfDaySeriesProvider>(SerieType.BB100ByTimeOfDay.ToString());
            Container.RegisterType<IGraphSeriesProvider, Top20ToughestOpponentsByLossAmountSeriesProvider>(SerieType.Top20ToughestOpponents.ToString());

            // Register views containers
            Container.RegisterType<IViewModelContainer, CashGraphPopupView>(RegionViewNames.CashGraphPopupView);
            Container.RegisterType<IViewModelContainer, HudUploadToStoreView>(RegionViewNames.HudUploadToStoreView);

            // Register view models
            Container.RegisterType<ICashGraphPopupViewModel, CashGraphPopupViewModel>();
            Container.RegisterType<IHudUploadToStoreViewModel, HudUploadToStoreViewModel>();

            ImporterBootstrapper.ConfigureImporter(Container);
        }
    }
}