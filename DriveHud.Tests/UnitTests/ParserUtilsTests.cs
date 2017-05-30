//-----------------------------------------------------------------------
// <copyright file="ParserUtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using HandHistories.Parser.Utils.FastParsing;
using NUnit.Framework;
using HandHistories.Objects.Cards;
using System.Text;
using System;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class ParserUtilsTests
    {
        [Test]
        [TestCase("1000", 1000)]
        [TestCase("0.01", 0.01)]
        [TestCase("0.02$", 0.02)]
        [TestCase("0.02 $", 0.02)]
        [TestCase("$0.03", 0.03)]
        [TestCase("$ 0.03", 0.03)]
        [TestCase("$2,000", 2000)]
        [TestCase("2,000 $", 2000)]
        [TestCase("$2,000.35", 2000.35)]
        [TestCase("2,000.35 $", 2000.35)]
        [TestCase("2,000.37$", 2000.37)]
        [TestCase("2,000.45", 2000.45)]
        [TestCase("2000.75", 2000.75)]
        [TestCase("2000.75 $", 2000.75)]
        [TestCase("$ 2000.75", 2000.75)]
        [TestCase("6h.4d", 0)]
        public void TryParseMoneyTest(string moneyText, decimal expectedMoney)
        {
            decimal money;
            ParserUtils.TryParseMoney(moneyText, out money);
            Assert.That(money, Is.EqualTo(expectedMoney));
        }

        [Test]
        [TestCase("Ac9d", "A9o")]
        [TestCase("AcAd", "AA")]
        [TestCase("2h2s", "22")]
        [TestCase("AdKc", "AKo")]
        [TestCase("AdKd", "AKs")]
        [TestCase("Qh7h", "Q7s")]
        [TestCase("AdKdQdTc", "")]
        [TestCase("Ad", "")]
        [TestCase("9d9d", "")]
        [TestCase("RcMd", "")]
        public void TestConvertToCardRange(string cards, string expectedRange)
        {
            var actualRange = ParserUtils.ConvertToCardRange(cards);
            Assert.That(actualRange, Is.EqualTo(expectedRange));
        }      
    }
}