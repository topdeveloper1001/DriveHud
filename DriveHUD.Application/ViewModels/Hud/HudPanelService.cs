//-----------------------------------------------------------------------
// <copyright file="HudPanelService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.ViewModels;
using Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using DriveHUD.Application.Controls;
using DriveHUD.Application.Views;
using DriveHUD.Common;
using Telerik.Windows.Controls;
using DriveHUD.Application.Views.Popups;
using DriveHUD.Application.ViewModels.Popups;
using DriveHUD.Common.Resources;
using System.Windows.Data;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Creates Hud Panels 
    /// </summary>
    internal class HudPanelService : IHudPanelService
    {
        /// <summary>
        /// Calculates hudElement position in window
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public virtual Tuple<double, double> CalculatePositions(HudElementViewModel hudElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => hudElement);
            Check.ArgumentNotNull(() => window);

            return new Tuple<double, double>(hudElement.Position.X, hudElement.Position.Y);
        }

        /// <summary>
        /// Create Hud Panel based on specified HUD element view model
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <returns>HUD panel</returns>
        public virtual FrameworkElement Create(HudElementViewModel hudElement, HudType hudType)
        {
            var contextMenu = CreateContextMenu(hudElement.PokerSiteId, hudElement.PlayerName, hudElement);

            if (hudType == HudType.Plain)
            {
                var panel = new HudPanel
                {
                    DataContext = hudElement
                };

                RadContextMenu.SetContextMenu(panel, contextMenu);

                return panel;
            }

            var richPanel = new HudRichPanel
            {
                DataContext = hudElement
            };

            RadContextMenu.SetContextMenu(richPanel, contextMenu);

            return richPanel;
        }

        /// <summary>
        /// Get initial table size 
        /// </summary>
        /// <returns>Return dimensions of initial table, Item1 - Width, Item - Height</returns>
        public virtual Tuple<double, double> GetInitialTableSize()
        {
            return new Tuple<double, double>(812, 648);
        }

        public virtual Tuple<double, double> GetInitialTrackConditionMeterPosition()
        {
            return new Tuple<double, double>(150, -30);
        }

        /// <summary>
        /// Get handle of window on which hud has to be attached
        /// </summary>
        /// <returns>Handle of window</returns>
        public virtual IntPtr GetWindowHandle(IntPtr handle)
        {
            return handle;
        }

        private RadContextMenu CreateContextMenu(short pokerSiteId, string playerName, HudElementViewModel datacontext)
        {
            RadContextMenu radMenu = new RadContextMenu();

            var item = new RadMenuItem();

            var binding = new Binding(nameof(HudElementViewModel.NoteMenuItemText)) { Source = datacontext, Mode = BindingMode.OneWay };
            item.SetBinding(RadMenuItem.HeaderProperty, binding);

            item.Click += (s, e) =>
            {
                PlayerNoteViewModel viewModel = new PlayerNoteViewModel(pokerSiteId, playerName);
                var frm = new PlayerNoteView(viewModel);
                frm.ShowDialog();

                if (viewModel.PlayerNoteEntity == null)
                {
                    return;
                }

                var clickedItem = s as FrameworkElement;
                if (clickedItem == null || !(clickedItem.DataContext is HudElementViewModel))
                {
                    return;
                }

                var hudElement = clickedItem.DataContext as HudElementViewModel;
                hudElement.IsNoteIconVisible = !string.IsNullOrWhiteSpace(viewModel.Note);
            };
            radMenu.Items.Add(item);

            return radMenu;
        }
    }
}