using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using Model.Enums;
using Model.Extensions;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsCurrencyViewModel : SettingsViewModel<CurrencySettingsModel>
    {
        internal SettingsCurrencyViewModel(string name) : base(name)
        {
            Initialize();
        }

        private void Initialize()
        {
            UpdateRatesCommand = new RelayCommand(UpdateRates);

            _isInProgress = false;
        }

        #region Properties
        private bool _isInProgress;
        public bool IsInProgress
        {
            get { return _isInProgress; }
            set
            {
                SetProperty(ref _isInProgress, value);
            }
        }

        public CurrencyRate[] RatesList
        {
            get { return SettingsModel?.Rates; }
            set
            {
                if (SettingsModel != null && SettingsModel.Rates != value)
                {
                    SettingsModel.Rates = value;
                    OnPropertyChanged(nameof(RatesList));
                }
            }
        }

        #endregion

        #region ICommand

        public ICommand UpdateRatesCommand { get; set; }

        #endregion

        #region ICommand Implementation

        private async void UpdateRates(object obj)
        {
            IsInProgress = true;
            try
            {
                await Task.Run(() =>
                {
                    foreach (var rate in RatesList)
                    {
                        if (rate.Currency == EnumCurrency.USD)
                        {
                            continue;
                        }

                        rate.Rate = CurrencyConverter.ConvertCurrency(1, rate.Currency.CurrencyToAPIString(), EnumCurrency.USD.CurrencyToAPIString());
                    }
                });
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            IsInProgress = false;
        }

        #endregion
    }
}
