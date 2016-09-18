using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Model.Settings
{
    [Serializable]
    public class CurrencySettingsModel : SettingsBase
    {
        [XmlElement]
        public CurrencyRate[] Rates { get; set; }

        public CurrencySettingsModel()
        {
            Rates = new CurrencyRate[]
            {
                new CurrencyRate() { Name = "USD", Currency = EnumCurrency.USD, Rate = 1m },
                new CurrencyRate() { Name = "EUR", Currency = EnumCurrency.EUR, Rate = 1.11385m },
                new CurrencyRate() { Name = "CAN", Currency = EnumCurrency.CAN, Rate = 0.774593m },
                new CurrencyRate() { Name = "YUAN", Currency = EnumCurrency.CNY, Rate = 0.150551m },
                new CurrencyRate() { Name = "SEK", Currency = EnumCurrency.SEK, Rate =  0.118549m },
            };
        }

        public override object Clone()
        {
            var model = (CurrencySettingsModel)this.MemberwiseClone();
            model.Rates = this.Rates.Where(x=> x != null).Select(x => (CurrencyRate)x.Clone()).ToArray();

            return model;
        }
    }

    [Serializable]
    public class CurrencyRate : SettingsBase
    {
        private string _name;
        private decimal _rate;
        private EnumCurrency _currency;

        [XmlText]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [XmlAttribute]
        public decimal Rate
        {
            get { return _rate; }
            set
            {
                if (_rate == value) return;
                _rate = value;
                OnPropertyChanged(nameof(Rate));
            }
        }

        [XmlAttribute]
        public EnumCurrency Currency
        {
            get { return _currency; }
            set
            {
                if (_currency == value) return;
                _currency = value;
                OnPropertyChanged(nameof(Currency));
            }
        }
    }

}
