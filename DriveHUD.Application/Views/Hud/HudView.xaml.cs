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
using DriveHUD.Common.Reflection;
using DriveHUD.Entities;
using Model.Enums;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Diagrams.Core;

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
                {
                    return;
                }

                ViewModel.PropertyChanged += ViewModel_PropertyChanged;

                var tableType = ViewModel.CurrentTableLayout != null && ViewModel.CurrentTableLayout.HudTableLayout != null ?
                                    ViewModel.CurrentTableLayout.HudTableLayout.TableType : EnumTableType.Six;
                Configurator.ConfigureTable(diagram, ViewModel.HudTableViewModelCurrent, (int)tableType);
                UpdatePreferredSeatingStateWithoutNotification();
            };
        }

        private void UpdatePreferredSeatingStateWithoutNotification()
        {
            ViewModel.UpdateSeatContextMenuState();
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModel == null)
            {
                return;
            }

            if (e.PropertyName == ReflectionHelper.GetPath<HudViewModel>(o => o.CurrentTableLayout) ||
                        e.PropertyName == ReflectionHelper.GetPath<HudViewModel>(o => o.HudType) ||
                            e.PropertyName == ReflectionHelper.GetPath<HudViewModel>(o => o.GameType))
            {
                var tableType = ViewModel.CurrentTableLayout != null && ViewModel.CurrentTableLayout.HudTableLayout != null ?
                                    ViewModel.CurrentTableLayout.HudTableLayout.TableType : EnumTableType.Six;
                Configurator.ConfigureTable(diagram, ViewModel.HudTableViewModelCurrent, (int)tableType);
                UpdatePreferredSeatingStateWithoutNotification();
            }
        }

        private HudViewModel ViewModel
        {
            get { return DataContext as HudViewModel; }
        }

        private ITableConfigurator Configurator
        {
            get
            {
                var site = ViewModel.CurrentTableLayout != null && ViewModel.CurrentTableLayout.HudTableLayout != null ?
                                ViewModel.CurrentTableLayout.HudTableLayout.Site : EnumPokerSites.Bovada;
                return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITableConfigurator>(TableConfiguratorHelper.GetServiceName(site, ViewModel.HudType));
            }
        }

        private void OnDiagramViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }
    }
}
