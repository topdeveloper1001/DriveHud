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
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Model.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
        public abstract HudViewType HudViewType { get; }

        protected abstract string BackgroundImage { get; }

        public virtual RadDiagramShape InitializeTable(RadDiagram diagram, HudTableViewModel hudTable, int seats)
        {
            Check.ArgumentNotNull(() => diagram);
            Check.ArgumentNotNull(() => hudTable);

            ClearDiagram(diagram);

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

        protected RadDiagramShape CreateHudLabel(HudElementViewModel viewModel)
        {
            var label = CreateRadDiagramShape(viewModel);
        
            SetPositionBinding(label, viewModel);
            SetOpacityBinding(label, viewModel);
            SetWidthBinding(label, viewModel);

            label.Height = double.NaN;

            AddContextMenu(label);

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
                Tag = HudViewType,
                Padding = new Thickness(0),
                IsDraggingEnabled = true,
                Opacity = viewModel.Opacity,
                ZIndex = 100
            };

            return label;
        }

        protected virtual void ClearDiagram(RadDiagram diagram)
        {
            if (diagram == null)
            {
                return;
            }

            diagram.Clear();
        }

        protected virtual void AddContextMenu(RadDiagramShape label)
        {
            if (label == null)
            {
                return;
            }

            var item = new RadMenuItem();
            item.Header = CommonResourceManager.Instance.GetResourceString("Common_Hud_ApplySizeToAll");

            item.Click += OnHudMenuItemClick;

            var contextMenu = new RadContextMenu();
            contextMenu.Items.Add(item);

            RadContextMenu.SetContextMenu(label, contextMenu);
        }

        protected virtual void OnHudMenuItemClick(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            var clickedItem = sender as FrameworkElement;

            if (clickedItem == null || !(clickedItem.DataContext is HudElementViewModel))
            {
                return;
            }

            var datacontext = clickedItem.DataContext as HudElementViewModel;

            Microsoft.Practices.ServiceLocation.ServiceLocator.Current.
                GetInstance<IEventAggregator>().
                GetEvent<UpdateHudEvent>().
                Publish(new UpdateHudEventArgs(datacontext.Height, datacontext.Width));
        }

        protected virtual void SetPositionBinding(RadDiagramShape shape, HudElementViewModel viewModel)
        {
            SetBinding(shape, RadDiagramItem.PositionProperty, viewModel, nameof(HudElementViewModel.Position));
        }

        protected virtual void SetOpacityBinding(RadDiagramShape shape, HudElementViewModel viewModel)
        {
            SetBinding(shape, UIElement.OpacityProperty, viewModel, nameof(HudElementViewModel.Opacity));
        }

        protected virtual void SetWidthBinding(RadDiagramShape shape, HudElementViewModel viewModel)
        {
            SetBinding(shape, FrameworkElement.WidthProperty, viewModel, nameof(HudElementViewModel.Width));
        }

        protected virtual void SetBinding(FrameworkElement element, DependencyProperty dp, object source, string property)
        {
            BindingOperations.ClearBinding(element, dp);

            var binding = new Binding(property)
            {
                Source = source,
                Mode = BindingMode.TwoWay
            };

            element.SetBinding(dp, binding);
        }
    }
}