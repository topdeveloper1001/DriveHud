using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.EquityCalculator.Analyzer;
using DriveHUD.EquityCalculator.Base.Calculations;
using DriveHUD.EquityCalculator.Models;
using DriveHUD.ViewModels;
using HandHistories.Objects.Actions;
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
        private HandHistories.Objects.Cards.Street _currentSreet;

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
            get { return _board; }
            set
            {
                SetProperty(ref _board, value);
                ClearEquity();
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

        public bool AutoGenerateHandRanges
        {
            get { return _autoGenerateHandRanges; }
            set
            {
                SetProperty(ref _autoGenerateHandRanges, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand SelectCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand CalculateEquityCommand { get; set; }
        public ICommand ExportDataCommand { get; set; }
        public ICommand ResetAllCommand { get; set; }
        public ICommand RangeCommand { get; set; }
        public ICommand CalculateBluffCommand { get; set; }
        public ICommand ShowStreetCardsCommand { get; set; }

        #endregion

        public EquityCalculatorViewModel()
        {
            Init();
        }

        private void Init()
        {
            SelectCommand = new RelayCommand(SelectBoardRange);
            ClearCommand = new RelayCommand(Clear);
            CalculateEquityCommand = new RelayCommand((Action<object>)this.CalculateEquity);
            ExportDataCommand = new RelayCommand(ExportData);
            ResetAllCommand = new RelayCommand(ResetAll);
            RangeCommand = new RelayCommand(SelectPlayerRange);
            CalculateBluffCommand = new RelayCommand(CalculateBluff);
            ShowStreetCardsCommand = new RelayCommand(ShowStreetCards);

            CardSelectorRequest = new InteractionRequest<CardSelectorNotification>();
            CalculateBluffRequest = new InteractionRequest<CalculateBluffNotification>();
            ExportRequest = new InteractionRequest<ExportNotification>();

            InitPlayersList();

            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<RequestEquityCalculatorEvent>().Subscribe(LoadData);
            ServiceLocator.Current.GetInstance<IEventAggregator>().GetEvent<EquityCalculatorRangeRemovedEvent>().Subscribe(RangeRemoved);
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
                var boardString = Board.ToString().Replace("x", "").Replace("X", "");
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

                var strongestOpponent = CalculateStrongestOpponent(_currentHandHistory, _currentSreet);

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
            IsPreflopVisible = CurrentGame.PreFlop != null && CurrentGame.PreFlop.Any();
            IsFlopVisible = CurrentGame.Flop != null && CurrentGame.Flop.Any();
            IsTurnVisible = CurrentGame.Turn != null && CurrentGame.Turn.Any();
            IsRiverVisible = CurrentGame.River != null && CurrentGame.River.Any();

            if (IsRiverVisible)
                _currentSreet = HandHistories.Objects.Cards.Street.River;
            else if (IsTurnVisible)
                _currentSreet = HandHistories.Objects.Cards.Street.Turn;
            else if (IsFlopVisible)
                _currentSreet = HandHistories.Objects.Cards.Street.Flop;
            else if (IsPreflopVisible)
                _currentSreet = HandHistories.Objects.Cards.Street.Preflop;
        }

        private void LoadPlayersData(HandHistories.Objects.Hand.HandHistory CurrentGame)
        {
            PlayersList.Clear();
            var players = CurrentGame.Players;
            for (int i = 0; i < players.Count(); i++)
            {
                List<CardModel> list = new List<CardModel>();
                if (players.ElementAt(i).hasHoleCards)
                {
                    foreach (var card in players.ElementAt(i).HoleCards)
                    {
                        list.Add(new CardModel()
                        {
                            Rank = new RangeCardRank().StringToRank(card.Rank),
                            Suit = new RangeCardSuit().StringToSuit(card.Suit)
                        });
                    }
                }
                if (i > PlayersList.Count() - 1)
                {
                    PlayersList.Add(new PlayerModel());
                }

                var currentPlayer = PlayersList.ElementAt(i);
                currentPlayer.SetCollection(list);
                currentPlayer.PlayerName = players.ElementAt(i).PlayerName;
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

        private string CalculateStrongestOpponent(HandHistories.Objects.Hand.HandHistory CurrentGame, HandHistories.Objects.Cards.Street CurrentStreet)
        {
            try
            {
                IEnumerable<RangeSelectorItemViewModel> oponnentHands = new List<RangeSelectorItemViewModel>();

                var opponentName = string.Empty;

                MainAnalyzer.GetStrongestOpponent(CurrentGame, CurrentStreet, out opponentName, out oponnentHands);

                if (AutoGenerateHandRanges)
                {
                    if (!string.IsNullOrEmpty(opponentName) && PlayersList.Any(x => x.PlayerName == opponentName))
                    {
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
        #endregion

        #region Popups
        private void RaiseCardSelectorView(ICardCollectionContainer container, CardSelectorType selectorType)
        {
            IsCalculateEquityError = false;

            this.CardSelectorRequest.Raise(
               new CardSelectorNotification
               {
                   Title = string.Empty,
                   CardsContainer = container,
                   SelectorType = selectorType,
                   UsedCards = GetProhibitedCardsFor(container)
               },
               returned =>
               {
                   if (returned != null)
                   {
                       if (returned.ReturnType.Equals(CardSelectorReturnType.Cards))
                       {
                           container.SetCollection(returned.CardsContainer.Cards);
                       }
                       else if (returned.ReturnType.Equals(CardSelectorReturnType.Range))
                       {
                           container.SetRanges(returned.CardsContainer.Ranges);
                       }
                       ClearEquity();
                   }
               });
        }

        private void RaiseCalculateBluffNotification(PlayerModel playerModel)
        {
            this.CalculateBluffRequest.Raise(new CalculateBluffNotification
            {
                Title = string.Empty,
                EquityValue = playerModel.EquityValue,
                NumberOfPlayers = PlayersList.Where(x => x.GetPlayersHand().Count > 0).Count()
            });
        }

        private void RaiseExportDataNotification()
        {
            this.ExportRequest.Raise(new ExportNotification()
            {
                Title = string.Empty,
                CurrentHandHistory = _currentHandHistory,
                PlayersList = this.PlayersList.Where(x => x.PlayerCards.Count > 0),
                BoardCards = this.Board.Cards
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
            _currentSreet = (HandHistories.Objects.Cards.Street)obj;

            switch (_currentSreet)
            {
                case HandHistories.Objects.Cards.Street.Preflop:
                    LoadBoardData(_currentHandHistory, 0);
                    break;
                case HandHistories.Objects.Cards.Street.Flop:
                    LoadBoardData(_currentHandHistory, 3);
                    break;
                case HandHistories.Objects.Cards.Street.Turn:
                    LoadBoardData(_currentHandHistory, 4);
                    break;
                case HandHistories.Objects.Cards.Street.River:
                    LoadBoardData(_currentHandHistory, 5);
                    break;
            }

            CalculateStrongestOpponent(_currentHandHistory, _currentSreet);
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
        #endregion
    }
}