using Model.Solvers;
using System.Collections.Generic;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class HoldemEvaluator : OmahaEvaluator, IPokerEvaluator
    {
        protected override string GetPlayerBestCards(string playerCards, IComparer<string> comparer)
        {
            // put all cards in one set
            var totalCards = playerCards + " " + cardsOnTable;
            // take all combination 5 of 7
            var totalCombination = GetAllCombinations(totalCards, 5);
            
            var bestCards = GetBestCards(totalCombination, comparer);
        
            return bestCards;
        }
    }
}