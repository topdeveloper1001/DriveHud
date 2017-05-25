//-----------------------------------------------------------------------
// <copyright file="LightIndicatorsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Settings;
using NUnit.Framework;
using System;

namespace DriveHud.Tests.UnitTests
{
    class BaseIndicatorsTests
    {
        IUnityContainer container;

        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            ResourceRegistrator.Initialization();

            container = new UnityContainer();

            container.RegisterType<ISettingsService, SettingsServiceStub>();

            UnityServiceLocator locator = new UnityServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        protected class SettingsServiceStub : ISettingsService
        {
            public SettingsModel GetSettings()
            {
                var settingsModel = new SettingsModel();
                settingsModel.GeneralSettings.TimeZoneOffset = 0;
                return settingsModel;
            }

            public void SaveSettings(SettingsModel settings)
            {
                throw new NotImplementedException();
            }
        }
    }
}