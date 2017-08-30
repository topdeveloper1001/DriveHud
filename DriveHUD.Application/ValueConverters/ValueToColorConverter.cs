//-----------------------------------------------------------------------
// <copyright file="ValueToColorConverter.cs" company="Ace Poker Solutions">
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
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.Application.ValueConverters
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
            {
                if (decimal.Parse(value.ToString()) > 0)
                {
                    return new SolidColorBrush(Color.FromRgb(32, 159, 86));
                }

                if (decimal.Parse(value.ToString()) < 0)
                {
                    return new SolidColorBrush(Color.FromRgb(230, 60, 44));
                }
            }

            return new SolidColorBrush(Color.FromRgb(125, 127, 132));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Do the conversion from visibility to bool
            return null;
        }
    }
}