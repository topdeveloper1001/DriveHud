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
            Loaded += PopupContainerFiltersView_Loaded;
        }

        private void PopupContainerFiltersView_Loaded(object sender, RoutedEventArgs e)
        {
            contentPresenter.Visibility = Visibility.Collapsed;
            contentPresenter.Visibility = Visibility.Visible;
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            (DataContext as PopupContainerBaseFilterViewModel).RestoreDefaultFiltersState_Command.Execute(null);
        }
    }
}