using System;
using DriveHUD.HUD.Service;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Application.ViewModels;
using System.Linq;
using Model;
using DriveHUD.Application.ViewModels.Replayer;
using Model.Interfaces;
using DriveHUD.Entities;

namespace DriveHUD.Application.HudServices
{
    internal class HudNamedPipeBindingCallbackService : IHudNamedPipeBindingCallbackService
    {
        public void SaveHudLayout(HudLayoutContract hudLayout)
        {
            //if (hudLayout == null)
            //{
            //    LogProvider.Log.Warn(this, "hudLayout is null");
            //    return;
            //}

            //var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            //var activeLayout = hudLayoutsService.GetActiveLayout(hudLayout.LayoutId);

            //if (activeLayout == null)
            //{
            //    LogProvider.Log.Info(this, $"Cannot find layout with id: {hudLayout.LayoutId}");
            //    return;
            //}

            //if (!hudLayoutsService.HudTableViewModels.ContainsKey(hudLayout.LayoutId))
            //{
            //    LogProvider.Log.Info(this, $"Cannot find view model for layout with id: {hudLayout.LayoutId}");
            //    return;
            //}

            //var viewModel = hudLayoutsService.HudTableViewModelDictionary[hudLayout.LayoutId];

            //// update positions 
            //foreach (var hudPosition in hudLayout.HudPositions)
            //{
            //    var hudToUpdate = activeLayout?.HudPositions?.FirstOrDefault(x => x.Seat == hudPosition.SeatNumber && (int)x.HudType == hudPosition.HudType);
            //    if (hudToUpdate == null)
            //    {
            //        continue;
            //    }

            //    hudToUpdate.Position = hudPosition.Position;

            //    if (viewModel == null)
            //    {
            //        continue;
            //    }

            //    var hudElementToUpdate = viewModel.HudElements.FirstOrDefault(x => x.Seat == hudToUpdate.Seat && x.HudType == hudToUpdate.HudType);
            //    if (hudElementToUpdate != null)
            //    {
            //        hudElementToUpdate.Position = hudToUpdate.Position;
            //    }
            //}

            //var hudData = new HudSavedDataInfo
            //{
            //    LayoutId = activeLayout.LayoutId,
            //    Name = activeLayout.Name,
            //    HudTable = viewModel,
            //    Stats = activeLayout.HudStats
            //};

            //App.Current.Dispatcher.Invoke(() => hudLayoutsService.SaveAs(hudData));
        }

        public void ReplayHand(long gameNumber, short pokerSiteId)
        {
            var currentlySelectedPlayer = ServiceLocator.Current.GetInstance<SingletonStorageModel>().PlayerSelectedItem;
            var playerName = pokerSiteId == (short)currentlySelectedPlayer.PokerSite
                ? currentlySelectedPlayer.Name
                : string.Empty;

            ServiceLocator.Current.GetInstance<IReplayerService>().ReplayHand(playerName, gameNumber, pokerSiteId, showHoleCards: true);
        }

        public void LoadLayout(int layoutId, string layoutName)
        {
            LogProvider.Log.Info($"Load layout {layoutId} {layoutName}");

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var hudToLoad = hudLayoutsService.GetLayout(layoutName);

            if (hudToLoad == null)
            {
                LogProvider.Log.Info(this, $"Cannot find layout with id {layoutId} and name {layoutName}");
                return;
            }

            //if (hudToLoad.IsDefault)
            //{
            //    LogProvider.Log.Info(this, $"Layout {layoutName} ({layoutId}) is already selected as default.");
            //    return;
            //}

            //App.Current.Dispatcher.Invoke(() => hudLayoutsService.SetLayoutActive(hudToLoad));
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
