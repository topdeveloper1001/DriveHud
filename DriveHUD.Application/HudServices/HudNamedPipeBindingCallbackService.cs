//-----------------------------------------------------------------------
// <copyright file="HudNamedPipeBindingCallbackService.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.HudServices
{
    internal class HudNamedPipeBindingCallbackService : IHudNamedPipeBindingCallbackService
    {
        public void SaveHudLayout(HudLayoutContract hudLayoutContract)
        {
            if (hudLayoutContract == null)
            {
                LogProvider.Log.Warn(this, "hudLayoutContract is null");
                return;
            }

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var existingHudLayout = hudLayoutsService.GetLayout(hudLayoutContract.LayoutName);

            if (existingHudLayout == null)
            {
                LogProvider.Log.Info(this, $"Could not find layout {hudLayoutContract.LayoutName}");
                return;
            }

            var existingHudPositions = existingHudLayout.HudPositionsInfo.FirstOrDefault(p => p.PokerSite == hudLayoutContract.PokerSite && p.GameType == hudLayoutContract.GameType);

            if (existingHudPositions == null)
            {
                existingHudPositions = new HudPositionsInfo
                {
                    GameType = hudLayoutContract.GameType,
                    PokerSite = hudLayoutContract.PokerSite,
                    HudPositions = new List<HudPositionInfo>()
                };
            }

            // update positions 
            foreach (var hudPosition in hudLayoutContract.HudPositions)
            {
                var hudPositionForUpdate = existingHudPositions.HudPositions.FirstOrDefault(x => x.Seat == hudPosition.SeatNumber);

                if (hudPositionForUpdate == null)
                {
                    hudPositionForUpdate = new HudPositionInfo
                    {
                        Seat = hudPosition.SeatNumber
                    };

                    existingHudPositions.HudPositions.Add(hudPositionForUpdate);
                }

                hudPositionForUpdate.Position = hudPosition.Position;
            }

            hudLayoutsService.Save(existingHudLayout);
        }

        public void ReplayHand(long gameNumber, short pokerSiteId)
        {
            var currentlySelectedPlayer = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var playerName = pokerSiteId == (short)currentlySelectedPlayer.PokerSite
                ? currentlySelectedPlayer.Name
                : string.Empty;

            ServiceLocator.Current.GetInstance<IReplayerService>().ReplayHand(playerName, gameNumber, pokerSiteId, showHoleCards: true);
        }

        public void LoadLayout(string layoutName, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            LogProvider.Log.Info($"Set layout {layoutName} as active for {pokerSite} {gameType} {tableType}");

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var hudToLoad = hudLayoutsService.GetLayout(layoutName);

            if (hudToLoad == null)
            {
                LogProvider.Log.Info(this, $"Could not find layout with name {layoutName} for {pokerSite} {gameType} {tableType}");
                return;
            }

            hudLayoutsService.SetLayoutActive(hudToLoad, pokerSite, gameType, tableType);
        }

        public void TagHand(long gameNumber, short pokerSiteId, int tag)
        {
            try
            {
                var dataService = ServiceLocator.Current.GetInstance<IDataService>();
                var handNote = dataService.GetHandNote(gameNumber, pokerSiteId);
                if (handNote == null)
                {
                    handNote = new Handnotes()
                    {
                        Gamenumber = gameNumber,
                        PokersiteId = pokerSiteId
                    };
                }
                handNote.HandTag = tag;
                dataService.Store(handNote);

                var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();
                var statistic = storageModel.StatisticCollection.FirstOrDefault(x => x.GameNumber == gameNumber && x.PokersiteId == pokerSiteId);
                if (statistic != null)
                {
                    statistic.HandNote = handNote;
                }
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, ex);
            }
        }
    }
}
