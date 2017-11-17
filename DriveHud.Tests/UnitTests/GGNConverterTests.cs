//-----------------------------------------------------------------------
// <copyright file="GGNConverterTests.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.GGNetwork;
using DriveHUD.Importers.GGNetwork.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class GGNConverterTests
    {
        private IGGNCacheService cacheService;

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

            InitializeCacheService();
        }

        [Test]
        [TestCase("TournamentHandHistory.json", "TournamentHandHistoryExpected.xml")]
        [TestCase("Tourn-hand-117999074.json", "Tourn-hand-117999074.xml")]
        [TestCase("Tourn-hand-118033657.json", "Tourn-hand-118033657.xml")]
        [TestCase("Tourn-hand-118044556.json", "Tourn-hand-118044556.xml")]
        public void HandHistoryInfoIsConvertered(string actualJsonFile, string expectedXmlFile)
        {
            var handHistoryJson = File.ReadAllText(GetTestDataFilePath(actualJsonFile));
            var handHistoryInfo = GGNUtils.DeserializeHandHistory(handHistoryJson);

            var handHistory = GGNConverter.ConvertHandHistory(handHistoryInfo.HandHistory, cacheService);

            var expectedHandHistoryXml = File.ReadAllText(GetTestDataFilePath(expectedXmlFile));

            var expectedHandHistory = SerializationHelper.DeserializeObject<HandHistories.Objects.Hand.HandHistory>(expectedHandHistoryXml);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(handHistory.GameDescription.Tournament, "TournamentDescriptor must be not null");
                Assert.That(handHistory.DateOfHandUtc, Is.EqualTo(expectedHandHistory.DateOfHandUtc), "DateOfHandUtc must be the same");
                Assert.That(handHistory.HandId, Is.EqualTo(expectedHandHistory.HandId), "HandId must be the same");
                Assert.That(handHistory.DealerButtonPosition, Is.EqualTo(expectedHandHistory.DealerButtonPosition), "DealerButtonPosition must be the same");
                Assert.That(handHistory.TableName, Is.EqualTo(expectedHandHistory.TableName), "TableName must be the same");
                Assert.That(handHistory.GameDescription.PokerFormat, Is.EqualTo(expectedHandHistory.GameDescription.PokerFormat), "GameDescription.PokerFormat must be the same");
                Assert.That(handHistory.GameDescription.Site, Is.EqualTo(expectedHandHistory.GameDescription.Site), "GameDescription.Site must be the same");
                Assert.That(handHistory.GameDescription.GameType, Is.EqualTo(expectedHandHistory.GameDescription.GameType), "GameDescription.GameType must be the same");
                Assert.That(handHistory.GameDescription.Limit, Is.EqualTo(expectedHandHistory.GameDescription.Limit), "GameDescription.Limit must be the same");
                Assert.That(handHistory.GameDescription.SeatType, Is.EqualTo(expectedHandHistory.GameDescription.SeatType), "GameDescription.SeatType must be the same");
                Assert.That(handHistory.TotalPot, Is.EqualTo(expectedHandHistory.TotalPot), "TotalPot must be the same");
                Assert.That(handHistory.Rake, Is.EqualTo(expectedHandHistory.Rake), "Rake must be the same");
                Assert.That(handHistory.CommunityCards, Is.EqualTo(expectedHandHistory.CommunityCards), "CommunityCards must be the same");

                // compare tournaments data
                if (expectedHandHistory.GameDescription.Tournament != null)
                {
                    Assert.That(handHistory.GameDescription.Tournament.TournamentId, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.TournamentId), "GameDescription.Tournament.TournamentId must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.TournamentInGameId, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.TournamentInGameId), "GameDescription.Tournament.TournamentInGameId must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.TournamentName, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.TournamentName), "GameDescription.Tournament.TournamentName must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.BuyIn, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.BuyIn), "GameDescription.Tournament.BuyIn must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.Bounty, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.Bounty), "GameDescription.Tournament.Bounty must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.Rebuy, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.Rebuy), "GameDescription.Tournament.Rebuy must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.Addon, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.Addon), "GameDescription.Tournament.Addon must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.Winning, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.Winning), "GameDescription.Tournament.Winning must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.FinishPosition, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.FinishPosition), "GameDescription.Tournament.FinishPosition must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.TotalPlayers, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.TotalPlayers), "GameDescription.Tournament.TotalPlayers must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.StartDate, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.StartDate), "GameDescription.Tournament.StartDate must be the same");
                    Assert.That(handHistory.GameDescription.Tournament.Speed, Is.EqualTo(expectedHandHistory.GameDescription.Tournament.Speed), "GameDescription.Tournament.Speed must be the same");
                }

                var actualHandHistoryXml = SerializationHelper.SerializeObject(handHistory);

                Assert.That(actualHandHistoryXml, Is.EqualTo(expectedHandHistoryXml), "Actual xml must match expected xml");
            });
        }

        private void InitializeCacheService()
        {
            var tournamentsInfoJson = File.ReadAllText(GetTestDataFilePath("TournamentsInfo.json"));

            var tournamentsInfoResponse = JsonConvert.DeserializeObject<TournamentInfoResponse>(tournamentsInfoJson);

            cacheService = new GGNCacheServiceStub(tournamentsInfoResponse.Tournaments);
        }

        private static string GetTestDataFilePath(string name)
        {
            return string.Format("UnitTests\\TestData\\GGNJsonData\\{0}", name);
        }

        private class GGNCacheServiceStub : IGGNCacheService
        {
            private Dictionary<string, TournamentInformation> tournamentsCache;

            public GGNCacheServiceStub(IEnumerable<TournamentInformation> tournaments)
            {
                tournamentsCache = tournaments.ToDictionary(x => x.Id);
            }

            public void Clear()
            {
                throw new NotImplementedException();
            }

            public TournamentInformation GetTournament(string tournamentId)
            {
                if (tournamentsCache.ContainsKey(tournamentId))
                {
                    return tournamentsCache[tournamentId];
                }

                return null;
            }

            public Task RefreshAsync()
            {
                throw new NotImplementedException();
            }
        }
    }
}