//-----------------------------------------------------------------------
// <copyright file="PokerBaaziUtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.PokerBaazi;
using HandHistories.Objects.Cards;
using NUnit.Framework;

namespace DriveHud.Tests.PipeImporterTests.PokerBaazi
{
    [TestFixture]
    class PokerBaaziUtilsTests
    {
        [TestCase("", "", false)]
        [TestCase("spade_12,club_9,diamond_8,diamond_3", "Qs9c8d3d", true)]
        [TestCase("card_back,card_back", "", false)]
        public void TryParseCardsTest(string cardsString, string expectedCards, bool expectedResult)
        {
            var result = PokerBaaziUtils.TryParseCards(cardsString, out HandHistories.Objects.Cards.Card[] cards);

            Assert.That(result, Is.EqualTo(expectedResult));

            var boardCards = BoardCards.FromCards(cards);

            Assert.That(boardCards.ToString(), Is.EqualTo(expectedCards));
        }
    }
}