using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for ChartGadget.xaml
    /// </summary>
    public partial class ChartGadget : UserControl
    {
        private static readonly String monthFormatString = "MMM";
        private static readonly String dayFormatString = "MMM dd";
        private static readonly String hourFormatString = "hh tt";

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

        // Using a DependencyProperty as the backing store for SelectedRange.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedRangeProperty =
            DependencyProperty.Register("SelectedRange", typeof(Model.Enums.EnumTelerikRadChartDisplayRange), typeof(ChartGadget), new PropertyMetadata(Model.Enums.EnumTelerikRadChartDisplayRange.Year, OnSelectedRangeChanged));

        private static void OnSelectedRangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var currentControl = d as ChartGadget;
            switch (currentControl.SelectedRange)
            {
                case Model.Enums.EnumTelerikRadChartDisplayRange.Year:
                    if (currentControl.YearBtn.IsChecked.HasValue && !currentControl.YearBtn.IsChecked.Value)
                        currentControl.YearBtn.IsChecked = true;
                    currentControl.AxisDateTime.LabelFormat = monthFormatString;
                    break;
                case Model.Enums.EnumTelerikRadChartDisplayRange.Month:
                    if (currentControl.MonthBtn.IsChecked.HasValue && !currentControl.MonthBtn.IsChecked.Value)
                        currentControl.MonthBtn.IsChecked = true;
                    currentControl.AxisDateTime.LabelFormat = dayFormatString;
                    break;
                case Model.Enums.EnumTelerikRadChartDisplayRange.Week:
                    if (currentControl.WeekBtn.IsChecked.HasValue && !currentControl.WeekBtn.IsChecked.Value)
                        currentControl.WeekBtn.IsChecked = true;
                    currentControl.AxisDateTime.LabelFormat = dayFormatString;
                    break;
            }
        }

        public Model.Enums.EnumTelerikRadChartDisplayRange SelectedRange
        {
            get { return (Model.Enums.EnumTelerikRadChartDisplayRange)GetValue(SelectedRangeProperty); }
            set
            {
                SetValue(SelectedRangeProperty, value);
            }
        }

        public ChartGadget()
        {
            InitializeComponent();
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton)
            {
                var btn = sender as RadioButton;
                if (btn.Name.Equals(YearBtn.Name)
                    && YearBtn.IsChecked.HasValue
                    && YearBtn.IsChecked.Value)
                {
                    SelectedRange = Model.Enums.EnumTelerikRadChartDisplayRange.Year;
                }
                else if ((btn.Name.Equals(MonthBtn.Name)
                    && MonthBtn.IsChecked.HasValue
                    && MonthBtn.IsChecked.Value))
                {
                    SelectedRange = Model.Enums.EnumTelerikRadChartDisplayRange.Month;
                }
                else if ((btn.Name.Equals(WeekBtn.Name)
                   && WeekBtn.IsChecked.HasValue
                   && WeekBtn.IsChecked.Value))
                {
                    SelectedRange = Model.Enums.EnumTelerikRadChartDisplayRange.Week;
                }
            }
        }
    }
}
