//-----------------------------------------------------------------------
// <copyright file="PKImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.Helpers;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.PokerKing.Model;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NHibernate.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using PacketDotNet;
using ProtoBuf;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerKing
{
    internal class PKImporter : TcpPacketImporter, IPKImporter
    {
        private readonly BlockingCollection<CapturedPacket> packetBuffer = new BlockingCollection<CapturedPacket>();
        private readonly IPKCatcherService pkCatcherService = ServiceLocator.Current.TryResolve<IPKCatcherService>();
        protected IPokerClientEncryptedLogger protectedLogger;

        protected override bool SupportDuplicates => true;

        protected override EnumPokerSites Site => EnumPokerSites.PokerKing;

        protected override string Logger => CustomModulesNames.PKCatcher;

        protected ConcurrentDictionary<uint, string> userTokens = new ConcurrentDictionary<uint, string>();

#if DEBUG
        protected override string HandHistoryFilePrefix => "pk";
#endif

        protected override string[] ProcessNames => throw new NotSupportedException($"Process name isn't supported for importer. [{SiteString}]");

        #region Implementation of ITcpImporter

        public override bool Match(TcpPacket tcpPacket, IPPacket ipPacket)
        {
            return PKImporterHelper.IsPortMatch(tcpPacket.SourcePort) ||
                PKImporterHelper.IsPortMatch(tcpPacket.DestinationPort);
        }

        public override void AddPacket(CapturedPacket capturedPacket)
        {
            packetBuffer.Add(capturedPacket);
        }

        public override bool IsDisabled()
        {
            return pkCatcherService == null || !pkCatcherService.IsEnabled();
        }

        #endregion

        /// <summary>
        /// Main importer task, executes in the background thread
        /// </summary>
        protected override void DoImport()
        {
            try
            {
                InitializeLogger();
                InitializeSettings();
                ProcessBuffer();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, $"Failed importing.", e);
            }

            packetBuffer.Clear();
            userTokens.Clear();

            protectedLogger.StopLogging();

            RaiseProcessStopped();
        }

        /// <summary>
        /// Initializes logger with protection
        /// </summary>
        protected virtual void InitializeLogger()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "pk-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "pk-games-{0}.log",
                MessagesInBuffer = 10
            };

            protectedLogger = ServiceLocator.Current.GetInstance<IPokerClientEncryptedLogger>(LogServices.Base.ToString());
            protectedLogger.Initialize(logger);
            protectedLogger.CleanLogs();
            protectedLogger.StartLogging();
        }

        /// <summary>
        /// Reads settings and initializes importer variables
        /// </summary>
        protected virtual void InitializeSettings()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            if (settings != null)
            {
                IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
            }
        }

        /// <summary>
        /// Processes packets in the buffer
        /// </summary>
        protected void ProcessBuffer()
        {
            var emulatorService = ServiceLocator.Current.GetInstance<IEmulatorService>();
            emulatorService.SetLogger(Logger);

            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager<PokerKingPackage>>();
            var handBuilder = ServiceLocator.Current.GetInstance<IPKHandBuilder>();

            var connectionsService = ServiceLocator.Current.GetInstance<INetworkConnectionsService>();
            connectionsService.SetLogger(Logger);

            var importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();

            var detectedTableWindows = new HashSet<IntPtr>();

            var handHistoriesToProcess = new ConcurrentDictionary<long, List<HandHistoryData>>();

            var usersRooms = new Dictionary<uint, int>();

            var playerNamePlayerIdMap = new Dictionary<string, int>();

            while (!cancellationTokenSource.IsCancellationRequested && !IsDisabled())
            {
                try
                {
                    if (!packetBuffer.TryTake(out CapturedPacket capturedPacket))
                    {
                        Task.Delay(NoDataDelay).Wait();
                        continue;
                    }

                    if (IsAdvancedLogEnabled)
                    {
                        LogPacket(capturedPacket, ".log");
                    }

                    if (!packetManager.TryParse(capturedPacket, out IList<PokerKingPackage> packages))
                    {
                        continue;
                    }

                    var isFastFold = PKImporterHelper.IsFastFoldPort(capturedPacket.Destination.Port) ||
                        PKImporterHelper.IsFastFoldPort(capturedPacket.Source.Port);

                    foreach (var package in packages)
                    {
                        package.IsFastFold = isFastFold;

                        if (!IsAllowedPackage(package))
                        {
                            continue;
                        }

                        var process = connectionsService.GetProcess(capturedPacket);
                        var windowHandle = emulatorService.GetTableWindowHandle(process);

                        if (!isFastFold && !TryDecryptBody(package, process, emulatorService))
                        {
                            if (package.PackageType == PackageType.RequestLeaveRoom)
                            {
                                CloseHUD(windowHandle);

                                detectedTableWindows.Remove(windowHandle);
                                continue;
                            }

                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PK_Relogin", windowHandle);
                            continue;
                        }

                        if (IsAdvancedLogEnabled)
                        {
                            LogPackage(package);
                        }

                        if (package.PackageType == PackageType.RequestJoinRoom)
                        {
                            ParsePackage<RequestJoinRoom>(package,
                              body =>
                              {
                                  if (!usersRooms.ContainsKey(package.UserId))
                                  {
                                      usersRooms.Add(package.UserId, body.RoomId);
                                  }
                                  else
                                  {
                                      usersRooms[package.UserId] = body.RoomId;
                                  }

                                  LogProvider.Log.Info(Logger, $"User {package.UserId} entered room {body.RoomId}{(isFastFold ? " (Fast Fold)" : string.Empty)}.");
                              },
                              () => LogProvider.Log.Info(Logger, $"User {package.UserId} entered room{(isFastFold ? " (Fast Fold)" : string.Empty)}."));

                            continue;
                        }

                        // need to close HUD if user left room
                        if (package.PackageType == PackageType.RequestLeaveRoom)
                        {
                            ParsePackage<RequestLeaveRoom>(package,
                                body =>
                                {
                                    LogProvider.Log.Info(Logger, $"User {package.UserId} left room {body.RoomId}.");
                                    handBuilder.CleanRoom(windowHandle.ToInt32(), body.RoomId);

                                },
                                () => LogProvider.Log.Info(Logger, $"User {package.UserId} left room {package.RoomId}."));

                            CloseHUD(windowHandle);

                            detectedTableWindows.Remove(windowHandle);
                            continue;
                        }

                        if (isFastFold && package.PackageType == PackageType.NoticeQuickLeave)
                        {
                            ParsePackage<NoticeQuickLeave>(package,
                                body =>
                                {
                                    handBuilder.CleanFastFoldRooms(package, windowHandle.ToInt32(), out List<HandHistory> handHistories);

                                    foreach (var fastFoldHandHistory in handHistories)
                                    {
                                        LogProvider.Log.Info(Logger,
                                            $"Hand #{fastFoldHandHistory.HandId} user #{package.UserId} room #{package.RoomId}: Process={(process != null ? process.Id : 0)}, windows={windowHandle}.");

                                        ExportHandHistory(package, fastFoldHandHistory, windowHandle, handHistoriesToProcess, false);
                                    }

                                    LogProvider.Log.Info(Logger, $"User {package.UserId} left room {body.RoomId} (Fast Fold).");
                                },
                                () => LogProvider.Log.Info(Logger, $"User {package.UserId} left room {package.RoomId} (Fast Fold)."));

                            CloseHUD(windowHandle);

                            detectedTableWindows.Remove(windowHandle);
                            continue;
                        }

                        var unexpectedRoomDetected = !usersRooms.TryGetValue(package.UserId, out int roomId) || roomId != package.RoomId;

                        // add detected window to list of detected tables
                        if (!unexpectedRoomDetected && !detectedTableWindows.Contains(windowHandle))
                        {
                            detectedTableWindows.Add(windowHandle);

                            if (package.PackageType == PackageType.NoticeGameSnapShot || handBuilder.IsRoomSnapShotAvailable(package))
                            {
                                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PK", windowHandle);
                            }
                            else
                            {
                                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PK_CanNotBeCapturedText", windowHandle);
                            }
                        }

                        if (isFastFold && unexpectedRoomDetected &&
                            package.PackageType == PackageType.NoticeResetGame)
                        {
                            ParsePackage<NoticeResetGame>(package,
                                noticeResetGame =>
                                {
                                    var fastFoldImportDto = new FastFoldImportDto
                                    {
                                        HandBuilder = handBuilder,
                                        ImporterSessionCacheService = importerSessionCacheService,
                                        NoticeResetGame = noticeResetGame,
                                        Package = package,
                                        PlayerNamePlayerIdMap = playerNamePlayerIdMap,
                                        WindowHandle = windowHandle
                                    };

                                    ProcessFastFoldNoticeGameReset(fastFoldImportDto);
                                },
                                () => LogProvider.Log.Info(Logger, $"Failed to process NoticeResetGame for {package.UserId} of {package.RoomId} (Fast Fold)."));
                        }

                        if (handBuilder.TryBuild(package, windowHandle.ToInt32(), out HandHistory handHistory))
                        {
                            if (IsAdvancedLogEnabled)
                            {
                                var unexpectedRoomLogMessage = string.Empty;

                                if (unexpectedRoomDetected && !isFastFold)
                                {
                                    unexpectedRoomLogMessage = " Unexpected room detected. No data will be sent to HUD.";
                                }

                                LogProvider.Log.Info(Logger,
                                    $"Hand #{handHistory.HandId} user #{package.UserId} room #{package.RoomId}: Process={(process != null ? process.Id : 0)}, windows={windowHandle}.{unexpectedRoomLogMessage}");
                            }

                            ExportHandHistory(package, handHistory, windowHandle, handHistoriesToProcess, unexpectedRoomDetected);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(Logger, $"Could not process captured packet.", e);
                }
            }
        }

        protected virtual void ProcessFastFoldNoticeGameReset(FastFoldImportDto fastFoldImportDto)
        {
            var noticeGameSnapshot = fastFoldImportDto.HandBuilder.GetNoticeRoomSnapShot(fastFoldImportDto.Package);

            if (noticeGameSnapshot == null ||
                fastFoldImportDto.NoticeResetGame.Players == null ||
                !long.TryParse(fastFoldImportDto.NoticeResetGame.GameId, out long handId))
            {
                LogProvider.Log.Error(Logger, $"Failed to get snapshot for {fastFoldImportDto.Package.UserId} of {fastFoldImportDto.Package.RoomId} (Fast Fold).");
                return;
            }

            // load players from db
            var playersToAdd = fastFoldImportDto.NoticeResetGame.Players
                .Select(x => x.Playerid.ToString())
                .Where(x => !fastFoldImportDto.PlayerNamePlayerIdMap.ContainsKey(x))
                .ToArray();

            if (playersToAdd.Length > 0)
            {
                using (var session = ModelEntities.OpenSession())
                {
                    var playerNamePlayerIdToAdd = session.Query<Players>()
                        .Where(x => x.PokersiteId == (short)Site && playersToAdd.Contains(x.Playername))
                        .Select(x => new { x.Playername, x.PlayerId })
                        .ToArray();

                    playerNamePlayerIdToAdd.ForEach(x => fastFoldImportDto.PlayerNamePlayerIdMap.Add(x.Playername, x.PlayerId));
                }
            }

            var gameInfo = new GameInfo
            {
                Session = $"{fastFoldImportDto.Package.RoomId}{fastFoldImportDto.Package.UserId}",
                WindowHandle = fastFoldImportDto.WindowHandle.ToInt32(),
                PokerSite = Site,
                GameType = Bovada.GameType.Holdem,
                TableType = (EnumTableType)noticeGameSnapshot.Params.PlayerCountMax,
                GameFormat = GameFormat.FastFold,
                GameNumber = handId
            };

            // Initialize cache
            gameInfo.ResetPlayersCacheInfo();

            var players = new PlayerList(fastFoldImportDto.NoticeResetGame.Players.Select(x =>
                  new Player(x.Playerid.ToString(), 0, x.Seatid + 1)
                  {
                      PlayerId = fastFoldImportDto.PlayerNamePlayerIdMap.ContainsKey(x.Playerid.ToString()) ?
                        fastFoldImportDto.PlayerNamePlayerIdMap[x.Playerid.ToString()] : 0,
                      PlayerNick = x.Name
                  }));

            Player heroPlayer = null;

            foreach (var player in players)
            {
                if (player.PlayerId == 0)
                {
                    continue;
                }

                var isHero = false;

                if (player.PlayerName.Equals(fastFoldImportDto.Package.UserId.ToString()))
                {
                    heroPlayer = player;
                    isHero = true;
                }

                var playerCollectionItem = new PlayerCollectionItem
                {
                    PlayerId = player.PlayerId,
                    Name = player.PlayerName,
                    PokerSite = Site
                };

                var playerCacheStatistic = fastFoldImportDto.ImporterSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem, out bool exists);

                if (exists && playerCacheStatistic.IsHero)
                {
                    heroPlayer = player;
                    gameInfo.GameFormat = playerCacheStatistic.GameFormat;
                    break;
                }
                else if (!exists && gameInfo.GameFormat == GameFormat.FastFold)
                {
                    var playerCacheInfo = new PlayerStatsSessionCacheInfo
                    {
                        Session = gameInfo.Session,
                        GameFormat = gameInfo.GameFormat,
                        Player = playerCollectionItem,
                        IsHero = isHero,
                        Stats = new Playerstatistic
                        {
                            SessionCode = gameInfo.Session,
                            PokergametypeId = (short)HandHistories.Objects.GameDescription.GameType.NoLimitHoldem
                        }
                    };

                    if (playerCacheInfo.Stats.PokergametypeId != 0)
                    {
                        gameInfo.AddToPlayersCacheInfo(playerCacheInfo);
                    }
                }
            }

            PreparePlayerList(players,
                noticeGameSnapshot.Params.PlayerCountMax,
                heroPlayer != null ? heroPlayer.SeatNumber : 0);

            var importedArgs = new DataImportedEventArgs(players, gameInfo, heroPlayer, 0);
            eventAggregator.GetEvent<DataImportedEvent>().Publish(importedArgs);
        }

        protected virtual void ExportHandHistory(PokerKingPackage package, HandHistory handHistory, IntPtr windowHandle,
            ConcurrentDictionary<long, List<HandHistoryData>> handHistoriesToProcess, bool unexpectedRoomDetected)
        {
            var handHistoryData = new HandHistoryData
            {
                Uuid = package.UserId,
                HandHistory = handHistory,
                WindowHandle = !unexpectedRoomDetected ? windowHandle : IntPtr.Zero
            };

            if (!pkCatcherService.CheckHand(handHistory))
            {
                LogProvider.Log.Info(Logger, $"License doesn't support cash hand {handHistory.HandId}. [BB={handHistory.GameDescription.Limit.BigBlind}]");

                if (handHistoryData.WindowHandle != IntPtr.Zero)
                {
                    SendPreImporedData("Notifications_HudLayout_PreLoadingText_PK_NoLicense", windowHandle);
                }

                return;
            }

            ExportHandHistory(handHistoryData, handHistoriesToProcess);
        }

        protected virtual void CloseHUD(IntPtr windowHandle)
        {
            Task.Run(() =>
            {
                Task.Delay(DelayToProcessHands * 2).Wait();

                var args = new TableClosedEventArgs(windowHandle.ToInt32());
                eventAggregator.GetEvent<TableClosedEvent>().Publish(args);
            });
        }

        // shell
        private static readonly byte[] arg1 = new byte[] { 0x73, 0x68, 0x65, 0x6C, 0x6C };

        // "cat /data/data/com.ylc.qp.Pokermate/shared_prefs/Cocos2dxPrefsFile.xml"
        private static readonly byte[] arg2 = new byte[] {
            0x22, 0x63, 0x61, 0x74, 0x20, 0x2F, 0x64, 0x61, 0x74, 0x61, 0x2F, 0x64, 0x61, 0x74, 0x61, 0x2F, 0x63, 0x6F, 0x6D,
            0x2E, 0x79, 0x6C, 0x63, 0x2E, 0x71, 0x70, 0x2E, 0x50, 0x6F, 0x6B, 0x65, 0x72, 0x6D, 0x61, 0x74, 0x65, 0x2F, 0x73,
            0x68, 0x61, 0x72, 0x65, 0x64, 0x5F, 0x70, 0x72, 0x65, 0x66, 0x73, 0x2F, 0x43, 0x6F, 0x63, 0x6F, 0x73, 0x32, 0x64,
            0x78, 0x50, 0x72, 0x65, 0x66, 0x73, 0x46, 0x69, 0x6C, 0x65, 0x2E, 0x78, 0x6D, 0x6C, 0x22 };


        protected virtual bool TryDecryptBody(PokerKingPackage package, Process process, IEmulatorService emulatorService, bool withRepeat = true)
        {
            try
            {
                if (!userTokens.TryGetValue(package.UserId, out string token))
                {
                    withRepeat = false;

                    LogProvider.Log.Info(Logger, $"Token for user #{package.UserId} not found");

                    // read token from emulator
                    var loginResponses = emulatorService.ExecuteAdbCommand(process,
                        Encoding.ASCII.GetString(arg1),
                        Encoding.ASCII.GetString(arg2));

                    foreach (var loginResponseXml in loginResponses)
                    {
                        if (!PKLoginResponse.TryParse(loginResponseXml, out PKLoginResponse loginResponse))
                        {
                            LogProvider.Log.Warn(Logger, $"Failed to parse token for user #{package.UserId} from response.");
                            continue;
                        }

                        userTokens.AddOrUpdate(loginResponse.UserId, loginResponse.UserToken, (dictKey, oldValue) => loginResponse.UserToken);
                        LogProvider.Log.Info(Logger, $"Token for user #{loginResponse.UserId} has been updated.");

                        if (loginResponse.UserId == package.UserId)
                        {
                            token = loginResponse.UserToken;
                        }
                    }

                    if (string.IsNullOrEmpty(token))
                    {
                        LogProvider.Log.Warn(Logger, $"Failed to find token for user #{package.UserId}.");
                        return false;
                    }
                }

                var key = Encoding.ASCII.GetBytes(token);

                package.Body = PKCipherHelper.Decode(key, package.Body);

                return true;
            }
            catch (Exception e)
            {
                if (withRepeat)
                {
                    LogProvider.Log.Warn(Logger, $"Failed to read the body of user #{package.UserId} room #{package.RoomId}. Removing invalid token.");
                    userTokens.TryRemove(package.UserId, out string removeToken);

                    return TryDecryptBody(package, process, emulatorService, false);
                }

                LogProvider.Log.Error(Logger, $"Couldn't read the body of {package.PackageType} user #{package.UserId} room #{package.RoomId}", e);
            }

            return false;
        }

        /// <summary>
        /// Parses package body into the specified type <see cref="{T}"/>, then performs <paramref name="onSuccess"/> if parsing succeed, 
        /// or <paramref name="onFail"/> if parsing failed
        /// </summary>
        /// <typeparam name="T">Type of the package body</typeparam>
        /// <param name="package">Package to parse</param>
        /// <param name="onSuccess">Action to perform if parsing succeed</param>
        /// <param name="onFail">Action to perform if parsing failed</param>
        protected virtual void ParsePackage<T>(PokerKingPackage package, Action<T> onSuccess, Action onFail)
        {
            if (SerializationHelper.TryDeserialize(package.Body, out T packageBody))
            {
                onSuccess?.Invoke(packageBody);
                return;
            }

            LogProvider.Log.Warn(Logger, $"Failed to deserialize {typeof(T)} package for user #{package.UserId} room #{package.RoomId}");

            onFail?.Invoke();
        }

        /// <summary>
        /// Checks whenever the specified package has to be processed
        /// </summary>
        /// <param name="package">Package to check</param>
        /// <returns></returns>
        protected static bool IsAllowedPackage(PokerKingPackage package)
        {
            switch (package.PackageType)
            {
                case PackageType.NoticeBuyin:
                case PackageType.NoticeGameAnte:
                case PackageType.NoticeGameBlind:
                case PackageType.NoticeGameCommunityCards:
                case PackageType.NoticeGameElectDealer:
                case PackageType.NoticeGameHoleCard:
                case PackageType.NoticeGamePost:
                case PackageType.NoticeGameRoundEnd:
                case PackageType.NoticeGameSettlement:
                case PackageType.NoticeGameShowCard:
                case PackageType.NoticeGameShowDown:
                case PackageType.NoticeGameSnapShot:
                case PackageType.NoticePlayerAction:
                case PackageType.NoticePlayerShowCard:
                case PackageType.NoticePlayerStayPosition:
                case PackageType.NoticeResetGame:
                case PackageType.NoticeStartGame:
                case PackageType.NoticeQuickLeave:
                case PackageType.RequestLeaveRoom:
                case PackageType.RequestJoinRoom:
                case PackageType.RequestQuickFold:
                    return true;

                default:
                    return false;
            }
        }

        #region Debug logging

        protected virtual void LogPackage(PokerKingPackage package)
        {
            switch (package.PackageType)
            {
                case PackageType.RequestLeaveRoom:
                    LogPackage<RequestLeaveRoom>(package);
                    break;
                case PackageType.NoticeStartGame:
                    LogPackage<NoticeStartGame>(package);
                    break;
                case PackageType.NoticeResetGame:
                    LogPackage<NoticeResetGame>(package);
                    break;
                case PackageType.NoticeGamePost:
                    LogPackage<NoticeGamePost>(package);
                    break;
                case PackageType.NoticeGameAnte:
                    LogPackage<NoticeGameAnte>(package);
                    break;
                case PackageType.NoticeGameElectDealer:
                    LogPackage<NoticeGameElectDealer>(package);
                    break;
                case PackageType.NoticeGameBlind:
                    LogPackage<NoticeGameBlind>(package);
                    break;
                case PackageType.NoticeGameHoleCard:
                    LogPackage<NoticeGameHoleCard>(package);
                    break;
                case PackageType.NoticePlayerAction:
                    LogPackage<NoticePlayerAction>(package);
                    break;
                case PackageType.NoticeGameRoundEnd:
                    LogPackage<NoticeGameRoundEnd>(package);
                    break;
                case PackageType.NoticeGameCommunityCards:
                    LogPackage<NoticeGameCommunityCards>(package);
                    break;
                case PackageType.NoticeGameShowCard:
                    LogPackage<NoticeGameShowCard>(package);
                    break;
                case PackageType.NoticeGameSettlement:
                    LogPackage<NoticeGameSettlement>(package);
                    break;
                case PackageType.NoticeGameShowDown:
                    LogPackage<NoticeGameShowDown>(package);
                    break;
                case PackageType.NoticePlayerStayPosition:
                    LogPackage<NoticePlayerStayPosition>(package);
                    break;
                case PackageType.NoticePlayerShowCard:
                    LogPackage<NoticePlayerShowCard>(package);
                    break;
                case PackageType.NoticeBuyin:
                    LogPackage<NoticeBuyin>(package);
                    break;
                case PackageType.NoticeGameSnapShot:
                    LogPackage<NoticeGameSnapShot>(package);
                    break;
                case PackageType.RequestHeartBeat:
                    LogPackage<RequestHeartBeat>(package);
                    break;
                case PackageType.RequestQuickFold:
                    LogPackage<RequestQuickFold>(package);
                    break;
                case PackageType.NoticeQuickLeave:
                    LogPackage<NoticeQuickLeave>(package);
                    break;
            }
        }

        protected virtual void LogPackage<T>(PokerKingPackage package)
        {
            try
            {
#if DEBUG
                if (!SerializationHelper.TryDeserialize(package.Body, out T packageContent))
                {
                    LogProvider.Log.Warn(Logger, $"Failed to deserialize {typeof(T)} package");
                }

                var packageJson = new PackageJson<T>
                {
                    PackageType = package.PackageType,
                    Content = packageContent,
                    Time = package.Timestamp,
                    UserId = package.UserId,
                    RoomId = package.RoomId,
                    IsFastFold = package.IsFastFold
                };

                var json = JsonConvert.SerializeObject(packageJson, Formatting.Indented, new StringEnumConverter());

                protectedLogger.Log(json);
#else
                using (var ms = new MemoryStream())
                {
                    Serializer.Serialize(ms, package);
                    var packageBytes = ms.ToArray();

                    var logText = Encoding.UTF8.GetString(packageBytes);
                    protectedLogger.Log(logText);
                }
#endif
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, "Failed to log package", e);
            }
        }

        protected virtual void LogPacket(CapturedPacket capturedPacket, string ext)
        {
            var packageFileName = Path.Combine("Logs", capturedPacket.ToString().Replace(":", ".").Replace("->", "-")) + ext;

            var pkManager = new PokerKingPacketManager();

            var sb = new StringBuilder();
            sb.AppendLine("-----------begin----------------");
            sb.AppendLine($"Date: {capturedPacket.CreatedTimeStamp}");
            sb.AppendLine($"Date Now: {DateTime.Now}");
            sb.AppendLine($"Date Now (ticks): {DateTime.Now.Ticks}");
            sb.AppendLine($"SequenceNumber: {capturedPacket.SequenceNumber}");
            sb.AppendLine($"Packet Full Length: {(pkManager.IsStartingPacket(capturedPacket.Bytes) ? pkManager.ReadPacketLength(capturedPacket.Bytes) : 0)}");
            sb.AppendLine($"Packet Current Length: {capturedPacket.Bytes.Length}");
            sb.AppendLine($"Body:");
            sb.AppendLine("---------body begin-------------");
            sb.AppendLine(BitConverter.ToString(capturedPacket.Bytes).Replace("-", " "));
            sb.AppendLine("----------body end--------------");
            sb.AppendLine("------------end-----------------");

            File.AppendAllText(packageFileName, sb.ToString());
        }

#if DEBUG
        private class PackageJson<T>
        {
            public PackageType PackageType { get; set; }

            public DateTime Time { get; set; }

            public T Content { get; set; }

            public uint UserId { get; set; }

            public int RoomId { get; set; }

            public bool IsFastFold { get; set; }
        }
#endif

        #endregion

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }

        private Dictionary<int, int> autoCenterSeats = new Dictionary<int, int>
        {
            { 2, 1 },
            { 3, 1 },
            { 4, 1 },
            { 5, 1 },
            { 6, 1 },
            { 7, 1 },
            { 8, 1 },
            { 9, 1 }
        };

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;
            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;
            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            PreparePlayerList(playerList, maxPlayers, heroSeat);

            return playerList;
        }

        protected virtual void PreparePlayerList(PlayerList playerList, int maxPlayers, int heroSeat)
        {
            if (heroSeat != 0 && autoCenterSeats.ContainsKey(maxPlayers))
            {
                var prefferedSeat = autoCenterSeats[maxPlayers];

                var shift = (prefferedSeat - heroSeat) % maxPlayers;

                foreach (var player in playerList)
                {
                    player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                }
            }
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return IntPtr.Zero;
        }

        protected override void PublishImportedResults(DataImportedEventArgs args)
        {
            // do not update HUD when hand is imported for FF games
            if (args.GameInfo.GameFormat == GameFormat.FastFold)
            {
                args.DoNotUpdateHud = true;
            }

            base.PublishImportedResults(args);
        }

        protected class FastFoldImportDto
        {
            public PokerKingPackage Package { get; set; }

            public IPKHandBuilder HandBuilder { get; set; }

            public IntPtr WindowHandle { get; set; }

            public Dictionary<string, int> PlayerNamePlayerIdMap { get; set; }

            public IImporterSessionCacheService ImporterSessionCacheService { get; set; }

            public NoticeResetGame NoticeResetGame { get; set; }
        }
    }
}