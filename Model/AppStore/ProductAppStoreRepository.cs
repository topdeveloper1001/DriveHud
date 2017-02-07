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
using System.IO;
using System.Xml.Serialization;

namespace Model.AppStore
{
    internal class ProductAppStoreRepository : IProductAppStoreRepository
    {
        private static int WebRequestTimeout = 10000;

        private readonly string storeDataFolder;
        private readonly string storeLocalProductRepo;
        private readonly string storeLocalTempProductRepo;
        private readonly string storeRemoteProductRepo;
        private readonly string storeRemoteProductHash;

        public ProductAppStoreRepository()
        {
            storeDataFolder = StringFormatter.GetAppStoreDataFolder();
            storeLocalProductRepo = StringFormatter.GetAppStoreLocalProductRepo();
            storeLocalTempProductRepo = StringFormatter.GetAppStoreLocalTempProductRepo();
            storeRemoteProductRepo = StringFormatter.GetAppStoreRemoteProductRepo();
            storeRemoteProductHash = StringFormatter.GetAppStoreRemoteProductHash();
        }

        public IEnumerable<AppStoreProduct> GetAllProducts()
        {
            UpdateLocalDataStorage();

            var products = GetAppStoreProducts();

            return products;        
        }

        private void UpdateLocalDataStorage()
        {
            try
            {
                var appStoreStorage = new DirectoryInfo(storeDataFolder);

                if (!appStoreStorage.Exists)
                {
                    appStoreStorage.Create();
                }

                var localRepositoryHash = GetLocalRepositoryHash(storeLocalProductRepo);
                var remoteRepositoryHash = GetRemoteRepositoryHash();

                // update is not required or is not possible
                if (string.IsNullOrWhiteSpace(remoteRepositoryHash) ||
                        ((localRepositoryHash != null) && localRepositoryHash.Equals(remoteRepositoryHash, StringComparison.OrdinalIgnoreCase)))
                {
                    return;
                }

                DownloadAndUpdateProductStorage(remoteRepositoryHash);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Local product repository has not been updated.", e);
            }
        }

        private string GetLocalRepositoryHash(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !File.Exists(fileName))
            {
                return null;
            }

            try
            {
                var localRepositoryHash = Utils.GetMD5HashFromFile(fileName);
                return localRepositoryHash;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Local hash of product app store {fileName} has not been obtained.", e);
            }

            return null;
        }

        private string GetRemoteRepositoryHash()
        {
            try
            {
                using (var webClient = new WebClientWithTimeout(WebRequestTimeout))
                {
                    var remoteRepositoryHash = webClient.DownloadString(storeRemoteProductHash);
                    return remoteRepositoryHash;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Remote hash of product app store has not been obtained.", e);
            }

            return null;
        }

        private void RemoveLocalTempRepository()
        {
            try
            {
                if (File.Exists(storeLocalTempProductRepo))
                {
                    File.Delete(storeLocalTempProductRepo);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Local temp product app store '{storeLocalTempProductRepo}' has not been removed.", e);
            }
        }

        private void DownloadAndUpdateProductStorage(string remoteRepositoryHash)
        {
            RemoveLocalTempRepository();
            DownloadRemoteProductStorage();

            var localRepositoryHash = GetLocalRepositoryHash(storeLocalTempProductRepo);

            if (localRepositoryHash != null && !localRepositoryHash.Equals(remoteRepositoryHash, StringComparison.OrdinalIgnoreCase))
            {
                LogProvider.Log.Error("Product app store has not been downloaded correctly.");
                RemoveLocalTempRepository();
                return;
            }

            MoveTempRepoToLocaRepo();
            RemoveLocalTempRepository();
        }

        private void MoveTempRepoToLocaRepo()
        {
            try
            {
                if (File.Exists(storeLocalProductRepo))
                {
                    File.Delete(storeLocalProductRepo);
                }

                File.Move(storeLocalTempProductRepo, storeLocalProductRepo);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Temp repo of product app store '{storeLocalTempProductRepo}' has not been moved to '{storeLocalProductRepo}'.", e);
            }
        }

        private void DownloadRemoteProductStorage()
        {
            try
            {
                using (var webClient = new WebClientWithTimeout(WebRequestTimeout))
                {
                    webClient.DownloadFile(storeRemoteProductRepo, storeLocalTempProductRepo);
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Remote storage of product app store has not been downloaded.", e);
            }
        }

        private IEnumerable<AppStoreProduct> GetAppStoreProducts()
        {
            try
            {
                using (var stream = File.Open(storeLocalProductRepo, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var xmlSerializer = new XmlSerializer(typeof(AppStoreProductStorage));
                    var appStoreProductStorage = xmlSerializer.Deserialize(stream) as AppStoreProductStorage;

                    if (appStoreProductStorage != null && appStoreProductStorage.Products != null)
                    {
                        return appStoreProductStorage.Products;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Local products app store has not been deserialized.", e);
            }

            return new List<AppStoreProduct>();
        }
    }
}