//-----------------------------------------------------------------------
// <copyright file="ShopModel.cs" company="Ace Poker Solutions">
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

namespace Model.Shop
{
    internal class ProductAppStoreModel : BindableBase, IProductAppStoreModel
    {
        private IProductAppStoreRepository repository;

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
                if (ReferenceEquals(items, value))
                {
                    return;
                }

                items = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Model methods

        public void Refresh(int start, int amount)
        {
            Items = repository.GetAllProducts().ToList();
        }

        /// <summary>
        /// Search data
        /// </summary>
        public void Search(string searchText)
        {

        }

        #endregion
    }
}