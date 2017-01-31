//-----------------------------------------------------------------------
// <copyright file="ProductAppStoreRepository.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Model.AppStore
{
    internal class ProductAppStoreRepository : IProductAppStoreRepository
    {
        private static int WebRequestTimeout = 10000;

        // paths
        // local repo - %appdata%\DriveHUD\AppStore\products.xml
        // local temp repo - %appdata%\DriveHUD\AppStore\products.tmp
        // remove repo http:\\www.drivehud.com\appstore\products.xml        

        public IEnumerable<AppStoreProduct> GetAllProducts()
        {
            UpdateLocalRepository();

            var shopProducts = Enumerable.Range(0, 50).Select((index, x) => new AppStoreProduct
            {
                ProductName = $"Upswing Poker Training #{index}",
                ProductDescription = "Poker strategies and courses, brought to you by two of the world's best poker players (Doug Polk & Ryan Fee), that will take your own poker skills to the next level.",
                Price = "$27/mo",
                ImageLink = "pack://application:,,,/DriveHUD.Common.Resources;component/images/Shop/upswing-poker-lab.gif",
                CartLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                LearnMoreLink = "https://hf322.isrefer.com/go/tpl/drivehud/",
                IsAnimatedGif = false
            }).ToList();

            return shopProducts;
        }

        private void UpdateLocalRepository()
        {
            var localRepositoryHash = GetLocalRepositoryHash();
            var remoteRepositoryHash = GetRemoteRepositoryHash();

            // update is not required
            if (localRepositoryHash.Equals(remoteRepositoryHash, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            DownloadRemoteProductStorage();
        }

        private string GetLocalRepositoryHash()
        {
            try
            {
                var localRepositoryHash = Utils.GetMD5HashFromFile("path to local repo");
                return localRepositoryHash;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Local hash of product app store has not been obtained.", e);
            }

            return null;
        }

        private string GetRemoteRepositoryHash()
        {
            try
            {
                using (var webClient = new WebClientWithTimeout(WebRequestTimeout))
                {
                    var remoteRepositoryHash = webClient.DownloadString("some address");
                    return remoteRepositoryHash;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Remote hash of product app store has not been obtained.", e);
            }

            return null;
        }

        private void DownloadRemoteProductStorage()
        {
            try
            {
                using (var webClient = new WebClientWithTimeout(WebRequestTimeout))
                {
                    webClient.DownloadFile("some address", "path to file");
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Remote storage of product app store has not been downloaded.", e);
            }
        }
    }
}