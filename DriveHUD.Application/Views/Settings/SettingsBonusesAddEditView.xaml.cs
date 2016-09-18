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

namespace DriveHUD.Application.Views.Settings
{
    /// <summary>
    /// Interaction logic for SettingsBonusesEditView.xaml
    /// </summary>
    public partial class SettingsBonusesAddEditView : UserControl
    {
        public SettingsBonusesAddEditView()
        {
            InitializeComponent();
        }

        private void RadDateTimePicker_Loaded(object sender, RoutedEventArgs e)
        {
            //var datePicker = sender as RadDateTimePicker;
            //var datePickerTextBox = datePicker.ChildrenOfType<RadWatermarkTextBox>().FirstOrDefault() as RadWatermarkTextBox;
            //datePickerTextBox.IsReadOnly = true;
        }
    }
}
