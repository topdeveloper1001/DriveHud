//-----------------------------------------------------------------------
// <copyright file="ICurrencyRatesService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Web;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DriveHUD.Application.Security
{
    internal class CurrencyRatesService : ICurrencyRatesService
    {
        private const string publicKey = "<RSAKeyValue><Modulus>11MmRv/7fVASZPbmvUSTliEXHB91+nBB7WZkuW6kOZpKvbrJQYM9Rtu/kDHytIOca/5eYTjDRKuFHJ5W5drvi1SYiq5x68IikPWpQ93OplzDx+I79ua1nxb2Er8/bO28euEIVClYfuIlqfgh7bPeUdrjWTUTrvJ7j7rf3l+nwVU=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

        public CurrencyRates GetCurrencyRates()
        {
            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var currencyRatesDataBase64 = settingsService.GetSettings()?.GeneralSettings?.CRates;

            return GetCurrencyRates(currencyRatesDataBase64);
        }

        public void RefreshCurrencyRates()
        {
            var currencyUrl = CommonResourceManager.Instance.GetResourceString("SystemSettings_CurrencyRates");

            Task.Run(() =>
            {
                try
                {
                    using (var webClient = new WebClientWithTimeout())
                    {
                        var currencyRatesDataBase64 = webClient.DownloadString(currencyUrl).Trim();

                        var currencyRates = GetCurrencyRates(currencyRatesDataBase64);

                        if (currencyRates == null)
                        {
                            LogProvider.Log.Warn(this, "Failed to obtain currency rates from server.");
                            return;
                        }

                        var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
                        var settingsModel = settingsService.GetSettings();
                        var generalSettings = settingsModel?.GeneralSettings;

                        if (generalSettings == null)
                        {
                            LogProvider.Log.Warn(this, "Failed to find general settings.");
                            return;
                        }

                        if (generalSettings.CRates.Equals(currencyRatesDataBase64, StringComparison.Ordinal))
                        {
                            return;
                        }

                        generalSettings.CRates = currencyRatesDataBase64;
                        settingsService.SaveSettings(settingsModel);
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Failed to refresh currency rates.", e);
                }
            });
        }

        public CurrencyRates GetCurrencyRates(string currencyRatesDataBase64)
        {
            if (string.IsNullOrEmpty(currencyRatesDataBase64))
            {
                LogProvider.Log.Error(this, "Currency rates could not be obtained due to empty data.");
                return null;
            }

            try
            {
                var currencyRatesData = Convert.FromBase64String(currencyRatesDataBase64);

                if (!SerializationHelper.TryDeserialize(currencyRatesData, out CurrencyRatesInfo currencyRatesInfo))
                {
                    LogProvider.Log.Error(this, $"Failed to deserialize currency rates from settings.");
                    return null;
                }

                if (!ValidateSign(currencyRatesInfo.Data, currencyRatesInfo.Sign))
                {
                    LogProvider.Log.Error(this, $"Currency rate data doesn't match sign.");
                    return null;
                }

                if (!SerializationHelper.TryDeserialize(currencyRatesInfo.Data, out CurrencyRates currencyRates))
                {
                    LogProvider.Log.Error(this, $"Failed to deserialize currency rates from data.");
                    return null;
                }

                return currencyRates;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to obtain currency rates from setting.", e);
                return null;
            }
        }

        private bool ValidateSign(byte[] data, byte[] signature)
        {
            using (var cipher = new RSACryptoServiceProvider(2048))
            {
                cipher.FromXmlString(publicKey);

                var result = cipher.VerifyData(data, new SHA256CryptoServiceProvider(), signature);
                return result;
            }
        }
    }
}