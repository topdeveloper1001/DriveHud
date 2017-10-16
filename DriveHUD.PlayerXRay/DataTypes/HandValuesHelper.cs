//-----------------------------------------------------------------------
// <copyright file="HandValuesHelper.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.DataTypes
{
    public class HandValuesHelper
    {
        private static Dictionary<NoteStageType, HandValueEnum[]> handValuesRestrictions = new Dictionary<NoteStageType, HandValueEnum[]>
        {
            [NoteStageType.Flop] = new[] { HandValueEnum.TwoPaironBoard, HandValueEnum.StraightStraightOnBoard, HandValueEnum.FlushOnBoardNutFlush,
                HandValueEnum.FlushOnBoardHighFlush, HandValueEnum.FlushOnBoardLowFlush, HandValueEnum.FullHouseonBoard, HandValueEnum.FourofaKindonBoard,
                HandValueEnum.StraightFlushonBoard },
            [NoteStageType.Turn] = new[] { HandValueEnum.StraightStraightOnBoard, HandValueEnum.FlushOnBoardNutFlush, HandValueEnum.FlushOnBoardHighFlush,
                HandValueEnum.FlushOnBoardLowFlush, HandValueEnum.FullHouseonBoard, HandValueEnum.StraightFlushonBoard }
        };

        public static IEnumerable<HandValueObject> GetHandValueObjects(NoteStageType stageType)
        {
            HandValueObject[] results =
            {
                Create("High Card: Two Overcards", HandValueEnum.HighCardTwoOvercards),
                Create("High Card: One Overcard", HandValueEnum.HighCardOneOvercard),
                Create("High Card: No Overcards", HandValueEnum.HighCardNoOvercards),
                Create("One Pair: Pocket Pair - Overpair", HandValueEnum.OnePairPocketPairOverpair),
                Create("One Pair: Pocket Pair - Second Pair", HandValueEnum.OnePairPocketPairSecondPair),
                Create("One Pair: Pocket Pair - Low Pair", HandValueEnum.OnePairPocketPairLowPair),
                Create("One Pair: Top Pair - Top Kicker", HandValueEnum.OnePairTopPairTopKicker),
                Create("One Pair: Top Pair - Good Kicker", HandValueEnum.OnePairTopPairGoodKicker),
                Create("One Pair: Top Pair - Weak Kicker", HandValueEnum.OnePairTopPairWeakKicker),
                Create("One Pair: Second Pair - Ace Kicker", HandValueEnum.OnePairSecondPairAceKicker),
                Create("One Pair: Second Pair - Non Ace Kicker", HandValueEnum.OnePairSecondPairNonAceKicker),
                Create("One Pair: Bottom Pair - Ace Kicker", HandValueEnum.OnePairBottomPairAceKicker),
                Create("One Pair: Bottom Pair - Non Ace Kicker", HandValueEnum.OnePairBottomPairNonAceKicker),
                Create("One Pair: Paired Board - Two Overcards", HandValueEnum.OnePairPairedBoardTwoOvercards),
                Create("One Pair: Paired Board - One Overcard", HandValueEnum.OnePairPairedBoardOneOvercard),
                Create("One Pair: Paired Board - No Overcards", HandValueEnum.OnePairPairedBoardNoOvercards),
                Create("Two Pair: Both Cards Paired - Top Two Pair", HandValueEnum.TwoPairBothCardsPairedTopTwoPair),
                Create("Two Pair: Both Cards Paired - Top Pair + Pair", HandValueEnum.TwoPairBothCardsPairedTopPairPlusPair),
                Create("Two Pair: Both Cards Paired - Middle + Bottom", HandValueEnum.TwoPairBothCardsPairedMiddlePlusBottom),
                Create("Two Pair: Pair + Paired Board - Top Pair", HandValueEnum.TwoPairPairPlusPairedBoardTopPair),
                Create("Two Pair: Pair + Paired Board - Second Pair", HandValueEnum.TwoPairPairPlusPairedBoardSecondPair),
                Create("Two Pair: Pair + Paired Board - Bottom Pair", HandValueEnum.TwoPairPairPlusPairedBoardBottomPair),
                Create("Two Pair: Pocket Pair + Paired Board - Overpair", HandValueEnum.TwoPairPocketPairPlusPairedBoardOverpair),
                Create("Two Pair: Pocket Pair + Paired Board - > Pair On Board", HandValueEnum.TwoPairPocketPairPlusPairedBoardGreaterPairOnBoard),
                Create("Two Pair: Pocket Pair + Paired Board - < Pair On Board", HandValueEnum.TwoPairPocketPairPlusPairedBoardLowerPairOnBoard),
                Create("Two Pair on Board", HandValueEnum.TwoPaironBoard),
                Create("Three of a Kind: Set - High Set", HandValueEnum.ThreeofaKindSetHighSet),
                Create("Three of a Kind: Set - Second Set", HandValueEnum.ThreeofaKindSetSecondSet),
                Create("Three of a Kind: Set - Low Set", HandValueEnum.ThreeofaKindSetLowSet),
                Create("Three of a Kind: Trips - High Trips High Kicker", HandValueEnum.ThreeofaKindTripsHighTripsHighKicker),
                Create("Three of a Kind: Trips - High Trips Low Kicker", HandValueEnum.ThreeofaKindTripsHighTripsLowKicker),
                Create("Three of a Kind: Trips - Second Trips High Kicker", HandValueEnum.ThreeofaKindTripsSecondTripsHighKicker),
                Create("Three of a Kind: Trips - Second Trips Low Kicker", HandValueEnum.ThreeofaKindTripsSecondTripsLowKicker),
                Create("Three of a Kind: Trips - Low Trips High Kicker", HandValueEnum.ThreeofaKindTripsLowTripsHighKicker),
                Create("Three of a Kind: Trips - Low Trips Low Kicker", HandValueEnum.ThreeofaKindTripsLowTripsLowKicker),
                Create("Trips On Board", HandValueEnum.TripsOnBoard),
                Create("Straight: Two Card Nut Straight", HandValueEnum.StraightTwoCardNutStraight),
                Create("Straight: Two Card Straight", HandValueEnum.StraightTwoCardStraight),
                Create("Straight: One Card Nut Straight", HandValueEnum.StraightOneCardNutStraight),
                Create("Straight: One Card Straight", HandValueEnum.StraightOneCardStraight),
                Create("Straight: Straight On Board", HandValueEnum.StraightStraightOnBoard),
                Create("Flush: 3 Flush Cards - Nut Flush", HandValueEnum.Flush3FlushCardsNutFlush),
                Create("Flush: 3 Flush Cards - High Flush", HandValueEnum.Flush3FlushCardsHighFlush),
                Create("Flush: 3 Flush Cards - Low Flush", HandValueEnum.Flush3FlushCardsLowFlush),
                Create("Flush: 4 Flush Cards - Nut Flush", HandValueEnum.Flush4FlushCardsNutFlush),
                Create("Flush: 4 Flush Cards - High Flush", HandValueEnum.Flush4FlushCardsHighFlush),
                Create("Flush: 4 Flush Cards - Low Flush", HandValueEnum.Flush4FlushCardsLowFlush),
                Create("Flush On Board: Nut Flush", HandValueEnum.FlushOnBoardNutFlush),
                Create("Flush On Board: High Flush", HandValueEnum.FlushOnBoardHighFlush),
                Create("Flush On Board: Low Flush", HandValueEnum.FlushOnBoardLowFlush),
                Create("Full House: 2 Cards - Pocket Pair no Trips on Board", HandValueEnum.FullHouse2CardsPocketPairnoTripsonBoard),
                Create("Full House: 2 Cards - Pocket Pair + Trips on Board", HandValueEnum.FullHouse2CardsPocketPairPlusTripsonBoard),
                Create("Full House: 2 Cards - No Pockets no Trips on Board", HandValueEnum.FullHouse2CardsNoPocketsnoTripsonBoard),
                Create("Full House: < 2 Cards - Trips on Board", HandValueEnum.FullHouseLess2CardsTripsonBoard),
                Create("Full House: < 2 Cards - Fill Top Pair no Trips", HandValueEnum.FullHouseLess2CardsFillTopPairnoTrips),
                Create("Full House: < 2 Cards - Fill Bottom Pair no Trips", HandValueEnum.FullHouseLess2CardsFillBottomPairnoTrips),
                Create("Full House on Board", HandValueEnum.FullHouseonBoard),
                Create("Four of a Kind: With Pocket Pair", HandValueEnum.FourofaKindWithPocketPair),
                Create("Four of a Kind: Without Pocket Pair", HandValueEnum.FourofaKindWithoutPocketPair),
                Create("Four of a Kind on Board", HandValueEnum.FourofaKindonBoard),
                Create("Straight Flush: 2 Cards", HandValueEnum.StraightFlush2Cards),
                Create("Straight Flush on Board", HandValueEnum.StraightFlushonBoard)
            };

            if (handValuesRestrictions.ContainsKey(stageType))
            {
                results = results.Where(x => !handValuesRestrictions[stageType].Contains((HandValueEnum)x.Value)).ToArray();
            }

            return results;
        }

        public static IEnumerable<HandValueObject> GetFlushHandValueObjects()
        {
            return new[]
            {
                Create("2 Card Nut Flush Draw", HandValueFlushDrawEnum.TwoCardNutFlushDraw),
                Create("2 Card High Flush Draw", HandValueFlushDrawEnum.TwoCardHighFlushDraw),
                Create("2 Card Low Flush Draw", HandValueFlushDrawEnum.TwoCardLowFlushDraw),
                Create("1 Card Nut Flush Draw", HandValueFlushDrawEnum.OneCardNutFlushDraw),
                Create("1 Card High Flush Draw", HandValueFlushDrawEnum.OneCardHighFlushDraw),
                Create("1 Card Low Flush Draw", HandValueFlushDrawEnum.OneCardLowFlushDraw),
                Create("2 Card Nut Backdoor Flush Draw", HandValueFlushDrawEnum.TwoCardNutBackdoorFlushDraw),
                Create("2 Card Backdoor Flush Draw", HandValueFlushDrawEnum.TwoCardBackdoorFlushDraw),
                Create("1 Card Nut Backdoor Flush Draw", HandValueFlushDrawEnum.OneCardNutBackdoorFlushDraw),
                Create("No Flush Draw", HandValueFlushDrawEnum.NoFlushDraw)
            };
        }

        public static IEnumerable<HandValueObject> GetStraightHandValueObjects()
        {
            return new[]
            {
                Create("2 Card Open Ended Straight Draw", HandValueStraightDraw.TwoCardOpenEndedStraightDraw),
                Create("2 Card Gutshot Draw", HandValueStraightDraw.TwoCardGutshotDraw),
                Create("1 Card Open Ended Straight Draw", HandValueStraightDraw.OneCardOpenEndedStraightDraw),
                Create("1 Card Gutshot Draw", HandValueStraightDraw.OneCardGutshotDraw),
                Create("2 Card Backdoor Straight Draw", HandValueStraightDraw.TwoCardBackdoorStraightDraw),
                Create("No Straight Draw", HandValueStraightDraw.NoStraightDraw)
            };
        }

        private static HandValueObject Create(string name, HandValueEnum handValue)
        {
            return new HandValueObject
            {
                Name = name,
                Value = (int)handValue
            };
        }

        private static HandValueObject Create(string name, HandValueFlushDrawEnum handValue)
        {
            return new HandValueObject
            {
                Name = name,
                Value = (int)handValue
            };
        }

        private static HandValueObject Create(string name, HandValueStraightDraw handValue)
        {
            return new HandValueObject
            {
                Name = name,
                Value = (int)handValue
            };
        }
    }
}