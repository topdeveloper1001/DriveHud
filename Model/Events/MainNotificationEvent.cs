//-----------------------------------------------------------------------
// <copyright file="HudLayoutsService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Events;
using System;

namespace Model.Events
{
    public class MainNotificationEventArgs : EventArgs
    {
        public string Title { get; }

        public string Message { get; }

        public string HyperLink { get; }

        public MainNotificationEventArgs(string title, string message)
        {
            Title = title;
            Message = message;
        }

        public MainNotificationEventArgs(string title, string message, string hyperLink) 
            : this(title, message)
        {
            HyperLink = hyperLink;
        }
    }

    public class MainNotificationEvent : PubSubEvent<MainNotificationEventArgs>
    {
    }
}