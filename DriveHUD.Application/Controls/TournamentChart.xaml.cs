//-----------------------------------------------------------------------
// <copyright file="TournamentChart.xaml.cs" company="Ace Poker Solutions">
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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for TournamentChart.xaml
    /// </summary>
    public partial class TournamentChart : UserControl
    {
        private static readonly string monthFormatString = "MMM";
        private static readonly string dayFormatString = "MMM dd";

        public TournamentChart()
        {
            InitializeComponent();
        }

        #region Dependency properties

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ObservableCollection<TournamentChartSeries>),
            typeof(TournamentChart), new PropertyMetadata(null, OnItemsSourceChanged));

        public ObservableCollection<TournamentChartSeries> ItemsSource
        {
            get
            {
                return (ObservableCollection<TournamentChartSeries>)GetValue(ItemsSourceProperty);
            }
            set
            {
                SetValue(ItemsSourceProperty, value);
            }
        }

        public static readonly DependencyProperty HeaderStartColorProperty = DependencyProperty.Register(
            "HeaderStartColor", typeof(Color), typeof(TournamentChart), new PropertyMetadata(default(Color)));

        public Color HeaderStartColor
        {
            get { return (Color)GetValue(HeaderStartColorProperty); }
            set { SetValue(HeaderStartColorProperty, value); }
        }

        public static readonly DependencyProperty HeaderStopColorProperty = DependencyProperty.Register(
            "HeaderStopColor", typeof(Color), typeof(TournamentChart), new PropertyMetadata(default(Color)));

        public Color HeaderStopColor
        {
            get { return (Color)GetValue(HeaderStopColorProperty); }
            set { SetValue(HeaderStopColorProperty, value); }
        }

        public static readonly DependencyProperty CaptionColorProperty = DependencyProperty.Register(
            "CaptionColor", typeof(Color), typeof(TournamentChart), new PropertyMetadata(default(Color)));

        public Color CaptionColor
        {
            get { return (Color)GetValue(CaptionColorProperty); }
            set { SetValue(CaptionColorProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(TournamentChart), new PropertyMetadata(default(string)));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register("SelectedRange", typeof(ChartDisplayRange), typeof(TournamentChart),
                new FrameworkPropertyMetadata(ChartDisplayRange.None, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedRangeChanged));

        public ChartDisplayRange SelectedRange
        {
            get
            {
                return (ChartDisplayRange)GetValue(SelectedRangeProperty);
            }
            set
            {
                SetValue(SelectedRangeProperty, value);
            }
        }

        public static readonly DependencyProperty TournamentChartFilterTypeProperty =
            DependencyProperty.Register("TournamentChartFilterType", typeof(TournamentChartFilterType), typeof(TournamentChart),
                new FrameworkPropertyMetadata(TournamentChartFilterType.All, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public TournamentChartFilterType TournamentChartFilterType
        {
            get
            {
                return (TournamentChartFilterType)GetValue(TournamentChartFilterTypeProperty);
            }
            set
            {
                SetValue(TournamentChartFilterTypeProperty, value);
            }
        }

        public static readonly DependencyProperty TournamentChartFilterTypesProperty =
            DependencyProperty.Register("TournamentChartFilterTypes", typeof(ObservableCollection<TournamentChartFilterType>), typeof(TournamentChart));

        public ObservableCollection<TournamentChartFilterType> TournamentChartFilterTypes
        {
            get
            {
                return (ObservableCollection<TournamentChartFilterType>)GetValue(TournamentChartFilterTypesProperty);
            }
            set
            {
                SetValue(TournamentChartFilterTypesProperty, value);
            }
        }

        #endregion

        private static void OnSelectedRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartGadget = d as TournamentChart;

            if (chartGadget == null)
            {
                return;
            }

            switch (chartGadget.SelectedRange)
            {
                case ChartDisplayRange.Year:
                    chartGadget.AxisDateTime.LabelFormat = monthFormatString;
                    break;

                case ChartDisplayRange.Month:
                    chartGadget.AxisDateTime.LabelFormat = dayFormatString;
                    break;

                case ChartDisplayRange.Week:
                    chartGadget.AxisDateTime.LabelFormat = dayFormatString;
                    break;
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}