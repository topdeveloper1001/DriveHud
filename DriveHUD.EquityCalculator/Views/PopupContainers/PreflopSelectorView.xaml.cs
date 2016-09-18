using DriveHUD.EquityCalculator.ViewModels;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using Telerik.Windows.Controls;
using DriveHUD.EquityCalculator.Models;

namespace DriveHUD.EquityCalculator.Views
{
    /// <summary>
    /// Interaction logic for PreflopSelectorView.xaml
    /// </summary>
    public partial class PreflopSelectorView : UserControl
    {
        public PreflopSelectorView()
        {
            InitializeComponent();
        }

        private void Grid_MouseEnter(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Released)
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
