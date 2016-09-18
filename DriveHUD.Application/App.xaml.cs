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

#if DEBUG
#define DBC_CHECK_ALL
#else
#define DBC_CHECK_PRECONDITION
#endif

using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Importers;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using Telerik.Windows.Controls;
using System.Diagnostics;
using Model.Settings;
using Model;
using DriveHUD.Application.SplashScreen;
using System.Threading;

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, ISingleInstanceApp
    {
        private readonly bool isUpdatable;

        internal static ISplashScreen SplashScreen;
        private ManualResetEvent ResetSplashCreated;
        private Thread SplashThread;

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

            LogProvider.Log.Info(string.Format("Initialize DriveHUD (v.{0})", Version));

            ValidateLicenseAssemblies();

            VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
            ResourceRegistrator.Initialization();

            /* Without this user won't be able to input decimal point for float bindings if UpdateSourceTrigger set to PropertyChanged */
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            if (IsCheckForUpdates())
            {
                CheckUpdates();
            }
        }

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DHCReg.dll", "DHHReg.dll", "DHOReg.dll" };
            var assembliesHashes = new string[] { "80f3233c02e7c5db0b553f86c4a6b6dd4681ce56", "5e0ffd7140673cbb3056cd533b095afbf92f0e4a", "e32a976e761508c5d7edca70c886e51582d5d91e" };
            var assemblySizes = new int[] { 45568, 45568, 43008 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                var isValid = SecurityUtils.ValidateFileHash(assemblies[i], assembliesHashes[i]) && assemblyInfo.Length == assemblySizes[i];

                if (!isValid)
                {
                    LogProvider.Log.Error("Application could not be initialized");
                    Current.Shutdown();
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogProvider.Log.Error("Unexpected error", e.Exception);

                if (!Debugger.IsAttached)
                {
                    e.Handled = true;
                }
            }
            catch (Exception)
            {

            }
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            HandleException(ex);
        }

        private void HandleException(Exception ex)
        {
            if (ex != null)
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var importService = ServiceLocator.Current.GetInstance<IImporterService>();

            if (importService != null)
            {
                importService.StopImport();
            }
        }

        private Dictionary<string, X509Certificate2> dynamicAssemblyCertificates = new Dictionary<string, X509Certificate2>();

        private bool IsCheckForUpdates()
        {
            return new SettingsService(StringFormatter.GetAppDataFolderPath()).GetSettings().GeneralSettings.IsAutomaticallyDownloadUpdates;
        }

        protected virtual Task<Assembly> LoadUpdaterAssemblyAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    var exists = GetUpdaterAssembly();

                    if (exists != null)
                        return exists;

                    var assemblyCertificate = new X509Certificate2(UpdaterInfo.UpdaterCoreAssemblyDll);

                    var executingAssembly = Assembly.GetExecutingAssembly();
                    var executingAssemblyCertificate = new X509Certificate2(executingAssembly.Location);

                    if (!assemblyCertificate.Equals(executingAssemblyCertificate))
                        return null;

                    var assemblyBytes = File.ReadAllBytes(UpdaterInfo.UpdaterCoreAssemblyDll);
                    var assembly = Assembly.Load(assemblyBytes);

                    dynamicAssemblyCertificates.Add(UpdaterInfo.UpdaterCoreAssembly, assemblyCertificate);

                    return assembly;
                }
                catch
                {
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

        protected async virtual void CheckUpdates()
        {
            try
            {
                var assembly = await LoadUpdaterAssemblyAsync();

                if (assembly == null)
                    return;

                Type httpApplicationInfoLoaderType = assembly.GetType(UpdaterInfo.HttpApplicationInfoLoader);
                dynamic httpApplicationInfoLoader = Activator.CreateInstance(httpApplicationInfoLoaderType);

                Type appUpdaterType = assembly.GetType(UpdaterInfo.AppUpdater);

                using (dynamic appUpdater = Activator.CreateInstance(appUpdaterType, httpApplicationInfoLoader))
                {
                    X509Certificate2 certif = dynamicAssemblyCertificates[UpdaterInfo.UpdaterCoreAssembly];

                    await appUpdater.InitializeAsync(assemblyCertificate: certif);

                    if (appUpdater.CheckIsUpdateAvailable(UpdaterInfo.UpdaterGuid, UpdaterInfo.UpdaterAssembly))
                        await appUpdater.UpdateApplicationAsync(UpdaterInfo.UpdaterGuid, UpdaterInfo.UpdaterAssembly);

                    if (appUpdater.CheckIsUpdateAvailable(ProgramInfo.AssemblyGuid, Assembly.GetExecutingAssembly().Location)
                        && IsUserAccepted())
                    {
                        ProcessStartInfo info = new ProcessStartInfo(UpdaterInfo.UpdaterAssembly);

                        Process updaterProc = new Process()
                        {
                            StartInfo = info
                        };

                        updaterProc.Start();
                        Shutdown();
                    }
                }
            }
            //silent mode
            catch
            {
            }
        }

        private static bool IsUserAccepted()
        {
            var messageBox = System.Windows.Forms.DialogResult.Cancel;
            App.Current.Dispatcher.Invoke(() =>
            {

                messageBox = System.Windows.Forms.MessageBox.Show(owner: new System.Windows.Forms.Form { TopMost = true }, text: CommonResourceManager.Instance.GetResourceString("Common_UpdateQuestion"),
                                           caption: CommonResourceManager.Instance.GetResourceString("Common_UpdateCaption"), buttons: System.Windows.Forms.MessageBoxButtons.YesNo);
            });

            return (messageBox == System.Windows.Forms.DialogResult.Yes);
        }

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
            SplashWindow splashWindow = new SplashWindow();
            SplashScreen = splashWindow;

            splashWindow.Show();

            ResetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }

        #endregion
    }
}