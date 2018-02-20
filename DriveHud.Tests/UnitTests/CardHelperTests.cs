//-----------------------------------------------------------------------
// <copyright file="CardHelperTests.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Extensions;
using NUnit.Framework;
using System;
using System.Linq;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class CardHelperTests
    {
        private const string splitter = " ";

        [Test]
        [TestCase("AA KK QQ JJ", "JJ+")]
        [TestCase("AA QQ", "AA QQ")]
        [TestCase("T5s T6s T7s T8s", "T5s-T8s")]
        public void TestGetHandsFormatted(string cardsToFormat, string expected)
        {
            var splittedCards = cardsToFormat.Split(new[] { splitter }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var resultList = CardHelper.GetHandsFormatted(splittedCards);

            var result = string.Join(splitter, resultList);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("JJ+", "JJ QQ KK AA")]
        [TestCase("AA", "AA")]
        [TestCase("T5s-T8s", "T5s T6s T7s T8s")]
        [TestCase("T2o+", "T2o T3o T4o T5o T6o T7o T8o T9o")]
        [TestCase("22-44", "22 33 44")]
        [TestCase("QQ+", "QQ KK AA")]
        [TestCase("42s+", "42s 43s")]
        [TestCase("72o-74o", "72o 73o 74o")]
        public void TestGetHandsUnFormatted(string cardsToFormat, string expected)
        {
            var resultList = CardHelper.GetHandsUnFormatted(cardsToFormat);

            var result = string.Join(splitter, resultList);

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}