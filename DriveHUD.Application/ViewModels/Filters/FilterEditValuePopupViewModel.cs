//-----------------------------------------------------------------------
// <copyright file="FilterEditValuePopupViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Model.Filters;
using ReactiveUI;
using System;

namespace DriveHUD.Application.ViewModels.Filters
{
    public class FilterEditValuePopupViewModel : WindowViewModelBase<FilterEditValuePopupViewModel>, IFilterEditValuePopupViewModel
    {
        private FilterAdvancedItem filter;

        private Action saveAction;

        public override void Configure(object viewModelInfo)
        {
            if (!(viewModelInfo is FilterEditValuePopupViewModelInfo filterEditValuePopupViewModelInfo) ||
                filterEditValuePopupViewModelInfo.Filter == null)
            {
                return;
            }

            filter = filterEditValuePopupViewModelInfo.Filter;
            saveAction = filterEditValuePopupViewModelInfo.SaveAction;

            Initialize();
        }

        #region Properties

        private bool percentBase;

        public bool PercentBase
        {
            get
            {
                return percentBase;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref percentBase, value);
            }
        }

        private string filterName;

        public string FilterName
        {
            get
            {
                return filterName;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref filterName, value);
            }
        }

        private double filterValue;

        public double FilterValue
        {
            get
            {
                return filterValue;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filterValue, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand SaveCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        #endregion

        private void Initialize()
        {
            PercentBase = FilterAdvancedModel.PercentBasedFilters.Contains(filter.FilterType);
            FilterName = filter.Name;

            if (filter.FilterValue.HasValue)
            {
                FilterValue = filter.FilterValue.Value;
            }

            InitializeCommands();
            OnInitialized();
        }

        private void InitializeCommands()
        {
            SaveCommand = ReactiveCommand.Create(() =>
            {
                filter.FilterValue = FilterValue;
                saveAction?.Invoke();
                OnClosed();
            });

            CancelCommand = ReactiveCommand.Create(() => OnClosed());
        }
    }
}