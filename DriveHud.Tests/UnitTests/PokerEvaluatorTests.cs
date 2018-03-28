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
        [TestCaseSource("HoldemTestDataCase")]
        public void HoldemGetWinners(TableTestData testData)
        {
            var holdemEvaluator = new HoldemEvaluator();

            holdemEvaluator.SetCardsOnTable(testData.CardsOnTable);

            foreach (var playerCards in testData.PlayerCards)
            {
                holdemEvaluator.SetPlayerCards(playerCards.Key, playerCards.Value);
            }

            var winners = holdemEvaluator.GetWinners();

            CollectionAssert.AreEquivalent(testData.Winners, winners.Hi);
        }

        [TestCaseSource("HoldemTestDataCaseGroupCards")]
        public void HoldemGetWinnersGroupCards(TableTestDataGroupCards testData)
        {
            var holdemEvaluator = new HoldemEvaluator();

            holdemEvaluator.SetCardsOnTable(testData.CardsOnTable);

            foreach (var playerCards in testData.PlayerCards)
            {
                holdemEvaluator.SetPlayerCards(playerCards.Key, playerCards.Value);
            }

            var winners = holdemEvaluator.GetWinners();

            CollectionAssert.AreEquivalent(testData.Winners, winners.Hi);
        }

        [TestCaseSource("OmahaHiLoTestDataCaseGroupCards")]
        public void OmahaHiLoGetWinnersGroupCards(TableTestDataHiLowGroupCards testData)
        {
            var holdemEvaluator = new OmahaHiLoEvaluator();

            holdemEvaluator.SetCardsOnTable(testData.CardsOnTable);

            foreach (var playerCards in testData.PlayerCards)
            {
                holdemEvaluator.SetPlayerCards(playerCards.Key, playerCards.Value);
            }

            var winners = holdemEvaluator.GetWinners();

            CollectionAssert.AreEquivalent(testData.HiWinners, winners.Hi, "Hi winners doesn't match.");
            CollectionAssert.AreEquivalent(testData.LoWinners, winners.Lo, "Lo winners doesn't match.");
        }

        private static IEnumerable<TestCaseData> HoldemTestDataCaseGroupCards()
        {
            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("Tc6d8cTs5c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 5, HoleCards.FromCards("Js9h") },
                     { 6, HoleCards.FromCards("Ah4d") },
                     { 8, HoleCards.FromCards("2d2s") }
                 },
                Winners = new List<int> { 8 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 1");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("4s8c4dQs5c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("9c4h") },
                     { 6, HoleCards.FromCards("Qd6c") },
                     { 8, HoleCards.FromCards("8d8s") }
                 },
                Winners = new List<int> { 8 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 2");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("9d2h2d2s2c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 2, HoleCards.FromCards("JsJc") },
                     { 4, HoleCards.FromCards("8dQd") }
                 },
                Winners = new List<int> { 4 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 3");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("2h2d2s7d9c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 2, HoleCards.FromCards("JsJc") },
                     { 4, HoleCards.FromCards("QdQc") }
                 },
                Winners = new List<int> { 4 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 4");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("2h2dQh9d8h"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 2, HoleCards.FromCards("2s2c") },
                     { 4, HoleCards.FromCards("QdQc") }
                 },
                Winners = new List<int> { 2 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 5");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("Ah8c2d3sJs"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 2, HoleCards.FromCards("7cAc") },
                     { 4, HoleCards.FromCards("QdAd") }
                 },
                Winners = new List<int> { 4 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 6");

            yield return new TestCaseData(new TableTestDataGroupCards
            {
                CardsOnTable = BoardCards.FromCards("5s2c6cJsQs"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 3, HoleCards.FromCards("KsKd") },
                     { 4, HoleCards.FromCards("3d4d") }
                 },
                Winners = new List<int> { 4 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners - 7");
        }

        private static IEnumerable<TestCaseData> HoldemTestDataCase()
        {
            yield return new TestCaseData(new TableTestData
            {
                CardsOnTable = "C10 D6 C8 S10 C5",
                PlayerCards = new Dictionary<int, string>
                 {
                     { 5, "SJ H9" },
                     { 6, "HA D4" },
                     { 8, "D2 S2" }
                 },
                Winners = new List<int> { 8 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners (IPoker builder) - 1");

            yield return new TestCaseData(new TableTestData
            {
                CardsOnTable = "S4 C8 D4 SQ C5",
                PlayerCards = new Dictionary<int, string>
                 {
                     { 1, "C9 H4" },
                     { 6, "DQ C6" },
                     { 8, "D8 S8" }
                 },
                Winners = new List<int> { 8 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners (IPoker builder) - 2");

            yield return new TestCaseData(new TableTestData
            {
                CardsOnTable = "D4 D2 DK S4 DJ",
                PlayerCards = new Dictionary<int, string>
                 {
                     { 3, "HK H10" },
                     { 6, "D7 C7" }
                 },
                Winners = new List<int> { 6 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners (IPoker builder) - 3");

            yield return new TestCaseData(new TableTestData
            {
                CardsOnTable = "H9 S6 HK DQ H3",
                PlayerCards = new Dictionary<int, string>
                 {
                     { 3, "H10 C7" },
                     { 5, "D5 D10" },
                     { 6, "H5 DA" }
                 },
                Winners = new List<int> { 6 }
            }).SetName($"{nameof(HoldemEvaluator)}: GetWinners (IPoker builder) - 4");
        }

        private static IEnumerable<TestCaseData> OmahaHiLoTestDataCaseGroupCards()
        {
            yield return new TestCaseData(new TableTestDataHiLowGroupCards
            {
                CardsOnTable = BoardCards.FromCards("Ah8s8c2d9c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("4d6d3sAs") },
                     { 2, HoleCards.FromCards("Ad3h5cQc") },
                     { 3, HoleCards.FromCards("5hTd4cAc") }
                 },
                HiWinners = new List<int> { 2 },
                LoWinners = new List<int> { 1 },
            }).SetName($"{nameof(OmahaHiLoEvaluator)}: GetWinners - 1");

            yield return new TestCaseData(new TableTestDataHiLowGroupCards
            {
                CardsOnTable = BoardCards.FromCards("Ad3dJc4sTh"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("3c8s4cQh") },
                     { 2, HoleCards.FromCards("Jh2s7c8c") },
                     { 3, HoleCards.FromCards("Kc3s2hJd") }
                 },
                HiWinners = new List<int> { 3 },
                LoWinners = new List<int> { 2 }
            }).SetName($"{nameof(OmahaHiLoEvaluator)}: GetWinners - 2");

            yield return new TestCaseData(new TableTestDataHiLowGroupCards
            {
                CardsOnTable = BoardCards.FromCards("2cJc8h9h4c"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("5s8s3c6h") },
                     { 2, HoleCards.FromCards("4sQc9d2d") },
                     { 3, HoleCards.FromCards("Qh2h7h3h") }
                 },
                HiWinners = new List<int> { 2 },
                LoWinners = new List<int> { 1 }
            }).SetName($"{nameof(OmahaHiLoEvaluator)}: GetWinners - 3");

            yield return new TestCaseData(new TableTestDataHiLowGroupCards
            {
                CardsOnTable = BoardCards.FromCards("5h6h8dTh2h"),
                PlayerCards = new Dictionary<int, HoleCards>
                 {
                     { 1, HoleCards.FromCards("2dAhJdAs") },
                     { 2, HoleCards.FromCards("4c3sQs8s") },
                 },
                HiWinners = new List<int> { 2 },
                LoWinners = new List<int> { 2 }
            }).SetName($"{nameof(OmahaHiLoEvaluator)}: GetWinners - 4");
        }

        internal class TableTestData
        {
            public string CardsOnTable { get; set; }

            public Dictionary<int, string> PlayerCards { get; set; }

            public IEnumerable<int> Winners { get; set; }
        }

        internal class TableTestDataBaseGroupCards
        {
            public BoardCards CardsOnTable { get; set; }

            public Dictionary<int, HoleCards> PlayerCards { get; set; }
        }

        internal class TableTestDataGroupCards : TableTestDataBaseGroupCards
        {
            public IEnumerable<int> Winners { get; set; }
        }

        internal class TableTestDataHiLowGroupCards : TableTestDataBaseGroupCards
        {
            public IEnumerable<int> HiWinners { get; set; }

            public IEnumerable<int> LoWinners { get; set; }
        }
    }
}