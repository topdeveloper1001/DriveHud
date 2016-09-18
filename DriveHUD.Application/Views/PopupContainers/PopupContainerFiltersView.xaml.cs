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

using Telerik.Windows.Controls;

using DriveHUD.Application.ViewModels.PopupContainers;

namespace DriveHUD.Application.Views.PopupContainers
{
    /// <summary>
    /// Interaction logic for PopupFilterView.xaml
    /// </summary>
    public partial class PopupContainerFiltersView : UserControl
    {
        public PopupContainerFiltersView()
        {
            DataContext = new PopupContainerFiltersViewModel();

            InitializeComponent();
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PopupContainerFiltersViewModel).Apply_CommandClick.Execute(null);
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            (DataContext as PopupContainerFiltersViewModel).Ok_CommandClick.Execute(null);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            (DataContext as PopupContainerFiltersViewModel).RestoreDefaultFiltersState();
        }
    }
}
