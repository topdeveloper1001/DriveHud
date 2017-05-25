//-----------------------------------------------------------------------
// <copyright file="PlayerCollectionItem.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using System.Web;

namespace Model
{
    public struct PlayerCollectionItem
    {
        public int PlayerId { get; set; }

        public string Name { get; set; }

        public EnumPokerSites PokerSite { get; set; }

        public string DecodedName
        {
            get
            {
                return HttpUtility.HtmlDecode(Name);
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerCollectionItem && this == (PlayerCollectionItem)obj;
        }

        public override int GetHashCode()
        {
            return PlayerId.GetHashCode();
        }

        public static bool operator ==(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return x.PlayerId == y.PlayerId;
        }

        public static bool operator !=(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return !(x == y);
        }
    }
}