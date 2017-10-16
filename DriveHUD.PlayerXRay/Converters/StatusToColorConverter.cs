//-----------------------------------------------------------------------
// <copyright file="StatusToColorConverter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.PlayerXRay.Converters
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value as RunningStatus?;

            if (!status.HasValue)
            {
                return new SolidColorBrush(Colors.White);
            }

            if (status == RunningStatus.Idle)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#6e45d7"));
            }
            else if (status == RunningStatus.Processing)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#3cb01d"));
            }
            else if (status == RunningStatus.Completed)
            {
                return (SolidColorBrush)(new BrushConverter().ConvertFrom("#40aadf"));
            }

            return new SolidColorBrush(Colors.Red);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}