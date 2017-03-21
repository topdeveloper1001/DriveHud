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
using System.Diagnostics;
using ReactiveUI;
using Telerik.Windows.Controls.Diagrams;
using System.Windows.Data;
using System.Windows.Controls;

namespace DriveHUD.Application.ViewModels.Hud.Designer.Behaviors
{
    public class HudDesignerBehavior : Behavior<RadDiagram>
    {
        #region Dependency Properties

        private IDisposable ActOnEveryObjectBehavior
        {
            get;
            set;
        }

        public static DependencyProperty RecommendedAreaTemplateProperty = DependencyProperty.Register("RecommendedAreaTemplate", typeof(DataTemplate), typeof(HudDesignerBehavior));

        public DataTemplate RecommendedAreaTemplate
        {
            get
            {
                return (DataTemplate)GetValue(RecommendedAreaTemplateProperty);
            }
            set
            {
                SetValue(RecommendedAreaTemplateProperty, value);
            }
        }

        public static DependencyProperty DesignerToolsProperty = DependencyProperty.Register("DesignerTools", typeof(ReactiveList<HudBaseToolViewModel>),
            typeof(HudDesignerBehavior), new PropertyMetadata(new ReactiveList<HudBaseToolViewModel>(), OnDesignerToolsChanged));

        public ReactiveList<HudBaseToolViewModel> DesignerTools
        {
            get
            {
                return (ReactiveList<HudBaseToolViewModel>)GetValue(DesignerToolsProperty);
            }
            set
            {
                SetValue(DesignerToolsProperty, value);
            }
        }

        private static void OnDesignerToolsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudDesignerBehavior;
            var toolsCollection = e.NewValue as ReactiveList<HudBaseToolViewModel>;

            if (behavior == null || toolsCollection == null)
            {
                return;
            }

            if (behavior.ActOnEveryObjectBehavior != null)
            {
                behavior.ActOnEveryObjectBehavior.Dispose();
            }

            behavior.ActOnEveryObjectBehavior = toolsCollection.ActOnEveryObject(behavior.OnAddTool, behavior.RemoveAddTool);
        }

        #endregion

        #region Initializing

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
            tableConfigurator.ConfigureTable(diagram, hudTableViewModel, (int)EnumTableType.HU);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnDetaching()
        {
            if (ActOnEveryObjectBehavior != null)
            {
                ActOnEveryObjectBehavior.Dispose();
            }

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

        #endregion

        #region Add/Remove tools 

        private void OnAddTool(HudBaseToolViewModel toolViewModel)
        {
            var shape = new RadDiagramShape()
            {
                DataContext = toolViewModel,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = true,
                Background = null,
                IsRotationEnabled = false,
                Padding = new Thickness(0),
                IsDraggingEnabled = true,
                ZIndex = 100
            };

            SetWidthBinding(shape, toolViewModel);
            SetHeightBinding(shape, toolViewModel);
            SetPositionBinding(shape, toolViewModel);
            SetOpacityBinding(shape, toolViewModel);

            AssociatedObject.AddShape(shape);
        }

        private void RemoveAddTool(HudBaseToolViewModel toolViewModel)
        {
            Debug.WriteLine("Tool removed");
        }

        protected static void SetPositionBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            SetBinding(shape, RadDiagramItem.PositionProperty, viewModel, nameof(HudBaseToolViewModel.Position));
        }

        protected static void SetOpacityBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            SetBinding(shape, UIElement.OpacityProperty, viewModel, nameof(HudBaseToolViewModel.Opacity));
        }

        protected static void SetWidthBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            SetBinding(shape, FrameworkElement.WidthProperty, viewModel, nameof(HudBaseToolViewModel.Width));
        }

        protected static void SetHeightBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            SetBinding(shape, FrameworkElement.HeightProperty, viewModel, nameof(HudBaseToolViewModel.Height));
        }

        protected static void SetBinding(FrameworkElement element, DependencyProperty dp, object source, string property)
        {
            BindingOperations.ClearBinding(element, dp);

            var binding = new Binding(property)
            {
                Source = source,
                Mode = BindingMode.TwoWay
            };

            element.SetBinding(dp, binding);
        }

        protected static void SetShapeSize(RadDiagramShape shape)
        {
            shape.Height = double.NaN;
        }

        #endregion
    }
}