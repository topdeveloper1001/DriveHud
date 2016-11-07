//-----------------------------------------------------------------------
// <copyright file="PokerStarsFastParserTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers.FastParser.PokerStars;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.IntegrationTests.Parsers.PokerStars.TestData
{
    [TestFixture]
    class PokerStarsFastParserTests
    {
        private const string TestDataFolder = @"..\..\IntegrationTests\Parsers\PokerStars\TestData";

        [Test]
        public void ParsingDoesNotThrowExceptions()
        {
            var testDataDirectoryInfo = new DirectoryInfo(TestDataFolder);

            var handHistoryFiles = testDataDirectoryInfo.GetFiles("*.txt", SearchOption.AllDirectories);

            var parser = new PokerStarsFastParserImpl();

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
        [TestCaseSource("TestTournamentData")]
        public void TestTournamentHands(TestData testData)
        {
            var parser = new PokerStarsFastParserImpl();

            var handHistory = parser.ParseFullHandHistory(testData.HandHistoryText, true);

            Assert.IsNotNull(handHistory.GameDescription.Tournament);
            AssertTournamentAreEqual(handHistory.GameDescription.Tournament, testData.ExpectedDescriptor);
        }

        protected virtual void AssertTournamentAreEqual(TournamentDescriptor actual, TournamentDescriptor expected)
        {
            if (actual == null)
            {
                throw new ArgumentNullException(nameof(actual));
            }

            if (expected == null)
            {
                throw new ArgumentNullException(nameof(expected));
            }

            var actualJson = JsonConvert.SerializeObject(actual);
            var expectedJson = JsonConvert.SerializeObject(expected);

            Assert.That(actualJson, Is.EqualTo(expectedJson));
        }

        public class TestData
        {
            public string HandHistoryText { get; set; }

            public TournamentDescriptor ExpectedDescriptor { get; set; }
        }

        private TestData[] TestTournamentData = new TestData[]
        {
            new TestData
            {
                 HandHistoryText = @"PokerStars Hand #3197395529: Tournament #6989260, $1+$0.10 USD Hold'em No Limit - Match Round I, Level V (30/60) - 2015/05/20 20:15:24
                                    Table '6989260 1' 9-max Seat #9 is the button
                                    Seat 1: Hero (1300 in chips)
                                    Seat 2: P14_493338CN (1740 in chips)
                                    Seat 3: P26_871171XV (4750 in chips)
                                    Seat 4: P27_519596DL (1100 in chips)
                                    Seat 5: P69_940032WQ (5099 in chips)
                                    Seat 6: P31_842876PN (1370 in chips)
                                    Seat 7: P48_985695NF (1410 in chips)
                                    Seat 8: P2_350540FJ (530 in chips)
                                    Hero: posts small blind 30
                                    P14_493338CN: posts big blind 60
                                    *** HOLE CARDS ***
                                    Dealt to Hero [Qc Th]
                                    P26_871171XV: folds
                                    P27_519596DL: folds
                                    P69_940032WQ: calls 60
                                    P31_842876PN: raises 230 to 290
                                    P48_985695NF: folds
                                    P2_350540FJ: folds
                                    Hero: folds
                                    P14_493338CN: folds
                                    P69_940032WQ: folds
                                    Uncalled bet (230) returned to P31_842876PN
                                    P31_842876PN collected 210 from pot
                                    *** SUMMARY ***
                                    Total pot 210 | Rake 0
                                    Board []
                                    Seat 1: Hero appropriate action description
                                    Seat 2: P14_493338CN appropriate action description
                                    Seat 3: P26_871171XV appropriate action description
                                    Seat 4: P27_519596DL appropriate action description
                                    Seat 5: P69_940032WQ appropriate action description
                                    Seat 6: P31_842876PN collected (210)
                                    Seat 7: P48_985695NF appropriate action description
                                    Seat 8: P2_350540FJ appropriate action description",
                 ExpectedDescriptor = new TournamentDescriptor
                 {
                     TournamentId = "6989260",
                     BuyIn = Buyin.FromBuyinRake(1m, 0.1m, Currency.USD)
                 }
            }
        };
    }
}