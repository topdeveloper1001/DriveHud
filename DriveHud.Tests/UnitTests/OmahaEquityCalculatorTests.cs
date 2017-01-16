using DriveHUD.EquityCalculator.Base.OmahaCalculations;
using HandHistories.Objects.Cards;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    class OmahaEquityCalculatorTests
    {
        [Test]
        [TestCase("8c4d3d", false, new float[] { })]
        [TestCase("8c4d3d", true, new float[] { })]
        public void OmahaEquityCalcTest(string boardCards, bool isHiLo, float[] equity)
        {
            BoardCards board = BoardCards.FromCards(boardCards);
            var cards = GetHoleCards().Select(x => CardGroup.Parse(x).Select(c => c.ToString()).ToArray()).ToArray();
            OmahaEquityCalculatorMain calc = new OmahaEquityCalculatorMain(true, isHiLo);

            var result = calc.Equity(board.Select(x => x.ToString()).ToArray(), cards, new string[] { }, 0);

            var i = 0;
        }

        private string[] GetHoleCards()
        {
            return new string[]
            {
                "KsAd6s6c",
                "Kh5c5dQs",
                "JdThTd7d",
            };

        }

    }
}
