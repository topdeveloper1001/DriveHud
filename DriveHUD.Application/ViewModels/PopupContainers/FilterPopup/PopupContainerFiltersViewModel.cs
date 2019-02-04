//-----------------------------------------------------------------------
// <copyright file="PopupContainerFiltersViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.Views;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Utils;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Events;
using Model.Filters;
using Model.Settings;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerFiltersViewModel : PopupContainerBaseFilterViewModel
    {
        private readonly IEventAggregator eventAggregator;

        protected override string FilterFileExtension
        {
            get { return ".df"; }
        }

        protected override FilterServices FilterService
        {
            get { return FilterServices.Main; }
        }

        #region Constructor

        public PopupContainerFiltersViewModel()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            eventAggregator.GetEvent<DateFilterChangedEvent>().Subscribe(UpdateDateFilter);
            eventAggregator.GetEvent<ResetFiltersEvent>().Subscribe(ResetFilters);
            eventAggregator.GetEvent<SettingsUpdatedEvent>().Subscribe(OnSettingsUpdated);
            eventAggregator.GetEvent<LoadDefaultFilterRequestedEvent>().Subscribe(LoadDefaultFilter);
            eventAggregator.GetEvent<SaveDefaultFilterRequestedEvent>().Subscribe(SaveDefaultFilter);
            eventAggregator.GetEvent<UpdateFilterRequestEvent>().Subscribe(UpdateFilter);

            InitializeBindings();
        }

        protected override void InitializeViewModelCollection()
        {
            if (FilterViewCollection != null)
            {
                return;
            }

            FilterViewCollection = new ObservableCollection<IFilterView>
            {
                new FilterBoardTextureView(FilterModelManager),
                new FilterHandActionView(FilterModelManager),
                new FilterHandValueView(FilterModelManager),
                new FilterQuickView(FilterModelManager),
                new FilterStandardView(FilterModelManager),
                new FilterDateView(FilterModelManager),
                new FilterHandGridView(FilterModelManager),
                new FilterAdvancedView(FilterModelManager)
            };
        }

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            StorageModel.PropertyChanged += OnStorageModelPropertyChanged;
        }

        #endregion

        #region Methods

        private void UpdateFilter(UpdateFilterRequestEventArgs obj)
        {
            InitializeViewModel(null);

            FilterViewCollection.ForEach(x => x.ViewModel.InitializeFilterModel());

            var currentFilter = GetCurrentFilter();

            if (FilterModelManager.FilterType == EnumFilterType.Cash)
            {
                StorageModel.CashFilterPredicate = currentFilter;
            }
            else
            {
                StorageModel.TournamentFilterPredicate = currentFilter;
            }

            var builtFilterArgs = new BuiltFilterChangedEventArgs(CurrentlyBuiltFilter.DeepCloneJson(), currentFilter);
            eventAggregator.GetEvent<BuiltFilterRefreshEvent>().Publish(builtFilterArgs);
        }

        private void ResetFilters(ResetFiltersEventArgs obj)
        {
            if (obj != null && obj.FilterSection != null)
            {
                CurrentlyBuiltFilter.RemoveBuiltFilterItem(obj.FilterSection);
            }
            else
            {
                ResetAllFilters();
            }

            ApplyFilters(null);
        }

        private void OnSettingsUpdated(SettingsUpdatedEventArgs obj)
        {
            if (CurrentlyBuiltFilter?.FilterSectionCollection == null || !CurrentlyBuiltFilter.FilterSectionCollection.Any(x => x.ItemType == EnumFilterSectionItemType.Date && x.IsActive))
            {
                return;
            }

            var dateFilter = FilterModelManager.FilterModelCollection.OfType<FilterDateModel>().FirstOrDefault();

            if (dateFilter != null)
            {
                if (dateFilter.DateFilterType.EnumDateRange != EnumDateFiterStruct.EnumDateFiter.ThisWeek)
                {
                    return;
                }

                var firstDayOfWeek = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.StartDayOfWeek;
                if (firstDayOfWeek != dateFilter.FirstDayOfWeek)
                {
                    UpdateDateFilter(new DateFilterChangedEventArgs(dateFilter.DateFilterType));
                }
            }
        }

        private void UpdateDateFilter(DateFilterChangedEventArgs obj)
        {
            if (CurrentlyBuiltFilter == null || FilterViewCollection == null)
            {
                InitializeViewModel(null);
            }

            var dateFilter = FilterModelManager.FilterModelCollection.OfType<FilterDateModel>().FirstOrDefault();

            if (dateFilter != null)
            {
                dateFilter.DateFilterType = obj.DateFilterType;
                ApplyFilters(null);
            }
        }

        private void LoadDefaultFilter(LoadDefaultFilterRequestedEventArgs obj)
        {
            ServiceLocator.Current.GetInstance<IFilterDataService>().LoadDefaultFilter(FilterModelManager.GetFilterModelDictionary());
        }

        private void SaveDefaultFilter(SaveDefaultFilterRequestedEvetnArgs obj)
        {
            var filterDataService = ServiceLocator.Current.GetInstance<IFilterDataService>();
            filterDataService.SaveDefaultFilter(FilterModelManager.GetFilterModelDictionary());
        }

        protected override void ApplyFilters(object obj)
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.UpdateDefaultState();
            }

            var currentFilter = GetCurrentFilter();

            var builtFilterArgs = new BuiltFilterChangedEventArgs(CurrentlyBuiltFilter.DeepCloneJson(), currentFilter);

            var isApplyForBoth = ServiceLocator.Current.GetInstance<ISettingsService>()
                .GetSettings()
                .GeneralSettings
                .IsApplyFiltersToTournamentsAndCashGames;

            if (isApplyForBoth)
            {
                StorageModel.CashFilterPredicate = currentFilter;
                StorageModel.TournamentFilterPredicate = currentFilter;

                builtFilterArgs.AffectedFilter = new[] { EnumFilterType.Cash, EnumFilterType.Tournament };

                FilterModelManager.SpreadFilter();
            }
            else if (FilterModelManager.FilterType == EnumFilterType.Cash)
            {
                StorageModel.CashFilterPredicate = currentFilter;
                builtFilterArgs.AffectedFilter = new[] { EnumFilterType.Cash };
            }
            else
            {
                StorageModel.TournamentFilterPredicate = currentFilter;
                builtFilterArgs.AffectedFilter = new[] { EnumFilterType.Tournament };
            }

            eventAggregator
                .GetEvent<BuiltFilterChangedEvent>()
                .Publish(builtFilterArgs);
        }

        #endregion

        private void OnStorageModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SingletonStorageModel.PlayerSelectedItem))
            {
                InitializeViewModelCollection();
            }
        }
    }
}