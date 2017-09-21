//-----------------------------------------------------------------------
// <copyright file="PokerStarsZoomDataManager.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Common.WinApi;
using DriveHUD.Entities;
using DriveHUD.Importers.Bovada;
using DriveHUD.Importers.Helpers;
using HandHistories.Objects.Players;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using Newtonsoft.Json;
using NHibernate.Linq;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DriveHUD.Importers.PokerStars
{
    internal class PokerStarsZoomDataManager : IPokerStarsZoomDataManager
    {
        private IEventAggregator eventAggregator;
        private IImporterSessionCacheService importerSessionCacheService;
        private IPokerClientEncryptedLogger logger;
        private bool isLoggingEnabled;
        private string site;

        /// <summary>
        /// Interval in minutes after which session will expire and will be removed from cache
        /// </summary>
        private const int cacheLifeTime = 3;

        private Dictionary<int, PokerStarsZoomCacheData> cachedData = new Dictionary<int, PokerStarsZoomCacheData>();

        public PokerStarsZoomDataManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);

            logger = dataManagerInfo.Logger;
            site = dataManagerInfo.Site;

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settings = settingsService.GetSettings();

            importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();

            isLoggingEnabled = settings.GeneralSettings.IsAdvancedLoggingEnabled;
        }

        public void ProcessData(byte[] data)
        {
            try
            {
                var dataText = Decrypt(data);

                var catcherDataObject = JsonConvert.DeserializeObject<PokerStarsZoomDataObject>(dataText);

                if (catcherDataObject == null)
                {
                    return;
                }

                PokerStarsZoomCacheData cachedObject;

                var isNew = false;

                if (!cachedData.ContainsKey(catcherDataObject.Handle))
                {
                    cachedObject = new PokerStarsZoomCacheData
                    {
                        Data = catcherDataObject
                    };

                    cachedData.Add(catcherDataObject.Handle, cachedObject);

                    isNew = true;
                }
                else
                {
                    cachedObject = cachedData[catcherDataObject.Handle];
                }

                cachedObject.LastModified = DateTime.Now;

                RemoveExpiredCache();

                if (!isNew)
                {
                    if (cachedObject.IsProcessed && ComparePlayers(cachedObject.Data, catcherDataObject))
                    {
                        return;
                    }

                    cachedObject.Data = catcherDataObject;
                }

                LoadPlayers(catcherDataObject.Players.Select(x => x.Player));

                var gameInfo = new GameInfo
                {
                    Session = catcherDataObject.Handle.ToString(),
                    WindowHandle = catcherDataObject.Handle,
                    PokerSite = EnumPokerSites.PokerStars,
                    GameFormat = GameFormat.Zoom,
                    GameType = BovadaConverters.ConvertGameTypeFromTitle(catcherDataObject.Title),
                    TableType = (EnumTableType)catcherDataObject.Size,
                    AddedPlayers = new PlayerCollectionItem[0]
                };

                var players = new PlayerList(catcherDataObject.Players.Select(x =>
                    new Player(x.Player, 0, x.Seat)
                    {
                        PlayerId = playerNamePlayerIdMap.ContainsKey(x.Player) ? playerNamePlayerIdMap[x.Player] : 0
                    }));

                var heroPlayer = ProcessPlayers(gameInfo, players, catcherDataObject);

                if (heroPlayer == null)
                {
                    return;
                }

                var importedArgs = new DataImportedEventArgs(players, gameInfo, heroPlayer);
                eventAggregator.GetEvent<DataImportedEvent>().Publish(importedArgs);

                cachedObject.IsProcessed = true;

                Console.WriteLine($"Data has been send to {catcherDataObject.Title}, {catcherDataObject.Handle}, {catcherDataObject.Size}-max, {string.Join(", ", players.Select(x => $"{x.PlayerName}[{x.PlayerId}]").ToArray())}");
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Stream data was in wrong format", ex);
            }
        }

        protected string Decrypt(byte[] encryptedData)
        {
            try
            {
                encryptedData = Utils.RemoveTrailingZeros(encryptedData);

                var key = "5u8x/A?D(G+KbPeShVmYq3t6w9y$B&E)";

                using (var aesCryptoProvider = new AesManaged())
                {
                    aesCryptoProvider.Mode = CipherMode.ECB;
                    aesCryptoProvider.IV = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    aesCryptoProvider.Key = Encoding.UTF8.GetBytes(key);
                    aesCryptoProvider.Padding = PaddingMode.None;

                    var decryptor = aesCryptoProvider.CreateDecryptor();

                    using (var msDecrypt = new MemoryStream(encryptedData))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var swDecrypt = new StreamReader(csDecrypt, Encoding.UTF8))
                            {
                                var decryptedXml = swDecrypt.ReadToEnd().Replace("\0", string.Empty).Trim().RemoveControlChars();
                                return decryptedXml;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("Could not recognize data");
                throw e;
            }
        }

        public void Dispose()
        {

        }

        private static bool ComparePlayers(PokerStarsZoomDataObject data1, PokerStarsZoomDataObject data2)
        {
            if (ReferenceEquals(data1, data2))
            {
                return true;
            }

            if (data1.Players.Length != data2.Players.Length)
            {
                return false;
            }

            var players1 = data1.Players.OrderBy(x => x.Seat).ToArray();
            var players2 = data2.Players.OrderBy(x => x.Seat).ToArray();

            for (var i = 0; i < players1.Length; i++)
            {
                if (!players1[i].Player.Equals(players2[i].Player, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }

        private void RemoveExpiredCache()
        {
            foreach (var cacheKey in cachedData.Keys.ToArray())
            {
                var windowHandle = new IntPtr(cacheKey);

                if (!WinApi.IsWindow(windowHandle))
                {
                    cachedData.Remove(cacheKey);
                }
            }
        }

        private Dictionary<string, int> playerNamePlayerIdMap = new Dictionary<string, int>();

        private void LoadPlayers(IEnumerable<string> players)
        {
            if (players == null)
            {
                return;
            }

            var playersToAdd = players.Where(x => !playerNamePlayerIdMap.ContainsKey(x)).ToArray();

            if (playersToAdd.Length > 0)
            {
                using (var session = ModelEntities.OpenSession())
                {
                    var playerNamePlayerIdToAdd = session.Query<Players>()
                        .Where(x => x.PokersiteId == (short)EnumPokerSites.PokerStars && playersToAdd.Contains(x.Playername))
                        .Select(x => new { x.Playername, x.PlayerId })
                        .ToArray();

                    playerNamePlayerIdToAdd.ForEach(x => playerNamePlayerIdMap.Add(x.Playername, x.PlayerId));
                }
            }
        }

        private Player ProcessPlayers(GameInfo gameInfo, PlayerList players, PokerStarsZoomDataObject catcherDataObject)
        {
            Player heroPlayer = null;

            foreach (var player in players)
            {
                if (player.PlayerId == 0)
                {
                    continue;
                }

                var playerCollectionItem = new PlayerCollectionItem
                {
                    PlayerId = player.PlayerId,
                    Name = player.PlayerName,
                    PokerSite = EnumPokerSites.PokerStars
                };

                var playerCacheStatistic = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem);

                if (playerCacheStatistic.IsHero)
                {
                    heroPlayer = player;
                    gameInfo.GameFormat = playerCacheStatistic.GameFormat;
                    break;
                }
            }

            if (heroPlayer == null)
            {
                return heroPlayer;
            }

            int prefferedSeatNumber = 0;

            if (gameInfo.GameFormat == GameFormat.Zoom)
            {
                // zoom is always auto-centered
                switch (gameInfo.TableType)
                {
                    case EnumTableType.HU:
                        prefferedSeatNumber = 2;
                        break;
                    case EnumTableType.Six:
                        prefferedSeatNumber = 3;
                        break;
                    case EnumTableType.Nine:
                        prefferedSeatNumber = 5;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                       SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PokerStars)?.PrefferedSeats;

                var prefferedSeat = preferredSeats.FirstOrDefault(x => (int)x.TableType == catcherDataObject.Size && x.IsPreferredSeatEnabled);

                if (prefferedSeat != null)
                {
                    prefferedSeatNumber = prefferedSeat.PreferredSeat;
                }
            }

            if (prefferedSeatNumber > 0)
            {
                var shift = (prefferedSeatNumber - heroPlayer.SeatNumber) % catcherDataObject.Size;

                foreach (var player in players)
                {
                    player.SeatNumber = GeneralHelpers.ShiftPlayerSeat(player.SeatNumber, shift, catcherDataObject.Size);
                }
            }

            return heroPlayer;
        }

        private struct PokerStarsPlayer
        {
            public int PlayerId { get; set; }

            public string PlayerName { get; set; }
        }
    }
}