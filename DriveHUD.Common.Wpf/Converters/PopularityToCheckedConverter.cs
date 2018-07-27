//-----------------------------------------------------------------------
// <copyright file="PopularityToCheckedConverter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.Common.Wpf.Converters
{
    public class PopularityToCheckedConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int popularity) ||
                parameter == null || !int.TryParse(parameter.ToString(), out int flag))
            {
                return false;
            }

            return popularity >= flag;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}