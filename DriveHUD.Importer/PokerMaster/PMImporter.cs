//-----------------------------------------------------------------------
// <copyright file="PMImporter.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.PokerMaster.Model;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using Newtonsoft.Json;
using PacketDotNet;
using SharpPcap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerMaster
{
    /// <summary>
    /// Poker master data importer
    /// </summary>
    internal class PMImporter : GenericImporter, IPMImporter
    {
        private readonly ManualResetEventSlim captureResetEvent = new ManualResetEventSlim();

        private readonly BlockingCollection<CapturedPacket> packetBuffer = new BlockingCollection<CapturedPacket>();

        private readonly ConcurrentStack<CaptureDevice> captureDevices = new ConcurrentStack<CaptureDevice>();

        private readonly IPMCatcherService pmCatcherService = ServiceLocator.Current.TryResolve<IPMCatcherService>();

        private Lazy<BodyDecryptor> bodyDecryptor = new Lazy<BodyDecryptor>(false);

        private const int StopTimeout = 5000;

        private const int NoDataDelay = 100;

        private Dictionary<long, bool> isReloginRequired = new Dictionary<long, bool>();

        private IPokerClientEncryptedLogger protectedLogger;

        private BodyDecryptor BodyDecryptor
        {
            get
            {
                return bodyDecryptor.Value;
            }
        }

        private Dictionary<long, byte[]> HeroesKeys
        {
            get;
            set;
        }

        protected override bool SupportDuplicates => true;

        protected override EnumPokerSites Site => EnumPokerSites.PokerMaster;

        protected override string ProcessName => throw new NotSupportedException($"Process name isn't supported for importer. [{SiteString}]");

        /// <summary>
        /// Main importer task, executes in the background thread
        /// </summary>
        protected override void DoImport()
        {
            try
            {
                InitializeLogger();
                InitializeSettings();
                StartNetworkDataCapture();
                ProcessBuffer();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Failed importing.", e);
            }

            captureResetEvent.Wait(StopTimeout);

            captureDevices.Clear();
            packetBuffer.Clear();

            protectedLogger.StopLogging();

            RaiseProcessStopped();
        }

        /// <summary>
        /// Reads settings and initializes importer variables
        /// </summary>
        protected void InitializeSettings()
        {

#if DEBUG
            if (!Directory.Exists("Hands"))
            {
                Directory.CreateDirectory("Hands");
            }
#endif
            isReloginRequired.Clear();

            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            if (settings != null)
            {
                IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
                HeroesKeys = pmCatcherService.GetHeroes().ToDictionary(x => x.Key, x => Convert.FromBase64String(x.Value));
            }
            else
            {
                HeroesKeys = new Dictionary<long, byte[]>();
            }
        }

        /// <summary>
        /// Initializes logger with protection
        /// </summary>
        protected void InitializeLogger()
        {
            var logger = new PokerClientLoggerConfiguration
            {
                DateFormat = "yyy-MM-dd",
                DateTimeFormat = "HH:mm:ss",
                LogCleanupTemplate = "pm-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "pm-games-{0}.log",
                MessagesInBuffer = 10
            };

            protectedLogger = ServiceLocator.Current.GetInstance<IPokerClientEncryptedLogger>(LogServices.Base.ToString());
            protectedLogger.Initialize(logger);
            protectedLogger.CleanLogs();
            protectedLogger.StartLogging();
        }

        /// <summary>
        /// Starts processes to capture network data for all available devices
        /// </summary>
        protected void StartNetworkDataCapture()
        {
            captureResetEvent.Set();

            var devices = CaptureDeviceList.Instance;

            foreach (var device in devices)
            {
                if (IsAdvancedLogEnabled)
                {
                    LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Found device: {device.Name}, {device.Description}.");
                }

                try
                {
                    device.Open(DeviceMode.Normal);
                    device.Filter = PMImporterHelper.TrafficFilter;

                    var captureDevice = new CaptureDevice(device)
                    {
                        IsOpened = true
                    };

                    captureDevices.Push(captureDevice);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Could not open device {device.Name}.", e);
                }
            }

            captureDevices.ForEach(captureDevice => Task.Run(() => CaptureData(captureDevice)));
        }

        /// <summary>
        /// Captures network data of the specified device
        /// </summary>
        /// <param name="device">Device to capture</param>
        protected void CaptureData(CaptureDevice captureDevice)
        {
            if (captureResetEvent.IsSet)
            {
                captureResetEvent.Reset();
            }

            while (true)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    try
                    {
                        captureDevice.Device.Close();
                        captureDevice.IsOpened = false;
                    }
                    catch (Exception e)
                    {
                        LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Device {captureDevice.Device.Name} has not been closed.", e);
                    }

                    if (!captureResetEvent.IsSet && captureDevices.All(x => !x.IsOpened))
                    {
                        captureResetEvent.Set();
                    }

                    return;
                }

                try
                {
                    var nextPacket = captureDevice.Device.GetNextPacket();

                    if (nextPacket != null)
                    {
                        ParsePacket(nextPacket);
                    }
                    else
                    {
                        Task.Delay(NoDataDelay).Wait();
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Data has not been captured from {captureDevice.Device.Name}.", e);
                }
            }
        }

        /// <summary>
        /// Parses captured data into the packet
        /// </summary>
        /// <param name="rawCapture">Captured data</param>
        protected void ParsePacket(RawCapture rawCapture)
        {
            var packet = Packet.ParsePacket(rawCapture.LinkLayerType, rawCapture.Data);

            var tcpPacket = packet.Extract<TcpPacket>();
            var ipPacket = packet.Extract<IpPacket>();

            var payloadData = tcpPacket.PayloadData;

            if (payloadData == null || payloadData.Length == 0)
            {
                return;
            }

            var capturedPacket = new CapturedPacket
            {
                Bytes = tcpPacket.PayloadData,
                Source = new IPEndPoint(ipPacket.SourceAddress, tcpPacket.SourcePort),
                Destination = new IPEndPoint(ipPacket.DestinationAddress, tcpPacket.DestinationPort),
                CreatedTimeStamp = rawCapture.Timeval.Date,
                SequenceNumber = tcpPacket.SequenceNumber
            };

            packetBuffer.Add(capturedPacket);
        }

        /// <summary>
        /// Processes packets in the buffer
        /// </summary>
        protected void ProcessBuffer()
        {
            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager>();
            var handBuilder = ServiceLocator.Current.GetInstance<IHandBuilder>();
            var connectionsService = ServiceLocator.Current.GetInstance<INetworkConnectionsService>();
            var tableWindowProvider = ServiceLocator.Current.GetInstance<ITableWindowProvider>();
            var handHistoriesToProcess = new ConcurrentDictionary<long, List<HandHistoryData>>();

            var detectedTableWindows = new HashSet<IntPtr>();

            var userRooms = new Dictionary<long, Tuple<long, IntPtr>>();

            while (!cancellationTokenSource.IsCancellationRequested)
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

                    if (!packetManager.TryParse(capturedPacket, out Package package) || !IsAllowedPackage(package))
                    {
                        continue;
                    }
#if DEBUG                 
                    //LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Cmd: {package.Cmd} Uuid: {package.Uuid} [{capturedPacket}].");
#endif                

                    if (package.Cmd == PackageCommand.Cmd_SCLoginRsp)
                    {
                        ParseLoginPackage<SCLoginRsp>(package, x => x?.UserInfo, x => x?.EncryptKey);
                        continue;
                    }

                    if (package.Cmd == PackageCommand.Cmd_SCUploadVerifyCodeRsp)
                    {
                        ParseLoginPackage<SCUploadVerifyCodeRsp>(package, x => x?.UserInfo, x => x?.EncryptKey);
                        continue;
                    }

                    if (package.Cmd == PackageCommand.Cmd_SCLoginThirdPartyRsp)
                    {
                        ParseLoginPackage<SCLoginThirdPartyRsp>(package, x => x?.UserInfo, x => x?.EncryptKey);
                        continue;
                    }

                    if (package.Cmd == PackageCommand.Cmd_SCLeaveGameRoomRsp ||
                        package.Cmd == PackageCommand.Cmd_SCLeaveMTTGameRoom ||
                        package.Cmd == PackageCommand.Cmd_SCLeaveSNGGameRoomRsp)
                    {
                        ParsePackage<SCLeaveGameRoomRsp>(package,
                            body =>
                            {
                                if (userRooms.ContainsKey(package.Uuid))
                                {
                                    userRooms.Remove(package.Uuid);
                                }

                                LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"User {package.Uuid} left room {body.RoomId}.");
                            },
                            () => LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"User {package.Uuid} left room."));

                        var process = connectionsService.GetProcess(capturedPacket);
                        var windowHandle = tableWindowProvider.GetTableWindowHandle(process);

                        Task.Run(() =>
                        {
                            Task.Delay(DelayToProcessHands * 2).Wait();

                            var args = new TableClosedEventArgs(windowHandle.ToInt32());
                            eventAggregator.GetEvent<TableClosedEvent>().Publish(args);
                        });

                        detectedTableWindows.Remove(windowHandle);

                        continue;
                    }

                    if (package.Cmd == PackageCommand.Cmd_SCEnterGameRoomRsp)
                    {
                        ParsePackage<SCEnterGameRoomRsp>(package,
                           body => LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"User {package.Uuid} entered room {body.RoomId}."),
                           () => LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"User {package.Uuid} entered room."));

                        continue;
                    }

                    if (package.Cmd == PackageCommand.Cmd_SCGameRoomStateChange)
                    {
                        var process = connectionsService.GetProcess(capturedPacket);
                        var windowHandle = tableWindowProvider.GetTableWindowHandle(process);

                        var newlyDetectedTable = false;

                        if (!detectedTableWindows.Contains(windowHandle))
                        {
                            detectedTableWindows.Add(windowHandle);
                            newlyDetectedTable = true;
                        }

                        if (!HeroesKeys.TryGetValue(package.Uuid, out byte[] encryptKey))
                        {
                            if (!isReloginRequired.ContainsKey(package.Uuid))
                            {
                                isReloginRequired.Add(package.Uuid, false);
                            }

                            if (isReloginRequired[package.Uuid])
                            {
                                continue;
                            }

                            LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Encryption key not found [User {package.Uuid}].");

                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_Relogin", windowHandle);

                            isReloginRequired[package.Uuid] = true;
                            continue;
                        }

                        if (package.Body == null || package.Body.Length == 0)
                        {
                            continue;
                        }

                        var bytes = BodyDecryptor.Decrypt(package.Body, encryptKey, IsAdvancedLogEnabled);

                        if (!SerializationHelper.TryDeserialize(bytes, out SCGameRoomStateChange scGameRoomStateChange))
                        {
                            LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Package has not been decrypted. Relogin [User {package.Uuid}] is required. [{capturedPacket}] [{capturedPacket.SequenceNumber}]");

                            var base64Body = Convert.ToBase64String(package.Body);
                            var base64Key = Convert.ToBase64String(encryptKey);

                            LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Package body: [{base64Key}, {base64Body}]");

                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_Relogin", windowHandle);
                            continue;
                        }

                        if (newlyDetectedTable)
                        {
                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM", windowHandle);
                        }
#if DEBUG
                        File.AppendAllText($"Hands\\hand_imported_{package.Uuid}_{scGameRoomStateChange.GameNumber}.json", JsonConvert.SerializeObject(scGameRoomStateChange, Formatting.Indented));
#endif

                        if (IsAdvancedLogEnabled)
                        {
                            protectedLogger.Log(JsonConvert.SerializeObject(scGameRoomStateChange, Formatting.Indented));
                        }

                        if (handBuilder.TryBuild(scGameRoomStateChange, package.Uuid, out HandHistory handHistory))
                        {
                            if (IsAdvancedLogEnabled)
                            {
                                LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Hand #{handHistory.HandId}: Found process={(process != null ? process.Id : 0)}, windows={windowHandle}.");
                            }

                            if (!pmCatcherService.CheckHand(handHistory))
                            {
                                if (handHistory.GameDescription.IsTournament)
                                {
                                    LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"License doesn't support tourney hand {handHistory.HandId}. [buyin={handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue}]");
                                }
                                else
                                {
                                    LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"License doesn't support cash hand {handHistory.HandId}. [bb={handHistory.GameDescription.Limit.BigBlind}]");
                                }

                                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_NoLicense", windowHandle);

                                continue;
                            }

                            var handHistoryData = new HandHistoryData
                            {
                                Uuid = package.Uuid,
                                HandHistory = handHistory,
                                WindowHandle = windowHandle
                            };

                            ExportHandHistory(handHistoryData, handHistoriesToProcess);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (IsAdvancedLogEnabled)
                    {
                        LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Could not process captured packet.", e);
                    }
                }
            }
        }

        /// <summary>
        /// Validates if room is supported
        /// </summary>
        /// <param name="scGameRoomStateChange">Room state change</param>
        /// <param name="userRooms">User rooms</param>
        /// <param name="windowHandle">Handle of window of table</param>
        /// <returns></returns>
        private bool ValidateRoom(long userId, SCGameRoomStateChange scGameRoomStateChange, Dictionary<long, Tuple<long, IntPtr>> userRooms, IntPtr windowHandle)
        {
            if (scGameRoomStateChange.GameRoomInfo == null)
            {
                LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Room info is empty.");
                return false;
            }

            var roomId = scGameRoomStateChange.GameRoomInfo.IsTournament ?
                    scGameRoomStateChange.GameRoomInfo.SNGGameRoomBaseInfo.GameRoomId :
                    scGameRoomStateChange.GameRoomInfo.GameRoomBaseInfo.GameRoomId;

            if (!userRooms.ContainsKey(userId))
            {
                userRooms.Add(userId, Tuple.Create(roomId, windowHandle));
            }
            else
            {
                userRooms[userId] = Tuple.Create(roomId, windowHandle);
            }

            var accountsPerRoom = userRooms.Values
                .Count(x => x.Item1 == roomId && WinApi.IsWindow(x.Item2));

            if (accountsPerRoom > 1)
            {
                LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"More than 1 account detected in room {roomId}.");
                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_AccountPerTable", windowHandle);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses login package
        /// </summary>
        /// <param name="package">Package to parse</param>
        private void ParseLoginPackage<T>(Package package, Func<T, UserBaseInfoNet> getUserInfo, Func<T, string> getEncryptKey)
        {
            if (IsAdvancedLogEnabled)
            {
                LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Login package [{package.Cmd}] has been received.");
            }

            var bytes = BodyDecryptor.Decrypt(package.Body);

            if (bytes == null)
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Failed to parse login package [{package.Cmd}].");
                return;
            }

            var scLoginRsp = SerializationHelper.Deserialize<T>(bytes);
            var userInfo = getUserInfo(scLoginRsp);

            if (scLoginRsp == null)
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"Login package is empty.");
                return;
            }

            if (userInfo == null)
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"UserInfo is missing.");
                return;
            }

            LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"User [{userInfo.Uuid}, {userInfo.ShowID}] has logged in.");

            var encryptKeyText = getEncryptKey(scLoginRsp);

            if (string.IsNullOrWhiteSpace(encryptKeyText))
            {
                LogProvider.Log.Error(CustomModulesNames.PMCatcher, $"EncryptKey is missing.");
                return;
            }

            // update settings
            var encryptKey = encryptKeyText.ToBytes();
            var base64EncryptKey = Convert.ToBase64String(encryptKey);
            var heroId = userInfo.Uuid;

            if (!HeroesKeys.ContainsKey(heroId))
            {
                HeroesKeys.Add(heroId, encryptKey);
            }
            else
            {
                HeroesKeys[heroId] = encryptKey;
            }

            var heroes = pmCatcherService.GetHeroes();

            if (!heroes.ContainsKey(heroId))
            {
                heroes.Add(heroId, base64EncryptKey);
            }
            else
            {
                heroes[heroId] = base64EncryptKey;
            }

            pmCatcherService.SaveHeroes(heroes);

            if (!isReloginRequired.ContainsKey(package.Uuid))
            {
                isReloginRequired.Add(package.Uuid, false);
                return;
            }

            isReloginRequired[package.Uuid] = false;
        }

        /// <summary>
        /// Parses package body into the specified type <see cref="{T}"/>, then performs <paramref name="onSuccess"/> if parsing succeed, 
        /// or <paramref name="onFail"/> if parsing failed
        /// </summary>
        /// <typeparam name="T">Type of the package body</typeparam>
        /// <param name="package">Package to parse</param>
        /// <param name="onSuccess">Action to perform if parsing succeed</param>
        /// <param name="onFail">Action to perform if parsing failed</param>
        private void ParsePackage<T>(Package package, Action<T> onSuccess, Action onFail)
        {
            if (HeroesKeys.TryGetValue(package.Uuid, out byte[] encryptKey))
            {
                var bytes = BodyDecryptor.Decrypt(package.Body, encryptKey, IsAdvancedLogEnabled);

                if (SerializationHelper.TryDeserialize(bytes, out T packageBody))
                {
                    onSuccess?.Invoke(packageBody);
                    return;
                }
            }

            onFail?.Invoke();
        }

        /// <summary>
        /// Checks whenever the specified package has to be processed
        /// </summary>
        /// <param name="package">Package to check</param>
        /// <returns></returns>
        private static bool IsAllowedPackage(Package package)
        {
            switch (package.Cmd)
            {
                case PackageCommand.Cmd_SCHelloRsp:
                case PackageCommand.Cmd_CSLogin:
                case PackageCommand.Cmd_CSHello:
                case PackageCommand.Cmd_CSHelloGame:
                case PackageCommand.Cmd_SCHelloGameRsp:
                case PackageCommand.Cmd_SCReconnectRsp:
                    return false;

                default:
                    return true;
            }
        }

        private const int DelayToProcessHands = 2000;

        /// <summary>
        /// Exports captured hand history to the supported DB
        /// </summary>
        private void ExportHandHistory(HandHistoryData handHistoryData, ConcurrentDictionary<long, List<HandHistoryData>> handHistoriesToProcess)
        {
            LogProvider.Log.Info(CustomModulesNames.PMCatcher, $"Hand #{handHistoryData.HandHistory.HandId} has been sent [{handHistoryData.Uuid}].");

            var handId = handHistoryData.HandHistory.HandId;

            if (!handHistoriesToProcess.TryGetValue(handId, out List<HandHistoryData> handHistoriesData))
            {
                handHistoriesData = new List<HandHistoryData>();
                handHistoriesToProcess.AddOrUpdate(handId, handHistoriesData, (key, prev) => handHistoriesData);

                Task.Run(() =>
                {
                    try
                    {
                        Task.Delay(DelayToProcessHands).Wait(cancellationTokenSource.Token);
                        ExportHandHistory(handHistoriesData.ToList());
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    finally
                    {
                        handHistoriesToProcess.TryRemove(handId, out List<HandHistoryData> removedData);
                    }
                });
            }

            handHistoriesData.Add(handHistoryData);
        }

        /// <summary>
        /// Exports captured hand history to the supported DB
        /// </summary>
        private void ExportHandHistory(List<HandHistoryData> handHistories)
        {
            // merge hands
            var playerWithHoleCards = handHistories
                .SelectMany(x => x.HandHistory.Players)
                .Where(x => x.hasHoleCards)
                .DistinctBy(x => x.SeatNumber)
                .ToDictionary(x => x.SeatNumber, x => x.HoleCards);

            GameInfo mainGameInfo = null;

            foreach (var handHistoryData in handHistories)
            {
                handHistoryData.HandHistory.Players.ForEach(player =>
                {
                    if (!player.hasHoleCards && playerWithHoleCards.ContainsKey(player.SeatNumber))
                    {
                        player.HoleCards = playerWithHoleCards[player.SeatNumber];
                    }
                });

                var handHistoryText = SerializationHelper.SerializeObject(handHistoryData.HandHistory);

#if DEBUG
                File.WriteAllText($"Hands\\hand_exported_{handHistoryData.Uuid}_{handHistoryData.HandHistory.HandId}.xml", handHistoryText);
#endif
                var gameInfo = new GameInfo
                {
                    PokerSite = Site,
                    WindowHandle = handHistoryData.WindowHandle.ToInt32(),
                    GameNumber = handHistoryData.HandHistory.HandId,
                    Session = $"{handHistoryData.HandHistory.GameDescription.Identifier}{handHistoryData.Uuid}"
                };

                if (mainGameInfo == null)
                {
                    mainGameInfo = gameInfo;
                }
                else
                {
                    var playersCashInfo = mainGameInfo.GetPlayersCacheInfo();

                    if (playersCashInfo != null)
                    {
                        gameInfo.ResetPlayersCacheInfo();
                        gameInfo.DoNotReset = true;

                        foreach (var playerCashInfo in playersCashInfo)
                        {
                            var cacheInfo = new PlayerStatsSessionCacheInfo
                            {
                                Session = gameInfo.Session,
                                Player = new PlayerCollectionItem
                                {
                                    PlayerId = playerCashInfo.Player.PlayerId,
                                    Name = playerCashInfo.Player.Name,
                                    PokerSite = playerCashInfo.Player.PokerSite,
                                },
                                Stats = playerCashInfo.Stats.Copy(),
                                IsHero = handHistoryData.HandHistory.HeroName != null && 
                                    handHistoryData.HandHistory.HeroName.Equals(playerCashInfo.Player.Name),
                                GameFormat = playerCashInfo.GameFormat,
                                GameNumber = playerCashInfo.GameNumber
                            };

                            gameInfo.AddToPlayersCacheInfo(cacheInfo);
                        }
                    }
                }

                ProcessHand(handHistoryText, gameInfo);
            }
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

        protected override PlayerList GetPlayerList(HandHistory handHistory)
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

        protected override bool IsDisabled()
        {
            return pmCatcherService == null || !pmCatcherService.IsEnabled();
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return IntPtr.Zero;
        }

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            return false;
        }

        private void LogPacket(CapturedPacket capturedPacket, string ext)
        {
            var packageFileName = Path.Combine("Logs", capturedPacket.ToString().Replace(":", ".").Replace("->", "-")) + ext;

            var sb = new StringBuilder();
            sb.AppendLine("-----------begin----------------");
            sb.AppendLine($"Date: {capturedPacket.CreatedTimeStamp}");
            sb.AppendLine($"Date Now: {DateTime.Now}");
            sb.AppendLine($"SequenceNumber: {capturedPacket.SequenceNumber}");
            sb.AppendLine($"Packet Full Length: {PacketManager.ReadPacketLength(capturedPacket.Bytes)}");
            sb.AppendLine($"Packet Current Length: {capturedPacket.Bytes.Length}");
            sb.AppendLine($"Body:");
            sb.AppendLine("---------body begin-------------");
            sb.AppendLine(BitConverter.ToString(capturedPacket.Bytes).Replace("-", " "));
            sb.AppendLine("----------body end--------------");
            sb.AppendLine("------------end-----------------");

            File.AppendAllText(packageFileName, sb.ToString());
        }

        private class HandHistoryData
        {
            public long Uuid { get; set; }

            public HandHistory HandHistory { get; set; }

            public IntPtr WindowHandle { get; set; }
        }
    }
}