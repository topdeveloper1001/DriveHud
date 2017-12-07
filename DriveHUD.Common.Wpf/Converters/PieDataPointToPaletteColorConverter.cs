//-----------------------------------------------------------------------
// <copyright file="PieDataPointToPaletteColorConverter.cs" company="Ace Poker Solutions">
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
    public class PieDataPointToPaletteColorConverter : MarkupExtensionConverterBase
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pieDataPoint = value as PieDataPoint;

            if (pieDataPoint == null)
            {
                return null;
            }

            var series = pieDataPoint.Presenter as PieSeries;

            if (series == null)
            {
                return null;
            }

            var chart = series.Chart as RadPieChart;

            if (chart == null)
            {
                return null;
            }

            var dataPointIndex = series.DataPoints.IndexOf(pieDataPoint);

            var entry = chart.Palette.GetEntry(series, dataPointIndex);

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
