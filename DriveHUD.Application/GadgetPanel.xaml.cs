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
    /// Interaction logic for GadgetPanel.xaml
    /// </summary>
    public partial class GadgetPanel : UserControl
    {

        public static readonly DependencyProperty HeaderColorProperty = DependencyProperty.Register(
            "HeaderColor", typeof(Brush), typeof(GadgetPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush HeaderColor
        {
            get { return (Brush) GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        public GadgetPanel()
        {
            InitializeComponent();
        }
    }
}
