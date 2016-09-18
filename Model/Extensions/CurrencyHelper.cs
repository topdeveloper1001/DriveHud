using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Extensions
{
    public static class CurrencyHelper
    {
        public static string CurrencyToAPIString(this EnumCurrency currency)
        {
            switch (currency)
            {
                case EnumCurrency.USD:
                    return "USD";
                case EnumCurrency.EUR:
                    return "EUR";
                case EnumCurrency.CNY:
                    return "CNY";
                case EnumCurrency.RUB:
                    return "RUB";
                case EnumCurrency.CAN:
                    return "CAD";
                case EnumCurrency.SEK:
                    return "SEK";
                default:
                    return string.Empty;
            }
        }
    }
}
