using DriveHUD.Application.ViewModels.PopupContainers;
using System.Windows;
using System.Windows.Controls;

namespace DriveHUD.Application.Views.PopupContainers
{
    /// <summary>
    /// Interaction logic for PopupFilterView.xaml
    /// </summary>
    public partial class PopupContainerFiltersView : UserControl
    {
        public PopupContainerFiltersView()
        {
            InitializeComponent();
        }
    
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            (DataContext as PopupContainerBaseFilterViewModel).RestoreDefaultFiltersState_Command.Execute(null);
        }
    }
}