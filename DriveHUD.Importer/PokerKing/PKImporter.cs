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
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.Importers.AndroidBase;
using DriveHUD.Importers.Loggers;
using DriveHUD.Importers.PokerKing.Model;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PacketDotNet;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;

namespace DriveHUD.Importers.PokerKing
{
    internal class PKImporter : GenericImporter, IPKImporter
    {
        private readonly BlockingCollection<CapturedPacket> packetBuffer = new BlockingCollection<CapturedPacket>();

        private const int StopTimeout = 5000;

        private const int NoDataDelay = 100;

        private IPokerClientEncryptedLogger protectedLogger;

        protected override bool SupportDuplicates => true;

        protected override EnumPokerSites Site => EnumPokerSites.PokerKing;

        protected override string ProcessName => throw new NotSupportedException($"Process name isn't supported for importer. [{SiteString}]");

        #region Implementation of ITcpImporter

        public bool Match(TcpPacket tcpPacket, IpPacket ipPacket)
        {
            return tcpPacket.SourcePort == PKImporterHelper.PortFilter;
        }

        public void AddPacket(CapturedPacket capturedPacket)
        {
            packetBuffer.Add(capturedPacket);
        }

        public override bool IsDisabled()
        {
            return false;
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
                LogProvider.Log.Error(CustomModulesNames.PKCatcher, $"Failed importing.", e);
            }

            packetBuffer.Clear();

            protectedLogger.StopLogging();

            RaiseProcessStopped();
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
            var packetManager = ServiceLocator.Current.GetInstance<IPacketManager<PokerKingPackage>>();

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (!packetBuffer.TryTake(out CapturedPacket capturedPacket))
                    {
                        Task.Delay(NoDataDelay).Wait();
                        continue;
                    }

                    if (!packetManager.TryParse(capturedPacket, out PokerKingPackage package))
                    {
                        continue;
                    }

                    var logFile = Path.Combine("Logs", capturedPacket.ToString().Replace(":", ".").Replace("->", "-") + ".log");

                    switch (package.PackageType)
                    {
                        case PackageType.NoticePlayerActionTurn:
                            LogPackage<NoticePlayerActionTurn>(logFile, package);
                            break;
                        case PackageType.RequestLeaveRoom:
                            LogPackage<RequestLeaveRoom>(logFile, package);
                            break;
                        case PackageType.NoticeSitDown:
                            LogPackage<NoticeSitDown>(logFile, package);
                            break;
                        case PackageType.NoticeStandup:
                            LogPackage<NoticeSitDown>(logFile, package);
                            break;
                        case PackageType.NoticeStartGame:
                            LogPackage<NoticeStartGame>(logFile, package);
                            break;
                        case PackageType.NoticeResetGame:
                            LogPackage<NoticeResetGame>(logFile, package);
                            break;
                        case PackageType.NoticeGamePost:
                            LogPackage<NoticeGamePost>(logFile, package);
                            break;
                        case PackageType.NoticeGameAnte:
                            LogPackage<NoticeGameAnte>(logFile, package);
                            break;
                        case PackageType.NoticeGameElectDealer:
                            LogPackage<NoticeGameElectDealer>(logFile, package);
                            break;
                        case PackageType.NoticeGameBlind:
                            LogPackage<NoticeGameBlind>(logFile, package);
                            break;
                        case PackageType.NoticeGameHoleCard:
                            LogPackage<NoticeGameHoleCard>(logFile, package);
                            break;
                        case PackageType.NoticePlayerAction:
                            LogPackage<NoticePlayerAction>(logFile, package);
                            break;
                        case PackageType.NoticeGameRoundEnd:
                            LogPackage<NoticePlayerAction>(logFile, package);
                            break;
                        case PackageType.NoticeGameCommunityCards:
                            LogPackage<NoticeGameCommunityCards>(logFile, package);
                            break;
                        case PackageType.NoticeGameShowCard:
                            LogPackage<NoticeGameShowCard>(logFile, package);
                            break;
                        case PackageType.NoticeGameSettlement:
                            LogPackage<NoticeGameSettlement>(logFile, package);
                            break;
                        case PackageType.NoticeGameShowDown:
                            LogPackage<NoticeGameShowDown>(logFile, package);
                            break;
                        case PackageType.NoticePlayerStayPosition:
                            LogPackage<NoticePlayerStayPosition>(logFile, package);
                            break;
                        case PackageType.NoticePlayerShowCard:
                            LogPackage<NoticePlayerShowCard>(logFile, package);
                            break;
                        case PackageType.NoticeBuyin:
                            LogPackage<NoticeBuyin>(logFile, package);
                            break;
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, $"Could not process captured packet.", e);
                }
            }
        }

        private void LogPackage<T>(string logFile, PokerKingPackage package)
        {
            if (!SerializationHelper.TryDeserialize(package.Body, out T playerActionTurn))
            {
                LogProvider.Log.Warn(this, $"Failed to deserialize {typeof(T)} package");
            }

            var packageJson = new PackageJson<T>
            {
                PackageType = package.PackageType,
                Message = playerActionTurn,
                Time = DateTime.Now
            };

            var json = JsonConvert.SerializeObject(packageJson, Formatting.Indented, new StringEnumConverter());

            File.AppendAllText(logFile, $"{json}{Environment.NewLine}");
        }

        private class PackageJson<T>
        {
            public PackageType PackageType { get; set; }

            public DateTime Time { get; set; }

            public T Message { get; set; }
        }

        protected override bool InternalMatch(string title, IntPtr handle, ParsingResult parsingResult)
        {
            return false;
        }
    }
}