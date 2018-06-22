//-----------------------------------------------------------------------
// <copyright file="EquityCalculatorViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.EquityCalculator.Analyzer;
using DriveHUD.EquityCalculator.Base.Calculations;
using DriveHUD.EquityCalculator.Models;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using Microsoft.Practices.ServiceLocation;
using Model.Enums;
using Model.Events;
using Model.Interfaces;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveHUD.EquityCalculator.ViewModels
{
    public class EquityCalculatorViewModel : BaseViewModel
    {
        #region Fields

        private CancellationTokenSource cts;
        private HandHistories.Objects.Hand.HandHistory _currentHandHistory = new HandHistories.Objects.Hand.HandHistory();
        private Street _currentStreet;

        private readonly int _playersCount = 4;
        private BoardModel _board = new BoardModel();
        private ObservableCollection<PlayerModel> _playersModelList = new ObservableCollection<PlayerModel>();
        private bool? _isCalculationRunning = false;
        private bool _isCalculateEquityError = false;
        private bool _isCanExport = false;

        private bool _isPreflopVisible = false;
        private bool _isFlopVisible = false;
        private bool _isTurnVisible = false;
        private bool _isRiverVisible = false;

        private bool _autoGenerateHandRanges = true;

        #endregion

        #region Properties

        public InteractionRequest<CardSelectorNotification> CardSelectorRequest { get; private set; }

        public InteractionRequest<CalculateBluffNotification> CalculateBluffRequest { get; private set; }

        public InteractionRequest<ExportNotification> ExportRequest { get; private set; }

        public BoardModel Board
        {
            get
            {
                return _board;
            }
        }

        public ObservableCollection<PlayerModel> PlayersList
        {
            get
            {
                return _playersModelList;
            }
            set
            {
                _playersModelList = value;
            }
        }

        public bool? IsCalculationRunning
        {
            get { return _isCalculationRunning; }
            set { SetProperty(ref _isCalculationRunning, value); }
        }

        public bool IsCalculateEquityError
        {
            get { return _isCalculateEquityError; }
            set { SetProperty(ref _isCalculateEquityError, value); }
        }

        public bool IsCanExport
        {
            get { return _isCanExport; }
            set { SetProperty(ref _isCanExport, value); }
        }

        public bool IsPreflopVisible
        {
            get
            {
                return _isPreflopVisible;
            }

            set
            {
                SetProperty(ref _isPreflopVisible, value);
            }
        }

        public bool IsFlopVisible
        {
            get
            {
                return _isFlopVisible;
            }

            set
            {
                SetProperty(ref _isFlopVisible, value);
            }
        }

        public bool IsTurnVisible
        {
            get
            {
                return _isTurnVisible;
            }

            set
            {
                SetProperty(ref _isTurnVisible, value);
            }
        }

        public bool IsRiverVisible
        {
            get
            {
                return _isRiverVisible;
            }

            set
            {
                SetProperty(ref _isRiverVisible, value);
            }
        }

        public Street CurrentStreet
        {
            get
            {
                return _currentStreet;
            }
            set
            {
                SetProperty(ref _currentStreet, value);
            }
        }

        public bool AutoGenerateHandRanges
        {
            get { return _autoGenerateHandRanges; }
            set
            {
                SetProperty(ref _autoGenerateHandRanges, value);
            }
        }

        public Street SelectedStreet
        {
            get
            {
                var boardCardsCount = Board.Cards?.Count(c => c.Validate()) ?? 0;

                var street = boardCardsCount == 3 ? Street.Flop :
                    boardCardsCount == 4 ? Street.Turn :
                    boardCardsCount == 5 ? Street.River : Street.Preflop;

                return street;
            }
        }

        #endregion

        #region ICommand

        public ICommand SelectCommand { get; private set; }

        public ICommand ClearCommand { get; private set; }

        public ICommand CalculateEquityCommand { get; private set; }

        public ICommand ExportDataCommand { get; private set; }

        public ICommand ResetAllCommand { get; private set; }

        public ICommand RangeCommand { get; private set; }

        public ICommand CalculateBluffCommand { get; private set; }

        public ICommand ShowStreetCardsCommand { get; private set; }

        public ICommand SetAutoRangeForHeroCommand { get; private set; }

        #endregion

        public EquityCalculatorViewModel()
        {
            Init();
        }

        private void Init()
        {
            SelectCommand = new RelayCommand(SelectBoardRange);
            ClearCommand = new RelayCommand(Clear);
            CalculateEquityCommand = new RelayCommand((Action<object>)CalculateEquity);
            ExportDataCommand = new RelayCommand(ExportData);
            ResetAllCommand = new RelayCommand(ResetAll);
            RangeCommand = new RelayCommand(SelectPlayerRange);
            CalculateBluffCommand = new RelayCommand(CalculateBluff);
            ShowStreetCardsCommand = new RelayCommand(ShowStreetCards);
            SetAutoRangeForHeroCommand = new RelayCommand(SetAutoRangeForHero);

            CardSelectorRequest = new InteractionRequest<CardSelectorNotification>();
            CalculateBluffRequest = new InteractionRequest<CalculateBluffNotification>();
            ExportRequest = new InteractionRequest<ExportNotification>();

            InitPlayersList();

            _board.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(BoardModel.Cards))
                {
                    // need to set new cards for each player model
                    PlayersList?.ForEach(x =>
                    {
                        x.Ranges
                         .OfType<EquityRangeSelectorItemViewModel>()
                         .ForEach(r => r.UsedCards = _board.Cards);

                        x.UpdateEquityData();

                        x.CheckBluffToValueBetRatio(CountOpponents(), SelectedStreet);
                    });
                }
            };

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RequestEquityCalculatorEvent>().Subscribe(LoadData);
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<EquityCalculatorRangeRemovedEvent>().Subscribe(RangeRemoved);
        }

        private void SetAutoRangeForHero(object obj)
        {
            if (_currentHandHistory == null || _currentHandHistory.Hero == null)
            {
                return;
            }

            var heroAutoHands = GetHeroAutoRange(_currentHandHistory.Hero.PlayerName);

            if (heroAutoHands != null)
            {
                var hero = PlayersList.FirstOrDefault(x => x.PlayerName == _currentHandHistory.Hero.PlayerName);

                if (hero != null)
                {
                    var opponentsCount = CountOpponents();

                    hero.SetRanges(heroAutoHands);
                    hero.CheckBluffToValueBetRatio(opponentsCount, SelectedStreet);
                }
            }
        }

        internal IEnumerable<EquityRangeSelectorItemViewModel> GetHeroAutoRange(string heroName)
        {
            try
            {
                var handHistory = _currentHandHistory.DeepClone();

                handHistory.Hero = handHistory.Players.FirstOrDefault(x => x.PlayerName == heroName);

                var heroAutoHands = MainAnalyzer.GetHeroRange(handHistory, _currentStreet);

                if (heroAutoHands != null && heroAutoHands.Any())
                {
                    heroAutoHands.ForEach(r => r.UsedCards = _board.Cards);

                    var opponentsCount = CountOpponents();

                    BluffToValueRatioCalculator.AdjustPlayerRange(heroAutoHands, _currentStreet, handHistory, GetBoardText(), opponentsCount);
                    return heroAutoHands;
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not build auto range for hero", e);
            }

            return null;
        }

        internal Dictionary<MadeHandType, int> GetCombosByHandType(IEnumerable<string> range)
        {
            try
            {
                var boardCards = GetBoardText();

                var combosByHandType = MainAnalyzer.GetCombosByHandType(range, boardCards);

                return combosByHandType;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not get combos by hand type.", e);
            }

            return new Dictionary<MadeHandType, int>();
        }

        private void InitPlayersList()
        {
            if (PlayersList == null)
            {
                PlayersList = new ObservableCollection<PlayerModel>();
            }
            else
            {
                PlayersList.Clear();
            }

            for (int i = 0; i < _playersCount; i++)
            {
                PlayersList.Add(new PlayerModel());
            }
        }

        private void RangeRemoved(EventArgs obj)
        {
            ClearEquity();
        }

        private void ClearEquity()
        {
            foreach (var player in PlayersList.Where(x => x.EquityValue != 0))
            {
                player.EquityValue = 0.0;
            }

            IsCalculateEquityError = false;
            IsCanExport = false;
        }

        private async Task CalculateEquity()
        {
            cts = new CancellationTokenSource();
            IsCalculationRunning = true;
            List<double[]> result = new List<double[]>();

            try
            {
                LogProvider.Log.Info("Equity calculation started");
                var boardString = GetBoardText();
                result = await HoldemEquityCalculator.CalculateEquityAsync(PlayersList.Select(x => string.Join(",", x.GetPlayersHand(true))), boardString, cts.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("Canceled by user");
                LogProvider.Log.Info("Equity calculation stopped by user");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogProvider.Log.Error(this, "Equity calculation error", ex);
            }
            catch (ArgumentException ex)
            {
                LogProvider.Log.Error(this, "Equity calculation error", ex);
            }
            finally
            {
                IsCalculationRunning = false;
                cts.Dispose();
                cts = null;
                LogProvider.Log.Info("Equity calculation finished");
            }

            if (result.Count() == PlayersList.Count())
            {
                for (int i = 0; i < result.Count(); i++)
                {
                    PlayersList.ElementAt(i).EquityValue = result[i][0];
                    PlayersList.ElementAt(i).WinPrct = result[i][1];
                    PlayersList.ElementAt(i).TiePrct = result[i][2];
                }
            }
            IsCanExport = true;
        }

        #region LoadData members

        private void LoadData(RequestEquityCalculatorEventArgs obj)
        {
            try
            {
                ResetAll(null);

                if (obj.IsEmptyRequest)
                {
                    _currentHandHistory = null;
                    IsPreflopVisible = IsFlopVisible = IsTurnVisible = IsRiverVisible = false;
                    CurrentStreet = Street.Null;
                    InitPlayersList();
                    return;
                }

                _currentHandHistory = ServiceLocator.Current.GetInstance<IDataService>().GetGame(obj.GameNumber, obj.PokersiteId);

                if (_currentHandHistory == null)
                {
                    return;
                }

                SetStreetVisibility(_currentHandHistory);
                LoadBoardData(_currentHandHistory, _currentHandHistory.CommunityCards.Count());
                LoadPlayersData(_currentHandHistory);

                var strongestOpponent = CalculateStrongestOpponent(_currentHandHistory, _currentStreet);

                PlayersList.RemoveAll(x =>
                       _currentHandHistory
                            .HandActions.Any(a => (a.HandActionType == HandActionType.FOLD)
                                                && (a.PlayerName == x.PlayerName))
                    && (x.PlayerName != StorageModel.PlayerSelectedItem?.Name)
                    && (x.PlayerName != strongestOpponent));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                LogProvider.Log.Error(this, "Board contains more than 5 cards", ex);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Error during EquityCalculatorViewModel.LoadData()", ex);
            }
        }

        private void SetStreetVisibility(HandHistories.Objects.Hand.HandHistory CurrentGame)
        {
            IsPreflopVisible = CurrentGame != null && CurrentGame.PreFlop != null && CurrentGame.PreFlop.Any();
            IsFlopVisible = CurrentGame != null && CurrentGame.Flop != null && CurrentGame.Flop.Any();
            IsTurnVisible = CurrentGame != null && CurrentGame.Turn != null && CurrentGame.Turn.Any();
            IsRiverVisible = CurrentGame != null && CurrentGame.River != null && CurrentGame.River.Any();

            if (IsRiverVisible)
                CurrentStreet = Street.River;
            else if (IsTurnVisible)
                CurrentStreet = Street.Turn;
            else if (IsFlopVisible)
                CurrentStreet = Street.Flop;
            else if (IsPreflopVisible)
                CurrentStreet = Street.Preflop;
        }

        private void LoadPlayersData(HandHistories.Objects.Hand.HandHistory CurrentGame)
        {
            PlayersList.Clear();

            var players = CurrentGame.Players;

            for (int i = 0; i < players.Count; i++)
            {
                var list = new List<CardModel>();

                if (players[i].hasHoleCards)
                {
                    foreach (var card in players[i].HoleCards)
                    {
                        list.Add(new CardModel()
                        {
                            Rank = new RangeCardRank().StringToRank(card.Rank),
                            Suit = new RangeCardSuit().StringToSuit(card.Suit)
                        });
                    }
                }

                if (i > PlayersList.Count - 1)
                {
                    PlayersList.Add(new PlayerModel());
                }

                var currentPlayer = PlayersList[i];
                currentPlayer.SetCollection(list);
                currentPlayer.PlayerName = players[i].PlayerName;
            }
        }

        private void LoadBoardData(HandHistories.Objects.Hand.HandHistory CurrentGame, int numberOfCards)
        {
            if (CurrentGame.CommunityCards.Count() > 5)
            {
                throw new ArgumentOutOfRangeException(
                    "CommunityCards",
                    CurrentGame.CommunityCards.Count(),
                    "Can handle no more than 5 cards on a board");
            }

            List<CardModel> boardCardsList = new List<CardModel>();

            int length = numberOfCards < CurrentGame.CommunityCards.Count() ?
                            numberOfCards :
                            CurrentGame.CommunityCards.Count();

            for (int i = 0; i < length; i++)
            {
                boardCardsList.Add(new CardModel()
                {
                    Rank = new RangeCardRank().StringToRank(CurrentGame.CommunityCards.ElementAt(i).Rank),
                    Suit = new RangeCardSuit().StringToSuit(CurrentGame.CommunityCards.ElementAt(i).Suit)
                });
            }

            Board.SetCollection(boardCardsList);
        }

        private string CalculateStrongestOpponent(HandHistories.Objects.Hand.HandHistory CurrentGame, Street CurrentStreet)
        {
            try
            {
                IEnumerable<EquityRangeSelectorItemViewModel> oponnentHands = new List<EquityRangeSelectorItemViewModel>();

                var opponentName = string.Empty;

                MainAnalyzer.GetStrongestOpponent(CurrentGame, CurrentStreet, out opponentName, out oponnentHands);

                if (AutoGenerateHandRanges)
                {
                    if (!string.IsNullOrEmpty(opponentName) &&
                        oponnentHands.Any() &&
                        PlayersList.Any(x => x.PlayerName == opponentName && x.Cards.All(c => !c.Validate())))
                    {
                        oponnentHands.ForEach(r => r.UsedCards = _board.Cards);

                        var player = PlayersList.FirstOrDefault(x => x.PlayerName == opponentName);
                        player?.SetRanges(oponnentHands);
                    }
                }

                return opponentName;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not determine the strongest opponent", ex);
            }

            return string.Empty;
        }

        private int CountOpponents()
        {
            if (_currentHandHistory == null)
            {
                return PlayersList.Count - 1;
            }

            var opponentsCount = _currentHandHistory.HandActions
                .Where(x => x.HandActionType != HandActionType.FOLD && x.PlayerName != _currentHandHistory.Hero?.PlayerName && x.Street >= Street.Flop)
                .Select(x => x.PlayerName)
                .Distinct()
                .Count() - 1;

            return opponentsCount;
        }

        #endregion

        #region Popups

        private void RaiseCardSelectorView(ICardCollectionContainer container, CardSelectorType selectorType)
        {
            IsCalculateEquityError = false;

            CardSelectorRequest.Raise(
               new CardSelectorNotification
               {
                   Source = this,
                   Title = string.Empty,
                   CardsContainer = container,
                   SelectorType = selectorType,
                   UsedCards = GetProhibitedCardsFor(container),
                   BoardCards = Board.Cards.Where(x => x.Rank != RangeCardRank.None && x.Suit != RangeCardSuit.None).ToList()
               },
               returned =>
               {
                   if (returned != null && returned.Confirmed)
                   {
                       if (returned.ReturnType.Equals(CardSelectorReturnType.Cards))
                       {
                           container.SetCollection(returned.CardsContainer.Cards);
                       }
                       else if (returned.ReturnType.Equals(CardSelectorReturnType.Range))
                       {
                           container.SetRanges(returned.CardsContainer.Ranges);
                           (container as PlayerModel)?.CheckBluffToValueBetRatio(CountOpponents(), SelectedStreet);
                       }

                       ClearEquity();
                   }
               });
        }

        private void RaiseCalculateBluffNotification(PlayerModel playerModel)
        {
            CalculateBluffRequest.Raise(new CalculateBluffNotification
            {
                Title = string.Empty,
                EquityValue = playerModel.EquityValue,
                NumberOfPlayers = PlayersList.Where(x => x.GetPlayersHand().Count > 0).Count()
            });
        }

        private void RaiseExportDataNotification()
        {
            ExportRequest.Raise(new ExportNotification()
            {
                Title = string.Empty,
                CurrentHandHistory = _currentHandHistory,
                PlayersList = PlayersList.Where(x => x.PlayerCards.Count > 0),
                BoardCards = Board.Cards
            });
        }

        private List<CardModel> GetProhibitedCardsFor(ICardCollectionContainer container)
        {
            var usedCards = new List<CardModel>();

            foreach (var player in PlayersList.Where(x => x != container))
            {
                usedCards.AddRange(player.Cards.Where(x => x.Rank != RangeCardRank.None && x.Suit != RangeCardSuit.None));
            }

            if (Board != container)
            {
                usedCards.AddRange(Board.Cards.Where(x => x.Rank != RangeCardRank.None && x.Suit != RangeCardSuit.None));
            }

            return usedCards;
        }
        #endregion

        #region ICommand implementation 

        private void ShowStreetCards(object obj)
        {
            CurrentStreet = (Street)obj;

            switch (CurrentStreet)
            {
                case Street.Preflop:
                    LoadBoardData(_currentHandHistory, 0);
                    break;
                case Street.Flop:
                    LoadBoardData(_currentHandHistory, 3);
                    break;
                case Street.Turn:
                    LoadBoardData(_currentHandHistory, 4);
                    break;
                case Street.River:
                    LoadBoardData(_currentHandHistory, 5);
                    break;
            }

            CalculateStrongestOpponent(_currentHandHistory, _currentStreet);
            ClearEquity();
        }

        private void ResetAll(object obj)
        {
            foreach (var player in PlayersList)
            {
                player.Reset();
            }

            Board.Reset();
            ClearEquity();

            IsCalculateEquityError = false;
        }

        private void ExportData(object obj)
        {
            RaiseExportDataNotification();
        }

        private async void CalculateEquity(object obj)
        {
            if (IsCalculationRunning.HasValue)
            {
                if (IsCalculationRunning.Value)
                {
                    if (cts != null)
                    {
                        IsCalculationRunning = null;
                        cts.Cancel();
                    }
                    return;
                }
            }
            else
            {
                return;
            }

            if (PlayersList.Where(x => x.GetPlayersHand().Count > 0).Count() < 2)
            {
                IsCalculateEquityError = true;
                return;
            }

            IsCalculateEquityError = false;

            await CalculateEquity();
        }

        private void Clear(object obj)
        {
            this.Board.SetCollection(null);
        }

        private void SelectBoardRange(object obj)
        {
            RaiseCardSelectorView(this.Board, CardSelectorType.BoardSelector);
        }

        private void SelectPlayerRange(object obj)
        {
            if ((obj != null) && (obj is PlayerModel))
            {
                var selectedPlayer = (obj as PlayerModel);
                RaiseCardSelectorView(selectedPlayer, CardSelectorType.PlayerSelector);
            }
        }

        private void CalculateBluff(object obj)
        {
            if ((obj != null) && (obj is PlayerModel))
            {
                var selectedPlayer = (obj as PlayerModel);
                RaiseCalculateBluffNotification(selectedPlayer);
            }
        }

        private string GetBoardText()
        {
            return Board?.ToString().Replace("x", string.Empty).Replace("X", string.Empty);
        }

        #endregion
    }
}