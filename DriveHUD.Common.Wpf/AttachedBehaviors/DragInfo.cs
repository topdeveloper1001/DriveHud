//-----------------------------------------------------------------------
// <copyright file="DragInfo.cs" company="Ace Poker Solutions">
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
using System.Windows.Input;

namespace DriveHUD.Common.Wpf.AttachedBehaviors
{
    public class DragInfo
    {
        public DragInfo(object sender, MouseButtonEventArgs e)
        {
            VisualSource = sender as UIElement;
            DragStartPosition = e.GetPosition((IInputElement)sender);
        }

        public UIElement VisualSource
        {
            get;
            private set;
        }

        public Point DragStartPosition
        {
            get;
            private set;
        }
    }
}