using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HandHistories.Objects.Cards;
using Model.Enums;
using Model.Extensions;
using Model.Filters;
using Model.HandAnalyzers;

namespace Model.BoardTextureAnalyzers
{
    public class ExactCardsTextureAnalyzer
    {
        public bool Analyze(string boardCardsString, IEnumerable<BoardCardItem> targetCards)
        {
            if(string.IsNullOrEmpty(boardCardsString))
            {
                return false;
            }

            var board = BoardCards.FromCards(boardCardsString);
            List<BoardCardItem> boardCards = new List<BoardCardItem>();
            foreach(var card in board)
            {
                boardCards.Add(new BoardCardItem() { Suit = new RangeCardSuit().StringToSuit(card.Suit), Rank = new RangeCardRank().StringToRank(card.Rank) });
            }

            if (boardCards == null || boardCards.Count() == 0 || targetCards == null || targetCards.Count() == 0 || boardCards.Count() < targetCards.Count())
            {
                return false;
            }

            int cardsCount = CardHelper.GetCardsAmountForStreet(targetCards.First().TargetStreet);
            var boardList = boardCards.Take(cardsCount).ToList();
            foreach(var card in targetCards.Where(x=> x.Rank != RangeCardRank.None))
            {
                var boardCard = boardList.FirstOrDefault(x => x.Rank == card.Rank && x.Suit == card.Suit);
                if(boardCard == null)
                {
                    return false;
                }

                boardList.Remove(boardCard);
            }

            foreach(var card in targetCards.Where(x => x.Rank == RangeCardRank.None))
            {
                var boardCard = boardList.FirstOrDefault(x => x.Suit == card.Suit);
                if (boardCard == null)
                {
                    return false;
                }

                boardList.Remove(boardCard);
            }

            return true;
        }

        public BoardTextures GetRank()
        {
            return BoardTextures.ExactCards;
        }
    }
}
