using DriveHUD.Application.ViewModels;
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

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for EditTournamentView.xaml
    /// </summary>
    public partial class EditTournamentView : Window
    {
        public EditTournamentView(EditTournamentViewModel vm)
        { 
            InitializeComponent();
            vm.CloseAction = this.Close;
            DataContext = vm;
        }
    }
}
