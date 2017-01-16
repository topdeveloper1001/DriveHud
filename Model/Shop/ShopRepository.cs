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

using System.Collections.Generic;
using System.Linq;

namespace Model.Shop
{
    internal class ShopRepository : IShopRepository
    {
        /// <summary>
        /// Currently we're using hard-coded values, in future here must be used some external storage
        /// </summary>     
        public IEnumerable<ShopProduct> GetProducts(ShopType shopType, int start, int amount)
        {
            if (shopType != ShopType.Recommended)
            {
                return new List<ShopProduct>();
            }

            var shopProducts = Enumerable.Range(0, 1).Select((index, x) => new ShopProduct
            {
                ProductName = "Upswing Poker Training",
                ProductDescription = "Poker strategies and courses, brought to you by two of the world's best poker players (Doug Polk & Ryan Fee), that will take your own poker skills to the next level.",
                Price = "$24.92/mo",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/upswing-poker-lab.gif",
                CartLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                LearnMoreLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                IsAnimatedGif = true
            }).ToList();

            return shopProducts;
        }
    }
}