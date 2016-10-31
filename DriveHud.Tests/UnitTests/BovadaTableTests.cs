//-----------------------------------------------------------------------
// <copyright file="BovadaTableTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Progress;
using DriveHUD.Entities;
using DriveHUD.Importers;
using DriveHUD.Importers.BetOnline;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Builders.iPoker;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Model.Site;
using Newtonsoft.Json;
using NSubstitute;
using NUnit.Framework;
using Prism.Events;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class BovadaTableTests
    {
        private IUnityContainer unityContainer;

        [SetUp]
        public void SetUp()
        {
            unityContainer = new UnityContainer();

            unityContainer.RegisterType<ICardsConverter, PokerCardsConverter>();
            unityContainer.RegisterType<ITournamentsCacheService, TournamentsCacheService>();
            unityContainer.RegisterType<ISiteConfiguration, BetOnlineConfiguration>(EnumPokerSites.BetOnline.ToString());
            unityContainer.RegisterType<ISiteConfiguration, BovadaConfigurationStub>(EnumPokerSites.Ignition.ToString());
            unityContainer.RegisterType<ISiteConfiguration, PokerStarsConfiguration>(EnumPokerSites.PokerStars.ToString());
            unityContainer.RegisterType<ISiteConfiguration, TigerGamingConfiguration>(EnumPokerSites.TigerGaming.ToString());
            unityContainer.RegisterType<ISiteConfiguration, SportsBettingConfiguration>(EnumPokerSites.SportsBetting.ToString());
            unityContainer.RegisterType<ISiteConfigurationService, SiteConfigurationService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IFileImporter, FileImporterStub>(new ContainerControlledLifetimeManager());

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configurationService.Initialize();
        }

        /// <summary>
        /// Convert original data from injector in iPoker format (for manual checks, because some data are randomized, so need to develop smart comparer)
        /// </summary>        
        [Test]
        [TestCase("bovada-MTT-moving")]
        public void TestProcessCommand(string testData)
        {
            // Read source
            var source = File.ReadAllLines(GetTestDataFilePath(testData));
            // Convert source to list of commands
            var bovadaCatcherDataObjects = SplitByDataObjects(source);

            var eventAggregator = Substitute.For<IEventAggregator>();
            var importedEvent = new DataImportedEvent();
            eventAggregator.GetEvent<DataImportedEvent>().ReturnsForAnyArgs(importedEvent);

            var bovadaTable = new BovadaTable(eventAggregator);

            foreach (var bovadaCatcherDataObject in bovadaCatcherDataObjects)
            {
                bovadaTable.ProcessCommand(bovadaCatcherDataObject);
            }

            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>() as FileImporterStub;

            var xml = fileImporter.Xml;

            Assert.IsNotNull(xml);
        }

        private IEnumerable<BovadaCatcherDataObject> SplitByDataObjects(string[] textLines)
        {
            var sb = new StringBuilder();

            var bovadaCatcherDataObjects = new List<BovadaCatcherDataObject>();

            foreach (var textLine in textLines)
            {
                if (string.IsNullOrWhiteSpace(textLine))
                {
                    var dataText = sb.ToString();
                    var catcherDataObject = JsonConvert.DeserializeObject<BovadaCatcherDataObject>(dataText);

                    bovadaCatcherDataObjects.Add(catcherDataObject);

                    sb.Clear();
                }

                sb.AppendLine(textLine);
            }

            return bovadaCatcherDataObjects;
        }

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\Bovada\\{0}.log", name);
        }

        private class FileImporterStub : IFileImporter
        {
            public XDocument Xml
            {
                get;
                set;
            }

            public void Import(DirectoryInfo directory, IDHProgress progress)
            {
            }

            public void Import(FileInfo[] files, IDHProgress progress)
            {
            }

            public void Import(FileInfo file, IDHProgress progress)
            {
            }

            public IEnumerable<ParsingResult> Import(string text, IDHProgress progress, GameInfo gameInfo)
            {
                var handXml = XDocument.Parse(text);

                if (Xml == null)
                {
                    Xml = handXml;
                    return null;
                }

                var sessionNode = Xml.Descendants("session").FirstOrDefault();
                var gameNode = handXml.Descendants("game").FirstOrDefault();

                sessionNode.Add(gameNode);

                return null;
            }
        }

        private class BovadaConfigurationStub : BovadaConfiguration
        {
            public override Dictionary<int, int> PreferredSeats
            {
                get
                {
                    return new Dictionary<int, int>(); ;
                }
            }
        }
    }
}