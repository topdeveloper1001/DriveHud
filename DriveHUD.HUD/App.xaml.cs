using DriveHUD.Application;
using DriveHUD.Application.Surrogates;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using DriveHUD.HUD.Services;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Interfaces;
using Model.Settings;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace DriveHUD.HUD
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application, ISingleInstanceApp
    {
        private IUnityContainer unityContainer;

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Initialize();

            var hudServiceHost = ServiceLocator.Current.GetInstance<IHudServiceHost>();
            hudServiceHost.Initialize();
            hudServiceHost.OpenHost();
        }

        public bool SignalExternalCommandLineArgs(IList<string> args)
        {
            return true;
        }

        private void Initialize()
        {
            VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
            ResourceRegistrator.Initialization();

            /* Without this user won't be able to input decimal point for float bindings if UpdateSourceTrigger set to PropertyChanged */
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            unityContainer = new UnityContainer();

            unityContainer.RegisterType<IHudServiceHost, HudServiceHost>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IDataService, DataService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));

            UnityServicesBootstrapper.ConfigureContainer(unityContainer);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            RuntimeTypeModel.Default.Add(typeof(System.Windows.Point), false).SetSurrogate(typeof(PointDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.Color), false).SetSurrogate(typeof(ColorDto));
            RuntimeTypeModel.Default.Add(typeof(System.Windows.Media.SolidColorBrush), false).SetSurrogate(typeof(SolidColorBrushDto));
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogProvider.Log.Error(this, "Unexpected error", e.Exception);

                if (!Debugger.IsAttached)
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }
    }
}