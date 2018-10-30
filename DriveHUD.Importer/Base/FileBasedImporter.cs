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
using System.Collections.Concurrent;
using System.Threading;

namespace DriveHUD.Importers
{
    internal abstract class FileBasedImporter : GenericImporter
    {
        protected readonly ManualResetEvent importingResetEvent = new ManualResetEvent(false);

        protected readonly ConcurrentDictionary<string, CapturedFile> actualCapturedFiles;
        protected readonly ConcurrentDictionary<string, CapturedFile> notActualCapturedFiles;

        private readonly HashSet<string> filesToSkip;

        protected readonly IDataService dataService;

        protected const int ReadingTimeout = 3000;

        protected const int ActualFileLifeTime = 1200;

        protected static readonly object dbLocker = new object();

        public FileBasedImporter()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            actualCapturedFiles = new ConcurrentDictionary<string, CapturedFile>();
            notActualCapturedFiles = new ConcurrentDictionary<string, CapturedFile>();
            filesToSkip = new HashSet<string>();
        }

        #region Properties     

        protected abstract string HandHistoryFilter { get; }

        #endregion

        // Import data from PS HH
        protected override void DoImport()
        {
            importingResetEvent.Reset();

            var isActualTaskRunning = false;
            var isNotActualTaskRunning = false;

            while (!cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    var settings = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings();
                    var siteSettings = settings.SiteSettings.SitesModelList?.FirstOrDefault(x => x.PokerSite == Site);

                    if (siteSettings != null && !siteSettings.Enabled)
                    {
                        Stop();
                        continue;
                    }

                    var handHistoryFolders = GetHandHistoryFolders(siteSettings);

                    IsAdvancedLogEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;

                    // detect all *.txt files in directories
                    var handHistoryFiles = handHistoryFolders
                        .Where(x => x.Exists)
                        .SelectMany(x => x.GetFiles(HandHistoryFilter, SearchOption.AllDirectories))
                        .DistinctBy(x => x.FullName.ToLower())
                        .ToArray();

                    var capturedFiles = actualCapturedFiles.Select(x => x.Value).Concat(notActualCapturedFiles.Select(x => x.Value)).ToArray();

                    var newlyDetectedHandHistories = (from handHistoryFile in handHistoryFiles
                                                      join capturedFile in capturedFiles on handHistoryFile.FullName equals capturedFile.ImportedFile.FileName into capturedFileGrouped
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

                        if (!actualCapturedFiles.ContainsKey(hh.FullName) &&
                            !notActualCapturedFiles.ContainsKey(hh.FullName))
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
                                LogProvider.Log.Error(this, $"Couldn't detect encoding of {hh.FullName}. UTF8 will be used. [{SiteString}]", ex);
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

                            var lastWriteTime = File.GetLastWriteTimeUtc(hh.FullName);

                            if (lastWriteTime > DateTime.UtcNow.AddSeconds(-ActualFileLifeTime))
                            {
                                actualCapturedFiles.TryAdd(hh.FullName, capturedFile);
                            }
                            else
                            {
                                notActualCapturedFiles.TryAdd(hh.FullName, capturedFile);
                            }
                        }
                    }

                    if (!isActualTaskRunning)
                    {
                        ImportCapturedFiles(actualCapturedFiles, true);
                        isActualTaskRunning = true;
                    }

                    if (!isNotActualTaskRunning)
                    {
                        ImportCapturedFiles(notActualCapturedFiles, false);
                        isNotActualTaskRunning = true;
                    }

                    if (!cancellationTokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            Task.Delay(ReadingTimeout).Wait(cancellationTokenSource.Token);
                        }
                        catch (OperationCanceledException)
                        {
                        }
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capturedFiles"></param>
        protected virtual void ImportCapturedFiles(ConcurrentDictionary<string, CapturedFile> capturedFiles, bool isHighestPriority)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                return;
            }

            Task.Run(() =>
            {
                while (!cancellationTokenSource.IsCancellationRequested)
                {
                    foreach (var capturedFile in capturedFiles)
                    {
                        if (!isHighestPriority)
                        {
                            importingResetEvent.WaitOne();
                        }
                        else
                        {
                            importingResetEvent.Reset();
                        }

                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            if (isHighestPriority)
                            {
                                importingResetEvent.Set();
                            }

                            return;
                        }

                        ProcessCapturedFile(capturedFile.Key, capturedFile.Value, isHighestPriority);

                        if (!capturedFile.Value.WasModified || capturedFile.Value.GameInfo == null)
                        {
                            continue;
                        }

                        // modify captured file
                        using (var session = ModelEntities.OpenSession())
                        {
                            lock (dbLocker)
                            {
                                try
                                {
                                    using (var transaction = session.BeginTransaction())
                                    {
                                        var existingImportedFile = dataService.GetImportedFiles(new[] { capturedFile.Key }, session).FirstOrDefault();

                                        if (existingImportedFile != null)
                                        {
                                            existingImportedFile.FileSize = capturedFile.Value.ImportedFile.FileSize;
                                            existingImportedFile.LastWriteTime = capturedFile.Value.ImportedFile.LastWriteTime;
                                            capturedFile.Value.ImportedFile = existingImportedFile;
                                        }

                                        session.Save(capturedFile.Value.ImportedFile);

                                        capturedFile.Value.WasModified = false;

                                        transaction.Commit();
                                    }
                                }
                                // ignore errors if transaction is locked too long
                                catch
                                {
                                }
                            }
                        }
                    }

                    // remove invalid files from the list of files
                    filesToSkip.ForEach(fileToSkip =>
                    {
                        if (!capturedFiles.ContainsKey(fileToSkip))
                        {
                            return;
                        }

                        if (capturedFiles.TryRemove(fileToSkip, out CapturedFile removedCapturedFile))
                        {
                            removedCapturedFile?.FileStream?.Close();
                        }
                    });

                    try
                    {
                        if (isHighestPriority)
                        {
                            importingResetEvent.Set();
                        }

                        Task.Delay(ReadingTimeout).Wait(cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                }

                if (isHighestPriority)
                {
                    importingResetEvent.Set();
                }
            });
        }

        protected virtual void ProcessCapturedFile(string file, CapturedFile capturedFile, bool isHighestPriority)
        {
            try
            {
                var fs = capturedFile.FileStream;

                var handText = GetHandTextFromStream(fs, capturedFile.Encoding, file);

                if (string.IsNullOrEmpty(handText))
                {
                    return;
                }

                // update size and last write time
                capturedFile.ImportedFile.FileSize = fs.Length;
                capturedFile.ImportedFile.LastWriteTime = File.GetLastWriteTimeUtc(capturedFile.ImportedFile.FileName);
                capturedFile.WasModified = true;

                // if file could not be parsed, mark it as invalid to prevent further processing 
                if (capturedFile.GameInfo == null)
                {
                    if (!TryGetPokerSiteName(handText, out EnumPokerSites siteName) && !filesToSkip.Contains(file))
                    {
                        lock (filesToSkip)
                        {
                            filesToSkip.Add(file);
                        }

                        fs.Close();

                        LogProvider.Log.Warn($"Cannot find parser for hand: {handText} [{SiteString}]");
                        LogProvider.Log.Warn($"File '{file}' has bad format. Skipped. [{SiteString}]");

                        return;
                    }

                    var fileName = Path.GetFileNameWithoutExtension(file);

                    var gameInfo = new GameInfo
                    {
                        PokerSite = siteName,
                        Session = capturedFile.Session,
                        TournamentSpeed = ParserUtils.ParseNullableTournamentSpeed(fileName, null),
                        FileName = fileName,
                        FullFileName = file
                    };

                    LogProvider.Log.Info(this, $"Found '{file}' file. [{(isHighestPriority ? "High" : "Low")}] [{SiteString}]");

                    capturedFile.GameInfo = gameInfo;
                }

                ProcessHand(handText, capturedFile.GameInfo);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not process file {file}. [{(isHighestPriority ? "High" : "Low")}] [{SiteString}]", e);
            }
        }

        protected virtual bool TryGetPokerSiteName(string handText, out EnumPokerSites siteName)
        {
            return EnumPokerSitesExtension.TryParse(handText, out siteName);
        }

        protected virtual string GetHandTextFromStream(Stream fs, Encoding encoding, string fileName)
        {
            try
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
            catch (ObjectDisposedException)
            {
            }

            return string.Empty;
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

            var capturedFiles = actualCapturedFiles.Values.Concat(notActualCapturedFiles.Values);

            foreach (var capturedFile in capturedFiles)
            {
                capturedFile.FileStream.Close();

                if (isMove && capturedFile.GameInfo != null)
                {
                    try
                    {
                        if (!Directory.Exists(moveLocation))
                        {
                            Directory.CreateDirectory(moveLocation);
                        }

                        File.Move(capturedFile.ImportedFile.FileName, Path.Combine(moveLocation, Path.GetFileName(capturedFile.ImportedFile.FileName)));
                        LogProvider.Log.Info(this, $"File {capturedFile.ImportedFile.FileName} was moved to processed files [{SiteString}]");
                    }
                    catch (Exception ex)
                    {
                        LogProvider.Log.Warn(string.Format("File {0} could not be moved: {1}. [{2}]", capturedFile.ImportedFile.FileName, ex.Message, SiteString));
                    }
                }
            }

            actualCapturedFiles.Clear();
            notActualCapturedFiles.Clear();
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