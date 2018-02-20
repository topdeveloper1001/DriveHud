using DriveHUD.Application.Views.Replayer;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Interfaces;
using Model.Replayer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DriveHUD.Application.ViewModels.Replayer
{
    internal class ReplayerService : IReplayerService
    {
        private const int REPLAYER_LAST_HANDS_AMOUNT = 10;

        private FixedSizeList<ReplayerDataModel> _replayerDataModelList;

        private IDataService _dataService;

        private SingletonStorageModel _storageModel { get { return ServiceLocator.Current.GetInstance<SingletonStorageModel>(); } }

        public ReplayerService()
        {
            _replayerDataModelList = new FixedSizeList<ReplayerDataModel>(REPLAYER_LAST_HANDS_AMOUNT);
            _dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public void ReplayHand(string playerName, long gamenumber, short pokerSiteId, bool showHoleCards)
        {
            var game = _dataService.GetGame(gamenumber, pokerSiteId);

            bool displayPotList = true;

            if (game == null)
            {
                LogProvider.Log.Error(this, $"Cannot find game with id {gamenumber} for site {pokerSiteId}");
                return;
            }

            if (string.IsNullOrWhiteSpace(playerName) || !game.Players.Any(x => x.PlayerName == playerName && !x.IsSittingOut))
            {
                playerName = game.Players.FirstOrDefault(x => !x.IsSittingOut).PlayerName;

                // didn't find player in the game, so do not display pot list
                displayPotList = false;
            }

            if (string.IsNullOrWhiteSpace(playerName))
            {
                LogProvider.Log.Error(this, $"Cannot find active player for game {gamenumber}, site {pokerSiteId}");
                return;
            }

            var statistics = _dataService.GetPlayerStatisticFromFile(playerName, pokerSiteId);

            var currentStat = statistics.FirstOrDefault(x => x.GameNumber == gamenumber);

            if (currentStat == null)
            {
                LogProvider.Log.Error(this, $"Cannot find statistics for player {playerName}, site {pokerSiteId}, game {gamenumber}");
                return;
            }

            ReplayHand(currentStat, displayPotList ? statistics : new List<Playerstatistic>(), showHoleCards);
        }

        public void ReplayHand(Playerstatistic currentStat, IList<Playerstatistic> statistics, bool showHoleCards)
        {
            if (currentStat == null)
            {
                LogProvider.Log.Error(this, new ArgumentNullException(nameof(currentStat)));
                return;
            }

            if (statistics == null)
            {
                LogProvider.Log.Error(this, new ArgumentNullException(nameof(statistics)));
                return;
            }

            _replayerDataModelList.ForEach(x => x.IsActive = false);

            var dataModelStatistic = new ReplayerDataModel(currentStat);

            var dataModel = _replayerDataModelList.FirstOrDefault(x => x.Equals(dataModelStatistic));

            if (dataModel == null)
            {
                dataModelStatistic.IsActive = true;
                _replayerDataModelList.Add(dataModelStatistic);
            }
            else
            {
                dataModel.IsActive = true;
                _replayerDataModelList.Move(_replayerDataModelList.IndexOf(dataModel), _replayerDataModelList.Count - 1);
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                ReplayerView replayer = new ReplayerView(_replayerDataModelList, ReplayerHelpers.CreateSessionHandsList(statistics, currentStat), showHoleCards);
                replayer.IsTopmost = true;
                replayer.Show();
                replayer.IsTopmost = false;
            });
        }
    }
}
