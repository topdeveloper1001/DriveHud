//-----------------------------------------------------------------------
// <copyright file="ShopRepository.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Shop
{
    internal class ProductAppStoreRepository : IProductAppStoreRepository
    {
        public IEnumerable<AppStoreProduct> GetAllProducts()
        {          
            var shopProducts = Enumerable.Range(0, 6).Select((index, x) => new AppStoreProduct
            {
                ProductName = $"Upswing Poker Training #{index}",
                ProductDescription = "Poker strategies and courses, brought to you by two of the world's best poker players (Doug Polk & Ryan Fee), that will take your own poker skills to the next level.",
                Price = "$27/mo",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/upswing-poker-lab.gif",
                CartLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                LearnMoreLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                IsAnimatedGif = true
            }).ToList();

            return shopProducts;
        }

        /// <summary>
        /// Currently we're using hard-coded values, in future here must be used some external storage
        /// </summary>     
        public IEnumerable<AppStoreProduct> GetProducts(int start, int amount)
        {          
            var shopProducts = Enumerable.Range(0, 1).Select((index, x) => new AppStoreProduct
            {
                ProductName = "Upswing Poker Training",
                ProductDescription = "Poker strategies and courses, brought to you by two of the world's best poker players (Doug Polk & Ryan Fee), that will take your own poker skills to the next level.",
                Price = "$27/mo",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/upswing-poker-lab.gif",
                CartLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                LearnMoreLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                IsAnimatedGif = true
            }).ToList();

            return shopProducts;
        }
    }
}