using HandHistories.Objects.GameDescription;
using Prism.Mvvm;

namespace CurrencyTool
{
    public class CurrencyViewModel : BindableBase
    {
        public CurrencyViewModel(Currency currency)
        {
            Currency = currency;
        }

        private Currency currency;

        public Currency Currency
        {
            get
            {
                return currency;
            }
            private set
            {
                SetProperty(ref currency, value);
            }
        }

        private decimal rate;

        public decimal Rate
        {
            get
            {
                return rate;
            }
            set
            {
                SetProperty(ref rate, value);
            }
        }
    }
}