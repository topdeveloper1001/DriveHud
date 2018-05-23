//-----------------------------------------------------------------------
// <copyright file="HandAnalyzerTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.EquityCalculator.Analyzer;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class HandAnalyzerTests
    {
        [TestCase('2', 'h', 73730)]
        [TestCase('3', 'h', 139523)]
        [TestCase('A', 'h', 268446761)]
        public void FastCardTest(char rank, char suit, int expectedValue)
        {
            var actualValue = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(rank, suit);
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        [TestCase("Ah2h3h7h6h", 807)]
        [TestCase("Ah2h3h6h7h", 807)]
        [TestCase("Ah2h4h6h7h", 806)]
        [TestCase("Ah2h4h6h8h", 796)]
        [TestCase("AhKhQhJhTh", 1)]
        [TestCase("AhAsAcAdKs", 11)]
        [TestCase("AhAsAcAdQs", 12)]
        public void Eval5Hand(string cards, int expected)
        {
            var card1 = cards.Substring(0, 2);
            var card2 = cards.Substring(2, 2);
            var card3 = cards.Substring(4, 2);
            var card4 = cards.Substring(6, 2);
            var card5 = cards.Substring(8, 2);

            var fCard1 = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(card1[0], card1[1]);
            var fCard2 = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(card2[0], card2[1]);
            var fCard3 = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(card3[0], card3[1]);
            var fCard4 = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(card4[0], card4[1]);
            var fCard5 = DriveHUD.EquityCalculator.Analyzer.HandHistory.fastCard(card5[0], card5[1]);      

            var actual = FastEvaluator.eval_5hand(new[] { fCard1, fCard2, fCard3, fCard4, fCard5 });

            Assert.That(actual, Is.EqualTo(expected));
        }     
    }
}