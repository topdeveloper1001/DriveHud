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

using DriveHUD.Common.Resources;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Diagrams;
using Telerik.Windows.Diagrams.Core;

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

        private RadDiagramShape dragDropShape;

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

        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(HudDesignerBehavior), new PropertyMetadata(IsReadOnlyChanged));

        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }
            set
            {
                SetValue(IsReadOnlyProperty, value);
            }
        }

        private static void IsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudDesignerBehavior;

            if (behavior == null)
            {
                return;
            }

            behavior.RemovableShapes.ForEach(x => behavior.SetReadOnly(x as RadDiagramShape, (bool)e.NewValue));
        }

        public static readonly DependencyProperty DragDropCommandProperty = DependencyProperty.Register("DragDropCommand", typeof(System.Windows.Input.ICommand),
            typeof(HudDesignerBehavior), new PropertyMetadata(OnDragDropCommandChanged));

        public System.Windows.Input.ICommand DragDropCommand
        {
            get
            {
                return (System.Windows.Input.ICommand)GetValue(DragDropCommandProperty);
            }
            set
            {
                SetValue(DragDropCommandProperty, value);
            }
        }

        private static void OnDragDropCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as HudDesignerBehavior;

            if (behavior == null || behavior.dragDropShape == null)
            {
                return;
            }

            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetDragDropCommand(behavior.dragDropShape, behavior.DragDropCommand);
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

            if (behavior.dragDropShape != null)
            {
                behavior.dragDropShape.IsSelected = false;
            }

            behavior.Disposables = new CompositeDisposable();

            behavior.ClearDiagram();

            for (var seat = 0; seat < hudElements.Count; seat++)
            {
                // add player label
                var playerLabel = CreatePlayerLabel(hudElements.Count, seat + 1);
                behavior.AssociatedObject.AddShape(playerLabel);
                behavior.RemovableShapes.Add(playerLabel);

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
            AssociatedObject.Initialized += OnInitialized;
        }

        private void OnInitialized(object sender, EventArgs e)
        {
            var diagram = sender as RadDiagram;

            if (diagram == null)
            {
                return;
            }

            diagram.SelectionMode = SelectionMode.Single;

            var table = CreateTableRadDiagramShape();

            var tableBackgroundImage = new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), HudDefaultSettings.TableBackgroundImage));

            RenderOptions.SetBitmapScalingMode(tableBackgroundImage, BitmapScalingMode.HighQuality);
            RenderOptions.SetEdgeMode(tableBackgroundImage, EdgeMode.Aliased);

            table.Background = new ImageBrush(tableBackgroundImage);

            diagram.AddShape(table);

            table.X = DiagramActualWidth(diagram) / 2 - table.Width / 2;
            table.Y = diagram.Height / 2 - table.Height / 2;

            dragDropShape = CreateDragDropRadDiagramShape();
            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetIsDragTarget(dragDropShape, true);
            DriveHUD.Common.Wpf.AttachedBehaviors.DragDrop.SetGroupProperty(dragDropShape, HudDragDropGroups.Common);
            diagram.AddShape(dragDropShape);
        }

        protected override void OnDetaching()
        {
            if (Disposables != null)
            {
                Disposables.Dispose();
            }

            AssociatedObject.ViewportChanged -= OnViewportChanged;
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

        private static RadDiagramShape CreateDragDropRadDiagramShape()
        {
            var shape = new RadDiagramShape()
            {
                Height = HudDefaultSettings.HudTableHeight,
                Width = HudDefaultSettings.HudTableWidth,
                StrokeThickness = 0,
                IsEnabled = true,
                IsEditable = false,
                IsManipulationEnabled = false,
                IsResizingEnabled = false,
                IsRotationEnabled = false,
                IsDraggingEnabled = false,
                IsConnectorsManipulationEnabled = false,
                IsManipulationAdornerVisible = false,
                IsSelected = false,
                IsInEditMode = false,
                SnapsToDevicePixels = true,
                Background = new SolidColorBrush(Colors.Transparent)
            };

            return shape;
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

            RemovableShapes.Clear();
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

            SetReadOnly(shape, IsReadOnly);

            SetWidthBinding(shape, toolViewModel);
            SetHeightBinding(shape, toolViewModel);
            SetPositionBinding(shape, toolViewModel);
            SetOpacityBinding(shape, toolViewModel);
            SetIsSelectedBinding(shape, toolViewModel);
            SetIsVisibleBinding(shape, toolViewModel);

            AssociatedObject.AddShape(shape);
            RemovableShapes.Add(shape);
        }

        /// <summary>
        /// Makes shape with tool read only
        /// </summary>
        /// <param name="shape">Shape</param>
        /// <param name="isReadOnly"></param>
        private void SetReadOnly(RadDiagramShape shape, bool isReadOnly)
        {
            if (shape == null)
            {
                return;
            }

            shape.IsEditable = !isReadOnly;            
            shape.IsResizingEnabled = !isReadOnly;
            shape.IsDraggingEnabled = !isReadOnly;
            shape.IsConnectorsManipulationEnabled = !isReadOnly;
            shape.IsManipulationAdornerVisible = !isReadOnly;                       
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
            SetBinding(shape, RadDiagramItem.IsSelectedProperty, viewModel, nameof(HudBaseToolViewModel.IsSelected), viewModel.IsSelectedBindingMode);
        }

        protected static void SetIsVisibleBinding(RadDiagramShape shape, HudBaseToolViewModel viewModel)
        {
            var converter = new BooleanToVisibilityConverter();
            SetBinding(shape, UIElement.VisibilityProperty, viewModel, nameof(HudBaseToolViewModel.IsVisible), BindingMode.OneWay, converter);
        }

        protected static void SetBinding(FrameworkElement element, DependencyProperty dp, object source, string property,
            BindingMode bindingMode = BindingMode.TwoWay, IValueConverter converter = null)
        {
            BindingOperations.ClearBinding(element, dp);

            var binding = new Binding(property)
            {
                Source = source,
                Mode = bindingMode
            };

            if (converter != null)
            {
                binding.Converter = converter;
            }

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