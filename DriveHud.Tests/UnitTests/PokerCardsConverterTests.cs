//-----------------------------------------------------------------------
// <copyright file="PokerCardsConverterTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Importers.Builders.iPoker;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class PokerCardsConverterTests
    {
        [Test]
        [TestCase("", "")]
        [TestCase("X X", "X X")]
        [TestCase("Error", "Error")]
        [TestCase("2d Jh", "D2 HJ")]
        [TestCase("Tc 7c", "C10 C7")]
        [TestCase("10s Qs 9s", "S10 SQ S9")]
        public void ConvertTest(string cards, string expected)
        {
            var converter = new PokerCardsConverter();
            var result = converter.Convert(cards);
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
