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
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Reactive.Disposables;

namespace DriveHUD.Application.ViewModels.Hud.Designer.Behaviors
{
    /// <summary>
    /// Behavior to display HUD tools
    /// </summary>
    public class HudDesignerBehavior : Behavior<RadDiagram>
    {
        #region Dependency Properties

        private CompositeDisposable Disposables = new CompositeDisposable();

        private readonly List<IShape> RemovableShapes = new List<IShape>();

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

        public static DependencyProperty HudElementsProperty = DependencyProperty.Register("HudElements", typeof(ReactiveList<HudElementViewModel>),
            typeof(HudDesignerBehavior), new PropertyMetadata(new ReactiveList<HudElementViewModel>(), OnHudElementsChanged));

        public ReactiveList<HudElementViewModel> HudElements
        {
            get
            {
                return (ReactiveList<HudElementViewModel>)GetValue(HudElementsProperty);
            }
            set
            {
                SetValue(HudElementsProperty, value);
            }
        }

        private static void OnHudElementsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudDesignerBehavior;
            var hudElements = e.NewValue as ReactiveList<HudElementViewModel>;

            if (behavior == null || hudElements == null || behavior.AssociatedObject == null)
            {
                return;
            }

            if (behavior.Disposables != null)
            {
                behavior.Disposables.Dispose();
            }

            behavior.Disposables = new CompositeDisposable();

            behavior.ClearDiagram();

            for (var seat = 0; seat < hudElements.Count; seat++)
            {
                // add player label
                var playerLabel = CreatePlayerLabel(hudElements.Count, seat + 1);
                behavior.AssociatedObject.AddShape(playerLabel);
                behavior.RemovableShapes.Add(playerLabel);

                foreach (var tool in hudElements[seat].Tools)
                {
                    behavior.AddTool(tool);
                }

                var disposable = hudElements[seat].Tools.ActOnEveryObject(x => behavior.AddTool(x), x => behavior.RemoveTool(x));

                behavior.Disposables.Add(disposable);
            }
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

            var table = CreateTableRadDiagramShape();

            var tableBackgroundImage = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), HudDefaultSettings.TableBackgroundImage));

            RenderOptions.SetBitmapScalingMode(tableBackgroundImage, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(tableBackgroundImage, EdgeMode.Aliased);

            table.Background = new ImageBrush(tableBackgroundImage);

            diagram.AddShape(table);

            table.X = DiagramActualWidth(diagram) / 2 - table.Width / 2;
            table.Y = diagram.Height / 2 - table.Height / 2;

            diagram.Group(HudDefaultSettings.TableRadDiagramGroup, diagram.Shapes.OfType<IGroupable>().ToArray());
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
        }

        protected override void OnDetaching()
        {
            if (Disposables != null)
            {
                Disposables.Dispose();
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

        #region Infrastructure

        private static RadDiagramShape CreateTableRadDiagramShape()
        {
            var table = new RadDiagramShape()
            {
                Height = HudDefaultSettings.TableHeight,
                Width = HudDefaultSettings.TableWidth,
                StrokeThickness = 0,
                IsEnabled = false,
                SnapsToDevicePixels = true
            };

            return table;
        }

        /// <summary>
        /// Creates <see cref="RadDiagramShape"/> for player based of seat
        /// </summary>
        /// <param name="seats">Total seats</param>
        /// <param name="seat">Player seat</param>
        /// <returns><see cref="RadDiagramShape"/></returns>
        private static RadDiagramShape CreatePlayerLabel(int seats, int seat)
        {
            var label = new RadDiagramShape
            {
                DataContext = new HudPlayerViewModel
                {
                    Player = string.Format(HudDefaultSettings.TablePlayerNameFormat, seat),
                    Bank = HudDefaultSettings.TablePlayerBank
                },
                Height = HudDefaultSettings.TablePlayerLabelHeight,
                Width = HudDefaultSettings.TablePlayerLabelWidth,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = false,
                IsHitTestVisible = false,
                IsRotationEnabled = false,
                X = HudDefaultSettings.TablePlayerLabelPositions[seats][seat - 1, 0],
                Y = HudDefaultSettings.TablePlayerLabelPositions[seats][seat - 1, 1],
                Background = System.Windows.Application.Current.Resources["HudPlayerBrush"] as VisualBrush
            };

            return label;
        }

        private void ClearDiagram()
        {
            if (AssociatedObject == null)
            {
                return;
            }

            foreach (var shape in RemovableShapes)
            {
                AssociatedObject.RemoveShape(shape);
            }
        }

        #endregion

        #region Add/Remove hud elements      

        /// <summary>
        /// Adds new shape for <see cref="HudBaseToolViewModel"/> to shapes
        /// </summary>
        /// <param name="toolViewModel">ViewModel of tool to add</param>
        private void AddTool(HudBaseToolViewModel toolViewModel)
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
            SetIsSelectedBinding(shape, toolViewModel);

            AssociatedObject.AddShape(shape);
            RemovableShapes.Add(shape);
        }

        /// <summary>
        /// Removes shape for <see cref="HudBaseToolViewModel"/> from shapes
        /// </summary>
        /// <param name="toolViewModel">ViewModel of tool to add</param>
        private void RemoveTool(HudBaseToolViewModel toolViewModel)
        {
            if (AssociatedObject == null || toolViewModel == null)
            {
                return;
            }

            var shape = AssociatedObject.Shapes.OfType<RadDiagramShape>().FirstOrDefault(x => ReferenceEquals(x.DataContext, toolViewModel));

            if (shape != null)
            {
                AssociatedObject.RemoveShape(shape);
            }
        }

        #endregion

        #region Helpers

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

        protected static void SetIsSelectedBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            SetBinding(shape, RadDiagramItem.IsSelectedProperty, viewModel, nameof(HudBaseToolViewModel.IsSelected), BindingMode.OneWayToSource);
        }

        protected static void SetBinding(FrameworkElement element, DependencyProperty dp, object source, string property, BindingMode bindingMode = BindingMode.TwoWay)
        {
            BindingOperations.ClearBinding(element, dp);

            var binding = new Binding(property)
            {
                Source = source,
                Mode = bindingMode
            };

            element.SetBinding(dp, binding);
        }

        protected static void SetShapeSize(RadDiagramShape shape)
        {
            shape.Height = double.NaN;
        }

        protected static double DiagramActualWidth(RadDiagram diagram)
        {
            return diagram.ActualWidth == 0 ? HudDefaultSettings.HudTableWidth : diagram.ActualWidth;
        }

        protected static double DiagramActualHeigth(RadDiagram diagram)
        {
            return diagram.ActualHeight == 0 ? HudDefaultSettings.HudTableHeight : diagram.ActualHeight;
        }

        #endregion
    }
}