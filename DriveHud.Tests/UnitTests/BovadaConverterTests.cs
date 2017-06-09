//-----------------------------------------------------------------------
// <copyright file="BovadaConverterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Bovada;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class BovadaConverterTests
    {
        [Test]
        [TestCase("You Placed 1st in the tournament.<br/>Prize<br/>Cash: $39.01<br/>", 3901)]
        [TestCase("You Placed 2nd in the tournament.<br/>Prize<br/>Cash: $21.20<br/>", 2120)]
        [TestCase("You Placed 6th in the tournament.<br/>", 0)]
        [TestCase("", 0)]
        [TestCase("You placed 1st in the tournament.<br/> Prize <br/> Tournament Ticket: ID:6099746Name: Any $50 + $5", 5500)]
        [TestCase("You placed 1st in the tournament.<br/> Prize <br/> Tournament Ticket: ID:6099746Name: Any $50", 5000)]
        [TestCase("You placed 2nd in the tournament.<br/>Prize<br/>Play Money: 1575.00<br/>", 157500)]
        public void TestConvertPrizeTextToDecimal(string prizeText, int expected)
        {
            var actual = (int)(BovadaConverters.ConvertPrizeTextToDecimal(prizeText) * 100);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}