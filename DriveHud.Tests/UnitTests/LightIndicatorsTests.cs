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

using DriveHud.Tests.UnitTests.Helpers;
using DriveHUD.Common.Resources;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Data;
using Model.Settings;
using NUnit.Framework;
using System;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class LightIndicatorsTests
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

        [Test]
        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestLightIndicators(string file)
        {
            var playerStatistic = DataServiceHelper.GetPlayerStatisticFromFile(file);

            var indicators = new Indicators(playerStatistic);
            var lightIndicators = new LightIndicators(playerStatistic);

            Assert.That(lightIndicators.BB, Is.EqualTo(indicators.BB));
            Assert.That(lightIndicators.SessionStart, Is.EqualTo(indicators.SessionStart));
            Assert.That(lightIndicators.SessionLength, Is.EqualTo(indicators.SessionLength));
            Assert.That(lightIndicators.UO_PFR_EP, Is.EqualTo(indicators.UO_PFR_EP));
            Assert.That(lightIndicators.UO_PFR_MP, Is.EqualTo(indicators.UO_PFR_MP));
            Assert.That(lightIndicators.UO_PFR_CO, Is.EqualTo(indicators.UO_PFR_CO));
            Assert.That(lightIndicators.UO_PFR_BN, Is.EqualTo(indicators.UO_PFR_BN));
            Assert.That(lightIndicators.UO_PFR_SB, Is.EqualTo(indicators.UO_PFR_SB));
            Assert.That(lightIndicators.UO_PFR_BB, Is.EqualTo(indicators.UO_PFR_BB));
            Assert.That(lightIndicators.HourOfHand, Is.EqualTo(indicators.HourOfHand));
        }

        [Test]
        [TestCase(@"..\..\UnitTests\TestData\testdata.stat")]
        public void TestReportIndicators(string file)
        {
            var playerStatistic = DataServiceHelper.GetPlayerStatisticFromFile(file);

            var indicators = new Indicators(playerStatistic);
            var reportIndicator = new ReportIndicators(playerStatistic);

            Assert.That(reportIndicator.BB, Is.EqualTo(indicators.BB));
            Assert.That(reportIndicator.SessionStart, Is.EqualTo(indicators.SessionStart));
            Assert.That(reportIndicator.SessionLength, Is.EqualTo(indicators.SessionLength));
            Assert.That(reportIndicator.UO_PFR_EP, Is.EqualTo(indicators.UO_PFR_EP));
            Assert.That(reportIndicator.UO_PFR_MP, Is.EqualTo(indicators.UO_PFR_MP));
            Assert.That(reportIndicator.UO_PFR_CO, Is.EqualTo(indicators.UO_PFR_CO));
            Assert.That(reportIndicator.UO_PFR_BN, Is.EqualTo(indicators.UO_PFR_BN));
            Assert.That(reportIndicator.UO_PFR_SB, Is.EqualTo(indicators.UO_PFR_SB));
            Assert.That(reportIndicator.UO_PFR_BB, Is.EqualTo(indicators.UO_PFR_BB));
            Assert.That(reportIndicator.HourOfHand, Is.EqualTo(indicators.HourOfHand));
            Assert.That(reportIndicator.Statistics.Count, Is.EqualTo(playerStatistic.Count));
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