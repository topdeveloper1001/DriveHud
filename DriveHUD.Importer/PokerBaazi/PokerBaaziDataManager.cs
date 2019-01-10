//-----------------------------------------------------------------------
// <copyright file="PokerBaaziDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Text;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziDataManager : BaseDataManager, IPokerBaaziDataManager
    {
        private IPokerClientEncryptedLogger logger;

        protected string site;

        private readonly IPokerBaaziImporter importer;

        public PokerBaaziDataManager()
        {
            var importerService = ServiceLocator.Current.GetInstance<IImporterService>();
            importer = importerService.GetImporter<IPokerBaaziImporter>();
        }

        public virtual void Dispose()
        {
        }

        public virtual void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            logger = dataManagerInfo.Logger;
            site = dataManagerInfo.Site;
        }

        /// <summary>
        /// Processes raw data recieved from pipe and put it into the buffer
        /// </summary>
        /// <param name="data">Data to process</param>
        public virtual void ProcessData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            var dataText = ConvertDataToString(data);

            // skip all short messages
            if (string.IsNullOrEmpty(dataText) || dataText.Length < 20)
            {
                return;
            }

            // Log stream data
            logger?.Log(dataText);

            // send data to importer 
            importer.AddPackage(dataText);
        }

        /// <summary>
        /// Converts raw data to string
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>The result of conversion</returns>
        protected virtual string ConvertDataToString(byte[] data)
        {
            try
            {
                var encryptedData = Encoding.UTF8.GetString(data).Replace("\0", string.Empty);

                //var dataText = Decrypt(encryptedData);

                return encryptedData;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to convert raw data to string. [{site}]", e);
                return null;
            }
        }
    }
}