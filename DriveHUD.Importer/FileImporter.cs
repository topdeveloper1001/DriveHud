//-----------------------------------------------------------------------
// <copyright file="FileImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common;
using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.Importers.Loggers;
using HandHistories.Objects.GameDescription;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Parsers.Base;
using HandHistories.Parser.Parsers.Exceptions;
using HandHistories.Parser.Parsers.Factory;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Importer;
using Model.Interfaces;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Implementation of file importer
    /// </summary>
    internal class FileImporter : IFileImporter
    {
        private readonly string[] importingExtensions = new[] { "txt", "xml" };

        private static readonly object locker = new object();

        private FileInfo processingFile;

        private readonly IImporterSessionCacheService importSessionCacheService;
        private readonly IDataService dataService;

        public FileImporter()
        {
            importSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="file">File to be imported</param>       
        /// <param name="progress">Progress object to report</param>     
        public void Import(FileInfo file, IDHProgress progress)
        {
            Import(new[] { file }, progress);
        }

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="files">Files to be imported</param>       
        /// <param name="progress">Progress object to report</param>     
        public void Import(FileInfo[] files, IDHProgress progress)
        {
            Check.ArgumentNotNull(() => files);
            Check.ArgumentNotNull(() => progress);

            progress.Report(new LocalizableString("Progress_StartingImport"));

            foreach (var file in files)
            {
                progress.Report(new LocalizableString("Progress_ReadingFile", file.Name));

                processingFile = file;

                try
                {
                    if (progress.CancellationToken.IsCancellationRequested)
                    {
                        progress.Report(new LocalizableString("Progress_StoppingImport"));
                        break;
                    }

                    var text = File.ReadAllText(file.FullName);

                    var gameInfo = new GameInfo
                    {
                        PokerSite = EnumPokerSites.Unknown,
                        FileName = file.FullName
                    };

                    Import(text, progress, gameInfo);
                }
                catch (DHInternalException ex)
                {
                    LogProvider.Log.Error(this, string.Format("File {0} has a bad format", file.FullName), ex);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(this, string.Format("File {0} couldn't be read", file.FullName), ex);
                }
            }
        }

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="directory">Directory to be imported</param>  
        /// <param name="progress">Progress object to report</param>           
        public void Import(DirectoryInfo directory, IDHProgress progress)
        {
            Check.ArgumentNotNull(() => directory);
            Check.ArgumentNotNull(() => progress);

            if (!directory.Exists)
            {
                return;
            }

            progress.Report(new LocalizableString("Progress_ScanningFolder"));

            var filesForImporting = importingExtensions.SelectMany(x => directory.GetFiles(x, SearchOption.AllDirectories))
                                    .Distinct(new LambdaComparer<FileInfo>((x, y) => x.FullName.Equals(y.FullName)))
                                    .ToArray();

            Import(filesForImporting, progress);
        }

        /// <summary>
        /// Import hands history data 
        /// </summary>
        /// <param name="text">Text to import</param>  
        /// <param name="progress">Progress object to report</param>     
        /// <param name="gameInfo">Game information</param>     
        public IEnumerable<ParsingResult> Import(string text, IDHProgress progress, GameInfo gameInfo)
        {
            Check.ArgumentNotNull(() => progress);
            Check.ArgumentNotNull(() => gameInfo);

            if (string.IsNullOrEmpty(text) || progress.CancellationToken.IsCancellationRequested)
            {
                return new List<ParsingResult>();
            }

            var handHistoryParserFactory = ServiceLocator.Current.GetInstance<IHandHistoryParserFactory>();

            IHandHistoryParser handHistoryParser;

            // get suitable parser for specified hh
            if (gameInfo != null && gameInfo.PokerSite != EnumPokerSites.Unknown)
            {
                handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser(gameInfo.PokerSite);
            }
            else
            {
                handHistoryParser = handHistoryParserFactory.GetFullHandHistoryParser(text);

                gameInfo = new GameInfo
                {
                    PokerSite = handHistoryParserFactory.LastSelected,
                    FileName = gameInfo?.FileName
                };
            }

            try
            {
                var hands = handHistoryParser.SplitUpMultipleHands(text).ToArray();

                var parsingResult = ParseHands(hands, handHistoryParser, gameInfo);

                if (gameInfo.UpdateAction != null && parsingResult.Count > 0)
                {
                    gameInfo.UpdateAction(parsingResult, gameInfo);
                }

#if DEBUG
                using (var perfomanceScope = new PerformanceMonitor($"Insert hand({gameInfo.GameNumber})"))
                {
#endif
                    InsertHands(parsingResult, progress, gameInfo);
#if DEBUG
                }
#endif

                return parsingResult;
            }
            catch (Exception e)
            {
                var logger = ServiceLocator.Current.GetInstance<IFileImporterLogger>();
                logger.Log(text);

                throw new DHInternalException(new NonLocalizableString("Could not import hand."), e);
            }
        }

        /// <summary>
        /// Parse hands with specified parser
        /// </summary>
        /// <param name="hands">Hands array</param>
        /// <param name="handHistoryParserser">Parser for parsing</param>
        /// <returns>Result of parsing</returns>
        private List<ParsingResult> ParseHands(string[] hands, IHandHistoryParser handHistoryParserser, GameInfo gameInfo)
        {
            Check.ArgumentNotNull(() => hands);

            var parsingResult = new List<ParsingResult>();

            for (int i = 0; i < hands.Length; i++)
            {
                var parsingHandResult = ParseHand(hands[i], handHistoryParserser, gameInfo);

                if (parsingHandResult == null)
                {
                    continue;
                }

                parsingResult.Add(parsingHandResult);
            }

            return parsingResult;
        }

        /// <summary>
        /// Parse hands with specified parser
        /// </summary>
        /// <param name="hand">Hand to be parsed</param>
        /// <param name="handHistoryParserser">Parser for parsing</param>
        /// <returns>Result of parsing</returns>
        /// <returns></returns>
        private ParsingResult ParseHand(string hand, IHandHistoryParser handHistoryParser, GameInfo gameInfo)
        {
            Check.ArgumentNotNull(() => handHistoryParser);

            var parsedHand = handHistoryParser.ParseFullHandHistory(hand, true);

            if (parsedHand == null)
            {
                return null;
            }

            if (parsedHand.GameDescription.IsTournament)
            {
                if (parsedHand.GameDescription.Tournament.IsSummary)
                {
                    return new ParsingResult
                    {
                        Source = parsedHand
                    };
                }

                if (string.IsNullOrEmpty(parsedHand.GameDescription.Tournament.TournamentId))
                {
                    parsedHand.GameDescription.Tournament.TournamentId = handHistoryParser.GetTournamentIdFromFileName(gameInfo?.FileName);
                }
            }

            var pokerSiteId = gameInfo != null ? (short)gameInfo.PokerSite : (short)EnumPokerSites.IPoker;

            var handHistory = new Handhistory
            {
                Gamenumber = parsedHand.HandId,
                GametypeId = (int)parsedHand.GameDescription.GameType,
                Handtimestamp = parsedHand.DateOfHandUtc,
                HandhistoryVal = parsedHand.FullHandHistoryText,
                PokersiteId = pokerSiteId,
                Tourneynumber = parsedHand.GameDescription.IsTournament ? parsedHand.GameDescription.Tournament.TournamentId : string.Empty
            };

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
                PokersiteId = pokerSiteId
            }).ToList();

            var parsingResult = new ParsingResult
            {
                HandHistory = handHistory,
                Players = players,
                GameType = gameType,
                Source = parsedHand
            };

            var matchInfo = new GameMatchInfo
            {
                GameType = parsedHand.GameDescription.GameType,
                CashBuyIn = !parsedHand.GameDescription.IsTournament ? gameType.Bigblindincents : 0,
                TournamentBuyIn = parsedHand.GameDescription.IsTournament ? parsedHand.GameDescription.Tournament.BuyIn.PrizePoolValue : 0
            };

            var sessionService = ServiceLocator.Current.GetInstance<ISessionService>();
            var userSession = sessionService.GetUserSession();

            if (!userSession.IsMatch(matchInfo))
            {
                throw new DHBusinessException(new NonLocalizableString("License doesn't support hand."));
            }

            return parsingResult;
        }

        /// <summary>
        /// Insert hands in DB (to do: create and use repository from model, not here)
        /// </summary>
        /// <param name="parsingResult">Parsing result</param>
        /// <param name="progress">Progress reporter</param>
        /// <param name="importerSession">Session code</param>
        private void InsertHands(List<ParsingResult> parsingResult, IDHProgress progress, GameInfo gameInfo)
        {
            Check.ArgumentNotNull(() => parsingResult);

            var duplicates = 0;

            var importerSession = gameInfo != null ? gameInfo.Session : string.Empty;

            // need to lock db operations to prevent deadlocks
            lock (locker)
            {
                try
                {
                    parsingResult = parsingResult.ToList();

                    using (var session = ModelEntities.OpenStatelessSession())
                    {
                        using (var transaction = session.BeginTransaction())
                        {
                            var tournamentsData = new List<Tournaments>();

                            var existingGames = GetExisting(session, parsingResult.Where(x => !x.IsSummary));
                            var existingPlayers = ComposePlayers(session, parsingResult.Where(x => !x.IsSummary), gameInfo);

                            for (var i = 0; i < parsingResult.Count; i++)
                            {
                                var handHistory = parsingResult[i];

                                // skip error hand
                                if (handHistory.Source.HasError)
                                {
                                    continue;
                                }

                                progress.Report(new LocalizableString("Progress_UpdatingData", i + 1, parsingResult.Count, duplicates));

                                // update tournament with summary data
                                if (handHistory.IsSummary)
                                {
                                    InsertSummaryHand(session, handHistory, gameInfo);
                                    continue;
                                }

                                // Check if this game was already parsed before
                                var exist = existingGames.Any(x => x.Item1 == handHistory.HandHistory.Gamenumber && x.Item2 == handHistory.HandHistory.PokersiteId);

                                if (exist)
                                {
                                    duplicates++;
                                    handHistory.IsDuplicate = true;
                                    continue;
                                }

                                existingGames.Add(new Tuple<long, short>(handHistory.HandHistory.Gamenumber, handHistory.HandHistory.PokersiteId));

                                InsertRegularHand(session, handHistory, existingPlayers, importerSession, tournamentsData, gameInfo);

                                if (progress.CancellationToken.IsCancellationRequested)
                                {
                                    progress.Report(new LocalizableString("Progress_StoppingImport"));
                                    break;
                                }
                            }

                            ProcessTournaments(tournamentsData, parsingResult, session);

                            transaction.Commit();

                            parsingResult.ForEach(p => p.WasImported = true);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "DB error", e);
                }
            }
        }

        private void InsertRegularHand(IStatelessSession session, ParsingResult handHistory, IEnumerable<Players> existingPlayers, string importerSession, List<Tournaments> tournamentsData, GameInfo gameInfo)
        {
            try
            {
                var existingGameType = SaveGameType(session, handHistory);

                session.Insert(handHistory.HandHistory);

                // join new players with existing
                var handPlayers = existingPlayers.Where(e => handHistory.Players.Any(h => h.Playername == e.Playername && h.PokersiteId == e.PokersiteId));

                foreach (var existingPlayer in handPlayers.ToArray())
                {
                    if (existingGameType.Istourney)
                    {
                        existingPlayer.Tourneyhands++;
                    }
                    else
                    {
                        existingPlayer.Cashhands++;
                    }

                    session.Update(existingPlayer);

                    var playerStat = ProcessPlayerStatistic(handHistory, existingPlayer, importerSession);

                    if (playerStat != null)
                    {
                        // copy player stat to prevent having the same reference in different caches
                        var playerStatCopy = playerStat.Copy();

                        var isHero = handHistory.Source.Hero != null ? handHistory.Source.Hero.PlayerName.Equals(existingPlayer.Playername) : false;

                        playerStatCopy.SessionCode = importerSession;

                        var cacheInfo = new PlayerStatsSessionCacheInfo
                        {
                            Session = importerSession,
                            Player = new PlayerCollectionItem
                            {
                                PlayerId = existingPlayer.PlayerId,
                                Name = existingPlayer.Playername,
                                PokerSite = (EnumPokerSites)existingPlayer.PokersiteId
                            },
                            Stats = playerStatCopy,
                            IsHero = isHero
                        };

                        importSessionCacheService.AddOrUpdatePlayerStats(cacheInfo);

                        var hh = Converter.ToHandHistoryRecord(handHistory.Source, playerStat);

                        if (hh != null)
                        {
                            hh.GameType = existingGameType;
                            hh.Player = existingPlayer;
                            session.Insert(hh);
                        }
                    }

                    #region Process tournament data

                    if (handHistory.GameType.Istourney)
                    {
                        // get all existing data for that tournament
                        if (tournamentsData == null)
                        {
                            // this shouldn't be possible
                            LogProvider.Log.Warn(this, "tournamentsData is null");
                            continue;
                        }

                        if (tournamentsData.Count == 0)
                        {
                            tournamentsData.AddRange(session.Query<Tournaments>().Where(x => x.Tourneynumber == handHistory.Source.GameDescription.Tournament.TournamentId && x.SiteId == handHistory.HandHistory.PokersiteId).Fetch(x => x.Player).ToList());
                        }

                        var existingTournament = tournamentsData.FirstOrDefault(x =>
                            x.Tourneynumber == handHistory.HandHistory.Tourneynumber && x.Player.PlayerId == existingPlayer.PlayerId);

                        if (existingTournament == null)
                        {
                            var tournaments = CreateTournaments(handHistory, existingPlayer, gameInfo);

                            if (gameInfo != null)
                            {
                                tournaments.SiteId = (short)gameInfo.PokerSite;
                            }

                            tournaments.Player = existingPlayer;

                            session.Insert(tournaments);

                            tournamentsData.Add(tournaments);
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, string.Format("Hand #{0} has not been inserted due the following error", handHistory.HandHistory.HandhistoryId), ex);
            }
        }

        private void InsertSummaryHand(IStatelessSession session, ParsingResult handHistory, GameInfo gameInfo)
        {
            try
            {
                var tournamentDescription = handHistory.Source.GameDescription.Tournament;

                var tournamentsData = session.Query<Tournaments>().Where(x => x.Tourneynumber == tournamentDescription.TournamentId &&
                        x.SiteId == (short)handHistory.Source.GameDescription.Site).Fetch(x => x.Player).ToList();

                tournamentsData.ForEach(t =>
                {
                    if (handHistory.Source.Hero != null && t.PlayerName.Equals(handHistory.Source.Hero.PlayerName))
                    {
                        t.Winningsincents = Utils.ConvertToCents(tournamentDescription.Winning);
                        t.Finishposition = tournamentDescription.FinishPosition;
                    }

                    t.Buyinincents = Utils.ConvertToCents(tournamentDescription.BuyIn.PrizePoolValue);
                    t.Rakeincents = Utils.ConvertToCents(tournamentDescription.BuyIn.Rake);
                    t.Rebuyamountincents = Utils.ConvertToCents(tournamentDescription.Rebuy);
                    t.Tourneysize = tournamentDescription.TotalPlayers;
                    t.Tourneytagscsv = tournamentDescription.TotalPlayers > t.Tablesize ? TournamentsTags.MTT.ToString() : TournamentsTags.STT.ToString();

                    session.Update(t);
                });
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, string.Format("Summary hand data #{0} has not been inserted due the following error", handHistory.Source.GameDescription.Tournament.TournamentId), ex);
            }
        }

        /// <summary>
        /// Calculate player statistic and store it in cache
        /// </summary>
        /// <param name="handHistory">Hand history</param>
        /// <param name="player">Player</param>
        /// <returns>Calculated player statistic</returns>
        public Playerstatistic ProcessPlayerStatistic(ParsingResult handHistory, Players player, string session)
        {
            try
            {
                var playerStatisticCalculator = ServiceLocator.Current.GetInstance<IPlayerStatisticCalculator>();

                var playerStat = playerStatisticCalculator.CalculateStatistic(handHistory, player);

                if (playerStat == null)
                {
                    return null;
                }

                playerStat.SessionCode = session;

                StorePlayerStatistic(playerStat, session);

                return playerStat;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(e);
                return null;
            }
        }

        /// <summary>
        /// Stores player statistics
        /// </summary>
        /// <param name="playerStat"></param>
        /// <param name="session"></param>
        protected virtual void StorePlayerStatistic(Playerstatistic playerStat, string session)
        {
            if (string.IsNullOrEmpty(session))
            {
                Task.Run(() => dataService.Store(playerStat));
                return;
            }

            dataService.Store(playerStat);
        }

        /// <summary>
        /// Create tournament-player entity
        /// </summary>
        /// <param name="parsingResult">Hand parsing data</param>
        /// <param name="player">Player</param>
        /// <returns></returns>
        private Tournaments CreateTournaments(ParsingResult parsingResult, Players player, GameInfo gameInfo)
        {
            var parsedHand = parsingResult.Source;

            var tournaments = new Tournaments
            {
                Buyinincents = Utils.ConvertToCents(parsedHand.GameDescription.Tournament.BuyIn.PrizePoolValue),
                CurrencyId = (short)parsedHand.GameDescription.Limit.Currency,
                Filelastmodifiedtime = processingFile != null ? processingFile.LastWriteTime : DateTime.MinValue,
                Filename = processingFile != null ? processingFile.FullName : string.Empty,
                Tourneynumber = parsedHand.GameDescription.Tournament.TournamentId,
                Rakeincents = Utils.ConvertToCents(parsedHand.GameDescription.Tournament.BuyIn.Rake),
                Tourneytagscsv = string.Empty,
                SiteId = parsingResult.HandHistory.PokersiteId,
                SpeedtypeId = gameInfo.TournamentSpeed.HasValue ? (short)gameInfo.TournamentSpeed.Value : (short)parsedHand.GameDescription.Tournament.Speed,
                PokergametypeId = (short)parsedHand.GameDescription.GameType,
            };

            return tournaments;
        }

        /// <summary>
        /// Checks which hands exist in the db from the specified list
        /// </summary>
        /// <param name="session">DB session</param>
        /// <param name="handHistories">Collection of hand histories</param>
        /// <returns>Collection of existing hands. Item1 - hand number, item2 - pokersite id</returns>
        private List<Tuple<long, short>> GetExisting(IStatelessSession session, IEnumerable<ParsingResult> handHistories)
        {
            var hhGroupedByPokersite = handHistories.GroupBy(x => x.HandHistory.PokersiteId);

            Disjunction restriction = Restrictions.Disjunction();
            foreach (var pokersiteGroup in hhGroupedByPokersite)
            {
                restriction.Add(Restrictions.Conjunction()
                     .Add(Restrictions.On<Handhistory>(x => x.Gamenumber).IsIn(pokersiteGroup.Select(x => x.HandHistory.Gamenumber).ToList()))
                     .Add(Restrictions.Where<Handhistory>(x => x.PokersiteId == pokersiteGroup.Key)));
            }

            var list = session.QueryOver<Handhistory>().Where(restriction)
                    .Select(x => x.Gamenumber, x => x.PokersiteId)
                    .List<object[]>()
                    .Select(x => new Tuple<long, short>((long)x[0], (short)x[1]))
                    .ToList();

            return list;
        }

        private IList<Players> ComposePlayers(IStatelessSession session, IEnumerable<ParsingResult> handHistories, GameInfo gameInfo)
        {
            var playersToSelect = handHistories
                .SelectMany(x => x.Players)
                .GroupBy(x => new { x.Playername, x.PokersiteId })
                .Select(x => x.FirstOrDefault())
                .Where(x => x != null).ToList();

            var playersGroupedByPokersite = playersToSelect.GroupBy(x => x.PokersiteId);

            Disjunction restriction = Restrictions.Disjunction();

            foreach (var pokersiteGroup in playersGroupedByPokersite)
            {
                restriction.Add(Restrictions.Conjunction()
                     .Add(Restrictions.On<Players>(x => x.Playername).IsIn(pokersiteGroup.Select(x => x.Playername).ToList()))
                     .Add(Restrictions.Where<Players>(x => x.PokersiteId == pokersiteGroup.Key)));
            }

            var existingPlayers = session.QueryOver<Players>().Where(restriction).List();

            var playersToAdd = playersToSelect.Where(s => !existingPlayers.Any(e => s.Playername == e.Playername
                                                                                && s.PokersiteId == e.PokersiteId));

            foreach (var player in playersToAdd)
            {
                var inserted = session.Insert(player);
            }

            var playerItemCollection = playersToAdd.Select(x =>
                new PlayerCollectionItem()
                {
                    PlayerId = x.PlayerId,
                    Name = x.Playername,
                    PokerSite = (EnumPokerSites)x.PokersiteId
                }).ToArray();

            dataService.AddPlayerRangeToList(playerItemCollection);
            gameInfo.AddedPlayers = playerItemCollection;

            existingPlayers.AddRange(playersToAdd);

            // update id for future use
            var playersToUpdate = (from handPlayer in existingPlayers
                                   join handHistoryPlayer in handHistories.SelectMany(x => x.Source.Players) on handPlayer.Playername equals handHistoryPlayer.PlayerName
                                   select new { Player = handPlayer, HandHistoryPlayer = handHistoryPlayer }).ToArray();

            playersToUpdate.ForEach(x =>
            {
                x.HandHistoryPlayer.PlayerId = x.Player.PlayerId;
            });

            return existingPlayers;
        }

        /// <summary>
        /// Save game type (to do: move to model)
        /// </summary>
        /// <param name="session"></param>
        /// <param name="handhistory"></param>
        /// <returns></returns>
        private Gametypes SaveGameType(IStatelessSession session, ParsingResult handhistory)
        {
            var existingGameType = session.Query<Gametypes>().FirstOrDefault(x =>
                x.Bigblindincents == handhistory.GameType.Bigblindincents &&
                x.Anteincents == handhistory.GameType.Anteincents &&
                x.CurrencytypeId == handhistory.GameType.CurrencytypeId &&
                x.PokergametypeId == handhistory.GameType.PokergametypeId &&
                x.Smallblindincents == handhistory.GameType.Smallblindincents &&
                x.Tablesize == handhistory.GameType.Tablesize &&
                x.Istourney == handhistory.GameType.Istourney);

            if (existingGameType == null)
            {
                session.Insert(handhistory.GameType);
                existingGameType = handhistory.GameType;
            }
            else
            {
                handhistory.GameType = existingGameType;
            }

            handhistory.HandHistory.GametypeId = existingGameType.GametypeId;

            return existingGameType;
        }

        /// <summary>
        /// Processes tournaments data
        /// </summary>
        /// <param name="tournaments">List of tournaments</param>
        /// <param name="parsingResult">Results of parsing of hh </param>
        /// <param name="session">DB session</param>
        private void ProcessTournaments(List<Tournaments> tournaments, List<ParsingResult> parsingResult, IStatelessSession session)
        {
            // if hh file contains data about several tournaments we need to group them
            var tournamentsDataGrouped = tournaments
                .GroupBy(x => x.Tourneynumber)
                .Select(x => new { TournamentId = x.Key, Tournaments = x.ToList() })
                .ToDictionary(x => x.TournamentId, x => x.Tournaments);

            var parsingResultGrouped = parsingResult
                .Where(x => x.Source.GameDescription != null && x.Source.GameDescription.IsTournament)
                .GroupBy(x => x.Source.GameDescription.Tournament.TournamentId)
                .Select(x => new { TournamentId = x.Key, Tournaments = x.ToList() })
                .ToDictionary(x => x.TournamentId, x => x.Tournaments);

            foreach (var tournamentData in tournamentsDataGrouped)
            {
                // that should never happen, but who knows :)
                if (!parsingResultGrouped.ContainsKey(tournamentData.Key))
                {
                    LogProvider.Log.Error(this, $"Inconsistent data. Couldn't find '{tournamentData.Key}' in parsing results. Tournament processing has been skipped.");
                    continue;
                }

                var tournamentParsingResult = parsingResultGrouped[tournamentData.Key];
                ProcessTournamentData(tournamentData.Value, tournamentParsingResult, session);
            }
        }

        /// <summary>
        /// Process tournament data
        /// </summary>
        /// <param name="tournaments">List of tournaments</param>
        /// <param name="parsingResult">Results of parsing of hh </param>
        /// <param name="session">DB session</param>
        private void ProcessTournamentData(List<Tournaments> tournaments, List<ParsingResult> parsingResult, IStatelessSession session)
        {
            if (tournaments == null || tournaments.Count < 1 || session == null)
            {
                return;
            }

            var totalPlayers = tournaments.Select(x => x.Player.PlayerId).Count();

            // max players among all new hands and existing
            var tableSize = parsingResult.Select(x => (short)x.Source.GameDescription.SeatType.MaxPlayers).Concat(tournaments.Select(x => x.Tablesize)).Max();

            if (tableSize == 0)
            {
                throw new DHBusinessException(new NonLocalizableString("Table size is 0"));
            }

            var tournamentTag = totalPlayers > tableSize ? TournamentsTags.MTT : TournamentsTags.STT;

            parsingResult.ForEach(x => x.TournamentsTags = tournamentTag);

            var tournamentTables = (short)Math.Ceiling((double)totalPlayers / tableSize);

            var tournamentBase = tournaments.FirstOrDefault();

            var firstParsingResult = parsingResult.FirstOrDefault();
            var lastParsingResult = parsingResult.LastOrDefault();

            var tournamentName = firstParsingResult.Source.GameDescription.Tournament.TournamentName;

            var initialStackSize = (tournamentBase.Startingstacksizeinchips != 0) ? tournamentBase.Startingstacksizeinchips : GetInitialStackSize(tournamentName, parsingResult);

            // get hands grouped by player name
            var handsByPlayer = (from parsingRes in parsingResult
                                 let handNumber = parsingRes.Source.HandId
                                 from player in parsingRes.Source.Players
                                 group new { IsLost = player.IsLost, HandNumber = handNumber, InitialStackSize = player.StartingStack } by player.PlayerName into grouped
                                 select new { Name = grouped.Key, Hands = grouped.ToArray() }).ToArray();

            // get hand where the player lost 
            var lastHandsByPlayer = (from player in handsByPlayer
                                     let lastHand = player.Hands.LastOrDefault()
                                     where lastHand != null && lastHand.IsLost
                                     select new { HandNumber = lastHand.HandNumber, PlayerName = player.Name, InitialStackSize = lastHand.InitialStackSize }).ToArray();

            var currentPosition = firstParsingResult.Players.Count;

            var tournamentsByPlayer = tournaments.ToDictionary(x => x.PlayerName, x => x);

            // get end position in tournament
            foreach (var lastHandByPlayer in lastHandsByPlayer.OrderBy(x => x.HandNumber).ThenBy(x => x.InitialStackSize))
            {
                if (!tournamentsByPlayer.ContainsKey(lastHandByPlayer.PlayerName))
                {
                    continue;
                }

                var isHero = lastParsingResult.Source.Hero != null && lastParsingResult.Source.Hero.PlayerName != null ?
                    lastParsingResult.Source.Hero.PlayerName.Equals(lastHandByPlayer.PlayerName) :
                    false;

                tournamentsByPlayer[lastHandByPlayer.PlayerName].Winningsincents = isHero && lastParsingResult.Source.GameDescription.Tournament.Winning != 0 ?
                    Utils.ConvertToCents(lastParsingResult.Source.GameDescription.Tournament.Winning) :
                    GetTournamentWinnings(tournamentName, currentPosition, tournamentBase.Buyinincents, totalPlayers, lastParsingResult.Source.GameDescription.Tournament.BuyIn.Currency, tournamentBase.SiteId);

                tournamentsByPlayer[lastHandByPlayer.PlayerName].Finishposition = isHero && lastParsingResult.Source.GameDescription.Tournament.FinishPosition != 0 ?
                    lastParsingResult.Source.GameDescription.Tournament.FinishPosition :
                    currentPosition;

                currentPosition--;

                tournamentsByPlayer[lastHandByPlayer.PlayerName].Tourneyendedforplayer = true;
            }

            var numberOfWinners = GetNumberOfWinnersForTournament(tournamentName, totalPlayers, tournamentTag);

            if (currentPosition <= numberOfWinners)
            {
                foreach (var winnerPlayer in tournaments.Where(x => x != null && x.Finishposition == 0).Take(numberOfWinners))
                {
                    winnerPlayer.Finishposition = 1;

                    var isHero = lastParsingResult.Source.Hero.PlayerName.Equals(winnerPlayer.PlayerName);

                    if (isHero && lastParsingResult.Source.GameDescription.Tournament.Winning != 0)
                    {
                        winnerPlayer.Winningsincents = Utils.ConvertToCents(lastParsingResult.Source.GameDescription.Tournament.Winning);
                    }
                    else
                    {
                        winnerPlayer.Winningsincents = GetTournamentWinnings(tournamentName, 1, tournamentBase.Buyinincents, totalPlayers, lastParsingResult.Source.GameDescription.Tournament.BuyIn.Currency, tournamentBase.SiteId);
                    }

                    winnerPlayer.Tourneyendedforplayer = true;
                }
            }

            // update and save all
            foreach (var tournament in tournaments)
            {
                tournament.Tourneytagscsv = tournamentTag.ToString();
                tournament.Startingstacksizeinchips = (short)initialStackSize;
                tournament.Tourneysize = totalPlayers;
                tournament.Tourneytables = tournamentTables;
                tournament.Tablesize = tableSize;

                if (lastParsingResult != null && lastParsingResult.HandHistory != null && lastParsingResult.HandHistory.Handtimestamp.HasValue)
                {
                    tournament.Lasthandtimestamp = lastParsingResult.HandHistory.Handtimestamp.Value;
                }

                if (tournament.Firsthandtimestamp == DateTime.MinValue)
                {
                    tournament.Firsthandtimestamp = tournamentBase.Firsthandtimestamp != DateTime.MinValue ?
                                                        tournamentBase.Firsthandtimestamp :
                                                        (firstParsingResult.HandHistory.Handtimestamp.HasValue ?
                                                            firstParsingResult.HandHistory.Handtimestamp.Value :
                                                            firstParsingResult.Source.GameDescription.Tournament.StartDate);
                }

                session.Update(tournament);
            }
        }

        private int GetNumberOfWinnersForTournament(string tournamentName, int totalPlayers, TournamentsTags tournamentTag)
        {
            int numberOfWinningPlaces = 1;
            if (tournamentTag == TournamentsTags.STT)
            {
                var sttType = Converter.ToSitNGoType(tournamentName);
                switch (sttType)
                {
                    case STTTypes.DoubleUp:
                        numberOfWinningPlaces = (totalPlayers / 2);
                        break;
                    case STTTypes.TripleUp:
                        numberOfWinningPlaces = (totalPlayers / 3);
                        break;
                }
            }

            return numberOfWinningPlaces;
        }

        /// <summary>
        /// Get initial tournament stack size
        /// </summary>
        /// <returns>Stack size in chips</returns>
        private int GetInitialStackSize(string tournamentName, List<ParsingResult> parsingResult)
        {
            // try to get stack size for specific tournament
            var stackSize = TournamentsResolver.GetInitialStackSizeByName(tournamentName);

            if (stackSize > 0)
            {
                return stackSize;
            }

            // get initial stack size for generic tournament
            var firstResult = parsingResult.FirstOrDefault();

            if (firstResult == null || firstResult.Source == null || firstResult.Source.Players == null)
            {
                return TournamentSettings.DefaultInitialStackSize;
            }

            var startingStacksSum = firstResult.Source.Players.Select(x => x.StartingStack).Sum();

            var stackSizePerPlayer = startingStacksSum / firstResult.Source.Players.Count;

            var possibleStackSizes = TournamentsResolver.GetPossibleInitialeStacksSizes();

            if (possibleStackSizes.Contains(stackSizePerPlayer))
            {
                return (int)stackSizePerPlayer;
            }

            if (firstResult.Source.Hero != null && possibleStackSizes.Contains(firstResult.Source.Hero.StartingStack))
            {
                return (int)firstResult.Source.Hero.StartingStack;
            }

            return TournamentSettings.DefaultInitialStackSize;
        }

        /// <summary>
        /// Gets player's winnings amount for the tournament
        /// </summary>
        /// <returns>Winnings in cents</returns>
        private int GetTournamentWinnings(string tournamentName, int finishPosition, int buyinInCents, int totalPlayers, Currency currency, short siteId, TournamentsTags tournamentTag = TournamentsTags.STT)
        {
            // try to find tournament in the predefined tournaments 
            var predefinedTournament = TournamentsResolver.GetPredefinedTournament(tournamentName, buyinInCents, totalPlayers, currency);
            if (predefinedTournament != null)
            {
                return predefinedTournament.Prizes.FirstOrDefault(x => x.Place == finishPosition)?.WinningsInCents ?? 0;
            }

            if (tournamentTag == TournamentsTags.STT)
            {
                var sttType = Converter.ToSitNGoType(tournamentName);
                switch (sttType)
                {
                    case STTTypes.DoubleUp:
                        return (finishPosition <= (totalPlayers / 2)) ? buyinInCents * 2 : 0;
                    case STTTypes.TripleUp:
                        return (finishPosition <= (totalPlayers / 3)) ? buyinInCents * 3 : 0;
                }

                var prizePool = buyinInCents * totalPlayers;

                var prizeRatesDictionary = TournamentSettings.GetWinningsMultiplier((EnumPokerSites)siteId, sttType == STTTypes.Beginner);

                if (prizeRatesDictionary == null || !prizeRatesDictionary.ContainsKey(totalPlayers))
                {
                    return 0;
                }

                var prizeRates = prizeRatesDictionary[totalPlayers];

                if (finishPosition <= prizeRates.Count())
                {
                    return (int)(prizePool * prizeRates.ElementAtOrDefault(finishPosition - 1));
                }
            }

            return 0;
        }
    }
}
