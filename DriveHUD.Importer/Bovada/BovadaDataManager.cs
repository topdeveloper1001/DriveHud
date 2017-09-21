//-----------------------------------------------------------------------
// <copyright file="BovadaDataManager.cs" company="Ace Poker Solutions">
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
using Model.Settings;
using Newtonsoft.Json;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DriveHUD.Importers.Bovada
{
    /// <summary>
    /// Bovada data manager
    /// </summary>
    internal class BovadaDataManager : BaseDataManager, IBovadaDataManager
    {
        protected IEventAggregator eventAggregator;

        protected Dictionary<uint, IPokerTable> openedTables = new Dictionary<uint, IPokerTable>();

        private IPokerClientEncryptedLogger logger;

        protected string site;

        private SubscriptionToken pokerClientTableClosedSubsciption;

        private bool isLoggingEnabled;

        public BovadaDataManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);

            logger = dataManagerInfo.Logger;
            site = dataManagerInfo.Site;

            pokerClientTableClosedSubsciption = eventAggregator.GetEvent<PokerClientTableClosedEvent>().Subscribe(RemoveOpenedTable);

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.GetSettings();

            isLoggingEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
        }

        /// <summary>
        /// Process stream data 
        /// </summary>
        /// <param name="data">Stream data</param>
        /// <returns>Result of processing</returns>
        public virtual void ProcessData(byte[] data)
        {
            try
            {
                var catcherDataObject = CreateDataObject(data);

                // skip if UID is empty
                if (catcherDataObject == null || string.IsNullOrEmpty(catcherDataObject.uid))
                {
                    return;
                }

                var tableUid = BovadaConverters.ConvertHexStringToUint32(catcherDataObject.uid);
                TryMergeTables(tableUid, catcherDataObject.handle);

                if (!openedTables.ContainsKey(tableUid))
                {
                    AddTable(tableUid);
                }

                var catcherTable = openedTables[tableUid];

                catcherTable.ProcessCommand(catcherDataObject);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Stream data was in wrong format", ex);
            }
        }

        /// <summary>
        /// Merge tables with same handle
        /// </summary>
        /// <param name="tableUid">Key to merge into</param>
        /// <param name="handleText"></param>
        private void TryMergeTables(uint tableUid, string handleText)
        {
            // it is possible to have more than 1 table with same handle, because of disconnection
            if (string.IsNullOrEmpty(handleText) || handleText.Equals("0xFFFFFFFF"))
            {
                return;
            }

            var handle = BovadaConverters.ConvertHexStringToInt32(handleText);

            var tablesToMerge = openedTables.Where(x => x.Value.WindowHandle.ToInt32() == handle && x.Key != tableUid);

            if (tablesToMerge.Any())
            {
                var mergeTable = tablesToMerge.Last();
                openedTables.Remove(mergeTable.Key);
                openedTables[tableUid] = mergeTable.Value;
            }
        }

        /// <summary>
        /// Remove table from tables dictionary
        /// </summary>      
        public virtual void RemoveOpenedTable(PokerClientTableClosedEventArgs e)
        {
            if (openedTables != null)
            {
                // it is possible to have more than 1 table here, because of disconnection
                var tablesToRemove = openedTables.Values.Where(x => x.WindowHandle == e.TableHandle).ToArray();

                foreach (var tableToRemove in tablesToRemove)
                {
                    if (!openedTables.ContainsKey(tableToRemove.Uid))
                    {
                        continue;
                    }

                    openedTables.Remove(tableToRemove.Uid);
                }
            }
        }

        #region Infrastructure     

        /// <summary>
        /// Converts raw data to string
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>The result of conversion</returns>
        protected virtual string ConvertDataToString(byte[] data)
        {
            var dataText = Encoding.ASCII.GetString(data).Replace("\0", string.Empty).Replace("\\", string.Empty);
            return dataText;
        }

        /// <summary>
        /// Create data object from stream data
        /// </summary>
        /// <param name="data">Stream data</param>
        /// <returns>Stream data object</returns>
        protected virtual BovadaCatcherDataObject CreateDataObject(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            var dataText = ConvertDataToString(data);

            // Log stream data
            if (logger != null && isLoggingEnabled)
            {
                logger.Log(dataText);
            }

            // skip not JSON data 
            if (!dataText.Contains("{") || !dataText.Contains("}") || dataText.Contains("|"))
            {
                return null;
            }

            // deserialize stream data
            var catcherDataObject = JsonConvert.DeserializeObject<BovadaCatcherDataObject>(dataText);

            return catcherDataObject;
        }

        /// <summary>
        /// Adds new table to tables dictionary
        /// </summary>
        /// <param name="tableUid">Unique identifier of table</param>
        protected virtual void AddTable(uint tableUid)
        {
            if (openedTables != null && !openedTables.ContainsKey(tableUid))
            {
                var catcherTable = new BovadaTable(eventAggregator);
                openedTables.Add(tableUid, catcherTable);
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            if (pokerClientTableClosedSubsciption != null)
            {
                eventAggregator.GetEvent<PokerClientTableClosedEvent>().Unsubscribe(pokerClientTableClosedSubsciption);
            }
        }

        #endregion
    }
}