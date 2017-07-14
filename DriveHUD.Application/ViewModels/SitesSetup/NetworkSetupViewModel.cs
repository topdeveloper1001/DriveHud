//-----------------------------------------------------------------------
// <copyright file="NetworkSetupViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.Entities;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels
{
    public class NetworkSetupViewModel : ViewModelBase
    {
        public NetworkSetupViewModel(EnumPokerNetworks network)
        {
            this.network = network;
            networkSites = new ObservableCollection<SiteSetupViewModel>();
        }

        private EnumPokerNetworks network;

        public EnumPokerNetworks Network
        {
            get
            {
                return network;
            }
        }

        private ObservableCollection<SiteSetupViewModel> networkSites;

        public ObservableCollection<SiteSetupViewModel> NetworkSites
        {
            get
            {
                return networkSites;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref networkSites, value);
            }
        }

        public string SiteLogo
        {
            get
            {                
                return NetworkSites.FirstOrDefault()?.SiteLogo;
            }
        }
    }
}