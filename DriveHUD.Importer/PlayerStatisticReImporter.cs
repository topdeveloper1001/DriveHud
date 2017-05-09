//-----------------------------------------------------------------------
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using NHibernate.Linq;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
        private readonly IHandHistoryParserFactory handHistoryParserFactory;
        private readonly IDataService dataService;
        private const int handHistoryRowsPerQuery = 1000;
        private Dictionary<HandPokerSiteKey, string> sessionDictionary;
        private Dictionary<PlayerPokerSiteKey, Players> playersDictionary;

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticReImporter"/> 
        /// </summary>
        protected PlayerStatisticReImporter(string playerStatisticDataFolder, string playerStatisticTempDataFolder, string playerStatisticBackupDataFolder)
        {
            this.playerStatisticDataFolder = playerStatisticDataFolder;
            this.playerStatisticTempDataFolder = playerStatisticTempDataFolder;
            this.playerStatisticBackupDataFolder = playerStatisticBackupDataFolder;
            handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticReImporter"/> 
        /// </summary>
        public PlayerStatisticReImporter() : this(StringFormatter.GetPlayerStatisticDataFolderPath(),
            StringFormatter.GetPlayerStatisticDataTempFolderPath(), StringFormatter.GetPlayerStatisticDataBackupFolderPath())
        {
        }

        /// <summary>
        /// Re-imports player statistic using hand histories in the current database
        /// </summary>
        public void ReImport()
        {
            try
            {
                BuildSessionDictionary();
                BuildPlayersDictonary();
                PrepareTemporaryPlayerStatisticData();
                ImportHandHistories();
                ReplaceOriginalPlayerstatistic();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Re-import of player statistic failed.", e);
            }
            finally
            {
                dataService?.SetPlayerStatisticPath(StringFormatter.GetPlayerStatisticDataFolderPath());
            }
        }

        /// <summary>
        /// Builds session dictionary for hand id, poker site
        /// </summary>
        private void BuildSessionDictionary()
        {
            var playerStatisticDataDirectoryInfo = new DirectoryInfo(playerStatisticDataFolder);

            if (!playerStatisticDataDirectoryInfo.Exists)
            {
                throw new DHBusinessException(new NonLocalizableString($"Directory '{playerStatisticDataFolder}' doesn't exist"));
            }

            sessionDictionary = new Dictionary<HandPokerSiteKey, string>();

            var playerStatisticFiles = playerStatisticDataDirectoryInfo.GetFiles($"*{StringFormatter.GetPlayerStatisticExtension()}", SearchOption.AllDirectories);

            foreach (var playerStatisticFile in playerStatisticFiles)
            {
                BuildSessionDictionary(playerStatisticFile);
            }
        }

        /// <summary>
        /// Builds session dictionary for hand id, poker site using the specified player statistic file
        /// </summary>
        private void BuildSessionDictionary(FileInfo playerStatisticFile)
        {
            using (var streamReader = new StreamReader(playerStatisticFile.FullName, Encoding.UTF8))
            {
                string line;

                while ((line = streamReader.ReadLine()) != null)
                {
                    var statBytes = Convert.FromBase64String(line.Replace('-', '+').Replace('_', '/'));

                    using (var memoryStream = new MemoryStream(statBytes))
                    {
                        var stat = Serializer.Deserialize<Playerstatistic>(memoryStream);

                        var handPokerSiteKey = new HandPokerSiteKey(stat.GameNumber, stat.PokersiteId);

                        if (sessionDictionary.ContainsKey(handPokerSiteKey))
                        {
                            continue;
                        }

                        sessionDictionary.Add(handPokerSiteKey, stat.SessionCode);
                    }
                }
            }
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

            dataService.SetPlayerStatisticPath(playerStatisticTempDataFolder);
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

                for (var i = 0; i < numOfQueries; i++)
                {
                    var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                    var handHistories = session.Query<Handhistory>()
                        .OrderBy(x => x.HandhistoryId)
                        .Skip(numOfRowToStartQuery)
                        .Take(handHistoryRowsPerQuery)
                        .ToArray();

                    handHistories.ForEach(handHistory =>
                    {
                        var parsingResult = ParseHandHistory(handHistory);

                        if (parsingResult != null)
                        {
                            parsingResult.Players.ForEach(player =>
                            {
                                if (player.PlayerId != 0)
                                {
                                    BuildPlayerStatistic(parsingResult, player);
                                }
                            });
                        }
                    });

                    GC.Collect();
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

            var handHistoryParser = pokerSite == EnumPokerSites.Unknown ?
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
        private void BuildPlayerStatistic(ParsingResult handHistory, Players player)
        {
            var playerStatisticCalculator = ServiceLocator.Current.GetInstance<IPlayerStatisticCalculator>();

            var playerStat = playerStatisticCalculator.CalculateStatistic(handHistory, player);

            var handPokerSiteKey = new HandPokerSiteKey(handHistory.HandHistory.Gamenumber, handHistory.HandHistory.PokersiteId);

            var session = sessionDictionary.ContainsKey(handPokerSiteKey) ?
                sessionDictionary[handPokerSiteKey] :
                string.Empty;

            playerStat.SessionCode = session;

            dataService.Store(playerStat);
        }

        /// <summary>
        /// Replaces original player statistic with re-imported player statistic
        /// </summary>
        private void ReplaceOriginalPlayerstatistic()
        {
            Directory.Move(playerStatisticDataFolder, playerStatisticBackupDataFolder);
            Directory.Move(playerStatisticTempDataFolder, playerStatisticDataFolder);
        }

        #region Class helpers

        /// <summary>
        /// Represents the combined key of the hand number and the poker site
        /// </summary>
        private class HandPokerSiteKey
        {
            public HandPokerSiteKey(long hand, int pokerSite)
            {
                Hand = hand;
                PokerSite = pokerSite;
            }

            public long Hand { get; set; }

            public int PokerSite { get; set; }

            public override int GetHashCode()
            {
                var hashcode = 23;
                hashcode = (hashcode * 31) + Hand.GetHashCode();
                hashcode = (hashcode * 31) + PokerSite;

                return hashcode;
            }

            public override bool Equals(object obj)
            {
                var playerKey = obj as HandPokerSiteKey;

                return Equals(playerKey);
            }

            public bool Equals(HandPokerSiteKey obj)
            {
                if (obj == null)
                {
                    return false;
                }

                return Hand == obj.Hand && PokerSite == obj.PokerSite;
            }
        }

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
                var hashcode = 23;
                hashcode = (hashcode * 31) + PlayerName.GetHashCode();
                hashcode = (hashcode * 31) + PokerSite;

                return hashcode;
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