//-----------------------------------------------------------------------
// <copyright file="FilterQuickView.xaml.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Filters;
using Model.Filters;
using System.Windows.Controls;

namespace DriveHUD.Application.Views
{
    public partial class FilterQuickView : UserControl, IFilterView
    {
        public FilterQuickView(IFilterModelManagerService service)
        {
            DataContext = new FilterQuickViewModel(service);
            InitializeComponent();
        }

        public IFilterViewModel ViewModel
        {
            get { return DataContext as IFilterViewModel; }
        }
    }
}