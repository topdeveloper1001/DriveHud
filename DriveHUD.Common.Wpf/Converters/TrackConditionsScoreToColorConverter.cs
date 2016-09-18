using System;
using System.Globalization;
//-----------------------------------------------------------------------
// <copyright file="TrackConditionsScoreToColorConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class TrackConditionsScoreToColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var scores = value as decimal?;

            var isBorder = parameter != null;

            if (scores == null)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
            }

            // score is 1-3
            if (scores.Value >= 1 && scores.Value < 4)
            {
                if (isBorder)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ff3c52"));
                }

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#c92a39"));
            }
            else if (scores.Value >= 4 && scores.Value < 8)
            {
                if (isBorder)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#ffda2a"));
                }

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b4981d"));
            }
            else if (scores.Value >= 8)
            {
                if (isBorder)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#43e246"));
                }

                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2f9e31"));
            }

            if (isBorder)
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#aaaaa8"));
            }

            return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#808080"));
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}