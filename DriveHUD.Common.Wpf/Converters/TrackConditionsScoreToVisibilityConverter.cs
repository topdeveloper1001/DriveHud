//-----------------------------------------------------------------------
// <copyright file="TrackConditionsScoreToVisibilityConverter.cs" company="Ace Poker Solutions">
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

namespace DriveHUD.Common.Wpf.Converters
{
    public class TrackConditionsScoreToVisibilityConverter : MarkupExtensionConverterBase
    {
        public TrackConditionsScoreToVisibilityConverter()
        {
            HiddenVisibility = Visibility.Collapsed;

            Divider = 1;
        }

        public Visibility HiddenVisibility { get; set; }

        public decimal Divider { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var meter = value as decimal?;
            var level = 0;

            if (!meter.HasValue || !int.TryParse(parameter.ToString(), out level))
            {
                return HiddenVisibility;
            }

            if (meter.Value < 1)
            {
                return HiddenVisibility;
            }

            var meterValue = Divider != 0 ? meter.Value / Divider : meter.Value;
            var visible = Math.Round(meterValue, MidpointRounding.AwayFromZero) >= level;

            return visible ? Visibility.Visible : HiddenVisibility;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
