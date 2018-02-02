//-----------------------------------------------------------------------
// <copyright file="ComparableCard.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using ProtoBuf;
using System;

namespace Model.Extensions
{
    [ProtoContract]
    public sealed class ComparableCard : BindableBase, IComparable<ComparableCard>
    {
        private ComparableCard()
        {
        }

        public ComparableCard(string cards)
        {
            Cards = cards;
        }

        [ProtoMember(1)]
        private string cards;

        public string Cards
        {
            get
            {
                return cards;
            }
            private set
            {
                SetProperty(ref cards, value);
            }
        }

        public override string ToString()
        {
            return Cards;
        }

        public int CompareTo(ComparableCard other)
        {
            if (other == null)
            {
                return 1;
            }

            var currentCards = CardHelper.Split(Cards);
            var otherCards = CardHelper.Split(other.Cards);

            if (currentCards.Count != otherCards.Count)
            {
                return currentCards.Count - otherCards.Count;
            }

            for (int i = 0; i < currentCards.Count; i++)
            {
                int result = CardHelper.GetCardRank(currentCards[i]) - CardHelper.GetCardRank(otherCards[i]);

                if (result != 0)
                {
                    return result;
                }
            }

            return 0;
        }
    }
}