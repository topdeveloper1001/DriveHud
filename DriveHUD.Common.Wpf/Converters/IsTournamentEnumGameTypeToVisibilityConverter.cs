//-----------------------------------------------------------------------
// <copyright file="EnumMultiParamToVisibilityConverter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Extensions;
using DriveHUD.Entities;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Common.Wpf.Converters
{
    /// <summary>
    /// EnumMultiParamToVisibilityConverter
    /// </summary>
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class IsTournamentEnumGameTypeToVisibilityConverter : MarkupExtensionConverterBase
    {
        public IsTournamentEnumGameTypeToVisibilityConverter()
        {
            HiddenVisibility = Visibility.Collapsed;
        }

        public Visibility HiddenVisibility { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return HiddenVisibility;
            }

            var gameType = value as EnumGameType?;

            if (gameType.HasValue && gameType.Value.IsTournamentGameType())
            {
                return Visibility.Visible;
            }

            return HiddenVisibility;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}