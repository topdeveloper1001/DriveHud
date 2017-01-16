//-----------------------------------------------------------------------
// <copyright file="HudTableLayoutMenuItem.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Controls;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using ReactiveUI;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels.Hud
{
    //public class HudTableLayoutMenuItem : ViewModelBase, IDropDownMenuItem
    //{
    //    private HudTableLayout hudTableLayout;

    //    public HudTableLayoutMenuItem()
    //    {
    //        items = new ObservableCollection<IDropDownMenuItem>();
    //    }

    //    public HudTableLayoutMenuItem(HudTableLayout hudTableLayout) : this()
    //    {
    //        this.hudTableLayout = hudTableLayout;
    //        header = CommonResourceManager.Instance.GetEnumResource(hudTableLayout.TableType);
    //    }

    //    public HudTableLayout HudTableLayout
    //    {
    //        get
    //        {
    //            return hudTableLayout;
    //        }
    //    }

    //    private string header;

    //    public string Header
    //    {
    //        get
    //        {
    //            return header;
    //        }

    //        set
    //        {
    //            this.RaiseAndSetIfChanged(ref header, value);
    //        }
    //    }

    //    private ObservableCollection<IDropDownMenuItem> items;

    //    public ObservableCollection<IDropDownMenuItem> Items
    //    {
    //        get
    //        {
    //            return items;
    //        }
    //        set
    //        {
    //            this.RaiseAndSetIfChanged(ref items, value);
    //        }
    //    }

    //    private IDropDownMenuItem parent;

    //    public IDropDownMenuItem Parent
    //    {
    //        get
    //        {
    //            return parent;
    //        }
    //        set
    //        {
    //            this.RaiseAndSetIfChanged(ref parent, value);
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return Parent != null ? string.Format("{0} {1}", Parent, Header) : Header;
    //    }
    //}
}