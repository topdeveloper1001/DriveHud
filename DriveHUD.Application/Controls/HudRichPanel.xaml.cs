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

using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.Application.Controls
{
    /// <summary>
    /// Interaction logic for HudRichPanel.xaml
    /// </summary>
    public partial class HudRichPanel : UserControl
    {
        private System.Windows.Point _startPoint;
        private bool _dragged = false;

        public HudRichPanel()
        {
            InitializeComponent();

            this.PreviewMouseMove += HudRichPanel_PreviewMouseMove;
            this.PreviewMouseRightButtonUp += HudRichPanel_PreviewMouseRightButtonUp;
            this.AddHandler(UserControl.PreviewMouseRightButtonDownEvent, new MouseButtonEventHandler(HudRichPanel_PreviewMouseRightButtonDown), true);
        }

        private void HudRichPanel_PreviewMouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_dragged)
            {
                e.Handled = true;
            }
            _dragged = false;
        }

        private void HudRichPanel_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.RightButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                System.Windows.Point position = e.GetPosition(null);

                if (Math.Abs(position.X - _startPoint.X) > System.Windows.SystemParameters.MinimumHorizontalDragDistance ||
                        Math.Abs(position.Y - _startPoint.Y) > System.Windows.SystemParameters.MinimumVerticalDragDistance)
                {
                    _dragged = true;
                }
            }
        }

        private void HudRichPanel_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);
            _dragged = false;
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