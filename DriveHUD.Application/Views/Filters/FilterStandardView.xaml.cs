using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Filters;
using Model.Filters;
using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

namespace DriveHUD.Application.Views
{
    public partial class FilterStandardView : UserControl, IFilterView
    {
        public FilterStandardView(IFilterModelManagerService service)
        {
            InitializeComponent();

            this.DataContext = new FilterStandardViewModel(service);
            Configurator.ConfigureTable(diagram, viewModel, 6);
        }

        public IFilterViewModel ViewModel
        {
            get { return (this.DataContext as IFilterViewModel); }
        }

        private IFilterTableConfigurator Configurator
        {
            get { return new FilterBaseTableConfigurator(); }
        }

        private FilterStandardViewModel viewModel
        {
            get { return (FilterStandardViewModel)this.DataContext; }
        }

        private void OnDiagramViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }

        private void TableRingRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (viewModel == null)
            {
                return;
            }

            if (sender is RadioButton)
            {
                var button = (RadioButton)sender;
                int seats = Int32.Parse(button.Tag.ToString());
                Configurator.ConfigureTable(diagram, viewModel, seats);
            }
        }
    }
}