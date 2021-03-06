//-----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.HudServices;
using DriveHUD.Application.Services;
using DriveHUD.Application.SplashScreen;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Graphs;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Importers;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Reports;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, ISingleInstanceApp
    {
        private const string binDirectory = "bin";
        private readonly bool isUpdatable;
        private ManualResetEvent ResetSplashCreated;
        private Thread SplashThread;

        internal static ISplashScreen SplashScreen { get; set; }

        internal static bool IsUpdateAvailable { get; set; }

        internal static dynamic UpdateApplicationInfo { get; set; }

        public App()
        {
        }

        public App(bool isUpdatable) : base()
        {
            this.isUpdatable = isUpdatable;
        }

        public static Version Version
        {
            get;
            private set;
        }

        public static MainWindowViewModel GetMainViewModel()
        {
            if (App.Current.MainWindow == null)
            {
                return null;
            }

            var mainWindow = App.Current.MainWindow.Content as RadWindow;

            if (mainWindow == null)
            {
                return null;
            }

            var mainViewModel = mainWindow.DataContext as MainWindowViewModel;

            return mainViewModel;
        }

        #region ISingleInstanceApp Members

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            if (MainWindow == null)
            {
                return false;
            }

            if (MainWindow.WindowState == WindowState.Minimized)
            {
                MainWindow.WindowState = WindowState.Normal;
            }
            else
            {
                MainWindow.Activate();
            }

            return true;
        }

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            ResourceRegistrator.Initialization();

            var generalSettingsModel = GetGeneralSettings();

            InitSplashScreen(generalSettingsModel);

            InitializeApplication(generalSettingsModel);

            base.OnStartup(e);

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void InitializeApplication(GeneralSettingsModel generalSettingsModel)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Version = Assembly.GetExecutingAssembly().GetName().Version;

            LogProvider.Log.Info(string.Format("---------------=============== Initialize DriveHUD (v.{0}) ===============---------------", Version));
            LogProvider.Log.Info(string.Format("OsVersion: {0}", Environment.OSVersion));
            LogProvider.Log.Info(string.Format("Current Culture: {0}", Thread.CurrentThread.CurrentCulture));
            LogProvider.Log.Info(string.Format("Current UI Culture: {0}", Thread.CurrentThread.CurrentUICulture));

            ValidateLicenseAssemblies();

            if (generalSettingsModel.IsAutomaticallyDownloadUpdates)
            {
                CheckUpdates();
            }

            VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);

            LogCleaner.ClearLogsFolder();

            /* Without this user won't be able to input decimal point for float bindings if UpdateSourceTrigger set to PropertyChanged */
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;
        }

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DeployLX.Licensing.v5.dll", "DHCReg.dll", "DHHReg.dll", "DHOReg.dll" };
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "b8574645b78bccf0c335b0065474269550a417f8", "b830193154bb28fdc6461593d2c7a228c4adb460", "b4e37214c7d3b770bbb90c1278bbbc946cb8ba98" };
            var assemblySizes = new int[] { 1032192, 54616, 55128, 54104 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                if (!assemblyInfo.Exists && assemblies[i].StartsWith("DeployLX", StringComparison.OrdinalIgnoreCase))
                {
                    assemblyInfo = new FileInfo(Path.Combine(binDirectory, assemblies[i]));
                }

                var isValid = SecurityUtils.ValidateFileHash(assemblyInfo.FullName, assembliesHashes[i]) && assemblyInfo.Length == assemblySizes[i];

                if (!isValid)
                {
                    LogProvider.Log.Error("Application could not be initialized");
                    Current.Shutdown();
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogProvider.Log.Error("Fatal error", e.Exception);
            HandleException(e.Exception);
            e.Handled = true;
            Current.Shutdown();
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            HandleException(ex);
            Current.Shutdown();
        }

        private void HandleException(Exception ex)
        {
            ErrorBox.Show(CommonResourceManager.Instance.GetResourceString("Common_FatalError"), ex, "Unexpected error occurred. Please contact support.");

            if (SplashScreen != null)
            {
                SplashScreen.CloseSplashScreen();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var importService = ServiceLocator.Current.GetInstance<IImporterService>();

            if (importService != null && importService.IsStarted)
            {
                importService.StopImport();
            }

            var hudTransmitter = ServiceLocator.Current.GetInstance<IHudTransmitter>();
            hudTransmitter.Dispose();

            var opponentReportService = ServiceLocator.Current.GetInstance<IOpponentReportService>();
            opponentReportService.Flush();

            var cashGraphSettingsService = ServiceLocator.Current.GetInstance<ICashGraphSettingsService>();
            cashGraphSettingsService.SaveSettings();

            LogProvider.Log.Info("DriveHUD exited.");
        }

        #region Updating        

        private GeneralSettingsModel GetGeneralSettings()
        {
            return new SettingsService(StringFormatter.GetAppDataFolderPath(), true).GetSettings().GeneralSettings;
        }

        private Task<Assembly> LoadUpdaterAssemblyAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    var exists = GetUpdaterAssembly();

                    if (exists != null)
                        return exists;

                    var assemblyBytes = File.ReadAllBytes(UpdaterInfo.UpdaterCoreAssemblyDll);
                    var assembly = Assembly.Load(assemblyBytes);

                    return assembly;
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Updater core has not been loaded.", e);
                }

                return null;
            });
        }

        protected virtual Assembly GetUpdaterAssembly()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            var assembly = asms.FirstOrDefault(x => x.GetName().Name == UpdaterInfo.UpdaterCoreAssembly);
            return assembly;
        }

        private async void CheckUpdates()
        {
            try
            {
                var assembly = await LoadUpdaterAssemblyAsync();

                if (assembly == null)
                {
                    return;
                }

                Type httpApplicationInfoLoaderType = assembly.GetType(UpdaterInfo.HttpApplicationInfoLoader);
                dynamic httpApplicationInfoLoader = Activator.CreateInstance(httpApplicationInfoLoaderType);

                Type appUpdaterType = assembly.GetType(UpdaterInfo.AppUpdater);

                using (dynamic appUpdater = Activator.CreateInstance(appUpdaterType, httpApplicationInfoLoader))
                {
                    await appUpdater.InitializeAsync();

                    // update updater silently
                    if (appUpdater.CheckIsUpdateAvailable(UpdaterInfo.UpdaterGuid, UpdaterInfo.UpdaterAssembly))
                    {
                        await appUpdater.UpdateApplicationAsync(UpdaterInfo.UpdaterGuid, UpdaterInfo.UpdaterAssembly, false);
                        LogProvider.Log.Info("Updater has been updated");
                        return;
                    }

                    bool isUpdateAvailable;

                    dynamic appInfo = appUpdater.CheckIsUpdateAvailable(ProgramInfo.AssemblyGuid, Assembly.GetExecutingAssembly().Location, out isUpdateAvailable);

                    IsUpdateAvailable = isUpdateAvailable;

                    if (IsUpdateAvailable)
                    {
                        UpdateApplicationInfo = appInfo;

                        var mainWindowViewModel = GetMainViewModel();
                        mainWindowViewModel?.ShowUpdate();
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Updates couldn't be checked.", e);
            }
        }

        #endregion

        #region SplashScreen members

        private void InitSplashScreen(GeneralSettingsModel generalSettingsModel)
        {
            ResetSplashCreated = new ManualResetEvent(false);

            SplashThread = new Thread(() => ShowSplashScreen(generalSettingsModel));
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Start();

            ResetSplashCreated.WaitOne();
        }

        private void ShowSplashScreen(GeneralSettingsModel generalSettingsModel)
        {
            var splashWindow = new SplashWindow
            {
                DataContext = new SplashWindowViewModel()
            };

            SplashScreen = splashWindow;

            if (generalSettingsModel.RememberScreenPosition)
            {
                var positionsInfo = new WindowPositionsInfo
                {
                    Width = splashWindow.Width,
                    Height = splashWindow.Height,
                    DisplaySettings = generalSettingsModel.DisplaySettings,
                    StartupLocation = WindowStartupLocation.CenterScreen
                };

                if (positionsInfo.DisplaySettings != null)
                {
                    positionsInfo.DisplaySettings.Maximized = false;
                }

                WindowPositionsService.SetPosition(splashWindow, positionsInfo);
            }

            splashWindow.Show();

            ResetSplashCreated.Set();
            Dispatcher.Run();
        }

        #endregion
    }
}
