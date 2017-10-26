//-----------------------------------------------------------------------
// <copyright file="ActionToBetweenVisibilityConverter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.DataTypes;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.PlayerXRay.Converters
{
    public class ActionToBetweenVisibilityConverter : IValueConverter
    {
        public Visibility HiddenVisibility { get; set; } = Visibility.Hidden;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var action = value as ActionTypeEnum?;

            if (!action.HasValue)
            {
                return HiddenVisibility;
            }

            switch (action.Value)
            {
                case ActionTypeEnum.Bet:
                case ActionTypeEnum.Call:
                case ActionTypeEnum.Raise:             
                    return Visibility.Visible;
                default:
                    return HiddenVisibility;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}