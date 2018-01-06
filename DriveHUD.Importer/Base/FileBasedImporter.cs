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
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using HandHistories.Parser.Utils.Extensions;
using HandHistories.Parser.Utils.FastParsing;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveHUD.Importers
{
    internal abstract class FileBasedImporter : GenericImporter
    {
        protected Dictionary<string, CapturedFile> capturedFiles;
        protected HashSet<string> filesToSkip;
        protected IDataService dataService;

        protected const int ReadingTimeout = 3000;

        public FileBasedImporter()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            capturedFiles = new Dictionary<string, CapturedFile>();
            filesToSkip = new HashSet<string>();
        }

        #region Properties     

        protected abstract string HandHistoryFilter { get; }

        #endregion

        // Import data from PS HH
        protected override void DoImport()
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
                    var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == Site);

                    if (siteSettings != null && !siteSettings.Enabled)
                    {
                        break;
                    }

                    var handHistoryFolders = GetHandHistoryFolders(siteSettings);

                    IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;

                    // detect all *.txt files in directories
                    var handHistoryFiles = handHistoryFolders.Where(x => x.Exists).SelectMany(x => x.GetFiles(HandHistoryFilter, SearchOption.AllDirectories)).ToArray();

                    var newlyDetectedHandHistories = (from handHistoryFile in handHistoryFiles
                                                      join capturedFile in capturedFiles.Keys on handHistoryFile.FullName equals capturedFile into capturedFileGrouped
                                                      from item in capturedFileGrouped.DefaultIfEmpty()
                                                      where item == null && !filesToSkip.Contains(handHistoryFile.FullName)
                                                      select handHistoryFile).ToArray();

                    // check if files were processed
                    if (newlyDetectedHandHistories.Length > 0)
                    {
                        var newlyDetectedHandHistoriesNames = newlyDetectedHandHistories.Select(x => x.FullName).ToArray();

                        var processedFiles = dataService.GetImportedFiles(newlyDetectedHandHistoriesNames);

                        var processedFilesToSkip = (from processedFile in processedFiles
                                                    join newlyDetectedHandHistory in newlyDetectedHandHistories on
                                                        new
                                                        {
                                                            FileName = processedFile.FileName,
                                                            FileSize = processedFile.FileSize,
                                                            LastWrite = DateTime.SpecifyKind(processedFile.LastWriteTime, DateTimeKind.Utc)
                                                        } equals
                                                        new
                                                        {
                                                            FileName = newlyDetectedHandHistory.FullName,
                                                            FileSize = newlyDetectedHandHistory.Length,
                                                            LastWrite = new DateTime(newlyDetectedHandHistory.LastWriteTimeUtc.Year,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Month,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Day,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Hour,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Minute,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Second,
                                                                newlyDetectedHandHistory.LastWriteTimeUtc.Kind)
                                                        }
                                                    select newlyDetectedHandHistory).ToArray();

                        if (processedFilesToSkip.Length > 0)
                        {
                            newlyDetectedHandHistories = newlyDetectedHandHistories
                                .Except(processedFilesToSkip, new LambdaComparer<FileInfo>((x, y) => x.FullName == y.FullName))
                                .ToArray();
                        }
                    }

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
                                Encoding = encoding,
                                ImportedFile = new ImportedFile
                                {
                                    FileName = hh.FullName
                                }
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

                        var handText = GetHandTextFromStream(fs, cf.Value.Encoding, cf.Key);

                        if (string.IsNullOrEmpty(handText))
                        {
                            continue;
                        }

                        // update size and last write time
                        cf.Value.ImportedFile.FileSize = fs.Length;
                        cf.Value.ImportedFile.LastWriteTime = File.GetLastWriteTimeUtc(cf.Value.ImportedFile.FileName);
                        cf.Value.WasModified = true;

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
                                FileName = fileName,
                                FullFileName = cf.Key
                            };

                            LogProvider.Log.Info($"Found '{cf.Key}' file. [{SiteString}]");

                            cf.Value.GameInfo = gameInfo;
                        }

                        ProcessHand(handText, cf.Value.GameInfo);
                    }

                    // update files info
                    // check if file was modified since last check
                    var modifiedCapturedFiles = capturedFiles.Values.Where(x => x.WasModified).ToArray();

                    if (modifiedCapturedFiles.Length > 0)
                    {
                        using (var session = ModelEntities.OpenSession())
                        {
                            using (var transaction = session.BeginTransaction())
                            {
                                var capturedImportedFileNames = modifiedCapturedFiles.Select(x => x.ImportedFile.FileName).ToArray();

                                var existingImportedFiles = dataService.GetImportedFiles(capturedImportedFileNames, session);

                                var capturedFilesWithExisting = (from capturedFile in modifiedCapturedFiles
                                                                 join existingImportedFile in existingImportedFiles on capturedFile.ImportedFile.FileName
                                                                    equals existingImportedFile.FileName into gj
                                                                 from existingImportedFile in gj.DefaultIfEmpty()
                                                                 select new { CapturedFile = capturedFile, ExistingImportedFile = existingImportedFile }).ToArray();

                                capturedFilesWithExisting.ForEach(x =>
                                {
                                    if (x.ExistingImportedFile != null)
                                    {
                                        x.ExistingImportedFile.FileSize = x.CapturedFile.ImportedFile.FileSize;
                                        x.ExistingImportedFile.LastWriteTime = x.CapturedFile.ImportedFile.LastWriteTime;

                                        x.CapturedFile.ImportedFile = x.ExistingImportedFile;
                                    }

                                    session.Save(x.CapturedFile.ImportedFile);

                                    x.CapturedFile.WasModified = false;
                                });

                                transaction.Commit();
                            }
                        }
                    }

                    // remove invalid files from captured
                    filesToSkip.ForEach(fileToSkip =>
                    {
                        if (capturedFiles.ContainsKey(fileToSkip))
                        {
                            capturedFiles[fileToSkip].FileStream?.Close();
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

        protected virtual string GetHandTextFromStream(Stream fs, Encoding encoding, string fileName)
        {
            if (fs.Position > fs.Length)
            {
                fs.Seek(0, SeekOrigin.End);
                return string.Empty;
            }
        
            var data = new byte[fs.Length - fs.Position];

            if (data.Length == 0)
            {
                return string.Empty;
            }

            fs.Read(data, 0, data.Length);

            return encoding.GetString(data);
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
                dirs = new DirectoryInfo[0];
            }

            return dirs;
        }

        protected virtual void Clean()
        {
            var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();

            var isMove = settings.SiteSettings.IsProcessedDataLocationEnabled && !string.IsNullOrEmpty(settings.SiteSettings.ProcessedDataLocation);

            var siteFolder = Site.ToString();
            var dateFolder = DateTime.Now.ToString("yyyy-MM");
            var moveLocation = Path.Combine(settings.SiteSettings.ProcessedDataLocation, siteFolder, dateFolder);

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

        protected class CapturedFile
        {
            public Stream FileStream { get; set; }

            public string Session { get; set; }

            public Encoding Encoding { get; set; }

            public GameInfo GameInfo { get; set; }

            public ImportedFile ImportedFile { get; set; }

            public bool WasModified { get; set; }
        }
    }
}