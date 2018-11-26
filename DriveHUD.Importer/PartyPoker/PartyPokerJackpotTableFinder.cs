//-----------------------------------------------------------------------
// <copyright file="PartyPokerJackpotTableFinder.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Log;
using System;
using System.IO;
using System.Text;

namespace DriveHUD.Importers.PartyPoker
{
    internal class PartyPokerJackpotTableFinder
    {
        private const string XmlHandHistoryFolder = "../../XMLHandHistory";
        private const string SearchPattern = "*.xml";

        public static string FindTableId(string tournamentId, string fileName)
        {
            try
            {
                var fileFolder = Path.GetDirectoryName(fileName);
                var xmlHandHistoryFolder = Path.Combine(fileFolder, XmlHandHistoryFolder);

                var xmlHandHistoryDirectory = new DirectoryInfo(xmlHandHistoryFolder);

                if (!xmlHandHistoryDirectory.Exists)
                {
                    LogProvider.Log.Warn(typeof(PartyPokerJackpotTableFinder), $"Directory with xml hand history for PP not found at '{xmlHandHistoryDirectory.FullName}'");
                    return string.Empty;
                }

                var xmlFiles = xmlHandHistoryDirectory.GetFiles(SearchPattern, SearchOption.TopDirectoryOnly);

                foreach (var xmlFile in xmlFiles)
                {
                    if (TryGetTableId(xmlFile, tournamentId, out string tableId))
                    {
                        return tableId;
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(typeof(PartyPokerJackpotTableFinder), $"Failed to find table id for tournament {tournamentId} and file '{fileName}'", e);
            }

            return string.Empty;
        }

        private static bool TryGetTableId(FileInfo file, string tournamentId, out string tableId)
        {
            tableId = string.Empty;

            try
            {
                string content;

                using (var fs = File.Open(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (var sr = new StreamReader(fs, Encoding.Unicode))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                if (string.IsNullOrEmpty(content))
                {
                    return false;
                }

                content = content.Replace("\r", string.Empty).Replace("\n", string.Empty);

                var tournamentIdIndex = content.IndexOf($"({tournamentId})", StringComparison.OrdinalIgnoreCase);

                if (tournamentIdIndex < 0)
                {
                    return false;
                }

                var tableIdIndexStart = content.LastIndexOf("id=\"", tournamentIdIndex);

                if (tableIdIndexStart < 0)
                {
                    return false;
                }

                var tableIdIndexEnd = content.IndexOf("\"", tableIdIndexStart + 4);

                tableId = content.Substring(tableIdIndexStart + 4, tableIdIndexEnd - tableIdIndexStart - 4);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}