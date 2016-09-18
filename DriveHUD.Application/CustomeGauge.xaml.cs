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

namespace GaugeExample
{
    /// <summary>
    /// Interaction logic for CustomeGauge.xaml
    /// </summary>
    /// 

   
    public partial class CustomeGauge : UserControl
    {
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(decimal), typeof(CustomeGauge), new PropertyMetadata(default(decimal)));

        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register(
            "Caption", typeof (string), typeof (CustomeGauge), new PropertyMetadata(default(string)));

        public string Caption
        {
            get { return (string) GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty ThemeColorProperty = DependencyProperty.Register(
            "ThemeColor", typeof (Color), typeof (CustomeGauge), new PropertyMetadata(Colors.DarkCyan));

        public Color ThemeColor
        {
            get { return (Color) GetValue(ThemeColorProperty); }
            set { SetValue(ThemeColorProperty, value); }
        }

        public static readonly DependencyProperty GlowColorProperty = DependencyProperty.Register(
            "GlowColor", typeof (Color), typeof (CustomeGauge), new PropertyMetadata(Colors.Cyan));

        public Color GlowColor
        {
            get { return (Color) GetValue(GlowColorProperty); }
            set { SetValue(GlowColorProperty, value); }
        }


        public CustomeGauge()
        {
            InitializeComponent();
        }


    }
}
