//-----------------------------------------------------------------------
// <copyright file="GraphViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using ReactiveUI;

namespace DriveHUD.Application.ViewModels.Graphs
{
    public class GraphViewModel : WindowViewModelBase, IBaseGraphViewModel
    {
        public GraphViewModel(IBaseGraphViewModel baseGraphViewModel)
        {
            this.baseGraphViewModel = baseGraphViewModel;
        }

        private IBaseGraphViewModel baseGraphViewModel;

        public IBaseGraphViewModel ViewModel
        {
            get
            {
                return baseGraphViewModel;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref baseGraphViewModel, value);
            }
        }

        public void Update()
        {
            StartAsyncOperation(() => baseGraphViewModel?.Update(), () => { });
        }
    }
}