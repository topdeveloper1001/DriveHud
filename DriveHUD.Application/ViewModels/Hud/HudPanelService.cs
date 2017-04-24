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

using DriveHUD.Application.Controls;
using DriveHUD.Application.ViewModels.Popups;
using DriveHUD.Application.Views;
using DriveHUD.Application.Views.Popups;
using DriveHUD.Common;
using DriveHUD.Entities;
using System;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

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
        public virtual Tuple<double, double> CalculatePositions(HudBaseToolViewModel toolViewModel, HudWindow window)
        {
            Check.ArgumentNotNull(() => toolViewModel);
            Check.ArgumentNotNull(() => window);

            var panelOffset = window.GetPanelOffset(toolViewModel);

            var xPosition = panelOffset.X != 0 ? panelOffset.X : toolViewModel.Position.X;
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : toolViewModel.Position.Y;

            return new Tuple<double, double>(xPosition * window.ScaleX, yPosition * window.ScaleY);
        }

        /// <summary>
        /// Converts offset values into position value
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public virtual Tuple<double, double> GetOffsetPosition(HudBaseToolViewModel toolViewModel, HudWindow window)
        {
            Check.ArgumentNotNull(() => toolViewModel);
            Check.ArgumentNotNull(() => window);

            var panelOffset = window.GetPanelOffset(toolViewModel);

            var xPosition = panelOffset.X != 0 ? panelOffset.X : toolViewModel.Position.X;
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : toolViewModel.Position.Y;

            return new Tuple<double, double>(xPosition, yPosition);
        }

        /// <summary>
        /// Creates <see cref="FrameworkElement"/>  based on specified <see cref="HudBaseToolViewModel"/>
        /// </summary>
        /// <param name="hudToolElement"><see cref="HudBaseToolViewModel"/></param>
        /// <returns>HUD panel as <see cref="FrameworkElement"/></returns>
        public virtual FrameworkElement Create(HudBaseToolViewModel hudToolElement)
        {
            Check.Require(hudToolElement != null);

            hudToolElement.Opacity = hudToolElement.Opacity / 100d;

            FrameworkElement hudTool = null;

            switch (hudToolElement.ToolType)
            {
                case HudDesignerToolType.PlainStatBox:
                    hudTool = new HudPlainBox
                    {
                        DataContext = hudToolElement
                    };
                    break;
                case HudDesignerToolType.FourStatBox:
                    hudTool = new HudFourStatsBoxDesigner
                    {
                        DataContext = hudToolElement
                    };
                    break;
                case HudDesignerToolType.GaugeIndicator:
                    break;
                default:
                    throw new NotSupportedException($"{hudToolElement.ToolType} isn't supported");
            }

            return hudTool;
        }

        /// <summary>
        /// Creates <see cref="FrameworkElementFactory"/> for the specified <see cref="HudBaseToolViewModel" />
        /// </summary>
        /// <param name="hudToolElement"><see cref="HudBaseToolViewModel"/> to create <see cref="FrameworkElementFactory"/></param>
        /// <returns><see cref="FrameworkElementFactory"/> for the specified <see cref="HudBaseToolViewModel" /></returns>
        public virtual FrameworkElementFactory CreateFrameworkElementFactory(HudBaseToolViewModel hudToolElement)
        {
            FrameworkElementFactory factory = null;

            switch (hudToolElement.ToolType)
            {
                case HudDesignerToolType.GaugeIndicator:
                    factory = new FrameworkElementFactory(typeof(HudGaugeIndicator));
                    break;
                default:
                    break;
            }

            return factory;
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
                hudElement.NoteToolTip = viewModel.Note;
            };
            radMenu.Items.Add(item);

            return radMenu;
        }
    }
}