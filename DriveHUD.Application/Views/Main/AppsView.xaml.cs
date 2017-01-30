using Model.Shop;
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

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for AppsView.xaml
    /// </summary>
    public partial class AppsView : UserControl
    {
        public AppsView()
        {
            InitializeComponent();
        }
    }

    public class AppsTestViewModel
    {
        public AppsTestViewModel()
        {
            Pages = Enumerable.Range(1, 7).Select(x => new PageViewModel
            {
                PageNumber = x,
                IsSelected = x == 5 || x == 7 || x == 1
            }).ToList();
        }

        public List<PageViewModel> Pages { get; set; }
    }

    public class PageViewModel
    {
        public int PageNumber { get; set; }

        public bool IsSelected { get; set; }
    }
}