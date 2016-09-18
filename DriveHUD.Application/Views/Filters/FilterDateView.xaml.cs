using DriveHUD.Application.ViewModels.Filters;
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

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for FilterDateView.xaml
    /// </summary>
    public partial class FilterDateView : UserControl, IFilterView
    {
        public FilterDateView()
        {
            InitializeComponent();

            this.DataContext = new FilterDateViewModel();
        }

        public IFilterViewModel ViewModel
        {
            get { return (this.DataContext as IFilterViewModel); }
        }
    }
}
