using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Extensions;
using Model.HandAnalyzers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Model.Filters
{
    [Serializable]
    public class FilterHandValueModel : FilterBaseEntity, IFilterModel, IXmlSerializable
    {
        #region Constructor

        public FilterHandValueModel()
        {
            Name = "Hand Value";
            Type = EnumFilterModelType.FilterHandValueModel;
        }

        public void Initialize()
        {
            FilterSectionFastFilterInitialize();
            FilterSectionFlopHandValuesInitialize();
            FilterSectionTurnHandValuesInitialize();
            FilterSectionRiverHandValuesInitialize();
        }

        #endregion

        #region Methods

        public void FilterSectionFastFilterInitialize()
        {
            FastFilterCollection = new ObservableCollection<FastFilterItem>()
            {
                new FastFilterItem() { Name = "Flop - TPTK", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.TPTK },
                new FastFilterItem() { Name = "Flop - Overpair", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.Overpair },
                new FastFilterItem() { Name = "Flop - Bottom Pair", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.BottomPair },
                new FastFilterItem() { Name = "Flop - Bottom Set", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.BottomSet },
                new FastFilterItem() { Name = "Flop - Any Flush Draw", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.AnyFlushDraw },
                new FastFilterItem() { Name = "Flop - Any Straight Draw", TargetStreet = Street.Flop, FastFilterType = EnumHandValuesFastFilterType.AnyStraightDraw },
                new FastFilterItem() { Name = "Turn - Overpair", TargetStreet = Street.Turn, FastFilterType = EnumHandValuesFastFilterType.Overpair },
                new FastFilterItem() { Name = "Turn - Second Pair", TargetStreet = Street.Turn, FastFilterType = EnumHandValuesFastFilterType.SecondPair },
                new FastFilterItem() { Name = "Turn - Third Pair", TargetStreet = Street.Turn, FastFilterType = EnumHandValuesFastFilterType.ThirdPair },
                new FastFilterItem() { Name = "Turn - Low Flush", TargetStreet = Street.Turn, FastFilterType = EnumHandValuesFastFilterType.LowFlush },
                new FastFilterItem() { Name = "Turn - Bottom Straight", TargetStreet = Street.Turn, FastFilterType = EnumHandValuesFastFilterType.BottomStraight },
                new FastFilterItem() { Name = "River - TPTK", TargetStreet = Street.River, FastFilterType = EnumHandValuesFastFilterType.TPTK },
                new FastFilterItem() { Name = "River - TPGK", TargetStreet = Street.River, FastFilterType = EnumHandValuesFastFilterType.TPGK },
                new FastFilterItem() { Name = "River - < Top Set", TargetStreet = Street.River, FastFilterType = EnumHandValuesFastFilterType.SetSecondOrLower},
                new FastFilterItem() { Name = "River - Low Flush", TargetStreet = Street.River, FastFilterType = EnumHandValuesFastFilterType.LowFlush },
                new FastFilterItem() { Name = "River - Bottom Straight", TargetStreet = Street.River, FastFilterType = EnumHandValuesFastFilterType.BottomStraight },
            };
        }

        public void FilterSectionFlopHandValuesInitialize()
        {
            FlopHandValuesCollection = new ObservableCollection<HandValueItem>()
            {
                new HandValueItem() { Name = "High Card: Two Overcards", Hand = ShowdownHands.HighCardTwoOvercards },
                new HandValueItem() { Name = "High Card: One Overcard", Hand = ShowdownHands.HighCardOneOvercard },
                new HandValueItem() { Name = "High Card: No Overcards", Hand = ShowdownHands.HighCardNoOvercards },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Overpair", Hand = ShowdownHands.PocketPairOverpair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Second Pair", Hand = ShowdownHands.PocketPairSecondPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Third Pair", Hand = ShowdownHands.PocketPairThirdPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Under Pair", Hand = ShowdownHands.PocketPairUnderPair },
                new HandValueItem() { Name = "One Pair: Top Pair – Top Kicker", Hand = ShowdownHands.TopPairTopKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Decent Kicker", Hand = ShowdownHands.TopPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Weak Kicker", Hand = ShowdownHands.TopPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Top Kicker", Hand = ShowdownHands.SecondPairTopKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Decent Kicker", Hand = ShowdownHands.SecondPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Weak Kicker", Hand = ShowdownHands.SecondPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Top Kicker", Hand = ShowdownHands.BottomPairTopKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Decent Kicker", Hand = ShowdownHands.BottomPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Weak Kicker", Hand = ShowdownHands.BottomPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Paired Board – Two Overcards", Hand = ShowdownHands.PairedBoardTwoOvercards },
                new HandValueItem() { Name = "One Pair: Paired Board – One Overcard", Hand = ShowdownHands.PairedBoardOneOvercard },
                new HandValueItem() { Name = "One Pair: Paired Board – No Overcards", Hand = ShowdownHands.PairedBoardNoOvercards },
                new HandValueItem() { Name = "Two Pair: Top Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopTwoPair },
                new HandValueItem() { Name = "Two Pair: Top & Bottom Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopAndBottomPair },
                new HandValueItem() { Name = "Two Pair: Bottom Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairBottomTwoPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Overpair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairOverpair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Second Pair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairSecondPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Top Pair", Hand = ShowdownHands.TwoPairPairedBoardTopPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Bottom Pair", Hand = ShowdownHands.TwoPairPairedBoardBottomPair },
                new HandValueItem() { Name = "Three of a Kind: Set – Top Set", Hand = ShowdownHands.ThreeOfAKindTopSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Middle Set", Hand = ShowdownHands.ThreeOfAKindMiddleSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Bottom Set", Hand = ShowdownHands.ThreeOfAKindBottomSet },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsSecondSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsSecondSetWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Three of a Kind on Flop", Hand = ShowdownHands.ThreeOfAKindTripsOnFlop },
                new HandValueItem() { Name = "Straight: Two Card Nut Straight", Hand = ShowdownHands.StraightTwoCardNut },
                new HandValueItem() { Name = "Straight: Two Card Straight", Hand = ShowdownHands.StraightTwoCard },
                new HandValueItem() { Name = "Flush: Nut Flush", Hand = ShowdownHands.FlushNut },
                new HandValueItem() { Name = "Flush: High Flush", Hand = ShowdownHands.FlushHigh },
                new HandValueItem() { Name = "Flush: Low Flush", Hand = ShowdownHands.FlushLow },
                new HandValueItem() { Name = "Full House: With Pocket Pair– No Trips on Board", Hand = ShowdownHands.FullHousePocketPairNoTripsOnBoard },
                new HandValueItem() { Name = "Full House: With Pocket Pair – Trips on Board", Hand = ShowdownHands.FullHousePocketPairTripsOnBoard },
                new HandValueItem() { Name = "Full House: With 2 Pocket Cards – No Trips on Board", Hand = ShowdownHands.FullHouseTwoPocketCardsNoTripsOnBoard },
                new HandValueItem() { Name = "Four of a Kind: With Pocket Pair", Hand = ShowdownHands.FourOfAKindPocketPair },
                new HandValueItem() { Name = "Four of a Kind: Without Pocket Pair", Hand = ShowdownHands.FourOfAKindNoPocketPair },
                new HandValueItem() { Name = "Straight Flush: With 2 Pocket Cards", Hand = ShowdownHands.StraightFlushTwoPocketCards },
                new HandValueItem() { Name = "Flush Draw: Nut Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardNut },
                new HandValueItem() { Name = "Flush Draw: High Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardHigh },
                new HandValueItem() { Name = "Flush Draw: Low Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardLow },
                new HandValueItem() { Name = "Flush Draw: Nut Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardNut },
                new HandValueItem() { Name = "Flush Draw: High Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardHigh },
                new HandValueItem() { Name = "Flush Draw: Low Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardLow },
                new HandValueItem() { Name = "Flush Draw: Backdoor Nut Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawBackdoorTwoCardNut },
                new HandValueItem() { Name = "Flush Draw: Backdoor Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawBackdoorTwoCard },
                new HandValueItem() { Name = "Flush Draw: Backdoor Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawBackdoorOneCard },
                new HandValueItem() { Name = "Flush Draw: No Flush Draws", Hand = ShowdownHands.FlushNoFlushDraw },
                new HandValueItem() { Name = "Straight Draw: Open Ended Straight Draw – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardOpenEnded },
                new HandValueItem() { Name = "Straight Draw: Double Gutshot – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardDoubleGutShot },
                new HandValueItem() { Name = "Straight Draw: Gutshot – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardGutShot },
                new HandValueItem() { Name = "Straight Draw: Open Ended Straight Draw – 1 Card", Hand = ShowdownHands.StraightDrawOneCardOpenEnded },
                new HandValueItem() { Name = "Straight Draw: GutShot – 1 Card", Hand = ShowdownHands.StraightDrawOneCardGutShot },
                new HandValueItem() { Name = "Straight Draw: Backdoor Straight Draw – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardBackdoor },
                new HandValueItem() { Name = "Straight Draw: No Straight Draws", Hand = ShowdownHands.StraightDrawNoStraightDraw },
            };

            foreach (var f in FlopHandValuesCollection)
            {
                f.TargetStreet = Street.Flop;
            }
        }

        public void FilterSectionTurnHandValuesInitialize()
        {
            TurnHandValuesCollection = new ObservableCollection<HandValueItem>()
            {
                new HandValueItem() { Name = "High Card: Two Overcards", Hand = ShowdownHands.HighCardTwoOvercards },
                new HandValueItem() { Name = "High Card: One Overcard", Hand = ShowdownHands.HighCardOneOvercard },
                new HandValueItem() { Name = "High Card: No Overcards", Hand = ShowdownHands.HighCardNoOvercards },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Overpair", Hand = ShowdownHands.PocketPairOverpair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Second Pair", Hand = ShowdownHands.PocketPairSecondPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Third Pair", Hand = ShowdownHands.PocketPairThirdPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Under Pair", Hand = ShowdownHands.PocketPairUnderPair },
                new HandValueItem() { Name = "One Pair: Top Pair – Top Kicker", Hand = ShowdownHands.TopPairTopKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Decent Kicker", Hand = ShowdownHands.TopPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Weak Kicker", Hand = ShowdownHands.TopPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Top Kicker", Hand = ShowdownHands.SecondPairTopKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Decent Kicker", Hand = ShowdownHands.SecondPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Weak Kicker", Hand = ShowdownHands.SecondPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Top Kicker", Hand = ShowdownHands.BottomPairTopKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Decent Kicker", Hand = ShowdownHands.BottomPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Weak Kicker", Hand = ShowdownHands.BottomPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Paired Board – Two Overcards", Hand = ShowdownHands.PairedBoardTwoOvercards },
                new HandValueItem() { Name = "One Pair: Paired Board – One Overcard", Hand = ShowdownHands.PairedBoardOneOvercard },
                new HandValueItem() { Name = "One Pair: Paired Board – No Overcards", Hand = ShowdownHands.PairedBoardNoOvercards },
                new HandValueItem() { Name = "Two Pair: Top Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopTwoPair },
                new HandValueItem() { Name = "Two Pair: Top & Bottom Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopAndBottomPair },
                new HandValueItem() { Name = "Two Pair: Bottom Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairBottomTwoPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Overpair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairOverpair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Second Pair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairSecondPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Top Pair", Hand = ShowdownHands.TwoPairPairedBoardTopPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Bottom Pair", Hand = ShowdownHands.TwoPairPairedBoardBottomPair },
                new HandValueItem() { Name = "Three of a Kind: Set – Top Set", Hand = ShowdownHands.ThreeOfAKindTopSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Middle Set", Hand = ShowdownHands.ThreeOfAKindMiddleSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Bottom Set", Hand = ShowdownHands.ThreeOfAKindBottomSet },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsSecondSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Three of a Kind on Flop", Hand = ShowdownHands.ThreeOfAKindTripsOnFlop },
                new HandValueItem() { Name = "Straight: Two Card Nut Straight", Hand = ShowdownHands.StraightTwoCardNut },
                new HandValueItem() { Name = "Straight: Two Card Straight", Hand = ShowdownHands.StraightTwoCard },
                new HandValueItem() { Name = "Flush: Nut Flush", Hand = ShowdownHands.FlushNut },
                new HandValueItem() { Name = "Flush: High Flush", Hand = ShowdownHands.FlushHigh },
                new HandValueItem() { Name = "Flush: Low Flush", Hand = ShowdownHands.FlushLow },
                new HandValueItem() { Name = "Full House: With Pocket Pair– No Trips on Board", Hand = ShowdownHands.FullHousePocketPairNoTripsOnBoard },
                new HandValueItem() { Name = "Full House: With Pocket Pair – Trips on Board", Hand = ShowdownHands.FullHousePocketPairTripsOnBoard },
                new HandValueItem() { Name = "Full House: With 2 Pocket Cards – No Trips on Board", Hand = ShowdownHands.FullHouseTwoPocketCardsNoTripsOnBoard },
                new HandValueItem() { Name = "Four of a Kind: With Pocket Pair", Hand = ShowdownHands.FourOfAKindPocketPair },
                new HandValueItem() { Name = "Four of a Kind: Without Pocket Pair", Hand = ShowdownHands.FourOfAKindNoPocketPair },
                new HandValueItem() { Name = "Straight Flush: With 2 Pocket Cards", Hand = ShowdownHands.StraightFlushTwoPocketCards },
                new HandValueItem() { Name = "Flush Draw: Nut Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardNut },
                new HandValueItem() { Name = "Flush Draw: High Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardHigh },
                new HandValueItem() { Name = "Flush Draw: Low Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawTwoCardLow },
                new HandValueItem() { Name = "Flush Draw: Nut Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardNut },
                new HandValueItem() { Name = "Flush Draw: High Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardHigh },
                new HandValueItem() { Name = "Flush Draw: Low Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawOneCardLow },
                new HandValueItem() { Name = "Flush Draw: Backdoor Nut Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawBackdoorTwoCardNut },
                new HandValueItem() { Name = "Flush Draw: Backdoor Flush Draw – 2 Card", Hand = ShowdownHands.FlushDrawBackdoorTwoCard },
                new HandValueItem() { Name = "Flush Draw: Backdoor Flush Draw – 1 Card", Hand = ShowdownHands.FlushDrawBackdoorOneCard },
                new HandValueItem() { Name = "Flush Draw: No Flush Draws", Hand = ShowdownHands.FlushNoFlushDraw },
                new HandValueItem() { Name = "Straight Draw: Open Ended Straight Draw – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardOpenEnded },
                new HandValueItem() { Name = "Straight Draw: Double Gutshot – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardDoubleGutShot },
                new HandValueItem() { Name = "Straight Draw: Gutshot – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardGutShot },
                new HandValueItem() { Name = "Straight Draw: Open Ended Straight Draw – 1 Card", Hand = ShowdownHands.StraightDrawOneCardOpenEnded },
                new HandValueItem() { Name = "Straight Draw: GutShot – 1 Card", Hand = ShowdownHands.StraightDrawOneCardGutShot },
                new HandValueItem() { Name = "Straight Draw: Backdoor Straight Draw – 2 Card", Hand = ShowdownHands.StraightDrawTwoCardBackdoor },
                new HandValueItem() { Name = "Straight Draw: No Straight Draws", Hand = ShowdownHands.StraightDrawNoStraightDraw },
            };
            foreach (var f in TurnHandValuesCollection)
            {
                f.TargetStreet = Street.Turn;
            }
        }

        public void FilterSectionRiverHandValuesInitialize()
        {
            RiverHandValuesCollection = new ObservableCollection<HandValueItem>()
            {
                new HandValueItem() { Name = "High Card: Two Overcards", Hand = ShowdownHands.HighCardTwoOvercards },
                new HandValueItem() { Name = "High Card: One Overcard", Hand = ShowdownHands.HighCardOneOvercard },
                new HandValueItem() { Name = "High Card: No Overcards", Hand = ShowdownHands.HighCardNoOvercards },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Overpair", Hand = ShowdownHands.PocketPairOverpair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Second Pair", Hand = ShowdownHands.PocketPairSecondPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Third Pair", Hand = ShowdownHands.PocketPairThirdPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Fourth Pair", Hand = ShowdownHands.PocketPairFourthPair },
                new HandValueItem() { Name = "One Pair: Pocket Pair – Under Pair", Hand = ShowdownHands.PocketPairUnderPair },
                new HandValueItem() { Name = "One Pair: Top Pair – Top Kicker", Hand = ShowdownHands.TopPairTopKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Decent Kicker", Hand = ShowdownHands.TopPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Top Pair – Weak Kicker", Hand = ShowdownHands.TopPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Top Kicker", Hand = ShowdownHands.SecondPairTopKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Decent Kicker", Hand = ShowdownHands.SecondPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Second Pair – Weak Kicker", Hand = ShowdownHands.SecondPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Third Pair – Top Kicker", Hand = ShowdownHands.ThirdPairTopKicker },
                new HandValueItem() { Name = "One Pair: Third Pair – Decent Kicker", Hand = ShowdownHands.ThirdPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Third Pair – Weak Kicker", Hand = ShowdownHands.ThirdPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Top Kicker", Hand = ShowdownHands.BottomPairTopKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Decent Kicker", Hand = ShowdownHands.BottomPairDecentKicker },
                new HandValueItem() { Name = "One Pair: Bottom Pair – Weak Kicker", Hand = ShowdownHands.BottomPairWeakKicker },
                new HandValueItem() { Name = "One Pair: Paired Board – Two Overcards", Hand = ShowdownHands.PairedBoardTwoOvercards },
                new HandValueItem() { Name = "One Pair: Paired Board – One Overcard", Hand = ShowdownHands.PairedBoardOneOvercard },
                new HandValueItem() { Name = "One Pair: Paired Board – No Overcards", Hand = ShowdownHands.PairedBoardNoOvercards },
                new HandValueItem() { Name = "Two Pair: Top Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopTwoPair },
                new HandValueItem() { Name = "Two Pair: Top & Bottom Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairTopAndBottomPair },
                new HandValueItem() { Name = "Two Pair: Bottom Two Pair – Both Cards Paired", Hand = ShowdownHands.TwoPairBottomTwoPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Overpair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairOverpair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Pocket Pair Second Pair", Hand = ShowdownHands.TwoPairPairedBoardPocketPairSecondPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Top Pair", Hand = ShowdownHands.TwoPairPairedBoardTopPair },
                new HandValueItem() { Name = "Two Pair: Paired Board & Bottom Pair", Hand = ShowdownHands.TwoPairPairedBoardBottomPair },
                new HandValueItem() { Name = "Three of a Kind: Set – Top Set", Hand = ShowdownHands.ThreeOfAKindTopSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Second Set", Hand = ShowdownHands.ThreeOfAKindSecondSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Third Set", Hand = ShowdownHands.ThreeOfAKindMiddleSet },
                new HandValueItem() { Name = "Three of a Kind: Set – Bottom Set", Hand = ShowdownHands.ThreeOfAKindBottomSet },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Top set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsTopSetWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & High Kicker", Hand = ShowdownHands.ThreeOfAKindTripsSecondSetHighKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Second set of Trips & Weak Kicker", Hand = ShowdownHands.ThreeOfAKindTripsSecondSetWeakKicker },
                new HandValueItem() { Name = "Three of a Kind: Trips – Three of a Kind on Flop", Hand = ShowdownHands.ThreeOfAKindTripsOnFlop },
                new HandValueItem() { Name = "Straight: Two Card Nut Straight", Hand = ShowdownHands.StraightTwoCardNut },
                new HandValueItem() { Name = "Straight: Two Card Straight", Hand = ShowdownHands.StraightTwoCard },
                new HandValueItem() { Name = "Straight: One Card Nut Straight", Hand = ShowdownHands.StraightOneCardNut },
                new HandValueItem() { Name = "Straight: One Card High Straight", Hand = ShowdownHands.StraightOneCardHigh },
                new HandValueItem() { Name = "Straight: One Card Low Straight", Hand = ShowdownHands.StraightOneCardLow },
                new HandValueItem() { Name = "Straight: Straight on board", Hand = ShowdownHands.OnBoardStraight },
                new HandValueItem() { Name = "Flush: Nut Flush w/ 2 Holecards", Hand = ShowdownHands.FlushTwoCardNut },
                new HandValueItem() { Name = "Flush: High Flush w/ 2 Holdecards", Hand = ShowdownHands.FlushTwoCardHigh },
                new HandValueItem() { Name = "Flush: Low Flush w/ 2 Holecards", Hand = ShowdownHands.FlushTwoCardLow },
                new HandValueItem() { Name = "Flush: Nut Flush w/ 1 Holecard", Hand = ShowdownHands.FlushOneCardNut },
                new HandValueItem() { Name = "Flush: High Flush w/ 1 Holdecard", Hand = ShowdownHands.FlushOneCardHigh },
                new HandValueItem() { Name = "Flush: Low Flush w/ 1 Holecard", Hand = ShowdownHands.FlushOneCardLow },
                new HandValueItem() { Name = "Flush on Board: Nut Flush", Hand = ShowdownHands.FlushOnBoardNut },
                new HandValueItem() { Name = "Flush on Board: High Flush", Hand = ShowdownHands.FlushOnBoardHigh },
                new HandValueItem() { Name = "Flush on Board: Low Flush", Hand = ShowdownHands.FlushOnBoardLow },
                new HandValueItem() { Name = "Full House: With Pocket Pair– No Trips on Board", Hand = ShowdownHands.FullHousePocketPairNoTripsOnBoard },
                new HandValueItem() { Name = "Full House: With Pocket Pair – Trips on Board", Hand = ShowdownHands.FullHousePocketPairTripsOnBoard },
                new HandValueItem() { Name = "Full House: With 2 Holecards", Hand = ShowdownHands.FullHouseTwoPocketCardsNoTripsOnBoard },
                new HandValueItem() { Name = "Full House: With 1 Holecard", Hand = ShowdownHands.FullHouseOnePocketCard },
                new HandValueItem() { Name = "Full House: Full House on Board", Hand = ShowdownHands.FullHouseOnBoard },
                new HandValueItem() { Name = "Four of a Kind: With Pocket Pair", Hand = ShowdownHands.FourOfAKindPocketPair },
                new HandValueItem() { Name = "Four of a Kind: Without Pocket Pair", Hand = ShowdownHands.FourOfAKindNoPocketPair },
                new HandValueItem() { Name = "Four of a Kind: 4 of a Kind on Board", Hand = ShowdownHands.OnBoardFourOfAKind },
                new HandValueItem() { Name = "Straight Flush: With 2 Holecards", Hand = ShowdownHands.StraightFlushTwoPocketCards },
                new HandValueItem() { Name = "Straight Flush: With 1 Holdecard", Hand = ShowdownHands.StraightFlushOneHoleCard },
                new HandValueItem() { Name = "Straight Flush: Straight Flush on Board", Hand = ShowdownHands.OnBoardStraightFlush },
            };

            foreach (var f in RiverHandValuesCollection)
            {
                f.TargetStreet = Street.River;
            }

        }

        public Expression<Func<Playerstatistic, bool>> GetFilterPredicate()
        {
            if (!FlopHandValuesCollection.Any(x => x.IsChecked) && !TurnHandValuesCollection.Any(x => x.IsChecked) && !RiverHandValuesCollection.Any(x => x.IsChecked))
            {
                return GetFastFilterPredicate();
            }

            return GetFastFilterPredicate()
                .And(GetFlopHandValuesPredicate()
                    .Or(GetTurnHandValuesPredicate())
                    .Or(GetRiverHandValuesPredicate()));
        }

        private IEnumerable<IAnalyzer> GetSelectedAnalyzers(ObservableCollection<HandValueItem> collection, IAnalyzer[] analyzers)
        {
            return analyzers.Where(d => collection.Any(c => c.IsChecked && c.Hand == d.GetRank()));
        }

        public void ResetFilter()
        {
            ResetFastFilterCollection();
            ResetFlopHandValuesCollection();
            ResetTurnHandValuesCollection();
            ResetRiverHandValuesCollection();
        }

        public override object Clone()
        {
            FilterHandValueModel model = this.DeepCloneJson();

            return model;
        }

        public void LoadFilter(IFilterModel filter)
        {
            if (filter is FilterHandValueModel)
            {
                var filterToLoad = filter as FilterHandValueModel;

                ResetFastFilterTo(filterToLoad.FastFilterCollection.ToList());
                ResetFlopHandValuesTo(filterToLoad.FlopHandValuesCollection.ToList());
                ResetTurnHandValuesTo(filterToLoad.TurnHandValuesCollection.ToList());
                ResetRiverHandValuesTo(filterToLoad.RiverHandValuesCollection.ToList());
            }
        }

        #endregion

        #region ResetFilters

        public void ResetFastFilterCollection()
        {
            FastFilterCollection.Where(x => x.CurrentTriState != EnumTriState.Any).ToList().ForEach(x => x.CurrentTriState = EnumTriState.Any);
        }

        public void ResetFlopHandValuesCollection()
        {
            FlopHandValuesCollection.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
        }

        public void ResetTurnHandValuesCollection()
        {
            TurnHandValuesCollection.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
        }

        public void ResetRiverHandValuesCollection()
        {
            RiverHandValuesCollection.Where(x => x.IsChecked).ToList().ForEach(x => x.IsChecked = false);
        }

        #endregion

        #region Restore Defaults

        private void ResetFastFilterTo(IEnumerable<FastFilterItem> fastFilterList)
        {
            foreach (var filter in fastFilterList)
            {
                var cur = FastFilterCollection.FirstOrDefault(x => x.Name == filter.Name);

                if (cur != null)
                {
                    cur.CurrentTriState = filter.CurrentTriState;
                }
            }
        }

        private void ResetFlopHandValuesTo(IEnumerable<HandValueItem> handValues)
        {
            foreach (var hand in handValues)
            {
                var cur = FlopHandValuesCollection.FirstOrDefault(x => x.Name == hand.Name);
                if (cur != null)
                {
                    cur.IsChecked = hand.IsChecked;
                }
            }
        }

        private void ResetTurnHandValuesTo(IEnumerable<HandValueItem> handValues)
        {
            foreach (var hand in handValues)
            {
                var cur = TurnHandValuesCollection.FirstOrDefault(x => x.Name == hand.Name);
                if (cur != null)
                {
                    cur.IsChecked = hand.IsChecked;
                }
            }
        }

        private void ResetRiverHandValuesTo(IEnumerable<HandValueItem> handValues)
        {
            foreach (var hand in handValues)
            {
                var cur = RiverHandValuesCollection.FirstOrDefault(x => x.Name == hand.Name);
                if (cur != null)
                {
                    cur.IsChecked = hand.IsChecked;
                }
            }
        }

        #endregion

        #region Predicates

        private Expression<Func<Playerstatistic, bool>> GetFlopHandValuesPredicate()
        {
            if (!FlopHandValuesCollection.Any(x => x.IsChecked))
            {
                return PredicateBuilder.False<Playerstatistic>();
            }

            var flopHandValuesPredicate = PredicateBuilder.False<Playerstatistic>();

            var selectedHandAnalyzersList = GetSelectedAnalyzers(FlopHandValuesCollection, HandAnalyzer.GetHandValuesAnalyzers()).Select(x => x.GetRank());
            var selectedDrawAnalyzersList = GetSelectedAnalyzers(FlopHandValuesCollection, HandAnalyzer.GetDrawAnalyzers()).Select(x => x.GetRank());
            if (selectedDrawAnalyzersList.Any())
            {
                var noDrawAnalyzers = HandAnalyzer.GetNoDrawAnalyzers();
                var straightDrawAnalyzers = HandAnalyzer.GetStraightDrawAnalyzers(includeBackdoor: true);
                var flushDrawAnalyzers = HandAnalyzer.GetFlushDrawAnalyzers(includeBackdoor: true);

                var streetPredicate = PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.Flop));
                var drawPredicate = PredicateBuilder.False<Playerstatistic>();

                if (selectedDrawAnalyzersList.Any(s => noDrawAnalyzers.Select(n => n.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(noDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop)).GetRank()));
                }

                if (selectedDrawAnalyzersList.Any(s => straightDrawAnalyzers.Select(d => d.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(straightDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop)).GetRank()));
                }

                if (selectedDrawAnalyzersList.Any(s => flushDrawAnalyzers.Select(d => d.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(flushDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop)).GetRank()));
                }

                flopHandValuesPredicate = flopHandValuesPredicate.Or(streetPredicate.And(drawPredicate));
            }

            if (selectedHandAnalyzersList.Any())
            {
                var handAnalyzers = new HandAnalyzer(HandAnalyzer.GetHandValuesAnalyzers());
                flopHandValuesPredicate = flopHandValuesPredicate.Or(x => CardHelper.IsStreetAvailable(x.Board, Street.Flop)
                                           && selectedHandAnalyzersList.Contains(handAnalyzers.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Flop)).GetRank()));
            }

            return flopHandValuesPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetTurnHandValuesPredicate()
        {
            if (!TurnHandValuesCollection.Any(x => x.IsChecked))
            {
                return PredicateBuilder.False<Playerstatistic>();
            }

            var turnHandValuesPredicate = PredicateBuilder.False<Playerstatistic>();

            var selectedHandAnalyzersList = GetSelectedAnalyzers(TurnHandValuesCollection, HandAnalyzer.GetHandValuesAnalyzers()).Select(x => x.GetRank());
            var selectedDrawAnalyzersList = GetSelectedAnalyzers(TurnHandValuesCollection, HandAnalyzer.GetDrawAnalyzers()).Select(x => x.GetRank());
            if (selectedDrawAnalyzersList.Any())
            {
                var noDrawAnalyzers = HandAnalyzer.GetNoDrawAnalyzers();
                var straightDrawAnalyzers = HandAnalyzer.GetStraightDrawAnalyzers(includeBackdoor: true);
                var flushDrawAnalyzers = HandAnalyzer.GetFlushDrawAnalyzers(includeBackdoor: true);

                var streetPredicate = PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, Street.Turn));
                var drawPredicate = PredicateBuilder.False<Playerstatistic>();

                if (selectedDrawAnalyzersList.Any(s => noDrawAnalyzers.Select(n => n.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(noDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Turn)).GetRank()));
                }

                if (selectedDrawAnalyzersList.Any(s => straightDrawAnalyzers.Select(d => d.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(straightDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Turn)).GetRank()));
                }

                if (selectedDrawAnalyzersList.Any(s => flushDrawAnalyzers.Select(d => d.GetRank()).Contains(s)))
                {
                    var analyzer = new HandAnalyzer(flushDrawAnalyzers);
                    drawPredicate = drawPredicate.Or(x => selectedDrawAnalyzersList.Contains(analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Turn)).GetRank()));
                }

                turnHandValuesPredicate = turnHandValuesPredicate.Or(streetPredicate.And(drawPredicate));
            }

            if (selectedHandAnalyzersList.Any())
            {
                var handAnalyzers = new HandAnalyzer(HandAnalyzer.GetHandValuesAnalyzers());
                turnHandValuesPredicate = turnHandValuesPredicate.Or(x => CardHelper.IsStreetAvailable(x.Board, Street.Turn)
                                           && selectedHandAnalyzersList.Contains(handAnalyzers.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.Turn)).GetRank()));
            }

            return turnHandValuesPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetRiverHandValuesPredicate()
        {
            if (!RiverHandValuesCollection.Any(x => x.IsChecked))
            {
                return PredicateBuilder.False<Playerstatistic>();
            }

            var riverHandValuesPredicate = PredicateBuilder.False<Playerstatistic>();

            var selectedHandAnalyzersList = GetSelectedAnalyzers(RiverHandValuesCollection, HandAnalyzer.GetRiverHandValuesAnalyzers()).Select(x => x.GetRank());

            if (selectedHandAnalyzersList.Any())
            {
                var handAnalyzers = new HandAnalyzer(HandAnalyzer.GetRiverHandValuesAnalyzers());
                riverHandValuesPredicate = riverHandValuesPredicate.Or(x => CardHelper.IsStreetAvailable(x.Board, Street.River)
                                           && selectedHandAnalyzersList.Contains(handAnalyzers.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(Street.River)).GetRank()));
            }

            return riverHandValuesPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetFastFilterPredicate()
        {
            if (!FastFilterCollection.Any(x => x.CurrentTriState != EnumTriState.Any))
            {
                return PredicateBuilder.True<Playerstatistic>();
            }

            var fastFilterPredicate = PredicateBuilder.True<Playerstatistic>();

            foreach (var item in FastFilterCollection.Where(x => x.CurrentTriState != EnumTriState.Any))
            {
                var predicate = GetPredicateForFastFilterType(item);

                if (item.CurrentTriState == EnumTriState.Off)
                {
                    predicate = PredicateBuilder.Not(predicate);
                }

                fastFilterPredicate = fastFilterPredicate.And(predicate);
            }

            return fastFilterPredicate;
        }

        private Expression<Func<Playerstatistic, bool>> GetPredicateForFastFilterType(FastFilterItem fastFilterItem)
        {
            var analyzer = new HandAnalyzer(HandAnalyzer.GetHandValuesAnalyzers());
            switch (fastFilterItem.FastFilterType)
            {
                case EnumHandValuesFastFilterType.TPTK:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && new HandAnalyzer(HandAnalyzer.GetHandValuesAnalyzers()).Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.TopPairTopKicker);
                case EnumHandValuesFastFilterType.TPGK:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.TopPairDecentKicker);
                case EnumHandValuesFastFilterType.Overpair:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.PocketPairOverpair);
                case EnumHandValuesFastFilterType.SecondPair:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.SecondPair);
                case EnumHandValuesFastFilterType.ThirdPair:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.ThirdPair);
                case EnumHandValuesFastFilterType.BottomPair:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.BottomPair);
                case EnumHandValuesFastFilterType.BottomSet:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.ThreeOfAKindBottomSet);
                case EnumHandValuesFastFilterType.AnyFlushDraw:
                    var flushDrawAnalyzer = new HandAnalyzer(HandAnalyzer.GetFlushDrawAnalyzers());
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && flushDrawAnalyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() != ShowdownHands.None);
                case EnumHandValuesFastFilterType.AnyStraightDraw:
                    var straightDrawAnalyzer = new HandAnalyzer(HandAnalyzer.GetStraightDrawAnalyzers());
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && straightDrawAnalyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() != ShowdownHands.None);
                case EnumHandValuesFastFilterType.LowFlush:
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && analyzer.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank() == ShowdownHands.FlushLow);
                case EnumHandValuesFastFilterType.BottomStraight:
                    var reportAnalyzers = new HandAnalyzer(HandAnalyzer.GetReportAnalyzers());
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && HandAnalyzerHelpers.IsBottomStraight(reportAnalyzers.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank()));
                case EnumHandValuesFastFilterType.SetSecondOrLower:
                    var riverAnalyzers = new HandAnalyzer(HandAnalyzer.GetRiverHandValuesAnalyzers());
                    return PredicateBuilder.Create<Playerstatistic>(x => CardHelper.IsStreetAvailable(x.Board, fastFilterItem.TargetStreet)
                                      && HandAnalyzerHelpers.IsSetSecondOrLower(riverAnalyzers.Analyze(CardGroup.Parse(x.Cards), BoardCards.FromCards(x.Board).GetBoardOnStreet(fastFilterItem.TargetStreet)).GetRank()));
                default:
                    return PredicateBuilder.False<Playerstatistic>();
            }
        }

        #endregion

        #region Properties

        private ObservableCollection<FastFilterItem> _fastFilterCollection;
        private ObservableCollection<HandValueItem> _flopHandValuesCollection;
        private ObservableCollection<HandValueItem> _turnHandValuesCollection;
        private ObservableCollection<HandValueItem> _riverHandValuesCollection;
        private EnumFilterModelType _type;

        public EnumFilterModelType Type
        {
            get { return _type; }
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<FastFilterItem> FastFilterCollection
        {
            get
            {
                return _fastFilterCollection;
            }

            set
            {
                if (value == _fastFilterCollection) return;
                _fastFilterCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HandValueItem> FlopHandValuesCollection
        {
            get
            {
                return _flopHandValuesCollection;
            }

            set
            {
                if (value == _flopHandValuesCollection) return;
                _flopHandValuesCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HandValueItem> TurnHandValuesCollection
        {
            get
            {
                return _turnHandValuesCollection;
            }

            set
            {
                if (value == _turnHandValuesCollection) return;
                _turnHandValuesCollection = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HandValueItem> RiverHandValuesCollection
        {
            get
            {
                return _riverHandValuesCollection;
            }

            set
            {
                if (value == _riverHandValuesCollection) return;
                _riverHandValuesCollection = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region IXmlSerializable implementation

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            Initialize();

            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.IsStartElement(nameof(Id)))
                {
                    Id = Guid.Parse(reader.ReadElementContentAsString());
                }

                if (reader.IsStartElement(nameof(FastFilterCollection)))
                {
                    if (reader.IsEmptyElement)
                    {
                        continue;
                    }

                    reader.ReadStartElement(nameof(FastFilterCollection));

                    while (reader.IsStartElement(nameof(FastFilterItem)))
                    {
                        var fastFilterItemType = typeof(FastFilterItem);
                        var serializer = new XmlSerializer(fastFilterItemType);

                        var fastFilterItem = (FastFilterItem)serializer.Deserialize(reader);

                        var existingFastFilterItem = FastFilterCollection.FirstOrDefault(x => x.Name == fastFilterItem.Name);

                        if (existingFastFilterItem == null)
                        {
                            FastFilterCollection.Add(fastFilterItem);
                        }
                        else
                        {
                            existingFastFilterItem.Id = fastFilterItem.Id;
                            existingFastFilterItem.IsActive = fastFilterItem.IsActive;
                            existingFastFilterItem.CurrentTriState = fastFilterItem.CurrentTriState;
                            existingFastFilterItem.FastFilterType = fastFilterItem.FastFilterType;
                            existingFastFilterItem.TargetStreet = fastFilterItem.TargetStreet;
                        }
                    }

                    reader.ReadEndElement();
                }

                ReadHandValueCollectionsXml(reader, FlopHandValuesCollection, nameof(FlopHandValuesCollection));
                ReadHandValueCollectionsXml(reader, TurnHandValuesCollection, nameof(TurnHandValuesCollection));
                ReadHandValueCollectionsXml(reader, RiverHandValuesCollection, nameof(RiverHandValuesCollection));

                if (reader.Name == nameof(FilterHandValueModel) && reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.ReadEndElement();
                    break;
                }
            }
        }

        private static void ReadHandValueCollectionsXml(XmlReader reader, ObservableCollection<HandValueItem> collection, string nameOfCollection)
        {
            if (!reader.IsStartElement(nameOfCollection) || reader.IsEmptyElement)
            {
                return;
            }

            reader.ReadStartElement(nameOfCollection);

            while (reader.IsStartElement(nameof(HandValueItem)))
            {
                var handValueItemType = typeof(HandValueItem);
                var serializer = new XmlSerializer(handValueItemType);

                var handValueItem = (HandValueItem)serializer.Deserialize(reader);

                var existingHandValueItem = collection.FirstOrDefault(x => x.Name == handValueItem.Name);

                if (existingHandValueItem == null)
                {
                    collection.Add(handValueItem);
                }
                else
                {
                    existingHandValueItem.Id = handValueItem.Id;
                    existingHandValueItem.IsActive = handValueItem.IsActive;
                    existingHandValueItem.IsChecked = handValueItem.IsChecked;
                    existingHandValueItem.Hand = handValueItem.Hand;
                    existingHandValueItem.TargetStreet = handValueItem.TargetStreet;
                }
            }

            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(nameof(Id));
            writer.WriteValue(Id.ToString());
            writer.WriteEndElement();

            writer.WriteStartElement(nameof(Type));
            writer.WriteValue(Type.ToString());
            writer.WriteEndElement();

            var fastFilterCollection = FastFilterCollection?
                .ToArray()
                .Where(x => x.CurrentTriState != EnumTriState.Any)
                .ToArray();

            if (fastFilterCollection != null && fastFilterCollection.Length > 0)
            {
                writer.WriteStartElement(nameof(FastFilterCollection));

                foreach (var fastFilterItem in fastFilterCollection)
                {
                    var xmlSerializer = new XmlSerializer(fastFilterItem.GetType());
                    xmlSerializer.Serialize(writer, fastFilterItem);
                }

                writer.WriteEndElement();
            }

            WriteHandValueCollectionsXml(writer, FlopHandValuesCollection, nameof(FlopHandValuesCollection));
            WriteHandValueCollectionsXml(writer, TurnHandValuesCollection, nameof(TurnHandValuesCollection));
            WriteHandValueCollectionsXml(writer, RiverHandValuesCollection, nameof(RiverHandValuesCollection));
        }

        private static void WriteHandValueCollectionsXml(XmlWriter writer, ObservableCollection<HandValueItem> collection, string nameOfCollection)
        {
            var filteredCollection = collection?
                .ToArray()
                .Where(x => x.IsChecked)
                .ToArray();

            if (filteredCollection != null && filteredCollection.Length > 0)
            {
                writer.WriteStartElement(nameOfCollection);

                foreach (var handValueItem in filteredCollection)
                {
                    var xmlSerializer = new XmlSerializer(handValueItem.GetType());
                    xmlSerializer.Serialize(writer, handValueItem);
                }

                writer.WriteEndElement();
            }
        }

        #endregion
    }

    [Serializable]
    public class FastFilterItem : FilterTriStateBase
    {
        public static Action OnTriState;

        public FastFilterItem() : this(EnumTriState.Any) { }

        public FastFilterItem(EnumTriState param = EnumTriState.Any) : base(param)
        {
        }

        private Street _targetStreet;
        private EnumHandValuesFastFilterType _fastFilterType;

        public Street TargetStreet
        {
            get
            {
                return _targetStreet;
            }

            set
            {
                _targetStreet = value;
            }
        }

        public override EnumTriState CurrentTriState
        {
            get
            {
                return base.CurrentTriState;
            }
            set
            {
                if (value == currentTriState)
                {
                    return;
                }

                currentTriState = value;

                OnPropertyChanged();

                if (OnTriState != null)
                {
                    OnTriState.Invoke();
                }
            }
        }

        public EnumHandValuesFastFilterType FastFilterType
        {
            get
            {
                return _fastFilterType;
            }

            set
            {
                _fastFilterType = value;
            }
        }
    }

    [Serializable]
    public class HandValueItem : FilterBaseEntity
    {
        public static Action<Street> OnIsChecked;

        private bool _isChecked;
        private Street _targetStreet;
        private ShowdownHands _hand;

        public bool IsChecked
        {
            get
            {
                return _isChecked;
            }

            set
            {
                if (value == _isChecked) return;
                _isChecked = value;
                OnPropertyChanged();

                if (OnIsChecked != null) OnIsChecked.Invoke(TargetStreet);
            }
        }

        public Street TargetStreet
        {
            get
            {
                return _targetStreet;
            }

            set
            {
                _targetStreet = value;
            }
        }

        public ShowdownHands Hand
        {
            get
            {
                return _hand;
            }

            set
            {
                _hand = value;
            }
        }
    }
}
