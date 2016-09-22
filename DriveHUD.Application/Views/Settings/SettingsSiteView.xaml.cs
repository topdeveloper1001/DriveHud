using DriveHUD.Application.TableConfigurators;
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

namespace DriveHUD.Application.Views.Settings
{
    /// <summary>
    /// Interaction logic for SiteSettingsView.xaml
    /// </summary>
    public partial class SettingsSiteView : UserControl
    {
        public SettingsSiteView()
        {
            InitializeComponent();

            Configurator.ConfigureTable(diagram, new ViewModels.FilterStandardViewModel(), 6);
        }

        private IFilterTableConfigurator Configurator
        {
            get { return new FilterBaseTableConfigurator(); }
        }

        private void OnDiagramViewportChanged(object sender, Telerik.Windows.Diagrams.Core.PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }
    }
}
