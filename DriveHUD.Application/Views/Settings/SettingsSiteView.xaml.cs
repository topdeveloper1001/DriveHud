//-----------------------------------------------------------------------
// <copyright file="SettingsSiteView.xaml" company="Ace Poker Solutions">
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
using DriveHUD.Application.ViewModels.Settings;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Windows;
using System.Windows.Controls;

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

                Configurator?.ConfigureTable(diagram, ViewModel, ViewModel.SelectedTableType);
            };
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsSiteViewModel.SelectedSiteType)
                || e.PropertyName == nameof(SettingsSiteViewModel.SelectedTableType))
            {
                Configurator?.ConfigureTable(diagram, ViewModel, ViewModel.SelectedTableType);
            }
        }

        private ISiteSettingTableConfigurator Configurator
        {
            get
            {
                ISiteSettingTableConfigurator configurator = null;

                try
                {
                    if (ViewModel != null)
                    {
                        configurator = ServiceLocator.Current.GetInstance<ISiteSettingTableConfigurator>(ViewModel.SelectedSiteType.ToString());
                    }
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

        private void diagram_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
