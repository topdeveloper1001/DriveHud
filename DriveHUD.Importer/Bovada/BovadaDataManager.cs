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

        protected bool isLoggingEnabled;

        public BovadaDataManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);

            logger = dataManagerInfo.Logger;
            site = dataManagerInfo.Site;

            pokerClientTableClosedSubsciption = eventAggregator.GetEvent<PokerClientTableClosedEvent>().Subscribe(RemoveOpenedTable, ThreadOption.BackgroundThread);

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

            lock (openedTables)
            {
                var tablesToMerge = openedTables.Where(x => x.Value.WindowHandle.ToInt32() == handle && x.Key != tableUid);

                if (tablesToMerge.Any())
                {
                    var mergeTable = tablesToMerge.Last();
                    openedTables.Remove(mergeTable.Key);
                    openedTables[tableUid] = mergeTable.Value;
                }
            }
        }

        /// <summary>
        /// Remove table from tables dictionary
        /// </summary>      
        public virtual void RemoveOpenedTable(PokerClientTableClosedEventArgs e)
        {
            if (openedTables != null)
            {
                lock (openedTables)
                {
                    // it is possible to have more than 1 table here, because of disconnection
                    var tablesToRemove = openedTables.Values.Where(x => x.WindowHandle == e.WindowHandle).ToArray();

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
        }

        #region Infrastructure     

        private Dictionary<string, List<string>> buffer = new Dictionary<string, List<string>>();

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

            if (dataText.StartsWith("error", StringComparison.OrdinalIgnoreCase))
            {
                LogProvider.Log.Error(this, $"Catcher data has errors: {dataText}");
                return null;
            }

            try
            {
                var splitData = dataText.Split('|');

                if (splitData.Length != 3 && splitData.Length != 2)
                {
                    return null;
                }

                var uid = splitData[0];
                var body = splitData.Length == 3 ? splitData[2] : splitData[1];

                if (!buffer.TryGetValue(uid, out List<string> socketBuffer))
                {
                    socketBuffer = new List<string>();
                    buffer.Add(uid, socketBuffer);
                }

                // not full command
                if (!body.Trim().EndsWith("}", StringComparison.OrdinalIgnoreCase))
                {
                    socketBuffer.Add(body);
                    return null;
                }

                // check if there is data in buffer         
                if (socketBuffer.Count > 0 && !body.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                {
                    socketBuffer.Add(body);
                    body = string.Join(string.Empty, socketBuffer);
                    socketBuffer.Clear();
                }
                else if (body.StartsWith("{", StringComparison.OrdinalIgnoreCase))
                {
                    socketBuffer.Clear();
                }
                else
                {
                    return null;
                }

                try
                {
                    // deserialize stream data
                    var catcherDataObject = new BovadaCatcherDataObject
                    {
                        uid = uid,
                        cmd = JsonConvert.DeserializeObject<BovadaCommandDataObject>(body)
                    };

                    return catcherDataObject;
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Failed to parse body: \r\n{body}", e);
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Failed to process stream data: \r\n{dataText}", ex);
                return null;
            }
        }

        /// <summary>
        /// Adds new table to tables dictionary
        /// </summary>
        /// <param name="tableUid">Unique identifier of table</param>
        protected virtual void AddTable(uint tableUid)
        {
            lock (openedTables)
            {
                if (openedTables != null && !openedTables.ContainsKey(tableUid))
                {
                    var catcherTable = new BovadaTable(eventAggregator);
                    openedTables.Add(tableUid, catcherTable);
                }
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