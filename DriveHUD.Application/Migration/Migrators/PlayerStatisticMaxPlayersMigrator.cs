//-----------------------------------------------------------------------
// <copyright file="PlayerStatisticMaxPlayersMigrator.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Common.Log;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Application.MigrationService.Migrators
{
    /// <summary>
    /// Defines methods to update max players property in <see cref="Playerstatistic"/>
    /// </summary>
    internal class PlayerStatisticMaxPlayersMigrator
    {
        private readonly string playerStatisticDataFolder;
        private readonly string playerStatisticTempDataFolder;
        private readonly string playerStatisticBackupDataFolder;

        private readonly IDataService dataService;

        private Dictionary<HandNumberPokerSiteKey, short> handHistoryNumberTableSize;

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticMaxPlayersMigrator"/> 
        /// </summary>
        protected PlayerStatisticMaxPlayersMigrator(string playerStatisticDataFolder, string playerStatisticTempDataFolder, string playerStatisticBackupDataFolder)
        {
            this.playerStatisticDataFolder = playerStatisticDataFolder;
            this.playerStatisticTempDataFolder = playerStatisticTempDataFolder;
            this.playerStatisticBackupDataFolder = playerStatisticBackupDataFolder;

            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticMaxPlayersMigrator"/> 
        /// </summary>
        public PlayerStatisticMaxPlayersMigrator() : this(StringFormatter.GetPlayerStatisticDataFolderPath(),
            StringFormatter.GetPlayerStatisticDataTempFolderPath(), StringFormatter.GetPlayerStatisticDataBackupFolderPath())
        {
        }

        /// <summary>
        /// Updates max players property in all <see cref="Playerstatistic"/>
        /// </summary>
        public void Update()
        {
            try
            {
                LogProvider.Log.Info(this, "Starting statistic update.");

                PrepareTemporaryPlayerStatisticData();
                PrepareHandHistoryTableSizeMap();

                var statFiles = Directory.EnumerateFiles(playerStatisticDataFolder, "*.stat", SearchOption.AllDirectories);
                var statFileCounter = 0;

                LogProvider.Log.Info(this, "Processing statistic files.");

                Parallel.ForEach(statFiles, file =>
                {
                    ProcessPlayerStatisticFile(file);
                    Interlocked.Increment(ref statFileCounter);
                });

                LogProvider.Log.Info(this, $"Processed {statFileCounter} statistic files.");

                ReplaceOriginalPlayerstatistic();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't update max players in statistic", e);
                throw new DHBusinessException(new NonLocalizableString("Player statistic updating failed"));
            }
        }

        /// <summary>
        /// Prepares dictionary with hand numbers and table sizes
        /// </summary>
        private void PrepareHandHistoryTableSizeMap()
        {
            LogProvider.Log.Info(this, "Loading hand histories map.");

            using (var session = ModelEntities.OpenStatelessSession())
            {
                handHistoryNumberTableSize = (from handHistory in session.Query<Handhistory>()
                                              join gameInfo in session.Query<Gametypes>() on handHistory.GametypeId equals gameInfo.GametypeId
                                              let handNumberPokerSiteKey = new HandNumberPokerSiteKey(handHistory.Gamenumber, handHistory.PokersiteId)
                                              select new { HandNumberPokerSiteKey = handNumberPokerSiteKey, TableSize = gameInfo.Tablesize })
                                     .ToDictionary(x => x.HandNumberPokerSiteKey, x => x.TableSize);

                LogProvider.Log.Info(this, $"Loaded {handHistoryNumberTableSize.Count} hand histories.");
            }
        }

        /// <summary>
        /// Prepares temporary folder for updated player statistic data
        /// </summary>
        private void PrepareTemporaryPlayerStatisticData()
        {
            LogProvider.Log.Info(this, "Creating temp folder for statistic files.");

            if (Directory.Exists(playerStatisticTempDataFolder))
            {
                LogProvider.Log.Info(this, $"'{playerStatisticTempDataFolder}' exists. Deleting...");

                Directory.Delete(playerStatisticTempDataFolder, true);
            }

            Directory.CreateDirectory(playerStatisticTempDataFolder);

            LogProvider.Log.Info(this, "Temp folder for statistic files has been created.");
        }

        /// <summary>
        /// Process the specified player statistic file
        /// </summary>        
        private void ProcessPlayerStatisticFile(string file)
        {
            var newStatFile = file.Replace(playerStatisticDataFolder, playerStatisticTempDataFolder);
            var newStatFileDirectory = Path.GetDirectoryName(newStatFile);

            if (!Directory.Exists(newStatFileDirectory))
            {
                Directory.CreateDirectory(newStatFileDirectory);
            }

            using (var streamWriter = new StreamWriter(newStatFile))
            {
                ProcessPlayerStatisticFile(file, streamWriter);
            }
        }

        /// <summary>
        /// Reads and processes player statistic for the specified file
        /// </summary>      
        private void ProcessPlayerStatisticFile(string srcFile, StreamWriter dstFileStreamWriter)
        {
            if (!File.Exists(srcFile))
            {
                return;
            }

            try
            {
                using (var sr = new StreamReader(srcFile))
                {
                    string line = null;

                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(line))
                            {
                                continue;
                            }

                            var byteAfter64 = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/').Trim());

                            using (var ms = new MemoryStream(byteAfter64))
                            {
                                var stat = Serializer.Deserialize<Playerstatistic>(ms);
                                ProcessPlayerStatistic(dstFileStreamWriter, stat);
                            }
                        }
                        catch (Exception ex)
                        {
                            LogProvider.Log.Error(this, $"Could not process the file: {srcFile}{Environment.NewLine}Error at line: {line}{Environment.NewLine}", ex);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"File '{srcFile}' has not been processed.", e);
                throw;
            }
        }

        /// <summary>
        /// Update <see cref="Playerstatistic.MaxPlayers"/> in the specified <see cref="Playerstatistic"/>, then save it to the specified file
        /// </summary>
        /// <param name="file">File to save updated player statistic</param>
        /// <param name="stat"><see cref="Playerstatistic"/> to update</param>
        private void ProcessPlayerStatistic(StreamWriter streamWriter, Playerstatistic stat)
        {
            var handNumberPokerSiteKey = new HandNumberPokerSiteKey(stat.GameNumber, stat.PokersiteId);

            if (!handHistoryNumberTableSize.ContainsKey(handNumberPokerSiteKey))
            {
                LogProvider.Log.Warn(this, $"Hand hasn't been found in db. It will be saved as is. Hand={stat.GameNumber}, PokerSite={(EnumPokerSites)stat.PokersiteId}");
                return;
            }

            var tableSize = handHistoryNumberTableSize[handNumberPokerSiteKey];

            stat.MaxPlayers = tableSize;

            StorePlayerStatistic(streamWriter, stat);
        }

        /// <summary>
        /// Serializes <see cref="Playerstatistic"/>, then appends the serialized data to the specified file
        /// </summary>
        /// <param name="file">File to append serialized <see cref="Playerstatistic"/></param>
        /// <param name="statistic"><see cref="Playerstatistic"/> to store in the specified file</param>
        private void StorePlayerStatistic(StreamWriter streamWriter, Playerstatistic statistic)
        {
            var data = string.Empty;

            using (var memoryStream = new MemoryStream())
            {
                Serializer.Serialize(memoryStream, statistic);
                data = Convert.ToBase64String(memoryStream.ToArray()).Trim();
            }

            if (!string.IsNullOrEmpty(data))
            {
                streamWriter.WriteLine(data);
            }
        }

        /// <summary>
        /// Replaces original player statistic with re-imported player statistic
        /// </summary>
        private void ReplaceOriginalPlayerstatistic()
        {
            LogProvider.Log.Info(this, $"Replacing '{playerStatisticDataFolder}' with '{playerStatisticTempDataFolder}'.");

            var newPlayerStatisticBackupDataFolder = playerStatisticBackupDataFolder;
            var backupFolderIndex = 1;

            while (Directory.Exists(newPlayerStatisticBackupDataFolder))
            {
                newPlayerStatisticBackupDataFolder = $"{playerStatisticBackupDataFolder}{backupFolderIndex++}";
            }

            if (newPlayerStatisticBackupDataFolder != playerStatisticBackupDataFolder)
            {
                Directory.Move(playerStatisticBackupDataFolder, newPlayerStatisticBackupDataFolder);
            }

            if (Directory.Exists(playerStatisticDataFolder))
            {
                Directory.Move(playerStatisticDataFolder, playerStatisticBackupDataFolder);
                LogProvider.Log.Info(this, $"Backup '{playerStatisticBackupDataFolder}' has been created.");
            }

            try
            {
                Directory.Move(playerStatisticTempDataFolder, playerStatisticDataFolder);
            }
            catch
            {
                try
                {
                    Directory.Move(playerStatisticBackupDataFolder, playerStatisticDataFolder);
                    LogProvider.Log.Error(this, $"Couldn't move temp data. Data from backup has been restored.");
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Couldn't move temp data. Data from backup couldn't be restored.", e);
                }

                throw;
            }

            LogProvider.Log.Info(this, "Replacing completed.");
        }

        #region Class helpers

        /// <summary>
        /// Represents the combined key of the player and the poker site
        /// </summary>
        private class HandNumberPokerSiteKey
        {
            public HandNumberPokerSiteKey(long handNumber, int pokerSite)
            {
                HandNumber = handNumber;
                PokerSite = pokerSite;
            }

            public long HandNumber { get; set; }

            public int PokerSite { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + HandNumber.GetHashCode();
                    hashcode = (hashcode * 31) + PokerSite;
                    return hashcode;
                }
            }

            public override bool Equals(object obj)
            {
                var playerKey = obj as HandNumberPokerSiteKey;

                return Equals(playerKey);
            }

            public bool Equals(HandNumberPokerSiteKey obj)
            {
                if (obj == null)
                {
                    return false;
                }

                return HandNumber == obj.HandNumber && PokerSite == obj.PokerSite;
            }
        }

        #endregion
    }
}