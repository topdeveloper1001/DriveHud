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

using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Web;

namespace Model
{
    public class PlayerCollectionItem : BindableBase, IPlayer
    {
        #region Fields

        public int PlayerId { get; set; }

        public IEnumerable<int> PlayerIds
        {
            get
            {
                return new[] { PlayerId };
            }
        }

        public string Name { get; set; }

        public string DecodedName
        {
            get
            {
                return HttpUtility.HtmlDecode(Name);
            }
        }

        public EnumPokerSites? PokerSite { get; set; }

        public IEnumerable<EnumPokerSites> PokerSites
        {
            get
            {
                return PokerSite.HasValue ?
                    new[] { PokerSite.Value } :
                    new EnumPokerSites[0];
            }
        }

        public string ShortDescription
        {
            get
            {
                return PokerSite.ToShortPokerSiteName();
            }
        }

        public string Description
        {
            get
            {
                return PokerSite.HasValue ?
                    CommonResourceManager.Instance.GetEnumResource(PokerSite.Value) :
                    CommonResourceManager.Instance.GetResourceString("Common_Alias");
            }
        }

        private List<AliasCollectionItem> _linkedAliases = new List<AliasCollectionItem>();

        public List<AliasCollectionItem> LinkedAliases
        {
            get
            {
                return _linkedAliases;
            }
        }

        #endregion

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
            return (x?.PlayerId ?? -1) == (y?.PlayerId ?? -1);
        }

        public static bool operator !=(PlayerCollectionItem x, PlayerCollectionItem y)
        {
            return !(x == y);
        }
    }
}