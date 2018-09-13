//-----------------------------------------------------------------------
// <copyright file="HudWindow.cs" company="Ace Poker Solutions">
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
using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Log;
using DriveHUD.HUD.Service;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using System.ComponentModel;
using Telerik.Windows.Controls;
using DriveHUD.Common.Linq;

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for HudWindow.xaml
    /// </summary>
    public partial class HudWindow : Window
    {
        private IHudPanelService hudPanelService;

        private Point? trackerConditionsMeterPosition;
        private Point trackerConditionsMeterPositionOffset;

        public HudWindowViewModel ViewModel
        {
            get
            {
                return DataContext as HudWindowViewModel;
            }
        }

        public HudWindow()
        {
            DataContext = new HudWindowViewModel(Dispatcher);

            InitializeComponent();

            dgCanvas.DragEnded += DgCanvas_DragEnded;
        }

        public HudLayout Layout { get; set; }

        public double ScaleX
        {
            get { return hudPanelService?.GetScaleX(this) ?? 0; }
        }

        public double ScaleY
        {
            get { return hudPanelService?.GetScaleX(this) ?? 0; }
        }

        public void Initialize(HudLayout layout, IntPtr hWnd)
        {
            if (layout == null)
            {
                return;
            }

            // clean old data
            Layout?.Cleanup();

            Layout = layout;

            if (ViewModel != null)
            {
                ViewModel.SetLayout(layout);
                ViewModel.WindowHandle = hWnd;

                if (ViewModel.RefreshHud == null)
                {
                    ViewModel.RefreshHud = Refresh;
                }
            }

            // set parents for tools
            Layout.ListHUDPlayer.Select(x => x.HudElement).ForEach(h => h.Tools.ForEach(t => t.Parent = h));

            if (hudPanelService == null)
            {
                hudPanelService = layout != null ?
                    ServiceLocator.Current.GetInstance<IHudPanelService>(layout.PokerSite.ToString()) :
                    ServiceLocator.Current.GetInstance<IHudPanelService>();
            }

            // remove old elements, but remember positions
            foreach (var panel in dgCanvas.Children.OfType<FrameworkElement>().ToList())
            {
                var toolViewModel = panel.DataContext as HudBaseToolViewModel;

                ViewModel.UpdatePanelOffset(toolViewModel);

                if (panel.DataContext is HudTrackConditionsViewModel hudTrackConditionsViewModel)
                {
                    trackerConditionsMeterPositionOffset = new Point(hudTrackConditionsViewModel.OffsetX, hudTrackConditionsViewModel.OffsetY);
                }

                dgCanvas.Children.Remove(panel);
            }

            dgCanvas.UpdateLayout();

            // add new elements
            foreach (var playerHudContent in Layout.ListHUDPlayer)
            {
                if (playerHudContent.HudElement == null || string.IsNullOrEmpty(playerHudContent.Name) ||
                    playerHudContent.HudElement.Tools == null || playerHudContent.HudElement.Tools.Count < 1)
                {
                    continue;
                }

                var isNoteIconSet = false;

                foreach (var toolViewModel in playerHudContent.HudElement.Tools.Where(x => x is IHudNonPopupToolViewModel).ToArray())
                {
                    if (!isNoteIconSet && toolViewModel is HudPlainStatBoxViewModel && !(toolViewModel is HudFourStatsBoxViewModel))
                    {
                        (toolViewModel as HudPlainStatBoxViewModel).IsNoteIconEnabled = true;
                        isNoteIconSet = true;
                    }

                    var toolKey = HudWindowViewModel.HudToolKey.BuildKey(toolViewModel);

                    if (!ViewModel.PanelOffsets.ContainsKey(toolKey))
                    {
                        ViewModel.PanelOffsets.Add(toolKey, new Point(0, 0));
                    }
                    else
                    {
                        toolViewModel.OffsetX = ViewModel.PanelOffsets[toolKey].X;
                        toolViewModel.OffsetY = ViewModel.PanelOffsets[toolKey].Y;
                    }

                    var panel = hudPanelService.Create(toolViewModel);

                    if (panel != null)
                    {
                        dgCanvas.Children.Add(panel);
                    }
                }
            }

            BuildTrackConditionsMeter(layout.HudTrackConditionsMeter);
        }

        public void Refresh()
        {
            dgCanvas.XFraction = ScaleX;
            dgCanvas.YFraction = ScaleY;

            foreach (var hudPanel in dgCanvas.Children.OfType<FrameworkElement>())
            {
                if (hudPanel is TrackConditionsMeterView && trackerConditionsMeterPosition != null)
                {
                    var hudElementViewModel = hudPanel.DataContext as IHudWindowElement;

                    trackerConditionsMeterPositionOffset = new Point(hudElementViewModel.OffsetX, hudElementViewModel.OffsetY);

                    var trackerXPosition = trackerConditionsMeterPositionOffset.X != 0 ? trackerConditionsMeterPositionOffset.X : trackerConditionsMeterPosition.Value.X;
                    var trackerYPosition = trackerConditionsMeterPositionOffset.Y != 0 ? trackerConditionsMeterPositionOffset.Y : trackerConditionsMeterPosition.Value.Y;

                    Canvas.SetLeft(hudPanel, trackerXPosition * ScaleX);
                    Canvas.SetTop(hudPanel, trackerYPosition * ScaleY);
                    continue;
                }

                var toolViewModel = hudPanel.DataContext as HudBaseToolViewModel;

                var toolKey = HudWindowViewModel.HudToolKey.BuildKey(toolViewModel);

                if (toolViewModel != null)
                {
                    ViewModel.PanelOffsets[toolKey] = new Point(toolViewModel.OffsetX, toolViewModel.OffsetY);
                }

                hudPanel.Width = !double.IsNaN(toolViewModel.Width) ? toolViewModel.Width * ScaleX : double.NaN;
                hudPanel.Height = !double.IsNaN(toolViewModel.Height) ? toolViewModel.Height * ScaleY : double.NaN;

                var positions = hudPanelService.CalculatePositions(toolViewModel, hudPanel, this);

                Canvas.SetLeft(hudPanel, positions.Item1);
                Canvas.SetTop(hudPanel, positions.Item2);
            }
        }

        private void BuildTrackConditionsMeter(HudTrackConditionsViewModelInfo trackConditionViewModelInfo)
        {
            if (trackConditionViewModelInfo == null)
            {
                return;
            }

            try
            {
                var trackConditionViewModel = new HudTrackConditionsViewModel(trackConditionViewModelInfo)
                {
                    OffsetX = trackerConditionsMeterPositionOffset.X,
                    OffsetY = trackerConditionsMeterPositionOffset.Y
                };

                var trackConditionView = new TrackConditionsMeterView
                {
                    DataContext = trackConditionViewModel
                };

                dgCanvas.Children.Add(trackConditionView);

                UpdateTrackConditionMeterPosition(trackConditionViewModelInfo, trackConditionView);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to initialize track condition meter.", e);
            }
        }

        private void UpdateTrackConditionMeterPosition(HudTrackConditionsViewModelInfo trackConditionViewModelInfo, FrameworkElement trackConditionView)
        {
            if (trackerConditionsMeterPosition != null)
            {
                Canvas.SetLeft(trackConditionView, trackerConditionsMeterPosition.Value.X);
                Canvas.SetTop(trackConditionView, trackerConditionsMeterPosition.Value.Y);
            }
            else
            {
                Tuple<double, double> positions;

                if (trackConditionViewModelInfo.Position.HasValue)
                {
                    positions = new Tuple<double, double>(trackConditionViewModelInfo.Position.Value.X, trackConditionViewModelInfo.Position.Value.Y);
                }
                else
                {
                    positions = hudPanelService.GetInitialTrackConditionMeterPosition();
                }

                Canvas.SetLeft(trackConditionView, positions.Item1);
                Canvas.SetTop(trackConditionView, positions.Item2);

                trackerConditionsMeterPosition = new Point(positions.Item1, positions.Item2);
                trackerConditionsMeterPositionOffset = new Point(0, 0);
            }
        }

        private int GetPanelOffsetsKey(HudElementViewModel viewModel)
        {
            if (viewModel == null)
            {
                return 0;
            }

            return viewModel.Seat;
        }

        private void SaveHudPositions_Click(object sender, RadRoutedEventArgs e)
        {
            try
            {
                if (Layout == null)
                {
                    return;
                }

                var trackMeterPositionX = trackerConditionsMeterPositionOffset.X != 0 ? trackerConditionsMeterPositionOffset.X :
                    (trackerConditionsMeterPosition.HasValue ? trackerConditionsMeterPosition.Value.X : 0);

                var trackMeterPositionY = trackerConditionsMeterPositionOffset.Y != 0 ? trackerConditionsMeterPositionOffset.Y :
                    (trackerConditionsMeterPosition.HasValue ? trackerConditionsMeterPosition.Value.Y : 0);

                var hudLayoutContract = new HudLayoutContract
                {
                    LayoutName = Layout.LayoutName,
                    GameType = Layout.GameType,
                    PokerSite = Layout.PokerSite,
                    TableType = Layout.TableType,
                    HudPositions = new List<HudPositionContract>(),
                    TrackMeterPosition = new Point(trackMeterPositionX, trackMeterPositionY)
                };

                // clone is needed
                var toolViewModels = dgCanvas.Children.OfType<FrameworkElement>()
                    .Where(x => x != null && (x.DataContext is IHudNonPopupToolViewModel))
                    .Select(x => (x.DataContext as HudBaseToolViewModel))
                    .ToList();

                var toolsIds = new HashSet<Guid>();
                var seats = new HashSet<int>();

                foreach (var toolViewModel in toolViewModels)
                {
                    if (!toolsIds.Contains(toolViewModel.Id))
                    {
                        toolsIds.Add(toolViewModel.Id);
                    }

                    var seatNumber = toolViewModel.Parent != null ? toolViewModel.Parent.Seat : 1;

                    if (!seats.Contains(seatNumber))
                    {
                        seats.Add(seatNumber);
                    }

                    var position = hudPanelService.GetOffsetPosition(toolViewModel, ViewModel);

                    hudLayoutContract.HudPositions.Add(new HudPositionContract
                    {
                        Id = toolViewModel.Id,
                        Position = new Point(position.Item1, position.Item2),
                        SeatNumber = seatNumber
                    });
                }

                var emptySeats = Enumerable.Range(1, (int)Layout.TableType).Except(seats);

                foreach (var seat in emptySeats)
                {
                    foreach (var toolId in toolsIds)
                    {
                        var toolkey = new HudWindowViewModel.HudToolKey
                        {
                            Id = toolId,
                            Seat = seat
                        };

                        if (!ViewModel.PanelOffsets.ContainsKey(toolkey))
                        {
                            continue;
                        }

                        var panelOffset = ViewModel.PanelOffsets[toolkey];

                        if (panelOffset.X != 0 && panelOffset.Y != 0)
                        {
                            hudLayoutContract.HudPositions.Add(new HudPositionContract
                            {
                                Id = toolId,
                                Position = new Point(panelOffset.X, panelOffset.Y),
                                SeatNumber = seat
                            });
                        }
                    }
                }

                ViewModel?.SaveHudPositions(hudLayoutContract);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
            }
        }

        private void DgCanvas_DragEnded(object sender, EventArgs e)
        {
            Refresh();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            dgCanvas.DragEnded -= DgCanvas_DragEnded;
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            try
            {
                if (dgCanvas != null && dgCanvas.Children != null)
                {
                    foreach (var panel in dgCanvas.Children.OfType<FrameworkElement>().ToList())
                    {
                        dgCanvas.Children.Remove(panel);
                    }

                    dgCanvas.UpdateLayout();
                }

                NameScope.GetNameScope(this).UnregisterName("dgCanvas");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could freed resources.", ex);
            }

            GC.Collect();

            base.OnClosed(e);
        }

        private List<RadMenuItem> GetSiblingGroupItems(RadMenuItem currentItem)
        {
            var parentItem = currentItem.ParentOfType<RadMenuItem>();

            if (parentItem == null)
            {
                return null;
            }

            var items = new List<RadMenuItem>();

            foreach (var item in parentItem.Items)
            {
                var container = parentItem.ItemContainerGenerator.ContainerFromItem(item) as RadMenuItem;

                if (container == null || container.Tag == null)
                {
                    continue;
                }

                if (container.Tag.Equals(currentItem.Tag))
                {
                    items.Add(container);
                }
            }

            return items;
        }

        private void OnRadMenuItemClick(object sender, RadRoutedEventArgs e)
        {
            var currentItem = e.OriginalSource as RadMenuItem;

            if (currentItem.IsCheckable && currentItem.Tag != null)
            {
                var siblingItems = this.GetSiblingGroupItems(currentItem);

                if (siblingItems != null)
                {
                    foreach (var item in siblingItems)
                    {
                        if (item != currentItem)
                        {
                            item.IsChecked = false;
                        }
                    }
                }
            }
        }
    }
}