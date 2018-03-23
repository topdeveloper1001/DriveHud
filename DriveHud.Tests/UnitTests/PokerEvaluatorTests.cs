//-----------------------------------------------------------------------
// <copyright file="PokerEvaluatorTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Builders.iPoker;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using System.Collections.Generic;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class PokerEvaluatorTests
    {
        [TestCaseSource("TestDataCase")]
        public void TestGetWinners(TableTestData testData)
        {
            var holdemEvaluator = new HoldemEvaluator();

            holdemEvaluator.SetCardsOnTable(testData.CardsOnTable);

            foreach (var playerCards in testData.PlayerCards)
            {
                holdemEvaluator.SetPlayerCards(playerCards.Key, playerCards.Value);
            }

            var winners = holdemEvaluator.GetWinners();

            CollectionAssert.AreEquivalent(testData.Winners, winners);
        }

        [TestCaseSource("TestDataCaseGroupCards")]
        public void TestGetWinnersGroupCards(TableTestDataGroupCards testData)
        {
            var holdemEvaluator = new HoldemEvaluator();

            holdemEvaluator.SetCardsOnTable(testData.CardsOnTable);

            foreach (var playerCards in testData.PlayerCards)
            {
                holdemEvaluator.SetPlayerCards(playerCards.Key, playerCards.Value);
            }

            var winners = holdemEvaluator.GetWinners();

            CollectionAssert.AreEquivalent(testData.Winners, winners);
        }

        private static List<TableTestDataGroupCards> TestDataCaseGroupCards = new List<TableTestDataGroupCards>
        {
            new TableTestDataGroupCards
            {
                 CardsOnTable = BoardCards.FromCards("Tc6d8cTs5c"),
                 PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 5, HoleCards.FromCards("Js9h") },
                     { 6, HoleCards.FromCards("Ah4d") },
                     { 8, HoleCards.FromCards("2d2s") }
                 },
                 Winners = new List<int> { 8 }
            },
            new TableTestDataGroupCards
            {
                 CardsOnTable =  BoardCards.FromCards("4s8c4dQs5c"),
                 PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("9c4h") },
                     { 6, HoleCards.FromCards("Qd6c") },
                     { 8, HoleCards.FromCards("8d8s") }
                 },
                 Winners = new List<int> { 8 }
            }
        };

        private static List<TableTestData> TestDataCase = new List<TableTestData>
        {
            new TableTestData
            {
                 CardsOnTable = "C10 D6 C8 S10 C5",
                 PlayerCards = new Dictionary<int, string>
                 {
                     { 5, "SJ H9" },
                     { 6, "HA D4" },
                     { 8, "D2 S2" }
                 },
                 Winners = new List<int> { 8 }
            },
            new TableTestData
            {
                 CardsOnTable = "S4 C8 D4 SQ C5",
                 PlayerCards = new Dictionary<int, string>
                 {
                     { 1, "C9 H4" },
                     { 6, "DQ C6" },
                     { 8, "D8 S8" }
                 },
                 Winners = new List<int> { 8 }
            },
            new TableTestData
            {
                 CardsOnTable = "D4 D2 DK S4 DJ",
                 PlayerCards = new Dictionary<int, string>
                 {
                     { 3, "HK H10" },
                     { 6, "D7 C7" }
                 },
                 Winners = new List<int> { 6 }
            },
            new TableTestData
            {
                 CardsOnTable = "H9 S6 HK DQ H3",
                 PlayerCards = new Dictionary<int, string>
                 {
                     { 3, "H10 C7" },
                     { 5, "D5 D10" },
                     { 6, "H5 DA" }
                 },
                 Winners = new List<int> { 6 }
            }
        };

        internal class TableTestData
        {
            public string CardsOnTable { get; set; }

            public Dictionary<int, string> PlayerCards { get; set; }

            public IEnumerable<int> Winners { get; set; }
        }

        internal class TableTestDataGroupCards
        {
            public BoardCards CardsOnTable { get; set; }

            public Dictionary<int, HoleCards> PlayerCards { get; set; }

            public IEnumerable<int> Winners { get; set; }
        }
    }
}