//-----------------------------------------------------------------------
// <copyright file="IgnitionDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Events;
using System.Text;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionDataManager : BovadaDataManager, IIgnitionDataManager
    {
        public IgnitionDataManager(IEventAggregator eventAggregator) : base(eventAggregator)
        {
        }

        /// <summary>
        /// Converts raw data to string
        /// </summary>
        /// <param name="data">Data to convert</param>
        /// <returns>The result of conversion</returns>
        protected override string ConvertDataToString(byte[] data)
        {
            var encryptedData = Encoding.UTF8.GetString(data).Replace("\0", string.Empty).Replace("\\", string.Empty);

            var dataText = Decrypt(encryptedData);

            return dataText;
        }

        /// <summary>
        /// Adds new table to tables dictionary
        /// </summary>
        /// <param name="tableUid">Unique identifier of table</param>
        protected override void AddTable(uint tableUid)
        {
            if (openedTables != null && !openedTables.ContainsKey(tableUid))
            {
                var catcherTable = new IgnitionTable(eventAggregator);
                openedTables.Add(tableUid, catcherTable);
            }
        }
    }
}