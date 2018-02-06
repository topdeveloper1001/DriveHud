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

using DriveHUD.Application.ValueConverters;
using Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Telerik.Charting;
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
            typeof(CashChart), new PropertyMetadata(null, OnItemsSourceChanged));

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
            typeof(CashChart), new FrameworkPropertyMetadata(ChartDisplayRange.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnDisplayRangeChanged));

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

        public static readonly DependencyProperty ShowShowdownProperty = DependencyProperty.Register("ShowShowdown", typeof(bool), typeof(CashChart),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnShowShowdownChanged));

        public bool ShowShowdown
        {
            get
            {
                return (bool)GetValue(ShowShowdownProperty);
            }
            set
            {
                SetValue(ShowShowdownProperty, value);
            }
        }

        public static readonly DependencyProperty ShowNonShowdownProperty = DependencyProperty.Register("ShowNonShowdown", typeof(bool), typeof(CashChart),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnShowShowdownChanged));

        public bool ShowNonShowdown
        {
            get
            {
                return (bool)GetValue(ShowNonShowdownProperty);
            }
            set
            {
                SetValue(ShowNonShowdownProperty, value);
            }
        }

        public static readonly DependencyProperty ShowNonShowdownEnabledProperty = DependencyProperty.Register("ShowNonShowdownEnabled", typeof(bool), typeof(CashChart),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public bool ShowNonShowdownEnabled
        {
            get
            {
                return (bool)GetValue(ShowNonShowdownEnabledProperty);
            }
            set
            {
                SetValue(ShowNonShowdownEnabledProperty, value);
            }
        }

        public static readonly DependencyProperty ShowEVProperty = DependencyProperty.Register("ShowEV", typeof(bool), typeof(CashChart),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnShowShowdownChanged));

        public bool ShowEV
        {
            get
            {
                return (bool)GetValue(ShowEVProperty);
            }
            set
            {
                SetValue(ShowEVProperty, value);
            }
        }

        public static readonly DependencyProperty ValueTypeProperty = DependencyProperty.Register("ValueType", typeof(ChartCashSeriesValueType), typeof(CashChart),
            new FrameworkPropertyMetadata(ChartCashSeriesValueType.Currency, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueTypeChanged));

        public ChartCashSeriesValueType ValueType
        {
            get
            {
                return (ChartCashSeriesValueType)GetValue(ValueTypeProperty);
            }
            set
            {
                SetValue(ValueTypeProperty, value);
            }
        }

        public static readonly DependencyProperty WinningValueTypeEnabledProperty = DependencyProperty.Register("WinningValueTypeEnabled", typeof(bool), typeof(CashChart),
         new FrameworkPropertyMetadata(false));

        public bool WinningValueTypeEnabled
        {
            get
            {
                return (bool)GetValue(WinningValueTypeEnabledProperty);
            }
            set
            {
                SetValue(WinningValueTypeEnabledProperty, value);
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

        public static readonly DependencyProperty HandsAxisMaxValuelProperty = DependencyProperty.Register("HandsAxisMaxValue", typeof(int), typeof(CashChart),
            new PropertyMetadata(1, OnHandsAxisMaxValueChanged));

        public int HandsAxisMaxValue
        {
            get
            {
                return (int)GetValue(HandsAxisMaxValuelProperty);
            }
            set
            {
                SetValue(HandsAxisMaxValuelProperty, value);
            }
        }

        public static readonly DependencyProperty HandsAxisLabelsCountProperty = DependencyProperty.Register("HandsAxisLabelsCount", typeof(int), typeof(CashChart),
           new PropertyMetadata(1, OnHandsAxisMaxValueChanged));

        public int HandsAxisLabelsCount
        {
            get
            {
                return (int)GetValue(HandsAxisLabelsCountProperty);
            }
            set
            {
                SetValue(HandsAxisLabelsCountProperty, value);
            }
        }

        public static readonly DependencyProperty ShowInPopupCommandProperty = DependencyProperty.Register("ShowInPopupCommand", typeof(ICommand), typeof(CashChart),
            new PropertyMetadata(null));

        public ICommand ShowInPopupCommand
        {
            get
            {
                return (ICommand)GetValue(ShowInPopupCommandProperty);
            }
            set
            {
                SetValue(ShowInPopupCommandProperty, value);
            }
        }

        #region Colors scheme

        public readonly static DependencyProperty LineColorsProperty = DependencyProperty.Register("LineColors",
            typeof(ColorBrushChartCashSeriesWinningTypeCollection), typeof(CashChart),
            new PropertyMetadata(new ColorBrushChartCashSeriesWinningTypeCollection()));
   
        public ColorBrushChartCashSeriesWinningTypeCollection LineColors
        {
            get
            {
                return (ColorBrushChartCashSeriesWinningTypeCollection)GetValue(LineColorsProperty);
            }
            set
            {
                SetValue(LineColorsProperty, value);
            }
        }

        public readonly static DependencyProperty PointColorsProperty = DependencyProperty.Register("PointColors",
           typeof(ColorBrushChartCashSeriesWinningTypeCollection), typeof(CashChart),
           new PropertyMetadata(new ColorBrushChartCashSeriesWinningTypeCollection()));

        public ColorBrushChartCashSeriesWinningTypeCollection PointColors
        {
            get
            {
                return (ColorBrushChartCashSeriesWinningTypeCollection)GetValue(PointColorsProperty);
            }
            set
            {
                SetValue(PointColorsProperty, value);
            }
        }

        public readonly static DependencyProperty TrackBallColorsProperty = DependencyProperty.Register("TrackBallColors",
            typeof(ColorBrushChartCashSeriesWinningTypeCollection), typeof(CashChart),
            new PropertyMetadata(new ColorBrushChartCashSeriesWinningTypeCollection()));

        public ColorBrushChartCashSeriesWinningTypeCollection TrackBallColors
        {
            get
            {
                return (ColorBrushChartCashSeriesWinningTypeCollection)GetValue(TrackBallColorsProperty);
            }
            set
            {
                SetValue(TrackBallColorsProperty, value);
            }
        }

        #endregion

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
                    chartGadget.MainSeriesDescriptor.Style = chartGadget.FindResource("CategoricalLineSeriesDescriptorWithPointsStyle") as Style;
                    chartGadget.MainChart.HorizontalAxis = CreateDateTimeCategoricalAxis(monthFormatString);
                    break;

                case ChartDisplayRange.Month:
                case ChartDisplayRange.Week:
                    chartGadget.MainSeriesDescriptor.Style = chartGadget.FindResource("CategoricalLineSeriesDescriptorWithPointsStyle") as Style;
                    chartGadget.MainChart.HorizontalAxis = CreateDateTimeCategoricalAxis(dayFormatString);
                    break;

                case ChartDisplayRange.Hands:
                    var interval = chartGadget.GetHandsTicksInterval();
                    chartGadget.MainSeriesDescriptor.Style = chartGadget.FindResource("CategoricalLineSeriesDescriptorStyle") as Style;
                    chartGadget.MainChart.HorizontalAxis = CreateCategoricalAxis(interval);
                    break;
            }
        }

        private static DateTimeCategoricalAxis CreateDateTimeCategoricalAxis(string labelFormat)
        {
            return new DateTimeCategoricalAxis
            {
                LabelFitMode = AxisLabelFitMode.MultiLine,
                TickThickness = 0,
                ElementBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#696a6c")),
                LabelFormat = labelFormat,
                FontSize = 11
            };
        }

        private static CategoricalAxis CreateCategoricalAxis(int majorTickInterval)
        {
            return new CategoricalAxis
            {
                LabelFitMode = AxisLabelFitMode.None,
                MajorTickInterval = majorTickInterval,
                TickThickness = 0,
                ElementBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom("#696a6c")),
                FontSize = 10
            };
        }

        private void ChartTrackBallBehavior_TrackInfoUpdated(object sender, TrackBallInfoEventArgs e)
        {
            if (e.Context == null || e.Context.ClosestDataPoint == null)
            {
                return;
            }

            var dataPoint = e.Context.ClosestDataPoint.DataPoint as CategoricalDataPoint;

            if (dataPoint == null)
            {
                return;
            }

            switch (DisplayRange)
            {
                case ChartDisplayRange.Hands:
                    e.Header = dataPoint.Category.ToString();
                    return;
                case ChartDisplayRange.Month:
                case ChartDisplayRange.Week:
                    e.Header = ConvertDataPointCategoryToDateString(dataPoint.Category, dayFormatString);
                    return;
                case ChartDisplayRange.Year:
                    e.Header = ConvertDataPointCategoryToDateString(dataPoint.Category, monthFormatString);
                    return;
            }
        }

        private static string ConvertDataPointCategoryToDateString(object category, string format)
        {
            if (category is DateTime)
            {
                var categoryDateTime = (DateTime)category;
                return categoryDateTime.ToString(format);
            }

            return category.ToString();
        }

        private static void OnValueTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CashChart)?.UpdateSeriesVisibility();
        }

        private void UpdateSeriesVisibility()
        {
            if (ItemsSource == null)
            {
                return;
            }

            foreach (var chartSerie in ItemsSource)
            {
                if (ValueType != chartSerie.ChartCashSeriesValueType)
                {
                    chartSerie.IsVisible = false;
                    continue;
                }

                var result = chartSerie.ChartCashSeriesWinningType == ChartCashSeriesWinningType.Netwon ||
                    (chartSerie.ChartCashSeriesWinningType == ChartCashSeriesWinningType.Showdown && ShowShowdown) ||
                    (chartSerie.ChartCashSeriesWinningType == ChartCashSeriesWinningType.NonShowdown && ShowNonShowdown) ||
                    (chartSerie.ChartCashSeriesWinningType == ChartCashSeriesWinningType.EV && ShowEV);

                chartSerie.IsVisible = result;
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cashChart = d as CashChart;

            if (cashChart == null)
            {
                return;
            }

            var oldItemsSource = e.OldValue as ObservableCollection<DriveHUD.ViewModels.ChartSeries>;

            if (oldItemsSource != null)
            {
                oldItemsSource.CollectionChanged -= cashChart.OnItemsSourceCollectionChanged;
            }

            var itemsSource = e.NewValue as ObservableCollection<DriveHUD.ViewModels.ChartSeries>;

            if (itemsSource == null)
            {
                return;
            }

            itemsSource.CollectionChanged += cashChart.OnItemsSourceCollectionChanged;
            cashChart.UpdateSeriesVisibility();
        }

        private void OnItemsSourceCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            UpdateSeriesVisibility();
        }

        private static void OnShowShowdownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as CashChart)?.UpdateSeriesVisibility();
        }

        private static void OnHandsAxisMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartGadget = d as CashChart;

            if (chartGadget == null)
            {
                return;
            }

            if (chartGadget.DisplayRange != ChartDisplayRange.Hands)
            {
                return;
            }

            var categoricalAxis = chartGadget.MainChart.HorizontalAxis as CategoricalAxis;

            if (categoricalAxis == null)
            {
                return;
            }

            categoricalAxis.MajorTickInterval = chartGadget.GetHandsTicksInterval();
        }

        private int GetHandsTicksInterval()
        {
            var interval = HandsAxisMaxValue > 0 && HandsAxisLabelsCount > 0 && HandsAxisMaxValue > HandsAxisLabelsCount ?
                HandsAxisMaxValue / HandsAxisLabelsCount : 1;

            return interval;
        }
    }
}