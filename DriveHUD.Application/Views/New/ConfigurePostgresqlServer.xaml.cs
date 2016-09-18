using System.Windows;
using DriveHUD.Application.ViewModels;

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for ConfigurePostgresqlServer.xaml
    /// </summary>
    public partial class ConfigurePostgresqlServer : Window
    {
        public ConfigurePostgresqlServer(ConfigurePostgresqlServerViewModel vm)
        {
            InitializeComponent();
            vm.CloseAction += CloseAction;
            vm.AfterConnectAction += AfterConnectAction;
            DataContext = vm;
            vm.Dispatcher = this.Dispatcher;
        }

        private void AfterConnectAction()
        {
            if (ConfigurePostgresqlServerViewModel.IsConnected)
            {
                this.Close();
            }
        }

        private void CloseAction()
        {
            this.Close();
        }
    }
}



