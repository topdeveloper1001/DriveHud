//-----------------------------------------------------------------------
// <copyright file="CardInfo.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Importers.GGNetwork.Model
{
    public class CardInfo : IEquatable<CardInfo>
    {
        public CardInfo()
        {
        }

        public CardInfo(int ordinal, string rank, CardKind suit)
        {
            if (rank == null)
            {
                throw new ArgumentNullException(nameof(rank));
            }

            Ordinal = ordinal;
            Rank = rank;
            Suit = suit;
        }

        public int Ordinal { get; set; }

        public string Rank { get; set; }

        public CardKind Suit { get; set; }

        public bool Equals(CardInfo other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Ordinal == other.Ordinal &&
                   string.Equals(Rank, other.Rank, StringComparison.OrdinalIgnoreCase) && Suit == other.Suit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((CardInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Ordinal;

                hashCode = (hashCode * 397) ^
                           (Rank != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(Rank) : 0);

                hashCode = (hashCode * 397) ^ (int)Suit;

                return hashCode;
            }
        }

        public static bool operator ==(CardInfo left, CardInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CardInfo left, CardInfo right)
        {
            return !Equals(left, right);
        }
    }
}