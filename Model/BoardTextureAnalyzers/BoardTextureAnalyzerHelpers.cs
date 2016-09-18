using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cards = HandHistories.Objects.Cards;

namespace Model.BoardTextureAnalyzers
{
    public static class BoardTextureAnalyzerHelpers
    {
        public static List<int> GetOrderedBoardNumericRanks(BoardCards boardCards, StraightBoardTextureItem boardTextureItem)
        {
            var board = boardCards.GetBoardOnStreet(boardTextureItem.TargetStreet).OrderBy(x => x.RankNumericValue).ToList();
            var orderedRanks = board.Select(x => x.RankNumericValue).Distinct().ToList();
            if (board.Any(x => x.RankNumericValue == Cards.Card.GetRankNumericValue("A")))
            {
                orderedRanks.Insert(0, 1);
            }

            return orderedRanks;
        }

        public static bool CheckEquality(EnumEquality equality, int firstValue, int secondValue)
        {
            switch (equality)
            {
                case EnumEquality.EqualTo:
                    return firstValue == secondValue;
                case EnumEquality.GreaterThan:
                    return firstValue > secondValue;
                case EnumEquality.LessThan:
                    return firstValue < secondValue;
            }

            return false;
        }

        public static int GetHighestUniqueRankNumberForList(List<int> ranks, int maxValue)
        {
            int aceRank = Cards.Card.GetRankNumericValue(Cards.Card.PossibleRanksHighCardFirst.First());
            int result = maxValue > aceRank ? aceRank : maxValue;
            while (ranks.Contains(result))
            {
                result--;
            }

            return result;
        }
    }
}
