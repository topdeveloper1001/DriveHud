//-----------------------------------------------------------------------
// <copyright file="OmahaEvaluator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Solvers;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class OmahaEvaluator : PokerEvaluator, IPokerEvaluator
    {
        protected override HandWinners GetWinnersInternal()
        {
            var comparer = new HiCardsComparer();

            var winners = new HandWinners
            {
                Hi = GetWinnersInternal(comparer)
            };

            return winners;
        }

        protected override IEnumerable<int> GetWinnersInternal(ICardsComparer comparer)
        {
            // omaha rules
            // 2 from player cards and 3 from table
            // get all possbile player combinations and get higher of them
            // compare best combinations between players and select the highest             
            var bestCardsByPlayer = playersCards
                .Select(playerCards => new { Seat = playerCards.Key, BestCards = GetPlayerBestCards(playerCards.Value, comparer) })
                .Where(player => player.BestCards != null)
                .ToArray();

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

        protected virtual string GetPlayerBestCards(string playerCards, ICardsComparer comparer)
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

        protected virtual string GetBestCards(IEnumerable<string> totalCombination, ICardsComparer comparer)
        {
            string bestCards = null;

            foreach (var cards in totalCombination)
            {
                if (!comparer.IsValid(cards))
                {
                    continue;
                }

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