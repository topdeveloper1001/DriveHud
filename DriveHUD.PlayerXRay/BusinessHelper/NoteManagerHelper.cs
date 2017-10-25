using DriveHUD.Entities;
using DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer;
using DriveHUD.PlayerXRay.BusinessHelper.OtherAnalyzers;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper
{
    public class NoteManagerHelper
    {
        public static List<PlayerstatisticExtended> HelperHandValueAnalyzer(Func<List<DataTypes.Card>, List<DataTypes.Card>, Street, bool> filter, Street targetStreet, List<PlayerstatisticExtended> incomingPlayerStatistics)
        {
            List<PlayerstatisticExtended> itemsAfterLocalFilter = new List<PlayerstatisticExtended>();
            foreach (var playerstatistic in incomingPlayerStatistics)
            {
                if (!filter(BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(playerstatistic.Playerstatistic.Cards),
                            BoardTextureAnalyzerHelpers.ParseStringSequenceOfCards(playerstatistic.Playerstatistic.Board),
                            targetStreet))
                    continue;

                itemsAfterLocalFilter.Add(playerstatistic);
            }
            return itemsAfterLocalFilter;
        }

        public static List<PlayerstatisticExtended> HandValueFilterHelper(List<PlayerstatisticExtended> incomingPlayerStatistics, HandValueEnum handValueEnum, Street targetStreet)
        {
            List<PlayerstatisticExtended> fileteredList = incomingPlayerStatistics;
            switch (handValueEnum)
            {

                case HandValueEnum.HighCardTwoOvercards:
                    fileteredList = HelperHandValueAnalyzer(new HighCardAnalyzer.HighCardTwoOvercardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.HighCardOneOvercard:
                    fileteredList = HelperHandValueAnalyzer(new HighCardAnalyzer.HighCardOneOvercardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.HighCardNoOvercards:
                    fileteredList = HelperHandValueAnalyzer(new HighCardAnalyzer.HighCardNoOvercardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPocketPairOverpair:
                    fileteredList = HelperHandValueAnalyzer(new PocketPairOverpairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPocketPairSecondPair:
                    fileteredList = HelperHandValueAnalyzer(new PocketPairSecondPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPocketPairLowPair:
                    fileteredList = HelperHandValueAnalyzer(new PocketPairLowPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairTopPairTopKicker:
                    fileteredList = HelperHandValueAnalyzer(new TopPairTopKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairTopPairGoodKicker:
                    fileteredList = HelperHandValueAnalyzer(new TopPairGoodKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairTopPairWeakKicker:
                    fileteredList = HelperHandValueAnalyzer(new TopPairWeakKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairSecondPairAceKicker:
                    fileteredList = HelperHandValueAnalyzer(new SecondPairAceKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairSecondPairNonAceKicker:
                    fileteredList = HelperHandValueAnalyzer(new SecondPairNonAceKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairBottomPairAceKicker:
                    fileteredList = HelperHandValueAnalyzer(new BottomPairAnalyzer.BottomPairAceKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairBottomPairNonAceKicker:
                    fileteredList = HelperHandValueAnalyzer(new BottomPairAnalyzer.BottomPairNonAceKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPairedBoardTwoOvercards:
                    fileteredList = HelperHandValueAnalyzer(new PairedBoardTwoOvercardsAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPairedBoardOneOvercard:
                    fileteredList = HelperHandValueAnalyzer(new PairedBoardOneOvercardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.OnePairPairedBoardNoOvercards:
                    fileteredList = HelperHandValueAnalyzer(new PairedBoardNoOvercardsAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairBothCardsPairedTopTwoPair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairTopTwoPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairBothCardsPairedTopPairPlusPair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairTopPairPlusPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairBothCardsPairedMiddlePlusBottom:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairBottomPairPlusMiddlePairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPairPlusPairedBoardTopPair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPairedBoardTopPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPairPlusPairedBoardSecondPair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPairedBoardSecondPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPairPlusPairedBoardBottomPair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPairedBoardBottomPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPocketPairPlusPairedBoardOverpair:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPocketPairPlusPairedBoardOverPair().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPocketPairPlusPairedBoardGreaterPairOnBoard:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPocketPairPlusPairedBoardGreaterPairOnBoard().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPairPocketPairPlusPairedBoardLowerPairOnBoard:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairPocketPairPlusPairedBoardLowerPairOnBoard().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TwoPaironBoard:
                    fileteredList = HelperHandValueAnalyzer(new TwoPairOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindSetHighSet:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTopSetAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindSetSecondSet:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindSecondSetAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindSetLowSet:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindLowSetAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsHighTripsHighKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsTopSetHighKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsHighTripsLowKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsTopSetWeakKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsSecondTripsHighKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsSecondSetHighKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsSecondTripsLowKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsSecondSetWeakKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsLowTripsHighKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsLowSetHighKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.ThreeofaKindTripsLowTripsLowKicker:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindTripsLowSetWeakKickerAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.TripsOnBoard:
                    fileteredList = HelperHandValueAnalyzer(new ThreeOfAKindOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightTwoCardNutStraight:
                    fileteredList = HelperHandValueAnalyzer(new StraightTwoCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightTwoCardStraight:
                    fileteredList = HelperHandValueAnalyzer(new StraightTwoCardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightOneCardNutStraight:
                    fileteredList = HelperHandValueAnalyzer(new StraightOneCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightOneCardStraight:
                    fileteredList = HelperHandValueAnalyzer(new StraightOneCardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightStraightOnBoard:
                    fileteredList = HelperHandValueAnalyzer(new StraightOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush3FlushCardsNutFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushTwoCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush3FlushCardsHighFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushTwoCardHighAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush3FlushCardsLowFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushTwoCardLowAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush4FlushCardsNutFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOneCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush4FlushCardsHighFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOneCardHighAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.Flush4FlushCardsLowFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOneCardLowAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FlushOnBoardNutFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOnBoardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FlushOnBoardHighFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOnBoardHighAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FlushOnBoardLowFlush:
                    fileteredList = HelperHandValueAnalyzer(new FlushOnBoardLowAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouse2CardsPocketPairnoTripsonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FullHousePocketPairNoTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouse2CardsPocketPairPlusTripsonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FullHousePocketPairTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouse2CardsNoPocketsnoTripsonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FullHouseNoPocketPairNoTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouseLess2CardsTripsonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FullHouseOneHoleCardTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouseLess2CardsFillTopPairnoTrips:
                    fileteredList = HelperHandValueAnalyzer(new FullHouseOneHoleCardTopPairNoTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouseLess2CardsFillBottomPairnoTrips:
                    fileteredList = HelperHandValueAnalyzer(new FullHouseOneHoleCardLowPairNoTripsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FullHouseonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FullHouseNoHoleCardsOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FourofaKindWithPocketPair:
                    fileteredList = HelperHandValueAnalyzer(new FourOfAKindPocketPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FourofaKindWithoutPocketPair:
                    fileteredList = HelperHandValueAnalyzer(new FourOfAKindNoPocketPairAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.FourofaKindonBoard:
                    fileteredList = HelperHandValueAnalyzer(new FourOfAKindOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightFlush2Cards:
                    fileteredList = HelperHandValueAnalyzer(new StraightFlushTwoPocketCardsAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueEnum.StraightFlushonBoard:
                    fileteredList = HelperHandValueAnalyzer(new StraightFlushOnBoardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                default:
                    return new List<PlayerstatisticExtended>();
            }
            return fileteredList;
        }

        internal static List<PlayerstatisticExtended> HandValueFilterFlushDrawHelper(List<PlayerstatisticExtended> incomingPlayerStatistics, HandValueFlushDrawEnum handValueFlushDrawEnum, Street targetStreet)
        {
            List<PlayerstatisticExtended> fileteredList = incomingPlayerStatistics;
            switch (handValueFlushDrawEnum)
            {
                case HandValueFlushDrawEnum.TwoCardNutFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawTwoCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.TwoCardHighFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawTwoCardHighAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.TwoCardLowFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawTwoCardLowAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.OneCardNutFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawOneCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.OneCardHighFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawOneCardHighAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.OneCardLowFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawOneCardLowAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.TwoCardNutBackdoorFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawBackdoorTwoCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.TwoCardBackdoorFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawBackdoorTwoCardAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.OneCardNutBackdoorFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawBackdoorOneCardNutAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueFlushDrawEnum.NoFlushDraw:
                    fileteredList = HelperHandValueAnalyzer(new FlushDrawNoFlushDrawAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                default:
                    return new List<PlayerstatisticExtended>();
            }

            return fileteredList;
        }

        public static List<PlayerstatisticExtended> HandValueFilterStraightDrawHelper(List<PlayerstatisticExtended> incomingPlayerStatistics, HandValueStraightDraw handValueStraightDrawEnum, Street targetStreet)
        {
            List<PlayerstatisticExtended> fileteredList = incomingPlayerStatistics;

            switch (handValueStraightDrawEnum)
            {
                case HandValueStraightDraw.TwoCardOpenEndedStraightDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawTwoCardOpenEndedAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueStraightDraw.TwoCardGutshotDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawTwoCardGutShotAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueStraightDraw.OneCardOpenEndedStraightDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawOneCardOpenEndedAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueStraightDraw.OneCardGutshotDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawOneCardGutShotAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueStraightDraw.TwoCardBackdoorStraightDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawTwoCardBackdoorAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                case HandValueStraightDraw.NoStraightDraw:
                    fileteredList = HelperHandValueAnalyzer(new StraightDrawNoStraightDrawAnalyzer().Analyze, targetStreet, fileteredList);
                    break;
                default:
                    return new List<PlayerstatisticExtended>();
            }

            return fileteredList;
        }

        public static List<PlayerstatisticExtended> FilterByASelectedFilter(List<PlayerstatisticExtended> incomingPlayerstatistics, FilterObject filter)
        {
            List<PlayerstatisticExtended> filteredList = new List<PlayerstatisticExtended>();

            FilterEnum filterEnum = (FilterEnum)filter.Tag;


            switch (filterEnum)
            {
                //Preflop   
                case FilterEnum.VPIP:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Vpiphands > 0).ToList();
                case FilterEnum.PutMoneyinPot:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Vpiphands > 0 || x.Playerstatistic.Position == EnumPosition.BB || x.Playerstatistic.Position == EnumPosition.SB).ToList();
                case FilterEnum.PFR:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Pfrhands > 0).ToList();
                case FilterEnum.PFRFalse:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Pfrhands == 0).ToList();
                case FilterEnum.Did3Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Didthreebet > 0).ToList();
                case FilterEnum.DidSqueeze:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Didsqueeze > 0).ToList();
                case FilterEnum.DidColdCall:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Didcoldcall > 0).ToList();
                case FilterEnum.CouldColdCall:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Couldcoldcall > 0).ToList();
                case FilterEnum.CouldColdCallFalse:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Couldcoldcall == 0).ToList();
                case FilterEnum.FacedPreflop3Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facedthreebetpreflop > 0).ToList();
                case FilterEnum.FoldedToPreflop3Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facedthreebetpreflop > 0 && x.Playerstatistic.Calledthreebetpreflop == 0 && x.Playerstatistic.Raisedthreebetpreflop == 0).ToList();
                case FilterEnum.CalledPreflop3Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Calledthreebetpreflop > 0).ToList();
                case FilterEnum.RaisedPreflop3Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Raisedthreebetpreflop > 0).ToList();
                case FilterEnum.FacedPreflop4Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facedfourbetpreflop > 0).ToList();
                case FilterEnum.FoldedToPreflop4Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facedfourbetpreflop > 0 && x.Playerstatistic.Calledfourbetpreflop == 0 && x.Playerstatistic.Raisedfourbetpreflop == 0).ToList();
                case FilterEnum.CalledPreflop4Bbet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Calledfourbetpreflop > 0).ToList();
                case FilterEnum.RaisedPreflop4Bet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Raisedfourbetpreflop > 0).ToList();
                case FilterEnum.InBBandStealAttempted:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Bigblindstealfaced > 0).ToList();
                case FilterEnum.InBBandStealDefended:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Bigblindstealdefended > 0).ToList();
                case FilterEnum.InBBandStealReraised:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Bigblindstealreraised > 0).ToList();
                case FilterEnum.InSBandStealAttempted:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Smallblindstealattempted > 0).ToList();
                case FilterEnum.InSBandStealDefended:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Smallblindstealdefended > 0).ToList();
                case FilterEnum.InSBandStealReraised:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Smallblindstealreraised > 0).ToList();
                case FilterEnum.LimpReraised:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.LimpReraised > 0).ToList();


                case FilterEnum.BBsBetPreflopisBiggerThan:      //check if it should be called bbRaise.. instead of the bbBet..
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                  Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                       .FirstOrDefault(y => y.HandActionType == HandActionType.RAISE)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsBetPreflopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                   Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                        .FirstOrDefault(y => y.HandActionType == HandActionType.RAISE)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledPreflopisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop)
                                                                                                                     .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                                     .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                                     .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledPreflopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop)
                                                                                                 .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                 .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                 .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutInPreflopisBiggerThan:   //todo check this kind of stat for correct calculation
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop &&
                                                                  y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutInPreflopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Preflop &&
                                                             y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();


                case FilterEnum.PreflopRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Preflop) > (decimal)filter.Value).ToList();
                case FilterEnum.PreflopRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Preflop) < (decimal)filter.Value).ToList();

                case FilterEnum.PreflopFacingRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Preflop) > (decimal)filter.Value).ToList();
                case FilterEnum.PreflopFacingRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Preflop) < (decimal)filter.Value).ToList();

                case FilterEnum.AllinPreflop:
                    return incomingPlayerstatistics.Where(x => x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null &&
                                                               x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName != x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null).ToList();

                //Flop
                case FilterEnum.SawFlop:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0).ToList();
                case FilterEnum.SawFlopFalse:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop == 0).ToList();
                case FilterEnum.LasttoActionFlop:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && LastToActOnStreetTrue(x, Street.Flop)).ToList();
                case FilterEnum.LasttoActionFlopFalse:
                    return incomingPlayerstatistics.Where(x => LastToActOnFlopFalse(x, Street.Flop)).ToList();
                case FilterEnum.FlopUnopened:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                    .All(y => y.HandActionType != HandActionType.BET &&
                                                                                              y.HandActionType != HandActionType.RAISE &&
                                                                                              y.HandActionType != HandActionType.ALL_IN)).ToList();


                case FilterEnum.PlayersonFlopisBiggerThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                             .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                             .Count() > filter.Value).ToList();
                case FilterEnum.PlayersonFlopisLessThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() < filter.Value).ToList();
                case FilterEnum.PlayersonFlopisEqualTo:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() == filter.Value).ToList();


                case FilterEnum.FlopContinuationBetPossible:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Flopcontinuationbetpossible > 0).ToList();
                case FilterEnum.FlopContinuationBetMade:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Flopcontinuationbetmade > 0).ToList();
                case FilterEnum.FacingFlopContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingflopcontinuationbet > 0).ToList();
                case FilterEnum.FoldedtoFlopContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingflopcontinuationbet > 0 && x.Playerstatistic.Calledflopcontinuationbet == 0 && x.Playerstatistic.Raisedflopcontinuationbet == 0).ToList();
                case FilterEnum.CalledFlopContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Calledflopcontinuationbet > 0).ToList();
                case FilterEnum.RaisedFlopContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Raisedflopcontinuationbet > 0).ToList();

                case FilterEnum.FlopBet:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.BET)).ToList();
                case FilterEnum.FlopBetFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.BET, HandActionType.FOLD)).ToList();
                case FilterEnum.FlopBetCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.BET, HandActionType.CALL)).ToList();
                case FilterEnum.FlopBetRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.BET, HandActionType.RAISE)).ToList();
                case FilterEnum.FlopRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.RAISE)).ToList();
                case FilterEnum.FlopRaiseFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.RAISE, HandActionType.FOLD)).ToList();
                case FilterEnum.FlopRaiseCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.RAISE, HandActionType.CALL)).ToList();
                case FilterEnum.FlopRaiseRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.RAISE, HandActionType.RAISE)).ToList();
                case FilterEnum.FlopCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CALL)).ToList();
                case FilterEnum.FlopCallFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CALL, HandActionType.FOLD)).ToList();
                case FilterEnum.FlopCallCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CALL, HandActionType.CALL)).ToList();
                case FilterEnum.FlopCallRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CALL, HandActionType.RAISE)).ToList();
                case FilterEnum.FlopCheck:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CHECK)).ToList();
                case FilterEnum.FlopCheckCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CHECK, HandActionType.CALL)).ToList();
                case FilterEnum.FlopCheckFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CHECK, HandActionType.FOLD)).ToList();
                case FilterEnum.FlopCheckRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.CHECK, HandActionType.RAISE)).ToList();
                case FilterEnum.FlopFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Flop, HandActionType.FOLD)).ToList();

                case FilterEnum.FlopWasCheckRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasCheckAndRaiseAnalyzer(x, Street.Flop)).ToList();
                case FilterEnum.FlopWasBetInto:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasBetIntoAnalyzer(x, Street.Flop)).ToList();
                case FilterEnum.FlopWasRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasRaisedAnalyzer(x, Street.Flop)).ToList();

                case FilterEnum.BBsBetFlopisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                      Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                           .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsBetFlopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                     Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                          .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledFlopisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                                 .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                 .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                 .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledFlopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop)
                                                                                                 .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                 .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                 .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutinFlopisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutinFlopisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Flop &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();

                case FilterEnum.FlopPotSizeinBBsisBiggerThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisBiggerThan(incomingPlayerstatistics, (decimal)filter.Value, Street.Flop) : new List<PlayerstatisticExtended>();
                case FilterEnum.FlopPotSizeinBBsisLessThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisLessThan(incomingPlayerstatistics, (decimal)filter.Value, Street.Flop) : new List<PlayerstatisticExtended>();

                case FilterEnum.FlopStackPotRatioisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.Flop) > (decimal)filter.Value).ToList();
                case FilterEnum.FlopStackPotRatioisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.Flop) < (decimal)filter.Value).ToList();

                case FilterEnum.FlopBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                      Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                           .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value).ToList();
                case FilterEnum.FlopBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                                          Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                                               .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value).ToList();

                case FilterEnum.FlopRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Flop) > (decimal)filter.Value).ToList();
                case FilterEnum.FlopRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Flop) < (decimal)filter.Value).ToList();

                case FilterEnum.FlopFacingBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.Flop) > (decimal)filter.Value).ToList();
                case FilterEnum.FlopFacingBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.Flop) < (decimal)filter.Value).ToList();

                case FilterEnum.FlopFacingRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Flop) > (decimal)filter.Value).ToList();
                case FilterEnum.FlopFacingRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Flop) < (decimal)filter.Value).ToList();


                case FilterEnum.AllinOnFlop:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).Any(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName)).ToList();
                case FilterEnum.AllinOnFlopOrEarlier:
                    return incomingPlayerstatistics.Where(x => (x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null &&
                                                                x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName != x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null) ||
                                                               (x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null
                                                                              && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Flop && y.PlayerName != x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null)).ToList();




                //Turn
                case FilterEnum.SawTurn:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0).ToList();
                case FilterEnum.LasttoActonTurn:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                                     .Select(y => y.PlayerName)
                                                                                                     .Distinct().LastOrDefault() == x.Playerstatistic.PlayerName).ToList();
                case FilterEnum.LasttoActonTurnFalse:
                    return incomingPlayerstatistics.Where(x => LastToActOnFlopFalse(x, Street.Turn)).ToList();
                case FilterEnum.TurnUnopened:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                    .All(y => y.HandActionType != HandActionType.BET &&
                                                                                              y.HandActionType != HandActionType.RAISE &&
                                                                                              y.HandActionType != HandActionType.ALL_IN &&
                                                                                              y.HandActionType != HandActionType.UNCALLED_BET)).ToList();
                case FilterEnum.TurnUnopenedFalse:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                    .Any(y => y.HandActionType != HandActionType.BET ||
                                                                                              y.HandActionType != HandActionType.RAISE ||
                                                                                              y.HandActionType != HandActionType.ALL_IN ||
                                                                                              y.HandActionType != HandActionType.UNCALLED_BET)).ToList();
                case FilterEnum.PlayersonTurnisBiggerThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                   .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                   .Count() > filter.Value).ToList();
                case FilterEnum.PlayersonTurnisLessThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() < filter.Value).ToList();
                case FilterEnum.PlayersonTurnisEqualTo:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() == filter.Value).ToList();
                case FilterEnum.TurnContinuationBetPossible:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Turncontinuationbetpossible > 0).ToList();
                case FilterEnum.TurnContinuationBetMade:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Turncontinuationbetmade > 0).ToList();
                case FilterEnum.FacingTurnContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingturncontinuationbet > 0).ToList();
                case FilterEnum.FoldedtoTurnContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingturncontinuationbet > 0 && x.Playerstatistic.Calledturncontinuationbet == 0 && x.Playerstatistic.Raisedturncontinuationbet == 0).ToList();
                case FilterEnum.CalledTurnContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Calledturncontinuationbet > 0).ToList();
                case FilterEnum.RaisedTurnContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Raisedturncontinuationbet > 0).ToList();

                case FilterEnum.TurnBet:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.BET)).ToList();
                case FilterEnum.TurnBetFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.BET, HandActionType.FOLD)).ToList();
                case FilterEnum.TurnBetCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.BET, HandActionType.CALL)).ToList();
                case FilterEnum.TurnBetRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.BET, HandActionType.RAISE)).ToList();
                case FilterEnum.TurnRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.RAISE)).ToList();
                case FilterEnum.TurnRaiseFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.RAISE, HandActionType.FOLD)).ToList();
                case FilterEnum.TurnRaiseCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.RAISE, HandActionType.CALL)).ToList();
                case FilterEnum.TurnRaiseRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.RAISE, HandActionType.RAISE)).ToList();
                case FilterEnum.TurnCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CALL)).ToList();
                case FilterEnum.TurnCallFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CALL, HandActionType.FOLD)).ToList();
                case FilterEnum.TurnCallCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CALL, HandActionType.CALL)).ToList();
                case FilterEnum.TurnCallRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CALL, HandActionType.RAISE)).ToList();
                case FilterEnum.TurnCheck:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CHECK)).ToList();
                case FilterEnum.TurnCheckFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CHECK, HandActionType.FOLD)).ToList();
                case FilterEnum.TurnCheckCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CHECK, HandActionType.CALL)).ToList();
                case FilterEnum.TurnCheckRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.CHECK, HandActionType.RAISE)).ToList();
                case FilterEnum.TurnFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.Turn, HandActionType.FOLD)).ToList();

                case FilterEnum.TurnWasCheckRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasCheckAndRaiseAnalyzer(x, Street.Turn)).ToList();
                case FilterEnum.TurnWasBetInto:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasBetIntoAnalyzer(x, Street.Turn)).ToList();
                case FilterEnum.TurnWasRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasRaisedAnalyzer(x, Street.Turn)).ToList();

                case FilterEnum.BBsBetTurnisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                     Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                          .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsBetTurnisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                     Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                          .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledTurnisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                                  .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                  .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                  .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsCalledTurnisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn)
                                                                                                 .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                 .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                 .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutinTurnisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutinTurnisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.Turn &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();

                case FilterEnum.TurnPotSizeinBBsisBiggerThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisBiggerThan(incomingPlayerstatistics, (decimal)filter.Value, Street.Turn) : new List<PlayerstatisticExtended>();
                case FilterEnum.TurnPotSizeinBBsisLessThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisLessThan(incomingPlayerstatistics, (decimal)filter.Value, Street.Turn) : new List<PlayerstatisticExtended>();
                case FilterEnum.TurnStackPotRatioisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.Turn) > (decimal)filter.Value).ToList();
                case FilterEnum.TurnStackPotRatioisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.Turn) < (decimal)filter.Value).ToList();

                case FilterEnum.TurnBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                  Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName)
                                                       .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value).ToList();
                case FilterEnum.TurnBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                  Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName)
                                                       .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value).ToList();

                case FilterEnum.TurnRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Turn) > (decimal)filter.Value).ToList();
                case FilterEnum.TurnRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.Turn) < (decimal)filter.Value).ToList();

                case FilterEnum.TurnFacingBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.Turn) > (decimal)filter.Value).ToList();
                case FilterEnum.TurnFacingBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.Turn) < (decimal)filter.Value).ToList();

                case FilterEnum.TurnFacingRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Turn) > (decimal)filter.Value).ToList();
                case FilterEnum.TurnFacingRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.Turn) < (decimal)filter.Value).ToList();

                case FilterEnum.AllinonTurn:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).Any(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName)).ToList();
                case FilterEnum.AllinonTurnOrEarlier:
                    return incomingPlayerstatistics.Where(x => (x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null &&
                                                                x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Preflop && y.PlayerName != x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null) ||
                                                               (x.Playerstatistic.Sawflop > 0 && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Flop && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null
                                                                              && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Flop && y.PlayerName != x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null) ||
                                                               (x.Playerstatistic.SawTurn > 0 && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null
                                                                              && x.HandHistory.HandActions.SkipWhile(y => !y.IsAllIn).FirstOrDefault(y => y.Street == Street.Turn && y.PlayerName == x.Playerstatistic.PlayerName && y.HandActionType != HandActionType.FOLD) != null)).ToList();

                //River
                case FilterEnum.SawRiver:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0).ToList();
                case FilterEnum.LasttoActonRiver:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                                     .Select(y => y.PlayerName)
                                                                                                     .Distinct().LastOrDefault() == x.Playerstatistic.PlayerName).ToList();
                case FilterEnum.LasttoActonRiverFalse:
                    return incomingPlayerstatistics.Where(x => LastToActOnFlopFalse(x, Street.River)).ToList();
                case FilterEnum.RiverUnopened:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                    .All(y => y.HandActionType != HandActionType.BET &&
                                                                                              y.HandActionType != HandActionType.RAISE &&
                                                                                              y.HandActionType != HandActionType.ALL_IN &&
                                                                                              y.HandActionType != HandActionType.UNCALLED_BET)).ToList();
                case FilterEnum.RiverUnopenedFalse:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                    .Any(y => y.HandActionType != HandActionType.BET ||
                                                                                              y.HandActionType != HandActionType.RAISE ||
                                                                                              y.HandActionType != HandActionType.ALL_IN ||
                                                                                              y.HandActionType != HandActionType.UNCALLED_BET)).ToList();
                case FilterEnum.PlayersonRiverisBiggerThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                             .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                             .Count() > filter.Value).ToList();
                case FilterEnum.PlayersonRiverisLessThan:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() < filter.Value).ToList();
                case FilterEnum.PlayersonRiverisEqualTo:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.SawRiver > 0 && x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                    .GroupBy(y => y.PlayerName).Select(y => y.First())
                                                                                    .Count() == filter.Value).ToList();

                case FilterEnum.RiverContinuationBetPossible:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Rivercontinuationbetpossible > 0).ToList();
                case FilterEnum.RiverContinuationBetMade:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Rivercontinuationbetmade > 0).ToList();
                case FilterEnum.FacingRiverContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingrivercontinuationbet > 0).ToList();
                case FilterEnum.FoldedtoRiverContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Facingrivercontinuationbet > 0 && x.Playerstatistic.Calledrivercontinuationbet == 0 && x.Playerstatistic.Raisedrivercontinuationbet == 0).ToList();
                case FilterEnum.CalledRiverContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Calledrivercontinuationbet > 0).ToList();
                case FilterEnum.RaisedRiverContinuationBet:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Raisedrivercontinuationbet > 0).ToList();

                case FilterEnum.RiverBet:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.BET)).ToList();
                case FilterEnum.RiverBetFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.BET, HandActionType.FOLD)).ToList();
                case FilterEnum.RiverBetCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.BET, HandActionType.CALL)).ToList();
                case FilterEnum.RiverBetRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.BET, HandActionType.RAISE)).ToList();
                case FilterEnum.RiverRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.RAISE)).ToList();
                case FilterEnum.RiverRaiseFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.RAISE, HandActionType.FOLD)).ToList();
                case FilterEnum.RiverRaiseCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.RAISE, HandActionType.CALL)).ToList();
                case FilterEnum.RiverRaiseRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.RAISE, HandActionType.RAISE)).ToList();
                case FilterEnum.RiverCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CALL)).ToList();
                case FilterEnum.RiverCallFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CALL, HandActionType.FOLD)).ToList();
                case FilterEnum.RiverCallCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CALL, HandActionType.CALL)).ToList();
                case FilterEnum.RiverCallRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CALL, HandActionType.RAISE)).ToList();
                case FilterEnum.RiverCheck:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CHECK)).ToList();
                case FilterEnum.RiverCheckCall:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CHECK, HandActionType.CALL)).ToList();
                case FilterEnum.RiverCheckFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CHECK, HandActionType.FOLD)).ToList();
                case FilterEnum.RiverCheckRaise:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.CHECK, HandActionType.RAISE)).ToList();
                case FilterEnum.RiverFold:
                    return incomingPlayerstatistics.Where(x => PlayerStreetAction(x, Street.River, HandActionType.FOLD)).ToList();

                case FilterEnum.RiverWasCheckRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasCheckAndRaiseAnalyzer(x, Street.River)).ToList();
                case FilterEnum.RiverWasBetInto:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasBetIntoAnalyzer(x, Street.River)).ToList();
                case FilterEnum.RiverWasRaised:
                    return incomingPlayerstatistics.Where(x => NotClassifiedAnalyzers.WasRaisedAnalyzer(x, Street.River)).ToList();

                case FilterEnum.BBsBetRiverisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                     Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                          .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value).ToList();
                case FilterEnum.BBsBetRiverisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                     Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River && y.PlayerName == x.Playerstatistic.PlayerName)
                                                                          .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value).ToList();
                case FilterEnum.BBsCalledRiverisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                                 .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                 .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                 .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) > (decimal)filter.Value).ToList();
                case FilterEnum.BBsCalledRiverisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River)
                                                                                                  .SkipWhile(y => y.HandActionType != HandActionType.CALL)
                                                                                                  .TakeWhile(y => y.HandActionType == HandActionType.CALL)
                                                                                                  .FirstOrDefault(y => y.PlayerName == x.Playerstatistic.PlayerName)?.Amount ?? 0) < (decimal)filter.Value).ToList();
                case FilterEnum.BBsPutinRiverisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.River &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.BBsPutinRiverisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.HandHistory.HandActions.Where(y => y.Street == Street.River &&
                                                            y.PlayerName == x.Playerstatistic.PlayerName).Sum(y => Math.Abs(y.Amount)) < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();

                case FilterEnum.RiverPotSizeinBBsisBiggerThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisBiggerThan(incomingPlayerstatistics, (decimal)filter.Value, Street.River) : new List<PlayerstatisticExtended>();
                case FilterEnum.RiverPotSizeinBBsisLessThan:
                    return filter.Value != null ? PotSizeAnalyzers.BBPotSizeisLessThan(incomingPlayerstatistics, (decimal)filter.Value, Street.River) : new List<PlayerstatisticExtended>();
                case FilterEnum.RiverStackPotRatioisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.River) > (decimal)filter.Value).ToList();
                case FilterEnum.RiverStackPotRatioisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && PotSizeAnalyzers.StackPotRatio(x, Street.River) < (decimal)filter.Value).ToList();

                case FilterEnum.RiverBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                  Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River && y.PlayerName == x.Playerstatistic.PlayerName)
                                                       .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) > (decimal)filter.Value).ToList();
                case FilterEnum.RiverBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                  Math.Abs(x.HandHistory.HandActions.Where(y => y.Street == Street.River && y.PlayerName == x.Playerstatistic.PlayerName)
                                                       .FirstOrDefault(y => y.HandActionType == HandActionType.BET)?.Amount ?? 0) < (decimal)filter.Value).ToList();

                case FilterEnum.RiverRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.River) > (decimal)filter.Value).ToList();
                case FilterEnum.RiverRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetAndDidRaiseSizePot(x, Street.River) < (decimal)filter.Value).ToList();

                case FilterEnum.RiverFacingBetSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.River) > (decimal)filter.Value).ToList();
                case FilterEnum.RiverFacingBetSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingBetSizePot(x, Street.River) < (decimal)filter.Value).ToList();


                case FilterEnum.RiverFacingRaiseSizePotisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.River) > (decimal)filter.Value).ToList();
                case FilterEnum.RiverFacingRaiseSizePotisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && BetSizeAnalyzers.GetFacingRaiseSizePot(x, Street.River) < (decimal)filter.Value).ToList();



                //Other
                case FilterEnum.SawShowdown:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Sawshowdown > 0).ToList();
                case FilterEnum.WonHand:
                    return incomingPlayerstatistics.Where(x => x.Playerstatistic.Wonhand > 0).ToList();
                case FilterEnum.FinalPotSizeinBBsisBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.Playerstatistic.Pot > (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.FinalPotSizeinBBsisLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.Playerstatistic.Pot < (decimal)filter.Value * x.Playerstatistic.BigBlind).ToList();
                case FilterEnum.PlayerWonBBsIsBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                               x.Playerstatistic.Totalamountwonincents > 0 &&
                                                               x.Playerstatistic.Totalamountwonincents > (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayerWonBBsIsLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                               x.Playerstatistic.Totalamountwonincents > 0 &&
                                                               x.Playerstatistic.Totalamountwonincents < (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayerLostBBsIsBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                               x.Playerstatistic.Totalamountwonincents < 0 &&
                                                               Math.Abs(x.Playerstatistic.Totalamountwonincents) > (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayerLostBBsIsLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                               x.Playerstatistic.Totalamountwonincents < 0 &&
                                                               Math.Abs(x.Playerstatistic.Totalamountwonincents) < (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayerWonOrLostBBsIsBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                                Math.Abs(x.Playerstatistic.Totalamountwonincents) > (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayerWonOrLostBBsIsLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null &&
                                                                Math.Abs(x.Playerstatistic.Totalamountwonincents) < (decimal)filter.Value * x.Playerstatistic.BigBlind * 100).ToList();
                case FilterEnum.PlayersSawShowdownIsBiggerThan:
                    return incomingPlayerstatistics.Where(x => x.HandHistory.HandActions.Count(y => y.Street == Street.Showdown) > filter.Value).ToList();
                case FilterEnum.PlayersSawShowdownIsLessThan:
                    return incomingPlayerstatistics.Where(x => x.HandHistory.HandActions.Count(y => y.Street == Street.Showdown) < filter.Value).ToList();
                case FilterEnum.PlayersSawShowdownIsEqualTo:
                    return incomingPlayerstatistics.Where(x => x.HandHistory.HandActions.Count(y => y.Street == Street.Showdown) == filter.Value).ToList();
                case FilterEnum.AllinWinIsBiggerThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.Playerstatistic.Equity > (decimal)filter.Value).ToList();
                case FilterEnum.AllinWinIsLessThan:
                    return incomingPlayerstatistics.Where(x => filter.Value != null && x.Playerstatistic.Equity < (decimal)filter.Value).ToList();
            }

            return filteredList;
        }

        private static bool LastToActOnStreetTrue(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            return playerstatistic.HandHistory.HandActions.Where(y => y.Street == targetStreet)
                .Select(y => y.PlayerName)
                .Distinct().LastOrDefault() == playerstatistic.Playerstatistic.PlayerName;
        }

        private static bool LastToActOnFlopFalse(PlayerstatisticExtended playerstatistic, Street targetStreet)
        {
            List<string> playerListPreflop = new List<string>();
            List<string> playerListAllinPreflop = new List<string>();
            List<string> playerListAllinFlop = new List<string>();
            List<string> playerListAllinTurn = new List<string>();
            switch (targetStreet)
            {
                case Street.Flop:

                    playerListPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();


                    if ((playerstatistic.Playerstatistic.Sawflop > 0 && !LastToActOnStreetTrue(playerstatistic, targetStreet)) ||
                       (playerListAllinPreflop.Count > 0 && playerListAllinPreflop.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinPreflop.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName))
                        return true;
                    break;
                case Street.Turn:
                    playerListPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinFlop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Flop).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();


                    if ((playerstatistic.Playerstatistic.SawTurn > 0 && !LastToActOnStreetTrue(playerstatistic, targetStreet)) ||
                       (playerListAllinPreflop.Count > 0 && playerListAllinPreflop.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinPreflop.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName) ||
                       (playerListAllinFlop.Count > 0 && playerListAllinFlop.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinFlop.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName))
                        return true;
                    break;
                case Street.River:
                    playerListPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinPreflop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Preflop).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinFlop = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Flop).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();
                    playerListAllinTurn = playerstatistic.HandHistory.HandActions.Where(y => y.Street == Street.Turn).SkipWhile(y => !y.IsAllIn).Select(y => y.PlayerName).Distinct().ToList();

                    if ((playerstatistic.Playerstatistic.SawRiver > 0 && !LastToActOnStreetTrue(playerstatistic, targetStreet)) ||
                       (playerListAllinPreflop.Count > 0 && playerListAllinPreflop.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinPreflop.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName) ||
                       (playerListAllinFlop.Count > 0 && playerListAllinFlop.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinFlop.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName) ||
                       (playerListAllinTurn.Count > 0 && playerListAllinTurn.Any(x => x == playerstatistic.Playerstatistic.PlayerName) && playerListPreflop.LastOrDefault(x => playerListAllinTurn.Any(y => y == x)) != playerstatistic.Playerstatistic.PlayerName))
                        return true;
                    break;
            }

            return false;
        }

        private static bool PlayerStreetAction(PlayerstatisticExtended playerstatistic, Street targetStreet, HandActionType firstActionType, HandActionType secondActionType = HandActionType.UNKNOWN)
        {
            //actions did by hero
            List<HandAction> consideredActions = playerstatistic.HandHistory.HandActions.Where(x => x.Street == targetStreet && x.PlayerName == playerstatistic.Playerstatistic.PlayerName).ToList();

            if (!consideredActions.Any())
                return false;
            //taking all actions after firstActionType match 
            List<HandAction> candidatePlayerActions = consideredActions.SkipWhile(p => p.HandActionType != firstActionType).ToList();

            if (!candidatePlayerActions.Any())
                return false;

            if (secondActionType == HandActionType.UNKNOWN)
                return true;

            //checking if second element HandActionType after firstActionType matches secondActionType 
            return candidatePlayerActions.ElementAtOrDefault(1)?.HandActionType == secondActionType;
        }
    }
}
