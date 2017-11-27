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
using System;
using DriveHud.Tests.UnitTests.Helpers;
using Model.Settings;
using DriveHUD.Common.Resources;
using System.Text.RegularExpressions;
using System.Xml;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class IgnitionTableTests
    {
        private IUnityContainer unityContainer;

        [SetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            ResourceRegistrator.Initialization();

            unityContainer = new UnityContainer();

            unityContainer.RegisterType<ICardsConverter, PokerCardsConverter>();
            unityContainer.RegisterType<IIgnitionWindowCache, IgnitionWindowCache>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<ITournamentsCacheService, TournamentsCacheService>();
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
            unityContainer.RegisterType<ISiteConfiguration, IPokerConfiguration>(EnumPokerSites.IPoker.ToString());
            unityContainer.RegisterType<ISiteConfiguration, GGNConfiguration>(EnumPokerSites.GGN.ToString());
            unityContainer.RegisterType<ISiteConfiguration, PartyPokerConfiguration>(EnumPokerSites.PartyPoker.ToString());
            unityContainer.RegisterType<ISiteConfigurationService, SiteConfigurationService>(new ContainerControlledLifetimeManager());
            unityContainer.RegisterType<IFileImporter, FileImporterStub>(new ContainerControlledLifetimeManager());

            var eventAggregator = Substitute.For<IEventAggregator>();
            var ignitionInfoDataManager = new IgnitionInfoDataManagerStub(eventAggregator);

            var ignitionInfoImporter = Substitute.For<IIgnitionInfoImporter>();
            ignitionInfoImporter.InfoDataManager.Returns(ignitionInfoDataManager);

            var importerService = Substitute.For<IImporterService>();
            importerService.GetRunningImporter<IIgnitionInfoImporter>().Returns(ignitionInfoImporter);
            unityContainer.RegisterInstance(importerService);

            var settingsService = Substitute.For<ISettingsService>();
            settingsService.GetSettings().Returns(new SettingsModel());
            unityContainer.RegisterInstance(settingsService);

            var locator = new UnityServiceLocator(unityContainer);

            ServiceLocator.SetLocatorProvider(() => locator);

            var configurationService = ServiceLocator.Current.GetInstance<ISiteConfigurationService>();
            configurationService.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>() as FileImporterStub;
            fileImporter.Clear();

            var infoDataManager = GetInfoDataManager();
            infoDataManager.Clear();
        }

        /// <summary>
        /// Convert original data from injector in iPoker format (for manual checks, because some data are randomized, so need to develop smart comparer)
        /// </summary>        
        [Test]
        [TestCase("ign-zone-2017-11-21.log", "ign-info.log", "ign-zone-2017-11-21.xml")]
        [TestCase("ign-zone-2017-11-21-2.log", "ign-info.log", "ign-zone-2017-11-21-2.xml")]
        [TestCase("ign-zone-2017-11-22-1.log", "ign-info.log", "ign-zone-2017-11-22-1.xml")]
        [TestCase("ign-zone-2017-11-22-2.log", "ign-info.log", "ign-zone-2017-11-22-2.xml")]
        [TestCase("ign-zone-2017-11-22-3.log", "ign-info.log", "ign-zone-2017-11-22-3.xml")]
        [TestCase("ign-zone-2017-11-24.log", "ign-info.log", "ign-zone-2017-11-24.xml")]
        [TestCase("ign-zone-2017-11-24-2.log", "ign-info.log", "ign-zone-2017-11-24-2.xml")]
        public void ZoneHandsIsImported(string testData, string infoTestData, string expectedFile)
        {
            // initialize info manager with test data
            InitializeInfoDataManager(infoTestData);

            // Read source
            var source = File.ReadAllLines(GetTestDataFilePath(testData));

            // Convert source to list of commands
            var bovadaCatcherDataObjects = SplitByDataObjects(source);

            var importedEvent = new DataImportedEvent();

            var eventAggregator = Substitute.For<IEventAggregator>();
            eventAggregator.GetEvent<DataImportedEvent>().ReturnsForAnyArgs(importedEvent);

            var ignitionTable = new IgnitionTableStub(eventAggregator);

            foreach (var bovadaCatcherDataObject in bovadaCatcherDataObjects)
            {
                ignitionTable.ProcessCommand(bovadaCatcherDataObject);
            }

            var fileImporter = ServiceLocator.Current.GetInstance<IFileImporter>() as FileImporterStub;

            var xml = fileImporter.Xml;

            Assert.IsNotNull(xml, "Result must be not null");

            var actualXml = ObfuscateXml(xml.ToString());
            var expectedXml = ObfuscateXml(File.ReadAllText(GetTestDataFilePath(expectedFile)));

            Assert.That(actualXml, Is.EqualTo(expectedXml), "Xml must be equal to expected.");
        }

        private void InitializeInfoDataManager(string fileName)
        {
            var infoDataManager = GetInfoDataManager();

            var infoData = IgnitionTestsHelper.PrepareInfoTestData(GetTestDataFilePath(fileName));

            foreach (var data in infoData)
            {
                infoDataManager.ProcessData(data);
            }
        }

        private IgnitionInfoDataManagerStub GetInfoDataManager()
        {
            var importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            var ignitionInfoImporter = importerService.GetRunningImporter<IIgnitionInfoImporter>();

            var infoDataManager = ignitionInfoImporter.InfoDataManager as IgnitionInfoDataManagerStub;

            return infoDataManager;
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
                    var catcherDataObject = JsonConvert.DeserializeObject<BovadaCatcherDataObjectWithTime>(dataText);

                    bovadaCatcherDataObjects.Add(catcherDataObject);

                    sb.Clear();
                }

                if (textLine.StartsWith("\"timestamp\""))
                {
                    sb.AppendLine(textLine + ",");
                    continue;
                }

                if (Regex.IsMatch(textLine, @"\d{2}:\d{2}:\d{2}"))
                {
                    continue;
                }

                sb.AppendLine(textLine);
            }

            return bovadaCatcherDataObjects;
        }

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\Ignition\\{0}", name);
        }

        private static readonly DateTime predefinedDate = new DateTime(2017, 11, 22);

        private static string ObfuscateXml(string xml)
        {
            xml = Regex.Replace(xml, "<startdate>[^<]+</startdate>", $"<startdate>{predefinedDate}</startdate>");
            xml = Regex.Replace(xml, @"(name|player)=""P(\d+)_[^""]+", "$1=\"P$2");

            return xml;
        }

        #region Stubs

        public class BovadaCatcherDataObjectWithTime : BovadaCatcherDataObject
        {
            public string timestamp { get; set; }
        }

        private class FileImporterStub : IFileImporter
        {
            private static readonly object locker = new object();

            public FileImporterStub()
            {
                ImportedHands = new List<ImportedHand>();
            }

            public XDocument Xml
            {
                get;
                set;
            }

            public List<ImportedHand> ImportedHands
            {
                get;
                private set;
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
                lock (locker)
                {
                    var handXml = XDocument.Parse(text);

                    ImportedHands.Add(new ImportedHand
                    {
                        HandXml = handXml,
                        GameInfo = gameInfo
                    });

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

            public void Clear()
            {
                Xml = null;
                ImportedHands.Clear();
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

        private class ImportedHand
        {
            public XDocument HandXml { get; set; }

            public GameInfo GameInfo { get; set; }

            public override string ToString()
            {
                return GameInfo.GameNumber.ToString();
            }
        }

        private class IgnitionTableStub : IgnitionTable
        {
            public IgnitionTableStub(IEventAggregator eventAggregator) : base(eventAggregator)
            {
            }

            protected override void ImportHandAsync(XmlDocument handHistoryXml, ulong handNumber, GameInfo gameInfo, Game game)
            {
                ImportHand(handHistoryXml, handNumber, gameInfo, game);
            }
        }

        #endregion
    }
}