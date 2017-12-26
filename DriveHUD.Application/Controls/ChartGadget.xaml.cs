//-----------------------------------------------------------------------
// <copyright file="ChartGadget.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for ChartGadget.xaml
    /// </summary>
    public partial class ChartGadget : UserControl
    {
        private static readonly string monthFormatString = "MMM";
        private static readonly string dayFormatString = "MMM dd";

        public ChartGadget()
        {
            InitializeComponent();
        }

        #region Dependency properties

        public static readonly DependencyProperty HeaderStartColorProperty = DependencyProperty.Register(
            "HeaderStartColor", typeof(Color), typeof(ChartGadget), new PropertyMetadata(default(Color)));

        public Color HeaderStartColor
        {
            get { return (Color)GetValue(HeaderStartColorProperty); }
            set { SetValue(HeaderStartColorProperty, value); }
        }

        public static readonly DependencyProperty HeaderStopColorProperty = DependencyProperty.Register(
            "HeaderStopColor", typeof(Color), typeof(ChartGadget), new PropertyMetadata(default(Color)));

        public Color HeaderStopColor
        {
            get { return (Color)GetValue(HeaderStopColorProperty); }
            set { SetValue(HeaderStopColorProperty, value); }
        }

        public static readonly DependencyProperty CaptionColorProperty = DependencyProperty.Register(
            "CaptionColor", typeof(Color), typeof(ChartGadget), new PropertyMetadata(default(Color)));

        public Color CaptionColor
        {
            get { return (Color)GetValue(CaptionColorProperty); }
            set { SetValue(CaptionColorProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(ChartGadget), new PropertyMetadata(default(string)));

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register("SelectedRange", typeof(ChartDisplayRange), typeof(ChartGadget), new PropertyMetadata(ChartDisplayRange.Year, OnSelectedRangeChanged));

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


        #endregion

        private static void OnSelectedRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var chartGadget = d as ChartGadget;

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
    }
}