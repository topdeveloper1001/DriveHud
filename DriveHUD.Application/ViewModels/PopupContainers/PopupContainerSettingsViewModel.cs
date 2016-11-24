using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Prism.Commands;
using Prism.Interactivity.InteractionRequest;

using DriveHUD.Application.Models;
using DriveHUD.Application.ViewModels.PopupContainers.Notifications;

using DriveHUD.ViewModels;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Application.ViewModels.Settings;
using Model.Enums;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using Model.Settings;
using Prism.Events;
using Model.Events;
using Model.Interfaces;

namespace DriveHUD.Application.ViewModels.PopupContainers
{
    public class PopupContainerSettingsViewModel : BaseViewModel, IInteractionRequestAware
    {
        private ISettingsService _settingsService
        {
            get { return ServiceLocator.Current.GetInstance<ISettingsService>(); }
        }

        private IDataService _dataService
        {
            get { return ServiceLocator.Current.GetInstance<IDataService>(); }
        }

        private SettingsModel _settingsModel;

        #region Constructor

        public PopupContainerSettingsViewModel()
        {
            Initialize();
        }

        private void Initialize()
        {
            SwitchViewCommand = new RelayCommand(SwitchView);
            ApplyCommand = new RelayCommand(Apply);
            OKCommand = new RelayCommand(OK);
        }

        private void InitializeViewModelCollection()
        {
            if (ViewModelCollection == null)
            {
                ViewModelCollection = new List<ISettingsViewModel>()
                {
                    new SettingsGeneralViewModel(name: "General"),
                    new SettingsSiteViewModel(name: "Site Settings"),
                    new SettingsCurrencyViewModel(name: "Currency"),
                    new SettingsRakeBackViewModel(name: "RakeBack"),
                    new SettingsUpgradeViewModel(name: "Upgrade"),
                    new SettingsSupportViewModel(name: "Support"),
                };
            }
        }

        #endregion

        #region Methods
        private void SwitchView(object obj)
        {
            if (obj != null)
            {
                var viewModel = ViewModelCollection.FirstOrDefault(x => x == obj);
                if (viewModel != null)
                {
                    SelectedViewModel = viewModel;
                }
            }
        }

        private void LoadSettings()
        {
            _settingsModel = _settingsService.GetSettings();

            ViewModelCollection.OfType<SettingsGeneralViewModel>().FirstOrDefault()?.SetSettingsModel(_settingsModel?.GeneralSettings);
            ViewModelCollection.OfType<SettingsCurrencyViewModel>().FirstOrDefault()?.SetSettingsModel(_settingsModel?.CurrencySettings);
            ViewModelCollection.OfType<SettingsRakeBackViewModel>().FirstOrDefault()?.SetSettingsModel(_settingsModel?.RakeBackSettings);
            ViewModelCollection.OfType<SettingsSiteViewModel>().FirstOrDefault()?.SetSettingsModel(_settingsModel?.SiteSettings);
        }

        private void Apply(object obj)
        {
            var oldSettings = _settingsService.GetSettings();
            var isUpdatePlayersRequired = oldSettings?.SiteSettings.IsProcessedDataLocationEnabled != _settingsModel?.SiteSettings.IsProcessedDataLocationEnabled
                || (oldSettings?.SiteSettings.ProcessedDataLocation != _settingsModel?.SiteSettings.ProcessedDataLocation
                    && (_settingsModel?.SiteSettings.IsProcessedDataLocationEnabled ?? false));

            if (isUpdatePlayersRequired)
            {
                _dataService.SaveActivePlayer(StorageModel.PlayerSelectedItem.Name, (short)StorageModel.PlayerSelectedItem.PokerSite);
            }

            _settingsService.SaveSettings(_settingsModel);

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<SettingsUpdatedEvent>()
                .Publish(new SettingsUpdatedEventArgs() { IsUpdatePlayersCollection = isUpdatePlayersRequired });
        }

        private void OK(object obj)
        {
            Apply(null);
            FinishInteraction.Invoke();
        }
        #endregion

        #region Properties

        private PopupContainerSettingsViewModelNotification _notification;
        private IEnumerable<ISettingsViewModel> _viewModelCollection;
        private object _selectedViewModel;

        public object SelectedViewModel
        {
            get { return _selectedViewModel; }
            set { SetProperty(ref _selectedViewModel, value); }
        }

        public IEnumerable<ISettingsViewModel> ViewModelCollection
        {
            get { return _viewModelCollection; }
            set { SetProperty(ref _viewModelCollection, value); }
        }

        public Action FinishInteraction { get; set; }

        public INotification Notification
        {
            get { return this._notification; }
            set
            {
                if (value is PopupContainerSettingsViewModelNotification)
                {
                    this._notification = value as PopupContainerSettingsViewModelNotification;
                    this.OnPropertyChanged(() => this.Notification);

                    InitializeViewModelCollection();
                    LoadSettings();
                    SwitchView(ViewModelCollection.FirstOrDefault());
                }
            }
        }

        #endregion

        #region Commands
        public ICommand SwitchViewCommand { get; set; }
        public ICommand ApplyCommand { get; set; }
        public ICommand OKCommand { get; set; }
        #endregion
    }
}
