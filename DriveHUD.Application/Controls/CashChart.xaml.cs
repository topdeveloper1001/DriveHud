//-----------------------------------------------------------------------
// <copyright file="CashChart.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System;
using Telerik.Windows.Controls.ChartView;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for CashChart.xaml
    /// </summary>
    public partial class CashChart : UserControl
    {
        public CashChart()
        {
            InitializeComponent();
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<DriveHUD.ViewModels.ChartSeries>),
            typeof(CashChart));

        public ObservableCollection<DriveHUD.ViewModels.ChartSeries> ItemsSource
        {
            get
            {
                return (ObservableCollection<DriveHUD.ViewModels.ChartSeries>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty DisplayRangeProperty = DependencyProperty.Register("DisplayRange", typeof(ChartDisplayRange),
            typeof(CashChart), new FrameworkPropertyMetadata(ChartDisplayRange.Month, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayRangeChanged));

        public ChartDisplayRange DisplayRange
        {
            get
            {
                return (ChartDisplayRange)GetValue(DisplayRangeProperty);
            }
            set
            {
                SetValue(DisplayRangeProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string),
            typeof(CashChart));

        public string Caption
        {
            get
            {
                return (string)GetValue(CaptionProperty);
            }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public static readonly DependencyProperty CaptionColorProperty = DependencyProperty.Register("CaptionColor", typeof(Color),
            typeof(CashChart), new PropertyMetadata(default(Color)));

        public Color CaptionColor
        {
            get
            {
                return (Color)GetValue(CaptionColorProperty);
            }
            set
            {
                SetValue(CaptionColorProperty, value);
            }
        }

        public static readonly DependencyProperty HeaderStartColorProperty = DependencyProperty.Register("HeaderStartColor", typeof(Color),
            typeof(CashChart), new PropertyMetadata(default(Color)));

        public Color HeaderStartColor
        {
            get
            {
                return (Color)GetValue(HeaderStartColorProperty);
            }
            set
            {
                SetValue(HeaderStartColorProperty, value);
            }
        }

        public static readonly DependencyProperty HeaderStopColorProperty = DependencyProperty.Register("HeaderStopColor", typeof(Color),
            typeof(CashChart), new PropertyMetadata(default(Color)));

        public Color HeaderStopColor
        {
            get
            {
                return (Color)GetValue(HeaderStopColorProperty);
            }
            set
            {
                SetValue(HeaderStopColorProperty, value);
            }
        }

        #endregion

        private static readonly string monthFormatString = "MMM";
        private static readonly string dayFormatString = "MMM dd";

        private static void OnDisplayRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartGadget = d as CashChart;

            if (chartGadget == null)
            {
                return;
            }

            switch (chartGadget.DisplayRange)
            {
                case ChartDisplayRange.Year:
                    chartGadget.MainChart.HorizontalAxis = CreateDateTimeCategoricalAxis(monthFormatString);
                    break;

                case ChartDisplayRange.Month:
                case ChartDisplayRange.Week:
                    chartGadget.MainChart.HorizontalAxis = CreateDateTimeCategoricalAxis(dayFormatString);
                    break;

                case ChartDisplayRange.Hands:
                    chartGadget.MainChart.HorizontalAxis = CreateCategoricalAxis();
                    break;
            }
        }

        private static DateTimeCategoricalAxis CreateDateTimeCategoricalAxis(string labelFormat)
        {
            return new DateTimeCategoricalAxis
            {
                LabelFitMode = Telerik.Charting.AxisLabelFitMode.MultiLine,
                TickThickness = 0,
                ElementBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303134")),
                LabelFormat = labelFormat,
                FontSize = 11
            };
        }

        private static CategoricalAxis CreateCategoricalAxis()
        {
            return new CategoricalAxis
            {
                LabelFitMode = Telerik.Charting.AxisLabelFitMode.None,
                MajorTickInterval = 500,
                TickThickness = 0,
                ElementBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#303134")),
                IsStepRecalculationOnZoomEnabled = true,
                FontSize = 11
            };
        }
    }
}