﻿//-----------------------------------------------------------------------
// <copyright file="ProductAppStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.Shop;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class ProductAppStoreViewModel : AppStoreViewModel<IProductAppStoreModel>
    {
        private static int ProductsPerPage = 6;

        public override void Initialize()
        {
            base.Initialize();

            GridColumns = 2;
            GridRows = 3;

            InitializeModelAsync(() => Model.Load());
        }

        protected override void ModelInitialized()
        {
        }

        protected override void OnAddToCart(object item)
        {
            var product = item as AppStoreProduct;

            if (product != null)
            {
                OpenLink(product.CartLink);
            }
        }

        protected override void OnLearnMore(object item)
        {
            var product = item as AppStoreProduct;

            if (product != null)
            {
                OpenLink(product.LearnMoreLink);
            }
        }
    }
}