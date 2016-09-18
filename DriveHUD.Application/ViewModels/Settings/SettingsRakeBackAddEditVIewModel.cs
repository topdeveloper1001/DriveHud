using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsRakeBackAddEditViewModel : BaseViewModel
    {
        internal SettingsRakeBackAddEditViewModel(SettingsRakeBackViewModelInfo<RakeBackModel> info)
        {
            InitializeBindings();
            InitializeData(info);
        }

        private void InitializeBindings()
        {
            OKCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(Cancel);
        }

        internal void InitializeData(SettingsRakeBackViewModelInfo<RakeBackModel> info)
        {
            _infoViewModel = info;
            _settingsModel = _infoViewModel?.Model;

            this.RakeBackName = _settingsModel?.RakeBackName ?? string.Empty;
            this.Player = _settingsModel?.Player ?? string.Empty;
            this.DateBegan = _settingsModel?.DateBegan ?? DateTime.Now;
            this.Percentage = _settingsModel?.Percentage ?? 0m;
        }

        private void SaveChanges()
        {
            bool isAdd = false;
            if (_settingsModel == null)
            {
                isAdd = true;
                _settingsModel = new RakeBackModel();
            }

            _settingsModel.RakeBackName = RakeBackName;
            _settingsModel.Player = Player;
            _settingsModel.DateBegan = DateBegan;
            _settingsModel.Percentage = Percentage;

            if (isAdd)
            {
                _infoViewModel?.Add(_settingsModel);
            }
            else
            {
                _infoViewModel?.Close();
            }
        }

        private void Cancel()
        {
            _infoViewModel?.Close();
        }

        #region Properties
        private SettingsRakeBackViewModelInfo<RakeBackModel> _infoViewModel;
        private RakeBackModel _settingsModel;

        private string _rakeBackName;
        private string _player;
        private decimal _percentage;
        private DateTime _dateBegan;

        public string RakeBackName
        {
            get
            {
                return _rakeBackName;
            }

            set
            {
                SetProperty(ref _rakeBackName, value);
            }
        }

        public string Player
        {
            get
            {
                return _player;
            }

            set
            {
                SetProperty(ref _player, value);
            }
        }

        public decimal Percentage
        {
            get
            {
                return _percentage;
            }

            set
            {
                SetProperty(ref _percentage, value);
            }
        }

        public DateTime DateBegan
        {
            get
            {
                return _dateBegan;
            }

            set
            {
                SetProperty(ref _dateBegan, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion
    }
}
