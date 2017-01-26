﻿//-----------------------------------------------------------------------
// <copyright file="FileBasedImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Progress;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Settings;
using Model.Site;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    internal abstract class FileBasedImporter : BaseImporter
    {
        protected Dictionary<string, CapturedFile> capturedFiles;
        protected HashSet<string> filesToSkip;
        protected IEventAggregator eventAggregator;

        protected const int ReadingTimeout = 3000;

        public FileBasedImporter()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();
            capturedFiles = new Dictionary<string, CapturedFile>();
            filesToSkip = new HashSet<string>();
        }

        #region Properties

        protected abstract string ProcessName { get; }

        protected abstract string HandHistoryFilter { get; }

        protected virtual bool IsAdvancedLogEnabled { get; set; }

        #endregion

        // Import data from PS HH
        protected override void DoImport()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
            var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == Site);

            var handHistoryFolders = GetHandHistoryFolders(siteSettings);

            IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    // detect all *.txt files in directories
                    var handHistoryFiles = handHistoryFolders.Where(x => x.Exists).SelectMany(x => x.GetFiles(HandHistoryFilter, SearchOption.AllDirectories)).ToArray();

                    var newlyDetectedHandHistories = (from handHistoryFile in handHistoryFiles
                                                      join capturedFile in capturedFiles.Keys on handHistoryFile.FullName equals capturedFile into capturedFileGrouped
                                                      from item in capturedFileGrouped.DefaultIfEmpty()
                                                      where item == null && !filesToSkip.Contains(handHistoryFile.FullName)
                                                      select handHistoryFile).ToArray();

                    // add new files and lock them
                    foreach (var hh in newlyDetectedHandHistories)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        if (!capturedFiles.ContainsKey(hh.FullName))
                        {
                            var fs = File.Open(hh.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                            fs.Seek(0, SeekOrigin.Begin);

                            Encoding encoding;

                            try
                            {
                                encoding = EncodingDetector.DetectTextFileEncoding(fs, 0x10000) ?? Encoding.UTF8;
                            }
                            catch (Exception ex)
                            {
                                LogProvider.Log.Error(this, $"Couldn't detect encoding of {hh.FullName}. UTF8 will be used.", ex);
                                encoding = Encoding.UTF8;
                            }

                            fs.Seek(0, SeekOrigin.Begin);

                            var capturedFile = new CapturedFile
                            {
                                FileStream = fs,
                                Session = GetSessionForFile(hh.FullName),
                                Encoding = encoding
                            };

                            capturedFiles.Add(hh.FullName, capturedFile);
                        }
                    }

                    // try to parse file
                    foreach (var cf in capturedFiles)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        var fs = cf.Value.FileStream;

                        var handText = GetHandTextFromStream(fs, cf.Value.Encoding);

                        if (string.IsNullOrEmpty(handText))
                        {
                            continue;
                        }

                        // if file could not be parsed, mark it as invalid to prevent further processing 
                        if (cf.Value.GameInfo == null)
                        {
                            EnumPokerSites siteName;

                            if (!TryGetPokerSiteName(handText, out siteName) && !filesToSkip.Contains(cf.Key))
                            {
                                filesToSkip.Add(cf.Key);
                                fs.Close();

                                LogProvider.Log.Warn($"Cannot find parser for hand: {handText}");
                                LogProvider.Log.Warn(string.Format("File '{0}' has bad format. Skipped.", cf.Key));

                                continue;
                            }

                            var fileName = Path.GetFileNameWithoutExtension(cf.Key);

                            var gameInfo = new GameInfo
                            {
                                PokerSite = siteName,
                                Session = cf.Value.Session,
                                TournamentSpeed = ParserUtils.ParseNullableTournamentSpeed(fileName, null),
                                FileName = fileName
                            };

                            LogProvider.Log.Info($"Found '{cf.Key}' file. [{SiteString}]");

                            cf.Value.GameInfo = gameInfo;
                        }

                        ProcessHand(handText, cf.Value.GameInfo);
                    }

                    // remove invalid files from captured
                    filesToSkip.ForEach(fileToSkip =>
                    {
                        if (capturedFiles.ContainsKey(fileToSkip))
                        {
                            capturedFiles.Remove(fileToSkip);
                        }
                    });

                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        Task.Delay(ReadingTimeout).Wait();
                    }
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, string.Format("{0} auto-import failed", SiteString), e);
                }
            }

            Clean();

            RaiseProcessStopped();
        }

        protected virtual bool TryGetPokerSiteName(string handText, out EnumPokerSites siteName)
        {
            return EnumPokerSitesExtension.TryParse(handText, out siteName);
        }

        // Import hand
        protected virtual void ProcessHand(string handHistory, GameInfo gameInfo)
        {
            var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            try
            {
                parsingResult = ImportHand(handHistory, gameInfo, dbImporter, progress);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Hand(s) has not been imported. [{SiteString}]", e);
            }

            if (parsingResult == null)
            {
                return;
            }

            foreach (var result in parsingResult)
            {
                if (result.HandHistory == null)
                {
                    continue;
                }

                if (result.IsDuplicate)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate. [{1}]", result.HandHistory.Gamenumber, SiteString));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. [{1}]", result.HandHistory.Gamenumber, SiteString));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} has been imported. [{1}]", result.HandHistory.Gamenumber, SiteString));

                var playerList = GetPlayerList(result.Source);

                if (gameInfo.WindowHandle == 0)
                {
                    gameInfo.WindowHandle = FindWindow(result).ToInt32();
                }
                gameInfo.GameFormat = ParseGameFormat(result);
                gameInfo.GameType = ParseGameType(result);
                gameInfo.TableType = ParseTableType(result);
                gameInfo.GameNumber = result.HandHistory.Gamenumber;

                var dataImportedArgs = new DataImportedEventArgs(playerList, gameInfo);

                eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
            }
        }

        protected virtual string GetHandTextFromStream(Stream fs, Encoding encoding)
        {
            var data = new byte[fs.Length - fs.Position];

            if (data.Length == 0)
            {
                return string.Empty;
            }

            fs.Read(data, 0, data.Length);

            return encoding.GetString(data);
        }

        protected virtual IEnumerable<ParsingResult> ImportHand(string handHistory, GameInfo gameInfo, IFileImporter dbImporter, DHProgress progress)
        {
            return dbImporter.Import(handHistory, progress, gameInfo);
        }

        // Get directories with hand histories
        protected virtual DirectoryInfo[] GetHandHistoryFolders(SiteModel siteSettings)
        {
            DirectoryInfo[] dirs;

            if (siteSettings != null && siteSettings.HandHistoryLocationList != null && siteSettings.HandHistoryLocationList.Any())
            {
                dirs = siteSettings.HandHistoryLocationList.Select(x => new DirectoryInfo(x)).ToArray();
            }
            else
            {
                var site = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(SiteString);
                dirs = site.GetHandHistoryFolders().Select(x => new DirectoryInfo(x)).ToArray();
            }

            return dirs;
        }

        protected virtual void Clean()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            var isMove = settings.SiteSettings.IsProcessedDataLocationEnabled;
            var moveLocation = settings.SiteSettings.ProcessedDataLocation;

            foreach (var capturedFile in capturedFiles)
            {
                capturedFile.Value.FileStream.Close();

                if (isMove)
                {
                    try
                    {
                        if (!Directory.Exists(moveLocation))
                        {
                            Directory.CreateDirectory(moveLocation);
                        }

                        File.Move(capturedFile.Key, Path.Combine(moveLocation, new FileInfo(capturedFile.Key).Name));
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Warn(string.Format("File {0} could not be moved: {1}. [{2}]", capturedFile.Key, ex.Message, SiteString));
                    }
                }
            }

            capturedFiles.Clear();
        }

        protected virtual string GetSessionForFile(string fileName)
        {
            var random = RandomProvider.GetThreadRandom();
            var session = random.Next(100000, 999999);
            return session.ToString();
        }

        protected virtual IntPtr FindWindow(ParsingResult parsingResult)
        {
            try
            {
                var pokerClientProcesses = GetPokerClientProcesses();

                if (IsAdvancedLogEnabled)
                {
                    var processesNames = string.Join(", ", pokerClientProcesses.Select(x => x.ProcessName).ToArray());
                    LogProvider.Log.Info($"Possible client processes: {processesNames} [{SiteString}]");
                }

                var handles = new List<IntPtr>();

                foreach (var pokerClientProcess in pokerClientProcesses)
                {
                    foreach (ProcessThread thread in pokerClientProcess.Threads)
                    {
                        WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                        {
                            handles.Add(hWnd);
                            return true;
                        }, IntPtr.Zero);
                    }
                }                

                foreach (var handle in handles)
                {
                    var title = WinApi.GetWindowText(handle);

                    if (!string.IsNullOrEmpty(title))
                    {
                        if (Match(title, parsingResult))
                        {
                            return handle;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not find table '{parsingResult?.Source?.TableName}'. [{SiteString}]", e);
            }

            return IntPtr.Zero;
        }

        protected virtual GameFormat ParseGameFormat(ParsingResult parsingResult)
        {
            if (parsingResult.Source != null && parsingResult.Source.GameDescription != null && parsingResult.Source.GameDescription.IsTournament)
            {
                return parsingResult.TournamentsTags == TournamentsTags.MTT ? GameFormat.MTT : GameFormat.SnG;
            }

            return GameFormat.Cash;
        }

        protected virtual bool Match(string title, ParsingResult parsingResult)
        {
            var matchResult = InternalMatch(title, parsingResult);

            if (IsAdvancedLogEnabled)
            {
                var tableName = parsingResult?.Source?.TableName;
                LogProvider.Log.Info($"Checking if window '{title}' matches '{tableName}' table: {matchResult} [{SiteString}]");
            }

            return matchResult;
        }

        protected abstract bool InternalMatch(string title, ParsingResult parsingResult);

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process[] GetPokerClientProcesses()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcesses = processes.Where(x => x.ProcessName.Equals(ProcessName, StringComparison.OrdinalIgnoreCase)).ToArray();

            return pokerClientProcesses;
        }

        protected virtual Bovada.GameType ParseGameType(ParsingResult parsingResult)
        {
            if (parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return Bovada.GameType.Holdem;
            }

            switch (parsingResult.Source.GameDescription.GameType)
            {
                case HandHistories.Objects.GameDescription.GameType.CapPotLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.FixedLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.NoLimitOmaha:
                case HandHistories.Objects.GameDescription.GameType.PotLimitOmaha:
                    return Bovada.GameType.Omaha;
                case HandHistories.Objects.GameDescription.GameType.FiveCardPotLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.PotLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.NoLimitOmahaHiLo:
                case HandHistories.Objects.GameDescription.GameType.FixedLimitOmahaHiLo:
                    return Bovada.GameType.OmahaHiLo;
                default:
                    return Bovada.GameType.Holdem;
            }
        }

        protected virtual EnumTableType ParseTableType(ParsingResult parsingResult)
        {
            if (parsingResult.Source == null || parsingResult.Source.GameDescription == null)
            {
                return EnumTableType.Nine;
            }

            var tableType = (EnumTableType)parsingResult.Source.GameDescription.SeatType.MaxPlayers;
            return tableType;
        }

        protected virtual PlayerList GetPlayerList(HandHistory handHistory)
        {
            return handHistory.Players;
        }

        protected class CapturedFile
        {
            public Stream FileStream { get; set; }

            public string Session { get; set; }

            public Encoding Encoding { get; set; }

            public GameInfo GameInfo { get; set; }
        }
    }
}
