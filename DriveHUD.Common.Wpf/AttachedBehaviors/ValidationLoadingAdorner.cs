//-----------------------------------------------------------------------
// <copyright file="ValidationLoadingAdorner.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class ValidationLoadingAdorner : Adorner
    {
        private Control child;
        private FrameworkElement _referenceElement;

        public ValidationLoadingAdorner(UIElement adornedElement, ControlTemplate adornerTemplate) : base(adornedElement)
        {
            Debug.Assert(adornedElement != null, "adornedElement should not be null");
            Debug.Assert(adornerTemplate != null, "adornerTemplate should not be null");

            var control = new Control
            {
                IsTabStop = false,
                Template = adornerTemplate
            };

            child = control;
            AddVisualChild(child);
        }

        /// <summary>
        /// The clear the single child of a TemplatedAdorner
        /// </summary>
        public void ClearChild()
        {
            RemoveVisualChild(child);
            child = null;
        }

        /// <summary>
        /// Adorners don't always want to be transformed in the same way as the elements they
        /// adorn.  Adorners which adorn points, such as resize handles, want to be translated
        /// and rotated but not scaled.  Adorners adorning an object, like a marquee, may want
        /// all transforms.  This method is called by AdornerLayer to allow the adorner to
        /// filter out the transforms it doesn't want and return a new transform with just the
        /// transforms it wants applied.  An adorner can also add an additional translation
        /// transform at this time, allowing it to be positioned somewhere other than the upper
        /// left corner of its adorned element.
        /// </summary>
        /// <param name="transform">The transform applied to the object the adorner adorns</param>
        /// <returns>Transform to apply to the adorner</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            if (ReferenceElement == null)
            {
                return transform;
            }

            GeneralTransformGroup group = new GeneralTransformGroup();
            group.Children.Add(transform);

            GeneralTransform t = TransformToDescendant(ReferenceElement);

            if (t != null)
            {
                group.Children.Add(t);
            }

            return group;
        }

        public FrameworkElement ReferenceElement
        {
            get
            {
                return _referenceElement;
            }
            set
            {
                _referenceElement = value;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (child == null || index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return child;
        }

        protected override int VisualChildrenCount
        {
            get { return child != null ? 1 : 0; }
        }

        /// <summary>
        /// Measure adorner.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            Debug.Assert(child != null, "_child should not be null");

            if (ReferenceElement != null && AdornedElement != null &&
                AdornedElement.IsMeasureValid &&
                !AreClose(ReferenceElement.DesiredSize, AdornedElement.DesiredSize))
            {
                ReferenceElement.InvalidateMeasure();
            }

            child.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

            return child.DesiredSize;
        }

        /// <summary>
        ///     Default control arrangement is to only arrange
        ///     the first visual child. No transforms will be applied.
        /// </summary>
        protected override Size ArrangeOverride(Size size)
        {
            Size finalSize;

            finalSize = base.ArrangeOverride(size);

            if (child != null)
            {
                child.Arrange(new Rect(new Point(), finalSize));
            }

            return finalSize;
        }

        private const double DBL_EPSILON = 2.2204460492503131e-016;

        private static bool AreClose(double value1, double value2)
        {
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * DBL_EPSILON;
            double delta = value1 - value2;

            return (-eps < delta) && (eps > delta);
        }

        private static bool AreClose(Size size1, Size size2)
        {
            return AreClose(size1.Width, size2.Width) &&
                   AreClose(size1.Height, size2.Height);
        }
    }
}