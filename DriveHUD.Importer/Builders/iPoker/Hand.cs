﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class Hand
    {
        private List<Card> cards;
        private Dictionary<int, int> cardsHistogram;
        private HandCombo combination;

        public Hand(List<Card> cards)
        {
            if (cards == null)
            {
                throw new ArgumentNullException("cards");
            }

            if (cards.Count != 5)
            {
                throw new ArgumentException("cards have wrong length, must be 5", "cards");
            }

            this.cards = cards;

            cardsHistogram = (from card in cards
                              group card by card.Rank into grouped
                              let count = grouped.Count()
                              select new { CardRank = grouped.Key, Count = count }).ToDictionary(x => x.CardRank, x => x.Count);

            if (cardsHistogram.Count < 2)
            {
                throw new ArgumentException("cards are wrong combination", "cards");
            }

            AnalyzeHand();
        }

        public List<Card> Cards
        {
            get
            {
                return cards;
            }
        }

        public Dictionary<int, int> CardsHistogram
        {
            get
            {
                return cardsHistogram;
            }
        }

        public HandCombo Combination
        {
            get
            {
                return combination;
            }
        }

        private void AnalyzeHand()
        {
            combination = HandCombo.HighCard;

            // Quads
            if (cardsHistogram.Any(x => x.Value == 4))
            {
                combination = HandCombo.Quads;
                return;
            }

            // Boat & Set
            if (cardsHistogram.Any(x => x.Value == 3))
            {
                if (cardsHistogram.Any(x => x.Value == 2))
                {
                    combination = HandCombo.FullHouse;
                }
                else
                {
                    combination = HandCombo.Set;
                }

                return;
            }

            var orderedCardsHistogram = cardsHistogram
                .OrderByDescending(x => x.Value)
                .Select(x => x.Value)
                .ToArray();

            // Two pair
            if (orderedCardsHistogram.ElementAt(0) == 2)
            {
                if (orderedCardsHistogram.ElementAt(1) == 2)
                {
                    combination = HandCombo.TwoPair;
                }
                else
                {
                    combination = HandCombo.OnePair;
                }

                return;
            }

            // Flush
            var firstCardSuit = cards[0].Suit;

            if (cards.All(x => x.Suit == firstCardSuit))
            {
                combination = HandCombo.Flush;
            }

            // Straight 
            var cardsSorted = cards.Select(x => x.Rank).GroupBy(x => x).Select(x => x.Key).OrderBy(x => x).ToArray();

            if (cardsSorted.Length == 5 && ((cardsSorted.Last() - cardsSorted.First()) == 4) ||
                (cardsSorted.Last() == "A".ConvertCardToRank() && cardsSorted.ElementAt(3) == 5))
            {
                if (combination == HandCombo.Flush)
                {
                    combination = HandCombo.StraightFlush;
                }
                else
                {
                    combination = HandCombo.Straight;

                }

                return;
            }
        }
    }
}