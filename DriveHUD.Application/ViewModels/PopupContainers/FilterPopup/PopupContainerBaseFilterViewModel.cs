using DriveHUD.Application.ViewModels.Filters;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.Views;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Win32;
using Model.Enums;
using Model.Filters;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public abstract class PopupContainerBaseFilterViewModel : BaseViewModel, IInteractionRequestAware
    {
        #region Fields

        protected abstract string FilterFileExtension { get; }

        protected abstract FilterServices FilterService { get; }

        public IFilterModelManagerService FilterModelManager
        {
            get { return ServiceLocator.Current.GetInstance<IFilterModelManagerService>(FilterService.ToString()); }
        }

        #endregion

        #region Initialization

        protected virtual void InitializeBindings()
        {
            Ok_CommandClick = new RelayCommand(Ok_OnClick);
            Cancel_CommandClick = new RelayCommand(Cancel_OnClick);
            Apply_CommandClick = new RelayCommand(Apply_OnClick);
            Save_CommandClick = new RelayCommand(Save_OnClick);
            Load_CommandClick = new RelayCommand(Load_OnClick);
            Reset_CommandClick = new RelayCommand(Reset_OnClick);
            ButtonFilterModelSectionRemove_CommandClick = new RelayCommand(ButtonFilterModelSectionRemove_OnClick);
            RadioButtonGroupFilters_CommandClick = new RelayCommand(RadioButtonGroupFilters_OnClick);
            RestoreDefaultFiltersState_Command = new RelayCommand(RestoreDefaultFiltersState);
        }

        protected abstract void InitializeViewModelCollection();

        protected void InitializeViewModel(FilterTuple filterTupleStartup)
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
            else
            {
                this.FilterViewSelectedItem = this.FilterViewCollection.ElementAt(0);
            }

            CurrentlyBuiltFilter = new BuiltFilterModel(FilterModelManager);
            CurrentlyBuiltFilter.BindFilterSectionCollection();
        }

        #endregion

        #region Methods

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

        protected void ResetAllFilters()
        {
            foreach (var model in FilterModelManager.FilterModelCollection)
            {
                model.ResetFilter();
            }
        }

        protected virtual void LoadFilter(string fileName)
        {
            if (File.Exists(fileName))
            {
                var loadedFilter = ServiceLocator.Current.GetInstance<IFilterDataService>().LoadFilter(fileName);

                if (loadedFilter == null)
                {
                    return;
                }

                ApplyLoadedData(fileName, loadedFilter);
            }
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

        private void ViewModelSwitch(FilterTuple filterTuple)
        {
            this.FilterViewSelectedItem = this.FilterViewCollection.Where(x => x.ViewModel.GetType().Name == filterTuple.ViewModelType.ToString()).FirstOrDefault();
        }

        protected virtual Expression<Func<Playerstatistic, bool>> GetCurrentFilter()
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

        #endregion

        #region Properties

        public object _filterViewModelSelectedItem;
        public object FilterViewSelectedItem
        {
            get { return _filterViewModelSelectedItem; }
            set
            {
                _filterViewModelSelectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<IFilterView> _filterViewCollection;
        public ObservableCollection<IFilterView> FilterViewCollection
        {
            get { return _filterViewCollection; }
            set
            {
                _filterViewCollection = value;
                OnPropertyChanged();
            }
        }

        private BuiltFilterModel _currentlyBuiltFilter;
        public BuiltFilterModel CurrentlyBuiltFilter
        {
            get { return _currentlyBuiltFilter; }
            set
            {
                _currentlyBuiltFilter = value;
                OnPropertyChanged();
            }
        }

        private bool _isHoleCardsViewSelected;
        public virtual bool IsHoleCardsViewSelected
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
        protected virtual void RadioButtonGroupFilters_OnClick(object param)
        {
            ViewModelSwitch((FilterTuple)param);
        }

        public ICommand Ok_CommandClick { get; private set; }
        protected virtual void Ok_OnClick()
        {
            Apply_OnClick(null);
            OkInteraction();
        }

        public ICommand Cancel_CommandClick { get; private set; }
        protected virtual void Cancel_OnClick()
        {
            CancelInteraction();
        }

        public ICommand Apply_CommandClick { get; private set; }
        protected abstract void Apply_OnClick(object obj);

        public ICommand Save_CommandClick { get; private set; }
        protected virtual void Save_OnClick()
        {
            if (this.CurrentlyBuiltFilter.FilterSectionCollection.Any(x => x.IsActive))
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filter = $"DriveHUD Filters ({FilterFileExtension})|*{FilterFileExtension}" };
                saveFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

                if (saveFileDialog.ShowDialog() == true)
                {
                    ServiceLocator.Current.GetInstance<IFilterDataService>().SaveFilter(FilterModelManager.FilterModelCollection, saveFileDialog.FileName);
                }
            }
        }

        public ICommand Load_CommandClick { get; private set; }
        protected virtual void Load_OnClick()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog() { Filter = $"DriveHUD Filters ({FilterFileExtension})|*{FilterFileExtension}" };
            openFileDialog.InitialDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");

            if (openFileDialog.ShowDialog() == true)
            {
                LoadFilter(openFileDialog.FileName);
            }
        }

        public ICommand Reset_CommandClick { get; private set; }
        protected virtual void Reset_OnClick()
        {
            ResetAllFilters();
        }

        public ICommand RestoreDefaultFiltersState_Command { get; private set; }
        protected virtual void RestoreDefaultFiltersState()
        {
            foreach (var filter in FilterViewCollection)
            {
                filter.ViewModel.RestoreDefaultState();
            }
        }

        #endregion

        #region IInteractionRequestAware

        private PopupContainerFiltersViewModelNotification _notification;

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

                    InitializeViewModel((value as PopupContainerFiltersViewModelNotification).FilterTuple);
                }
            }
        }

        #endregion
    }
}
