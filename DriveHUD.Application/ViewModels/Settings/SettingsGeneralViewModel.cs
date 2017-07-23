//-----------------------------------------------------------------------
// <copyright file="SettingsGeneralViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.PopupContainers;
using DriveHUD.Common.Infrastructure.Base;
using Model.Settings;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsGeneralViewModel : SettingsViewModel<GeneralSettingsModel>
    {
        private readonly PopupContainerSettingsViewModel parent;

        #region Constructor

        internal SettingsGeneralViewModel(string name, PopupContainerSettingsViewModel parent) : base(name)
        {
            this.parent = parent;
            Initialize();
        }

        private void Initialize()
        {
            PopupModel = new SettingsPopupViewModelBase();
            PopupModel.InitializeCommands();

            SendLogsCommand = new RelayCommand(SendLogs);
            RebuildStatsCommand = new RelayCommand(RebuildStats, x => CanRunStatsCommand());
            RecoverStatsCommand = new RelayCommand(RecoverStats, x => CanRunStatsCommand());

            SettingsSendLogViewModel.OnClose += PopupModel.ClosePopup;

            TimeZones = TimeZoneInfo.GetSystemTimeZones();
            MinOffset = TimeZones?.Min(x => x.BaseUtcOffset).Hours ?? -12;
            MaxOffset = TimeZones?.Max(x => x.BaseUtcOffset).Hours ?? 12;
        }

        #endregion

        #region Properties

        private SettingsPopupViewModelBase _popupModel;
        private ReadOnlyCollection<TimeZoneInfo> _timeZones;
        private int _maxOffset;
        private int _minOffset;

        public bool IsTurnOnAdvancedLogging
        {
            get { return SettingsModel?.IsAdvancedLoggingEnabled ?? false; }
            set
            {
                if (SettingsModel != null && SettingsModel.IsAdvancedLoggingEnabled != value)
                {
                    SettingsModel.IsAdvancedLoggingEnabled = value;
                    OnPropertyChanged(nameof(IsTurnOnAdvancedLogging));
                }
            }
        }

        public bool IsSaveFiltersOnExit
        {
            get { return SettingsModel?.IsSaveFiltersOnExit ?? false; }
            set
            {
                if (SettingsModel != null && SettingsModel.IsSaveFiltersOnExit != value)
                {
                    SettingsModel.IsSaveFiltersOnExit = value;
                    OnPropertyChanged(nameof(IsSaveFiltersOnExit));
                }
            }
        }

        public bool IsApplyFiltersToTournamentsAndCashGames
        {
            get { return SettingsModel?.IsApplyFiltersToTournamentsAndCashGames ?? false; }
            set
            {
                if (SettingsModel != null && SettingsModel.IsApplyFiltersToTournamentsAndCashGames != value)
                {
                    SettingsModel.IsApplyFiltersToTournamentsAndCashGames = value;
                    OnPropertyChanged(nameof(IsApplyFiltersToTournamentsAndCashGames));
                }
            }
        }

        public bool IsAutomaticallyDownloadUpdates
        {
            get { return SettingsModel?.IsAutomaticallyDownloadUpdates ?? false; }
            set
            {
                if (SettingsModel != null && SettingsModel.IsAutomaticallyDownloadUpdates != value)
                {
                    SettingsModel.IsAutomaticallyDownloadUpdates = value;
                    OnPropertyChanged(nameof(IsAutomaticallyDownloadUpdates));
                }
            }
        }

        public bool RunSiteDetection
        {
            get { return SettingsModel?.RunSiteDetection ?? false; }
            set
            {
                if (SettingsModel != null && SettingsModel.RunSiteDetection != value)
                {
                    SettingsModel.RunSiteDetection = value;
                    OnPropertyChanged(nameof(RunSiteDetection));
                }
            }
        }

        public int TimeZoneOffset
        {
            get { return SettingsModel?.TimeZoneOffset ?? 0; }
            set
            {
                if (SettingsModel != null && SettingsModel.TimeZoneOffset != value)
                {
                    SettingsModel.TimeZoneOffset = value;
                    OnPropertyChanged(nameof(TimeZoneOffset));
                }
            }
        }

        public DayOfWeek StartDayOfWeek
        {
            get { return SettingsModel?.StartDayOfWeek ?? DayOfWeek.Monday; }
            set
            {
                if (SettingsModel != null && SettingsModel.StartDayOfWeek != value)
                {
                    SettingsModel.StartDayOfWeek = value;
                    OnPropertyChanged(nameof(StartDayOfWeek));
                }
            }
        }

        public ReadOnlyCollection<TimeZoneInfo> TimeZones
        {
            get { return _timeZones; }
            set
            {
                SetProperty(ref _timeZones, value);
            }
        }

        public int MaxOffset
        {
            get
            {
                return _maxOffset;
            }

            set
            {
                SetProperty(ref _maxOffset, value);
            }
        }

        public int MinOffset
        {
            get
            {
                return _minOffset;
            }

            set
            {
                SetProperty(ref _minOffset, value);
            }
        }

        public SettingsPopupViewModelBase PopupModel
        {
            get
            {
                return _popupModel;
            }

            set
            {
                SetProperty(ref _popupModel, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand SendLogsCommand { get; set; }

        public ICommand RebuildStatsCommand { get; private set; }

        public ICommand RecoverStatsCommand { get; private set; }

        #endregion

        #region ICommand Implementation

        private void SendLogs(object obj)
        {
            var viewModel = new SettingsSendLogViewModel();
            PopupModel.OpenPopup(viewModel);
        }

        private void RebuildStats(object obj)
        {
            var mainViewModel = App.GetMainViewModel();
            mainViewModel?.RebuildStats();
            parent.FinishInteraction?.Invoke();
        }

        private void RecoverStats(object obj)
        {
            var mainViewModel = App.GetMainViewModel();
            mainViewModel?.RecoverStats();
            parent.FinishInteraction?.Invoke();
        }

        private bool CanRunStatsCommand()
        {
            var mainViewModel = App.GetMainViewModel();
            return mainViewModel != null && !mainViewModel.IsHudRunning;
        }

        #endregion
    }
}