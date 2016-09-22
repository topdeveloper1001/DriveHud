using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels.Settings;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
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

            this.DataContextChanged += (o, e) =>
            {
                if (ViewModel == null)
                {
                    return;
                }

                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
                ViewModel.PropertyChanged += ViewModel_PropertyChanged;

                Configurator?.ConfigureTable(diagram, ViewModel.SelectedSite, ViewModel.SelectedTableType);
            };
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsSiteViewModel.SelectedSiteType)
                || e.PropertyName == nameof(SettingsSiteViewModel.SelectedTableType))
            {
                Configurator?.ConfigureTable(diagram, ViewModel.SelectedSite, ViewModel.SelectedTableType);
            }
        }

        private ISiteSettingTableConfigurator Configurator
        {
            get
            {
                ISiteSettingTableConfigurator configurator = null;
                try
                {
                    configurator = ServiceLocator.Current.GetInstance<ISiteSettingTableConfigurator>(ViewModel.SelectedSiteType.ToString());
                }
                catch (ActivationException) when (ViewModel.SelectedSiteType == Entities.EnumPokerSites.Unknown)
                {
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, $"Failed to load configurator for {ViewModel?.SelectedSiteType}", ex);
                }

                return configurator;
            }
        }

        private SettingsSiteViewModel ViewModel
        {
            get { return DataContext as SettingsSiteViewModel; }
        }


        private void OnDiagramViewportChanged(object sender, Telerik.Windows.Diagrams.Core.PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }

    }
}
