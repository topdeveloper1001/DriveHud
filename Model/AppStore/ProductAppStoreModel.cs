//-----------------------------------------------------------------------
// <copyright file="ProductAppStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Linq;

namespace Model.AppStore
{
    internal class ProductAppStoreModel : BindableBase, IProductAppStoreModel
    {
        protected IProductAppStoreRepository repository;

        private IList<AppStoreProduct> allItems = new List<AppStoreProduct>();

        private IList<AppStoreProduct> activeItems = new List<AppStoreProduct>();

        public ProductAppStoreModel()
        {
            repository = ServiceLocator.Current.GetInstance<IProductAppStoreRepository>();
        }

        #region Properties

        private IList<AppStoreProduct> items = new List<AppStoreProduct>();

        public IList<AppStoreProduct> Items
        {
            get
            {
                return items;
            }
            private set
            {
                SetProperty(ref items, value);
            }
        }

        public int ItemsCount
        {
            get
            {
                return activeItems != null ? activeItems.Count : default(int);
            }
        }

        #endregion

        #region Model methods

        public void Load()
        {
            allItems = repository.GetAllProducts().ToList();
            activeItems = allItems;
        }

        public void Refresh(int start, int amount)
        {
            if (activeItems == null)
            {
                return;
            }

            Items = activeItems.Skip(start).Take(amount).ToList();
        }

        /// <summary>
        /// Search data
        /// </summary>
        public void Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                activeItems = allItems;
            }

            activeItems = allItems.Where(x => x.ProductName.Contains(searchText) || x.ProductDescription.Contains(searchText)).ToList();
        }

        #endregion
    }
}