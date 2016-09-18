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
using DriveHUD.Application.ViewModels.Filters;

namespace DriveHUD.Application.Views
{
    public partial class FilterHoleCardsView : UserControl, IFilterView
    {
        public FilterHoleCardsView()
        {
            this.DataContext = new FilterHoleCardsViewModel();

            InitializeComponent();
        }

        public IFilterViewModel ViewModel
        {
            get { return (this.DataContext as IFilterViewModel); }
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }
    }
}
