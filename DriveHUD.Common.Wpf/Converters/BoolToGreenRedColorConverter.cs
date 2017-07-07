//-----------------------------------------------------------------------
// <copyright file="BoolToGreenRedColorConverter.cs" company="Ace Poker Solutions">
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
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class BoolToGreenRedColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var b = value as bool?;

            if (b == true)
            {
                return Colors.Green;
            }

            return Colors.Red;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value as Color?;

            if (color == Colors.Green)
            {
                return true;
            }

            return false;
        }
    }
}