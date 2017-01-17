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

namespace DriveHUD.Application.Views
{
    /// <summary>
    /// Interaction logic for HudWindow.xaml
    /// </summary>
    public partial class HudWindow : Window, IDisposable
    {
        private IHudPanelService hudPanelCreator;

        private Dictionary<int, Point> panelOffsets;

        private Point? trackerConditionsMeterPosition;
        private Point trackerConditionsMeterPositionOffset;

        private HudWindowViewModel ViewModel
        {
            get { return DataContext as HudWindowViewModel; }
        }

        public HudWindow()
        {
            InitializeComponent();

            panelOffsets = new Dictionary<int, Point>();
            dgCanvas.DragEnded += DgCanvas_DragEnded;
        }

        public HudLayout Layout { get; set; }

        public double InitWidth { get; set; }

        public double InitHeight { get; set; }

        public double XFraction
        {
            get { return Width / InitWidth; }
        }

        public double YFraction
        {
            get { return Height / InitHeight; }
        }

        public void Init(HudLayout layout)
        {
            Layout?.Cleanup();
            Layout = layout;
            ViewModel?.SetLayout(layout);
            if (ViewModel != null)
            {
                ViewModel.GameType = layout.TableHud.GameType;
                ViewModel.TableType = layout.TableType;
            }
            if (layout?.TableHud != null)
            {
                hudPanelCreator = ServiceLocator.Current.GetInstance<IHudPanelService>(layout.TableHud.PokerSite.ToString());
            }
            else
            {
                hudPanelCreator = ServiceLocator.Current.GetInstance<IHudPanelService>();
            }

            foreach (var panel in dgCanvas.Children.OfType<FrameworkElement>().ToList())
            {
                var hudElementViewModel = panel.DataContext as HudElementViewModel;

                if (hudElementViewModel != null)
                {
                    panelOffsets[GetPanelOffsetsKey(hudElementViewModel)] = new Point(hudElementViewModel.OffsetX, hudElementViewModel.OffsetY);
                }

                var hudTrackConditionsViewModel = panel.DataContext as HudTrackConditionsViewModel;

                if (hudTrackConditionsViewModel != null)
                {
                    trackerConditionsMeterPositionOffset = new Point(hudTrackConditionsViewModel.OffsetX, hudTrackConditionsViewModel.OffsetY);
                }

                dgCanvas.Children.Remove(panel);
            }

            dgCanvas.UpdateLayout();

            foreach (var playerHudContent in Layout.ListHUDPlayer)
            {
                if (playerHudContent.HudElement == null || string.IsNullOrEmpty(playerHudContent.Name))
                {
                    continue;
                }

                if (!panelOffsets.ContainsKey(GetPanelOffsetsKey(playerHudContent.HudElement)))
                {
                    panelOffsets.Add(GetPanelOffsetsKey(playerHudContent.HudElement), new Point(0, 0));
                }
                else
                {
                    playerHudContent.HudElement.OffsetX = panelOffsets[GetPanelOffsetsKey(playerHudContent.HudElement)].X;
                    playerHudContent.HudElement.OffsetY = panelOffsets[GetPanelOffsetsKey(playerHudContent.HudElement)].Y;
                }

                var panel = hudPanelCreator.Create(playerHudContent.HudElement, layout.HudType);
                panel.Opacity = playerHudContent.HudElement.Opacity;

                dgCanvas.Children.Add(panel);
            }

            BuildTrackConditionsMeter(layout.HudTrackConditionsMeter);
        }

        public void Dispose()
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

            GC.Collect();
        }

        public void Update()
        {
            if (InitHeight == 0 || InitWidth == 0)
            {
                var initialSizes = hudPanelCreator.GetInitialTableSize();

                InitWidth = initialSizes.Item1;
                InitHeight = initialSizes.Item2;
            }

            dgCanvas.XFraction = XFraction;
            dgCanvas.YFraction = YFraction;

            foreach (var hudPanel in dgCanvas.Children.OfType<FrameworkElement>())
            {
                if (hudPanel is TrackConditionsMeterView && trackerConditionsMeterPosition != null)
                {
                    var hudElementViewModel = hudPanel.DataContext as IHudWindowElement;

                    trackerConditionsMeterPositionOffset = new Point(hudElementViewModel.OffsetX, hudElementViewModel.OffsetY);

                    var trackerXPosition = trackerConditionsMeterPositionOffset.X != 0 ? trackerConditionsMeterPositionOffset.X : trackerConditionsMeterPosition.Value.X;
                    var trackerYPosition = trackerConditionsMeterPositionOffset.Y != 0 ? trackerConditionsMeterPositionOffset.Y : trackerConditionsMeterPosition.Value.Y;

                    Canvas.SetLeft(hudPanel, trackerXPosition * XFraction);
                    Canvas.SetTop(hudPanel, trackerYPosition * YFraction);
                    continue;
                }

                var viewModel = hudPanel.DataContext as HudElementViewModel;

                if (viewModel != null)
                {
                    panelOffsets[GetPanelOffsetsKey(viewModel)] = new Point(viewModel.OffsetX, viewModel.OffsetY);
                }

                if (hudPanel is HudRichPanel)
                {
                    var hudRichPanel = hudPanel as HudRichPanel;

                    hudRichPanel.Height = double.NaN;
                    hudRichPanel.Width = viewModel.Width * XFraction;

                    hudRichPanel.vbMain.Height = double.NaN;
                    hudRichPanel.vbMain.Width = viewModel.Width * XFraction;
                    hudRichPanel.Opacity = viewModel.Opacity;
                }

                if (hudPanel is HudPanel)
                {
                    hudPanel.Height = double.NaN;
                    hudPanel.Width = viewModel.Width * XFraction;
                    hudPanel.Opacity = viewModel.Opacity;
                }

                var positions = hudPanelCreator.CalculatePositions(viewModel, this);


                Canvas.SetLeft(hudPanel, positions.Item1);
                Canvas.SetTop(hudPanel, positions.Item2);
            }
        }

        public Point GetPanelOffset(HudElementViewModel viewModel)
        {
            if (viewModel != null && panelOffsets.ContainsKey(GetPanelOffsetsKey(viewModel)))
            {
                return panelOffsets[GetPanelOffsetsKey(viewModel)];
            }

            return new Point(0, 0);
        }

        private void BuildTrackConditionsMeter(HudTrackConditionsViewModelInfo trackConditionViewModelInfo)
        {
            if (trackConditionViewModelInfo == null)
            {
                return;
            }

            var trackConditionViewModel = new HudTrackConditionsViewModel(trackConditionViewModelInfo);

            trackConditionViewModel.OffsetX = trackerConditionsMeterPositionOffset.X;
            trackConditionViewModel.OffsetY = trackerConditionsMeterPositionOffset.Y;

            var trackConditionView = new TrackConditionsMeterView
            {
                DataContext = trackConditionViewModel
            };

            dgCanvas.Children.Add(trackConditionView);

            UpdateTrackConditionMeterPosition(trackConditionView);
        }

        private void UpdateTrackConditionMeterPosition(FrameworkElement trackConditionView)
        {
            if (trackerConditionsMeterPosition != null)
            {
                Canvas.SetLeft(trackConditionView, trackerConditionsMeterPosition.Value.X);
                Canvas.SetTop(trackConditionView, trackerConditionsMeterPosition.Value.Y);
            }
            else
            {
                var positions = hudPanelCreator.GetInitialTrackConditionMeterPosition();

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

            return viewModel.Seat + (viewModel.HudType == HudType.Default ? 0 : 100);
        }

        //private void SaveHudPositions_Click(object sender, RadRoutedEventArgs e)
        //{
        //    try
        //    {
        //        var layoutId = Layout?.LayoutId;

        //        if (!layoutId.HasValue)
        //        {
        //            return;
        //        }

        //        HudLayoutContract hudLayout = new HudLayoutContract();
        //        hudLayout.LayoutId = layoutId.Value;
        //        hudLayout.HudPositions = new List<HudPositionContract>();

        //        var hudPanels = dgCanvas.Children.OfType<FrameworkElement>()
        //            .Where(x => x != null && !(x is TrackConditionsMeterView) && (x.DataContext is HudElementViewModel))
        //            .Select(x => (x.DataContext as HudElementViewModel).Clone())
        //            .ToList();

        //        foreach (var hudPanel in hudPanels)
        //        {
        //            var position = hudPanelCreator.GetOffsetPosition(hudPanel, this);

        //            hudLayout.HudPositions.Add(new HudPositionContract
        //            {
        //                Position = new Point(position.Item1, position.Item2),
        //                SeatNumber = hudPanel.Seat,
        //                HudType = (int)hudPanel.HudType
        //            });
        //        }

        //        ViewModel?.SaveHudPositions(hudLayout);
        //    }
        //    catch (Exception ex)
        //    {
        //        LogProvider.Log.Error(this, ex);
        //    }
        //}

        private void DgCanvas_DragEnded(object sender, EventArgs e)
        {
            this.Update();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            dgCanvas.DragEnded -= DgCanvas_DragEnded;
            base.OnClosing(e);
        }

    }
}