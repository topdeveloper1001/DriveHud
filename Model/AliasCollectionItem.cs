//-----------------------------------------------------------------------
// <copyright file="AliasCollectionItem.cs" company="Ace Poker Solutions">
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
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Model
{
    public class AliasCollectionItem : BindableBase, IPlayer
    {
        #region Fields

        private string name = string.Empty;

        private ObservableCollection<PlayerCollectionItem> playersInAlias = new ObservableCollection<PlayerCollectionItem>();

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name == value)
                {
                    return;
                }

                name = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(DecodedName));
            }
        }

        public string DecodedName
        {
            get
            {
                return Name;
            }
        }

        public int PlayerId { get; set; }

        public IEnumerable<int> PlayerIds
        {
            get
            {
                return PlayersInAlias != null ?
                   PlayersInAlias
                   .Select(x => x.PlayerId)
                   .ToArray() :
                   new int[0];
            }
        }

        public EnumPokerSites? PokerSite
        {
            get
            {
                return null;
            }
        }

        public IEnumerable<EnumPokerSites> PokerSites
        {
            get
            {
                return PlayersInAlias != null ?
                    PlayersInAlias
                    .Where(x => x.PokerSite.HasValue)
                    .Select(x => x.PokerSite.Value)
                    .ToArray() :
                    new EnumPokerSites[0];
            }
        }

        public string ShortDescription
        {
            get
            {
                return CommonResourceManager.Instance.GetResourceString("Common_Alias");
            }
        }

        public string Description
        {
            get
            {
                return string.Format(CommonResourceManager.Instance.GetResourceString("Common_Alias_Players"), playersInAlias.Count);
            }
        }

        public ObservableCollection<PlayerCollectionItem> PlayersInAlias
        {
            get
            {
                return playersInAlias;
            }
            set
            {
                if (ReferenceEquals(playersInAlias, value))
                {
                    return;
                }

                playersInAlias = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        #endregion
    }
}