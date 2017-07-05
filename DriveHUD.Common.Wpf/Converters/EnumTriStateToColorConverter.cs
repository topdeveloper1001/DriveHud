//-----------------------------------------------------------------------
// <copyright file="EnumTriStateToColorConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Converters
{
    public class EnumTriStateToColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var triState = value as EnumTriState?;

            if (triState == EnumTriState.On)
            {
                return Colors.Green;
            }

            if (triState == EnumTriState.Off)
            {
                return Colors.Red;
            }

            return Colors.Black;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}