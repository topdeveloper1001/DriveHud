using DriveHUD.Importers.Builders.iPoker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
