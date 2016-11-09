//-----------------------------------------------------------------------
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
using HandHistories.Parser.Parsers;
using HandHistories.Parser.Utils.Extensions;
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
        protected Process pokerClientProcess;

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

        protected virtual Encoding ByteEncoder
        {
            get { return Encoding.UTF8; }
        }

        #endregion

        // Import data from PS HH
        protected override void DoImport()
        {
            var handHistoryFolders = GetHandHistoryFolders();

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
                    newlyDetectedHandHistories.ForEach(hh =>
                    {
                        var fs = File.Open(hh.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        fs.Seek(0, SeekOrigin.Begin);

                        var capturedFile = new CapturedFile
                        {
                            FileStream = fs,
                            Session = GenerateSession()
                        };

                        capturedFiles.Add(hh.FullName, capturedFile);
                    });

                    // try to parse file
                    capturedFiles.ForEach(cf =>
                    {
                        var fs = cf.Value.FileStream;

                        var data = new byte[fs.Length - fs.Position];

                        if (data.Length == 0)
                        {
                            return;
                        }

                        fs.Read(data, 0, data.Length);

                        var handText = ByteEncoder.GetString(data);

                        // if file could not be parsed, mark it as invalid to prevent further processing 
                        if (cf.Value.GameInfo == null)
                        {
                            EnumPokerSites siteName;

                            if (!EnumPokerSitesExtension.TryParse(handText, out siteName) && !filesToSkip.Contains(cf.Key))
                            {
                                filesToSkip.Add(cf.Key);
                                fs.Close();

                                LogProvider.Log.Warn(string.Format("File '{0}' has bad format. Skipped.", cf.Key));

                                return;
                            }

                            var gameInfo = new GameInfo
                            {
                                PokerSite = siteName,
                                Session = cf.Value.Session
                            };

                            LogProvider.Log.Info(string.Format("Found '{0}' file.", cf.Key));

                            cf.Value.GameInfo = gameInfo;
                        }

                        ImportHand(handText, cf.Value.GameInfo);
                    });

                    // remove invalid files from captured
                    filesToSkip.ForEach(fileToSkip =>
                    {
                        if (capturedFiles.ContainsKey(fileToSkip))
                        {
                            capturedFiles.Remove(fileToSkip);
                        }
                    });

                    Task.Delay(ReadingTimeout).Wait();
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, string.Format("{0} auto-import failed", Site), e);
                }
            }

            Clean();

            RaiseProcessStopped();
        }

        // Import hand
        protected virtual void ImportHand(string handHistory, GameInfo gameInfo)
        {
            var dbImporter = ServiceLocator.Current.GetInstance<IFileImporter>();
            var progress = new DHProgress();

            IEnumerable<ParsingResult> parsingResult = null;

            try
            {
                parsingResult = dbImporter.Import(handHistory, progress, gameInfo);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, string.Format("Hand(s) has not been imported"), e);
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
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported. Duplicate.", result.HandHistory.Gamenumber));
                    continue;
                }

                if (!result.WasImported)
                {
                    LogProvider.Log.Info(this, string.Format("Hand {0} has not been imported.", result.HandHistory.Gamenumber));
                    continue;
                }

                LogProvider.Log.Info(this, string.Format("Hand {0} imported", result.HandHistory.Gamenumber));

                var playerList = result.Source.Players;

                gameInfo.WindowHandle = FindWindow(result).ToInt32();
                gameInfo.GameFormat = ParseGameFormat(result);
                gameInfo.GameType = ParseGameType(result);
                gameInfo.TableType = ParseTableType(result);

                var dataImportedArgs = new DataImportedEventArgs(playerList, gameInfo);

                eventAggregator.GetEvent<DataImportedEvent>().Publish(dataImportedArgs);
            }
        }

        // Get directories with hand histories
        protected virtual DirectoryInfo[] GetHandHistoryFolders()
        {
            var siteSettings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings()
                .SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite.ToString() == this.Site);

            DirectoryInfo[] dirs;
            if (siteSettings != null && siteSettings.HandHistoryLocationList != null && siteSettings.HandHistoryLocationList.Any())
            {
                dirs = siteSettings.HandHistoryLocationList.Select(x => new DirectoryInfo(x)).ToArray();
            }
            else
            {
                var site = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(Site);
                dirs = site.GetHandHistoryFolders().Select(x => new DirectoryInfo(x)).ToArray();
            }

            return dirs;
        }

        protected virtual void Clean()
        {
            foreach (var capturedFile in capturedFiles)
            {
                capturedFile.Value.FileStream.Close();

                try
                {
                    File.Delete(capturedFile.Key);
                }
                catch
                {
                    LogProvider.Log.Warn(string.Format("File {0} could not be deleted", capturedFile.Key));
                }
            }

            capturedFiles.Clear();
        }

        protected virtual string GenerateSession()
        {
            var random = RandomProvider.GetThreadRandom();
            var session = random.Next(100000, 999999);
            return session.ToString();
        }

        protected virtual IntPtr FindWindow(ParsingResult parsingResult)
        {
            try
            {
                if (pokerClientProcess == null || pokerClientProcess.HasExited)
                {
                    pokerClientProcess = GetPokerClientProcess();

                    if (pokerClientProcess == null)
                    {
                        return IntPtr.Zero;
                    }
                }

                var handles = new List<IntPtr>();

                foreach (ProcessThread thread in pokerClientProcess.Threads)
                {
                    WinApi.EnumThreadWindows(thread.Id, (hWnd, lParam) =>
                    {
                        handles.Add(hWnd);
                        return true;
                    }, IntPtr.Zero);
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
                LogProvider.Log.Error(this, "Could not find PokerStars table", e);
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
            var tableName = parsingResult.Source.TableName;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }

            var titleSplitted = title.Split('-');

            return titleSplitted.Length > 2 &&
                    titleSplitted[0].StartsWith(tableName.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get client process
        /// </summary>
        /// <returns>Client process if exist, otherwise - null</returns>
        protected virtual Process GetPokerClientProcess()
        {
            var processes = Process.GetProcesses();

            var pokerClientProcess = processes.FirstOrDefault(x => x.ProcessName.Equals(ProcessName));

            return pokerClientProcess;
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

        protected class CapturedFile
        {
            public Stream FileStream { get; set; }

            public string Session { get; set; }

            public GameInfo GameInfo { get; set; }
        }
    }
}
