﻿//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticReImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Objects.Cards;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using Model.Reports;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Defines methods for player statistic re-importer
    /// </summary>
    internal class PlayerStatisticReImporter : IPlayerStatisticReImporter
    {
        private readonly string playerStatisticDataFolder;
        private readonly string playerStatisticTempDataFolder;
        private readonly string playerStatisticBackupDataFolder;
        private readonly string playerStatisticOldDataFolder;
        private readonly IHandHistoryParserFactory handHistoryParserFactory;
        private readonly IPlayerStatisticRepository playerStatisticRepository;
        private const int handHistoryRowsPerQuery = 1000;
        private Dictionary<PlayerPokerSiteKey, Players> playersDictionary;

        private IDHProgress progress;

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticReImporter"/> 
        /// </summary>
        protected PlayerStatisticReImporter(string playerStatisticDataFolder, string playerStatisticTempDataFolder, string playerStatisticBackupDataFolder, string playerStatisticOldDataFolder)
        {
            this.playerStatisticDataFolder = playerStatisticDataFolder;
            this.playerStatisticTempDataFolder = playerStatisticTempDataFolder;
            this.playerStatisticBackupDataFolder = playerStatisticBackupDataFolder;
            this.playerStatisticOldDataFolder = playerStatisticOldDataFolder;
            handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticReImporter"/> 
        /// </summary>
        public PlayerStatisticReImporter() : this(StringFormatter.GetPlayerStatisticDataFolderPath(),
            StringFormatter.GetPlayerStatisticDataTempFolderPath(), StringFormatter.GetPlayerStatisticDataBackupFolderPath(), StringFormatter.GetPlayerStatisticDataOldFolderPath())
        {
        }

        /// <summary>
        /// Re-imports player statistic using hand histories in the current database
        /// </summary>
        public void ReImport()
        {
            try
            {
                BuildPlayersDictonary();
                PrepareTemporaryPlayerStatisticData();
                ImportHandHistories();
                ClosePlayerstatisticStreams();
                ReplaceOriginalPlayerstatistic();
                ResetOpponentReport();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Re-import of player statistic failed.", e);
                ClosePlayerstatisticStreams();
                throw new DHBusinessException(new NonLocalizableString("Player statistic rebuilding failed"));
            }
            finally
            {
                playerStatisticRepository?.SetPlayerStatisticPath(StringFormatter.GetPlayerStatisticDataFolderPath());
            }
        }

        /// <summary>
        /// Recovers player statistic data from backup
        /// </summary>
        public void Recover()
        {
            try
            {
                if (!Directory.Exists(playerStatisticBackupDataFolder))
                {
                    LogProvider.Log.Info("Folder with player statistic backup data has not been found");
                    return;
                }

                var newPlayerStatisticOldDataFolder = playerStatisticOldDataFolder;
                var backupFolderIndex = 1;

                while (Directory.Exists(newPlayerStatisticOldDataFolder))
                {
                    newPlayerStatisticOldDataFolder = $"{playerStatisticOldDataFolder}{backupFolderIndex++}";
                }

                if (newPlayerStatisticOldDataFolder != playerStatisticOldDataFolder)
                {
                    Directory.Move(playerStatisticOldDataFolder, newPlayerStatisticOldDataFolder);
                }

                Directory.Move(playerStatisticDataFolder, playerStatisticOldDataFolder);
                Directory.Move(playerStatisticBackupDataFolder, playerStatisticDataFolder);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Recovering of player statistic failed.", e);
            }
        }

        /// <summary>
        /// Initializes progress to report progress
        /// </summary>
        public void InitializeProgress(IDHProgress progress)
        {
            this.progress = progress;
        }

        /// <summary>
        /// Builds players dictionary
        /// </summary>
        private void BuildPlayersDictonary()
        {
            playersDictionary = new Dictionary<PlayerPokerSiteKey, Players>();

            using (var session = ModelEntities.OpenStatelessSession())
            {
                var players = session.Query<Players>().ToArray();

                players.ForEach(player =>
                {
                    var playerPokerSiteKey = new PlayerPokerSiteKey(player.Playername, player.PokersiteId);

                    if (!playersDictionary.ContainsKey(playerPokerSiteKey))
                    {
                        playersDictionary.Add(playerPokerSiteKey, player);
                    }
                });
            }
        }

        /// <summary>
        /// Prepares temporary folder for re-imported player statistic data
        /// </summary>
        private void PrepareTemporaryPlayerStatisticData()
        {
            if (Directory.Exists(playerStatisticTempDataFolder))
            {
                Directory.Delete(playerStatisticTempDataFolder, true);
            }

            Directory.CreateDirectory(playerStatisticTempDataFolder);

            playerStatisticRepository.SetPlayerStatisticPath(playerStatisticTempDataFolder);

            if (playerStatisticStreamWriters.Count > 0)
            {
                ClosePlayerstatisticStreams();
            }
        }

        /// <summary>
        /// Imports hand histories stored in DB
        /// </summary>
        private void ImportHandHistories()
        {
            using (var session = ModelEntities.OpenStatelessSession())
            {
                var entitiesCount = session.Query<Handhistory>().Count();

                var numOfQueries = (int)Math.Ceiling((double)entitiesCount / handHistoryRowsPerQuery);

                progress?.Report(new LocalizableString("Progress_RebuildingStatistics_Status", 0, entitiesCount));

                var progressResult = 0;

                for (var i = 0; i < numOfQueries; i++)
                {
                    var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                    var handHistories = session.Query<Handhistory>()
                        .OrderBy(x => x.Handtimestamp)
                        .Skip(numOfRowToStartQuery)
                        .Take(handHistoryRowsPerQuery)
                        .ToArray();

                    // close opened handles
                    var minDateOfHand = handHistories.MinOrDefault(x => x.Handtimestamp);

                    if (minDateOfHand.HasValue)
                    {
                        ClosePlayerstatisticStreams(minDateOfHand.Value);
                    }

                    Parallel.ForEach(handHistories, handHistory =>
                    {
                        var parsingResult = ParseHandHistory(handHistory);

                        if (parsingResult != null)
                        {
                            var playerStatisticCreationInfo = new PlayerStatisticCreationInfo
                            {
                                ParsingResult = parsingResult
                            };

                            parsingResult.Players.ForEach(player =>
                            {
                                var calculatedEquity = new Dictionary<string, Dictionary<Street, decimal>>();

                                if (player.PlayerId != 0)
                                {
                                    playerStatisticCreationInfo.Player = player;

                                    BuildPlayerStatistic(handHistory, playerStatisticCreationInfo);
                                }
                            });
                        }
                    });

                    GC.Collect();

                    progressResult += handHistories.Length;
                    progress?.Report(new LocalizableString("Progress_RebuildingStatistics_Status", progressResult, entitiesCount));
                }
            }
        }

        /// <summary>
        /// Parses the specified <see cref="Handhistory"/>
        /// </summary>
        /// <param name="handHistory"><see cref="Handhistory"/> to import</param>
        private ParsingResult ParseHandHistory(Handhistory handHistory)
        {
            if (string.IsNullOrEmpty(handHistory.HandhistoryVal))
            {
                LogProvider.Log.Warn($"Hand #{handHistory.Gamenumber} has been skipped, because it has no history.");
                return null;
            }

            var pokerSite = (EnumPokerSites)handHistory.PokersiteId;

            var pokerSiteNetwork = EntityUtils.GetSiteNetwork(pokerSite);

            var handHistoryParser = pokerSite == EnumPokerSites.Unknown || pokerSiteNetwork == EnumPokerNetworks.WPN ?
                handHistoryParserFactory.GetFullHandHistoryParser(handHistory.HandhistoryVal) :
                handHistoryParserFactory.GetFullHandHistoryParser(pokerSite);

            var parsedHand = handHistoryParser.ParseFullHandHistory(handHistory.HandhistoryVal, true);

            var gameType = new Gametypes
            {
                Anteincents = Utils.ConvertToCents(parsedHand.GameDescription.Limit.Ante),
                Bigblindincents = Utils.ConvertToCents(parsedHand.GameDescription.Limit.BigBlind),
                CurrencytypeId = (short)parsedHand.GameDescription.Limit.Currency,
                Istourney = parsedHand.GameDescription.IsTournament,
                PokergametypeId = (short)(parsedHand.GameDescription.GameType),
                Smallblindincents = Utils.ConvertToCents(parsedHand.GameDescription.Limit.SmallBlind),
                Tablesize = (short)parsedHand.GameDescription.SeatType.MaxPlayers
            };

            var players = parsedHand.Players.Select(player =>
            {
                var playerPokerSiteKey = new PlayerPokerSiteKey(player.PlayerName, (int)pokerSite);

                if (playersDictionary.ContainsKey(playerPokerSiteKey))
                {
                    return playersDictionary[playerPokerSiteKey];
                }

                return new Players
                {
                    Playername = player.PlayerName,
                    PokersiteId = (short)pokerSite
                };
            }).ToList();

            var parsingResult = new ParsingResult
            {
                HandHistory = handHistory,
                Players = players,
                GameType = gameType,
                Source = parsedHand
            };

            return parsingResult;
        }

        /// <summary>
        /// Builds player statistic
        /// </summary>
        /// <param name="handHistory"></param>
        /// <param name="player"></param>
        private void BuildPlayerStatistic(Handhistory dbHandHistory, PlayerStatisticCreationInfo playerStatisticCreationInfo)
        {
            try
            {
                var playerStatisticCalculator = ServiceLocator.Current.GetInstance<IPlayerStatisticCalculator>(playerStatisticCreationInfo.GetServiceName());

                var statistic = playerStatisticCalculator.CalculateStatistic(playerStatisticCreationInfo);

                if (!string.IsNullOrEmpty(dbHandHistory.Tourneynumber))
                {
                    statistic.IsTourney = true;
                    statistic.TournamentId = dbHandHistory.Tourneynumber;
                }

                StorePlayerStatistic(statistic);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to process hand #{dbHandHistory.Gamenumber}, player {playerStatisticCreationInfo.Player?.Playername} .", e);
                throw new DHInternalException(new NonLocalizableString($"Failed to rebuild stats of hand #{dbHandHistory.Gamenumber}"));
            }
        }

        /// <summary>
        /// Replaces original player statistic with re-imported player statistic
        /// </summary>
        private void ReplaceOriginalPlayerstatistic()
        {
            var newPlayerStatisticBackupDataFolder = playerStatisticBackupDataFolder;
            var backupFolderIndex = 1;

            while (Directory.Exists(newPlayerStatisticBackupDataFolder))
            {
                newPlayerStatisticBackupDataFolder = $"{playerStatisticBackupDataFolder}{backupFolderIndex++}";
            }

            if (newPlayerStatisticBackupDataFolder != playerStatisticBackupDataFolder)
            {
                try
                {
                    Directory.Move(playerStatisticBackupDataFolder, newPlayerStatisticBackupDataFolder);
                }
                catch
                {
                    LogProvider.Log.Error(this, $"Could not move {playerStatisticBackupDataFolder} to {newPlayerStatisticBackupDataFolder}");
                    throw;
                }
            }

            if (Directory.Exists(playerStatisticDataFolder))
            {
                try
                {
                    Directory.Move(playerStatisticDataFolder, playerStatisticBackupDataFolder);
                }
                catch
                {
                    LogProvider.Log.Error(this, $"Could not move {playerStatisticDataFolder} to {playerStatisticBackupDataFolder}");
                    throw;
                }
            }

            try
            {
                Directory.Move(playerStatisticTempDataFolder, playerStatisticDataFolder);
            }
            catch
            {
                LogProvider.Log.Error(this, $"Could not move {playerStatisticTempDataFolder} to {playerStatisticDataFolder}");
                throw;
            }
        }

        /// <summary>
        /// Resets opponent report cache
        /// </summary>
        private void ResetOpponentReport()
        {
            var opponentReportService = ServiceLocator.Current.GetInstance<IOpponentReportService>();
            opponentReportService.Reset();
        }

        private void StorePlayerStatistic(Playerstatistic statistic)
        {
            try
            {
                var serializedStatistic = string.Empty;

                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, statistic);
                    serializedStatistic = Convert.ToBase64String(ms.ToArray()).Trim();
                }

                var streamWriter = GetOrCreatePlayestatisticStreamWriter(statistic);

                lock (streamWriter)
                {
                    streamWriter.WriteLine(serializedStatistic);
                }
            }
            catch
            {
                LogProvider.Log.Error(this, "Could not store statistic.");
                throw;
            }
        }

        private readonly ConcurrentDictionary<string, Lazy<StreamWriter>> playerStatisticStreamWriters = new ConcurrentDictionary<string, Lazy<StreamWriter>>();

        private StreamWriter GetOrCreatePlayestatisticStreamWriter(Playerstatistic statistic)
        {
            var playerDirectory = Path.Combine(playerStatisticTempDataFolder, statistic.PlayerId.ToString());

            var fileName = Path.Combine(playerDirectory, statistic.Playedyearandmonth.ToString()) + ".stat";

            var playestatisticStreamWriter = playerStatisticStreamWriters.GetOrAdd(fileName,
                key => new Lazy<StreamWriter>(() => CreatePlayerstatisticStreamWriter(playerDirectory, key)));

            return playestatisticStreamWriter.Value;
        }

        private StreamWriter CreatePlayerstatisticStreamWriter(string playerDirectory, string fileName)
        {
            try
            {
                if (!Directory.Exists(playerDirectory))
                {
                    Directory.CreateDirectory(playerDirectory);
                }

                var streamWriter = new StreamWriter(fileName, true);

                return streamWriter;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not open stream for '{fileName}'.");
                throw e;
            }
        }

        private void ClosePlayerstatisticStreams(DateTime timestamp)
        {
            playerStatisticStreamWriters.ToArray().ForEach(x =>
            {
                var fileName = Path.GetFileNameWithoutExtension(x.Key);
                var minDate = new DateTime(timestamp.Year, timestamp.Month, 1);

                var fileDate = DateTime.ParseExact(fileName, "yyyyMM", CultureInfo.InvariantCulture);

                if (fileDate < minDate)
                {
                    try
                    {
                        x.Value.Value.Close();

                        Lazy<StreamWriter> removedValue = null;

                        playerStatisticStreamWriters.TryRemove(x.Key, out removedValue);
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Error(this, $"Could not release StreamWriter of '{x.Key}'", ex);
                    }
                }
            });
        }

        private void ClosePlayerstatisticStreams()
        {
            try
            {
                foreach (KeyValuePair<string, Lazy<StreamWriter>> playerStatisticStreamWriter in playerStatisticStreamWriters)
                {
                    try
                    {
                        playerStatisticStreamWriter.Value.Value.Close();
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(this, $"Could not close stream for '{playerStatisticStreamWriter.Key}'", e);
                    }
                }

                playerStatisticStreamWriters.Clear();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not close file streams", ex);
            }
        }

        #region Class helpers

        /// <summary>
        /// Represents the combined key of the player and the poker site
        /// </summary>
        private class PlayerPokerSiteKey
        {
            public PlayerPokerSiteKey(string playerName, int pokerSite)
            {
                PlayerName = playerName;
                PokerSite = pokerSite;
            }

            public string PlayerName { get; set; }

            public int PokerSite { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + PlayerName.GetHashCode();
                    hashcode = (hashcode * 31) + PokerSite;
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                var playerKey = obj as PlayerPokerSiteKey;

                return Equals(playerKey);
            }

            public bool Equals(PlayerPokerSiteKey obj)
            {
                if (obj == null)
                {
                    return false;
                }

                return PlayerName == obj.PlayerName && PokerSite == obj.PokerSite;
            }
        }

        #endregion
    }
}