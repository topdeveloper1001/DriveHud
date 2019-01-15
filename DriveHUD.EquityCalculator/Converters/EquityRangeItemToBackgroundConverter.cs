//-----------------------------------------------------------------------
// <copyright file="EquityRangeItemToBackgroundConverter.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.EquityCalculator.ViewModels;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace DriveHUD.EquityCalculator.Converters
{
    public class EquityRangeItemToBackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 || !(values[2] is EquityRangeSelectorItemViewModel rangeItem))
            {
                return new SolidColorBrush(RangeBackground);
            }

            if (rangeItem.IsSelected)
            {
                if (rangeItem.Combos > 0 &&
                    (rangeItem.ValueBetCombos > 0 || rangeItem.BluffCombos > 0 ||
                        rangeItem.CallCombos > 0 || rangeItem.FoldCheckCombos > 0))
                {
                    var brush = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(0, 1)
                    };

                    var offset = 0d;

                    if (rangeItem.ValueBetCombos > 0)
                    {
                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = ValueBetRangeBackground
                        });

                        offset += (double)rangeItem.ValueBetCombos / rangeItem.Combos;

                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = ValueBetRangeBackground
                        });
                    }

                    if (rangeItem.BluffCombos > 0)
                    {
                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = BluffRangeBackground
                        });

                        offset += (double)rangeItem.BluffCombos / rangeItem.Combos;

                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = BluffRangeBackground
                        });
                    }

                    if (rangeItem.CallCombos > 0)
                    {
                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = CallRangeBackground
                        });

                        offset += (double)rangeItem.CallCombos / rangeItem.Combos;

                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = CallRangeBackground
                        });
                    }

                    if (rangeItem.FoldCheckCombos > 0)
                    {
                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = FoldCheckRangeBackground
                        });

                        offset += (double)rangeItem.FoldCheckCombos / rangeItem.Combos;

                        brush.GradientStops.Add(new GradientStop
                        {
                            Offset = offset,
                            Color = FoldCheckRangeBackground
                        });
                    }

                    return brush;
                }

                return new SolidColorBrush(SelectedRangeBackground);
            }

            return new SolidColorBrush(RangeBackground);
        }

        public Color RangeBackground { get; set; }

        public Color SelectedRangeBackground { get; set; }

        public Color FoldCheckRangeBackground { get; set; }

        public Color CallRangeBackground { get; set; }

        public Color BluffRangeBackground { get; set; }

        public Color ValueBetRangeBackground { get; set; }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { DependencyProperty.UnsetValue };
        }
    }
}