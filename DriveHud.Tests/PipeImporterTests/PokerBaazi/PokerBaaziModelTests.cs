//-----------------------------------------------------------------------
// <copyright file="PokerBaaziModelTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerBaazi.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziModelTests
    {
        [OneTimeSetUp]
        public virtual void SetUp()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
        }

        [TestCaseSource("InitReponseTestData")]
        public void InitResponseDeserializationTest(string file, PokerBaaziInitResponse expectedResponse)
        {
            var initResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziInitResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(initResponse.ClassObj.MaxPlayers, Is.EqualTo(expectedResponse.MaxPlayers));
                Assert.That(initResponse.ClassObj.TournamentName, Is.EqualTo(expectedResponse.TournamentName));
                Assert.That(initResponse.ClassObj.RoomId, Is.EqualTo(expectedResponse.RoomId));
                Assert.That(initResponse.ClassObj.SmallBlind, Is.EqualTo(expectedResponse.SmallBlind));
                Assert.That(initResponse.ClassObj.BigBlind, Is.EqualTo(expectedResponse.BigBlind));
                Assert.That(initResponse.ClassObj.UserId, Is.EqualTo(expectedResponse.UserId));
                Assert.That(initResponse.ClassObj.TournamentId, Is.EqualTo(expectedResponse.TournamentId));
                Assert.That(initResponse.ClassObj.Straddle, Is.EqualTo(expectedResponse.Straddle));
                Assert.That(initResponse.ClassObj.GameType, Is.EqualTo(expectedResponse.GameType));
                Assert.That(initResponse.ClassObj.GameLimit, Is.EqualTo(expectedResponse.GameLimit));
                Assert.That(initResponse.ClassObj.TournamentTableName, Is.EqualTo(expectedResponse.TournamentTableName));
            });
        }

        [TestCaseSource("SpectatorResponseTestData")]
        public void SpectatorResponseDeserializationTest(string file, PokerBaaziStartGameResponse expectedResponse)
        {
            var spectatorResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziStartGameResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(spectatorResponse.ClassObj.RoomId, Is.EqualTo(expectedResponse.RoomId));
                Assert.That(spectatorResponse.ClassObj.HandId, Is.EqualTo(expectedResponse.HandId));
                Assert.That(spectatorResponse.ClassObj.SmallBlind, Is.EqualTo(expectedResponse.SmallBlind));
                Assert.That(spectatorResponse.ClassObj.BigBlind, Is.EqualTo(expectedResponse.BigBlind));
                Assert.That(spectatorResponse.ClassObj.TotalAnte, Is.EqualTo(expectedResponse.TotalAnte));
                Assert.That(spectatorResponse.ClassObj.Ante, Is.EqualTo(expectedResponse.Ante));
                Assert.That(spectatorResponse.ClassObj.TournamentId, Is.EqualTo(expectedResponse.TournamentId));

                Assert.That(spectatorResponse.ClassObj.Players.Count, Is.EqualTo(expectedResponse.Players.Count));

                foreach (var player in spectatorResponse.ClassObj.Players)
                {
                    Assert.IsTrue(expectedResponse.Players.TryGetValue(player.Key, out PokerBaaziPlayerInfo expectedPlayer));

                    Assert.That(player.Value.Cards, Is.EqualTo(expectedPlayer.Cards), "Cards must be equal");
                    Assert.That(player.Value.Chips, Is.EqualTo(expectedPlayer.Chips), "Chips must be equal");
                    Assert.That(player.Value.IsBigBlind, Is.EqualTo(expectedPlayer.IsBigBlind), "IsBigBlind must be equal");
                    Assert.That(player.Value.IsDealer, Is.EqualTo(expectedPlayer.IsDealer), "IsDealer must be equal");
                    Assert.That(player.Value.IsSmallBlind, Is.EqualTo(expectedPlayer.IsSmallBlind), "IsSmallBlind must be equal");
                    Assert.That(player.Value.PlayerId, Is.EqualTo(expectedPlayer.PlayerId), "PlayerId must be equal");
                    Assert.That(player.Value.PlayerName, Is.EqualTo(expectedPlayer.PlayerName), "PlayerName must be equal");
                    Assert.That(player.Value.Seat, Is.EqualTo(expectedPlayer.Seat), "Seat must be equal");
                    Assert.That(player.Value.BetAmount, Is.EqualTo(expectedPlayer.BetAmount), "BetAmount must be equal");
                }
            });
        }

        [TestCaseSource("UserButtonActionResponseTestData")]
        public void UserButtonActionResponseDeserializationTest(string file, PokerBaaziUserActionResponse expectedResponse)
        {
            var userActionResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziUserActionResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(userActionResponse.ClassObj.RoomId, Is.EqualTo(expectedResponse.RoomId));
                Assert.That(userActionResponse.ClassObj.HandId, Is.EqualTo(expectedResponse.HandId));
                Assert.That(userActionResponse.ClassObj.Action, Is.EqualTo(expectedResponse.Action));
                Assert.That(userActionResponse.ClassObj.Amount, Is.EqualTo(expectedResponse.Amount));
                Assert.That(userActionResponse.ClassObj.BetAmount, Is.EqualTo(expectedResponse.BetAmount));
                Assert.That(userActionResponse.ClassObj.PlayerId, Is.EqualTo(expectedResponse.PlayerId));
                Assert.That(userActionResponse.ClassObj.PlayerName, Is.EqualTo(expectedResponse.PlayerName));
                Assert.That(userActionResponse.ClassObj.PotAmount, Is.EqualTo(expectedResponse.PotAmount));
                Assert.That(userActionResponse.ClassObj.RemainingStack, Is.EqualTo(expectedResponse.RemainingStack));
            });
        }

        [TestCaseSource("RoundReponseTestData")]
        public void RoundResponseDeserializationTest(string file, PokerBaaziRoundResponse expectedResponse)
        {
            var roundResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziRoundResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(roundResponse.ClassObj.RoomId, Is.EqualTo(expectedResponse.RoomId));
                Assert.That(roundResponse.ClassObj.HandId, Is.EqualTo(expectedResponse.HandId));
                Assert.That(roundResponse.ClassObj.CommunitryCards, Is.EqualTo(expectedResponse.CommunitryCards));
            });
        }

        [TestCaseSource("WinnerResponseTestData")]
        public void WinnerResponseDeserializationTest(string file, PokerBaaziWinnerResponse expectedResponse)
        {
            var winnerResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziWinnerResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(winnerResponse.ClassObj.RoomId, Is.EqualTo(expectedResponse.RoomId));
                Assert.That(winnerResponse.ClassObj.HandId, Is.EqualTo(expectedResponse.HandId));

                Assert.That(winnerResponse.ClassObj.Winners.Count, Is.EqualTo(expectedResponse.Winners.Count));

                foreach (var player in winnerResponse.ClassObj.Winners)
                {
                    Assert.IsTrue(expectedResponse.Winners.TryGetValue(player.Key, out PokerBaaziWinnerInfo expectedPlayer));

                    Assert.That(player.Value.HoleCards, Is.EqualTo(expectedPlayer.HoleCards));
                    Assert.That(player.Value.PlayerId, Is.EqualTo(expectedPlayer.PlayerId));
                    Assert.That(player.Value.PlayerName, Is.EqualTo(expectedPlayer.PlayerName));
                    Assert.That(player.Value.Seat, Is.EqualTo(expectedPlayer.Seat));
                    Assert.That(player.Value.WinAmount, Is.EqualTo(expectedPlayer.WinAmount));
                }
            });
        }

        [TestCaseSource("TournamentDetailsResponseTestData")]
        public void TournamentDetailsResponseDeserializationTest(string file, PokerBaaziTournamentDetailsResponse expectedResponse)
        {
            var tournamentDetailsResponse = ReadPackageFromFile<PokerBaaziResponse<PokerBaaziTournamentDetailsResponse>>(file);

            Assert.Multiple(() =>
            {
                Assert.That(tournamentDetailsResponse.ClassObj.TournamentId, Is.EqualTo(expectedResponse.TournamentId), "Tournament id must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.BuyIn, Is.EqualTo(expectedResponse.Details.BuyIn), $"BuyIn must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.EntryFee, Is.EqualTo(expectedResponse.Details.EntryFee), $"EntryFee must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.MaxEntries, Is.EqualTo(expectedResponse.Details.MaxEntries), $"MaxEntries must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.StartingStake, Is.EqualTo(expectedResponse.Details.StartingStake), $"StartingStake must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.TotalPlayers, Is.EqualTo(expectedResponse.Details.TotalPlayers), $"TotalPlayers must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.TournamentName, Is.EqualTo(expectedResponse.Details.TournamentName), $"TournamentName must be equal");
                Assert.That(tournamentDetailsResponse.ClassObj.Details.TournamentStartDate, Is.EqualTo(expectedResponse.Details.TournamentStartDate), $"TournamentStartDate must be equal");
            });
        }

        private T ReadPackageFromFile<T>(string file) where T : class
        {
            file = Path.Combine(PokerBaaziTestsHelper.TestDataFolder, file);

            FileAssert.Exists(file);

            var data = File.ReadAllText(file);

            Assert.IsNotEmpty(data);

            var response = JsonConvert.DeserializeObject<T>(data);

            return response;
        }

        private static IEnumerable<TestCaseData> InitReponseTestData()
        {
            yield return new TestCaseData(
                "Packets\\InitResponse.json",
                new PokerBaaziInitResponse
                {
                    TournamentId = 231044,
                    TournamentName = "Dazzling Deuces",
                    MaxPlayers = 9,
                    RoomId = 100529,
                    SmallBlind = 1,
                    BigBlind = 2,
                    UserId = 375337,
                    Straddle = false,
                    GameType = "Texas Hold'em",
                    GameLimit = "NO LIMIT"
                }
            );

            yield return new TestCaseData(
              "Packets\\InitResponse-2.json",
              new PokerBaaziInitResponse
              {
                  TournamentId = 292754,
                  TournamentName = "The Summit 15 LAC GTD (RE)",
                  TournamentTableName = "Table#1",
                  MaxPlayers = 9,
                  RoomId = 48710,
                  SmallBlind = 800,
                  BigBlind = 1600,
                  UserId = 375337,
                  Straddle = false,
                  GameType = "Texas Hold'em",
                  GameLimit = "NO LIMIT"
              }
          );
        }

        private static IEnumerable<TestCaseData> SpectatorResponseTestData()
        {
            yield return new TestCaseData(
                "Packets\\SpectatorResponse.json",
                new PokerBaaziStartGameResponse
                {
                    RoomId = 100529,
                    HandId = 1546956879558,
                    Players = new Dictionary<int, PokerBaaziPlayerInfo>
                    {
                        [0] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 43,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = true,
                            PlayerId = 454215,
                            PlayerName = "RarInG",
                            Seat = 7,
                            BetAmount = 1
                        },
                        [1] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 136,
                            IsBigBlind = true,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 272093,
                            PlayerName = "meherzad7",
                            Seat = 8,
                            BetAmount = 2
                        },
                        [2] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 98,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 255740,
                            PlayerName = "aadi8851777105",
                            Seat = 0,
                            BetAmount = 2
                        },
                        [3] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 54,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 501618,
                            PlayerName = "scarry",
                            Seat = 1
                        },
                        [4] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 72,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 342356,
                            PlayerName = "Neizo",
                            Seat = 2
                        },
                        [5] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 36,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 234609,
                            PlayerName = "Vintos",
                            Seat = 3
                        },
                        [6] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 111,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 456618,
                            PlayerName = "Janmejayxxx",
                            Seat = 4
                        },
                        [7] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 141,
                            IsBigBlind = false,
                            IsDealer = false,
                            IsSmallBlind = false,
                            PlayerId = 474929,
                            PlayerName = "aditya333",
                            Seat = 5
                        },
                        [8] = new PokerBaaziPlayerInfo
                        {
                            Cards = "card_back,card_back",
                            Chips = 89,
                            IsBigBlind = false,
                            IsDealer = true,
                            IsSmallBlind = false,
                            PlayerId = 487994,
                            PlayerName = "atul198904",
                            Seat = 6
                        }
                    }
                }
            );

            yield return new TestCaseData(
              "Packets\\StartGameResponse.json",
              new PokerBaaziStartGameResponse
              {
                  RoomId = 100399,
                  HandId = 1547748908472,
                  Players = new Dictionary<int, PokerBaaziPlayerInfo>
                  {
                      [0] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 86,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 494515,
                          PlayerName = "lily",
                          Seat = 3
                      },
                      [1] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 281,
                          IsBigBlind = true,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 501249,
                          PlayerName = "niketshah04",
                          Seat = 0,
                          BetAmount = 2
                      },
                      [2] = new PokerBaaziPlayerInfo
                      {
                          Cards = "heart_2,club_6",
                          Chips = 98,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 375337,
                          PlayerName = "masterpiece",
                          Seat = 6,
                          BetAmount = 2
                      },
                      [3] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 36,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 576799,
                          PlayerName = "Vijendra14",
                          Seat = 5
                      },
                      [4] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 190,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 520717,
                          PlayerName = "preharman1",
                          Seat = 4
                      },
                      [5] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 17,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = true,
                          PlayerId = 548313,
                          PlayerName = "Mangesh1986",
                          Seat = 8,
                          BetAmount = 1
                      },
                      [6] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 218,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 185893,
                          PlayerName = "patelprajay",
                          Seat = 1
                      },
                      [7] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 98,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 23457,
                          PlayerName = "nish4x",
                          Seat = 2
                      },
                      [8] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 76,
                          IsBigBlind = false,
                          IsDealer = true,
                          IsSmallBlind = false,
                          PlayerId = 531648,
                          PlayerName = "abhishek1289",
                          Seat = 7
                      }
                  }
              }
          );

            yield return new TestCaseData(
              "Packets\\SpectatorResponse-2.json",
              new PokerBaaziStartGameResponse
              {
                  RoomId = 48710,
                  HandId = 1547746531954,
                  TotalAnte = 1800,
                  Ante = 200,
                  SmallBlind = 800,
                  BigBlind = 1600,
                  TournamentId = 292754,
                  Players = new Dictionary<int, PokerBaaziPlayerInfo>
                  {
                      [0] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 47200,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = true,
                          PlayerId = 239059,
                          PlayerName = "Ellis_Dee",
                          Seat = 2,
                          BetAmount = 800
                      },
                      [1] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 61537,
                          IsBigBlind = true,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 419286,
                          PlayerName = "Jeet0",
                          Seat = 3,
                          BetAmount = 1600
                      },
                      [2] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 42802,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 37902,
                          PlayerName = "karan1515",
                          Seat = 4
                      },
                      [3] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 35139,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 35200,
                          PlayerName = "LungFakeer",
                          Seat = 5
                      },
                      [4] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 62280,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 171158,
                          PlayerName = "saurabhiim",
                          Seat = 6
                      },
                      [5] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 69208,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 100479,
                          PlayerName = "ImHighIshove",
                          Seat = 7
                      },
                      [6] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 67057,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 521327,
                          PlayerName = "Wynn",
                          Seat = 8
                      },
                      [7] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 55800,
                          IsBigBlind = false,
                          IsDealer = false,
                          IsSmallBlind = false,
                          PlayerId = 35024,
                          PlayerName = "SpadeHunter",
                          Seat = 0
                      },
                      [8] = new PokerBaaziPlayerInfo
                      {
                          Cards = "card_back,card_back",
                          Chips = 24800,
                          IsBigBlind = false,
                          IsDealer = true,
                          IsSmallBlind = false,
                          PlayerId = 39094,
                          PlayerName = "gangajal",
                          Seat = 1
                      }
                  }
              }
          );
        }

        private static IEnumerable<TestCaseData> UserButtonActionResponseTestData()
        {
            yield return new TestCaseData(
               "Packets\\UserButtonActionResponse.json",
               new PokerBaaziUserActionResponse
               {
                   Action = "Check",
                   Amount = 0,
                   BetAmount = 0,
                   HandId = 1546956879558,
                   PlayerId = 255740,
                   PlayerName = "aadi8851777105",
                   PotAmount = 0,
                   RemainingStack = 0,
                   RoomId = 100529
               }
            );

            yield return new TestCaseData(
              "Packets\\UserButtonActionResponse-2.json",
              new PokerBaaziUserActionResponse
              {
                  Action = "Raise",
                  Amount = 6,
                  BetAmount = 6,
                  HandId = 1546956924578,
                  PlayerId = 342356,
                  PlayerName = "Neizo",
                  PotAmount = 9,
                  RemainingStack = 71,
                  RoomId = 100529
              }
           );
        }

        private static IEnumerable<TestCaseData> RoundReponseTestData()
        {
            yield return new TestCaseData(
                "Packets\\RoundResponse.json",
                new PokerBaaziRoundResponse
                {
                    HandId = 1546956924578,
                    RoomId = 100529,
                    CommunitryCards = "diamond_4,club_5,spade_2"
                }
            );
        }

        private static IEnumerable<TestCaseData> WinnerResponseTestData()
        {
            yield return new TestCaseData(
               "Packets\\WinnerResponse.json",
               new PokerBaaziWinnerResponse
               {
                   HandId = 1546956924578,
                   RoomId = 100529,
                   PotAmount = 34,
                   Winners = new Dictionary<int, PokerBaaziWinnerInfo>
                   {
                       [0] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 272093,
                           PlayerName = "meherzad7",
                           HoleCards = "card_back,card_back",
                           Seat = 8,
                           WinAmount = 32
                       },
                       [1] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 255740,
                           PlayerName = "aadi8851777105",
                           HoleCards = "card_back,card_back",
                           Seat = 0,
                           WinAmount = 0
                       },
                       [2] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 342356,
                           PlayerName = "Neizo",
                           HoleCards = "card_back,card_back",
                           Seat = 2,
                           WinAmount = 0
                       },
                       [3] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 456618,
                           PlayerName = "Janmejayxxx",
                           HoleCards = "card_back,card_back",
                           Seat = 4,
                           WinAmount = 0
                       },
                       [4] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 474929,
                           PlayerName = "aditya333",
                           HoleCards = "card_back,card_back",
                           Seat = 5,
                           WinAmount = 0
                       },
                       [5] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 487994,
                           PlayerName = "atul198904",
                           HoleCards = "card_back,card_back",
                           Seat = 6,
                           WinAmount = 0
                       },
                       [6] = new PokerBaaziWinnerInfo
                       {
                           PlayerId = 454215,
                           PlayerName = "RarInG",
                           HoleCards = "card_back,card_back",
                           Seat = 7,
                           WinAmount = 0
                       }
                   }
               }
           );
        }

        private static IEnumerable<TestCaseData> TournamentDetailsResponseTestData()
        {
            yield return new TestCaseData(
              "Packets\\TournamentDetailsResponse.json",
              new PokerBaaziTournamentDetailsResponse
              {
                  TournamentId = 292754,
                  Details = new PokerBaaziTournamentDetails
                  {
                      BuyIn = 7500,
                      EntryFee = 750,
                      MaxEntries = 2500,
                      StartingStake = 25000,
                      TotalPlayers = 119,
                      TournamentName = "The Summit 15 LAC GTD (RE)",
                      TournamentStartDate = new DateTime(2019, 01, 17, 17, 0, 0, DateTimeKind.Utc)
                  }
              }
            );
        }
    }
}