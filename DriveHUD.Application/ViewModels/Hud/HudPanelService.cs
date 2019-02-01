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
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using System;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Hud
{
    /// <summary>
    /// Determines service to create HUD tools
    /// </summary>
    internal class HudPanelService : IHudPanelService
    {
        private static readonly Point initialTableSize = new Point(811, 677);

        /// <summary>
        /// Gets the initial(default) size of the table 
        /// </summary>
        public virtual Point InitialTableSize
        {
            get
            {
                return initialTableSize;
            }
        }

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

            var panelOffset = window.ViewModel.GetPanelOffset(toolViewModel);

            var xPosition = panelOffset.X != 0 ? panelOffset.X : toolViewModel.Position.X;
            var yPosition = panelOffset.Y != 0 ? panelOffset.Y : toolViewModel.Position.Y;

            var scaledXPosition = xPosition * window.ScaleX;
            var scaledYPosition = yPosition * window.ScaleY;

            // border limits
            var toolWidth = toolElement.ActualWidth != 0 && !double.IsNaN(toolElement.ActualWidth) ?
                toolElement.ActualWidth :
                (!double.IsNaN(toolViewModel.Width) ? toolViewModel.Width : 0);

            var toolHeight = toolElement.ActualHeight != 0 && !double.IsNaN(toolElement.ActualHeight) ?
                toolElement.ActualHeight :
                !double.IsNaN(toolViewModel.Height) ? toolViewModel.Height : 0;

            var widthLimit = toolWidth >= HudDefaultSettings.MinimumDistanceToTheBorder ? HudDefaultSettings.MinimumDistanceToTheBorder : toolWidth;
            var heightLimit = toolHeight >= HudDefaultSettings.MinimumDistanceToTheBorder ? HudDefaultSettings.MinimumDistanceToTheBorder : toolHeight;

            if (toolWidth == 0 || toolWidth >= window.Width ||
                widthLimit >= window.Width || toolHeight == 0 ||
                toolHeight >= window.Height || (heightLimit + HudDefaultSettings.HudIconHeaderHeight) >= window.Height)
            {
                return new Tuple<double, double>(scaledXPosition, scaledYPosition);
            }

            if (scaledXPosition > window.Width - widthLimit)
            {
                scaledXPosition = window.Width - widthLimit;
                toolViewModel.OffsetX = window.ScaleX != 0 ? scaledXPosition / window.ScaleX : toolViewModel.OffsetX;
            }
            else if (scaledXPosition < widthLimit - toolWidth)
            {
                scaledXPosition = widthLimit - toolWidth;
                toolViewModel.OffsetX = widthLimit - toolWidth;
            }

            if (scaledYPosition > window.Height - heightLimit - HudDefaultSettings.HudIconHeaderHeight)
            {
                scaledYPosition = window.Height - heightLimit - HudDefaultSettings.HudIconHeaderHeight;
                toolViewModel.OffsetY = window.ScaleY != 0 ?
                    ((window.Height - heightLimit) / window.ScaleY - HudDefaultSettings.HudIconHeaderHeight) :
                    toolViewModel.OffsetY;
            }
            else if (scaledYPosition < heightLimit - toolHeight - HudDefaultSettings.HudIconHeaderHeight)
            {
                scaledYPosition = heightLimit - toolHeight - HudDefaultSettings.HudIconHeaderHeight;
                toolViewModel.OffsetY = heightLimit - toolHeight - HudDefaultSettings.HudIconHeaderHeight;
            }

            return new Tuple<double, double>(scaledXPosition, scaledYPosition);
        }

        /// <summary>
        /// Converts offset values into position value
        /// </summary>
        /// <param name="hudElement">HUD element view model</param>
        /// <param name="window">Overlay window</param>
        /// <returns>Item1 - X, Item2 - Y</returns>
        public virtual Tuple<double, double> GetOffsetPosition(HudBaseToolViewModel toolViewModel, HudWindowViewModel viewModel)
        {
            Check.ArgumentNotNull(() => toolViewModel);
            Check.ArgumentNotNull(() => viewModel);

            var panelOffset = viewModel.GetPanelOffset(toolViewModel);

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
                case HudDesignerToolType.HeatMap:
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
                case HudDesignerToolType.HeatMap:
                    factory = new FrameworkElementFactory(typeof(HudHeatMap));
                    break;
                default:
                    break;
            }

            return factory;
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
                var playerNoteViewModel = new PlayerNoteViewModel(hudToolViewModel.Parent.PlayerId);

                var playerNoteView = new PlayerNoteView(playerNoteViewModel);
                playerNoteView.ShowDialog();
           
                hudToolViewModel.Parent.NoteToolTip = playerNoteViewModel.HasNotes ? playerNoteViewModel.Note : string.Empty;
            };

            var contextMenu = new RadContextMenu();
            contextMenu.Items.Add(menuItem);
            RadContextMenu.SetContextMenu(hudTool, contextMenu);
        }

        /// <summary>
        /// Gets the shift for the specified table type and seat
        /// </summary>
        /// <param name="tableType"></param>
        /// <param name="seat"></param>
        /// <returns></returns>
        public virtual Point GetPositionShift(EnumTableType tableType, int seat)
        {
            return new Point(0, 0);
        }

        /// <summary>
        /// Calculates x-scale for the specified window
        /// </summary>
        /// <param name="window">Window to calculate x-scale</param>
        /// <returns>X-scale</returns>
        public virtual double GetScaleX(HudWindow window)
        {
            return window.Width / InitialTableSize.X;
        }

        /// <summary>
        /// Calculates y-scale for the specified window
        /// </summary>
        /// <param name="window">Window to calculate y-scale</param>
        /// <returns>Y-scale</returns>
        public virtual double GetScaleY(HudWindow window)
        {
            return window.Height / InitialTableSize.Y;
        }
    }
}