//-----------------------------------------------------------------------
// <copyright file="FilterStandardView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.TableConfigurators;
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Filters;
using Model.Filters;
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

            DataContext = new FilterStandardViewModel(service);
            Configurator.ConfigureTable(diagram, viewModel, 6);
        }

        public IFilterViewModel ViewModel
        {
            get { return DataContext as IFilterViewModel; }
        }

        private IFilterTableConfigurator Configurator
        {
            get { return new FilterBaseTableConfigurator(); }
        }

        private FilterStandardViewModel viewModel
        {
            get { return (FilterStandardViewModel)DataContext; }
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

            if (sender is RadioButton button)
            {
                var seats = int.Parse(button.Tag.ToString());
                Configurator.ConfigureTable(diagram, viewModel, seats);
            }
        }
      
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (viewModel == null)
            {
                return;
            }

            Configurator.ConfigureTable(diagram, viewModel);
        }
    }
}