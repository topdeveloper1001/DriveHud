//-----------------------------------------------------------------------
// <copyright file="IPokerImporter.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Progress;
using DriveHUD.Entities;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Hand;
using HandHistories.Objects.Players;
using HandHistories.Parser.Parsers;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace DriveHUD.Importers.IPoker
{
    internal class IPokerImporter : FileBasedImporter, IIPokerImporter
    {
        protected override string HandHistoryFilter
        {
            get
            {
                return "*.xml";
            }
        }

        protected override string ProcessName
        {
            get
            {
                return "pokerclient";
            }
        }

        protected override EnumPokerSites Site
        {
            get
            {
                return EnumPokerSites.IPoker;
            }
        }

        protected override bool InternalMatch(string title, ParsingResult parsingResult)
        {
            if (string.IsNullOrWhiteSpace(title) || parsingResult == null ||
              parsingResult.Source == null || parsingResult.Source.GameDescription == null || string.IsNullOrEmpty(parsingResult.Source.TableName))
            {
                return false;
            }

            return title.Contains(parsingResult.Source.TableName.Replace(",", string.Empty));
        }

        private Dictionary<int, Dictionary<int, int>> seatMap = new Dictionary<int, Dictionary<int, int>>
        {
            { 2, new Dictionary<int, int>
                 {
                    { 3, 1 },
                    { 8, 2 }
                 }
            },
            { 3, new Dictionary<int, int>
                 {
                    { 3, 1 },
                    { 6, 2 },
                    { 10, 3 }
                 }
            },
            { 4, new Dictionary<int, int>
                {
                    {2, 1},
                    {4, 2},
                    {7, 3},
                    {9, 4}
                }
            },
            { 6, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 3, 2 },
                    { 5, 3 },
                    { 6, 4 },
                    { 8, 5 },
                    { 10, 6 }
                 }
            },
            { 8, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 2, 2 },
                    { 4, 3 },
                    { 5, 4 },
                    { 6, 5 },
                    { 7, 6 },
                    { 9, 7 },
                    { 10, 8 }
                 }
            },
            { 9, new Dictionary<int, int>
                 {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 },
                    { 5, 5 },
                    { 6, 6 },
                    { 8, 7 },
                    { 9, 8 },
                    { 10, 9 }
                 }
            }
        };

        protected override PlayerList GetPlayerList(HandHistory handHistory)
        {
            var playerList = handHistory.Players;

            var maxPlayers = handHistory.GameDescription.SeatType.MaxPlayers;

            NormalizeSeats(playerList, maxPlayers);

            var heroSeat = handHistory.Hero != null ? handHistory.Hero.SeatNumber : 0;

            if (heroSeat != 0)
            {
                ApplyPreferredSeating(playerList, maxPlayers, heroSeat);
            }

            return playerList;
        }

        protected override string GetHandTextFromStream(Stream fs, Encoding encoding, string fileName)
        {
            IPokerFileInfo importedFileInfo = null;

            var fileWasImportedOnce = importedHandsHistory.ContainsKey(fileName);

            var fileInfo = new FileInfo(fileName);

            if (fileWasImportedOnce)
            {
                importedFileInfo = importedHandsHistory[fileName];

                // file hasn't been changed so we don't read it
                if (importedFileInfo.LastWriteTime == fileInfo.LastWriteTime
                    && importedFileInfo.Length == fs.Length)
                {
                    return null;
                }
            }

            var data = new byte[fs.Length];

            if (data.Length == 0)
            {
                return string.Empty;
            }

            fs.Position = 0;
            fs.Read(data, 0, data.Length);

            if (importedFileInfo == null)
            {
                importedFileInfo = new IPokerFileInfo();
            }

            importedFileInfo.LastWriteTime = fileInfo.LastWriteTime;
            importedFileInfo.Length = fs.Length;

            var handText = encoding.GetString(data);

            if (!fileWasImportedOnce)
            {
                importedHandsHistory.Add(fileName, importedFileInfo);
                return handText;
            }

            var xmlHandDocument = XDocument.Parse(handText, LoadOptions.PreserveWhitespace);
            var gameNodes = xmlHandDocument.Document.Descendants("game").ToArray();

            foreach (var gameNode in gameNodes)
            {
                var gamecodeAttribute = gameNode.Attribute("gamecode");

                // skip game nodes without gamecode attribute
                if (gamecodeAttribute == null)
                {
                    gameNode.Remove();
                    continue;
                }

                long gamecode = 0;

                // skip game nodes if gamecode can't be parsed
                if (!long.TryParse(gamecodeAttribute.Value, out gamecode))
                {
                    gameNode.Remove();
                    continue;
                }

                if (importedFileInfo != null && importedFileInfo.ImportedHands.Contains(gamecode))
                {
                    gameNode.Remove();
                }
            }

            handText = xmlHandDocument.ToString(SaveOptions.None);

            return handText;
        }

        private Dictionary<string, IPokerFileInfo> importedHandsHistory = new Dictionary<string, IPokerFileInfo>();

        protected override IEnumerable<ParsingResult> ImportHand(string handHistory, GameInfo gameInfo, IFileImporter dbImporter, DHProgress progress)
        {
            var parsingResults = dbImporter.Import(handHistory, progress, gameInfo);

            IPokerFileInfo importedFileInfo = null;

            if (importedHandsHistory.ContainsKey(gameInfo.FullFileName))
            {
                importedFileInfo = importedHandsHistory[gameInfo.FullFileName];
            }
            else
            {
                importedFileInfo = new IPokerFileInfo();
                importedHandsHistory.Add(gameInfo.FullFileName, importedFileInfo);
            }

            foreach (var parsingResult in parsingResults)
            {
                if (parsingResult != null && parsingResult.HandHistory != null &&
                    !importedFileInfo.ImportedHands.Contains(parsingResult.HandHistory.Gamenumber))
                {
                    importedFileInfo.ImportedHands.Add(parsingResult.HandHistory.Gamenumber);
                }
            }

            return parsingResults;
        }

        protected override void Clean()
        {
            base.Clean();
            importedHandsHistory.Clear();
        }

        protected override EnumTableType ParseTableType(ParsingResult parsingResult, GameInfo gameInfo)
        {
            var tableType = base.ParseTableType(parsingResult, gameInfo);

            if (gameInfo.TableType > tableType)
            {
                return gameInfo.TableType;
            }

            return tableType;
        }

        private void NormalizeSeats(PlayerList playerList, int maxPlayers)
        {
            // normalize seats
            if (!seatMap.ContainsKey(maxPlayers))
            {
                return;
            }

            var seats = seatMap[maxPlayers];

            foreach (var player in playerList)
            {
                if (seats.ContainsKey(player.SeatNumber))
                {
                    player.SeatNumber = seats[player.SeatNumber];
                }
            }
        }

        private Dictionary<int, int> autoCenterSeats = new Dictionary<int, int>
        {
            { 2, 2 },
            { 3, 2 },
            { 4, 2 },
            { 6, 3 },
            { 9, 5 },
            { 10, 5 }
        };

        private void ApplyPreferredSeating(PlayerList playerList, int maxPlayers, int heroSeat)
        {
            var isAutoCenter = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                       SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.IPoker)?.IsAutoCenter;

            if (!isAutoCenter.HasValue || !isAutoCenter.Value || !autoCenterSeats.ContainsKey(maxPlayers))
            {
                return;
            }

            var prefferedSeat = autoCenterSeats[maxPlayers];

            var shift = (prefferedSeat - heroSeat) % maxPlayers;

            foreach (var player in playerList)
            {
                player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, maxPlayers);
            }
        }

        /// <summary>
        /// Represents information about imported file
        /// </summary>
        private class IPokerFileInfo
        {
            public IPokerFileInfo()
            {
                ImportedHands = new HashSet<long>();
                LastWriteTime = DateTime.MinValue;
            }

            public DateTime LastWriteTime { get; set; }

            public long Length { get; set; }

            public HashSet<long> ImportedHands { get; set; }
        }
    }
}