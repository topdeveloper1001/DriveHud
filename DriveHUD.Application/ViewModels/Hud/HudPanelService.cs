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
using System;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;
using DriveHUD.Entities;
using DriveHUD.Common.Resources;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Determines service to create HUD tools
    /// </summary>
    internal class HudPanelService : IHudPanelService
    {
        /// <summary>
        /// Calculates hudElement position in window
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public virtual Tuple<double, double> CalculatePositions(HudBaseToolViewModel toolViewModel, FrameworkElement toolElement, HudWindow window)
        {
            Check.ArgumentNotNull(() => toolViewModel);
            Check.ArgumentNotNull(() => window);
            Check.ArgumentNotNull(() => toolElement);

            var panelOffset = window.GetPanelOffset(toolViewModel);

            var xPosition = panelOffset.X != 0 ? panelOffset.X : toolViewModel.Position.X;
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : toolViewModel.Position.Y;

            var scaledXPosition = xPosition * window.ScaleX;
            var scaledYPosition = yPosition * window.ScaleY;

            if (scaledXPosition < 0)
            {
                scaledXPosition = 0;
                toolViewModel.OffsetX = 0;
            }

            if (scaledYPosition + HudDefaultSettings.HudIconHeaderHeight < 0)
            {
                scaledYPosition = -HudDefaultSettings.HudIconHeaderHeight;
                toolViewModel.OffsetY = -HudDefaultSettings.HudIconHeaderHeight;
            }

            var toolWidth = toolElement.ActualWidth != 0 && !double.IsNaN(toolElement.ActualWidth) ?
                toolElement.ActualWidth :
                (!double.IsNaN(toolViewModel.Width) ? toolViewModel.Width : 0);

            var toolHeight = toolElement.ActualHeight != 0 && !double.IsNaN(toolElement.ActualHeight) ?
                toolElement.ActualHeight :
                !double.IsNaN(toolViewModel.Height) ? toolViewModel.Height : 0;

            if (scaledXPosition > window.Width - toolWidth)
            {
                scaledXPosition = window.Width - toolWidth;
                toolViewModel.OffsetX = window.ScaleX != 0 ? scaledXPosition / window.ScaleX : toolViewModel.OffsetX;
            }

            if (scaledYPosition > window.Height - toolHeight - HudDefaultSettings.HudIconHeaderHeight)
            {
                scaledYPosition = window.Height - toolHeight - HudDefaultSettings.HudIconHeaderHeight;
                toolViewModel.OffsetY = window.ScaleY != 0 ?
                    ((window.Height - toolHeight) / window.ScaleY - HudDefaultSettings.HudIconHeaderHeight) :
                    toolViewModel.OffsetY;
            }

            return new Tuple<double, double>(scaledXPosition, scaledYPosition);
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

            FrameworkElement hudTool = null;

            switch (hudToolElement.ToolType)
            {
                case HudDesignerToolType.PlainStatBox:
                    hudTool = new HudPlainBox();
                    AttachContextMenu(hudToolElement, hudTool);
                    break;
                case HudDesignerToolType.FourStatBox:
                    hudTool = new HudFourStatsBox();
                    break;
                case HudDesignerToolType.TiltMeter:
                    hudTool = new HudTiltMeter();
                    break;
                case HudDesignerToolType.PlayerProfileIcon:
                    hudTool = new HudPlayerIcon();
                    break;
                case HudDesignerToolType.TextBox:
                    hudTool = new HudTextBox();
                    break;
                case HudDesignerToolType.BumperStickers:
                    hudTool = new HudBumperStickers();
                    break;
                case HudDesignerToolType.GaugeIndicator:
                case HudDesignerToolType.Graph:
                    break;
                default:
                    throw new NotSupportedException($"{hudToolElement.ToolType} isn't supported");
            }

            if (hudTool != null)
            {
                hudTool.DataContext = hudToolElement;
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
                case HudDesignerToolType.Graph:
                    factory = new FrameworkElementFactory(typeof(HudGraph));
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

        /// <summary>
        /// Attaches context menu to the specified <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="hudToolViewModel"></param>
        /// <param name="hudTool"></param>
        protected virtual void AttachContextMenu(HudBaseToolViewModel hudToolViewModel, FrameworkElement hudTool)
        {
            if (hudToolViewModel == null || hudToolViewModel.Parent == null || hudTool == null)
            {
                return;
            }

            var menuItem = new RadMenuItem();

            var binding = new Binding(nameof(HudElementViewModel.NoteMenuItemText))
            {
                Source = hudToolViewModel.Parent,
                Mode = BindingMode.OneWay
            };

            menuItem.SetBinding(RadMenuItem.HeaderProperty, binding);

            menuItem.Click += (s, e) =>
            {
                var playerNoteViewModel = new PlayerNoteViewModel(hudToolViewModel.Parent.PokerSiteId, hudToolViewModel.Parent.PlayerName);

                var playerNoteView = new PlayerNoteView(playerNoteViewModel);

                playerNoteView.ShowDialog();

                if (playerNoteViewModel.PlayerNoteEntity == null)
                {
                    return;
                }

                hudToolViewModel.Parent.NoteToolTip = playerNoteViewModel.Note;
            };

            var contextMenu = new RadContextMenu();
            contextMenu.Items.Add(menuItem);
            RadContextMenu.SetContextMenu(hudTool, contextMenu);
        }

        public virtual Point GetPositionShift(EnumTableType tableType, int seat)
        {
            return new Point(0, 0);
        }
    }
}