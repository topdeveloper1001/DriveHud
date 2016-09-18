using HandHistories.Objects.Cards;
using HoldemHand;
using Model.BoardTextureAnalyzers;
using Model.Filters;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHud.Tests.UnitTests
{
    [TestFixture]
    public class BoardTextureTests
    {
        [Test]
        public void TestHandEvaluator()
        {
            string board = "8d 5d 6c";
            Hand player1 = new Hand("ac as", board);
            var player1_mask = Hand.ParseHand("8d 5d 6c ac as");
            Hand player2 = new Hand("2c Ks", board); 
            Hand player3 = new Hand("4c Ks Ac As", board); 
            Hand player4 = new Hand("Tc Ks", board); 
            Hand player5 = new Hand("3d Kd", board);
            Hand player6 = new Hand("4d Ks Ad As", board);
        }

        [Test]
        public void TestExactCardsTextureAnalyzer()
        {
            var analyzer = new ExactCardsTextureAnalyzer();
            var boardCards = "4s 5h 7s";
            var targetCards = new List<BoardCardItem>()
            { //4s 5h 7s
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Four, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop },
                new BoardCardItem {Rank = Model.Enums.RangeCardRank.Seven, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop }
            };

            var targetCards2 = new List<BoardCardItem>()
            { //4d 5h 7s
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Four, Suit = Model.Enums.RangeCardSuit.Diamonds, TargetStreet = Street.Flop },
                new BoardCardItem {Rank = Model.Enums.RangeCardRank.Seven, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop }
            };
            var targetCards3 = new List<BoardCardItem>()
            { //4s 5h Xh
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Four, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop },
                new BoardCardItem {Rank = Model.Enums.RangeCardRank.None, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop }
            };
            var targetCards4 = new List<BoardCardItem>()
            { //Xs 5h 7s
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.None, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop },
                new BoardCardItem {Rank = Model.Enums.RangeCardRank.Seven, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop }
            };

            var targetCards5 = new List<BoardCardItem>()
            { //Xs 5h
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.None, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop },
            };

            var targetCards6 = new List<BoardCardItem>()
            { //4s 5h
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Five, Suit = Model.Enums.RangeCardSuit.Hearts, TargetStreet = Street.Flop },
                new BoardCardItem { Rank = Model.Enums.RangeCardRank.Four, Suit = Model.Enums.RangeCardSuit.Spades, TargetStreet = Street.Flop },
            };

            Assert.That(analyzer.Analyze(boardCards, targetCards), Is.True);
            Assert.That(analyzer.Analyze(boardCards, targetCards2), Is.False);
            Assert.That(analyzer.Analyze(boardCards, targetCards3), Is.False);
            Assert.That(analyzer.Analyze(boardCards, targetCards4), Is.True);
            Assert.That(analyzer.Analyze(boardCards, targetCards5), Is.True);
            Assert.That(analyzer.Analyze(boardCards, targetCards6), Is.True);
        }

        [Test]
        public void TestHighCardTextureAnalyzer()
        {
            var analyzer = new HighCardTextureAnalyzer();
            var highCard = new HighCardBoardTextureItem() { SelectedRank = "6", TargetStreet = Street.Flop };
            var boardCards = BoardCards.FromCards("4s 5s 6s");
            var boardCards2 = BoardCards.FromCards("4s 5s 7s");
            var boardCards3 = BoardCards.FromCards("8s 5s 6s");
            var boardCards4 = BoardCards.FromCards("4s 5s 5h 6s");
            var boardCards5 = BoardCards.FromCards("4s 5s 6s 7s");

            Assert.That(analyzer.Analyze(boardCards, highCard), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, highCard), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, highCard), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, highCard), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, highCard), Is.True);
        }

        [Test]
        public void TestRainbowTextureAnalyzer()
        {
            var analyzer = new RainbowTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Flop };
            var boardCards = BoardCards.FromCards("4s 5d 6c");
            var boardCards2 = BoardCards.FromCards("4s 5s 7c");
            var boardCards3 = BoardCards.FromCards("4s 5d 5c 6s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
        }

        [Test]
        public void TestTwoFlushDrawsTextureAnalyzer()
        {
            var analyzer = new TwoFlushDrawsTextureAnalyzer();

            var item = new BoardTextureItem() { TargetStreet = Street.Turn };
            var boardCards = BoardCards.FromCards("4d 5s 6d 7s");
            var boardCards2 = BoardCards.FromCards("4s 5d 7c 8c");
            var boardCards3 = BoardCards.FromCards("4s 6s 7d 6c 8c");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
        }

        [Test]
        public void TestTwoToneTextureAnalyzer()
        {
            var analyzer = new TwoToneTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Flop };
            var boardCards = BoardCards.FromCards("4s 5s 6d");
            var boardCards2 = BoardCards.FromCards("4s 5d 7c");
            var boardCards3 = BoardCards.FromCards("4s 6s 7d 6c");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
        }

        [Test]
        public void TestThreeToneTextureAnalyzer()
        {
            var analyzer = new ThreeToneTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Flop };
            var boardCards = BoardCards.FromCards("4s 5c 6d");
            var boardCards2 = BoardCards.FromCards("4s 5d 7d");
            var boardCards3 = BoardCards.FromCards("4s 6c 7d 8c");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
        }

        [Test]
        public void TestMonotoneTextureAnalyzer()
        {
            var analyzer = new MonotoneTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Flop };
            var boardCards = BoardCards.FromCards("4s 5s 6s");
            var boardCards2 = BoardCards.FromCards("4s 5s 7c");
            var boardCards3 = BoardCards.FromCards("4s 6s 7s 6d");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
        }

        [Test]
        public void TestNoFlushPossibleTextureAnalyzer()
        {
            var analyzer = new NoFlushPossibleTextureAnalyzer();

            var item = new BoardTextureItem() { TargetStreet = Street.River };
            var boardCards = BoardCards.FromCards("4s 5s 6d 7d 8c");
            var boardCards2 = BoardCards.FromCards("4s 5s 7s 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
        }

        [Test]
        public void TestFlushPossibleTextureAnalyzer()
        {
            var analyzer = new FlushPossibleTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.River };
            var boardCards = BoardCards.FromCards("4s 5s 6d 7d 8s");
            var boardCards2 = BoardCards.FromCards("4s 5s 7s 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
        }

        [Test]
        public void TestFourFlushTextureAnalyzer()
        {
            var analyzer = new FourFlushTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.River };
            var boardCards = BoardCards.FromCards("4s 5s 6d 7s 8s");
            var boardCards2 = BoardCards.FromCards("4s 5s 7s 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
        }

        [Test]
        public void TestFlushOnBoardTextureAnalyzer()
        {
            var analyzer = new FlushOnBoardTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.River };
            var boardCards = BoardCards.FromCards("4s 5s 6s 7s 8s");
            var boardCards2 = BoardCards.FromCards("4s 5s 7d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
        }

        [Test]
        public void TestNoPairTextureAnalyzer()
        {
            var analyzer = new NoPairTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Turn };
            var boardCards = BoardCards.FromCards("4s 5s 6s 7s 7d");
            var boardCards2 = BoardCards.FromCards("4s 7s 7d 7c 9s");
            var boardCards3 = BoardCards.FromCards("4s 7s 7d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
        }

        [Test]
        public void TestSinglePairTextureAnalyzer()
        {
            var analyzer = new SinglePairTextureAnalyzer();
            var item = new BoardTextureItem() { TargetStreet = Street.Turn };
            var boardCards = BoardCards.FromCards("4s 5s 7s 7c 7d");
            var boardCards2 = BoardCards.FromCards("4s 7s 7d 7c 9s");
            var boardCards3 = BoardCards.FromCards("4s 7s 7d 8s 9s");
            var boardCards4 = BoardCards.FromCards("7s 7d 8d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
        }

        [Test]
        public void TestTwoPairTextureAnalyzer()
        {
            var analyzer = new TwoPairTextureAnalyzer();

            var itemTurn = new BoardTextureItem() { TargetStreet = Street.Turn };
            var itemRiver = new BoardTextureItem() { TargetStreet = Street.River };

            var boardCards = BoardCards.FromCards("5c 5s 7s 7c 7d");
            var boardCards2 = BoardCards.FromCards("3s 4s 7d 7c 4c");
            var boardCards3 = BoardCards.FromCards("4s 7s 7d 8s 9s");
            var boardCards4 = BoardCards.FromCards("7s 7d 8d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, itemTurn), Is.True);
            Assert.That(analyzer.Analyze(boardCards, itemRiver), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemRiver), Is.False);
        }

        [Test]
        public void TestThreeOfAKindTextureAnalyzer()
        {
            var analyzer = new ThreeOfAKindTextureAnalyzer();

            var itemTurn = new BoardTextureItem() { TargetStreet = Street.Turn };
            var itemRiver = new BoardTextureItem() { TargetStreet = Street.River };

            var boardCards = BoardCards.FromCards("5c 6s 7s 5d 5h");
            var boardCards2 = BoardCards.FromCards("3s 7s 7d 7c 4c");
            var boardCards3 = BoardCards.FromCards("4s 7s 8d 8s 9s");
            var boardCards4 = BoardCards.FromCards("7s 7d 8d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, itemRiver), Is.True);
            Assert.That(analyzer.Analyze(boardCards, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemRiver), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemRiver), Is.False);
        }

        [Test]
        public void TestFourOfAKindTextureAnalyzer()
        {
            var analyzer = new FourOfAKindTextureAnalyzer();

            var itemTurn = new BoardTextureItem() { TargetStreet = Street.Turn };
            var itemRiver = new BoardTextureItem() { TargetStreet = Street.River };

            var boardCards = BoardCards.FromCards("5c 5s 7s 5d 5h");
            var boardCards2 = BoardCards.FromCards("7h 7s 7d 7c 4c");
            var boardCards3 = BoardCards.FromCards("4s 8h 8d 8s 9s");
            var boardCards4 = BoardCards.FromCards("7s 7d 7h 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, itemRiver), Is.True);
            Assert.That(analyzer.Analyze(boardCards, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemRiver), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemTurn), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemRiver), Is.False);
        }

        [Test]
        public void TestFullHouseTextureAnalyzer()
        {
            var analyzer = new FullHouseTextureAnalyzer();

            var itemRiver = new BoardTextureItem() { TargetStreet = Street.River };

            var boardCards = BoardCards.FromCards("5c 5s 7s 7d 5h");
            var boardCards2 = BoardCards.FromCards("7h 7s 7d 7c 4c");
            var boardCards3 = BoardCards.FromCards("4s 8h 8d 8s 9s");

            Assert.That(analyzer.Analyze(boardCards, itemRiver), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, itemRiver), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemRiver), Is.False);
        }

        [Test]
        public void TestUncoordinatedTextureAnalyzer()
        {
            var analyzer = new UncoordinatedTextureAnalyzer();

            var item = new StraightBoardTextureItem() { TargetStreet = Street.Flop };

            var boardCards = BoardCards.FromCards("6s 5s 4d");
            var boardCards2 = BoardCards.FromCards("4s 7d 3c");
            var boardCards3 = BoardCards.FromCards("4s 4d 7d 6c");
            var boardCards4 = BoardCards.FromCards("4s 4d 5d 8d");
            var boardCards5 = BoardCards.FromCards("4s 4d 6d 5d");
            var boardCards6 = BoardCards.FromCards("As 2d 4c");


            Assert.That(analyzer.Analyze(boardCards, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item), Is.False);
        }

        [Test]
        public void TestOneGapperTextureAnalyzer()
        {
            var analyzer = new OneGapperTextureAnalyzer();

            var item = new StraightBoardTextureItem() { TargetStreet = Street.Flop };

            var boardCards = BoardCards.FromCards("4s 4d 6d 5d");
            var boardCards2 = BoardCards.FromCards("4s 7d 3c");
            var boardCards3 = BoardCards.FromCards("4s 4d 7d 6c");
            var boardCards4 = BoardCards.FromCards("4s 4d 5d 8d");
            var boardCards5 = BoardCards.FromCards("6s 5s 4d");
            var boardCards6 = BoardCards.FromCards("As 3s 4d");


            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, item), Is.True);
        }

        [Test]
        public void TestTwoGapperTextureAnalyzer()
        {
            var analyzer = new TwoGapperTextureAnalyzer();

            var item = new StraightBoardTextureItem() { TargetStreet = Street.Flop };

            var boardCards = BoardCards.FromCards("3s 4d 7d 5d");
            var boardCards2 = BoardCards.FromCards("4s 7d 3c");
            var boardCards3 = BoardCards.FromCards("4s 4d 5d 8c");
            var boardCards4 = BoardCards.FromCards("4s 5d 7d");
            var boardCards5 = BoardCards.FromCards("7s 5s 4d");
            var boardCards6 = BoardCards.FromCards("4s As 5d");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, item), Is.True);

        }

        [Test]
        public void TestThreeConnectedTextureAnalyzer()
        {
            var analyzer = new ThreeConnectedTextureAnalyzer();

            var item = new StraightBoardTextureItem() { TargetStreet = Street.Flop };

            var boardCards = BoardCards.FromCards("6s 5d 4d 7d");
            var boardCards2 = BoardCards.FromCards("As 2d 3c");
            var boardCards3 = BoardCards.FromCards("As 3d 4d 5c");
            var boardCards4 = BoardCards.FromCards("4s 5d 7d");
            var boardCards5 = BoardCards.FromCards("7s 5s 4d");
            var boardCards6 = BoardCards.FromCards("3s As 2d");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, item), Is.True);
        }

        [Test]
        public void TestFourConnectedTextureAnalyzer()
        {
            var analyzer = new FourConnectedTextureAnalyzer();

            var item = new StraightBoardTextureItem() { TargetStreet = Street.Turn };

            var boardCards = BoardCards.FromCards("6s 5d 4d 7d Ad");
            var boardCards2 = BoardCards.FromCards("As 2d 3c 4c");
            var boardCards3 = BoardCards.FromCards("As 3d 4d 5c");
            var boardCards4 = BoardCards.FromCards("As 2d Qd Kc");
            var boardCards5 = BoardCards.FromCards("3s As 2d 4d");
            var boardCards6 = BoardCards.FromCards("As Ks Qs Js 8s");

            Assert.That(analyzer.Analyze(boardCards, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item), Is.True);
        }

        [Test]
        public void TestOpenEndedStraightTextureAnalyzer()
        {
            var analyzer = new OpenEndedStraightTextureAnalyzer();

            var item3 = new StraightBoardTextureItem() { TargetStreet = Street.Flop, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.EqualTo, "") };
            var itemLessThan3 = new StraightBoardTextureItem() { TargetStreet = Street.Flop, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.LessThan, "") };
            var itemGreaterThan2 = new StraightBoardTextureItem() { TargetStreet = Street.Flop, SelectedNumber = 2, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.GreaterThan, "") };

            /* 3 */
            var boardCards = BoardCards.FromCards("8s 3s 6s");
            var boardCards2 = BoardCards.FromCards("3s 8s 6s");
            var boardCards3 = BoardCards.FromCards("8s 5s Ts");
            var boardCards4 = BoardCards.FromCards("8s 6s 4s 8h 4h");
            var boardCards5 = BoardCards.FromCards("Ks Ts 8s 7s As");

            /* less than 3*/
            var boardCards6 = BoardCards.FromCards("8s Ks Qs 7s Js");
            var boardCards7 = BoardCards.FromCards("3s Ks 7s Qs Kh");
            var boardCards8 = BoardCards.FromCards("3s 5h As 5s 2s");

            Assert.That(analyzer.Analyze(boardCards, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, item3), Is.False);

            Assert.That(analyzer.Analyze(boardCards, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards7, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards8, itemLessThan3), Is.True);

            Assert.That(analyzer.Analyze(boardCards, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, itemGreaterThan2), Is.False);

        }

        [Test]
        public void TestMadeStraightTextureAnalyzer()
        {
            var analyzer = new MadeStraightTextureAnalyzer();

            var item3 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.EqualTo, "") };
            var itemLessThan3 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.LessThan, "") };
            var itemGreaterThan2 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 2, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.GreaterThan, "") };

            /* 3 */
            var boardCards = BoardCards.FromCards("Ks Qs 8s Js 4s"); // !
            var boardCards2 = BoardCards.FromCards("6s 5s 7s 5h");
            var boardCards3 = BoardCards.FromCards("3s 8s 6s 4s 4h"); // !
            var boardCards4 = BoardCards.FromCards("6s 4s 5s Qh 3h");
            var boardCards5 = BoardCards.FromCards("Qs 8s 7s 6s 6h");

            /* less than 3*/
            var boardCards6 = BoardCards.FromCards("4s 2s 6s 2h");
            var boardCards7 = BoardCards.FromCards("Qs 2s 3s 2h");
            var boardCards8 = BoardCards.FromCards("6s 2h Ks 2d 2s");

            Assert.That(analyzer.Analyze(boardCards, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, item3), Is.False);

            Assert.That(analyzer.Analyze(boardCards, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards7, itemLessThan3), Is.True);
            Assert.That(analyzer.Analyze(boardCards8, itemLessThan3), Is.True);

            Assert.That(analyzer.Analyze(boardCards, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, itemGreaterThan2), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, itemGreaterThan2), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, itemGreaterThan2), Is.False);
        }

        [Test]
        public void TestOpenEndedBeatNutsTextureAnalyzer()
        {
            var analyzer = new OpenEndedBeatNutsTextureAnalyzer();

            var item3 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 1, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.EqualTo, "") };

            /* true */
            var boardCards = BoardCards.FromCards("6s 8s 5s 9s");
            var boardCards2 = BoardCards.FromCards("Js 8s 2s Ts 9h");
            var boardCards3 = BoardCards.FromCards("5s 4s Js 2s");
            var boardCards4 = BoardCards.FromCards("8s 6s 4s 8h 4h");
            var boardCards5 = BoardCards.FromCards("Ks Ts 8s 7s As");
            var boardCards8 = BoardCards.FromCards("3s 5h As 5s 2s");


            /* false */
            var boardCards6 = BoardCards.FromCards("8s Ks Qs 7s Js");
            var boardCards7 = BoardCards.FromCards("3s Ks 7s Qs Kh");

            Assert.That(analyzer.Analyze(boardCards, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, item3), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, item3), Is.True);
        }

        [Test]
        public void TestGutShotBeatNutsTextureAnalyzer()
        {
            var analyzer = new GutShotBeatNutsTextureAnalyzer();

            var item3 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.EqualTo, "") };
            var itemLessThan3 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 3, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.LessThan, "") };
            var item2 = new StraightBoardTextureItem() { TargetStreet = Street.Turn, SelectedNumber = 1, SelectedSign = new KeyValuePair<Model.Enums.EnumEquality, string>(Model.Enums.EnumEquality.EqualTo, "") };

            /* 3 */
            var boardCards = BoardCards.FromCards("6s 8s 5s 9s");
            var boardCards2 = BoardCards.FromCards("Js 8s 2s Ts 9s");
            var boardCards3 = BoardCards.FromCards("2s 5s Js 4s");
            var boardCards4 = BoardCards.FromCards("5s 4s Js 2h 3h");
            var boardCards5 = BoardCards.FromCards("2s Ts Js 8s 7s");
            var boardCards6 = BoardCards.FromCards("4s 6s 7s 2s 5s");

            /* less than 3 */
            var boardCards7 = BoardCards.FromCards("3s 8s 6s 4s 4h");
            var boardCards8 = BoardCards.FromCards("Qs Qh 9s Js 5s");

            Assert.That(analyzer.Analyze(boardCards, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards2, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards3, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards4, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards5, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards6, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards7, item3), Is.True);
            Assert.That(analyzer.Analyze(boardCards8, item3), Is.False);

            Assert.That(analyzer.Analyze(boardCards, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, itemLessThan3), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, itemLessThan3), Is.True);

            Assert.That(analyzer.Analyze(boardCards, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards2, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards3, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards4, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards5, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards6, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards7, item2), Is.False);
            Assert.That(analyzer.Analyze(boardCards8, item2), Is.True);
        }

    }
}
