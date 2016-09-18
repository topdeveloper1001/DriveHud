﻿//-----------------------------------------------------------------------
// <copyright file="ProgressBarEx.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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

namespace DriveHUD.Common.Wpf.Controls
{
    public class ProgressBarEx : ProgressBar
    {
        public string Text
        {
            get { return GetValue(TextProperty) as string; }
            set { SetValue(TextProperty, value); }
        }

        private static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ProgressBarEx), new PropertyMetadata(null));

        public Visibility TextVisibility
        {
            get
            {
                return string.IsNullOrWhiteSpace(Text) ? Visibility.Collapsed : Visibility.Visible;
            }
        }
    }
}