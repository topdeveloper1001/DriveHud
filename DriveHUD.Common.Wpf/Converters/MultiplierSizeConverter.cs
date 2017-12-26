//-----------------------------------------------------------------------
// <copyright file="MultiplierConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Linq;
using System.Windows;

namespace DriveHUD.Common.Wpf.Converters
{
    public class MultiplierSizeConverter : MarkupExtensionMultiConverterBase
    {
        public override object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || !(parameter is double))
            {
                return value;
            }

            var values = value.OfType<double>().ToArray();

            if (values.Length == 0)
            {
                return value;
            }

            var actualSize = values.Min();
            var multiplier = (double)parameter;

            return actualSize * multiplier;
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { DependencyProperty.UnsetValue };
        }
    }
}