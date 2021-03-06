﻿//-----------------------------------------------------------------------
// <copyright file="EmptyCollectionToVisibilityConverter.cs" company="Ace Poker Solutions">
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
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    [ValueConversion(typeof(ICollection), typeof(Visibility))]
    public class EmptyCollectionToVisibilityConverter : MarkupExtensionConverterBase
    {
        public EmptyCollectionToVisibilityConverter()
        {
            HiddenVisibility = Visibility.Collapsed;
        }

        public Visibility HiddenVisibility { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as ICollection;

            if (collection == null || collection.Count == 0)
            {
                return HiddenVisibility;
            }

            return Visibility.Visible;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}