using Model.Solvers;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class OmahaEvaluator : PokerEvaluator, IPokerEvaluator
    {
        protected override IEnumerable<int> GetWinnersInternal()
        {
            // omaha rules
            // 2 from player cards and 3 from table
            // get all possbile player combinations and get higher of them
            // compare best combinations between players and select the highest 

            var comparer = new HiCardsComparer();

            var bestCardsByPlayer = (from playerCards in playersCards
                                     select new { Seat = playerCards.Key, BestCards = GetPlayerBestCards(playerCards.Value, comparer) }).ToArray();

            var bestPlayers = new List<int>();

            int? bestPlayerSeat = null;
            string bestPlayerCards = null;

            foreach (var bestCards in bestCardsByPlayer)
            {
                if (bestPlayerSeat == null)
                {
                    bestPlayerSeat = bestCards.Seat;
                    bestPlayerCards = bestCards.BestCards;
                    bestPlayers.Add(bestCards.Seat);
                    continue;
                }

                // compare player best cards to current best cards
                var compareResult = comparer.Compare(bestCards.BestCards, bestPlayerCards);

                // if player cards are better then clear all best players and reset best cards and seat
                if (compareResult > 0)
                {
                    bestPlayers.Clear();
                    bestPlayerSeat = bestCards.Seat;
                    bestPlayerCards = bestCards.BestCards;
                    bestPlayers.Add(bestCards.Seat);
                }
                else if (compareResult == 0)
                {
                    bestPlayers.Add(bestCards.Seat);
                }
            }

            return bestPlayers;
        }

        protected virtual string GetPlayerBestCards(string playerCards, IComparer<string> comparer)
        {
            // get 2 of players cards
            var playerCardsCombination = GetAllCombinations(playerCards, 2);
            // get 3 of table cards
            var tableCardsCombination = GetAllCombinations(cardsOnTable, 3);

            var totalCombination = new List<string>();

            foreach (var cards in playerCardsCombination)
            {
                foreach (var tableCards in tableCardsCombination)
                {
                    var combination = cards + " " + tableCards;
                    totalCombination.Add(combination);
                }
            }

            var bestCards = GetBestCards(totalCombination, comparer);

            return bestCards;
        }

        protected virtual string GetBestCards(IEnumerable<string> totalCombination, IComparer<string> comparer)
        {
            string bestCards = null;

            foreach (var cards in totalCombination)
            {
                if (bestCards == null)
                {
                    bestCards = cards;
                    continue;
                }

                var compareResult = comparer.Compare(cards, bestCards);

                if (compareResult > 0)
                {
                    bestCards = cards;
                }
            }

            return bestCards;
        }
    }
}