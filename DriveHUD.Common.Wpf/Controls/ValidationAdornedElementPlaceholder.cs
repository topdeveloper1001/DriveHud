//-----------------------------------------------------------------------
// <copyright file="ValidationAdornedElementPlaceholder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.AttachedBehaviors;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;

namespace DriveHUD.Common.Wpf.Controls
{
    public class ValidationAdornedElementPlaceholder : FrameworkElement, IAddChild
    {
        #region Constructors

        /// <summary>
        ///     Default Control constructor
        /// </summary>
        /// <remarks>
        ///     Automatic determination of current Dispatcher. Use alternative constructor
        ///     that accepts a Dispatcher for best performance.
        /// </remarks>
        public ValidationAdornedElementPlaceholder() : base()
        {
        }

        #endregion Constructors

        ///<summary>
        /// This method is called to Add the object as a child.  This method is used primarily
        /// by the parser; a more direct way of adding a child is to use the <see cref="Child" />
        /// property.
        ///</summary>
        ///<param name="value">
        /// The object to add as a child; it must be a UIElement.
        ///</param>
        void IAddChild.AddChild(object value)
        {
            // keeping consistent with other elements:  adding null is a no-op.
            if (value == null)
            {
                return;
            }

            if (!(value is UIElement))
            {
                throw new ArgumentException(nameof(value));
            }

            if (Child != null)
            {
                throw new ArgumentException("Can only have one child");
            }

            Child = (UIElement)value;
        }

        ///<summary>
        /// This method is called by the parser when text appears under the tag in markup.
        /// Calling this method has no effect if text is just whitespace.  If text is not
        /// just whitespace, throw an exception.
        ///</summary>
        ///<param name="text">
        /// Text to add as a child.
        ///</param>
        void IAddChild.AddText(string text)
        {
            if (text != null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (!Char.IsWhiteSpace(text[i]))
                    {
                        throw new ArgumentException("NonWhiteSpaceInAddText");
                    }
                }
            }
        }

        ///<summary>
        /// Element for which the AdornedElementPlaceholder is reserving space.
        ///</summary>
        public UIElement AdornedElement
        {
            get
            {
                ValidationLoadingAdorner adorner = TemplatedAdorner;
                return adorner == null ? null : TemplatedAdorner.AdornedElement;
            }
        }

        /// <summary>
        /// The single child of an <see cref="AdornedElementPlaceholder" />
        /// </summary>
        [DefaultValue(null)]
        public virtual UIElement Child
        {
            get
            {
                return child;
            }
            set
            {
                UIElement old = child;

                if (old != value)
                {
                    RemoveVisualChild(old);
                    //need to remove old element from logical tree
                    RemoveLogicalChild(old);
                    child = value;

                    AddVisualChild(child);
                    AddLogicalChild(value);

                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets the Visual children count.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                return (child == null) ? 0 : 1;
            }
        }

        /// <summary>
        /// Gets the Visual child at the specified index.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            if (child == null || index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return child;
        }

        /// <summary>
        /// Returns enumerator to logical children.
        /// </summary>
        protected override IEnumerator LogicalChildren
        {
            get
            {
                // Could optimize this code by returning EmptyEnumerator.Instance if _child == null.
                return new SingleChildEnumerator(child);
            }
        }

        /// <summary>
        /// This virtual method in called when IsInitialized is set to true and it raises an Initialized event
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            if (TemplatedParent == null)
            {
                throw new InvalidOperationException("ValidationAdornedElementPlaceholder must be in template");
            }

            base.OnInitialized(e);
        }

        /// <summary>
        ///     ValidationAdornedElementPlaceholder measure behavior is to measure
        ///     only the first visual child.  Note that the return value
        ///     of Measure on this child is ignored as the purpose of this
        ///     class is to match the size of the element for which this
        ///     is a placeholder.
        /// </summary>
        /// <param name="constraint">The measurement constraints.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            if (TemplatedParent == null)
            {
                throw new InvalidOperationException("ValidationAdornedElementPlaceholder must be in template");
            }

            if (AdornedElement == null)
            {
                return new Size(0, 0);
            }

            var desiredSize = AdornedElement.RenderSize;

            var child = Child;

            if (child != null)
            {
                child.Measure(desiredSize);
            }

            return desiredSize;
        }

        /// <summary>
        ///     Default AdornedElementPlaceholder arrangement is to only arrange
        ///     the first visual child. No transforms will be applied.
        /// </summary>
        /// <param name="arrangeBounds">The computed size.</param>
        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var child = Child;

            if (child != null)
            {
                child.Arrange(new Rect(arrangeBounds));
            }

            return arrangeBounds;
        }

        private ValidationLoadingAdorner TemplatedAdorner
        {
            get
            {
                if (templatedAdorner == null)
                {
                    // find the TemplatedAdorner
                    if (TemplatedParent is FrameworkElement templateParent)
                    {
                        templatedAdorner = VisualTreeHelper.GetParent(templateParent) as ValidationLoadingAdorner;

                        if (templatedAdorner != null && templatedAdorner.ReferenceElement == null)
                        {
                            templatedAdorner.ReferenceElement = this;
                        }
                    }
                }

                return templatedAdorner;
            }
        }

        private UIElement child;
        private ValidationLoadingAdorner templatedAdorner;

        #region Helpers

        private class SingleChildEnumerator : IEnumerator
        {
            internal SingleChildEnumerator(object Child)
            {
                _child = Child;
                _count = Child == null ? 0 : 1;
            }

            object IEnumerator.Current
            {
                get { return (_index == 0) ? _child : null; }
            }

            bool IEnumerator.MoveNext()
            {
                _index++;
                return _index < _count;
            }

            void IEnumerator.Reset()
            {
                _index = -1;
            }

            private int _index = -1;
            private int _count = 0;
            private object _child;
        }

        #endregion;
    }
}