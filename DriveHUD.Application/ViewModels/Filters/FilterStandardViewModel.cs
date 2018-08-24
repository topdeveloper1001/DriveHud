//-----------------------------------------------------------------------
// <copyright file="FilterStandardViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Filters;
using Model.Interfaces;
using Prism.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels
{
    public class FilterStandardViewModel : FilterViewModel<FilterStandardModel>
    {
        internal FilterStandardViewModel(IFilterModelManagerService service) : base(EnumViewModelType.FilterStandardViewModel, service)
        {
            InitializeBindings();
        }

        public override void InitializeFilterModel()
        {
            var player = StorageModel.PlayerSelectedItem;

            var gameTypes = player != null && FilterModelManager.FilterType == EnumFilterType.Cash ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerGameTypes(player.PlayerIds) :
                new List<Gametypes>();

            var tournaments = player != null && FilterModelManager.FilterType == EnumFilterType.Tournament ?
                ServiceLocator.Current.GetInstance<IDataService>().GetPlayerTournaments(player.PlayerIds) :
                new List<Tournaments>();

            FilterModel = (FilterStandardModel)FilterModelManager.FilterModelCollection.FirstOrDefault(x => x.GetType().Equals(typeof(FilterStandardModel)));

            FilterModel.UpdateFilterSectionStakeLevelCollection(gameTypes);
            FilterModel.UpdateFilterSectionBuyinCollection(tournaments);

            FilterModelClone = (FilterStandardModel)FilterModel.Clone();

            RaisePropertyChanged(nameof(IsStakeLevelVisible));
            RaisePropertyChanged(nameof(IsBuyinVisible));
        }

        public bool IsStakeLevelVisible
        {
            get
            {
                return FilterModelManager.FilterType == EnumFilterType.Cash;
            }
        }

        public bool IsBuyinVisible
        {
            get
            {
                return FilterModelManager.FilterType == EnumFilterType.Tournament;
            }
        }

        private void InitializeBindings()
        {
            ButtonFilterModelStatItemSwap_CommandClick = new DelegateCommand<object>(ButtonFilterModelStatItemSwap_OnClick);
        }

        private void FilterModelStatItemSwap(StatItem param)
        {
            param.TriStateSwap();
        }

        #region Commands

        public ICommand ButtonFilterModelStatItemSwap_CommandClick { get; set; }

        private void ButtonFilterModelStatItemSwap_OnClick(object param)
        {
            FilterModelStatItemSwap((StatItem)param);
        }

        #endregion
    }
}