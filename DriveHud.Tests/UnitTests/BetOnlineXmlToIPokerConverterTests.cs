//-----------------------------------------------------------------------
// <copyright file="BetOnlineXmlToIPokerConverterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Builders.iPoker;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Site;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml.Linq;

namespace DriveHud.Tests
{
    [TestFixture]
    public class BetOnlineXmlToIPokerXmlConverterTests
    {      
        private const int SessionCode = 7777777;

        private class TableServiceStub : IBetOnlineTableService
        {
            public bool IsRunning
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public event EventHandler ProcessStopped;

            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public string GetRandomPlayerName(string sessionCode, int seat)
            {
                return Utils.GenerateRandomPlayerName(seat);
            }

            public int GetSessionCode(string tableName, out EnumPokerSites site)
            {
                site = EnumPokerSites.BetOnline;
                return SessionCode;
            }

            public int GetWindowHandle(ulong handId, out EnumPokerSites site)
            {
                site = EnumPokerSites.BetOnline;
                return SessionCode;
            }

            public void Reset()
            {
            }

            public void ResetCache()
            {
            }

            public void Start()
            {
                throw new NotImplementedException();
            }

            public void Stop()
            {
                throw new NotImplementedException();
            }
        }

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var unityContainer = new UnityContainer();

            unityContainer.RegisterType<ICardsConverter, PokerCardsConverter>();
            unityContainer.RegisterType<ITournamentsCacheService, TournamentsCacheService>();
            unityContainer.RegisterType<IBetOnlineTableService, TableServiceStub>();
            unityContainer.RegisterType<ISiteConfiguration, BovadaConfiguration>(EnumPokerSites.Ignition.ToString());
            unityContainer.RegisterType<ISiteConfiguration, BetOnlineConfiguration>(EnumPokerSites.BetOnline.ToString());
            unityContainer.RegisterType<ISiteConfiguration, TigerGamingConfiguration>(EnumPokerSites.TigerGaming.ToString());
            unityContainer.RegisterType<ISiteConfiguration, SportsBettingConfiguration>(EnumPokerSites.SportsBetting.ToString());
            unityContainer.RegisterType<ISiteConfiguration, PokerStarsConfiguration>(EnumPokerSites.PokerStars.ToString());
            unityContainer.RegisterType<ISiteConfiguration, Poker888Configuration>(EnumPokerSites.Poker888.ToString());
            unityContainer.RegisterType<ISiteConfiguration, AmericasCardroomConfiguration>(EnumPokerSites.AmericasCardroom.ToString());
            unityContainer.RegisterType<ISiteConfiguration, BlackChipPokerConfiguration>(EnumPokerSites.BlackChipPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, TruePokerConfiguration>(EnumPokerSites.TruePoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, YaPokerConfiguration>(EnumPokerSites.YaPoker.ToString());
            unityContainer.RegisterType<ISiteConfigurationService, SiteConfigurationService>(new ContainerControlledLifetimeManager());

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configurationService.Initialize();
        }

        [Test]
        [TestCase("CashHand-10-max-potsize-error", "CashHand-10-max-potsize-error-ipoker")]
        [TestCase("CashHand-10-max", "CashHand-10-max-ipoker")]
        [TestCase("CashHand-2-max", "CashHand-2-max-ipoker")]
        [TestCase("SnGHand-8-max", "SnGHand-8-max-ipoker")]
        [TestCase("CashHand-10-max-relocate", "CashHand-10-max-relocate-ipoker")]
        [TestCase("CashOmaha-10-max-big-rake-error", "CashOmaha-10-max-big-rake-error-ipoker")]
        [TestCase("MTT-Holdem-10-max-invalid-relocation", "MTT-Holdem-10-max-invalid-relocation-ipoker")]
        public void TestConverter(string sourceXmlFile, string expectedXmlFile)
        {
            var source = File.ReadAllText(GetTestDataFilePath(sourceXmlFile));

            var expected = XDocument.Load(GetTestDataFilePath(expectedXmlFile));

            var betOnlineXmlToIPokerConverter = new BetOnlineXmlToIPokerXmlConverter();         

            var convertedResult = betOnlineXmlToIPokerConverter.Convert(source);

            Assert.IsNotNull(convertedResult);

            var actual = XDocument.Parse(convertedResult.ConvertedXml);

            var result = XNode.DeepEquals(expected, actual);

            Assert.IsTrue(result);
        }

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\{0}.xml", name);
        }
    }
}