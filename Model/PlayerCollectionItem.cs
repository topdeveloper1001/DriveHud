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
using Prism.Mvvm;
using System.Collections.Generic;
using System.Web;

namespace Model
{
    public class PlayerCollectionItem : BindableBase, IPlayer
    {
        #region Fields

        public int PlayerId { get; set; }

        public string Name { get; set; }

        public string DecodedName { get { return HttpUtility.HtmlDecode(Name); } }

        public EnumPokerSites PokerSite { get; set; }

        public string Description
        {
            get { return GetShortPokerSiteName(PokerSite); }
        }

        private List<AliasCollectionItem> _linkedAliases = new List<AliasCollectionItem>();

        public List<AliasCollectionItem> LinkedAliases
        {
            get { return _linkedAliases; }
            set { _linkedAliases = value; }
        }

        #endregion

        #region Methods

        public static string GetShortPokerSiteName(EnumPokerSites site)
        {
            // TODO : check it for adding new poker sites

            switch (site)
            {
                case EnumPokerSites.BetOnline:
                    return "BOL";
                case EnumPokerSites.Poker888:
                    return "888";
                case EnumPokerSites.WinningPokerNetwork:
                    return "WPN";
                case EnumPokerSites.AmericasCardroom:
                    return "ACR";
                case EnumPokerSites.BlackChipPoker:
                    return "BCP";
                case EnumPokerSites.TruePoker:
                    return "TP";
                case EnumPokerSites.YaPoker:
                    return "YP";
                default:
                    break;
            }

            return null;
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