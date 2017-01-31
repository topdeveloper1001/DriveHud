//-----------------------------------------------------------------------
// <copyright file="AppStorePageViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using ReactiveUI;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class AppStorePageViewModel : ReactiveObject
    {
        public AppStorePageViewModel(int pageNumber)
        {
            this.pageNumber = pageNumber;
        }

        private int pageNumber;

        public int PageNumber
        {
            get
            {
                return pageNumber;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref pageNumber, value);
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