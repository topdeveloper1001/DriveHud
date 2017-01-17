//-----------------------------------------------------------------------
// <copyright file="HudView.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using Model.Enums;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using System;
using System.Linq;

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for HudContentView.xaml
    /// </summary>
    public partial class HudView : UserControl
    {
        public HudView()
        {
            InitializeComponent();

            DataContextChanged += (o, e) =>
            {
                if (ViewModel == null)
                    return;
                ViewModel.TableUpdate += ViewModel_TableUpdated;
                ViewModel.UpdateActiveLayout();
                var tableType = ViewModel.CurrentTableType?.TableType ?? EnumTableType.Six;
                Configurator.ConfigureTable(diagram, ViewModel.CurrentHudTableViewModel, (int) tableType);
                UpdatePreferredSeatingStateWithoutNotification();
            };
        }

        private void ViewModel_TableUpdated(object sender, EventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            var tableType = ViewModel.CurrentTableType?.TableType ?? EnumTableType.Six;
            Configurator.ConfigureTable(diagram, ViewModel.CurrentHudTableViewModel, (int) tableType);
            UpdatePreferredSeatingStateWithoutNotification();
        }

        private void UpdatePreferredSeatingStateWithoutNotification()
        {
            ViewModel.UpdateSeatContextMenuState();
        }

        private HudViewModel ViewModel
        {
            get { return DataContext as HudViewModel; }
        }

        private ITableConfigurator Configurator
        {
            get
            {
                var pokerSite = ViewModel.CurrentPokerSite ?? ViewModel.DefaultPokerSite;
                return
                    Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITableConfigurator>(
                        TableConfiguratorHelper.GetServiceName(pokerSite, ViewModel.HudType));
            }
        }

        private void OnDiagramViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }
    }
}