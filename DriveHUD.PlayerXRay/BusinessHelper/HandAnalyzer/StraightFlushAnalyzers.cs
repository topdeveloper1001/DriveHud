using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.PlayerXRay.BusinessHelper.TextureHelpers;
using DriveHUD.PlayerXRay.DataTypes;
using HandHistories.Objects.Cards;

namespace DriveHUD.PlayerXRay.BusinessHelper.HandAnalyzer
{
    public class StraightFlushTwoPocketCardsAnalyzer
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 3)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.Rank);

            for (int i = 0; i < sortedBoard.Count() - 3; i++)
            {
                var cards = sortedBoard.Take(3).ToList();

                if (cards.Any(x => x.CardSuit != playerCards.First().CardSuit))
                {
                    return false;
                }

                cards.AddRange(playerCards);
                if (HandAnalyzerHelpers.IsStraight(cards, true))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class StraightFlushOneHoleCardAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (playerCards == null || boardCards == null || playerCards.Count() != 2 || boardCards.Count() < 4)
                return false;

            if (!HandAnalyzerHelpers.IsFlush(playerCards))
            {
                return false;
            }

            var sortedBoard = boardCards.OrderBy(x => x.Rank);

            for (int i = 0; i < sortedBoard.Count() - 4; i++)
            {
                var cards = sortedBoard.Take(4).ToList();

                if (!HandAnalyzerHelpers.IsFlush(cards))
                {
                    return false;
                }

                foreach (var card in playerCards)
                {
                    var straightFlushCards = new List<DataTypes.Card>(cards);
                    straightFlushCards.Add(card);
                    if (HandAnalyzerHelpers.IsFlush(straightFlushCards) && HandAnalyzerHelpers.IsStraight(straightFlushCards, true))
                    {
                        return true;
                    }
                }
            }

            return false;
        }    
    }

    public class StraightFlushOnBoardAnalyzer 
    {
        public bool Analyze(List<DataTypes.Card> playerCards, List<DataTypes.Card> allBoardCards, Street targetStreet)
        {
            List<DataTypes.Card> boardCards = BoardTextureAnalyzerHelpers.GetCardsAccoringStreet(allBoardCards, targetStreet);

            if (boardCards == null || boardCards.Count() == 0)
                return false;

            if (HandAnalyzerHelpers.IsFlush(boardCards) && HandAnalyzerHelpers.IsStraight(boardCards, true))
            {
                return true;
            }
            return false;
        }      
    }
}
