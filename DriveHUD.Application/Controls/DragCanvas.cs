using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Hud;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DriveHUD.Application.Controls
{
    /// <summary> 
    /// A Canvas which manages dragging of the UIElements it contains.   
    /// </summary>   
    public class DragCanvas : Canvas
    {
        #region Data 

        private UIElement elementBeingDragged;
        private Point origCursorLocation;
        private double origHorizOffset, origVertOffset;
        private bool modifyLeftOffset, modifyTopOffset;
        private bool isDragInProgress;

        internal double XFraction { get; set; }

        internal double YFraction { get; set; }

        #endregion

        #region Events

        public event EventHandler DragEnded;

        #endregion

        #region Static Constructor 

        static DragCanvas()
        {
            AllowDraggingProperty = DependencyProperty.Register(
                "AllowDragging",
                typeof(bool),
                typeof(DragCanvas),
                new PropertyMetadata(true));

            AllowDragOutOfViewProperty = DependencyProperty.Register(
                "AllowDragOutOfView",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(false));

            CanBeDraggedProperty = DependencyProperty.RegisterAttached(
                "CanBeDragged",
                typeof(bool),
                typeof(DragCanvas),
                new UIPropertyMetadata(true));
        }

        #endregion

        #region Constructor 

        public DragCanvas()
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
                return (bool)base.GetValue(AllowDraggingProperty);
            }
            set
            {
                base.SetValue(AllowDraggingProperty, value);
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
            this.UpdateZOrder(element, true);
        }

        public void SendToBack(UIElement element)
        {
            this.UpdateZOrder(element, false);
        }

        #endregion

        #region ElementBeingDragged 

        public UIElement ElementBeingDragged
        {
            get
            {
                if (!AllowDragging)
                {
                    return null;
                }
                else
                {
                    return this.elementBeingDragged;
                }
            }
            protected set
            {
                if (elementBeingDragged != null)
                {
                    elementBeingDragged.ReleaseMouseCapture();
                }

                if (!AllowDragging)
                {
                    elementBeingDragged = null;
                }
                else
                {
                    if (GetCanBeDragged(value))
                    {
                        elementBeingDragged = value;
                        elementBeingDragged.CaptureMouse();
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
            isDragInProgress = false;
            OrigCursorLocation = e.GetPosition(this);

            ElementBeingDragged = this.FindCanvasChild(e.Source as DependencyObject);

            if (ElementBeingDragged == null)
            {
                return;
            }

            double left = GetLeft(ElementBeingDragged);
            double right = GetRight(ElementBeingDragged);
            double top = GetTop(ElementBeingDragged);
            double bottom = GetBottom(ElementBeingDragged);

            origHorizOffset = ResolveOffset(left, right, out modifyLeftOffset);
            origVertOffset = ResolveOffset(top, bottom, out modifyTopOffset);

            e.Handled = true;

            isDragInProgress = true;
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

            double newHorizontalOffset, newVerticalOffset;

            #region Calculate Offsets 

            if (modifyLeftOffset)
            {
                newHorizontalOffset = this.origHorizOffset + (cursorLocation.X - this.OrigCursorLocation.X);
            }
            else
            {
                newHorizontalOffset = this.origHorizOffset - (cursorLocation.X - this.OrigCursorLocation.X);
            }

            if (modifyTopOffset)
            {
                newVerticalOffset = this.origVertOffset + (cursorLocation.Y - this.OrigCursorLocation.Y);
            }
            else
            {
                newVerticalOffset = this.origVertOffset - (cursorLocation.Y - this.OrigCursorLocation.Y);
            }

            #endregion

            if (!AllowDragOutOfView)
            {
                #region Verify Drag Element Location 

                Rect elemRect = this.CalculateDragElementRect(newHorizontalOffset, newVerticalOffset);

                bool leftAlign = elemRect.Left < 0;
                bool rightAlign = elemRect.Right > this.ActualWidth;

                if (leftAlign)
                {
                    newHorizontalOffset = modifyLeftOffset ? 0 : ActualWidth - elemRect.Width;
                }
                else if (rightAlign)
                {
                    newHorizontalOffset = modifyLeftOffset ? ActualWidth - elemRect.Width : 0;
                }

                bool topAlign = elemRect.Top < 0;

                bool bottomAlign = elemRect.Bottom > ActualHeight;

                if (topAlign)
                {
                    newVerticalOffset = modifyTopOffset ? 0 : ActualHeight - elemRect.Height;
                }
                else if (bottomAlign)
                {
                    newVerticalOffset = modifyTopOffset ? ActualHeight - elemRect.Height : 0;
                }

                #endregion
            }

            #region Move Drag Element 

            var hudPanelBeingDragged = ElementBeingDragged as FrameworkElement;

            IHudWindowElement hudElementViewModel = null;

            if (hudPanelBeingDragged != null)
            {
                hudElementViewModel = hudPanelBeingDragged.DataContext as IHudWindowElement;
            }

            if (modifyLeftOffset)
            {
                SetLeft(ElementBeingDragged, newHorizontalOffset);

                if (hudElementViewModel != null)
                {
                    hudElementViewModel.OffsetX = newHorizontalOffset / XFraction;
                }
            }
            else
            {
                SetRight(ElementBeingDragged, newHorizontalOffset);
            }

            if (modifyTopOffset)
            {
                SetTop(ElementBeingDragged, newVerticalOffset);

                if (hudElementViewModel != null)
                {
                    hudElementViewModel.OffsetY = newVerticalOffset / YFraction;
                }
            }
            else
            {
                SetBottom(ElementBeingDragged, newVerticalOffset);
            }

            #endregion
        }

        #endregion

        #region OnHostPreviewMouseUp 

        protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseUp(e);
            if (isDragInProgress)
            {
                DragEnded?.Invoke(this.ElementBeingDragged, new EventArgs());
            }
            this.ElementBeingDragged = null;
        }

        #endregion

        #endregion

        #region Private Helpers 

        #region CalculateDragElementRect 

        private Rect CalculateDragElementRect(double newHorizOffset, double newVertOffset)
        {
            if (ElementBeingDragged == null)
            {
                throw new InvalidOperationException("ElementBeingDragged is null.");
            }

            Size elemSize = ElementBeingDragged.RenderSize;

            double x, y;

            if (modifyLeftOffset)
            {
                x = newHorizOffset;
            }
            else
            {
                x = ActualWidth - newHorizOffset - elemSize.Width;
            }

            if (modifyTopOffset)
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
    }
}