using DriveHUD.Application.Views.Replayer;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using DriveHUD.ViewModels.Replayer;
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

        private FixedSizeList<ReplayerDataModel> replayerDataModelList;

        private readonly IDataService dataService;

        private readonly IPlayerStatisticRepository playerStatisticRepository;

        private SingletonStorageModel _storageModel { get { return ServiceLocator.Current.GetInstance<SingletonStorageModel>(); } }

        public ReplayerService()
        {
            replayerDataModelList = new FixedSizeList<ReplayerDataModel>(REPLAYER_LAST_HANDS_AMOUNT);
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
        }

        public void ReplayHand(string playerName, long gamenumber, short pokerSiteId, bool showHoleCards)
        {
            var game = dataService.GetGame(gamenumber, pokerSiteId);

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

            var currentStat = _storageModel.StatisticCollection.FirstOrDefault(x => x.GameNumber == gamenumber);
           
            if (currentStat == null)
            {
                LogProvider.Log.Error(this, $"Cannot find statistics for player {playerName}, site {pokerSiteId}, game {gamenumber}");
                return;
            }

            var statistics = currentStat.IsTourney ? _storageModel.FilteredTournamentPlayerStatistic : _storageModel.FilteredCashPlayerStatistic;

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

            replayerDataModelList.ForEach(x => x.IsActive = false);

            var dataModelStatistic = new ReplayerDataModel(currentStat);

            var dataModel = replayerDataModelList.FirstOrDefault(x => x.Equals(dataModelStatistic));

            if (dataModel == null)
            {
                dataModelStatistic.IsActive = true;
                replayerDataModelList.Add(dataModelStatistic);
            }
            else
            {
                dataModel.IsActive = true;
                replayerDataModelList.Move(replayerDataModelList.IndexOf(dataModel), replayerDataModelList.Count - 1);
            }

            App.Current.Dispatcher.Invoke(() =>
            {
                ReplayerView replayer = new ReplayerView(replayerDataModelList, ReplayerHelpers.CreateSessionHandsList(statistics, currentStat), showHoleCards);
                replayer.IsTopmost = true;
                replayer.Show();
                replayer.IsTopmost = false;
            });
        }
    }
}
