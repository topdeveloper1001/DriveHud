//-----------------------------------------------------------------------
// <copyright file="UtilsTests.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Utils;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class UtilsTests
    {
        [Test]
        [TestCase("", false)]
        [TestCase("1", false)]
        [TestCase("22@22.232", true)]
        [TestCase("peon84@yandex.ru", true)]
        [TestCase("al.v.dan@gmail.com", true)]

        public void TestIsValidEmail(string email, bool expected)
        {
            var actual = Utils.IsValidEmail(email);
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
