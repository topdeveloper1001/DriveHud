﻿//-----------------------------------------------------------------------
// <copyright file="Migration0029_FillHandPlayersTable.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.MigrationService.Migrators;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using FluentMigrator;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using NHibernate.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrations
{
    [Migration(29)]
    public class Migration0029_FillHandPlayersTable : Migration
    {
        private const string HandsPlayersTable = "HandsPlayers";

        private readonly static object locker = new object();

        private readonly IPlayerStatisticRepository playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();

        private const int fastModeAllowedHands = 1500000;

        public override void Down()
        {
            try
            {
                MigrationUtils.TruncateTable(HandsPlayersTable);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Rollback of Migration #29 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Rollback of Migration #29 executed.");
        }

        public override void Up()
        {
            LogProvider.Log.Info("Preparing migration #29.");

            try
            {
                MigrationUtils.TruncateTable(HandsPlayersTable);
                FillHandsPlayersTable();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Migration #29 failed.", e);
                throw;
            }

            LogProvider.Log.Info("Migration #29 executed.");
        }

        private void FillHandsPlayersTable()
        {
            MigrationUtils.SetStatusMessage("Preparing data for migration");

            using (var session = ModelEntities.OpenStatelessSession())
            {
                LogProvider.Log.Info("Preparing data to insert.");

                var playersId = session.Query<Players>()
                    .Select(x => x.PlayerId).ToArray();

                var handsQuery = session.Query<Handhistory>()
                    .Select(x => new { x.HandhistoryId, x.Gamenumber, x.PokersiteId });

                var hands = new Dictionary<HandHistoryKey, int>();

                foreach (var hand in handsQuery)
                {
                    var handKey = new HandHistoryKey(hand.Gamenumber, hand.PokersiteId);

                    if (!hands.ContainsKey(handKey))
                    {
                        hands.Add(handKey, hand.HandhistoryId);
                    }
                }

                var statisticFiles = new HashSet<string>();

                foreach (var playerId in playersId)
                {
                    var playerStatsFiles = playerStatisticRepository.GetPlayerFiles(playerId);
                    statisticFiles.AddRange(playerStatsFiles);
                }

                var conn = session.Connection as SQLiteConnection;

                if (fastModeAllowedHands >= hands.Count)
                {
                    InsertInFastMode(statisticFiles, hands, conn);
                    return;
                }

                InsertInLowMemoryMode(statisticFiles, hands, conn);
            }
        }

        private void InsertInLowMemoryMode(HashSet<string> statisticFiles, Dictionary<HandHistoryKey, int> hands, SQLiteConnection conn)
        {
            LogProvider.Log.Info("Data will be proccessed in low memory mode.");

            var insertedRows = 0;

            LogProvider.Log.Info($"Begin data processing: {statisticFiles.Count} files for processing.");

            using (var transaction = conn.BeginTransaction())
            {
                using (var cmd = new SQLiteCommand("insert into HandsPlayers (HandId, PlayerId, NetWon) values (@HandId, @PlayerId, @NetWon)", conn))
                {
                    var counter = 0;

                    Parallel.ForEach(statisticFiles, file =>
                    {
                        playerStatisticRepository.GetPlayerStatisticFromFile(file).Where(stat => !stat.IsTourney).ForEach(stat =>
                       {
                           var handKey = new HandHistoryKey(stat.GameNumber, (short)stat.PokersiteId);

                           if (hands.ContainsKey(handKey))
                           {
                               lock (locker)
                               {
                                   var handPlayer = new HandPlayer
                                   {
                                       PlayerId = stat.PlayerId,
                                       HandId = hands[handKey],
                                       NetWon = Utils.ConvertToCents(stat.NetWon)
                                   };

                                   cmd.Parameters.Add("@HandId", DbType.Int32);
                                   cmd.Parameters.Add("@PlayerId", DbType.Int32);
                                   cmd.Parameters.Add("@NetWon", DbType.Int64);

                                   cmd.Prepare();

                                   if (counter % 25 == 0)
                                   {
                                       MigrationUtils.SetStatusMessage($"Processing statistic {counter}/{statisticFiles.Count}");
                                   }

                                   cmd.Parameters[0].Value = handPlayer.HandId;
                                   cmd.Parameters[1].Value = handPlayer.PlayerId;
                                   cmd.Parameters[2].Value = handPlayer.NetWon;
                                   cmd.ExecuteNonQuery();

                                   insertedRows++;
                               }
                           }
                       });

                        Interlocked.Increment(ref counter);
                    });
                }

                LogProvider.Log.Info($"Inserted data: {insertedRows} rows");

                transaction.Commit();
            }
        }

        private void InsertInFastMode(HashSet<string> statisticFiles, Dictionary<HandHistoryKey, int> hands, SQLiteConnection conn)
        {
            LogProvider.Log.Info("Data will be proccessed in fast mode.");

            var handsPlayers = new ConcurrentStack<HandPlayer>();

            var counter = 0;

            LogProvider.Log.Info($"Begin the reading of statistic: {statisticFiles.Count} files to read.");

            Parallel.ForEach(statisticFiles, file =>
            {
                playerStatisticRepository.GetPlayerStatisticFromFile(file).Where(stat => !stat.IsTourney).ForEach(stat =>
                {
                    var handKey = new HandHistoryKey(stat.GameNumber, (short)stat.PokersiteId);

                    if (hands.ContainsKey(handKey))
                    {
                        var handPlayer = new HandPlayer
                        {
                            PlayerId = stat.PlayerId,
                            HandId = hands[handKey],
                            NetWon = Utils.ConvertToCents(stat.NetWon)
                        };

                        handsPlayers.Push(handPlayer);

                        if (counter % 100 == 0)
                        {
                            MigrationUtils.SetStatusMessage($"Reading statistic {counter}/{statisticFiles.Count}");
                        }
                    }
                });

                Interlocked.Increment(ref counter);
            });

            counter = 0;

            LogProvider.Log.Info($"Begin data inserting : {handsPlayers.Count} rows to insert.");

            using (var cmd = new SQLiteCommand("insert into HandsPlayers (HandId, PlayerId, NetWon) values (@HandId, @PlayerId, @NetWon)", conn))
            {
                cmd.Parameters.Add("@HandId", DbType.Int32);
                cmd.Parameters.Add("@PlayerId", DbType.Int32);
                cmd.Parameters.Add("@NetWon", DbType.Int64);

                cmd.Prepare();

                using (var transaction = conn.BeginTransaction())
                {
                    foreach (var hand in handsPlayers)
                    {
                        if (counter % 25000 == 0)
                        {
                            MigrationUtils.SetStatusMessage($"Inserting data {counter}/{handsPlayers.Count}");
                        }

                        cmd.Parameters[0].Value = hand.HandId;
                        cmd.Parameters[1].Value = hand.PlayerId;
                        cmd.Parameters[2].Value = hand.NetWon;
                        cmd.ExecuteNonQuery();

                        counter++;
                    }

                    transaction.Commit();
                }
            }
        }

        private class HandHistoryKey
        {
            public HandHistoryKey(long gameNumber, short pokerSiteId)
            {
                GameNumber = gameNumber;
                PokerSiteId = pokerSiteId;
            }

            public long GameNumber { get; private set; }

            public short PokerSiteId { get; private set; }

            public override bool Equals(object obj)
            {
                var handHistoryKey = obj as HandHistoryKey;

                return Equals(handHistoryKey);
            }

            private bool Equals(HandHistoryKey handHistoryKey)
            {
                return handHistoryKey != null && handHistoryKey.GameNumber == GameNumber && handHistoryKey.PokerSiteId == handHistoryKey.PokerSiteId;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + GameNumber.GetHashCode();
                    hashcode = (hashcode * 31) + PokerSiteId;
                    return hashcode;
                }
            }
        }
    }
}