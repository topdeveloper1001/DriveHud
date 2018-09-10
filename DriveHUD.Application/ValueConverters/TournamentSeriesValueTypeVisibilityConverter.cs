//-----------------------------------------------------------------------
// <copyright file="TournamentSeriesValueTypeVisibilityConverter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using Model.Enums;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class TournamentSeriesValueTypeVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
            {
                return Visibility.Visible;
            }

            var tournamentSeries = values[0] as TournamentChartSeries;
            var valueType = values[1] as ChartTournamentSeriesValueType?;

            if (tournamentSeries == null || !valueType.HasValue)
            {
                return Visibility.Visible;
            }

            return tournamentSeries.SeriesValueType == ChartTournamentSeriesValueType.None ||
                tournamentSeries.SeriesValueType == valueType.Value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { DependencyProperty.UnsetValue };
        }
    }
}