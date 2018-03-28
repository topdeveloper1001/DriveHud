//-----------------------------------------------------------------------
// <copyright file="LoCardsComparer.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.Builders.iPoker
{
    internal class LoCardsComparer : ICardsComparer
    {
        private static readonly int AceRank = "A".ConvertCardToRank();

        public int Compare(string cards1, string cards2)
        {
            if (string.IsNullOrEmpty(cards1))
            {
                throw new ArgumentNullException(nameof(cards1));
            }

            if (string.IsNullOrEmpty(cards2))
            {
                throw new ArgumentNullException(nameof(cards2));
            }

            var hand1 = cards1.GetHandFromString();
            var hand2 = cards2.GetHandFromString();

            if (!IsValid(hand1))
            {
                throw new ArgumentException($"{nameof(cards1)} is invalid.");
            }

            if (!IsValid(hand2))
            {
                throw new ArgumentException($"{nameof(cards2)} is invalid.");
            }

            UpdateAceRank(hand1.CardsHistogram);
            UpdateAceRank(hand2.CardsHistogram);

            var cardRanks1 = hand1.CardsHistogram.Select(x => x.Key).OrderByDescending(x => x).ToArray();
            var cardRanks2 = hand2.CardsHistogram.Select(x => x.Key).OrderByDescending(x => x).ToArray();

            var result = 0;

            for (var i = 0; i < cardRanks1.Length; i++)
            {
                result = cardRanks2[i].CompareTo(cardRanks1[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            return result;
        }

        public bool IsValid(string cards)
        {
            var hand = cards.GetHandFromString();
            return IsValid(hand);
        }

        private bool IsValid(Hand hand)
        {
            // Omaha lo - pairs and sets aren't allowed
            if (hand.CardsHistogram.Count != 5)
            {
                return false;
            }

            var cardsHistogram = hand.CardsHistogram
                .Select(x => x.Key)
                .OrderByDescending(x => x)
                .ToArray();

            // A is the lowest card in terms of Omaha Lo, but rank is the highest, so need to check the 2nd element          
            return cardsHistogram[0] < 9 ||
                cardsHistogram[0] == AceRank && cardsHistogram[1] < 9;
        }

        private void UpdateAceRank(Dictionary<int, int> cardsHistorgam)
        {
            if (!cardsHistorgam.ContainsKey(AceRank))
            {
                return;
            }

            var aceValue = cardsHistorgam[AceRank];

            cardsHistorgam.Remove(AceRank);
            cardsHistorgam.Add(1, aceValue);
        }
    }
}