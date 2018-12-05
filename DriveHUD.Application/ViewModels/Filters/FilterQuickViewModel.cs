//-----------------------------------------------------------------------
// <copyright file="FilterQuickViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using Model.Enums;
using Model.Filters;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterQuickViewModel : FilterViewModel<FilterQuickModel>
    {
        internal FilterQuickViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterQuickViewModel, service)
        {
            InitializeBindings();
        }

        private void InitializeBindings()
        {
            ButtonFilterModelStatItemSwap_CommandClick = new RelayCommand(ButtonFilterModelStatItemSwap_OnClick);
        }

        private void FilterModelStatItemSwap(QuickFilterItem param)
        {
            param.TriStateSwap();
        }

        #region Commands

        public ICommand ButtonFilterModelStatItemSwap_CommandClick { get; set; }

        private void ButtonFilterModelStatItemSwap_OnClick(object param)
        {
            FilterModelStatItemSwap((QuickFilterItem)param);
        }

        #endregion
    }
}