//-----------------------------------------------------------------------
// <copyright file="Gametypes.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace DriveHUD.Entities
{
    public partial class Gametypes
    {
        public virtual int GametypeId { get; set; }

        [Required]
        public virtual short PokergametypeId { get; set; }

        [Required]
        public virtual short Tablesize { get; set; }

        [Required]
        public virtual int Smallblindincents { get; set; }

        [Required]
        public virtual short CurrencytypeId { get; set; }

        [Required]
        public virtual int Bigblindincents { get; set; }

        [Required]
        public virtual bool Istourney { get; set; }

        [Required]
        public virtual int Anteincents { get; set; }

        protected bool Equals(Gametypes other)
        {
            return GametypeId == other.GametypeId && PokergametypeId == other.PokergametypeId && Tablesize == other.Tablesize && Smallblindincents == other.Smallblindincents && CurrencytypeId == other.CurrencytypeId && Bigblindincents == other.Bigblindincents && Istourney.Equals(other.Istourney) && Anteincents == other.Anteincents;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = GametypeId;
                hashCode = (hashCode * 397) ^ PokergametypeId.GetHashCode();
                hashCode = (hashCode * 397) ^ Tablesize.GetHashCode();
                hashCode = (hashCode * 397) ^ Smallblindincents;
                hashCode = (hashCode * 397) ^ CurrencytypeId.GetHashCode();
                hashCode = (hashCode * 397) ^ Bigblindincents;
                hashCode = (hashCode * 397) ^ Istourney.GetHashCode();
                hashCode = (hashCode * 397) ^ Anteincents;
                return hashCode;
            }
        }

        public static bool operator ==(Gametypes left, Gametypes right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Gametypes left, Gametypes right)
        {
            return !Equals(left, right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Gametypes)obj);
        }
    }
}