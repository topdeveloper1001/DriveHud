using System;
using DriveHUD.HUD.Service;
using DriveHUD.Common.Log;
using Microsoft.Practices.ServiceLocation;
using DriveHUD.Application.ViewModels;
using System.Linq;

namespace DriveHUD.Application.HudServices
{
    internal class HudNamedPipeBindingCallbackService : IHudNamedPipeBindingCallbackService
    {
        public void SaveHudLayout(HudLayoutContract hudLayout)
        {
            if (hudLayout == null)
            {
                LogProvider.Log.Warn(this, "hudLayout is null");
                return;
            }

            var hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            var activeLayout = hudLayoutsService.GetActiveLayout(hudLayout.LayoutId);

            if (activeLayout == null)
            {
                LogProvider.Log.Info(this, $"Cannot find layout with id: {hudLayout.LayoutId}");
                return;
            }

            if (!hudLayoutsService.HudTableViewModelDictionary.ContainsKey(hudLayout.LayoutId))
            {
                LogProvider.Log.Info(this, $"Cannot find view model for layout with id: {hudLayout.LayoutId}");
                return;
            }

            var viewModel = hudLayoutsService.HudTableViewModelDictionary[hudLayout.LayoutId];

            // update positions 
            foreach (var hudPosition in hudLayout.HudPositions)
            {
                var hudToUpdate = activeLayout?.HudPositions?.FirstOrDefault(x => x.Seat == hudPosition.SeatNumber && (int)x.HudType == hudPosition.HudType);
                if (hudToUpdate == null)
                {
                    continue;
                }

                hudToUpdate.Position = hudPosition.Position;

                if (viewModel == null)
                {
                    continue;
                }

                var hudElementToUpdate = viewModel.HudElements.FirstOrDefault(x => x.Seat == hudToUpdate.Seat && x.HudType == hudToUpdate.HudType);
                if (hudElementToUpdate != null)
                {
                    hudElementToUpdate.Position = hudToUpdate.Position;
                }
            }

            var hudData = new HudSavedDataInfo
            {
                LayoutId = activeLayout.LayoutId,
                Name = activeLayout.Name,
                HudTable = viewModel,
                Stats = activeLayout.HudStats
            };

            App.Current.Dispatcher.Invoke(() => hudLayoutsService.SaveAs(hudData));

            LogProvider.Log.Info("Save Hud Layout received");
        }

        public void ReplayHand(long gameNumber, short pokerSiteId)
        {
            LogProvider.Log.Info("ReplayHand received");
        }
    }
}
