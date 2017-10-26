//-----------------------------------------------------------------------
// <copyright file="SetFilterValueViewModel.cs" company="Ace Poker Solutions">
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
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.ViewModels.PopupViewModels
{
    public class SetFilterValueViewModel : ReactiveObject, IPopupInteractionAware
    {
        private Dictionary<FilterEnum, string> filtersDictionary;

        public SetFilterValueViewModel()
        {
            filtersDictionary = FiltersHelper.GetFiltersObjects().ToDictionary(x => x.Filter, x => x.Description);

            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(x =>
            {
                OnSaveAction?.Invoke();
                FinishInteraction?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                OnCancelAction?.Invoke();
                FinishInteraction?.Invoke();
            });
        }

        private FilterEnum filter;

        public FilterEnum Filter
        {
            get
            {
                return filter;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filter, value);
                FilterName = filtersDictionary.ContainsKey(filter) ? filtersDictionary[filter] : string.Empty;
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

        public bool PercentBase
        {
            get
            {
                return FiltersHelper.PercentBasedFilters.Contains(filter);
            }
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

        public Action FinishInteraction
        {
            get;
            set;
        }

        public Action OnSaveAction
        {
            get;
            set;
        }

        public Action OnCancelAction
        {
            get;
            set;
        }
    }
}