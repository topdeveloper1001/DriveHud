﻿//-----------------------------------------------------------------------
// <copyright file="IgnitionInfoDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using DriveHUD.Entities;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace DriveHUD.Importers.Bovada
{
    internal class IgnitionInfoDataManager : BovadaDataManager, IIgnitionInfoDataManager
    {
        private ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

        private const string cashUrlPattern = @"/pokeragp/cash/play?authToken";
        private const string tournUrlPattern = @"/services/api/poker-lobby-api/tournaments";
        private const string tournEndUrlPattern = @"info";

        public Dictionary<uint, IgnitionTableData> tableData = new Dictionary<uint, IgnitionTableData>();

        public IgnitionInfoDataManager(IEventAggregator eventAggregator)
            : base(eventAggregator)
        {
        }

        #region Infrastructure            

        public IgnitionTableData GetTableData(uint tableId)
        {
            rwLock.EnterReadLock();

            try
            {
                if (tableData.ContainsKey(tableId))
                {
                    return tableData[tableId];
                }

                return null;
            }
            finally
            {
                rwLock.ExitReadLock();
            }
        }

        public override void ProcessData(byte[] data)
        {
            try
            {
                var dataText = ConvertDataToString(data);
                ParseCatcherInfoData(dataText);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        private void ParseCatcherInfoData(string dataText)
        {
            var url = dataText.Split(new[] { '\r', '\n' }).FirstOrDefault().Trim();

            if (url.Contains(cashUrlPattern))
            {
                ProcessCashData(url, dataText);
                return;
            }

            if (url.Contains(tournUrlPattern) && url.EndsWith(tournEndUrlPattern))
            {
                ProcessTournamentData(url, dataText);
            }
        }

        private void ProcessCashData(string url, string data)
        {
            try
            {
                var uri = new Uri(url);
                var parsedQueryString = HttpUtility.ParseQueryString(uri.Query);
                var tableSeatsText = parsedQueryString.Get("tableSeats");
                var limitText = parsedQueryString.Get("limitType");
                var gameTypeText = parsedQueryString.Get("gameType");

                if (string.IsNullOrWhiteSpace(data))
                {
                    LogProvider.Log.Warn("CashData was empty.");
                    return;
                }

                var tableText = ReadJsonData(data, "table");

                uint tableId;

                if (!uint.TryParse(tableText, out tableId))
                {
                    LogProvider.Log.Error(this, $"Table id couldn't be parsed from: {tableText}");
                    return;
                }

                int tableSize;

                if (!int.TryParse(tableSeatsText, out tableSize))
                {
                    LogProvider.Log.Error(this, $"Table size couldn't be parsed from: {tableSeatsText}.");
                    return;
                }

                rwLock.EnterUpgradeableReadLock();

                try
                {
                    if (tableData.ContainsKey(tableId))
                    {
                        return;
                    }

                    rwLock.EnterWriteLock();

                    try
                    {
                        var table = new IgnitionTableData
                        {
                            Id = tableId,
                            TableSize = tableSize,
                            GameFormat = GameFormat.Cash,
                            GameLimit = BovadaConverters.ConvertGameLimit(limitText),
                            GameType = BovadaConverters.ConvertGameType(gameTypeText)
                        };

                        tableData.Add(tableId, table);
                        LogProvider.Log.Info($"Table info has been read from client: {table} [{site}]");
                    }
                    finally
                    {
                        rwLock.ExitWriteLock();
                    }
                }
                finally
                {
                    rwLock.ExitUpgradeableReadLock();
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        private void ProcessTournamentData(string url, string data)
        {
            try
            {
                var tournLastIndex = url.LastIndexOf("/");
                var tournFistIndex = url.LastIndexOf("/", tournLastIndex - 1) + 1;
                var tournamentText = url.Substring(tournFistIndex, tournLastIndex - tournFistIndex);

                uint tounamentId;

                if (!uint.TryParse(tournamentText, out tounamentId))
                {
                    LogProvider.Log.Error(this, $"Tournament id couldn't be parsed from: {tournamentText}");
                    return;
                }

                var seatText = ReadJsonData(data, "\"seats\"");
                var maxPlayersText = ReadJsonData(data, "maxPlayers");
                var limitText = ReadJsonTextData(data, "limit");
                var gameTypeText = ReadJsonTextData(data, "gameType");
                var tournamentName = ReadJsonTextData(data, "tournamentName");

                if (string.IsNullOrEmpty(seatText) && string.IsNullOrEmpty(maxPlayersText))
                {
                    LogProvider.Log.Warn(this, $"Seat and max players data hasn't been found.");
                    return;
                }

                int seat;

                if (!int.TryParse(seatText, out seat))
                {
                    LogProvider.Log.Error(this, $"Seat couldn't be parsed from: {seatText}");
                    return;
                }

                int maxPlayers;

                if (!int.TryParse(maxPlayersText, out maxPlayers))
                {
                    LogProvider.Log.Error(this, $"Max players couldn't be parsed from: {maxPlayersText}");
                }

                var gameLimit = BovadaConverters.ConvertGameLimit(limitText);

                var table = new IgnitionTableData
                {
                    Id = tounamentId,
                    TableSize = seat,
                    GameFormat = maxPlayers > seat ? GameFormat.MTT : GameFormat.SnG,
                    GameLimit = gameLimit,
                    GameType = BovadaConverters.ConvertGameType(gameTypeText),
                    TableName = tournamentName
                };

                rwLock.EnterUpgradeableReadLock();

                try
                {
                    if (tableData.ContainsKey(tounamentId))
                    {
                        return;
                    }

                    rwLock.EnterWriteLock();

                    try
                    {
                        tableData.Add(tounamentId, table);
                        LogProvider.Log.Info($"Tournament table info has been read from client: {table} [{site}]");
                    }
                    finally
                    {
                        rwLock.ExitWriteLock();
                    }
                }
                finally
                {
                    rwLock.ExitUpgradeableReadLock();
                }

            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        protected override BovadaCatcherDataObject CreateDataObject(byte[] data)
        {
            return null;
        }

        protected override string ConvertDataToString(byte[] data)
        {
            var encryptedData = Encoding.UTF8.GetString(data).Replace("\0", string.Empty).Replace("\\", string.Empty);

            var dataText = Decrypt(encryptedData);

            return dataText;
        }

        protected override void AddTable(uint tableUid)
        {
        }

        public override void RemoveOpenedTable(PokerClientTableClosedEventArgs e)
        {
        }

        private string ReadJsonData(string data, string name)
        {
            var nameIndex = data.IndexOf(name, StringComparison.OrdinalIgnoreCase) + 1;

            if (nameIndex < 0)
            {
                return string.Empty;
            }

            var startIndex = data.IndexOf(":", nameIndex) + 1;
            var endIndex = data.IndexOf(",", startIndex, StringComparison.OrdinalIgnoreCase);

            string value = string.Empty;

            if (endIndex > startIndex)
            {
                value = data.Substring(startIndex, endIndex - startIndex);
            }

            return value;
        }

        private string ReadJsonTextData(string data, string name)
        {
            var nameIndex = data.IndexOf(name, StringComparison.OrdinalIgnoreCase) + 1;

            if (nameIndex < 0)
            {
                return string.Empty;
            }

            var startIndex = data.IndexOf(":", nameIndex) + 2;
            var endIndex = data.IndexOf("\",", startIndex, StringComparison.OrdinalIgnoreCase);

            string value = string.Empty;

            if (endIndex > startIndex)
            {
                value = data.Substring(startIndex, endIndex - startIndex);
            }

            return value;
        }

        #endregion      
    }
}