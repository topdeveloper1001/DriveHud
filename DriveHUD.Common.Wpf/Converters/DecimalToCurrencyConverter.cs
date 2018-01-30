//-----------------------------------------------------------------------
// <copyright file="DecimalToCurrencyConverter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class DecimalToCurrencyConverter : MarkupExtensionConverterBase
    {
        private readonly static NumberFormatInfo numberFormatInfo = new NumberFormatInfo
        {
            NegativeSign = "-",
            CurrencyNegativePattern = 1,
            CurrencyDecimalSeparator = ".",
            CurrencyGroupSeparator = ",",
            CurrencySymbol = "$"
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // formally we need to read settings here, then use selected currency
            decimal inputValue = 0;

            try
            {
                inputValue = System.Convert.ToDecimal(value);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Value {value} cannot be converted to currency", ex);
            }

            return inputValue.ToString("C", numberFormatInfo);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}