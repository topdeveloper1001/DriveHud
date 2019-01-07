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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using PacketDotNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
            var tableWindowProvider = ServiceLocator.Current.GetInstance<ITableWindowProvider>();
            tableWindowProvider.SetLogger(Logger);

            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager<PokerKingPackage>>();
            var handBuilder = ServiceLocator.Current.GetInstance<IPKHandBuilder>();

            var connectionsService = ServiceLocator.Current.GetInstance<INetworkConnectionsService>();
            connectionsService.SetLogger(Logger);

            var detectedTableWindows = new HashSet<IntPtr>();

            var handHistoriesToProcess = new ConcurrentDictionary<long, List<HandHistoryData>>();

            var usersRooms = new Dictionary<uint, int>();

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

                    if (capturedPacket.Destination.Port == PKImporterHelper.LoginPort ||
                        capturedPacket.Source.Port == PKImporterHelper.LoginPort)
                    {
                        ProcessLoginPortPacket(capturedPacket);
                        continue;
                    }

                    if (!packetManager.TryParse(capturedPacket, out IList<PokerKingPackage> packages))
                    {
                        continue;
                    }

                    foreach (var package in packages)
                    {
                        if (!IsAllowedPackage(package))
                        {
                            continue;
                        }

                        var process = connectionsService.GetProcess(capturedPacket);
                        var windowHandle = tableWindowProvider.GetTableWindowHandle(process);

                        if (!TryDecryptBody(package))
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

                                  LogProvider.Log.Info(Logger, $"User {package.UserId} entered room {body.RoomId}.");
                              },
                              () => LogProvider.Log.Info(Logger, $"User {package.UserId} entered room."));

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

                        if (handBuilder.TryBuild(package, windowHandle.ToInt32(), out HandHistory handHistory))
                        {
                            if (IsAdvancedLogEnabled)
                            {
                                var unexpectedRoomLogMessage = string.Empty;

                                if (unexpectedRoomDetected)
                                {
                                    unexpectedRoomLogMessage = " Unexpected room detected. No data will be sent to HUD.";
                                }

                                LogProvider.Log.Info(Logger,
                                    $"Hand #{handHistory.HandId} user #{package.UserId} room #{package.RoomId}: Process={(process != null ? process.Id : 0)}, windows={windowHandle}.{unexpectedRoomLogMessage}");
                            }

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

                                continue;
                            }

                            ExportHandHistory(handHistoryData, handHistoriesToProcess);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(Logger, $"Could not process captured packet.", e);
                }
            }
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

        protected virtual void ProcessLoginPortPacket(CapturedPacket packet)
        {
            try
            {
                if (packet.Bytes.Length == 0)
                {
                    return;
                }

                var packetText = Encoding.UTF8.GetString(packet.Bytes);

                var tokenIndex = packetText.IndexOf("token\":\"", StringComparison.OrdinalIgnoreCase);

                if (tokenIndex <= 0)
                {
                    return;
                }

                var openBracketIndex = packetText.IndexOf("{", StringComparison.OrdinalIgnoreCase);

                if (openBracketIndex < 0)
                {
                    return;
                }

                var closeBracketIndex = packetText.IndexOf("}", openBracketIndex, StringComparison.OrdinalIgnoreCase);

                if (closeBracketIndex < 0)
                {
                    return;
                }

                var json = packetText.Substring(openBracketIndex, closeBracketIndex - openBracketIndex + 1);

                var tokenInfo = JsonConvert.DeserializeObject<PKLoginTokenInfo>(json);

                if (userTokens.TryGetValue(tokenInfo.UserId, out string token) &&
                    token.Equals(tokenInfo.Token, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }

                userTokens.AddOrUpdate(tokenInfo.UserId, tokenInfo.Token, (key, oldValue) => tokenInfo.Token);

                LogProvider.Log.Info(Logger, $"Token info {tokenInfo.Token} has been saved for user #{tokenInfo.UserId}");
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, $"Failed to save token info.", e);
            }
        }

        protected virtual bool TryDecryptBody(PokerKingPackage package)
        {
            try
            {
                if (!userTokens.TryGetValue(package.UserId, out string token))
                {
                    LogProvider.Log.Warn(Logger, $"Token for user #{package.UserId} not found");
                    return false;
                }

                var key = Encoding.ASCII.GetBytes(token);

                var cipher = CipherUtilities.GetCipher("AES/ECB/PKCS5Padding");
                cipher.Init(false, new KeyParameter(key));

                var bytes = cipher.ProcessBytes(package.Body);
                var final = cipher.DoFinal();

                package.Body = (bytes == null) ? final :
                    ((final == null) ? bytes : bytes.Concat(final).ToArray());

                return true;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, $"Couldn't decode body of {package.PackageType} user #{package.UserId} room #{package.RoomId}", e);
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
                case PackageType.RequestLeaveRoom:
                case PackageType.RequestJoinRoom:
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
                    Time = package.Timestamp
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

            if (heroSeat != 0 && autoCenterSeats.ContainsKey(maxPlayers))
            {
                var prefferedSeat = autoCenterSeats[maxPlayers];

                var shift = (prefferedSeat - heroSeat) % maxPlayers;

                foreach (var player in playerList)
                {
                    player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                }
            }

            return playerList;
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return IntPtr.Zero;
        }
    }
}