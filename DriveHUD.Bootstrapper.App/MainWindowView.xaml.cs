using DriveHUD.Bootstrapper.App.ViewModels;
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
using System.Windows.Shapes;

namespace DriveHUD.Bootstrapper.App
{
    /// <summary>
    /// Interaction logic for MainWindowView.xaml
    /// </summary>
    public partial class MainWindowView : Window
    {
        public MainWindowView(MainWindowViewModel viewModel)
        {
            this.DataContext = viewModel;
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ICancelable viewModel = ((this.DataContext as MainWindowViewModel)?.SelectedView as UserControl)?.DataContext as ICancelable;
            if(viewModel != null)
            {
                e.Cancel = true;
                viewModel.Cancel(null);
            }
        }
    }
}
