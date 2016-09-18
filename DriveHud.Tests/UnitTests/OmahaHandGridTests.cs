using System;
using System.Text;
using System.Collections.Generic;
using Model.OmahaHoleCardsAnalyzers;
using HandHistories.Objects.Cards;
using Model.Filters;
using NUnit.Framework;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class OmahaHandGridTests
    {
        [Test]
        public void TestOmahaNoPairAnalyzer()
        {
            var analyzer = new OmahaNoPairAnalyzer();
            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("4c8cKcAc");
            var cards2 = CardGroup.Parse("KcKd4c5c");
            var cards3 = CardGroup.Parse("5c5d9c9d");
            var cards4 = CardGroup.Parse("6d6c6h2c");
            var cards5 = CardGroup.Parse("6c6d6h6s");

            Assert.That(analyzer.Analyze(cards, item), Is.True);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
        }

        [Test]
        public void TestOmahaOnePairAnalyzer()
        {
            var analyzer = new OmahaOnePairAnalyzer();

            var item11 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "") };
            var item12 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "4") };
            var cards1 = CardGroup.Parse("4c8cKcAc");

            var item21 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "") };
            var item22 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "K") };
            var item23 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "4") };
            var cards2 = CardGroup.Parse("KcKd4c5c");

            var item31 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "") };
            var item32 = new OnePairHandGridItem() { SelectedRank = new Tuple<string, string>("", "5") };
            var cards3 = CardGroup.Parse("5c5d9c9d");
            var cards4 = CardGroup.Parse("5d5c5h2c");
            var cards5 = CardGroup.Parse("5c5d5h5s");

            Assert.That(analyzer.Analyze(cards1, item11), Is.False);
            Assert.That(analyzer.Analyze(cards1, item12), Is.False);
            Assert.That(analyzer.Analyze(cards2, item21), Is.True);
            Assert.That(analyzer.Analyze(cards2, item22), Is.True);
            Assert.That(analyzer.Analyze(cards2, item23), Is.False);
            Assert.That(analyzer.Analyze(cards3, item31), Is.False);
            Assert.That(analyzer.Analyze(cards3, item32), Is.False);
            Assert.That(analyzer.Analyze(cards4, item31), Is.False);
            Assert.That(analyzer.Analyze(cards4, item32), Is.False);
            Assert.That(analyzer.Analyze(cards5, item31), Is.False);
            Assert.That(analyzer.Analyze(cards5, item32), Is.False);
        }

        [Test]
        public void TestOmahaTwoPairsAnalyzer()
        {
            var analyzer = new OmahaTwoPairsAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("4c8cKcAc");
            var cards2 = CardGroup.Parse("KcKd4c5c");
            var cards3 = CardGroup.Parse("5c5d9c9d");
            var cards4 = CardGroup.Parse("6d6c6h2c");
            var cards5 = CardGroup.Parse("6c6d6h6s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.True);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
        }

        [Test]
        public void TestOmahaTripsAnalyzer()
        {
            var analyzer = new OmahaTripsAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("4c8cKcAc");
            var cards2 = CardGroup.Parse("KcKd4c5c");
            var cards3 = CardGroup.Parse("5c5d9c9d");
            var cards4 = CardGroup.Parse("6d6c6h2c");
            var cards5 = CardGroup.Parse("6c6d6h6s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.True);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
        }

        [Test]
        public void TestOmahaQuadsAnalyzer()
        {
            var analyzer = new OmahaQuadsAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("4c8cKcAc");
            var cards2 = CardGroup.Parse("KcKd4c5c");
            var cards3 = CardGroup.Parse("5c5d9c9d");
            var cards4 = CardGroup.Parse("6d6c6h2c");
            var cards5 = CardGroup.Parse("6c6d6h6s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.True);
        }

        [Test]
        public void TestOmahaRainbowAnalyzer()
        {
            var analyzer = new OmahaRainbowAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("6c6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4d5s");
            var cards3 = CardGroup.Parse("2c3c4d5c");
            var cards4 = CardGroup.Parse("2c3c4d5d");
            var cards5 = CardGroup.Parse("2c3c4c5d");
            var cards6 = CardGroup.Parse("2c3c4sAd");

            Assert.That(analyzer.Analyze(cards, item), Is.True);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
            Assert.That(analyzer.Analyze(cards6, item), Is.False);
        }

        [Test]
        public  void TestOmahaAceSuitedAnalyzer()
        {
            var analyzer = new OmahaAceSuitedAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("6c6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4d5s");
            var cards3 = CardGroup.Parse("2c3c4d5c");
            var cards4 = CardGroup.Parse("Ac2c3d4d");
            var cards5 = CardGroup.Parse("2c3c4c5d");
            var cards6 = CardGroup.Parse("2c3c4sAd");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.True);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
            Assert.That(analyzer.Analyze(cards6, item), Is.False);
        }

        [Test]
        public void TestOmahaNoAceSuitedAnalyzer()
        {
            var analyzer = new OmahaNoAceSuitedAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("6c6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4d4s");
            var cards3 = CardGroup.Parse("2c3c3d4s");
            var cards4 = CardGroup.Parse("Ac2c3d4d");
            var cards5 = CardGroup.Parse("2c3c4c5d");
            var cards6 = CardGroup.Parse("2c3h4sAd");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.True);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
            Assert.That(analyzer.Analyze(cards6, item), Is.False);
        }

        [Test]
        public void TestOmahaDoubleSuitedAnalyzer()
        {
            var analyzer = new OmahaDoubleSuitedAnalyzer();

            var item = new OmahaHandGridItem();
            var cards = CardGroup.Parse("6c6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4d4s");
            var cards3 = CardGroup.Parse("2c3c3d4c");
            var cards4 = CardGroup.Parse("Ac2c3d4d");
            var cards5 = CardGroup.Parse("2c3c4c5d");
            var cards6 = CardGroup.Parse("2c3c4sAd");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.True);
            Assert.That(analyzer.Analyze(cards5, item), Is.False);
            Assert.That(analyzer.Analyze(cards6, item), Is.False);
        }

        [Test]
        public void TestOmahaHoleCardStructureAcesAnalyzer()
        {
            var analyzer = new OmahaHoleCardStructureAcesAnalyzer();

            var item = new HoleCardStructureHandGridItem() { SelectedNumber = 1 };
            var item2 = new HoleCardStructureHandGridItem() { SelectedNumber = 2 };
            var cards = CardGroup.Parse("Ac6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4dAs");
            var cards3 = CardGroup.Parse("KdAdAsAh");
            var cards4 = CardGroup.Parse("KdTs9s8s");

            Assert.That(analyzer.Analyze(cards, item), Is.True);
            Assert.That(analyzer.Analyze(cards, item2), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.True);
            Assert.That(analyzer.Analyze(cards3, item), Is.False);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.False);
        }

        [Test]
        public void TestOmahaHoleCardStructureBroadwaysAnalyzer()
        {
            var analyzer = new OmahaHoleCardStructureBroadwaysAnalyzer();

            var item = new HoleCardStructureHandGridItem() { SelectedNumber = 1 };
            var item2 = new HoleCardStructureHandGridItem() { SelectedNumber = 2 };
            var cards = CardGroup.Parse("Ac6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4dAs");
            var cards3 = CardGroup.Parse("Kd9s8s7s");
            var cards4 = CardGroup.Parse("KdTs9s8s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards, item2), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.True);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.True);
        }

        [Test]
        public void TestOmahaHoleCardStructureMidHandAnalyzer()
        {
            var analyzer = new OmahaHoleCardStructureMidHandAnalyzer();

            var item = new HoleCardStructureHandGridItem() { SelectedNumber = 1 };
            var item2 = new HoleCardStructureHandGridItem() { SelectedNumber = 2 };
            var cards = CardGroup.Parse("Ac6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4dAs");
            var cards3 = CardGroup.Parse("KdAsAd7s");
            var cards4 = CardGroup.Parse("KdTs9s8s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards, item2), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.True);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.True);
        }

        [Test]
        public void TestOmahaHoleCardStructureLowCardsAnalyzer()
        {
            var analyzer = new OmahaHoleCardStructureLowCardsAnalyzer();

            var item = new HoleCardStructureHandGridItem() { SelectedNumber = 1 };
            var item2 = new HoleCardStructureHandGridItem() { SelectedNumber = 2 };
            var cards = CardGroup.Parse("Ac6d6h6s");
            var cards2 = CardGroup.Parse("Ac2c4d3s");
            var cards3 = CardGroup.Parse("KdAsAd2s");
            var cards4 = CardGroup.Parse("KdTs2s5s");

            Assert.That(analyzer.Analyze(cards, item), Is.False);
            Assert.That(analyzer.Analyze(cards, item2), Is.False);
            Assert.That(analyzer.Analyze(cards2, item), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item), Is.True);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.True);
        }

        [Test]
        public void TestOmahaTwoCardWrapAnalyzer()
        {
            var analyzer = new OmahaTwoCardWrapAnalyzer();

            var cards0 = CardGroup.Parse("2s3sJsJd");
            var cards1 = CardGroup.Parse("2s4sJsJd");
            var cards2 = CardGroup.Parse("As4sJsJd");
            var cards3 = CardGroup.Parse("Ad5sAcAs");
            var cards4 = CardGroup.Parse("Ad6sAcAs");

            var item0 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 0) };
            var item1 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 1) };
            var item2 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 2) };
            var item3 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 3) };
            var item4 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 4) };

            Assert.That(analyzer.Analyze(cards0, item0), Is.True);
            Assert.That(analyzer.Analyze(cards0, item1), Is.False);
            Assert.That(analyzer.Analyze(cards0, item2), Is.False);
            Assert.That(analyzer.Analyze(cards0, item3), Is.False);
            Assert.That(analyzer.Analyze(cards0, item4), Is.False);

            Assert.That(analyzer.Analyze(cards1, item0), Is.False);
            Assert.That(analyzer.Analyze(cards1, item1), Is.True);
            Assert.That(analyzer.Analyze(cards1, item2), Is.False);
            Assert.That(analyzer.Analyze(cards1, item3), Is.False);
            Assert.That(analyzer.Analyze(cards1, item4), Is.False);

            Assert.That(analyzer.Analyze(cards2, item0), Is.False);
            Assert.That(analyzer.Analyze(cards2, item1), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.True);
            Assert.That(analyzer.Analyze(cards2, item3), Is.False);
            Assert.That(analyzer.Analyze(cards2, item4), Is.False);

            Assert.That(analyzer.Analyze(cards3, item0), Is.False);
            Assert.That(analyzer.Analyze(cards3, item1), Is.False);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item3), Is.True);
            Assert.That(analyzer.Analyze(cards3, item4), Is.False);

            Assert.That(analyzer.Analyze(cards4, item0), Is.False);
            Assert.That(analyzer.Analyze(cards4, item1), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item3), Is.False);
            Assert.That(analyzer.Analyze(cards4, item4), Is.True);
        }

        [Test]
        public void TestOmahaThreeCardWrapAnalyzer()
        {
            var analyzer = new OmahaThreeCardWrapAnalyzer();

            var cards0 = CardGroup.Parse("2s3s4sJd");
            var cards1 = CardGroup.Parse("2s4s5sJd");
            var cards2 = CardGroup.Parse("As4s5sJd");
            var cards3 = CardGroup.Parse("Ad5s6sAd");
            var cards4 = CardGroup.Parse("Ad2d7s7d");

            var item0 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 0) };
            var item1 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 1) };
            var item2 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 2) };
            var item3 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 3) };
            var item4 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 4) };

            Assert.That(analyzer.Analyze(cards0, item0), Is.True);
            Assert.That(analyzer.Analyze(cards0, item0), Is.True);
            Assert.That(analyzer.Analyze(cards0, item1), Is.False);
            Assert.That(analyzer.Analyze(cards0, item2), Is.False);
            Assert.That(analyzer.Analyze(cards0, item3), Is.False);
            Assert.That(analyzer.Analyze(cards0, item4), Is.False);

            Assert.That(analyzer.Analyze(cards1, item0), Is.False);
            Assert.That(analyzer.Analyze(cards1, item1), Is.True);
            Assert.That(analyzer.Analyze(cards1, item2), Is.False);
            Assert.That(analyzer.Analyze(cards1, item3), Is.False);
            Assert.That(analyzer.Analyze(cards1, item4), Is.False);

            Assert.That(analyzer.Analyze(cards2, item0), Is.False);
            Assert.That(analyzer.Analyze(cards2, item1), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.True);
            Assert.That(analyzer.Analyze(cards2, item3), Is.False);
            Assert.That(analyzer.Analyze(cards2, item4), Is.False);

            Assert.That(analyzer.Analyze(cards3, item0), Is.False);
            Assert.That(analyzer.Analyze(cards3, item1), Is.False);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item3), Is.True);
            Assert.That(analyzer.Analyze(cards3, item4), Is.False);

            Assert.That(analyzer.Analyze(cards4, item0), Is.False);
            Assert.That(analyzer.Analyze(cards4, item1), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item3), Is.False);
            Assert.That(analyzer.Analyze(cards4, item4), Is.True);
        }

        [Test]
        public void TestOmahaFourCardWrapAnalyzer()
        {
            var analyzer = new OmahaFourCardWrapAnalyzer();

            var cards0 = CardGroup.Parse("2s3s4sAd");
            var cards1 = CardGroup.Parse("2s4s5s6d");
            var cards2 = CardGroup.Parse("As2s3s6d");
            var cards3 = CardGroup.Parse("Ad5s6s7d");
            var cards4 = CardGroup.Parse("Ad2d7s8s");

            var item0 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 0) };
            var item1 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 1) };
            var item2 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 2) };
            var item3 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 3) };
            var item4 = new WrapsAndRundownsHandGridItem() { SelectedGap = new Tuple<string, int>("", 4) };

            Assert.That(analyzer.Analyze(cards0, item0), Is.True);
            Assert.That(analyzer.Analyze(cards0, item0), Is.True);
            Assert.That(analyzer.Analyze(cards0, item1), Is.False);
            Assert.That(analyzer.Analyze(cards0, item2), Is.False);
            Assert.That(analyzer.Analyze(cards0, item3), Is.False);
            Assert.That(analyzer.Analyze(cards0, item4), Is.False);

            Assert.That(analyzer.Analyze(cards1, item0), Is.False);
            Assert.That(analyzer.Analyze(cards1, item1), Is.True);
            Assert.That(analyzer.Analyze(cards1, item2), Is.False);
            Assert.That(analyzer.Analyze(cards1, item3), Is.False);
            Assert.That(analyzer.Analyze(cards1, item4), Is.False);

            Assert.That(analyzer.Analyze(cards2, item0), Is.False);
            Assert.That(analyzer.Analyze(cards2, item1), Is.False);
            Assert.That(analyzer.Analyze(cards2, item2), Is.True);
            Assert.That(analyzer.Analyze(cards2, item3), Is.False);
            Assert.That(analyzer.Analyze(cards2, item4), Is.False);

            Assert.That(analyzer.Analyze(cards3, item0), Is.False);
            Assert.That(analyzer.Analyze(cards3, item1), Is.False);
            Assert.That(analyzer.Analyze(cards3, item2), Is.False);
            Assert.That(analyzer.Analyze(cards3, item3), Is.True);
            Assert.That(analyzer.Analyze(cards3, item4), Is.False);

            Assert.That(analyzer.Analyze(cards4, item0), Is.False);
            Assert.That(analyzer.Analyze(cards4, item1), Is.False);
            Assert.That(analyzer.Analyze(cards4, item2), Is.False);
            Assert.That(analyzer.Analyze(cards4, item3), Is.False);
            Assert.That(analyzer.Analyze(cards4, item4), Is.True);
        }
    }
}
