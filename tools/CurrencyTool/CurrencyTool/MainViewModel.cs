using DriveHUD.Application.Security;
using HandHistories.Objects.GameDescription;
using Prism.Commands;
using Prism.Mvvm;
using ProtoBuf;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Input;

namespace CurrencyTool
{
    public class MainViewModel : BindableBase
    {
        private const string file = "currency.dat";

        private const string privateKey = "<RSAKeyValue><Modulus>11MmRv/7fVASZPbmvUSTliEXHB91+nBB7WZkuW6kOZpKvbrJQYM9Rtu/kDHytIOca/5eYTjDRKuFHJ5W5drvi1SYiq5x68IikPWpQ93OplzDx+I79ua1nxb2Er8/bO28euEIVClYfuIlqfgh7bPeUdrjWTUTrvJ7j7rf3l+nwVU=</Modulus><Exponent>AQAB</Exponent><P>40KQuQyfKCqZg34KCtzNsqz5e7fxRUwHQa4zVXzRBXxHcbahSiZ0RRrEqCDabTSYzWuSrD2ylXc9Sq2RtpDK8w==</P><Q>8o4v7TQOJZ0iXIgqRpHYuLx2p41NMjZlM+Glhpd2h7twWN9FqDt+wtA6SHG6LjGS4jEwX95csQTs3JA+kyrElw==</Q><DP>IaLkkL8Rf3xupEuvaNQtjdiURH+BMmSCXnJOHsUOmuU+HdKOJM90PhYsLUZPjgJO63iUiPHI0N6JL9hozMC5iQ==</DP><DQ>sJ0n+Kg2xNyt8IKVhd0c2Schns8crrs85ZIgXOhcvmiVXaD1r5Hisye7yJRG5Ovj8B+xAZ2AEMVtUw0VA0PY8Q==</DQ><InverseQ>ON7z4x+RormdhcwWw1FAcOEf5cC/irEqpSVvgGErMQPtXAgWuw1L3m8Az9hFTFVdqyM/p980jiniEJ2FP42odQ==</InverseQ><D>DJZAu4/FLLsciChirpeupO0EQ9GW/O/I+s6sfqZ4FSHu0o5zq0+3qke4N/6jeGflIe75p2dBCueX3WdoHwGgVQJDPY0jC8qTcLdWE9eZ2QVLyatREtD1RjVnG2UrJYEx4ZGTWM0Itn4oRc0oVQX4KAsJkkeMyhms0UxDAd3IM9U=</D></RSAKeyValue>";        

        private static Currency[] currencyToIgnore = new[] { Currency.All, Currency.Chips, Currency.PlayMoney };

        public MainViewModel()
        {
            currencyRates = new ObservableCollection<CurrencyViewModel>(
                Enum.GetValues(typeof(Currency)).OfType<Currency>().Where(x => !currencyToIgnore.Contains(x)).Select(x => new CurrencyViewModel(x)));

            Load();

            SaveCommand = new DelegateCommand(() => Save());
        }

        private readonly ObservableCollection<CurrencyViewModel> currencyRates;

        public ObservableCollection<CurrencyViewModel> CurrencyRates
        {
            get
            {
                return currencyRates;
            }
        }

        public ICommand SaveCommand { get; private set; }

        private void Save()
        {
            try
            {
                var currencyRates = new CurrencyRates
                {
                    Rates = CurrencyRates.ToDictionary(x => x.Currency, x => x.Rate)
                };

                var currencyRatesData = Serialize(currencyRates);

                var currencyRatesInfo = new CurrencyRatesInfo
                {
                    Data = currencyRatesData,
                    Sign = CreateSign(currencyRatesData)
                };

                var currencyRatesInfoData = Serialize(currencyRatesInfo);
                var currencyRatesInfoDataBase64 = Convert.ToBase64String(currencyRatesInfoData);

                File.WriteAllText(file, currencyRatesInfoDataBase64);

                Clipboard.SetText(currencyRatesInfoDataBase64);

                MessageBox.Show($"Data has been saved to {file}", "Done!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to save data to {file}: {e}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Load()
        {
            if (!File.Exists(file))
            {
                return;
            }

            try
            {
                var currencyRatesInfoDataBase64 = File.ReadAllText(file);

                var currencyRatesInfoData = Convert.FromBase64String(currencyRatesInfoDataBase64);

                var currencyRatesInfo = Deserialize<CurrencyRatesInfo>(currencyRatesInfoData);

                var currencyRates = Deserialize<CurrencyRates>(currencyRatesInfo.Data);

                foreach (var currencyRate in CurrencyRates)
                {
                    if (!currencyRates.Rates.TryGetValue(currencyRate.Currency, out decimal rate))
                    {
                        continue;
                    }

                    currencyRate.Rate = rate;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Failed to load data from {file}: {e}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private byte[] Serialize<T>(T obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        private T Deserialize<T>(byte[] data)
        {
            using (var memoryStream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(memoryStream);
            }
        }

        private byte[] CreateSign(byte[] data)
        {
            using (var cipher = new RSACryptoServiceProvider(2048))
            {
                cipher.FromXmlString(privateKey);
                var sign = cipher.SignData(data, new SHA256CryptoServiceProvider());
                return sign;
            }            
        }    
    }
}