using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Model.Enums;
using Model.HandAnalyzers;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    public class ShowdownHandsReportRecord : Indicators
    {
        public ShowdownHands ShowdownHand { get; set; }

        public string ShowdownHandString
        {
            get
            {
                switch (ShowdownHand)
                {
                    case (ShowdownHands.HighCard):
                        return ShowdownHandsResourceStrings.HighCardResourceString;
                        case (ShowdownHands.PocketPairOverpair):
                        return ShowdownHandsResourceStrings.PocketPairOverpairResourceString;
                    case (ShowdownHands.PocketPairSecondOrWorse):
                        return ShowdownHandsResourceStrings.PocketPairSecondOrWorseResourceString;
                    case (ShowdownHands.TopPair):
                        return ShowdownHandsResourceStrings.TopPairResourceString;

                    case (ShowdownHands.TopPairTopKicker):
                        return ShowdownHandsResourceStrings.TopPairTopKickerResourceString;
                    case (ShowdownHands.SecondPair):
                        return ShowdownHandsResourceStrings.SecondPairResourceString;

                    case (ShowdownHands.ThirdPair):
                        return ShowdownHandsResourceStrings.ThirdPairResourceString;
                    case (ShowdownHands.BottomPair):
                        return ShowdownHandsResourceStrings.BottomPairResourceString;
                    case (ShowdownHands.TwoPairNoTopPair):
                        return ShowdownHandsResourceStrings.TwoPairNoTopPairResourceString;
                    case (ShowdownHands.TwoPairTopTwoPair):
                        return ShowdownHandsResourceStrings.TwoPairTopTwoPaiResourceString;
                    case (ShowdownHands.ThreeOfAKindTopSet):
                        return ShowdownHandsResourceStrings.ThreeOfAKindTopSetResourceString;
                    case (ShowdownHands.ThreeOfAKindMiddleSet):
                        return ShowdownHandsResourceStrings.ThreeOfAKindMiddleSetResourceString;
                    case (ShowdownHands.ThreeOfAKindBottomSet):
                        return ShowdownHandsResourceStrings.ThreeOfAKindBottomSetResourceString;
                    case (ShowdownHands.ThreeOfAKindTripsHighKicker):
                        return ShowdownHandsResourceStrings.ThreeOfAKindTripsHighKickerResourceString;
                    case (ShowdownHands.ThreeOfAKindTripsWeakKicker):
                        return ShowdownHandsResourceStrings.ThreeOfAKindTripsWeakKickerResourceString;
                    case (ShowdownHands.ThreeOfAKindTripsOnFlop):
                        return ShowdownHandsResourceStrings.ThreeOfAKindTripsOnFlopResourceString;
                    case (ShowdownHands.StraightOneCard):
                        return ShowdownHandsResourceStrings.StraightOneCardResourceString;
                    case (ShowdownHands.StraightOneCardNut):
                        return ShowdownHandsResourceStrings.StraightOneCardNutResourceString;
                    case (ShowdownHands.StraightOneCardBottom):
                        return ShowdownHandsResourceStrings.StraightOneCardBottomResourceString;
                    case (ShowdownHands.StraightTwoCard):
                        return ShowdownHandsResourceStrings.StraightTwoCardResourceString;
                    case (ShowdownHands.StraightTwoCardNut):
                        return ShowdownHandsResourceStrings.StraightTwoCardNutResourceString;
                    case (ShowdownHands.StraightTwoCardBottom):
                        return ShowdownHandsResourceStrings.StraightTwoCardBottomResourceString;
                    case (ShowdownHands.FlushTwoCardHigh):
                        return ShowdownHandsResourceStrings.FlushTwoCardHighResourceString;
                    case (ShowdownHands.FlushTwoCardNut):
                        return ShowdownHandsResourceStrings.FlushTwoCardNutResourceString;
                    case (ShowdownHands.FlushTwoCardLow):
                        return ShowdownHandsResourceStrings.FlushTwoCardLowResourceString;
                    case (ShowdownHands.FlushOneCardHigh):
                        return ShowdownHandsResourceStrings.FlushOneCardHighResourceString;
                    case (ShowdownHands.FlushOneCardNut):
                        return ShowdownHandsResourceStrings.FlushOneCardNutResourceString;
                    case (ShowdownHands.FlushOneCardLow):
                        return ShowdownHandsResourceStrings.FlushOneCardLowResourceString;
                    case (ShowdownHands.FullHousePocketPairNoTripsOnBoard):
                        return ShowdownHandsResourceStrings.FullHousePocketPairNoTripsOnBoardResourceString;
                    case (ShowdownHands.FullHousePocketPairTripsOnBoard):
                        return ShowdownHandsResourceStrings.FullHousePocketPairTripsOnBoardResourceString;
                    case (ShowdownHands.FullHouseOneHoleCardNoTripsOnBoard):
                        return ShowdownHandsResourceStrings.FullHouseOneHoleCardNoTripsOnBoardResourceString;
                    case (ShowdownHands.FullHouseTwoPocketCardsNoTripsOnBoard):
                        return ShowdownHandsResourceStrings.FullHouseTwoPocketCardNoTripsOnBoardResourceString;
                    case (ShowdownHands.FourOfAKindNoPocketPair):
                        return ShowdownHandsResourceStrings.FourOfAKindNoPocketPairResourceString;
                    case (ShowdownHands.FourOfAKindPocketPair):
                        return ShowdownHandsResourceStrings.FourOfAKindPocketPairResourceString;
                    case (ShowdownHands.StraightFlushOneHoleCard):
                        return ShowdownHandsResourceStrings.StraightFlushOneHoleCardResourceString;
                    case (ShowdownHands.StraightFlushTwoPocketCards):
                        return ShowdownHandsResourceStrings.StraightFlushTwoPocketCardsResourceString;
                    case (ShowdownHands.RoyalFlushOneHoleCard):
                        return ShowdownHandsResourceStrings.RoyalFlushOneHoleCardResourceString;
                    case (ShowdownHands.RoyalFlushTwoPocketCards):
                        return ShowdownHandsResourceStrings.RoyalFlushTwoPocketCardsResourceString;
                }

                return ShowdownHand.ToString();
            }
        }

        public virtual decimal WonHandProc
        {
            get
            {
                if (TotalHands == 0)
                    return 0;

                return (Source.Wonhand / TotalHands) * 100;
            }
        }

        public static IEnumerable<IGrouping<IAnalyzer, Playerstatistic>> FilterHands(IEnumerable<IGrouping<IAnalyzer, Playerstatistic>> hands)
        {
            return hands.Where(x => x.Key.GetRank() != ShowdownHands.None
                            && x.Key.GetRank() != ShowdownHands.PairedBoard
                            && x.Key.GetRank() != ShowdownHands.OnBoardTwoPair
                            && x.Key.GetRank() != ShowdownHands.OnBoardThreeOfAKind
                            && x.Key.GetRank() != ShowdownHands.OnBoardStraight
                            && x.Key.GetRank() != ShowdownHands.FlushOnBoard
                            && x.Key.GetRank() != ShowdownHands.FullHouseOnBoard
                            && x.Key.GetRank() != ShowdownHands.OnBoardFourOfAKind
                            && x.Key.GetRank() != ShowdownHands.OnBoardStraightFlush
                            && x.Key.GetRank() != ShowdownHands.OnBoardRoyalFlush);
        }
    }
}
