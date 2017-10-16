//-----------------------------------------------------------------------
// <copyright file="ResizableRadioButton.cs" company="Ace Poker Solutions">
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
using System.Windows.Controls;

namespace DriveHUD.PlayerXRay.Controls
{
    public class ResizableRadioButton : RadioButton
    {
        /// <summary>
        /// Diameter of outer ellipse dependency property 
        /// </summary>
        public static readonly DependencyProperty OuterDiameterProperty = DependencyProperty.Register("OuterDiameter", typeof(double), typeof(ResizableRadioButton));

        /// <summary>
        /// Diameter of outer ellipse
        /// </summary>
        public double OuterDiameter
        {
            get
            {
                return (double)GetValue(OuterDiameterProperty);
            }
            set
            {
                SetValue(OuterDiameterProperty, value);
            }
        }

        /// <summary>
        /// Diameter of inner ellipse dependency property 
        /// </summary>
        public static readonly DependencyProperty InnerDiameterProperty = DependencyProperty.Register("InnerDiameter", typeof(double), typeof(ResizableRadioButton));

        /// <summary>
        /// Diameter of inner ellipse
        /// </summary>
        public double InnerDiameter
        {
            get
            {
                return (double)GetValue(InnerDiameterProperty);
            }
            set
            {
                SetValue(InnerDiameterProperty, value);
            }
        }
    }
}
