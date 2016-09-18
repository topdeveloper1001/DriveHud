using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using Model.Enums;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsGeneralViewModel : SettingsViewModel<GeneralSettingsModel>
    {
        #region Constructor

        internal SettingsGeneralViewModel(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            PopupModel = new SettingsPopupViewModelBase();
            PopupModel.InitializeCommands();

            SendLogsCommand = new RelayCommand(SendLogs);
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
                    OnPropertyChanged(nameof(IsApplyFiltersToTournamentsAndCashGames));
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

        #endregion

        #region ICommand Implementation

        private void SendLogs(object obj)
        {
            var viewModel = new SettingsSendLogViewModel();
            PopupModel.OpenPopup(viewModel);
        }

        #endregion
    }
}
