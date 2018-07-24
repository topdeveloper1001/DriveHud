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
using System.Linq;

namespace Model.AppStore
{
    internal class ProductAppStoreModel : BaseAppStoreModel<AppStoreProduct>, IProductAppStoreModel
    {
        protected IProductAppStoreRepository repository;

        public ProductAppStoreModel()
        {
            repository = ServiceLocator.Current.GetInstance<IProductAppStoreRepository>();
        }

        #region Model methods

        public override void Load(object loadInfo)
        {
            allItems = repository.GetAllProducts().ToList();
            activeItems = allItems;
        }
     
        /// <summary>
        /// Search data
        /// </summary>
        public override void Search(string searchText)
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