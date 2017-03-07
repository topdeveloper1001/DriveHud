//-----------------------------------------------------------------------
// <copyright file="DragAdorner.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class DragAdorner : Adorner
    {
        private readonly AdornerLayer adornerLayer;
        private readonly UIElement adornment;

        public DragAdorner(UIElement adornedElement, UIElement adornment, DragDropEffects effects = DragDropEffects.None) : base(adornedElement)
        {
            Check.ArgumentNotNull(() => adornedElement);
            Check.ArgumentNotNull(() => adornment);

            adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
            adornerLayer.Add(this);

            this.adornment = adornment;
            IsHitTestVisible = false;

            Effects = effects;
        }

        public DragDropEffects Effects { get; private set; }

        private Point mousePosition;

        public Point MousePosition
        {
            get
            {
                return mousePosition;
            }
            set
            {
                if (mousePosition != value)
                {
                    mousePosition = value;
                    adornerLayer.Update(AdornedElement);
                }
            }
        }

        public void Detatch()
        {
            adornerLayer.Remove(this);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            adornment.Arrange(new Rect(finalSize));
            return finalSize;
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();
            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(MousePosition.X - 4, MousePosition.Y - 4));

            return result;
        }

        protected override Visual GetVisualChild(int index)
        {
            return adornment;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            adornment.Measure(constraint);
            return adornment.DesiredSize;
        }

        protected override int VisualChildrenCount
        {
            get { return 1; }
        }
    }
}