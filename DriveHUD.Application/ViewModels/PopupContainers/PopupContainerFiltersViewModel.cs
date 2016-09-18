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
    public class PopupContainerFiltersViewModel : BaseViewModel, IInteractionRequestAware
    {
        public IFilterModelManagerService FilterModelManager
        {
            get { return ServiceLocator.Current.GetInstance<IFilterModelManagerService>(); }
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

        private void InitializeViewModelCollection()
        {
            this.FilterViewCollection = new ObservableCollection<IFilterView>
                (
                    new List<IFilterView>()
                    {
                        new FilterBoardTextureView(),
                        new FilterHandActionView(),
                        new FilterHandValueView(),
                        new FilterQuickView(),
                        new FilterStandardView(),
                        new FilterDateView(),
                        new FilterHandGridView(),
                    });
        }

        private void InitializeViewModel(FilterTuple filterTupleStartup)
        {
            if (this.FilterViewCollection == null)
            {
                InitializeViewModelCollection();
            }
            InitializeData(filterTupleStartup);
        }

        private void InitializeData(FilterTuple filterTupleStartup)
        {
            if (filterTupleStartup != null)
            {
                EnumViewModelType enumViewModelTypeStartup = filterTupleStartup.ViewModelType;

                if (this.FilterViewCollection.Where(x => x.ViewModel.GetType().Name == filterTupleStartup.ViewModelType.ToString()).Any())
                {
                    this.FilterViewSelectedItem = this.FilterViewCollection.Where(x => x.ViewModel.GetType().Name == filterTupleStartup.ViewModelType.ToString()).FirstOrDefault();
                }
                else
                {
                    this.FilterViewSelectedItem = this.FilterViewCollection.ElementAt(0);
                }
            }

            CurrentlyBuiltFilter = new BuiltFilterModel();
            CurrentlyBuiltFilter.BindFilterSectionCollection();
        }

        private void InitializeBindings()
        {
            this.RadioButtonGroupFilters_CommandClick = new RelayCommand(new Action<object>(this.RadioButtonGroupFilters_OnClick));

            this.Ok_CommandClick = new RelayCommand(this.Ok_OnClick);
            this.Cancel_CommandClick = new DelegateCommand(this.Cancel_OnClick);
            this.Apply_CommandClick = new RelayCommand(new Action<object>(this.Apply_OnClick));
            this.Save_CommandClick = new RelayCommand(Save_OnClick);
            this.Load_CommandClick = new RelayCommand(Load_OnClick);
            this.Reset_CommandClick = new RelayCommand(Reset_OnClick);
            this.ButtonFilterModelSectionRemove_CommandClick = new DelegateCommand<object>(this.ButtonFilterModelSectionRemove_OnClick);

            StorageModel.PropertyChanged += StorageModel_PropertyChanged;
        }

        #endregion

        #region Methods
        private void UpdateFilter(UpdateFilterRequestEventArgs obj)
        {
            InitializeViewModel(null);
            FilterViewCollection.ForEach(x => x.ViewModel.InitializeFilterModel());

            Apply_OnClick(null);
        }

        private void ResetAllFilters()
        {
            foreach (var model in FilterModelManager.FilterModelCollection)
            {
                model.ResetFilter();
            }
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

        private void ViewModelSwitch(FilterTuple filterTuple)
        {
            this.FilterViewSelectedItem = this.FilterViewCollection.Where(x => x.ViewModel.GetType().Name == filterTuple.ViewModelType.ToString()).FirstOrDefault();
        }

        /// <summary>
        /// Initializes all startup data.
        /// Fires upon assigning this.Notification from an outside caller
        /// </summary>
        /// <param name="pubSubMessage"></param>
        private void PubSubMessage_Process(PubSubMessage pubSubMessage)
        {
        }

        private void OkInteraction()
        {
            this._notification.Confirmed = true;
            this.FinishInteraction();
        }

        private void CancelInteraction()
        {
            this._notification.Confirmed = false;
            this.FinishInteraction();
        }

        private Expression<Func<Playerstatistic, bool>> GetCurrentFilter()
        {
            var predicate = PredicateBuilder.True<Playerstatistic>();
            if (CurrentlyBuiltFilter != null && CurrentlyBuiltFilter.FilterSectionCollection.Any(x => x.IsActive))
            {
                foreach (var filter in FilterModelManager.FilterModelCollection)
                {
                    var filterPredicate = filter.GetFilterPredicate();
                    if (filterPredicate != null)
                    {
                        predicate = predicate.And(filterPredicate);
                    }
                }

            }
            return predicate;
        }

        private void ApplyLoadedData(string filename, ICollection<IFilterModel> loadedFilter)
        {
            try
            {
                var defaultStateModels = FilterViewCollection.Select(x => x.ViewModel.GetDefaultStateModel() as FilterBaseEntity).Where(x => x != null).ToList();
                var loadedList = loadedFilter.Select(x => x as FilterBaseEntity).Where(x => x != null);
                foreach (var viewModel in FilterViewCollection)
                {
                    var model = viewModel.ViewModel.GetDefaultStateModel() as FilterBaseEntity;
                    if (model == null)
                        continue;

                    var loadedModel = loadedList.FirstOrDefault(x => x.Id == model.Id);
                    if (loadedModel == null)
                        continue;

                    viewModel.ViewModel.UpdateDefaultStateModel(loadedModel);
                }

                RestoreDefaultFiltersState();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, String.Format("Error during applying filter '{0}'", filename), ex);
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

        #endregion

        #region Properties

        private PopupContainerFiltersViewModelNotification _notification;
        private BuiltFilterModel _currentlyBuiltFilter;
        private bool _isHoleCardsViewSelected;

        public ObservableCollection<IFilterView> _filterViewCollection;
        public object _filterViewModelSelectedItem;

        public BuiltFilterModel CurrentlyBuiltFilter
        {
            get { return _currentlyBuiltFilter; }
            set
            {
                _currentlyBuiltFilter = value;
                OnPropertyChanged();
            }
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this._notification; }
            set
            {
                if (value is PopupContainerFiltersViewModelNotification)
                {
                    this._notification = value as PopupContainerFiltersViewModelNotification;
                    this.OnPropertyChanged(() => this.Notification);

                    // Process the supplued PubSubMessage
                    InitializeViewModel((value as PopupContainerFiltersViewModelNotification).FilterTuple);
                }
            }
        }

        public ObservableCollection<IFilterView> FilterViewCollection
        {
            get { return _filterViewCollection; }
            set
            {
                _filterViewCollection = value;
                OnPropertyChanged();
            }
        }

        public object FilterViewSelectedItem
        {
            get { return _filterViewModelSelectedItem; }
            set
            {
                _filterViewModelSelectedItem = value;
                OnPropertyChanged();
            }
        }

        public bool IsHoleCardsViewSelected
        {
            get { return _isHoleCardsViewSelected; }
            set
            {
                (FilterViewCollection.OfType<FilterHandGridView>().FirstOrDefault()?.ViewModel as FilterHandGridViewModel)?.SwitchView(!value);
                SetProperty(ref _isHoleCardsViewSelected, value);
            }
        }
        #endregion

        #region Commands
        public ICommand ButtonFilterModelSectionRemove_CommandClick { get; set; }
        private void ButtonFilterModelSectionRemove_OnClick(object param)
        {
            CurrentlyBuiltFilter.RemoveBuiltFilterItem((FilterSectionItem)param);
        }

        public ICommand RadioButtonGroupFilters_CommandClick { get; private set; }
        private void RadioButtonGroupFilters_OnClick(object param)
        {
            ViewModelSwitch((FilterTuple)param);
        }

        public ICommand Ok_CommandClick { get; private set; }
        private void Ok_OnClick()
        {
            Apply_OnClick(null);
            OkInteraction();
        }

        public ICommand Cancel_CommandClick { get; private set; }
        private void Cancel_OnClick()
        {
            CancelInteraction();
        }

        public ICommand Apply_CommandClick { get; private set; }
        private void Apply_OnClick(object obj)
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

        public ICommand Save_CommandClick { get; private set; }
        private void Save_OnClick()
        {
            if (this.CurrentlyBuiltFilter.FilterSectionCollection.Any(x => x.IsActive))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = "DriveHUD Filters (.df)|*.df" };
                saveFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
                if (saveFileDialog.ShowDialog() == true)
                {
                    ServiceLocator.Current.GetInstance<IFilterDataService>().SaveFilter(FilterModelManager.FilterModelCollection, saveFileDialog.FileName);
                }
            }
        }

        public ICommand Load_CommandClick { get; private set; }
        private void Load_OnClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = "DriveHUD Filters (.df)|*.df" };
            openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (openFileDialog.ShowDialog() == true)
            {
                var loadedFilter = ServiceLocator.Current.GetInstance<IFilterDataService>().LoadFilter(openFileDialog.FileName);

                if (loadedFilter == null)
                {
                    return;
                }

                ApplyLoadedData(openFileDialog.FileName, loadedFilter);
            }
        }

        public ICommand Reset_CommandClick { get; private set; }
        private void Reset_OnClick()
        {
            ResetAllFilters();
        }

        internal void RestoreDefaultFiltersState()
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.RestoreDefaultState();
            }
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
