//-----------------------------------------------------------------------
// <copyright file="HandBuilderTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.PokerMaster;
using DriveHUD.Importers.PokerMaster.Model;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMCatcher.Tests
{
    [TestFixture]
    class HandBuilderTests
    {
        private const string TestDataFolder = "PMTests\\TestData\\HandsRawData";
        private const string JsonExt = ".json";
        private const string ExpectedResultFile = "ExpectedResult.xml";

        [OneTimeSetUp]
        public void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        /*** Testcases to add *** 
         * 1. SnG hands (done)                            
         * 2. MTT hands (not done)
         * 3. 6-max (done)
         * 4. Uncalled bets (done)
         * 5. Straddle hands (done)
         * 6. Side pots (not done)
         */
        [TestCase("HU-1", 1044518, false)]
        [TestCase("HU-2", 1044518, false)]
        [TestCase("HU-3", 1044518, false)]
        [TestCase("HU-4", 1044518, false)]
        [TestCase("HU-5", 1044518, false)]
        [TestCase("SNG-HU-1", 1044518, false)]
        [TestCase("6-max-Straddle-1", 1044518, true)]
        [TestCase("6-max-Straddle-2", 1044518, true)]
        [TestCase("6-max-Omaha-1", 1044518, true)]
        [TestCase("6-max-Omaha-2", 1044518, true)]
        [TestCase("6-max-Omaha-3", 1044518, true)]
        [TestCase("6-max-Omaha-4", 1044518, true)]
        public void TryBuildTest(string dataFolder, long heroId, bool isStringEnum)
        {
            var jsonTestData = GetJsonFileFromFolder(dataFolder, isStringEnum);

            var handBuilder = new HandBuilder();

            var result = false;
            HandHistory actual = null;

            foreach (var gameRoomStateChange in jsonTestData.GameRoomStateChanges)
            {
                var res = JsonConvert.SerializeObject(gameRoomStateChange, new StringEnumConverter());

                foreach (var c in gameRoomStateChange.GameRoomInfo.UserGameInfos)
                {
                    if (!c.IsActive)
                    {
                        continue;
                    }

                    Console.WriteLine($"{c.GameState}, {c.UserInfo.Nick}: {DateTimeHelper.UnixTimeToDateTime(c.ActTime / 1000)}");
                }

                result = handBuilder.TryBuild(gameRoomStateChange, heroId, out actual);

                if (result)
                {
                    break;
                }
            }

            Assert.IsTrue(result, "Result must be true");
            Assert.IsNotNull(actual, "Hand history must be built as a result.");

            var xx = SerializationHelper.SerializeObject(actual);

            AssertHandHistory(actual, jsonTestData.ExpectedResult);
        }

        //[Test]
        public void CreateSample()
        {
            var handHistory = new HandHistory
            {
                DateOfHandUtc = new DateTime(2018, 2, 15, 7, 35, 39, DateTimeKind.Utc),
                HandId = 274421590014,
                DealerButtonPosition = 1,
                TableName = "test1",
                GameDescription = new GameDescriptor(EnumPokerSites.PokerMaster,
                    GameType.NoLimitHoldem,
                    Limit.FromSmallBlindBigBlind(1, 2, Currency.YUAN),
                    TableType.FromTableTypeDescriptions(new TableTypeDescription[] { TableTypeDescription.Regular, TableTypeDescription.SuperSpeed }),
                    SeatType.FromMaxPlayers(2),
                    null),
                Rake = 0,
                TotalPot = 0,
                CommunityCards = BoardCards.FromCards("3s 4h 8d Js 5s"),
                Players = new PlayerList
                {
                    new Player("Alex84", 206, 1),
                    new Player("Peon", 194, 2)
                },
                HandActions = new List<HandAction>
                {
                    new HandAction("Alex84", HandActionType.SMALL_BLIND, 1, Street.Preflop, 0),
                    new HandAction("Peon", HandActionType.BIG_BLIND, 2, Street.Preflop, 1),
                    new HandAction("Alex84", HandActionType.CALL, 1, Street.Preflop, 2),
                    new HandAction("Peon", HandActionType.CHECK, 0, Street.Preflop, 3),
                    new HandAction("Peon", HandActionType.CHECK, 0, Street.Flop, 4),
                    new HandAction("Alex84", HandActionType.CHECK, 0, Street.Flop, 5),
                    new HandAction("Peon", HandActionType.CHECK, 0, Street.Turn, 6),
                    new HandAction("Alex84", HandActionType.CHECK, 0, Street.Turn, 7),
                    new HandAction("Peon", HandActionType.CHECK, 0, Street.River, 8),
                    new HandAction("Alex84", HandActionType.BET, 2, Street.River, 9),
                    new HandAction("Peon", HandActionType.FOLD, 0, Street.River, 10),
                    new WinningsAction("Alex84", HandActionType.WINS, 6, 0),
                }
            };

            var result = SerializationHelper.SerializeObject(handHistory);
            var ds = SerializationHelper.DeserializeObject<HandHistory>(result);

            CollectionAssert.AreEquivalent(handHistory.GameDescription.TableTypeDescriptors, ds.GameDescription.TableTypeDescriptors);

            Assert.IsNotEmpty(result);
        }

        //[Test]
        //[TestCase(@"d:\Git\DriveHUD\DriveHUD.Application\bin\Debug\Hands\hand_imported_1044518_277964540025.json", @"d:\Git\DriveHUD\DriveHud.Tests\PMTests\TestData\HandsRawData\6-max-Omaha-4")]
        //[TestCase(@"d:\Git\PMCatcher\PMCatcher\bin\Debug\Hands\hand_imported_1044518_275254740013.json", @"d:\Git\PMCatcher\PMCatcher.Tests\TestData\HandsRawData\SNG-HU-2")]
        public void SplitTestToFile(string file, string folder)
        {
            if (Directory.Exists(folder) && Directory.EnumerateFiles(folder).Any())
            {
                Directory.Delete(folder, true);
            }

            Directory.CreateDirectory(folder);

            if (!File.Exists(file))
            {
                throw new FileNotFoundException(file);
            }

            var fileText = File.ReadAllText(file);

            var splittedFileText = fileText.Split(new[] { "}{" }, StringSplitOptions.None);

            string GetStateTxt(object state)
            {
                var stateText = state.ToString();
                var indexOfUndescore = stateText.LastIndexOf("_");
                stateText = FirstLetterToUpperCase(stateText.Substring(indexOfUndescore + 1).ToLower());
                return stateText;
            }

            for (var index = 0; index < splittedFileText.Length; index++)
            {
                var jsonText = splittedFileText[index];

                var json = index == 0 ?
                    $"{jsonText}}}" :
                    (index == splittedFileText.Length - 1 ? $"{{{jsonText}" : $"{{{jsonText}}}");

                var scGameRoomStateChange = JsonConvert.DeserializeObject<SCGameRoomStateChange>(json);

                var indexStr = index + 1 < 10 ? $"0{index + 1}" : (index + 1).ToString();
                var newFile = Path.Combine(folder, $"{indexStr}-{GetStateTxt(scGameRoomStateChange.GameRoomInfo.GameState)}.json");

                var newJson = JsonConvert.SerializeObject(scGameRoomStateChange, Formatting.Indented, new StringEnumConverter());

                File.WriteAllText(newFile, newJson);
            }
        }

        /// <summary>
        /// Returns the input string with the first character converted to uppercase
        /// </summary>
        public static string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("There is no first letter");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        private void AssertHandHistory(HandHistory actual, HandHistory expected)
        {
            Assert.Multiple(() =>
            {
                Assert.That(actual.DateOfHandUtc, Is.EqualTo(expected.DateOfHandUtc), "DateOfHandUtc must be equal");
                Assert.That(actual.HandId, Is.EqualTo(expected.HandId), "HandId must be equal");
                Assert.That(actual.DealerButtonPosition, Is.EqualTo(expected.DealerButtonPosition), "DealerButtonPosition must be equal");
                Assert.That(actual.TableName, Is.EqualTo(expected.TableName), "TableName must be equal");
                Assert.That(actual.GameDescription.PokerFormat, Is.EqualTo(expected.GameDescription.PokerFormat), "GameDescription.PokerFormat must be equal");
                Assert.That(actual.GameDescription.Site, Is.EqualTo(expected.GameDescription.Site), "GameDescription.Site must be equal");
                Assert.That(actual.GameDescription.GameType, Is.EqualTo(expected.GameDescription.GameType), "GameDescription.GameType must be equal");
                Assert.That(actual.GameDescription.Limit, Is.EqualTo(expected.GameDescription.Limit), "GameDescription.Limit must be equal");
                Assert.That(actual.GameDescription.SeatType, Is.EqualTo(expected.GameDescription.SeatType), "GameDescription.SeatType must be equal");
                Assert.That(actual.TotalPot, Is.EqualTo(expected.TotalPot), "TotalPot must be equal");
                Assert.That(actual.Rake, Is.EqualTo(expected.Rake), "Rake must be equal");
                Assert.That(actual.CommunityCards, Is.EqualTo(expected.CommunityCards), "CommunityCards must be equal");
                Assert.That(actual.HeroName, Is.EqualTo(expected.HeroName), "HeroName must be equal");

                Assert.That(actual.GameDescription.GameType, Is.EqualTo(expected.GameDescription.GameType), "GameDescription.GameType must be equal");
                Assert.That(actual.GameDescription.PokerFormat, Is.EqualTo(expected.GameDescription.PokerFormat), "GameDescription.PokerFormat must be equal");
                Assert.That(actual.GameDescription.Site, Is.EqualTo(expected.GameDescription.Site), "GameDescription.Site must be equal");
                Assert.That(actual.GameDescription.IsStraddle, Is.EqualTo(expected.GameDescription.IsStraddle), "GameDescription.IsStraddle must be equal");
                Assert.That(actual.GameDescription.SeatType.MaxPlayers, Is.EqualTo(expected.GameDescription.SeatType.MaxPlayers), "GameDescription.SeatType.MaxPlayers must be equal");
                Assert.That(actual.GameDescription.Limit.Ante, Is.EqualTo(expected.GameDescription.Limit.Ante), "GameDescription.Limit.Ante must be equal");
                Assert.That(actual.GameDescription.Limit.BigBlind, Is.EqualTo(expected.GameDescription.Limit.BigBlind), "GameDescription.Limit.BigBlind must be equal");
                Assert.That(actual.GameDescription.Limit.Currency, Is.EqualTo(expected.GameDescription.Limit.Currency), "GameDescription.Limit.Currency must be equal");
                Assert.That(actual.GameDescription.Limit.IsAnteTable, Is.EqualTo(expected.GameDescription.Limit.IsAnteTable), "GameDescription.Limit.IsAnteTable must be equal");
                Assert.That(actual.GameDescription.Limit.SmallBlind, Is.EqualTo(expected.GameDescription.Limit.SmallBlind), "GameDescription.Limit.SmallBlind must be equal");
                CollectionAssert.AreEquivalent(actual.GameDescription.TableType, expected.GameDescription.TableType, "GameDescription.TableType must be equivalent");
                CollectionAssert.AreEquivalent(actual.GameDescription.TableTypeDescriptors, expected.GameDescription.TableTypeDescriptors, "GameDescription.TableTypeDescriptors must be equivalent");
                Assert.That(actual.GameDescription.IsTournament, Is.EqualTo(expected.GameDescription.IsTournament), "GameDescription.IsTournament must be equal");

                if (actual.GameDescription.IsTournament)
                {
                    Assert.That(actual.GameDescription.Tournament.TournamentId, Is.EqualTo(expected.GameDescription.Tournament.TournamentId), "GameDescription.Tournament.TournamentId must be equal");
                    Assert.That(actual.GameDescription.Tournament.TournamentInGameId, Is.EqualTo(expected.GameDescription.Tournament.TournamentInGameId), "GameDescription.Tournament.TournamentInGameId must be equal");
                    Assert.That(actual.GameDescription.Tournament.TournamentName, Is.EqualTo(expected.GameDescription.Tournament.TournamentName), "GameDescription.Tournament.TournamentName must be equal");
                    Assert.That(actual.GameDescription.Tournament.BuyIn, Is.EqualTo(expected.GameDescription.Tournament.BuyIn), "GameDescription.Tournament.BuyIn must be equal");
                    Assert.That(actual.GameDescription.Tournament.Bounty, Is.EqualTo(expected.GameDescription.Tournament.Bounty), "GameDescription.Tournament.Bounty must be equal");
                    Assert.That(actual.GameDescription.Tournament.Rebuy, Is.EqualTo(expected.GameDescription.Tournament.Rebuy), "GameDescription.Tournament.Rebuy must be equal");
                    Assert.That(actual.GameDescription.Tournament.Addon, Is.EqualTo(expected.GameDescription.Tournament.Addon), "GameDescription.Tournament.Addon must be equal");
                    Assert.That(actual.GameDescription.Tournament.Winning, Is.EqualTo(expected.GameDescription.Tournament.Winning), "GameDescription.Tournament.Winning must be equal");
                    Assert.That(actual.GameDescription.Tournament.FinishPosition, Is.EqualTo(expected.GameDescription.Tournament.FinishPosition), "GameDescription.Tournament.FinishPosition must be equal");
                    Assert.That(actual.GameDescription.Tournament.TotalPlayers, Is.EqualTo(expected.GameDescription.Tournament.TotalPlayers), "GameDescription.Tournament.TotalPlayers must be equal");
                    Assert.That(actual.GameDescription.Tournament.StartDate, Is.EqualTo(expected.GameDescription.Tournament.StartDate), "GameDescription.Tournament.StartDate must be equal");
                    Assert.That(actual.GameDescription.Tournament.Speed, Is.EqualTo(expected.GameDescription.Tournament.Speed), "GameDescription.Tournament.Speed must be equal");
                }

                Assert.That(actual.Players.Count, Is.EqualTo(expected.Players.Count), "Players.Count must be equal");

                actual.Players.SortList();
                expected.Players.SortList();

                for (var i = 0; i < actual.Players.Count; i++)
                {
                    Assert.That(actual.Players[i].PlayerName, Is.EqualTo(expected.Players[i].PlayerName), $"Player.PlayerName must be equal [{expected.Players[i].PlayerName}]");
                    Assert.That(actual.Players[i].PlayerNick, Is.EqualTo(expected.Players[i].PlayerNick), $"Player.PlayerNick must be equal [{expected.Players[i].PlayerNick}]");
                    Assert.That(actual.Players[i].Bet, Is.EqualTo(expected.Players[i].Bet), $"Player.Bet must be equal [{expected.Players[i].PlayerName}]");
                    Assert.That(actual.Players[i].Cards, Is.EqualTo(expected.Players[i].Cards), $"Player.Cards must be equal [{expected.Players[i].PlayerName}]");
                    Assert.That(actual.Players[i].SeatNumber, Is.EqualTo(expected.Players[i].SeatNumber), $"Player.SeatNumber must be equal [{expected.Players[i].PlayerName}]");
                    Assert.That(actual.Players[i].StartingStack, Is.EqualTo(expected.Players[i].StartingStack), $"Player.StartingStack must be equal [{expected.Players[i].PlayerName}]");
                    Assert.That(actual.Players[i].Win, Is.EqualTo(expected.Players[i].Win), $"Player.Win must be equal []");
                }

                Assert.That(actual.HandActions, Is.EqualTo(expected.HandActions), "HandActions.Count must be equal");

                var actualActions = actual.HandActions.OrderBy(x => x.ActionNumber).ToArray();
                var expectedActions = expected.HandActions.OrderBy(x => x.ActionNumber).ToArray();

                for (var i = 0; i < actualActions.Length; i++)
                {
                    Assert.That(actualActions[i].ActionNumber, Is.EqualTo(expectedActions[i].ActionNumber), "HandActions.ActionNumber must be equal");
                    Assert.That(actualActions[i].Amount, Is.EqualTo(expectedActions[i].Amount), "HandActions.Amount must be equal");
                    Assert.That(actualActions[i].HandActionType, Is.EqualTo(expectedActions[i].HandActionType), "HandActions.HandActionType must be equal");
                    Assert.That(actualActions[i].PlayerName, Is.EqualTo(expectedActions[i].PlayerName), "HandActions.PlayerName must be equal");
                    Assert.That(actualActions[i].Street, Is.EqualTo(expectedActions[i].Street), "HandActions.Street must be equal");
                }
            });
        }

        private JsonTestData GetJsonFileFromFolder(string folderName, bool isStringEnum)
        {
            var folder = Path.Combine(TestDataFolder, folderName);

            Assert.IsTrue(Directory.Exists(folder), $"Directory {folder} not found.");

            var jsonTestData = new JsonTestData();

            foreach (var file in Directory.GetFiles(folder).OrderBy(x => x))
            {
                if (file.IndexOf(ExpectedResultFile, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    try
                    {
                        var handHistoryText = File.ReadAllText(file);
                        jsonTestData.ExpectedResult = SerializationHelper.DeserializeObject<HandHistory>(handHistoryText);
                    }
                    catch (Exception e)
                    {
                        Assert.Fail($"{ExpectedResultFile} in {folder} has not been deserialized: {e}");
                    }

                    continue;
                }

                if (!Path.GetExtension(file).Equals(JsonExt))
                {
                    continue;
                }

                try
                {
                    var gameRoomStateChangeText = File.ReadAllText(file);
                    var gameRoomStateChange = isStringEnum ?
                        JsonConvert.DeserializeObject<SCGameRoomStateChange>(gameRoomStateChangeText, new StringEnumConverter()) :
                        JsonConvert.DeserializeObject<SCGameRoomStateChange>(gameRoomStateChangeText);

                    jsonTestData.GameRoomStateChanges.Add(gameRoomStateChange);
                }
                catch (Exception e)
                {
                    Assert.Fail($"{file} in {folder} has not been deserialized into {nameof(SCGameRoomStateChange)}: {e}");
                }
            }

            Assert.That(jsonTestData.GameRoomStateChanges.Count, Is.GreaterThan(0), $"Directory {folder} must contain at least 1 file with commands.");
            Assert.IsNotNull(jsonTestData.ExpectedResult, $"Directory {folder} must contain proper {ExpectedResultFile}.");

            return jsonTestData;
        }

        private class JsonTestData
        {
            public List<SCGameRoomStateChange> GameRoomStateChanges { get; set; } = new List<SCGameRoomStateChange>();

            public HandHistory ExpectedResult { get; set; }
        }
    }
}
