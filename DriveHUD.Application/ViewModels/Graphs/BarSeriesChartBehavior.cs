//-----------------------------------------------------------------------
// <copyright file="BarSeriesChartBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace DriveHUD.Application.ViewModels.Graphs
{
    public class BarSeriesChartBehavior : Behavior<RadCartesianChart>
    {
        public static readonly DependencyProperty SelectedSerieTypeProperty = DependencyProperty.Register("SelectedSerieType", typeof(SerieType?),
            typeof(BarSeriesChartBehavior), new PropertyMetadata(null, OnSelectedSerieTypePropertyChanged));

        public SerieType? SelectedSerieType
        {
            get
            {
                return (SerieType)GetValue(SelectedSerieTypeProperty);
            }
            set
            {
                SetValue(SelectedSerieTypeProperty, value);
            }
        }

        private static void OnSelectedSerieTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as BarSeriesChartBehavior;

            if (behavior == null)
            {
                return;
            }

            behavior.UpdateHorizontalAxis();
        }

        public static readonly DependencyProperty AxisesProperty = DependencyProperty.Register("Axises", typeof(BarSeriesChartAxisCollection),
            typeof(BarSeriesChartBehavior), new PropertyMetadata(new BarSeriesChartAxisCollection(), OnAxisesPropertyChanged));

        public BarSeriesChartAxisCollection Axises
        {
            get
            {
                return (BarSeriesChartAxisCollection)GetValue(AxisesProperty);
            }
            set
            {
                SetValue(AxisesProperty, value);
            }
        }

        private static void OnAxisesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as BarSeriesChartBehavior;

            if (behavior == null)
            {
                return;
            }

            behavior.UpdateHorizontalAxis();
        }

        private void UpdateHorizontalAxis()
        {
            if (AssociatedObject == null)
            {
                return;
            }            

            var barSeriesChartAxis = Axises.FirstOrDefault(x => x.SerieType == SelectedSerieType);

            if (barSeriesChartAxis == null)
            {
                return;
            }

            if (!ReferenceEquals(AssociatedObject.HorizontalAxis, barSeriesChartAxis.Axis))
            {
                try
                {
                    AssociatedObject.HorizontalAxis = barSeriesChartAxis.GetAxis();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Could not update horizontal axis of bar series chart", e);
                }
            }
        }
    }

    public class BarSeriesChartAxisCollection : ObservableCollection<BarSeriesChartAxis>
    {
    }

    public class BarSeriesChartAxis : FrameworkElement
    {
        public static readonly DependencyProperty SerieTypeProperty = DependencyProperty.Register("SerieType", typeof(SerieType), typeof(BarSeriesChartAxis));

        public SerieType SerieType
        {
            get
            {
                return (SerieType)GetValue(SerieTypeProperty);
            }
            set
            {
                SetValue(SerieTypeProperty, value);
            }
        }

        public static readonly DependencyProperty CartesianAxisProperty = DependencyProperty.Register("CartesianAxis", typeof(CartesianAxis),
            typeof(BarSeriesChartAxis));

        public CartesianAxis Axis
        {
            get
            {
                return (CartesianAxis)GetValue(CartesianAxisProperty);
            }
            set
            {
                SetValue(CartesianAxisProperty, value);
            }
        }

        public CartesianAxis GetAxis()
        {
            if (Axis is DateTimeCategoricalAxis)
            {
                var axis = (DateTimeCategoricalAxis)Axis;

                var axisCopy = new DateTimeCategoricalAxis
                {
                    LabelFormat = axis.LabelFormat,
                    MajorTickStyle = axis.MajorTickStyle,
                    ShowLabels = axis.ShowLabels,
                    ElementBrush = axis.ElementBrush,
                    FontSize = axis.FontSize,
                    FontWeight = axis.FontWeight                    
                };

                return axisCopy;
            }

            if (Axis is CategoricalAxis)
            {
                var axis = (CategoricalAxis)Axis;

                var axisCopy = new CategoricalAxis
                {
                    MajorTickStyle = axis.MajorTickStyle,
                    ShowLabels = axis.ShowLabels,
                    ElementBrush = axis.ElementBrush,
                    FontSize = axis.FontSize,
                    FontWeight = axis.FontWeight
                };

                return axisCopy;
            }

            return Axis;
        }
    }
}