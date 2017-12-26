//-----------------------------------------------------------------------
// <copyright file="GadgetPanel.xaml.cs" company="Ace Poker Solutions">
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
using System.Windows.Media;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for GadgetPanel.xaml
    /// </summary>
    public partial class GadgetPanel : UserControl
    {
        public static readonly DependencyProperty HeaderColorProperty = DependencyProperty.Register(
            "HeaderColor", typeof(Brush), typeof(GadgetPanel), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        public Brush HeaderColor
        {
            get { return (Brush) GetValue(HeaderColorProperty); }
            set { SetValue(HeaderColorProperty, value); }
        }

        public GadgetPanel()
        {
            InitializeComponent();
        }
    }
}