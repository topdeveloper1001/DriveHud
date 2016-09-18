//-----------------------------------------------------------------------
// <copyright file="IDropDownMenuItem.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace DriveHUD.Application.Controls
{
    public interface IDropDownMenuItem : INotifyPropertyChanged
    {
        string Header { get; set; }

        ObservableCollection<IDropDownMenuItem> Items { get; set; }

        IDropDownMenuItem Parent { get; set; }
    }
}