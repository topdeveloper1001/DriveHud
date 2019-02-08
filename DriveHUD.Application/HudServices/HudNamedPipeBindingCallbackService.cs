//-----------------------------------------------------------------------
// <copyright file="HudNamedPipeBindingCallbackService.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Services;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Common.Log;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using DriveHUD.ViewModels.Replayer;
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
        private readonly Lazy<IHandNoteCacheService> handNoteCacheService = new Lazy<IHandNoteCacheService>(() => ServiceLocator.Current.GetInstance<IHandNoteCacheService>());

        private IHandNoteCacheService HandNoteCacheService => handNoteCacheService.Value;

        public void SaveHudLayout(HudLayoutContract hudLayoutContract)
        {
            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            hudLayoutsService.Save(hudLayoutContract);
        }

        public void ReplayHand(long gameNumber, short pokerSiteId)
        {
            try
            {
                var playerName = string.Empty;

                var currentlySelectedPlayer = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;

                if (currentlySelectedPlayer is AliasCollectionItem)
                {
                    var playerCollectionItem = (currentlySelectedPlayer as AliasCollectionItem).PlayersInAlias
                        .FirstOrDefault(x => x.PokerSite.HasValue && (short)x.PokerSite.Value == pokerSiteId);

                    if (playerCollectionItem != null)
                    {
                        playerName = playerCollectionItem.Name;
                    }
                }
                else if (currentlySelectedPlayer != null && currentlySelectedPlayer.PokerSite.HasValue && (short)currentlySelectedPlayer.PokerSite.Value == pokerSiteId)
                {
                    playerName = currentlySelectedPlayer.Name;
                }

                ServiceLocator.Current.GetInstance<IReplayerService>().ReplayHand(playerName, gameNumber, pokerSiteId, showHoleCards: true);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not replay hand #{gameNumber} for {(EnumPokerSites)pokerSiteId}", e);
            }
        }

        public void LoadLayout(string layoutName, EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            try
            {
                LogProvider.Log.Info($"Set layout {layoutName} as active for {pokerSite} {gameType} {tableType}");

                var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

                var hudToLoad = hudLayoutsService.GetLayout(layoutName);

                if (hudToLoad == null)
                {
                    LogProvider.Log.Info(this, $"Could not find layout with name {layoutName} for {pokerSite} {gameType} {tableType}");
                    return;
                }

                hudLayoutsService.SetActiveLayout(hudToLoad, pokerSite, gameType, tableType);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not load layout #{layoutName} for {pokerSite}, {gameType}, {tableType}", e);
            }
        }

        public void TagHands(IEnumerable<long> gameNumbers, short pokerSiteId, int tag)
        {
            foreach (var gameNumber in gameNumbers)
            {
                TagHand(gameNumber, pokerSiteId, tag);
            }
        }

        public void TreatTableAs(IntPtr handle, EnumTableType tableType)
        {
            var treatAsService = ServiceLocator.Current.GetInstance<ITreatAsService>();
            treatAsService.AddOrUpdate(handle, tableType);
        }

        private void TagHand(long gameNumber, short pokerSiteId, int tag)
        {
            try
            {
                var dataService = ServiceLocator.Current.GetInstance<IDataService>();

                var handNote = dataService.GetHandNote(gameNumber, pokerSiteId);

                if (handNote == null)
                {
                    handNote = new Handnotes
                    {
                        Gamenumber = gameNumber,
                        PokersiteId = pokerSiteId
                    };
                }

                handNote.HandTag = tag;

                dataService.Store(handNote);

                var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();

                var statistic = storageModel.StatisticCollection.FirstOrDefault(x => x.GameNumber == gameNumber && x.PokersiteId == pokerSiteId);

                if (statistic == null)
                {
                    HandNoteCacheService.AddHandNote(handNote);
                    return;
                }

                statistic.HandNote = handNote;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not tag hand #{gameNumber} for {(EnumPokerSites)pokerSiteId}", e);
            }
        }
    }
}