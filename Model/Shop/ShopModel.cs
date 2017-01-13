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
    public class ShopModel : BindableBase, IShopModel
    {
        private IShopRepository shopRepository;

        public ShopModel()
        {
            shopRepository = ServiceLocator.Current.GetInstance<IShopRepository>();
        }

        #region Properties

        private List<ShopProduct> shopProducts = new List<ShopProduct>();

        public List<ShopProduct> ShopProducts
        {
            get
            {
                return shopProducts;
            }
            private set
            {
                if (ReferenceEquals(shopProducts, value))
                {
                    return;
                }

                shopProducts = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Model methods

        public void Refresh(int start, int amount)
        {
            ShopProducts = shopRepository.GetProducts(start, amount).ToList();
        }

        #endregion
    }
}