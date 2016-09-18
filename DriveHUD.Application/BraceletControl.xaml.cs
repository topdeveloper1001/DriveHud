using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace DriveHUD.Application
{
    /// <summary>
    /// Interaction logic for BraceletControl.xaml
    /// </summary>
    public partial class BraceletControl : UserControl
    {
        #region Color
        public SolidColorBrush BraceletSideBrush
        {
            get { return (SolidColorBrush)GetValue(BraceletSideBrushProperty); }
            set { SetValue(BraceletSideBrushProperty, value); }
        }

        public static readonly DependencyProperty BraceletSideBrushProperty =
            DependencyProperty.Register("BraceletSideBrush",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF694A02"))));


        public SolidColorBrush BraceletTopBrush
        {
            get { return (SolidColorBrush)GetValue(BraceletTopBrushProperty); }
            set { SetValue(BraceletTopBrushProperty, value); }
        }

        public static readonly DependencyProperty BraceletTopBrushProperty =
            DependencyProperty.Register("BraceletTopBrush",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE5A715"))));

        public SolidColorBrush BraceletInsideBrush
        {
            get { return (SolidColorBrush)GetValue(BraceletInsideBrushProperty); }
            set { SetValue(BraceletInsideBrushProperty, value); }
        }

        public static readonly DependencyProperty BraceletInsideBrushProperty =
            DependencyProperty.Register("BraceletInsideBrush",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF463306"))));


        public SolidColorBrush BraceletLightBrush
        {
            get { return (SolidColorBrush)GetValue(BraceletLightBrushProperty); }
            set { SetValue(BraceletLightBrushProperty, value); }
        }

        public static readonly DependencyProperty BraceletLightBrushProperty =
            DependencyProperty.Register("BraceletLightBrush",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFDC8A"))));



        public SolidColorBrush BraceletFrontOuterBrush
        {
            get { return (SolidColorBrush)GetValue(BraceletFrontOuterBrushProperty); }
            set { SetValue(BraceletFrontOuterBrushProperty, value); }
        }

        public static readonly DependencyProperty BraceletFrontOuterBrushProperty =
            DependencyProperty.Register("BraceletFrontOuterBrush",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFE3A209"))));

        public Color BraceletFrontDarkColor
        {
            get { return (Color)GetValue(BraceletFrontDarkColorProperty); }
            set { SetValue(BraceletFrontDarkColorProperty, value); }
        }

        public static readonly DependencyProperty BraceletFrontDarkColorProperty =
            DependencyProperty.Register("BraceletFrontDarkColor",
                typeof(Color),
                typeof(BraceletControl),
                new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FFFFC845")));



        public Color BraceletFrontLightColor
        {
            get { return (Color)GetValue(BraceletFrontLightColorProperty); }
            set { SetValue(BraceletFrontLightColorProperty, value); }
        }

        public static readonly DependencyProperty BraceletFrontLightColorProperty =
            DependencyProperty.Register("BraceletFrontLightColor",
                typeof(Color),
                typeof(BraceletControl),
                new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FFFFDC8A")));

        public Color BraceletFrontSemiLightColor
        {
            get { return (Color)GetValue(BraceletFrontSemiLightColorProperty); }
            set { SetValue(BraceletFrontSemiLightColorProperty, value); }
        }

        public static readonly DependencyProperty BraceletFrontSemiLightColorProperty =
            DependencyProperty.Register("BraceletFrontSemiLightColor",
                typeof(Color),
                typeof(BraceletControl),
                new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FFF8BE37")));

        public Color BraceletFrontSemiDarkColor
        {
            get { return (Color)GetValue(BraceletFrontSemiDarkColorProperty); }
            set { SetValue(BraceletFrontSemiDarkColorProperty, value); }
        }

        public static readonly DependencyProperty BraceletFrontSemiDarkColorProperty =
            DependencyProperty.Register("BraceletFrontSemiDarkColor",
                typeof(Color),
                typeof(BraceletControl),
                new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FFF0B325")));

        public Color BraceletCircleColor
        {
            get { return (Color)GetValue(BraceletCircleColorProperty); }
            set { SetValue(BraceletCircleColorProperty, value); }
        }

        public static readonly DependencyProperty BraceletCircleColorProperty =
            DependencyProperty.Register("BraceletCircleColor",
                typeof(Color),
                typeof(BraceletControl),
                new PropertyMetadata((Color)ColorConverter.ConvertFromString("#FFE3A209")));



        public SolidColorBrush HeaderColor
        {
            get { return (SolidColorBrush)GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderColorProperty =
            DependencyProperty.Register("HeaderColor",
                typeof(SolidColorBrush),
                typeof(BraceletControl),
                new PropertyMetadata(new SolidColorBrush()));


        #endregion

        public static readonly DependencyProperty CommandProperty =
          DependencyProperty.Register("Command", typeof(ICommand), typeof(BraceletControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(BraceletControl), new UIPropertyMetadata(null));

        public static readonly DependencyProperty CommandTargetProperty =
          DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(BraceletControl), new UIPropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }


        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public BraceletControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var command = Command;
            var parameter = CommandParameter ??
                ((sender is Button) ? (sender as Button).DataContext : null);
            var target = CommandTarget;

            var routedCmd = command as RoutedCommand;
            if (routedCmd != null && routedCmd.CanExecute(parameter, target))
            {
                routedCmd.Execute(parameter, target);
            }
            else if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
