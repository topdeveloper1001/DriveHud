using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers
{
    public static class BoardTextureAnalyzerHelpers
    {
        public static List<int> GetOrderedBoardNumericRanks(List<DataTypes.Card> boardCards, Street targetStreet)
        {
            List<DataTypes.Card> board = GetCardsAccoringStreet(boardCards, targetStreet);

            var orderedRanks = board.OrderBy(x => x.Rank).Select(x => x.Rank).Distinct().ToList();
            if (board.Any(x => x.Rank == (int)CardRank.Ace))
            {
                orderedRanks.Insert(0, 1);
            }

            return orderedRanks;
        }

        public static List<DataTypes.Card> GetCardsAccoringStreet(List<DataTypes.Card> boardCards, Street targetStreet)
        {
            switch (targetStreet)
            {
                case Street.Flop:
                    if (boardCards.Count >= 3)
                        return boardCards.Take(3).ToList();
                    break;
                case Street.Turn:
                    if (boardCards.Count >= 4)
                        return boardCards.Take(4).ToList();
                    break;
                case Street.River:
                    if (boardCards.Count == 5)
                        return boardCards;
                    break;
            }
            return new List<DataTypes.Card>();
        }


        /// <summary>
        /// return if cards there is enough cards for the considered street
        /// </summary>
        /// <param name="boardCards">All board cards</param>
        /// <param name="targetStreet">Street where we expect to have enought cards</param>
        /// <returns></returns>
        public static bool IsStreetAvailable(List<DataTypes.Card> boardCards, Street targetStreet)
        {
            var count = boardCards.Count;

            switch (targetStreet)
            {
                case Street.Preflop:
                    return true;
                case Street.Flop:
                    return count > 2;
                case Street.Turn:
                    return count > 3;
                case Street.River:
                    return count > 4;
                default:
                    return false;
            }
        }

        public static int GetHighestUniqueRankNumberForList(List<int> ranks, int maxValue)
        {
            int aceRank = (int)CardRank.Ace;
            //GetRankNumericValue(PossibleRanksHighCardFirst.First());
            int result = maxValue > aceRank ? aceRank : maxValue;
            while (ranks.Contains(result))
            {
                result--;
            }

            return result;
        }

        public static bool CheckEquality(CompareEnum equality, int firstValue, int secondValue)
        {
            switch (equality)
            {
                case CompareEnum.EqualTo:
                    return firstValue == secondValue;
                case CompareEnum.GreaterThan:
                    return firstValue > secondValue;
                case CompareEnum.LessThan:
                    return firstValue < secondValue;
            }

            return false;
        }

        public static List<DataTypes.Card> ParseStringSequenceOfCards(string boardCards)
        {
            List<DataTypes.Card> list = new List<DataTypes.Card>();

            for (int i = 0; i < boardCards.Length; i += 2)
            {
                list.Add(new DataTypes.Card("" + boardCards[i] + boardCards[i + 1]));
            }

            return list;
        }

        public static CardRank HighestBoardCardRank(string boardCards, Street targetStreet)
        {
            List<DataTypes.Card> cards = ParseStringSequenceOfCards(boardCards);
            List<DataTypes.Card> board = GetCardsAccoringStreet(cards, targetStreet);
            return board.Count == 0 ? CardRank.None : board.Max(x => x.CardValue);
        }

        public static bool BoardContainsExactTextureCards(string boardCards, List<string> selectedCardTextureList, Street targetStreet)
        {
            List<DataTypes.Card> cards = ParseStringSequenceOfCards(boardCards);
            List<DataTypes.Card> board = GetCardsAccoringStreet(cards, targetStreet);

            List<int> orderedRanks = board.OrderBy(x => x.Rank).Select(x => x.Rank).Distinct().ToList();
            if (orderedRanks.Count < selectedCardTextureList.Count)
                return false;

            foreach (string card in selectedCardTextureList)
                orderedRanks.RemoveAll(x => x == (int)DataTypes.Card.GetCardRank(card));  

            return orderedRanks.Count == 0;
        }

        public static bool BoardContainsAPair(string boardCards, Street targetStreet)
        {
            List<DataTypes.Card> cards = ParseStringSequenceOfCards(boardCards);
            List<DataTypes.Card> board = GetCardsAccoringStreet(cards, targetStreet);
            List<int> orderedRanks = board.OrderBy(x => x.Rank).Select(x => x.Rank).Distinct().ToList();

            return board.Count > orderedRanks.Count;
        }
    }
}
