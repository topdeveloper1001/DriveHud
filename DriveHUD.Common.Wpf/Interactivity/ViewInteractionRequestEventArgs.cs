//-----------------------------------------------------------------------
// <copyright file="ViewInteractionRequestEventArgs.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.Common.Wpf.Interactivity
{
    public class ViewInteractionRequestEventArgs : EventArgs
    {
        public ViewInteractionRequestEventArgs(string viewName)
           : this(viewName, null, null)
        {
        }

        public ViewInteractionRequestEventArgs(string viewName, Action<object> availableCallback)
            : this(viewName, null, availableCallback)
        {
        }

        public ViewInteractionRequestEventArgs(string viewName, ViewModelInfo info, Action<object> availableCallback)
        {
            ViewName = viewName;
            Info = info;
            AvailableCallback = availableCallback;
        }

        public string ViewName { get; private set; }

        public ViewModelInfo Info { get; private set; }

        public Action<object> AvailableCallback { get; private set; }
    }
}