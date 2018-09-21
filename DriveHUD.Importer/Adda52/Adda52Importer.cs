//-----------------------------------------------------------------------
// <copyright file="Adda52Importer.cs" company="Ace Poker Solutions">
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
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers.Adda52.Model;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.Helpers;
using DriveHUD.Importers.Loggers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy.Http;

namespace DriveHUD.Importers.Adda52
{
    internal class Adda52Importer : GenericImporter, IAdda52Importer
    {
        private const int NoDataDelay = 200;

        private readonly BlockingCollection<CapturedPacket> packetBuffer = new BlockingCollection<CapturedPacket>();

        protected IPokerClientEncryptedLogger protectedLogger;

        protected readonly ISettingsService settings;
        protected readonly IAdda52TableService tableService;

        private uint sequenceNumber = 0;

        public Adda52Importer()
        {
            settings = ServiceLocator.Current.GetInstance<ISettingsService>();
            tableService = ServiceLocator.Current.GetInstance<IAdda52TableService>();
        }

        protected override string[] ProcessNames => new string[0];

        protected override EnumPokerSites Site => EnumPokerSites.Adda52;

        protected override bool IsAdvancedLogEnabled
        {
            get
            {
                return settings.GetSettings()?.GeneralSettings.IsAdvancedLoggingEnabled ?? false;
            }
        }

        #region IProxyPacketImporter implementation

        public bool IsMatch(Request request)
        {
            return request.RequestUri.Port == 8893 &&
                request.RequestUri.Host.Contains("adda52.com") &&
                request.RequestUri.AbsolutePath.Contains("websocket");
        }

        public void AddPacket(CapturedPacket capturedPacket)
        {
            unchecked
            {
                sequenceNumber++;
            }

            capturedPacket.SequenceNumber = sequenceNumber;

            packetBuffer.Add(capturedPacket);
        }

        #endregion

        protected override void DoImport()
        {
            try
            {
                InitializeLogger();
                ProcessBuffer();
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to process imported data. [{SiteString}]", e);
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
                LogCleanupTemplate = "adda-games-*-*-*.log",
                LogDirectory = "Logs",
                LogTemplate = "adda-games-{0}.log",
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
            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager<Adda52Package>>();
            var handBuilder = ServiceLocator.Current.GetInstance<IAdda52HandBuilder>();

            while (!cancellationTokenSource.IsCancellationRequested && !IsDisabled())
            {
                try
                {
                    if (!packetBuffer.TryTake(out CapturedPacket capturedPacket))
                    {
                        Task.Delay(NoDataDelay).Wait();
                        continue;
                    }

                    if (!packetManager.TryParse(capturedPacket, out IList<Adda52Package> packages))
                    {
                        continue;
                    }

                    foreach (var package in packages)
                    {
                        if (IsAdvancedLogEnabled)
                        {
                            LogPackage(package);
                        }

                        if (!Adda52JsonPackage.TryParse(package.Bytes, out Adda52JsonPackage jsonPackage))
                        {
                            continue;
                        }

                        if (!handBuilder.TryBuild(jsonPackage, out HandHistory handHistory))
                        {
                            continue;
                        }

                        var handHistoryText = SerializationHelper.SerializeObject(handHistory);

#if DEBUG
                        if (!Directory.Exists("Hands"))
                        {
                            Directory.CreateDirectory("Hands");
                        }

                        File.WriteAllText($"Hands\\adda52_hand_exported_{handHistory.HandId}.xml", handHistoryText);
#endif
                        var windowHandle = tableService.GetWindow(handHistory);

                        var gameInfo = new GameInfo
                        {
                            WindowHandle = windowHandle.ToInt32(),
                            PokerSite = Site,
                            GameNumber = handHistory.HandId,
                            Session = windowHandle.ToString()
                        };

                        ProcessHand(handHistoryText, gameInfo);
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Failed to process captured packet.", e);
                }
            }
        }

        protected override IntPtr FindWindow(ParsingResult parsingResult)
        {
            return parsingResult != null ? tableService.GetWindow(parsingResult.Source) : IntPtr.Zero;
        }

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }

        protected override PlayerList GetPlayerList(HandHistory handHistory, GameInfo gameInfo)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                        SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == Site)?.PrefferedSeats;

                var prefferedSeat = preferredSeats?.FirstOrDefault(x => (int)x.TableType == maxPlayers && x.IsPreferredSeatEnabled);

                if (prefferedSeat != null)
                {
                    var shift = (prefferedSeat.PreferredSeat - heroSeat) % maxPlayers;

                    foreach (var player in playerList)
                    {
                        player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
                    }
                }
            }

            return playerList;
        }

        #region Debug logging

        private void LogPacket(CapturedPacket capturedPacket, string ext)
        {
            var packageFileName = Path.Combine("Logs", capturedPacket.ToString().Replace(":", ".").Replace("->", "-")) + ext;

            var packetManager = new Adda52PacketManager();

            var sb = new StringBuilder();
            sb.AppendLine("-----------begin----------------");
            sb.AppendLine($"Date: {capturedPacket.CreatedTimeStamp}");
            sb.AppendLine($"Date Now: {DateTime.Now}");
            sb.AppendLine($"Date Now (ticks): {DateTime.Now.Ticks}");
            sb.AppendLine($"SequenceNumber: {capturedPacket.SequenceNumber}");
            sb.AppendLine($"Packet Full Length: {(packetManager.IsStartingPacket(capturedPacket.Bytes) ? packetManager.ReadPacketLength(capturedPacket.Bytes) : 0)}");
            sb.AppendLine($"Packet Current Length: {capturedPacket.Bytes.Length}");
            sb.AppendLine($"Body:");
            sb.AppendLine("---------body begin-------------");
            sb.AppendLine(BitConverter.ToString(capturedPacket.Bytes).Replace("-", " "));
            sb.AppendLine("----------body end--------------");
            sb.AppendLine("---------text body begin-------------");
            sb.AppendLine(Encoding.UTF8.GetString(capturedPacket.Bytes));
            sb.AppendLine("----------text body end--------------");
            sb.AppendLine("------------end-----------------");

            File.AppendAllText(packageFileName, sb.ToString());
        }

        private void LogPackage(Adda52Package package)
        {
            try
            {
                var contentJson = Encoding.UTF8.GetString(package.Bytes);

                var ignoreList = new[] { "game.keepAlive", "game.Message", "game.usercount", "game.avgstack", "game.account" };

                if (ignoreList.Any(x => contentJson.Contains(x)))
                {
                    return;
                }
#if DEBUG
                Console.WriteLine(contentJson);

                if (contentJson.Contains("game.started"))
                {
                    protectedLogger.Log("-------------------------------------------------------------------------------------");
                }
#endif
                protectedLogger.Log(contentJson);
#if DEBUG
                if (contentJson.Contains("game.roundend"))
                {
                    protectedLogger.Log("-------------------------------------------------------------------------------------");
                }
#endif
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Failed to log package. [{SiteString}]", e);
            }
        }

        #endregion
    }
}