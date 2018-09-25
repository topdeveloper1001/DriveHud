﻿//-----------------------------------------------------------------------
// <copyright file="TcpPacketImporter.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Extensions;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using HandHistories.Objects.Hand;
using Model;
using PacketDotNet;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DriveHUD.Importers.AndroidBase
{
    internal abstract class TcpPacketImporter : GenericImporter, ITcpPacketImporter
    {
        #region ITcpPacketImporter implementation

        public abstract void AddPacket(CapturedPacket capturedPacket);

        public abstract bool Match(TcpPacket tcpPacket, IpPacket ipPacket);

        #endregion

        protected const int StopTimeout = 5000;
        protected const int NoDataDelay = 100;
        protected const int DelayToProcessHands = 2000;

        protected abstract string Logger
        {
            get;
        }

#if DEBUG
        protected abstract string HandHistoryFilePrefix
        {
            get;
        }
#endif

        /// <summary>
        /// Exports captured hand history to the supported DB
        /// </summary>
        protected virtual void ExportHandHistory(HandHistoryData handHistoryData, ConcurrentDictionary<long, List<HandHistoryData>> handHistoriesToProcess)
        {
            LogProvider.Log.Info(Logger, $"Hand #{handHistoryData.HandHistory.HandId} has been sent [{handHistoryData.Uuid}].");

            var handId = handHistoryData.HandHistory.HandId;

            if (!handHistoriesToProcess.TryGetValue(handId, out List<HandHistoryData> handHistoriesData))
            {
                handHistoriesData = new List<HandHistoryData>();
                handHistoriesToProcess.AddOrUpdate(handId, handHistoriesData, (key, prev) => handHistoriesData);

                Task.Run(() =>
                {
                    try
                    {
                        Task.Delay(DelayToProcessHands).Wait(cancellationTokenSource.Token);
                        ExportHandHistory(handHistoriesData.ToList());
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    finally
                    {
                        handHistoriesToProcess.TryRemove(handId, out List<HandHistoryData> removedData);
                    }
                });
            }

            handHistoriesData.Add(handHistoryData);
        }

        /// <summary>
        /// Exports captured hand history to the supported DB
        /// </summary>
        protected virtual void ExportHandHistory(List<HandHistoryData> handHistories)
        {
            // merge hands
            var playerWithHoleCards = handHistories
                .SelectMany(x => x.HandHistory.Players)
                .Where(x => x.hasHoleCards)
                .DistinctBy(x => x.SeatNumber)
                .ToDictionary(x => x.SeatNumber, x => x.HoleCards);

            GameInfo mainGameInfo = null;

            foreach (var handHistoryData in handHistories)
            {
                handHistoryData.HandHistory.Players.ForEach(player =>
                {
                    if (!player.hasHoleCards && playerWithHoleCards.ContainsKey(player.SeatNumber))
                    {
                        player.HoleCards = playerWithHoleCards[player.SeatNumber];
                    }
                });

                var handHistoryText = SerializationHelper.SerializeObject(handHistoryData.HandHistory);

#if DEBUG                                
                if (!Directory.Exists("Hands"))
                {
                    Directory.CreateDirectory("Hands");
                }

                File.WriteAllText($"Hands\\{HandHistoryFilePrefix}_hand_exported_{handHistoryData.Uuid}_{handHistoryData.HandHistory.HandId}.xml", handHistoryText);
#endif
                var gameInfo = new GameInfo
                {
                    PokerSite = Site,
                    WindowHandle = handHistoryData.WindowHandle.ToInt32(),
                    GameNumber = handHistoryData.HandHistory.HandId,
                    Session = $"{handHistoryData.HandHistory.GameDescription.Identifier}{handHistoryData.Uuid}"
                };

                if (mainGameInfo == null)
                {
                    mainGameInfo = gameInfo;
                }
                else
                {
                    var playersCashInfo = mainGameInfo.GetPlayersCacheInfo();

                    if (playersCashInfo != null)
                    {
                        gameInfo.ResetPlayersCacheInfo();
                        gameInfo.DoNotReset = true;

                        foreach (var playerCashInfo in playersCashInfo)
                        {
                            var cacheInfo = new PlayerStatsSessionCacheInfo
                            {
                                Session = gameInfo.Session,
                                Player = new PlayerCollectionItem
                                {
                                    PlayerId = playerCashInfo.Player.PlayerId,
                                    Name = playerCashInfo.Player.Name,
                                    PokerSite = playerCashInfo.Player.PokerSite,
                                },
                                Stats = playerCashInfo.Stats.Copy(),
                                IsHero = handHistoryData.HandHistory.HeroName != null &&
                                    handHistoryData.HandHistory.HeroName.Equals(playerCashInfo.Player.Name),
                                GameFormat = playerCashInfo.GameFormat,
                                GameNumber = playerCashInfo.GameNumber
                            };

                            gameInfo.AddToPlayersCacheInfo(cacheInfo);
                        }
                    }
                }

                ProcessHand(handHistoryText, gameInfo);
            }
        }

        protected class HandHistoryData
        {
            public long Uuid { get; set; }

            public HandHistory HandHistory { get; set; }

            public IntPtr WindowHandle { get; set; }
        }
    }
}