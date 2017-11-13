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
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Importers;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
            InitSplashScreen();

            InitializeApplication();

            base.OnStartup(e);

            Bootstrapper bootstrapper = new Bootstrapper();
            bootstrapper.Run();
        }

        private void InitializeApplication()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            Version = Assembly.GetExecutingAssembly().GetName().Version;

            LogProvider.Log.Info(string.Format("---------------=============== Initialize DriveHUD (v.{0}) ===============---------------", Version));
            LogProvider.Log.Info(string.Format("OsVersion: {0}", Environment.OSVersion));
            LogProvider.Log.Info(string.Format("Current Culture: {0}", Thread.CurrentThread.CurrentCulture));
            LogProvider.Log.Info(string.Format("Current UI Culture: {0}", Thread.CurrentThread.CurrentUICulture));

            ValidateLicenseAssemblies();

            ResourceRegistrator.Initialization();

            if (IsCheckForUpdates())
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
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "9dcc02cf60b458504c402810d969418bff944c0a", "ddc4ad2b78c769c8ac4c0cb30581ba78bdd038ed", "1946d835b06ebec4e74931222c11cdc69018a434" };
            var assemblySizes = new int[] { 1032192, 53592, 54104, 54104 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                if (!assemblyInfo.Exists)
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

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogProvider.Log.Error("Fatal error", e.Exception);
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            HandleException(ex);
        }

        private void HandleException(Exception ex)
        {
            var errorMessage = ex != null ? ex.Message : "Unexpected error occurred. Please contact support.";

            MessageBox.Show(errorMessage, "Fatal error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);

            if (SplashScreen != null)
            {
                SplashScreen.CloseSplashScreen();
            }

            Current.Shutdown();
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

            LogProvider.Log.Info("DriveHUD exited");
        }

        #region Updating

        private Dictionary<string, X509Certificate2> dynamicAssemblyCertificates = new Dictionary<string, X509Certificate2>();

        private bool IsCheckForUpdates()
        {
            return new SettingsService(StringFormatter.GetAppDataFolderPath(), true).GetSettings().GeneralSettings.IsAutomaticallyDownloadUpdates;
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

        private void InitSplashScreen()
        {
            ResetSplashCreated = new ManualResetEvent(false);

            SplashThread = new Thread(ShowSplashScreen);
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Start();

            ResetSplashCreated.WaitOne();
        }

        private void ShowSplashScreen()
        {
            var splashWindow = new SplashWindow();
            splashWindow.DataContext = new SplashWindowViewModel();
            SplashScreen = splashWindow;

            splashWindow.Show();

            ResetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        #endregion
    }
}
