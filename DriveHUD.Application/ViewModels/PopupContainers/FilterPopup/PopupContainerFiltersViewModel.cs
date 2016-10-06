using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

using Model.Enums;

using DriveHUD.Application.Models;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;

using DriveHUD.ViewModels;
using DriveHUD.Common.Infrastructure.Base;
using Model;
using Microsoft.Practices.ServiceLocation;
using Model.Filters;
using DriveHUD.Entities;
using DriveHUD.Common.Reflection;
using Prism.Events;
using Model.Events;
using DriveHUD.Common.Utils;
using Model.Extensions;
using System.Linq.Expressions;
using System.Diagnostics;
using Microsoft.Win32;
using DriveHUD.Common.Log;
using Model.Settings;
using DriveHUD.Common.Linq;
using System.Windows.Threading;
using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.Views;
using System.IO;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerFiltersViewModel : PopupContainerBaseFilterViewModel
    {
        protected override string FilterFileExtension
        {
            // drivehud filter
            get { return ".df"; }
        }

        protected override FilterServices FilterService
        {
            get { return FilterServices.Main; }
        }

        #region Constructor

        public PopupContainerFiltersViewModel()
        {
            var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
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
            this.FilterViewCollection = new ObservableCollection<IFilterView>
                (
                    new List<IFilterView>()
                    {
                        new FilterBoardTextureView(FilterModelManager),
                        new FilterHandActionView(FilterModelManager),
                        new FilterHandValueView(FilterModelManager),
                        new FilterQuickView(FilterModelManager),
                        new FilterStandardView(FilterModelManager),
                        new FilterDateView(FilterModelManager),
                        new FilterHandGridView(FilterModelManager),
                    });
        }

        protected override void InitializeBindings()
        {
            base.InitializeBindings();

            StorageModel.PropertyChanged += StorageModel_PropertyChanged;
        }

        #endregion

        #region Methods

        protected override void InitializeData(FilterTuple filterTupleStartup)
        {
            base.InitializeData(filterTupleStartup);

            CurrentlyBuiltFilter = new BuiltFilterModel(FilterService);
            CurrentlyBuiltFilter.BindFilterSectionCollection();
        }

        private void UpdateFilter(UpdateFilterRequestEventArgs obj)
        {
            InitializeViewModel(null);
            FilterViewCollection.ForEach(x => x.ViewModel.InitializeFilterModel());

            Apply_OnClick(null);
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
            Apply_OnClick(null);
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
                if (dateFilter.DateFilterType != EnumDateFiter.ThisWeek)
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
                Apply_OnClick(null);
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

        protected override void Apply_OnClick(object obj)
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.UpdateDefaultState();
            }

            var currentFilter = GetCurrentFilter();
            StorageModel.FilterPredicate = currentFilter;

            var isApplyForBoth = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().GeneralSettings.IsApplyFiltersToTournamentsAndCashGames;
            if (isApplyForBoth)
            {
                FilterModelManager.SpreadFilter();
            }

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<BuiltFilterChangedEvent>().Publish(new BuiltFilterChangedEventArgs(this.CurrentlyBuiltFilter.DeepCloneJson(), currentFilter));
        }

        #endregion

        #region Events

        private void StorageModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ReflectionHelper.GetPath<SingletonStorageModel>(p => p.PlayerSelectedItem))
            {
                InitializeViewModelCollection();
            }
        }

        #endregion
    }
}
