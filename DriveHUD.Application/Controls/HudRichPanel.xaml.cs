//-----------------------------------------------------------------------
// <copyright file="HudRichPanel.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Windows.Controls;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for HudRichPanel.xaml
    /// </summary>
    public partial class HudRichPanel : UserControl
    {
        public HudRichPanel()
        {
            InitializeComponent();
        }
    }

    public class TestData
    {
        public TestData()
        {
            IsRightOriented = false;
            IsVertical = true;
        }

        public bool IsRightOriented
        {
            get;
            set;
        }

        public bool IsVertical
        {
            get;
            set;
        }
    }
}