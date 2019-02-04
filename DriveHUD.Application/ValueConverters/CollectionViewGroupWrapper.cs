//-----------------------------------------------------------------------
// <copyright file="CollectionViewGroupWrapper.cs" company="Ace Poker Solutions">
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
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class CollectionViewGroupWrapper : BindableBase
    {
        private readonly CollectionViewGroup collectionViewGroup;

        public CollectionViewGroupWrapper(CollectionViewGroup collectionViewGroup)
        {
            this.collectionViewGroup = collectionViewGroup;
        }

        public object Name
        {
            get
            {
                return collectionViewGroup.Name;
            }
        }

        public int ItemCount
        {
            get
            {
                return collectionViewGroup.ItemCount;
            }
        }

        public ReadOnlyObservableCollection<object> Items
        {
            get
            {
                return collectionViewGroup.Items;
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                SetProperty(ref isSelected, value);
            }
        }
    }
}