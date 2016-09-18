//-----------------------------------------------------------------------
// <copyright file="BackgroundToForegroundColorConverter.cs" company="Ace Poker Solutions">
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
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class BackgroundToForegroundColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;

            if (color == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var averageColor = (color.Value.R + color.Value.B + color.Value.G) / 3;

            if (averageColor > 210 || ((color.Value.R + color.Value.G) > 400))
            {
                return new SolidColorBrush(Color.FromRgb(0, 0, 0));
            }

            return new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = value as SolidColorBrush;

            if (brush == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return brush.Color;
        }
    }
}