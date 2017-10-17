﻿//-----------------------------------------------------------------------
// <copyright file="NoteProcessingService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHud.Common.Log;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.PlayerXRay.Services
{
    internal class NoteProcessingService : INoteProcessingService
    {
        private const int handHistoryRowsPerQuery = 1000;
        private readonly IHandHistoryParserFactory handHistoryParserFactory;
        private readonly SingletonStorageModel storageModel;

        private Dictionary<PlayerPokerSiteKey, Players> playersDictionary;
        private Dictionary<PlayerPokerSiteKey, List<Playernotes>> playersNotesDictionary;

        public NoteProcessingService()
        {
            handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();
        }

        public event EventHandler<NoteProcessingServiceProgressChangedEventArgs> ProgressChanged;

        public Playernotes ProcessHand(IEnumerable<NoteObject> notes, Playerstatistic stats, HandHistory handHistory)
        {
            var notesMessages = new List<string>();

            foreach (var note in notes)
            {
                var playerstatistic = new PlayerstatisticExtended
                {
                    Playerstatistic = stats,
                    HandHistory = handHistory
                };

                var noteMessage = NoteManager.GetPlayerNote(note, new List<PlayerstatisticExtended> { playerstatistic });

                if (!string.IsNullOrEmpty(noteMessage))
                {
                    notesMessages.Add(noteMessage);
                }
            }

            if (notesMessages.Count == 0)
            {
                return null;
            }

            var playerNotes = new Playernotes
            {
                PlayerId = stats.PlayerId,
                PokersiteId = (short)stats.PokersiteId,
                AutoNote = string.Join(Environment.NewLine, notesMessages)
            };

            return playerNotes;
        }

        public void ProcessNotes(IEnumerable<NoteObject> notes)
        {
            try
            {
                ClearAllNotes();

                var currentPlayerIds = new HashSet<int>(storageModel.PlayerSelectedItem.PlayerIds);

                using (var session = ModelEntities.OpenSession())
                {
                    using (var pf = new PerformanceMonitor("ReadAndParseAllHands"))
                    {

                        BuildPlayersDictonary(session);
                        BuildPlayersNotesDictonary(session);

                        var entitiesCount = session.Query<Handhistory>().Count();

                        var progressCounter = 0;
                        var progress = 0;

                        var numOfQueries = (int)Math.Ceiling((double)entitiesCount / handHistoryRowsPerQuery);

                        for (var i = 0; i < numOfQueries; i++)
                        {
                            var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                            var handHistories = session.Query<Handhistory>()
                                .OrderBy(x => x.HandhistoryId)
                                .Skip(numOfRowToStartQuery)
                                .Take(handHistoryRowsPerQuery)
                                .ToArray();

                            foreach (var handHistory in handHistories)
                            {
                                try
                                {
                                    var parsingResult = ParseHandHistory(handHistory);

                                    if (parsingResult != null)
                                    {
                                        foreach (var player in parsingResult.Players)
                                        {
                                            // skip notes for current player (need to check setting)
                                            if (currentPlayerIds.Contains(player.PlayerId))
                                            {
                                                continue;
                                            }

                                            if (player.PlayerId != 0)
                                            {
                                                var playerStatistic = BuildPlayerStatistic(parsingResult, player);

                                                var playerNote = ProcessHand(notes, playerStatistic, parsingResult.Source);

                                                if (playerNote == null)
                                                {
                                                    continue;
                                                }

                                                var playerNoteKey = new PlayerPokerSiteKey(playerNote.PlayerId, playerNote.PokersiteId);

                                                if (playersNotesDictionary.ContainsKey(playerNoteKey))
                                                {
                                                    var playersNote = playersNotesDictionary[playerNoteKey].FirstOrDefault();
                                                    playersNote.AutoNote = NoteHelper.CombineAutoNotes(playersNote, playerNote);
                                                }
                                                else
                                                {
                                                    playersNotesDictionary.Add(playerNoteKey, new List<Playernotes> { playerNote });
                                                }
                                            }
                                        };
                                    }
                                }
                                catch (Exception hhEx)
                                {
                                    LogProvider.Log.Error(this, $"Hand history {handHistory.Gamenumber} has not been processed", hhEx);
                                }

                                // reporting progress
                                progressCounter++;

                                var currentProgress = progressCounter * 100 / entitiesCount;

                                if (progress < currentProgress)
                                {
                                    progress = currentProgress;
                                    ProgressChanged?.Invoke(this, new NoteProcessingServiceProgressChangedEventArgs(progress));
                                }
                            }
                        }
                    }

                    using (var pf = new PerformanceMonitor("SaveNotes"))
                    {
                        var notesToSave = playersNotesDictionary.Values.SelectMany(x => x).ToArray();
                        notesToSave.ForEach(x => session.SaveOrUpdate(x));
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Notes has not been processed.", e);
            }
        }

        /// <summary>
        /// Builds dictionary with player notes
        /// </summary>        
        private void BuildPlayersNotesDictonary(ISession session)
        {
            playersNotesDictionary = session.Query<Playernotes>()
                .Select(x => new { PlayerKey = new PlayerPokerSiteKey(x.PlayerId, x.PokersiteId), Notes = x })
                .GroupBy(x => x.PlayerKey)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Notes).ToList());
        }

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
        /// Builds players dictionary
        /// </summary>
        private void BuildPlayersDictonary(ISession session)
        {
            playersDictionary = new Dictionary<PlayerPokerSiteKey, Players>();

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

        /// <summary>
        /// Builds player statistic
        /// </summary>
        /// <param name="handHistory"></param>
        /// <param name="player"></param>
        private Playerstatistic BuildPlayerStatistic(ParsingResult handHistory, Players player)
        {
            var playerStatisticCalculator = ServiceLocator.Current.GetInstance<IPlayerStatisticCalculator>();

            var playerStat = playerStatisticCalculator.CalculateStatistic(handHistory, player);

            return playerStat;
        }

        private void ClearAllNotes()
        {
            using (var session = ModelEntities.OpenStatelessSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.CreateSQLQuery($"UPDATE \"PlayerNotes\" SET AutoNote=null").ExecuteUpdate();
                    session.CreateSQLQuery($"DELETE FROM \"PlayerNotes\" WHERE AutoNote is null and Note is null").ExecuteUpdate();
                    transaction.Commit();
                }
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

            public PlayerPokerSiteKey(int playerId, int pokerSite)
            {
                PlayerId = playerId;
                PokerSite = pokerSite;
            }

            public string PlayerName { get; set; }

            public int PlayerId { get; set; }

            public int PokerSite { get; set; }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashcode = 23;
                    hashcode = (hashcode * 31) + PlayerId;
                    hashcode = (hashcode * 31) + PokerSite;

                    if (PlayerName != null)
                    {
                        hashcode = (hashcode * 31) + PlayerName.GetHashCode();
                    }

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

                return PlayerId == obj.PlayerId && PokerSite == obj.PokerSite && PlayerName == obj.PlayerName;
            }
        }

        #endregion
    }
}