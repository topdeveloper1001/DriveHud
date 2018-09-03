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
using DriveHUD.Common.Log;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.AndroidBase;
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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerMaster
{
    /// <summary>
    /// Poker master data importer
    /// </summary>
    internal class PMImporter : TcpPacketImporter, IPMImporter
    {
        private readonly BlockingCollection<CapturedPacket> packetBuffer = new BlockingCollection<CapturedPacket>();
        private readonly IPMCatcherService pmCatcherService = ServiceLocator.Current.TryResolve<IPMCatcherService>();
        private Lazy<BodyDecryptor> bodyDecryptor = new Lazy<BodyDecryptor>(false);
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

        protected override string[] ProcessNames => throw new NotSupportedException($"Process name isn't supported for importer. [{SiteString}]");
        protected override string Logger => CustomModulesNames.PMCatcher;

#if DEBUG
        protected override string HandHistoryFilePrefix => "pm";
#endif


        #region Implementation of ITcpImporter

        public override bool Match(TcpPacket tcpPacket, IpPacket ipPacket)
        {
            return tcpPacket.SourcePort == PMImporterHelper.PortFilter;
        }

        public override void AddPacket(CapturedPacket capturedPacket)
        {
            packetBuffer.Add(capturedPacket);
        }

        public override bool IsDisabled()
        {
            return pmCatcherService == null || !pmCatcherService.IsEnabled();
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
        /// Reads settings and initializes importer variables
        /// </summary>
        protected void InitializeSettings()
        {

#if DEBUG
            if (!Directory.Exists("PMHands"))
            {
                Directory.CreateDirectory("PMHands");
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
        /// Processes packets in the buffer
        /// </summary>
        protected void ProcessBuffer()
        {
            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager<PokerMasterPackage>>();
            var handBuilder = ServiceLocator.Current.GetInstance<IHandBuilder>();
            var tableWindowProvider = ServiceLocator.Current.GetInstance<ITableWindowProvider>();
            var handHistoriesToProcess = new ConcurrentDictionary<long, List<HandHistoryData>>();

            var connectionsService = ServiceLocator.Current.GetInstance<INetworkConnectionsService>();
            connectionsService.SetLogger(Logger);

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

                    if (!packetManager.TryParse(capturedPacket, out PokerMasterPackage package) || !IsAllowedPackage(package))
                    {
                        continue;
                    }

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

                                LogProvider.Log.Info(Logger, $"User {package.Uuid} left room {body.RoomId}.");
                            },
                            () => LogProvider.Log.Info(Logger, $"User {package.Uuid} left room."));

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
                           body => LogProvider.Log.Info(Logger, $"User {package.Uuid} entered room {body.RoomId}."),
                           () => LogProvider.Log.Info(Logger, $"User {package.Uuid} entered room."));

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

                            LogProvider.Log.Info(Logger, $"Encryption key not found [User {package.Uuid}].");

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
                            LogProvider.Log.Error(Logger, $"Package has not been decrypted. Relogin [User {package.Uuid}] is required. [{capturedPacket}] [{capturedPacket.SequenceNumber}]");

                            var base64Body = Convert.ToBase64String(package.Body);
                            var base64Key = Convert.ToBase64String(encryptKey);

                            LogProvider.Log.Error(Logger, $"Package body: [{base64Key}, {base64Body}]");

                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_Relogin", windowHandle);
                            continue;
                        }

                        if (newlyDetectedTable)
                        {
                            SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM", windowHandle);
                        }
#if DEBUG
                        File.AppendAllText($"Hands\\pm_hand_imported_{package.Uuid}_{scGameRoomStateChange.GameNumber}.json", JsonConvert.SerializeObject(scGameRoomStateChange, Formatting.Indented));
#endif

                        if (IsAdvancedLogEnabled)
                        {
                            protectedLogger.Log(JsonConvert.SerializeObject(scGameRoomStateChange, Formatting.Indented));
                        }

                        if (handBuilder.TryBuild(scGameRoomStateChange, package.Uuid, out HandHistory handHistory))
                        {
                            if (IsAdvancedLogEnabled)
                            {
                                LogProvider.Log.Info(Logger, $"Hand #{handHistory.HandId}: Found process={(process != null ? process.Id : 0)}, windows={windowHandle}.");
                            }

                            if (!pmCatcherService.CheckHand(handHistory))
                            {
                                if (handHistory.GameDescription.IsTournament)
                                {
                                    LogProvider.Log.Info(Logger, $"License doesn't support tourney hand {handHistory.HandId}. [buyin={handHistory.GameDescription.Tournament.BuyIn.PrizePoolValue}]");
                                }
                                else
                                {
                                    LogProvider.Log.Info(Logger, $"License doesn't support cash hand {handHistory.HandId}. [bb={handHistory.GameDescription.Limit.BigBlind}]");
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
                        LogProvider.Log.Error(Logger, $"Could not process captured packet.", e);
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
                LogProvider.Log.Info(Logger, $"Room info is empty.");
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
                LogProvider.Log.Info(Logger, $"More than 1 account detected in room {roomId}.");
                SendPreImporedData("Notifications_HudLayout_PreLoadingText_PM_AccountPerTable", windowHandle);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Parses login package
        /// </summary>
        /// <param name="package">Package to parse</param>
        private void ParseLoginPackage<T>(PokerMasterPackage package, Func<T, UserBaseInfoNet> getUserInfo, Func<T, string> getEncryptKey)
        {
            if (IsAdvancedLogEnabled)
            {
                LogProvider.Log.Info(Logger, $"Login package [{package.Cmd}] has been received.");
            }

            var bytes = BodyDecryptor.Decrypt(package.Body);

            if (bytes == null)
            {
                LogProvider.Log.Error(Logger, $"Failed to parse login package [{package.Cmd}].");
                return;
            }

            var scLoginRsp = SerializationHelper.Deserialize<T>(bytes);
            var userInfo = getUserInfo(scLoginRsp);

            if (scLoginRsp == null)
            {
                LogProvider.Log.Error(Logger, $"Login package is empty.");
                return;
            }

            if (userInfo == null)
            {
                LogProvider.Log.Error(Logger, $"UserInfo is missing.");
                return;
            }

            LogProvider.Log.Info(Logger, $"User [{userInfo.Uuid}, {userInfo.ShowID}] has logged in.");

            var encryptKeyText = getEncryptKey(scLoginRsp);

            if (string.IsNullOrWhiteSpace(encryptKeyText))
            {
                LogProvider.Log.Error(Logger, $"EncryptKey is missing.");
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
        private void ParsePackage<T>(PokerMasterPackage package, Action<T> onSuccess, Action onFail)
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
        private static bool IsAllowedPackage(PokerMasterPackage package)
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

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }      
    }
}