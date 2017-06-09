//-----------------------------------------------------------------------
// <copyright file="HeatMapValueToColorsBaseConverter.cs" company="Ace Poker Solutions">
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
using System.Windows;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public abstract class HeatMapValueToColorsBaseConverter : MarkupExtensionConverterBase
    {
        protected abstract string[] Colors
        {
            get;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rangeBlockValue = value as decimal?;

            if (!rangeBlockValue.HasValue)
            {
                return new BrushConverter().ConvertFromString(Colors[0]) as SolidColorBrush;
            }

            var colorIndex = (int)Math.Ceiling(rangeBlockValue.Value * 10);

            if (colorIndex >= Colors.Length)
            {
                colorIndex = Colors.Length - 1;
            }

            var color = Colors[colorIndex];

            return new BrushConverter().ConvertFromString(color) as SolidColorBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}