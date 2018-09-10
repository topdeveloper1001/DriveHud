﻿//-----------------------------------------------------------------------
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
using PacketDotNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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

#if DEBUG
        protected override string HandHistoryFilePrefix => "pk";
#endif

        protected override string[] ProcessNames => throw new NotSupportedException($"Process name isn't supported for importer. [{SiteString}]");

        #region Implementation of ITcpImporter

        public override bool Match(TcpPacket tcpPacket, IpPacket ipPacket)
        {
            return tcpPacket.SourcePort == PKImporterHelper.PortFilter ||
                tcpPacket.DestinationPort == PKImporterHelper.PortFilter;
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
        protected void InitializeSettings()
        {

#if DEBUG
            if (!Directory.Exists("PKHands"))
            {
                Directory.CreateDirectory("PKHands");
            }
#endif            

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

                        if (IsAdvancedLogEnabled)
                        {
                            LogPackage(capturedPacket, package);
                        }

                        var process = connectionsService.GetProcess(capturedPacket);
                        var windowHandle = tableWindowProvider.GetTableWindowHandle(process);

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

                            Task.Run(() =>
                            {
                                Task.Delay(DelayToProcessHands * 2).Wait();

                                var args = new TableClosedEventArgs(windowHandle.ToInt32());
                                eventAggregator.GetEvent<TableClosedEvent>().Publish(args);
                            });

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
                                LogProvider.Log.Info(Logger, $"Hand #{handHistory.HandId} user #{package.UserId} room #{package.RoomId}: Process={(process != null ? process.Id : 0)}, windows={windowHandle}.");
                            }

                            var handHistoryData = new HandHistoryData
                            {
                                Uuid = package.UserId,
                                HandHistory = handHistory,
                                WindowHandle = windowHandle
                            };

                            if (unexpectedRoomDetected)
                            {
                                if (IsAdvancedLogEnabled)
                                {
                                    LogProvider.Log.Info(Logger, $"Hand #{handHistory.HandId} user #{package.UserId} room #{package.RoomId}: unexpected room detected.");
                                }

                                handHistoryData.WindowHandle = IntPtr.Zero;
                            }

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

            LogProvider.Log.Warn(Logger, $"Failed to deserialize {typeof(T)} package");

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

        protected virtual void LogPackage(CapturedPacket capturedPacket, PokerKingPackage package)
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
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(Logger, "Failed to log package", e);
            }
        }

        private void LogPacket(CapturedPacket capturedPacket, string ext)
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

        private class PackageJson<T>
        {
            public PackageType PackageType { get; set; }

            public DateTime Time { get; set; }

            public T Content { get; set; }
        }

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