//-----------------------------------------------------------------------
// <copyright file="IBetOnlineTournamentManager.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.BetOnline
{
    internal class BetOnlineTournamentManager : BetOnlineBaseDataManager, IBetOnlineTournamentManager
    {
        private IPokerClientEncryptedLogger logger;
        private bool isLoggingEnabled;

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);

            logger = dataManagerInfo.Logger;

            isLoggingEnabled = false;
        }

        /// <summary>
        /// Process stream data 
        /// </summary>
        /// <param name="data">Stream data</param>
        /// <returns>Result of processing</returns>
        public void ProcessData(byte[] data)
        {
            try
            {
                var encryptedXml = Encoding.UTF8.GetString(data).Replace("\0", string.Empty);
       
                // convert xml to iPoker xml in separate thread, then save it in hh file                
                Task.Run(() =>
                {
                    var xml = Decrypt(encryptedXml);

                    if (logger != null && isLoggingEnabled)
                    {
                        logger.Log(xml);
                    }

                    var tournamentCacheService = ServiceLocator.Current.GetInstance<ITournamentsCacheService>();
                    tournamentCacheService.Update(xml);              
                });
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Stream (info) data has wrong format", ex);
            }
        }

        #region IDisposable implementation

        public void Dispose()
        {
        }

        #endregion
    }
}