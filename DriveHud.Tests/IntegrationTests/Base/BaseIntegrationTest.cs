//-----------------------------------------------------------------------
// <copyright file="BaseIntegrationTest.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;
using System;

namespace DriveHud.Tests.IntegrationTests.Base
{
    abstract class BaseIntegrationTest
    {
        protected IDHLog customLogger;

        protected abstract string TestDataFolder { get; }

        protected virtual void Initalize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            InitializeEnvironment();
            ConfigureCustomLogger();
        }

        protected virtual void InitializeEnvironment()
        {
            var unityContainer = new UnityContainer();

            InitializeContainer(unityContainer);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);
        }

        protected abstract void InitializeContainer(UnityContainer unityContainer);

        protected virtual void ConfigureCustomLogger()
        {
            customLogger = Substitute.For<IDHLog>();
            LogProvider.SetCustomLogger(customLogger);
        }
    }
}