//-----------------------------------------------------------------------
// <copyright file="ICardsComparer.cs" company="Ace Poker Solutions">
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
using DriveHUD.Importers.Builders.iPoker;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class LoCardsComparerTests
    {
        [TestCase("6h5h4c3s2h", true)]
        [TestCase("8d6h5h2hAs", true)]
        [TestCase("9d6h5h2hAs", false)]
        [TestCase("8d6h6c2hAs", false)]
        [TestCase("8d5h6c2hAs", true)]
        public void IsValidTest(string cards, bool expected)
        {
            cards = ConvertCards(cards);

            var comparer = new LoCardsComparer();
            var actual = comparer.IsValid(cards);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("6h5h4c3s2h", "6d5d4h3h2d", 0)]
        [TestCase("6h5h4c3s2h", "8d6h5h2hAs", 1)]
        [TestCase("4c2h6h5h3s", "As8d6h5h2h", 1)]
        [TestCase("6h5h4c3s2h", "6d5d4s3cAs", -1)]
        [TestCase("7h5h4c3s2h", "6d5d4s3c2s", -1)]
        [TestCase("7h5h4c3s2h", "7d6d4s3c2s", 1)]
        [TestCase("As2h4c5s6d", "As2h3c5s7d", 1)]
        public void CompareTests(string cards1, string cards2, int expected)
        {
            cards1 = ConvertCards(cards1);
            cards2 = ConvertCards(cards2);

            var comparer = new LoCardsComparer();
            var actual = comparer.Compare(cards1, cards2);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private string ConvertCards(string cards)
        {
            var cardGroup = CardGroup.Parse(cards);
            return string.Join(" ", cardGroup.Select(card => card.ToString().ToUpper().Reverse().Replace("T", "10")));
        }
    }
}