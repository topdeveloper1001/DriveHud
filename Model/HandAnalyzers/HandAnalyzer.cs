using DriveHUD.Common.Log;
using HandHistories.Objects.Cards;
using Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards = HandHistories.Objects.Cards;

namespace Model.HandAnalyzers
{
    public class HandAnalyzer
    {
        private readonly IAnalyzer[] _combinations;

        public HandAnalyzer(params IAnalyzer[] combinations)
        {
            _combinations = combinations.OrderByDescending(r => r.GetRank()).ToArray();
        }

        public IAnalyzer Analyze(IEnumerable<HandHistories.Objects.Cards.Card> playerCards, BoardCards boardCards)
        {
            try
            {
                IAnalyzer result = null;

                var highestHand = CardHelper.FindBestHand(string.Join("", playerCards.Select(c => c.CardStringValue)), boardCards.ToString());

                var analyzers = ReferenceEquals(highestHand, null) ? _combinations : _combinations.Where(x => x.IsValidAnalyzer(highestHand)).ToArray();

                if (playerCards.Count() > 2 && !string.IsNullOrEmpty(highestHand.PocketCards))
                {
                    result = analyzers.FirstOrDefault(combination => combination.Analyze(CardGroup.Parse(highestHand.PocketCards), boardCards)) ?? new StubAnalyzer();
                }
                else
                {
                    result = analyzers.FirstOrDefault(combination => combination.Analyze(playerCards, boardCards)) ?? new StubAnalyzer();
                }

                return result;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, String.Format("Hand Analyzer Error occurred: Player cards = {0}; Board Cards = {1}", string.Join("", playerCards.Select(c => c.CardStringValue)), boardCards.ToString()), ex);
            }

            return new StubAnalyzer();
        }

        #region Default Analyzers
        public static IAnalyzer[] GetReportAnalyzers()
        {
            return new IAnalyzer[]
                {
                    new StubAnalyzer(),
                    new HighCardAnalyzer(),
                    new PairedBoardAnalyzer(),
                    new PocketPairOverpairAnalyzer(),
                    new PocketPairSecondOrWorseAnalyzer(),
                    new TopPairAnalyzer(),
                    new TopPairTopKickerAnalyzer(),
                    new SecondPairAnalyzer(),
                    new ThirdPairAnalyzer(),
                    new BottomPairAnalyzer(),
                    new TwoPairOnBoardAnaluzer(),
                    new TwoPairNoTopPairAnalyzer(),
                    new TwoPairTopTwoPairAnalyzer(),
                    new ThreeOfAKindOnBoardAnalyzer(),
                    new ThreeOfAKindTopSetAnalyzer(),
                    new ThreeOfAKindSecondSetAnalyzer(),
                    new ThreeOfAKindMiddleSetAnalyzer(),
                    new ThreeOfAKindBottomSetAnalyzer(),
                    new ThreeOfAKindTripsHighKickerAnalyzer(),
                    new ThreeOfAKindTripsWeakKickerAnalyzer(),
                    new ThreeOfAKindTripsOnFlopAnalyzer(),
                    new StraightOnBoardAnalyzer(),
                    new StraightOneCardAnalyzer(),
                    new StraightOneCardNutAnalyzer(),
                    new StraightOneCardBottomAnalyzer(),
                    new StraightTwoCardAnalyzer(),
                    new StraightTwoCardNutAnalyzer(),
                    new StraightTwoCardBottomAnalyzer(),
                    new FlushOnBoardAnalyzer(),
                    new FlushTwoCardHighAnalyzer(),
                    new FlushTwoCardLowAnalyzer(),
                    new FlushOneCardHighAnalyzer(),
                    new FlushOneCardNutAnalyzer(),
                    new FlushOneCardLowAnalyzer(),
                    new FullHouseOnBoardAnalyzer(),
                    new FullHousePocketPairNoTripsOnBoardAnalyzer(),
                    new FullHousePocketPairTripsOnBoardAnalyzer(),
                    new FullHouseOneHoleCardNoTripsOnBoardAnalyzer(),
                    new FullHouseTwoPocketCardsNoTripsOnBoardAnalyzer(),
                    new FourOfAKindOnBoardAnalyzer(),
                    new FourOfAKindNoPocketPairAnalyzer(),
                    new FourOfAKindPocketPairAnalyzer(),
                    new StraightFlushOneHoleCardAnalyzer(),
                    new StraightFlushTwoPocketCardsAnalyzer(),
                    new RoyalFlushOnBoardAnalyzer(),
                    new RoyalFlushOneHoleCardAnalyzer(),
                    new RoyalFlushTwoPocketCardsAnalyzer(),
                };
        }

        public static IAnalyzer[] GetHandValuesAnalyzers()
        {
            return new IAnalyzer[] {
                     new StubAnalyzer(),
                     new HighCardNoOverCardsAnalyzer(),
                     new HighCardOneOvercardAnalyzer(),
                     new HighCardTwoOvercardsAnalyzer(),
                     new PairedBoardAnalyzer(),
                     new PairedBoardNoOvercardsAnalyzer(),
                     new PairedBoardOneOvercardAnalyzer(),
                     new PairedBoardTwoOvercardsAnalyzer(),
                     new PocketPairUnderPairAnalyzer(),
                     new PocketPairThirdPairAnalyzer(),
                     new PocketPairSecondPairAnalyzer(),
                     new PocketPairOverpairAnalyzer(),
                     new TopPairWeakKickerAnalyzer(),
                     new TopPairDecentKickerAnalyzer(),
                     new TopPairTopKickerAnalyzer(),
                     new SecondPairAnalyzer(),
                     new SecondPairWeakKickerAnalyzer(),
                     new SecondPairDecentKickerAnalyzer(),
                     new SecondPairTopKickerAnalyzer(),
                     new ThirdPairAnalyzer(),
                     new BottomPairWeakKickerAnalyzer(),
                     new BottomPairDecentKickerAnalyzer(),
                     new BottomPairTopKickerAnalyzer(),
                     new TwoPairOnBoardAnaluzer(),
                     new TwoPairBottomTwoPairAnalyzer(),
                     new TwoPairTopAndBottomPairAnalyzer(),
                     new TwoPairTopTwoPairAnalyzer(),
                     new TwoPairPairedBoardBottomPairAnalyzer(),
                     new TwoPairPairedBoardTopPairAnalyzer(),
                     new TwoPairPairedBoardPocketPairSecondPairAnalyzer(),
                     new TwoPairPairedBoardPocketPairOverpairAnalyzer(),
                     new ThreeOfAKindOnBoardAnalyzer(),
                     new ThreeOfAKindBottomSetAnalyzer(),
                     new ThreeOfAKindMiddleSetAnalyzer(),
                     new ThreeOfAKindTopSetAnalyzer(),
                     new ThreeOfAKindTripsOnFlopAnalyzer(),
                     new ThreeOfAKindTripsSecondSetWeakKickerAnalyzer(),
                     new ThreeOfAKindTripsSecondSetHighKickerAnalyzer(),
                     new ThreeOfAKindTripsTopSetWeakKickerAnalyzer(),
                     new ThreeOfAKindTripsTopSetHighKickerAnalyzer(),
                     new StraightOnBoardAnalyzer(),
                     new StraightOneCardAnalyzer(),
                     new StraightTwoCardAnalyzer(),
                     new StraightTwoCardNutAnalyzer(),
                     new FlushLowAnalyzer(),
                     new FlushHighAnalyzer(),
                     new FlushNutAnalyzer(),
                     new FullHouseOnBoardAnalyzer(),
                     new FullHousePocketPairNoTripsOnBoardAnalyzer(),
                     new FullHousePocketPairTripsOnBoardAnalyzer(),
                     new FullHouseTwoPocketCardsNoTripsOnBoardAnalyzer(),
                     new FourOfAKindOnBoardAnalyzer(),
                     new FourOfAKindNoPocketPairAnalyzer(),
                     new FourOfAKindPocketPairAnalyzer(),
                     new StraightFlushOnBoardAnalyzer(),
                     new StraightFlushOneHoleCardAnalyzer(),
                     new StraightFlushTwoPocketCardsAnalyzer(),
                     new RoyalFlushOnBoardAnalyzer(),
                     new RoyalFlushOneHoleCardAnalyzer(),
                     new RoyalFlushTwoPocketCardsAnalyzer(),
            };
        }

        public static IAnalyzer[] GetRiverHandValuesAnalyzers()
        {
            return new IAnalyzer[] {
                     new StubAnalyzer(),
                     new HighCardNoOverCardsAnalyzer(),
                     new HighCardOneOvercardAnalyzer(),
                     new HighCardTwoOvercardsAnalyzer(),
                     new PairedBoardAnalyzer(),
                     new PairedBoardNoOvercardsAnalyzer(),
                     new PairedBoardOneOvercardAnalyzer(),
                     new PairedBoardTwoOvercardsAnalyzer(),
                     new PocketPairUnderPairAnalyzer(),
                     new PocketPairThirdPairAnalyzer(),
                     new PocketPairFourthPairAnalyzer(),
                     new PocketPairSecondPairAnalyzer(),
                     new PocketPairOverpairAnalyzer(),
                     new TopPairWeakKickerAnalyzer(),
                     new TopPairDecentKickerAnalyzer(),
                     new TopPairTopKickerAnalyzer(),
                     new SecondPairWeakKickerAnalyzer(),
                     new SecondPairDecentKickerAnalyzer(),
                     new SecondPairTopKickerAnalyzer(),
                     new ThirdPairTopKickerAnalyzer(),
                     new ThirdPairDecentKickerAnalyzer(),
                     new ThirdPairWeakKickerAnalyzer(),
                     new BottomPairWeakKickerAnalyzer(),
                     new BottomPairDecentKickerAnalyzer(),
                     new BottomPairTopKickerAnalyzer(),
                     new TwoPairOnBoardAnaluzer(),
                     new TwoPairBottomTwoPairAnalyzer(),
                     new TwoPairTopAndBottomPairAnalyzer(),
                     new TwoPairTopTwoPairAnalyzer(),
                     new TwoPairPairedBoardBottomPairAnalyzer(),
                     new TwoPairPairedBoardTopPairAnalyzer(),
                     new TwoPairPairedBoardPocketPairSecondPairAnalyzer(),
                     new TwoPairPairedBoardPocketPairOverpairAnalyzer(),
                     new ThreeOfAKindOnBoardAnalyzer(),
                     new ThreeOfAKindBottomSetAnalyzer(),
                     new ThreeOfAKindMiddleSetAnalyzer(),
                     new ThreeOfAKindSecondSetAnalyzer(),
                     new ThreeOfAKindTopSetAnalyzer(),
                     new ThreeOfAKindTripsOnFlopAnalyzer(),
                     new ThreeOfAKindTripsSecondSetWeakKickerAnalyzer(),
                     new ThreeOfAKindTripsSecondSetHighKickerAnalyzer(),
                     new ThreeOfAKindTripsTopSetWeakKickerAnalyzer(),
                     new ThreeOfAKindTripsTopSetHighKickerAnalyzer(),
                     new StraightTwoCardAnalyzer(),
                     new StraightTwoCardNutAnalyzer(),
                     new StraightOneCardNutAnalyzer(),
                     new StraightOneCardHighAnalyzer(),
                     new StraightOneCardLowAnalyzer(),
                     new StraightOnBoardAnalyzer(),
                     new FlushTwoCardNutAnalyzer(),
                     new FlushTwoCardHighAnalyzer(),
                     new FlushTwoCardLowAnalyzer(),
                     new FlushOneCardNutAnalyzer(),
                     new FlushOneCardHighAnalyzer(),
                     new FlushOneCardLowAnalyzer(),
                     new FlushOnBoardNutAnalyzer(),
                     new FlushOnBoardHighAnalyzer(),
                     new FlushOnBoardLowAnalyzer(),
                     new FullHouseTwoPocketCardsNoTripsOnBoardAnalyzer(),
                     new FullHouseOnePocketCardAnalyzer(),
                     new FullHouseOnBoardAnalyzer(),
                     new FourOfAKindOnBoardAnalyzer(),
                     new FourOfAKindPocketPairAnalyzer(),
                     new FourOfAKindNoPocketPairAnalyzer(),
                     new StraightFlushTwoPocketCardsAnalyzer(),
                     new StraightFlushOneHoleCardAnalyzer(),
                     new StraightFlushOnBoardAnalyzer(),
            };
        }

        public static IAnalyzer[] GetDrawAnalyzers()
        {
            return new IAnalyzer[]
               {
                   new StubAnalyzer(),
                   new StraightDrawTwoCardBackdoorAnalyzer(),
                   new StraightDrawOneCardGutShotAnalyzer(),
                   new StraightDrawOneCardDoubleGutShotAnalyzer(),
                   new StraightDrawOneCardOpenEndedAnalyzer(),
                   new StraightDrawTwoCardGutShotAnalyzer(),
                   new StraightDrawTwoCardDoubleGutShotAnalyzer(),
                   new StraightDrawTwoCardOpenEndedAnalyzer(),
                   new StraightDrawNoStraightDrawAnalyzer(),
                   new FlushDrawBackdoorOneCardAnalyzer(),
                   new FlushDrawBackdoorTwoCardAnalyzer(),
                   new FlushDrawBackdoorTwoCardNutAnalyzer(),
                   new FlushDrawOneCardLowAnalyzer(),
                   new FlushDrawOneCardHighAnalyzer(),
                   new FlushDrawOneCardNutAnalyzer(),
                   new FlushDrawTwoCardLowAnalyzer(),
                   new FlushDrawTwoCardHighAnalyzer(),
                   new FlushDrawTwoCardNutAnalyzer(),
                   new FlushDrawNoFlushDrawAnalyzer(),
               };
        }

        public static IAnalyzer[] GetFlushDrawAnalyzers(bool includeBackdoor = false)
        {
            var analyzers = new List<IAnalyzer>
            {
                new StubAnalyzer(),
                new FlushDrawOneCardLowAnalyzer(),
                new FlushDrawOneCardHighAnalyzer(),
                new FlushDrawOneCardNutAnalyzer(),
                new FlushDrawTwoCardLowAnalyzer(),
                new FlushDrawTwoCardHighAnalyzer(),
                new FlushDrawTwoCardNutAnalyzer(),

            };

            if (includeBackdoor)
            {
                analyzers.AddRange(new IAnalyzer[]
                {
                    new FlushDrawBackdoorOneCardAnalyzer(),
                    new FlushDrawBackdoorTwoCardAnalyzer(),
                    new FlushDrawBackdoorTwoCardNutAnalyzer(),
                });
            }

            return analyzers.ToArray();
        }

        public static IAnalyzer[] GetStraightDrawAnalyzers(bool includeBackdoor = false)
        {
            var analyzers = new List<IAnalyzer>
            {
                new StubAnalyzer(),
                new StraightDrawOneCardGutShotAnalyzer(),
                new StraightDrawOneCardDoubleGutShotAnalyzer(),
                new StraightDrawOneCardOpenEndedAnalyzer(),
                new StraightDrawTwoCardGutShotAnalyzer(),
                new StraightDrawTwoCardDoubleGutShotAnalyzer(),
                new StraightDrawTwoCardOpenEndedAnalyzer(),
            };

            if (includeBackdoor)
            {
                analyzers.AddRange(new IAnalyzer[]
                {
                    new StraightDrawTwoCardBackdoorAnalyzer(),
                });
            }

            return analyzers.ToArray();

        }

        public static IAnalyzer[] GetNoDrawAnalyzers()
        {
            return new IAnalyzer[]
                {
                    new StraightDrawNoStraightDrawAnalyzer(),
                    new FlushDrawNoFlushDrawAnalyzer(),
                };

        }

        public static IAnalyzer[] GetDefaultAnalyzers()
        {
            return new IAnalyzer[]
            {
             new StubAnalyzer(),
             new HighCardAnalyzer(),
             new HighCardNoOverCardsAnalyzer(),
             new HighCardOneOvercardAnalyzer(),
             new HighCardTwoOvercardsAnalyzer(),
             new PairedBoardAnalyzer(),
             new PairedBoardNoOvercardsAnalyzer(),
             new PairedBoardOneOvercardAnalyzer(),
             new PairedBoardTwoOvercardsAnalyzer(),
             new PocketPairUnderPairAnalyzer(),
             new PocketPairFourthPairAnalyzer(),
             new PocketPairThirdPairAnalyzer(),
             new PocketPairSecondPairAnalyzer(),
             new PocketPairSecondOrWorseAnalyzer(),
             new PocketPairOverpairAnalyzer(),
             new TopPairAnalyzer(),
             new TopPairWeakKickerAnalyzer(),
             new TopPairDecentKickerAnalyzer(),
             new TopPairTopKickerAnalyzer(),
             new SecondPairAnalyzer(),
             new SecondPairWeakKickerAnalyzer(),
             new SecondPairDecentKickerAnalyzer(),
             new SecondPairTopKickerAnalyzer(),
             new ThirdPairAnalyzer(),
             new ThirdPairWeakKickerAnalyzer(),
             new ThirdPairDecentKickerAnalyzer(),
             new ThirdPairTopKickerAnalyzer(),
             new BottomPairAnalyzer(),
             new BottomPairWeakKickerAnalyzer(),
             new BottomPairDecentKickerAnalyzer(),
             new BottomPairTopKickerAnalyzer(),
             new TwoPairOnBoardAnaluzer(),
             new TwoPairNoTopPairAnalyzer(),
             new TwoPairBottomTwoPairAnalyzer(),
             new TwoPairTopAndBottomPairAnalyzer(),
             new TwoPairTopTwoPairAnalyzer(),
             new TwoPairPairedBoardBottomPairAnalyzer(),
             new TwoPairPairedBoardTopPairAnalyzer(),
             new TwoPairPairedBoardPocketPairSecondPairAnalyzer(),
             new TwoPairPairedBoardPocketPairOverpairAnalyzer(),
             new ThreeOfAKindOnBoardAnalyzer(),
             new ThreeOfAKindBottomSetAnalyzer(),
             new ThreeOfAKindMiddleSetAnalyzer(),
             new ThreeOfAKindSecondSetAnalyzer(),
             new ThreeOfAKindTopSetAnalyzer(),
             new ThreeOfAKindTripsOnFlopAnalyzer(),
             new ThreeOfAKindTripsWeakKickerAnalyzer(),
             new ThreeOfAKindTripsHighKickerAnalyzer(),
             new ThreeOfAKindTripsSecondSetWeakKickerAnalyzer(),
             new ThreeOfAKindTripsSecondSetHighKickerAnalyzer(),
             new ThreeOfAKindTripsTopSetWeakKickerAnalyzer(),
             new ThreeOfAKindTripsTopSetHighKickerAnalyzer(),
             new StraightDrawTwoCardBackdoorAnalyzer(),
             new StraightDrawOneCardGutShotAnalyzer(),
             new StraightDrawOneCardOpenEndedAnalyzer(),
             new StraightDrawTwoCardGutShotAnalyzer(),
             new StraightDrawTwoCardDoubleGutShotAnalyzer(),
             new StraightDrawTwoCardOpenEndedAnalyzer(),
             new StraightDrawNoStraightDrawAnalyzer(),
             new StraightOnBoardAnalyzer(),
             new StraightOneCardAnalyzer(),
             new StraightOneCardLowAnalyzer(),
             new StraightOneCardBottomAnalyzer(),
             new StraightOneCardHighAnalyzer(),
             new StraightOneCardNutAnalyzer(),
             new StraightTwoCardAnalyzer(),
             new StraightTwoCardNutAnalyzer(),
             new StraightTwoCardBottomAnalyzer(),
             new FlushDrawBackdoorOneCardAnalyzer(),
             new FlushDrawBackdoorTwoCardAnalyzer(),
             new FlushDrawBackdoorTwoCardNutAnalyzer(),
             new FlushDrawOneCardLowAnalyzer(),
             new FlushDrawOneCardHighAnalyzer(),
             new FlushDrawOneCardNutAnalyzer(),
             new FlushDrawTwoCardLowAnalyzer(),
             new FlushDrawTwoCardHighAnalyzer(),
             new FlushDrawTwoCardNutAnalyzer(),
             new FlushDrawNoFlushDrawAnalyzer(),
             new FlushLowAnalyzer(),
             new FlushHighAnalyzer(),
             new FlushNutAnalyzer(),
             new FlushOnBoardAnalyzer(),
             new FlushOnBoardLowAnalyzer(),
             new FlushOnBoardHighAnalyzer(),
             new FlushOnBoardNutAnalyzer(),
             new FlushTwoCardLowAnalyzer(),
             new FlushTwoCardHighAnalyzer(),
             new FlushTwoCardNutAnalyzer(),
             new FlushOneCardLowAnalyzer(),
             new FlushOneCardHighAnalyzer(),
             new FlushOneCardNutAnalyzer(),
             new FullHouseOnBoardAnalyzer(),
             new FullHousePocketPairNoTripsOnBoardAnalyzer(),
             new FullHousePocketPairTripsOnBoardAnalyzer(),
             new FullHouseOnePocketCardAnalyzer(),
             new FullHouseOneHoleCardNoTripsOnBoardAnalyzer(),
             new FullHouseTwoPocketCardsNoTripsOnBoardAnalyzer(),
             new FourOfAKindOnBoardAnalyzer(),
             new FourOfAKindNoPocketPairAnalyzer(),
             new FourOfAKindPocketPairAnalyzer(),
             new StraightFlushOnBoardAnalyzer(),
             new StraightFlushOneHoleCardAnalyzer(),
             new StraightFlushTwoPocketCardsAnalyzer(),
             new RoyalFlushOnBoardAnalyzer(),
             new RoyalFlushOneHoleCardAnalyzer(),
             new RoyalFlushTwoPocketCardsAnalyzer(),
            };
        }
        #endregion

    }
}
