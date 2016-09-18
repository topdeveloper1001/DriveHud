using Model.HandAnalyzers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class HandAnalyzerHelpersTest
    {
        [TestCase("AcQs", true)]
        [TestCase("KsKc", true)]
        [TestCase("KsQs", false)]
        [Test]
        public void TestPremiumHand(string cards, bool isPremium)
        {
            var result = HandAnalyzerHelpers.IsPremiumHand(cards);

            Assert.That(result, Is.EqualTo(isPremium));
        }
    }
}
