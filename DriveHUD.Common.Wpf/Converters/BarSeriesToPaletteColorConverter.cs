//-----------------------------------------------------------------------
// <copyright file="BarSerieToPaletteColorConverter.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace DriveHUD.Common.Wpf.Converters
{
    public class BarSeriesToPaletteColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var barSeries = value as BarSeries;

            if (barSeries == null)
            {
                return null;
            }

            var chart = barSeries.Chart as RadCartesianChart;

            if (chart == null)
            {
                return null;
            }

            var seriesIndex = chart.Series.IndexOf(barSeries);

            var entry = chart.Palette.GetEntry(barSeries, seriesIndex);

            if (entry.HasValue)
            {
                return entry.Value.Fill;
            }

            return null;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
