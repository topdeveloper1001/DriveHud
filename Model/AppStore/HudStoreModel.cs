//-----------------------------------------------------------------------
// <copyright file="HudStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;
using Model.AppStore.HudStore;
using Model.AppStore.HudStore.Model;
using Model.AppStore.HudStore.ServiceData;
using Model.AppStore.HudStore.ServiceResponses;
using System.Collections.ObjectModel;
using System.IO;

namespace Model.AppStore
{
    internal class HudStoreModel : BaseAppStoreModel<HudStoreItem>, IHudStoreModel
    {
        protected readonly IHudStoreWebService service;

        protected string serial;

        protected HudStoreHudsData hudsData;

        protected string searchText;

        public HudStoreModel()
        {
            service = ServiceLocator.Current.GetInstance<IHudStoreWebService>();
        }

        #region Properties

        private int itemsCount;

        public override int ItemsCount
        {
            get
            {
                return itemsCount;
            }
        }

        private HudStoreFilter selectedFilter;

        public HudStoreFilter SelectedFilter
        {
            get
            {
                return selectedFilter;
            }
            set
            {
                SetProperty(ref selectedFilter, value);
            }
        }

        private HudStoreSorting selectedSorting;

        public HudStoreSorting SelectedSorting
        {
            get
            {
                return selectedSorting;
            }
            set
            {
                SetProperty(ref selectedSorting, value);
            }
        }

        #endregion

        #region Model methods

        public override void Load(object loadInfo)
        {
            if (!(loadInfo is HudStoreGetHudsRequest request))
            {
                return;
            }

            serial = request.Serial;

            hudsData = service.GetHuds(request);
            itemsCount = hudsData.ItemsCount;
        }

        public override void Refresh(int page, int amount)
        {
            var request = new HudStoreGetHudsRequest
            {
                Serial = serial,
                Page = page,
                Filter = (int)SelectedFilter,
                Sorting = (int)SelectedSorting,
                Search = searchText
            };

            hudsData = service.GetHuds(request);
            itemsCount = hudsData.ItemsCount;
        }

        public void Refresh()
        {
            Items.Clear();

            if (hudsData != null)
            {
                Items.AddRange(hudsData.Items);
            }
        }

        public Stream DownloadLayout(int layoutId, string serial)
        {
            if (string.IsNullOrEmpty(serial))
            {
                return null;
            }

            var request = new HudStoreDownloadHudRequest
            {
                Serial = serial,
                LayoutId = layoutId
            };

            var downloadStream = service.DownloadHud(request);

            return downloadStream;
        }

        /// <summary>
        /// Search data
        /// </summary>
        public override void Search(string searchText)
        {
            this.searchText = searchText;
            Refresh(0, 4);
        }

        #endregion
    }
}