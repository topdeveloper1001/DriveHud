//-----------------------------------------------------------------------
// <copyright file="BaseTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Common;
using DriveHUD.Entities;
using Model.Enums;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

namespace DriveHUD.Application.TableConfigurators
{
    internal abstract class BaseTableConfigurator : ITableConfigurator
    {
        public abstract EnumPokerSites Type { get; }

        public abstract HudType HudType { get; }

        protected abstract ITableSeatAreaConfigurator TableSeatAreaConfigurator { get; }

        protected abstract string BackgroundImage { get; }

        public virtual RadDiagramShape InitializeTable(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            Check.ArgumentNotNull(() => diagram);
            Check.ArgumentNotNull(() => hudTable);

            diagram.Clear();

            var table = CreateTableRadDiagramShape();

            var tableImage = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), string.Format(BackgroundImage, seats)));

            RenderOptions.SetBitmapScalingMode(tableImage, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(tableImage, EdgeMode.Aliased);

            table.Background = new ImageBrush(tableImage);

            diagram.AddShape(table);

            table.X = DiagramActualWidth(diagram) / 2 - table.Width / 2;
            table.Y = diagram.Height / 2 - table.Height / 2;

            hudTable.IsRelativePosition = true;
            hudTable.RelativePosition = new Point(table.X, table.Y);
            hudTable.StartPosition = new Point(11, 64);

            diagram.Group("tableGroup", diagram.Shapes.OfType<IGroupable>().ToArray());

            return table;
        }

        public virtual void Update(RadDiagram diagram, HudTableViewModel hudTable)
        {
            var hudElements = diagram.Shapes.OfType<RadDiagramItem>().Where(x => x.Tag.ToString() == "hud").ToList();

            for (int i = 0; i < hudElements.Count; i++)
            {
                Binding myBinding = new Binding(nameof(HudElementViewModel.Position))
                {
                    Source = hudTable.HudElements[i],
                    Mode = BindingMode.TwoWay
                };

                hudElements[i].SetBinding(RadDiagramItem.PositionProperty, myBinding);
            }
        }

        protected RadDiagramShape CreateHudLabel(HudElementViewModel viewModel)
        {
            var label = CreateRadDiagramShape(viewModel);
            
            BindingOperations.ClearBinding(label, RadDiagramItem.PositionProperty);
            BindingOperations.ClearBinding(label, FrameworkElement.WidthProperty);
            BindingOperations.ClearBinding(label, FrameworkElement.HeightProperty);
            BindingOperations.ClearBinding(label, UIElement.OpacityProperty);

            var opacityBinding = new Binding(nameof(HudElementViewModel.Opacity))
            {
                Source = viewModel,
                Mode = BindingMode.TwoWay
            };
            label.SetBinding(UIElement.OpacityProperty, opacityBinding);

            var positionBinding = new Binding(nameof(HudElementViewModel.Position)) { Source = viewModel, Mode = BindingMode.TwoWay };
            label.SetBinding(RadDiagramItem.PositionProperty, positionBinding);

            if (viewModel.HudType == HudType.Plain)
            {
                var widthBinding = new Binding(nameof(HudElementViewModel.Width)) { Source = viewModel, Mode = BindingMode.TwoWay };
                label.SetBinding(FrameworkElement.WidthProperty, widthBinding);
            }
            else
            {
                label.Width = double.NaN;
            }

            label.Height = double.NaN;

            RadContextMenu contextMenu = new RadContextMenu();
            RadMenuItem item = new RadMenuItem();

            item.Header = "Apply Size to All";
            item.Click += (s, e) =>
            {
                var clickedItem = s as FrameworkElement;
                if (clickedItem == null || !(clickedItem.DataContext is HudElementViewModel))
                {
                    return;
                }

                var datacontext = clickedItem.DataContext as HudElementViewModel;

                Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<UpdateHudEvent>().Publish(new UpdateHudEventArgs(datacontext.Height, datacontext.Width));
            };

            contextMenu.Items.Add(item);
            RadContextMenu.SetContextMenu(label, contextMenu);

            return label;
        }

        public virtual double DiagramActualWidth(RadDiagram diagram)
        {
            return diagram.ActualWidth == 0 ? HudDefaultSettings.HudTableWidth : diagram.ActualWidth;
        }

        public virtual double DiagramActualHeigth(RadDiagram diagram)
        {
            return diagram.ActualHeight == 0 ? HudDefaultSettings.HudTableHeight : diagram.ActualHeight;
        }

        public abstract void ConfigureTable(RadDiagram diagram, HudTableViewModel hudTable, int seats);

        public abstract IEnumerable<HudElementViewModel> GenerateElements(int seats);

        protected abstract RadDiagramShape CreateTableRadDiagramShape();

        protected virtual RadDiagramShape CreateRadDiagramShape(HudElementViewModel viewModel)
        {
            var label = new RadDiagramShape()
            {
                DataContext = viewModel,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = true,
                Background = null,
                IsRotationEnabled = false,
                Tag = HudType,
                Padding = new Thickness(0),
                IsDraggingEnabled = false,
                Opacity = viewModel.Opacity
            };

            return label;
        }

        protected abstract Dictionary<int, int[,]> GetPredefinedMarkersPositions();

        protected virtual void CreatePreferredSeatMarkers(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            var predefinedPositions = GetPredefinedMarkersPositions();

            if (!predefinedPositions.ContainsKey(seats))
            {
                return;
            }

            for (int i = 0; i < seats; i++)
            {
                var hudElementPositionX = predefinedPositions[seats][i, 0];
                var hudElementPositionY = predefinedPositions[seats][i, 1];
                var datacontext = hudTable.TableSeatAreaCollection?.ElementAt(i);

                var shape = new RadDiagramShape
                {
                    Height = 30,
                    Width = 30,
                    IsEnabled = true,
                    SnapsToDevicePixels = true,
                    X = hudElementPositionX,
                    Y = hudElementPositionY,
                    DataContext = datacontext,
                    Template = App.Current.Resources["PreferredSeatControlTemplate"] as ControlTemplate,
                    IsDraggingEnabled = false,
                };

                diagram.Items.Add(shape);
            }
        }

        protected virtual IEnumerable<ITableSeatArea> CreateSeatAreas(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            var seatAreas = TableSeatAreaConfigurator.GetTableSeatAreas((EnumTableType)seats);
            var tableSeatSetting = TableSeatAreaHelpers.GetSeatSetting((EnumTableType)seats, Type);

            foreach (var v in seatAreas)
            {
                v.StartPoint = new Point(0, 0);
                v.PokerSite = Type;
                v.TableType = (EnumTableType)seats;
                v.SetContextMenuEnabled(tableSeatSetting.IsPreferredSeatEnabled);

                if (v.SeatNumber == tableSeatSetting.PreferredSeat)
                {
                    v.IsPreferredSeat = true;
                }
                else
                {
                    v.IsPreferredSeat = false;
                }

                diagram.AddShape(v.SeatShape);

            }
            hudTable.TableSeatAreaCollection = new System.Collections.ObjectModel.ObservableCollection<ITableSeatArea>(seatAreas);
            return seatAreas;
        }

    }
}