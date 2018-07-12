//-----------------------------------------------------------------------
// <copyright file="SelectableItemViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;

namespace DriveHUD.Common.Wpf.Mvvm
{
    public class SelectableItemViewModel<T> : ViewModelBase
    {
        private readonly T item;

        public SelectableItemViewModel(T item)
        {
            this.item = item;
        }

        public T Item
        {
            get
            {
                return item;
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
                this.RaiseAndSetIfChanged(ref isSelected, value);
            }
        }
    }
}