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

using DriveHUD.Application;
using DriveHUD.Application.Surrogates;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.HUD.Services;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model;
using Model.Interfaces;
using Model.Settings;
using Prism.Events;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            VisualStudio2013Palette.LoadPreset(VisualStudio2013Palette.ColorVariation.Dark);
            ResourceRegistrator.Initialization();

            /* Without this user won't be able to input decimal point for float bindings if UpdateSourceTrigger set to PropertyChanged */
            System.Windows.FrameworkCompatibilityPreferences.KeepTextBoxDisplaySynchronizedWithTextProperty = false;

            unityContainer = new UnityContainer();

            unityContainer.RegisterType<IEventAggregator, EventAggregator>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IHudServiceHost, HudServiceHost>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IDataService, DataService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ISettingsService, SettingsService>(new ContainerControlledLifetimeManager(), new InjectionConstructor(StringFormatter.GetAppDataFolderPath()));
            unityContainer.RegisterType<IHandHistoryParserFactory, HandHistoryParserFactoryImpl>();

            UnityServicesBootstrapper.ConfigureContainer(unityContainer);
            ModelBootstrapper.ConfigureContainer(unityContainer);

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
                LogProvider.Log.Error(this, "Unexpected fatal error", e.Exception);

                if (!Debugger.IsAttached)
                {
                    e.Handled = true;
                }
            }
            catch
            {
            }
        }

        private void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            LogProvider.Log.Error(this, "Unexpected domain fatal error", ex);
        }
    }
}