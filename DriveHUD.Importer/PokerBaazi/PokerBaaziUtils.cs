//-----------------------------------------------------------------------
// <copyright file="PokerBaaziUtils.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Importers.PokerBaazi.Model;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziUtils
    {
        public static bool TryParseCards(string cardsString, out Card[] cards)
        {
            cards = new Card[0];

            if (string.IsNullOrEmpty(cardsString))
            {
                return false;
            }

            try
            {
                var splitCards = cardsString.Split(',');

                cards = (from cardString in splitCards
                         let card = ParseCard(cardString)
                         where !card.isEmpty
                         select card).ToArray();

                return cards.Length == splitCards.Length;
            }
            catch
            {
                return false;
            }
        }    

        private static Card ParseCard(string cardString)
        {
            var splitCards = cardString.Split('_');

            if ((splitCards.Length != 2) ||
                !int.TryParse(splitCards[1], out int rank))
            {
                return new Card();
            }

            var card = 0;

            switch (splitCards[0][0])
            {
                case 'c':
                    card = 0;
                    break;
                case 'd':
                    card = 13;
                    break;
                case 'h':
                    card = 26;
                    break;
                case 's':
                    card = 39;
                    break;
            }

            card = card + rank - 2;

            return Card.GetCardFromIntValue(card);
        }
    }
}