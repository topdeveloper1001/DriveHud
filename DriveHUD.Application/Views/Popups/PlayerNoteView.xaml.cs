using DriveHUD.Application.ViewModels.Popups;
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

namespace DriveHUD.Application.Views.Popups
{
    /// <summary>
    /// Interaction logic for PlayerNoteView.xaml
    /// </summary>
    public partial class PlayerNoteView : Window
    {
        public PlayerNoteView(PlayerNoteViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.CloseAction = this.Close;
        }
    }
}
