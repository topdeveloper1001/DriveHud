//-----------------------------------------------------------------------
// <copyright file="MouseDragPopupBehavior.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class MouseDragPopupBehavior : Behavior<Popup>
    {
        private bool mouseDown;
        private Point oldMousePosition;

        protected override void OnAttached()
        {
            AssociatedObject.MouseLeftButtonDown += (s, e) =>
            {
                mouseDown = true;
                oldMousePosition = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));
                AssociatedObject.Child.CaptureMouse();
            };

            AssociatedObject.MouseMove += (s, e) =>
            {
                if (!mouseDown)
                {
                    return;
                }

                var newMousePosition = AssociatedObject.PointToScreen(e.GetPosition(AssociatedObject));
                var offset = newMousePosition - oldMousePosition;
                oldMousePosition = newMousePosition;

                AssociatedObject.HorizontalOffset += offset.X;
                AssociatedObject.VerticalOffset += offset.Y;
            };

            AssociatedObject.MouseLeftButtonUp += (s, e) =>
            {
                mouseDown = false;
                AssociatedObject.Child.ReleaseMouseCapture();
            };

            AssociatedObject.Closed += delegate
            {
                AssociatedObject.HorizontalOffset = 0;
                AssociatedObject.VerticalOffset = 0;
            };
        }
    }
}