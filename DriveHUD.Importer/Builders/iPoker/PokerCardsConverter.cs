using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Builders.iPoker
{
    /// <summary>
    /// iPoker cards converter
    /// </summary>
    public class PokerCardsConverter : ICardsConverter
    {
        public string Convert(string cards)
        {
            if (string.IsNullOrEmpty(cards))
            {
                return cards;
            }

            var cardsArray = cards.Split(' ');

            for (var i = 0; i < cardsArray.Length; i++)
            {
                cardsArray[i] = ConvertCard(cardsArray[i]);
            }

            var convertedCards  = string.Join(" ", cardsArray);

            return convertedCards;
        }

        private string ConvertCard(string card)
        {
            if(string.IsNullOrEmpty(card) || card.Equals("Error") || card.Length < 2)
            {
                return card;
            }

            // make all characters capital
            card = card.ToUpper();

            // swap characters
            var sb = new StringBuilder(card);
            sb.Insert(0, sb[sb.Length - 1]);
            sb.Remove(sb.Length - 1, 1);
            card = sb.ToString();

            // replace ten with 10
            card = card.Replace("T", "10");
                        
            return card;
        }
    }
}