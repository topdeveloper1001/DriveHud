//-----------------------------------------------------------------------
// <copyright file="PopupContainerSettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.PopupContainers.Notifications;
using DriveHUD.Application.ViewModels.Settings;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Events;
using Model.Interfaces;
using Model.Settings;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

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
                    new SettingsGeneralViewModel(name: "General", parent: this),
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
                    SettingsSiteViewModel vm = viewModel as SettingsSiteViewModel;
                    if (vm != null && _preferredSeatingFlag)
                    {
                        vm.SelectedSiteType = EnumPokerSites.Ignition;
                        _preferredSeatingFlag = false;
                        SelectedViewModel = vm;
                    }
                    else
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

            if (isUpdatePlayersRequired && StorageModel.PlayerSelectedItem != null)
            {
                _dataService.SaveActivePlayer(StorageModel.PlayerSelectedItem.Name, (short?)StorageModel.PlayerSelectedItem.PokerSite);
            }

            _settingsService.SaveSettings(_settingsModel);

            if (_settingsModel != null && _settingsModel.GeneralSettings != null &&
                _settingsModel.GeneralSettings.IsAdvancedLoggingEnabled != LogProvider.Log.IsAdvanced)
            {
                LogProvider.Log.IsAdvanced = _settingsModel.GeneralSettings.IsAdvancedLoggingEnabled;
                LogProvider.Log.Info($"Advanced logging: {_settingsModel.GeneralSettings.IsAdvancedLoggingEnabled}");
            }

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

        private bool _preferredSeatingFlag;
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

                    if (_notification.Parameter == "Preferred Seating")
                        _preferredSeatingFlag = true;       //become true when need start setting tab from Bodog/Ignition


                    SwitchView(_notification.Parameter == "Preferred Seating"
                        ? ViewModelCollection.FirstOrDefault(x => x.Name == "Site Settings")
                        : ViewModelCollection.FirstOrDefault());
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
