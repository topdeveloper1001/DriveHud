//-----------------------------------------------------------------------
// <copyright file="IPokerFastParserTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers.FastParser.IPoker;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;

namespace DriveHud.Tests.IntegrationTests.Parsers.IPoker
{
    [TestFixture]
    public class IPokerFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\IPoker\TestData";

        [OneTimeSetUp]
        public void Initialize()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        //[Test]
        //[TestCase("en-US")]
        //[TestCase("hu-HU")]
        //[TestCase("ru-RU")]
        //[TestCase("en-CA")]
        //[TestCase("fr-CA")]
        //[TestCase("zh-CN")]
        //[TestCase("zh-HK")]
        //[TestCase("zh-SG")]
        public void ParsingDoesNotThrowExceptions(string culture)
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            var cultureInfo = new CultureInfo(culture);

            Thread.CurrentThread.CurrentCulture = cultureInfo;

            var testDataDirectoryInfo = new DirectoryInfo(TestDataFolder);

            var handHistoryFiles = testDataDirectoryInfo.GetFiles("*.xml", SearchOption.AllDirectories);

            var parser = new IPokerFastParserImpl();

            var succeded = 0;
            var total = 0;

            foreach (var handHistoryFile in handHistoryFiles)
            {
                var handHistory = File.ReadAllText(handHistoryFile.FullName);

                var hands = parser.SplitUpMultipleHands(handHistory).ToArray();

                total += hands.Length;

                var hash = new HashSet<string>();

                foreach (var hand in hands)
                {
                    try
                    {
                        parser.ParseFullHandHistory(hand, true);
                        succeded++;
                    }
                    catch (Exception e)
                    {
                        if (!hash.Contains(handHistoryFile.FullName))
                        {
                            Debug.WriteLine(handHistoryFile.FullName);
                        }

                        Assert.Fail(e.ToString());
                    }
                }
            }

            Assert.AreEqual(total, succeded);

            Debug.WriteLine("Processed hands: {0}/{1}", succeded, total);
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 7668185353)]
        public void HandIdIsParsedTest(string handHistoryFile, long handId)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.HandId, Is.EqualTo(handId));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", GameType.NoLimitHoldem)]
        public void GameTypeIsParsedTest(string handHistoryFile, GameType gameType)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(gameType));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 0.02)]
        public void BigBlindIsParsedTest(string handHistoryFile, decimal bigBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.BigBlind, Is.EqualTo(bigBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 0.01)]
        public void SmallBlindIsParsedTest(string handHistoryFile, decimal smallBlind)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.SmallBlind, Is.EqualTo(smallBlind));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", Currency.EURO)]        
        public void CurrencyIsParsedTest(string handHistoryFile, Currency currency)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Limit.Currency, Is.EqualTo(currency));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", PokerFormat.CashGame)]
        public void PokerFormatIsParsedTest(string handHistoryFile, PokerFormat pokerFormat)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(pokerFormat));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 6)]
        public void SeatTypeMaxPlayersIsParsedTest(string handHistoryFile, int maxPlayers)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.SeatType.MaxPlayers, Is.EqualTo(maxPlayers));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", EnumPokerSites.IPoker)]
        public void SiteIsParsedTest(string handHistoryFile, EnumPokerSites pokerSite)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.GameDescription.Site, Is.EqualTo(pokerSite));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", "Peon384")]
        public void HeroPlayerNameIsParsedTest(string handHistoryFile, string heroPlayerName)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.PlayerName, Is.EqualTo(heroPlayerName));
        }

        [Test]
        [TestCase(@"..\..\IntegrationTests\Parsers\IPoker\SingleHands\NLH-6-max.xml", 8)]
        public void HeroSeatNumberIsParsedTest(string handHistoryFile, int seatNumber)
        {
            var handHistory = ParseHandHistory(handHistoryFile);
            Assert.That(handHistory.Hero.SeatNumber, Is.EqualTo(seatNumber));
        }



        private HandHistory ParseHandHistory(string handHistoryFile)
        {
            var parser = new IPokerFastParserImpl();

            var handHistoryText = File.ReadAllText(handHistoryFile);

            var hands = parser.SplitUpMultipleHands(handHistoryText).ToArray();

            var hand = hands.Single();

            var handHistory = parser.ParseFullHandHistory(hand, true);

            return handHistory;
        }

        private void ConfigureContainer()
        {
            var unityContainer = new UnityContainer();
            unityContainer.RegisterType<IEventAggregator, EventAggregator>();

            var locator = new UnityServiceLocator(unityContainer);
            ServiceLocator.SetLocatorProvider(() => locator);
        }
    }
}