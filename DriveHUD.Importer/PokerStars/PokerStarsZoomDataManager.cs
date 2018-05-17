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
        private readonly IEventAggregator eventAggregator;
        private IImporterSessionCacheService importerSessionCacheService;

        private readonly Dictionary<int, PokerStarsZoomCacheData> cachedData = new Dictionary<int, PokerStarsZoomCacheData>();

        public PokerStarsZoomDataManager(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        public void Initialize(PokerClientDataManagerInfo dataManagerInfo)
        {
            Check.ArgumentNotNull(() => dataManagerInfo);
            importerSessionCacheService = ServiceLocator.Current.GetInstance<IImporterSessionCacheService>();
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
                    GameType = BovadaConverters.ConvertGameTypeFromTitle(catcherDataObject.Title),
                    TableType = (EnumTableType)catcherDataObject.Size
                };

                // Initialize cache
                gameInfo.ResetPlayersCacheInfo();

                var players = new PlayerList(catcherDataObject.Players.Select(x =>
                    new Player(x.Player, 0, x.Seat)
                    {
                        PlayerId = playerNamePlayerIdMap.ContainsKey(x.Player) ? playerNamePlayerIdMap[x.Player] : 0
                    }));

                var gameFormat = ParseGameFormatFromTitle(catcherDataObject.Title);

                if (gameFormat.HasValue)
                {
                    gameInfo.GameFormat = gameFormat.Value;
                }

                var heroPlayer = ProcessPlayers(gameInfo, players, catcherDataObject);

                if (heroPlayer == null || (gameInfo.GameFormat != GameFormat.Zoom && gameFormat != GameFormat.Cash))
                {
                    return;
                }

                var importedArgs = new DataImportedEventArgs(players, gameInfo, heroPlayer, 0);
                eventAggregator.GetEvent<DataImportedEvent>().Publish(importedArgs);

                cachedObject.IsProcessed = true;
#if DEBUG
                Console.WriteLine($@"Data has been send to {catcherDataObject.Title}, {catcherDataObject.Handle}, {catcherDataObject.TableName} {catcherDataObject.Size}-max, {string.Join(", ", players.Select(x => $"{x.PlayerName}[{x.PlayerId}]").ToArray())}");
#endif
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

                const string key = "5u8x/A?D(G+KbPeShVmYq3t6w9y$B&E)";

                using (var aesCryptoProvider = new AesManaged())
                {
                    aesCryptoProvider.Mode = CipherMode.ECB;
                    aesCryptoProvider.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
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
            catch (Exception)
            {
                LogProvider.Log.Error("Could not recognize data");
                throw;
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

        private readonly Dictionary<string, int> playerNamePlayerIdMap = new Dictionary<string, int>();

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

            var heroName = ParseHeroNameFromTitle(catcherDataObject.Title);

            foreach (var player in players)
            {
                if (player.PlayerId == 0)
                {
                    continue;
                }

                var isHero = false;

                if (player.PlayerName.Equals(heroName))
                {
                    heroPlayer = player;
                    isHero = true;
                }

                var playerCollectionItem = new PlayerCollectionItem
                {
                    PlayerId = player.PlayerId,
                    Name = player.PlayerName,
                    PokerSite = EnumPokerSites.PokerStars
                };

                var playerCacheStatistic = importerSessionCacheService.GetPlayerStats(gameInfo.Session, playerCollectionItem, out bool exists);

                if (exists && playerCacheStatistic.IsHero)
                {
                    heroPlayer = player;
                    gameInfo.GameFormat = playerCacheStatistic.GameFormat;
                    break;
                }
                else if (!exists && gameInfo.GameFormat == GameFormat.Zoom)
                {
                    var playerCacheInfo = new PlayerStatsSessionCacheInfo
                    {
                        Session = gameInfo.Session,
                        GameFormat = gameInfo.GameFormat,
                        Player = playerCollectionItem,
                        IsHero = isHero,
                        Stats = new Playerstatistic
                        {
                            SessionCode = gameInfo.Session,
                            PokergametypeId = ParsePokergametypeIdFromTitle(catcherDataObject.Title)
                        }
                    };

                    if (playerCacheInfo.Stats.PokergametypeId != 0)
                    {
                        gameInfo.AddToPlayersCacheInfo(playerCacheInfo);
                    }
                }
            }

            if (heroPlayer == null)
            {
                return null;
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
                }
            }
            else
            {
                var preferredSeats = ServiceLocator.Current.GetInstance<ISettingsService>().GetSettings().
                                       SiteSettings.SitesModelList.FirstOrDefault(x => x.PokerSite == EnumPokerSites.PokerStars)?.PrefferedSeats;

                var prefferedSeat = preferredSeats?.FirstOrDefault(x => (int)x.TableType == catcherDataObject.Size && x.IsPreferredSeatEnabled);

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

        private static string ParseHeroNameFromTitle(string title)
        {
            // Halley - $0.01/$0.02 USD - No Limit Hold'em - Logged In as Peon347
            const string loggedInText = "Logged In as";

            var heroNameStartIndex = title.IndexOf(loggedInText, StringComparison.OrdinalIgnoreCase) + loggedInText.Length + 1;

            if (heroNameStartIndex < loggedInText.Length + 1)
            {
                return null;
            }

            var heroName = title.Substring(heroNameStartIndex);

            return heroName;
        }

        private static GameFormat? ParseGameFormatFromTitle(string title)
        {
            var tableEndStartIndex = title.IndexOf("#", StringComparison.OrdinalIgnoreCase) - 1;

            if (tableEndStartIndex <= 0)
            {
                tableEndStartIndex = title.IndexOf("-", StringComparison.OrdinalIgnoreCase) - 1;

                if (tableEndStartIndex <= 0)
                {
                    return null;
                }
            }

            var tableName = title.Substring(0, tableEndStartIndex);

            if (ZoomTables.Contains(tableName))
            {
                return GameFormat.Zoom;
            }
            else if (!title.Contains("Tournament"))
            {
                return GameFormat.Cash;
            }

            return null;
        }

        private static short ParsePokergametypeIdFromTitle(string title)
        {
            HandHistories.Objects.GameDescription.GameType gameType;

            if (title.ContainsIgnoreCase("No Limit Hold'em"))
            {
                gameType = HandHistories.Objects.GameDescription.GameType.NoLimitHoldem;
            }
            else if (title.ContainsIgnoreCase("No Limit Omaha Hi/Lo"))
            {
                gameType = HandHistories.Objects.GameDescription.GameType.NoLimitOmahaHiLo;
            }
            else if (title.ContainsIgnoreCase("Pot Limit Omaha"))
            {
                gameType = HandHistories.Objects.GameDescription.GameType.PotLimitOmaha;
            }
            else if (title.ContainsIgnoreCase("Pot Limit Omaha Hi/Lo"))
            {
                gameType = HandHistories.Objects.GameDescription.GameType.PotLimitOmahaHiLo;
            }
            else if (title.ContainsIgnoreCase("Limit Hold'em"))
            {
                gameType = HandHistories.Objects.GameDescription.GameType.FixedLimitHoldem;
            }
            else
            {
                return 0;
            }

            return (short)gameType;
        }

        private static readonly string[] ZoomTables = new[] { "McNaught", "Borrelllly", "Halley", "Lovejoy", "Hyakutake", "Donati", "Lynx", "Hartley", "Aludra", "Devanssay",
            "Eulalia", "Nansen", "Amundsen", "Whirlpool", "Hydra", "Thyestes", "Arp", "Baade", "Aquarius Dwarf", "Serpens Caput", "Triangulum", "Gotha", "Aenaa", "Diotima",
            "Lambda Velorum", "Humason", "Centaurus", "Dorado", "Lupus", "Coma Berenices", "Cassiopeia", "Perseus", "C Carinae", "Alpha Reticuli (CAP)", "Chi Sagittarii",
            "Sirius", "Omicron Capricorni", "Beta Tucanae (CAP)", "Delta Antilae", "Theta Cancri", "Chi Draconis", "Sigam Aquilae (CAP)", "Iota Apodis", "Zeta Phoenicis",
            "Delta Boötis", "Gamma Delphini (CAP)", "Phi Piscium", "Tau Hydrae", "Adhara", "Iota Cancri (CAP)", "Deneb el Okab", "Lambda Arietis", "Cetus", "Mira (CAP)",
            "Crux", "Rho Capricorni", "Gamma Crateris", "Alpha Crucis (CAP)", "Norma", "Canes Venatici", "Draco", "Amália", "Eusébio", "Pessoa", "Cervantes", "Velazquez",
            "Gaudi", "Dali", "Goya", "Picasso", "Clubs", "Spades", "Hears", "Diamonds", "Turn", "River", "Portland", "Los Angeles", "Houston", "New York", "Las Vegas",
            "Boston", "Boulder", "Washington", "Dallas", "New Orleans", "Miami", "Antares", "Atena", "Fenice", "Pegaso", "Cigno", "Shun", "Sirio", "Cronos" };
    }
}