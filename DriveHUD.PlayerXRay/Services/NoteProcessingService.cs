//-----------------------------------------------------------------------
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
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.Licensing;
using HandHistories.Objects.Hand;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.PlayerXRay.Services
{
    internal class NoteProcessingService : INoteProcessingService
    {
        private const int handHistoryRowsPerQuery = 1000;
        private readonly IHandHistoryParserFactory handHistoryParserFactory;
        private readonly SingletonStorageModel storageModel;
        private readonly ILicenseService licenseService;

        private Dictionary<PlayerPokerSiteKey, Players> playersDictionary;
        private Dictionary<PlayerPokerSiteKey, Dictionary<long, List<Playernotes>>> playersNotesDictionary;

        public NoteProcessingService()
        {
            handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();
            storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();
            licenseService = ServiceLocator.Current.GetInstance<ILicenseService>();

            InitializeLimits();
        }

        private void InitializeLimits()
        {
            var registeredLicenses = licenseService.LicenseInfos.Where(x => x.IsRegistered).ToArray();

            // if any license is not trial
            if (registeredLicenses.Any(x => !x.IsTrial))
            {
                registeredLicenses = registeredLicenses.Where(x => !x.IsTrial).ToArray();
            }

            var gameTypes = registeredLicenses.SelectMany(x => ConvertLicenseType(x.LicenseType)).Distinct().ToArray();

            Limit = new LicenseLimit
            {
                TournamentLimit = registeredLicenses.Length > 0 ? registeredLicenses.Max(x => x.TournamentLimit) : 0,
                CashLimit = registeredLicenses.Length > 0 ? registeredLicenses.Max(x => x.CashLimit) : 0,
                AllowedGameTypes = gameTypes
            };
        }

        private static IEnumerable<HandHistories.Objects.GameDescription.GameType> ConvertLicenseType(LicenseType licenseType)
        {
            var gameTypes = new List<HandHistories.Objects.GameDescription.GameType>();

            switch (licenseType)
            {
                case LicenseType.Holdem:
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.CapNoLimitHoldem);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FixedLimitHoldem);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.NoLimitHoldem);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.PotLimitHoldem);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.SpreadLimitHoldem);
                    break;
                case LicenseType.Omaha:
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.CapPotLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmahaHiLo);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FixedLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.FixedLimitOmahaHiLo);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.NoLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.NoLimitOmahaHiLo);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.PotLimitOmaha);
                    gameTypes.Add(HandHistories.Objects.GameDescription.GameType.PotLimitOmahaHiLo);
                    break;
                case LicenseType.Combo:
                case LicenseType.Trial:
                    foreach (HandHistories.Objects.GameDescription.GameType gameType in Enum.GetValues(typeof(HandHistories.Objects.GameDescription.GameType)))
                    {
                        gameTypes.Add(gameType);
                    }
                    break;
                default:
                    throw new DHInternalException(new NonLocalizableString("Not supported license type"));
            }

            return gameTypes;
        }

        private LicenseLimit Limit { get; set; }

        public event EventHandler<NoteProcessingServiceProgressChangedEventArgs> ProgressChanged;

        public IEnumerable<Playernotes> ProcessHand(IEnumerable<NoteObject> notes, Playerstatistic stats, HandHistory handHistory)
        {
            if (!licenseService.IsRegistered)
            {
                return null;
            }

            var matchInfo = new GameMatchInfo
            {
                GameType = handHistory.GameDescription.GameType,
                CashBuyIn = !handHistory.GameDescription.IsTournament ? Utils.ConvertToCents(handHistory.GameDescription.Limit.BigBlind) : 0,
                TournamentBuyIn = handHistory.GameDescription.IsTournament ? handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue : 0
            };

            if (!Limit.IsMatch(matchInfo))
            {
                return null;
            }

            var playernotes = new List<Playernotes>();

            foreach (var note in notes)
            {
                var playerstatistic = new PlayerstatisticExtended
                {
                    Playerstatistic = stats,
                    HandHistory = handHistory
                };

                if (NoteManager.IsMatch(note, playerstatistic))
                {
                    var playerNote = new Playernotes
                    {
                        PlayerId = stats.PlayerId,
                        PokersiteId = (short)stats.PokersiteId,
                        Note = note.DisplayedNote,
                        CardRange = playerstatistic.Playerstatistic.Cards,
                        IsAutoNote = true,
                        GameNumber = handHistory.HandId,
                        Timestamp = handHistory.DateOfHandUtc
                    };

                    playernotes.Add(playerNote);
                }
            }

            return playernotes;
        }

        public void ProcessNotes(IEnumerable<NoteObject> notes, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (!licenseService.IsRegistered || (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested))
                {
                    return;
                }

                var currentPlayerIds = new HashSet<int>(storageModel.PlayerSelectedItem.PlayerIds);

                var noteService = ServiceLocator.Current.GetInstance<IPlayerNotesService>() as IPlayerXRayNoteService;

                var sinceDate = noteService.CurrentNotesAppSettings.IsNoteCreationSinceDate ?
                        noteService.CurrentNotesAppSettings.NoteCreationSinceDate : (DateTime?)null;

                var takesNotesOnHero = noteService.CurrentNotesAppSettings.TakesNotesOnHero;

                using (var session = ModelEntities.OpenStatelessSession())
                {
                    BuildPlayersDictonary(session);
                    BuildPlayersNotesDictonary(session);

                    var entitiesCount = sinceDate != null ?
                        session.Query<Handhistory>().Count(x => x.Handtimestamp >= sinceDate.Value) :
                        session.Query<Handhistory>().Count();

                    var progressCounter = 0;
                    var progress = 0;

                    var numOfQueries = (int)Math.Ceiling((double)entitiesCount / handHistoryRowsPerQuery);

                    LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"Processing notes [{notes.Count()}] for {entitiesCount} hands.");

                    for (var i = 0; i < numOfQueries; i++)
                    {
                        if (cancellationTokenSource != null && cancellationTokenSource.IsCancellationRequested)
                        {
                            LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"The note processing has been canceled [{i}/{numOfQueries}].");
                            break;
                        }

                        using (var transaction = session.BeginTransaction())
                        {
                            var numOfRowToStartQuery = i * handHistoryRowsPerQuery;

                            var handHistories = sinceDate != null ?
                                session.Query<Handhistory>()
                                    .Where(x => x.Handtimestamp >= sinceDate.Value)
                                    .Skip(numOfRowToStartQuery)
                                    .Take(handHistoryRowsPerQuery)
                                    .ToArray() :
                                session.Query<Handhistory>()
                                    .Skip(numOfRowToStartQuery)
                                    .Take(handHistoryRowsPerQuery)
                                    .ToArray();

                            var notesToInsert = new ConcurrentBag<Playernotes>();

                            Parallel.ForEach(handHistories, handHistory =>
                            {
                                try
                                {
                                    var parsingResult = ParseHandHistory(handHistory);

                                    if (parsingResult != null)
                                    {
                                        var matchInfo = new GameMatchInfo
                                        {
                                            GameType = parsingResult.Source.GameDescription.GameType,
                                            CashBuyIn = !parsingResult.Source.GameDescription.IsTournament ?
                                                Utils.ConvertToCents(parsingResult.Source.GameDescription.Limit.BigBlind) : 0,
                                            TournamentBuyIn = parsingResult.Source.GameDescription.IsTournament ?
                                                parsingResult.Source.GameDescription.Tournament.BuyIn.PrizePoolValue : 0
                                        };

                                        if (!Limit.IsMatch(matchInfo))
                                        {
                                            return;
                                        }

                                        foreach (var player in parsingResult.Players)
                                        {
                                            // skip notes for current player (need to check setting)
                                            if (!takesNotesOnHero && currentPlayerIds.Contains(player.PlayerId))
                                            {
                                                continue;
                                            }

                                            if (player.PlayerId != 0)
                                            {
                                                var playerNoteKey = new PlayerPokerSiteKey(player.PlayerId, player.PokersiteId);

                                                var playerStatistic = BuildPlayerStatistic(parsingResult, player);

                                                var playerNotes = ProcessHand(notes, playerStatistic, parsingResult.Source);

                                                if (playerNotes.Count() == 0)
                                                {
                                                    continue;
                                                }

                                                // if player has no notes, then just save all new notes
                                                if (!playersNotesDictionary.ContainsKey(playerNoteKey)
                                                || (playersNotesDictionary.ContainsKey(playerNoteKey) &&
                                                    !playersNotesDictionary[playerNoteKey].ContainsKey(handHistory.Gamenumber)))
                                                {
                                                    playerNotes.ForEach(note => notesToInsert.Add(note));
                                                    continue;
                                                }

                                                var existingNotes = playersNotesDictionary[playerNoteKey][handHistory.Gamenumber];

                                                var notesToAdd = (from playerNote in playerNotes
                                                                  join existingNote in existingNotes on playerNote.Note equals existingNote.Note into gj
                                                                  from note in gj.DefaultIfEmpty()
                                                                  where note == null
                                                                  select playerNote).ToArray();

                                                notesToAdd.ForEach(note => notesToInsert.Add(note));
                                            }
                                        };
                                    }
                                }
                                catch (Exception hhEx)
                                {
                                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, $"Hand history {handHistory.Gamenumber} has not been processed", hhEx);
                                }

                                // reporting progress
                                progressCounter++;

                                var currentProgress = progressCounter * 100 / entitiesCount;

                                if (progress < currentProgress)
                                {
                                    progress = currentProgress;
                                    ProgressChanged?.Invoke(this, new NoteProcessingServiceProgressChangedEventArgs(progress));
                                }
                            });

                            notesToInsert.ForEach(x => session.Insert(x));

                            transaction.Commit();
                        }
                    }

                    LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"Notes have been processed.");
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Notes have not been processed.", e);
            }
        }

        /// <summary>
        /// Deletes notes before the specified date
        /// </summary>
        /// <param name="sinceDate"></param>
        public void DeletesNotes(DateTime? beforeDate)
        {
            try
            {
                using (var session = ModelEntities.OpenStatelessSession())
                {
                    using (var transaction = session.BeginTransaction())
                    {
                        var date = beforeDate.HasValue ? beforeDate : DateTime.MaxValue;

                        var entitiesToDelete = session.Query<Playernotes>().Where(x => x.IsAutoNote && x.Timestamp <= date).ToArray();

                        LogProvider.Log.Info(CustomModulesNames.PlayerXRay, $"Deleting {entitiesToDelete.Length} notes.");

                        var progress = 0;

                        for (var i = 0; i < entitiesToDelete.Length; i++)
                        {
                            session.Delete(entitiesToDelete[i]);

                            var currentProgress = i * 100 / entitiesToDelete.Length;

                            if (progress < currentProgress)
                            {
                                progress = currentProgress;
                                ProgressChanged?.Invoke(this, new NoteProcessingServiceProgressChangedEventArgs(progress));
                            }
                        }

                        transaction.Commit();
                    }

                    LogProvider.Log.Info(CustomModulesNames.PlayerXRay, "Notes have been deleted.");
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PlayerXRay, "Notes haven't been deleted.", e);
            }
        }

        /// <summary>
        /// Builds dictionary with player notes
        /// </summary>        
        private void BuildPlayersNotesDictonary(IStatelessSession session)
        {
            playersNotesDictionary = session.Query<Playernotes>()
                .Where(x => x.IsAutoNote)
                .Select(x => new { PlayerKey = new PlayerPokerSiteKey(x.PlayerId, x.PokersiteId), Notes = x })
                .GroupBy(x => x.PlayerKey)
                .ToDictionary(x => x.Key, x => x.Select(y => y.Notes)
                    .GroupBy(y => y.GameNumber)
                    .ToDictionary(y => y.Key, y => y.ToList()));
        }

        private ParsingResult ParseHandHistory(Handhistory handHistory)
        {
            if (string.IsNullOrEmpty(handHistory.HandhistoryVal))
            {
                LogProvider.Log.Warn(CustomModulesNames.PlayerXRay, $"Hand #{handHistory.Gamenumber} has been skipped, because it has no history.");
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
        private void BuildPlayersDictonary(IStatelessSession session)
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
                    session.CreateSQLQuery($"DELETE FROM \"PlayerNotes\" WHERE IsAutoNote=1").ExecuteUpdate();
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

        private class LicenseLimit
        {
            public HandHistories.Objects.GameDescription.GameType[] AllowedGameTypes
            {
                get;
                set;
            }

            public decimal TournamentLimit
            {
                get;
                set;
            }

            public int CashLimit
            {
                get;
                set;
            }

            public bool IsMatch(GameMatchInfo gameInfo)
            {
                if (gameInfo == null)
                {
                    return false;
                }

                var match = AllowedGameTypes.Contains(gameInfo.GameType) &&
                                gameInfo.CashBuyIn <= CashLimit &&
                                    gameInfo.TournamentBuyIn <= TournamentLimit;

                return match;
            }
        }

        private class GameMatchInfo
        {
            public HandHistories.Objects.GameDescription.GameType GameType { get; set; }

            /// <summary>
            /// Tournament buyin in $ (or other currency)
            /// </summary>
            public decimal TournamentBuyIn { get; set; }

            /// <summary>
            /// Max table buyin in NL (100NL = 1$)
            /// </summary>
            public int CashBuyIn { get; set; }
        }

        #endregion
    }
}