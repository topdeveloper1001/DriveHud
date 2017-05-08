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

using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using DriveHUD.Common.Log;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Resources;
using DriveHUD.Entities;
using ProtoBuf;
using System.Diagnostics;
using Microsoft.Practices.ServiceLocation;
using Model.Interfaces;
using NHibernate.Linq;
using DriveHUD.Common.Linq;
using HandHistories.Parser.Parsers.Factory;
using HandHistories.Parser.Parsers;
using DriveHUD.Common.Utils;

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
        private const int handHistoryRowsPerQuery = 1000;
        private Dictionary<HandPokerSiteKey, string> sessionDictionary;

        /// <summary>
        /// Initialize a new instance of <see cref="PlayerStatisticReImporter"/> 
        /// </summary>
        public PlayerStatisticReImporter()
        {
            playerStatisticDataFolder = StringFormatter.GetPlayerStatisticDataFolderPath();
            playerStatisticTempDataFolder = StringFormatter.GetPlayerStatisticDataTempFolderPath();
            playerStatisticBackupDataFolder = StringFormatter.GetPlayerStatisticDataBackupFolderPath();
            handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
        }

        /// <summary>
        /// Re-imports player statistic using hand histories in the current database
        /// </summary>
        public void ReImport()
        {
            try
            {
                BuildSessionDictionary();
                PrepareTemporaryPlayerStatisticData();
                ImportHandHistories();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Re-import of player statistic failed.", e);
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
        /// Prepares temporary folder for re-imported player statistic data
        /// </summary>
        private void PrepareTemporaryPlayerStatisticData()
        {
            if (Directory.Exists(playerStatisticTempDataFolder))
            {
                Directory.Delete(playerStatisticTempDataFolder, true);
            }

            Directory.CreateDirectory(playerStatisticTempDataFolder);

            var dataService = ServiceLocator.Current.GetInstance<IDataService>();
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

            var players = parsedHand.Players.Select(player => new Players
            {
                Playername = player.PlayerName,
                PokersiteId = handHistory.PokersiteId
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

        #endregion
    }
}