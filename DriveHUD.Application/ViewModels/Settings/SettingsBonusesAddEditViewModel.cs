using DriveHUD.Common.Infrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Settings;
using System.Windows.Input;
using Model;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsBonusesAddEditViewModel : BaseViewModel
    {
        internal SettingsBonusesAddEditViewModel(SettingsRakeBackViewModelInfo<BonusModel> info)
        {
            InitializeBindings();
            InitializeData(info);
        }

        private void InitializeBindings()
        {
            OKCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(Cancel);
        }

        internal void InitializeData(SettingsRakeBackViewModelInfo<BonusModel> info)
        {
            _infoViewModel = info;
            _settingsModel = _infoViewModel?.Model;

            this.BonusName = _settingsModel?.BonusName ?? string.Empty;
            this.Player = StorageModel.PlayerCollection.Where(pl => pl is PlayerCollectionItem).Select(pl => pl as PlayerCollectionItem).FirstOrDefault(pl => pl.DecodedName == (_settingsModel?.Player ?? string.Empty));
            this.Date = _settingsModel?.Date ?? DateTime.Now;
            this.Amount = _settingsModel?.Amount ?? 0m;
        }

        private void SaveChanges()
        {
            bool isAdd = false;
            if (_settingsModel == null)
            {
                isAdd = true;
                _settingsModel = new BonusModel();
            }

            _settingsModel.BonusName = BonusName;
            _settingsModel.Player = Player.DecodedName;
            _settingsModel.Date = Date;
            _settingsModel.Amount = Amount;

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
        private SettingsRakeBackViewModelInfo<BonusModel> _infoViewModel;
        private BonusModel _settingsModel;

        private string _bonusName;
        private PlayerCollectionItem _player;
        private decimal _amount;
        private DateTime _date;

        public string BonusName
        {
            get
            {
                return _bonusName;
            }

            set
            {
                SetProperty(ref _bonusName, value);
            }
        }

        public PlayerCollectionItem Player
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

        public decimal Amount
        {
            get
            {
                return _amount;
            }

            set
            {
                SetProperty(ref _amount, value);
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }

            set
            {
                SetProperty(ref _date, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }

        #endregion
    }
}
