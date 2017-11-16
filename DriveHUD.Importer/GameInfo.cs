//-----------------------------------------------------------------------
// <copyright file="GameInfo.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Helpers;
using HandHistories.Parser.Parsers;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Importers
{
    /// <summary>
    /// Game information class
    /// </summary>
    public class GameInfo
    {
        private readonly object locker = new object();

        private List<PlayerStatsSessionCacheInfo> playersCacheInfo;

        public string Session { get; set; }

        public int WindowHandle { get; set; }

        public long GameNumber { get; set; }

        public EnumPokerSites PokerSite { get; set; }

        internal GameType GameType { get; set; }

        public GameFormat GameFormat { get; set; }

        public EnumGameType EnumGameType
        {
            get
            {
                return BovadaHelper.ConvertToEnumGameType(GameType, GameFormat);
            }
        }

        public EnumTableType TableType { get; set; }

        public HandHistories.Objects.GameDescription.TournamentSpeed? TournamentSpeed { get; set; }

        public Action<IEnumerable<ParsingResult>, GameInfo> UpdateAction { get; set; }

        public PlayerCollectionItem[] AddedPlayers { get; set; }
        
        public string FileName { get; set; }

        public string FullFileName { get; set; }

        public void ResetPlayersCacheInfo()
        {
            lock (locker)
            {
                playersCacheInfo = new List<PlayerStatsSessionCacheInfo>();
            }
        }

        public void AddToPlayersCacheInfo(PlayerStatsSessionCacheInfo cacheInfo)
        {
            lock (locker)
            {
                playersCacheInfo?.Add(cacheInfo);
            }
        }

        public List<PlayerStatsSessionCacheInfo> GetPlayersCacheInfo()
        {
            lock (locker)
            {
                return playersCacheInfo.ToList();
            }
        }
    }
}