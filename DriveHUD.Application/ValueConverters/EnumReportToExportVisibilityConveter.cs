//-----------------------------------------------------------------------
// <copyright file="EnumReportToExportVisibilityConveter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace DriveHUD.Application.ValueConverters
{
    public class EnumReportToExportVisibilityConveter : IValueConverter
    {
        private static readonly EnumReports[] supportedReports = new[]
        {
            EnumReports.OverAll,
            EnumReports.Session,
            EnumReports.Stake,
            EnumReports.PokerSite,
            EnumReports.TournamentResults,
            EnumReports.Tournaments,
            EnumReports.TournamentStats,
            EnumReports.TournamentPokerSite
        };

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is EnumReports report))
            {
                return Visibility.Collapsed;
            }

            return supportedReports.Contains(report) ?
                Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}