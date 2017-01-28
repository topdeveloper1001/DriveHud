//-----------------------------------------------------------------------
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class ProductAppStoreViewModel : AppStoreViewModel<IProductAppStoreModel>
    {
        public override void Initialize()
        {
            InitializeModelAsync(() => Model.Refresh(0, 0));
        }

        protected override void ModelInitialized()
        {
        }

        protected override void OnAddToCart(object item)
        {
            var product = item as AppStoreProduct;

            if(product != null)
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