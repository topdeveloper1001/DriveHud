//-----------------------------------------------------------------------
// <copyright file="HoleCardsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes;
using ReactiveUI;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class HoleCardsViewModel : ReactiveObject
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }

        }

        private RangeSelectorItemType itemType;

        public RangeSelectorItemType ItemType
        {
            get
            {
                return itemType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref itemType, value);
            }
        }

        private bool isChecked;

        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isChecked, value);
            }
        }
    }
}