using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal abstract class PokerEvaluator : IPokerEvaluator
    {
        protected string cardsOnTable;
        protected Dictionary<int, string> playersCards = new Dictionary<int, string>();

        public IEnumerable<int> GetWinners()
        {
            if (playersCards.Count == 0 || string.IsNullOrWhiteSpace(cardsOnTable))
            {
                return new List<int>();
            }

            if (playersCards.Count == 1)
            {
                return new List<int>(playersCards.Keys);
            }
        
            var winners = GetWinnersInternal();

            return winners;
        }

        public void SetCardsOnTable(string cardsOnTable)
        {
            this.cardsOnTable = cardsOnTable;
        }

        public void SetPlayerCards(int seat, string cards)
        {
            if (playersCards.ContainsKey(seat))
            {
                playersCards[seat] = cards;
                return;
            }

            playersCards.Add(seat, cards);
        }

        protected abstract IEnumerable<int> GetWinnersInternal();

        protected List<string> GetAllCombinations(string cardsToCombinate, int k)
        {
            var cards = cardsToCombinate.Split(' ');

            var combinations = new List<string>();

            foreach (var cardCombinations in cards.Combinations(k))
            {
                var cardCombinationsString = string.Join(" ", cardCombinations);
                combinations.Add(cardCombinationsString);
            }

            return combinations;
        }
    }
}