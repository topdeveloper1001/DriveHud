//-----------------------------------------------------------------------
// <copyright file="PokerEvaluator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriveHUD.Common.Extensions;
using HandHistories.Objects.Cards;
using Model.Solvers;

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

        public void SetCardsOnTable(BoardCards cardsOnTable)
        {
            this.cardsOnTable = ConvertCards(cardsOnTable);
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

        public void SetPlayerCards(int seat, HoleCards cards)
        {
            SetPlayerCards(seat, ConvertCards(cards));
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

        protected string ConvertCards(CardGroup cards)
        {
            return string.Join(" ", cards.Select(card => card.ToString().ToUpper().Reverse().Replace("T", "10")));
        }
    }
}