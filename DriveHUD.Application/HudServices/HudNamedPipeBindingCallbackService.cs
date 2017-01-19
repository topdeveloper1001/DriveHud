using System;
using DriveHUD.HUD.Service;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Application.ViewModels;
using System.Linq;
using DriveHUD.Application.ViewModels.Layouts;
using Model;
using DriveHUD.Application.ViewModels.Replayer;
using Model.Interfaces;
using DriveHUD.Entities;
using Model.Enums;

namespace DriveHUD.Application.HudServices
{
    internal class HudNamedPipeBindingCallbackService : IHudNamedPipeBindingCallbackService
    {
        public void SaveHudLayout(HudLayoutContract hudLayout, short pokerSiteId, short gameType, short tableType)
        {
            if (hudLayout == null)
            {
                LogProvider.Log.Warn(this, "hudLayout is null");
                return;
            }

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            var activeLayout = hudLayoutsService.GetLayout(hudLayout.LayoutName);

            if (activeLayout == null)
            {
                LogProvider.Log.Info(this, $"Cannot find layout with id: {hudLayout.LayoutName}");
                return;
            }
            var viewModel = hudLayoutsService.GetHudTableViewModel((EnumPokerSites) pokerSiteId,
                (EnumTableType) tableType, (EnumGameType) gameType);
            if (viewModel == null)
            {
                LogProvider.Log.Info(this, $"Cannot find view model for layout with id: {hudLayout.LayoutName}");
                return;
            }

            // update positions 
            foreach (var hudPosition in hudLayout.HudPositions)
            {
                var hudPositions =
                    activeLayout.HudPositionsInfo.FirstOrDefault(
                        p =>
                            p.PokerSite == (EnumPokerSites) pokerSiteId &&
                            p.GameType == (EnumGameType) gameType);
                var hudToUpdate = hudPositions?.HudPositions.FirstOrDefault(x => x.Seat == hudPosition.SeatNumber);
                if (hudToUpdate == null)
                {
                    continue;
                }

                hudToUpdate.Position = hudPosition.Position;

                if (viewModel == null)
                {
                    continue;
                }

                var hudElementToUpdate = viewModel.HudElements.FirstOrDefault(x => x.Seat == hudToUpdate.Seat);
                if (hudElementToUpdate != null)
                {
                    hudElementToUpdate.Position = hudToUpdate.Position;
                }
            }

            var hudData = new HudSavedDataInfo
            {
                LayoutInfo = activeLayout,
                Name = activeLayout.Name,
                HudTable = viewModel,
                Stats = activeLayout?.HudStats
            };

            App.Current.Dispatcher.Invoke(() => hudLayoutsService.SaveAs(hudData));
        }

        public void ReplayHand(long gameNumber, short pokerSiteId)
        {
            var currentlySelectedPlayer = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var playerName = pokerSiteId == (short)currentlySelectedPlayer.PokerSite
                ? currentlySelectedPlayer.Name
                : string.Empty;

            ServiceLocator.Current.GetInstance<IReplayerService>().ReplayHand(playerName, gameNumber, pokerSiteId, showHoleCards: true);
        }

        public void LoadLayout(string layoutName, short pokerSiteId, short gameType, short tableType)
        {
            LogProvider.Log.Info($"Load layout {layoutName}");

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var hudToLoad = hudLayoutsService.GetLayout(layoutName);

            if (hudToLoad == null)
            {
                LogProvider.Log.Info(this, $"Cannot find layout with name {layoutName}");
                return;
            }

            if (hudToLoad.IsDefault)
            {
                LogProvider.Log.Info(this, $"Layout {layoutName} is already selected as default.");
                return;
            }

            App.Current.Dispatcher.Invoke(() => hudLayoutsService.SetLayoutActive(hudToLoad, pokerSiteId, gameType, tableType));
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
