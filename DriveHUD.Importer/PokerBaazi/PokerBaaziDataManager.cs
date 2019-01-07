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

using DriveHUD.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerBaazi
{
    internal class PokerBaaziDataManager : BaseDataManager, IPokerBaaziDataManager
    {
        private IPokerClientEncryptedLogger logger;

        protected string site;

        public void Dispose()
        {
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            logger = dataManagerInfo.Logger;
            site = dataManagerInfo.Site;
        }

        public void ProcessData(byte[] data)
        {
            if (data == null)
            {
                return;
            }

            var dataText = ConvertDataToString(data);

            // Log stream data
            if (logger != null)
            {
                logger.Log(dataText);
            }

            Console.WriteLine(dataText);
        }

        /// <summary>
        /// Converts raw data to string
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>The result of conversion</returns>
        protected string ConvertDataToString(byte[] data)
        {
            var encryptedData = Encoding.UTF8.GetString(data).Replace("\0", string.Empty);

            //var dataText = Decrypt(encryptedData);

            return encryptedData;
        }
    }
}