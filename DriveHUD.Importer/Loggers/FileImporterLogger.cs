//-----------------------------------------------------------------------
// <copyright file="FileImporterLogger.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;
using System.Text;
using System.IO;

namespace DriveHUD.Importers.Loggers
{
    internal class FileImporterLogger : IFileImporterLogger
    {
        private const string logFolder = "Logs\\Import";
        private const string logFileNamePattern = "bad-hand-{0}.txt";
        private const string logFileIdPattern = "yyyy-MM-dd-HH-mm-ss";

        public void Log(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            try
            {
                PrepareLogDirectory();
                WriteLog(text);
            }
            catch
            {
            }
        }

        private void PrepareLogDirectory()
        {
            var directory = new DirectoryInfo(logFolder);

            if (!directory.Exists)
            {
                directory.Create();
            }
        }

        private void WriteLog(string text)
        {
            var fileId = GenerateFileId();
            var logFile = GenerateFileName(fileId);

            File.WriteAllText(logFile, text, Encoding.UTF8);
        }

        private static string GenerateFileId()
        {
            var fileId = DateTime.UtcNow.ToString(logFileIdPattern);
            return fileId;
        }

        private static string GetLogFileName(string fileId)
        {
            var logFileName = string.Format(logFileNamePattern, fileId);
            var logFile = Path.Combine(logFolder, logFileName);
            return logFile;
        }

        private string GenerateFileName(string fileId)
        {
            var logFile = GetLogFileName(fileId);

            if (!File.Exists(logFile))
            {
                return logFile;
            }

            var counter = 1;

            while (File.Exists(logFile) && counter < 100)
            {
                var fileIdWithCounter = string.Format("{0}-{1}", fileId, counter);

                logFile = GetLogFileName(fileIdWithCounter);

                counter++;
            }

            return logFile;
        }
    }
}