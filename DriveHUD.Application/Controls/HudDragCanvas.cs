//-----------------------------------------------------------------------
// <copyright file="DragCanvas.cs" company="Ace Poker Solutions">
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
using DriveHUD.Application.ViewModels.Hud;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Linq;
using DriveHUD.Common.Linq;
using System.Collections.Generic;

namespace DriveHUD.Application.Controls
{
    /// <summary> 
    /// A Canvas which manages dragging of the UIElements it contains.   
    /// </summary>   
    public class HudDragCanvas : Canvas
    {
        #region Data 

        private DragElement elementBeingDragged;
        private List<DragElement> elementsInDragGroup;

        private Point origCursorLocation;

        private bool isDragInProgress;
        private bool isGroupDragInProgress;

        internal double XFraction { get; set; }

        internal double YFraction { get; set; }

        #endregion

        #region Events

        public event EventHandler DragEnded;

        #endregion

        #region Static Constructor 

        static HudDragCanvas()
        {
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(HudDragCanvas),
                new PropertyMetadata(true));

            AllowDragOutOfViewProperty = DependencyProperty.Register(
                "AllowDragOutOfView",
                typeof(bool),
                typeof(HudDragCanvas),
                new UIPropertyMetadata(false));

            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(HudDragCanvas),
                new UIPropertyMetadata(true));
        }

        #endregion

        #region Constructor 

        public HudDragCanvas()
        {
        }

        #endregion

        #region Attached Properties 

        #region CanBeDragged 

        public static readonly DependencyProperty CanBeDraggedProperty;

        public static bool GetCanBeDragged(UIElement uiElement)
        {
            if (uiElement == null)
            {
                return false;
            }

            return (bool)uiElement.GetValue(CanBeDraggedProperty);
        }
        public static void SetCanBeDragged(UIElement uiElement, bool value)
        {
            if (uiElement != null)
            {
                uiElement.SetValue(CanBeDraggedProperty, value);
            }
        }

        #endregion

        #endregion

        #region Interface 

        #region AllowDragging 

        public static readonly DependencyProperty AllowDraggingProperty;

        public bool AllowDragging
        {
            get
            {
                return (bool)GetValue(AllowDraggingProperty);
            }
            set
            {
                SetValue(AllowDraggingProperty, value);
            }
        }

        #endregion

        #region AllowDragOutOfView 

        public static readonly DependencyProperty AllowDragOutOfViewProperty;

        public bool AllowDragOutOfView
        {
            get
            {
                return (bool)GetValue(AllowDragOutOfViewProperty);
            }
            set
            {
                SetValue(AllowDragOutOfViewProperty, value);
            }
        }

        #endregion

        #region BringToFront / SendToBack 

        public void BringToFront(UIElement element)
        {
            UpdateZOrder(element, true);
        }

        public void SendToBack(UIElement element)
        {
            UpdateZOrder(element, false);
        }

        #endregion

        #region ElementBeingDragged 

        internal DragElement ElementBeingDragged
        {
            get
            {
                if (!AllowDragging)
                {
                    return null;
                }
                else
                {
                    return elementBeingDragged;
                }
            }
            private set
            {
                if (elementBeingDragged != null)
                {
                    elementBeingDragged.Element.ReleaseMouseCapture();
                }

                if (!AllowDragging)
                {
                    elementBeingDragged = null;
                }
                else
                {
                    if (value != null && GetCanBeDragged(value.Element))
                    {
                        elementBeingDragged = value;
                        elementBeingDragged.Element.CaptureMouse();
                    }
                    else
                    {
                        elementBeingDragged = null;
                    }
                }
            }
        }

        public Point OrigCursorLocation
        {
            get
            {
                return origCursorLocation;
            }

            set
            {
                origCursorLocation = value;
            }
        }

        #endregion

        #region FindCanvasChild

        public UIElement FindCanvasChild(DependencyObject depObj)
        {
            while (depObj != null)
            {
                UIElement elem = depObj as UIElement;

                if (elem != null && base.Children.Contains(elem))
                {
                    break;
                }

                if (depObj is Visual || depObj is Visual3D)
                {
                    depObj = VisualTreeHelper.GetParent(depObj);
                }
                else
                {
                    depObj = LogicalTreeHelper.GetParent(depObj);
                }
            }

            return depObj as UIElement;
        }

        #endregion

        #endregion

        #region Overrides 

        #region OnPreviewMouseRightButtonDown 

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseRightButtonDown(e);
            isGroupDragInProgress = false;
            InitializeDragProgress(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            InitializeDragProgress(e);

            isGroupDragInProgress = false;

            var elementBeingDraggedViewModel = ElementBeingDragged.DataContext as IHudNonPopupToolViewModel;

            if (elementBeingDraggedViewModel == null)
            {
                return;
            }

            var nonPopupTools = elementBeingDraggedViewModel.Parent.Tools
                   .OfType<IHudNonPopupToolViewModel>()
                   .ToArray();

            var toolViews = (from child in Children.OfType<FrameworkElement>()
                             let childViewModel = child.DataContext as IHudNonPopupToolViewModel
                             where childViewModel != null
                             select new { FrameworkElement = child, DataContext = childViewModel }).ToArray();

            elementsInDragGroup = (from toolView in toolViews
                                   join nonPopupTool in nonPopupTools on toolView.DataContext equals nonPopupTool
                                   select CreateDragElement(toolView.FrameworkElement)).ToList();

            isGroupDragInProgress = true;
        }

        private void InitializeDragProgress(MouseButtonEventArgs e)
        {
            isDragInProgress = false;
            OrigCursorLocation = e.GetPosition(this);

            var uiElementBeingDragged = FindCanvasChild(e.Source as DependencyObject) as FrameworkElement;

            if (uiElementBeingDragged == null)
            {
                return;
            }

            ElementBeingDragged = CreateDragElement(uiElementBeingDragged);

            e.Handled = true;

            isDragInProgress = true;
        }

        private DragElement CreateDragElement(FrameworkElement element)
        {
            bool modifyLeftOffset, modifyTopOffset;

            double left = GetLeft(element);
            double right = GetRight(element);
            double top = GetTop(element);
            double bottom = GetBottom(element);

            var origHorizOffset = ResolveOffset(left, right, out modifyLeftOffset);
            var origVertOffset = ResolveOffset(top, bottom, out modifyTopOffset);

            var dragElement = new DragElement
            {
                Element = element,
                DataContext = element.DataContext as IHudWindowElement,
                OrigHorizOffset = origHorizOffset,
                OrigVertOffset = origVertOffset,
                ModifyLeftOffset = modifyLeftOffset,
                ModifyTopOffset = modifyTopOffset
            };

            return dragElement;
        }

        #endregion

        #region OnPreviewMouseMove 

        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnPreviewMouseMove(e);

            if (ElementBeingDragged == null || !isDragInProgress)
            {
                return;
            }

            Point cursorLocation = e.GetPosition(this);

            var shiftX = cursorLocation.X - this.OrigCursorLocation.X;
            var shiftY = cursorLocation.Y - this.OrigCursorLocation.Y;

            #region Move Drag Element          

            SetElementPosition(ElementBeingDragged, shiftX, shiftY);

            if (isGroupDragInProgress)
            {
                elementsInDragGroup.ForEach(x => SetElementPosition(x, shiftX, shiftY));
            }

            #endregion
        }

        private void SetElementPosition(DragElement element, double shiftX, double shiftY)
        {
            double newHorizontalOffset, newVerticalOffset;

            #region Calculate Offsets 

            if (element.ModifyLeftOffset)
            {
                newHorizontalOffset = element.OrigHorizOffset + shiftX;
            }
            else
            {
                newHorizontalOffset = element.OrigHorizOffset - shiftX;
            }

            if (element.ModifyTopOffset)
            {
                newVerticalOffset = element.OrigVertOffset + shiftY;
            }
            else
            {
                newVerticalOffset = element.OrigVertOffset - shiftY;
            }

            if (!AllowDragOutOfView)
            {
                #region Verify Drag Element Location 

                Rect elemRect = CalculateDragElementRect(element, newHorizontalOffset, newVerticalOffset);

                bool leftAlign = elemRect.Left < 0;
                bool rightAlign = elemRect.Right > ActualWidth;

                if (leftAlign)
                {
                    newHorizontalOffset = element.ModifyLeftOffset ? 0 : ActualWidth - elemRect.Width;
                }
                else if (rightAlign)
                {
                    newHorizontalOffset = element.ModifyLeftOffset ? ActualWidth - elemRect.Width : 0;
                }

                bool topAlign = elemRect.Top < 0;

                bool bottomAlign = elemRect.Bottom > ActualHeight;

                if (topAlign)
                {
                    newVerticalOffset = element.ModifyTopOffset ? 0 : ActualHeight - elemRect.Height;
                }
                else if (bottomAlign)
                {
                    newVerticalOffset = element.ModifyTopOffset ? ActualHeight - elemRect.Height : 0;
                }

                #endregion
            }

            #endregion

            if (element.ModifyLeftOffset)
            {
                SetLeft(element.Element, newHorizontalOffset);

                if (element.DataContext != null)
                {
                    element.DataContext.OffsetX = newHorizontalOffset / XFraction;
                }
            }
            else
            {
                SetRight(element.Element, newHorizontalOffset);
            }

            if (element.ModifyTopOffset)
            {
                SetTop(element.Element, newVerticalOffset);

                if (element.DataContext != null)
                {
                    element.DataContext.OffsetY = newVerticalOffset / YFraction;
                }
            }
            else
            {
                SetBottom(element.Element, newVerticalOffset);
            }
        }

        #endregion

        #region OnHostPreviewMouseUp 

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);

            if (isDragInProgress)
            {
                isDragInProgress = false;
                DragEnded?.Invoke(ElementBeingDragged.Element, new EventArgs());
            }

            ElementBeingDragged = null;
            elementsInDragGroup = null;
        }

        #endregion

        #endregion

        #region Private Helpers 

        #region CalculateDragElementRect 

        private Rect CalculateDragElementRect(DragElement element, double newHorizOffset, double newVertOffset)
        {
            if (ElementBeingDragged == null)
            {
                throw new InvalidOperationException("ElementBeingDragged is null.");
            }

            Size elemSize = element.Element.RenderSize;

            double x, y;

            if (element.ModifyLeftOffset)
            {
                x = newHorizOffset;
            }
            else
            {
                x = ActualWidth - newHorizOffset - elemSize.Width;
            }

            if (element.ModifyTopOffset)
            {
                y = newVertOffset;
            }
            else
            {
                y = ActualHeight - newVertOffset - elemSize.Height;
            }

            Point elemLoc = new Point(x, y);

            return new Rect(elemLoc, elemSize);
        }

        #endregion

        #region ResolveOffset   

        private static double ResolveOffset(double side1, double side2, out bool useSide1)
        {
            useSide1 = true;

            double result;

            if (double.IsNaN(side1))
            {
                if (double.IsNaN(side2))
                {
                    result = 0;
                }
                else
                {
                    result = side2;
                    useSide1 = false;
                }
            }
            else
            {
                result = side1;
            }

            return result;
        }

        #endregion

        #region UpdateZOrder 

        private void UpdateZOrder(UIElement element, bool bringToFront)
        {
            #region Safety Check 

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (!Children.Contains(element))
            {
                throw new ArgumentException("Must be a child element of the Canvas.", "element");
            }

            #endregion

            #region Calculate Z-Index And Offset 

            int elementNewZIndex = -1;

            if (bringToFront)
            {
                foreach (UIElement elem in base.Children)
                {
                    if (elem.Visibility != Visibility.Collapsed)
                    {
                        ++elementNewZIndex;
                    }
                }
            }
            else
            {
                elementNewZIndex = 0;
            }

            int offset = (elementNewZIndex == 0) ? +1 : -1;

            int elementCurrentZIndex = GetZIndex(element);

            #endregion

            #region Update Z-Index

            foreach (UIElement childElement in Children)
            {
                if (childElement == element)
                {
                    SetZIndex(element, elementNewZIndex);
                }
                else
                {
                    int zIndex = GetZIndex(childElement);

                    if (bringToFront && elementCurrentZIndex < zIndex ||
                        !bringToFront && zIndex < elementCurrentZIndex)
                    {
                        SetZIndex(childElement, zIndex + offset);
                    }
                }
            }

            #endregion
        }

        #endregion

        #endregion

        internal class DragElement
        {
            public FrameworkElement Element { get; set; }

            public IHudWindowElement DataContext { get; set; }

            public double OrigHorizOffset { get; set; }

            public double OrigVertOffset { get; set; }

            public bool ModifyLeftOffset { get; set; }

            public bool ModifyTopOffset { get; set; }
        }
    }
}