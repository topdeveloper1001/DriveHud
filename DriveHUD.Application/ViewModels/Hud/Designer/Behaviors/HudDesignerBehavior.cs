//-----------------------------------------------------------------------
// <copyright file="HudDesignerBehavior.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Diagrams.Core;
using Microsoft.Practices.ServiceLocation;

namespace DriveHUD.Application.ViewModels.Hud.Designer.Behaviors
{
    public class HudDesignerBehavior : Behavior<RadDiagram>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.ViewportChanged += OnViewportChanged;
            AssociatedObject.Unloaded += OnUnloaded;
            AssociatedObject.Initialized += OnInitialized;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            var diagram = sender as RadDiagram;

            if (diagram == null)
            {
                return;
            }

            var hudTableViewModel = new HudTableViewModel
            {
                HudViewType = HudViewType.CustomDesigned,
                TableType = EnumTableType.HU,
                GameType = EnumGameType.CashHoldem,
                HudElements = new ObservableCollection<HudElementViewModel>()
                {
                    new HudElementViewModel
                    {
                         Seat = 1
                    }
                }
            };

            var tableConfigurator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<ITableConfigurator>();
            tableConfigurator.ConfigureTable(diagram, hudTableViewModel, 2);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {

        }

        protected override void OnDetaching()
        {
            AssociatedObject.ViewportChanged -= OnViewportChanged;
            AssociatedObject.Unloaded -= OnUnloaded;
            AssociatedObject.Initialized -= OnInitialized;

            base.OnDetaching();
        }

        private void OnViewportChanged(object sender, PropertyEventArgs<Rect> e)
        {
            var diagram = sender as RadDiagram;

            if (diagram == null)
            {
                return;
            }

            diagram.BringIntoView(new Rect(1, 1, e.NewValue.Width, e.NewValue.Height), false);
        }
    }
}