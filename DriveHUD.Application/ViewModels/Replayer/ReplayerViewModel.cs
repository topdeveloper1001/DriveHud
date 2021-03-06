﻿//-----------------------------------------------------------------------
// <copyright file="ReplayerViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2019 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Common.Wpf.Actions;
using DriveHUD.Entities;
using HandHistories.Objects.Actions;
using HandHistories.Objects.Cards;
using HandHistories.Objects.Players;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Events;
using Model.Importer;
using Model.Interfaces;
using Model.Replayer;
using Model.Settings;
using Model.Solvers;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Telerik.Windows.Controls;
using HandHistory = HandHistories.Objects.Hand.HandHistory;
using Player = HandHistories.Objects.Players.Player;

namespace DriveHUD.Application.ViewModels.Replayer
{
    public class ReplayerViewModel : BaseViewModel
    {
        #region Fields

        private const int PLAYERS_COLLECTION_SIZE = 10;
        private const int TIMER_INTERVAL_MS = 500;

        private IDataService dataService;
        private DispatcherTimer timer;

        private readonly ISettingsService settingsService;
        private readonly SingletonStorageModel storageModel;
        private readonly IEventAggregator eventAggregator;

        #endregion

        #region Constructor

        public ReplayerViewModel()
        {
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            Initialize();
        }

        private void Initialize()
        {
            NotificationRequest = new InteractionRequest<INotification>();

            ToEndCommand = new RelayCommand(ToEnd);
            NextStepCommand = new RelayCommand(NextStep);
            PrevStepCommand = new RelayCommand(PrevStep);
            ToStartCommand = new RelayCommand(ToStart);
            PlayCommand = new RelayCommand(StartTimer);
            StopCommand = new RelayCommand(StopTimer);
            ToStreetCommand = new RelayCommand(ToStreet);

            TwitterOAuthCommand = new RelayCommand(TwitterOAuthCommandHandler);
            FacebookOAuthCommand = new RelayCommand(FacebookOAuthCommandHandler);
            HandNoteCommand = new RelayCommand(HandNoteShow);
            ShowSupportForumsCommand = new RelayCommand(ShowSupportForums);
            TagHandCommand = new RelayCommand(TagHand);
            PreviousSessionHandCommand = new RelayCommand(() => MoveToSessionHand(false));
            NextSessionHandCommand = new RelayCommand(() => MoveToSessionHand(true));
            OpenEquityCalculatorCommand = new RelayCommand(() =>
            {
                if (CurrentHand == null)
                {
                    return;
                }

                var requestEquityCalculatorEventArgs = new RequestEquityCalculatorEventArgs(CurrentHand.GameNumber, CurrentHand.PokersiteId);
                eventAggregator.GetEvent<RequestEquityCalculatorEvent>().Publish(requestEquityCalculatorEventArgs);

                System.Windows.Application.Current.MainWindow.Topmost = true;
                System.Windows.Application.Current.MainWindow.Topmost = false;
            });

            TableStateList = new List<ReplayerTableState>();
            PlayersCollection = new ObservableCollection<ReplayerPlayerViewModel>();

            for (int i = 0; i < PLAYERS_COLLECTION_SIZE; i++)
            {
                PlayersCollection.Add(new ReplayerPlayerViewModel());
            }

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(TIMER_INTERVAL_MS);
            timer.Tick += OnTimerTick;

            BBFilter = settingsService.GetSettings().GeneralSettings.ReplayerBBFilter;
        }

        #endregion

        #region Methods

        private void OnTimerTick(object sender, EventArgs e)
        {
            StateIndex++;
        }

        private void Update()
        {
            TableStateList.Clear();
            SetPlayersDefaults();

            TotalPotChipsContainer = new ReplayerChipsContainer();
            CommunityCards = new List<ReplayerCardViewModel>();

            if (CurrentGame == null)
            {
                return;
            }

            SetCommunityCards(CurrentGame.CommunityCards);

            decimal anteAmount = Math.Abs(CurrentGame.HandActions.Where(x => x.HandActionType == HandActionType.ANTE).Sum(x => x.Amount));
            decimal currentPotValue = anteAmount;
            decimal totalPotValue = anteAmount;

            foreach (var action in CurrentGame.HandActions.Where(x => x.HandActionType != HandActionType.ANTE))
            {
                if (IsSkipAction(action))
                {
                    continue;
                }

                ReplayerTableState state = new ReplayerTableState();

                ReplayerTableState lastAction = TableStateList.LastOrDefault();

                if (lastAction != null && lastAction.CurrentStreet != action.Street && action.Street >= Street.Flop && action.Street <= Street.River)
                {
                    // if we are inside this "if" we create an extra state between two actions
                    totalPotValue = currentPotValue;
                    state.TotalPotValue = totalPotValue;
                    state.CurrentPotValue = currentPotValue;

                    state.IsStreetChangedAction = true;
                    state.ActivePlayer = new ReplayerPlayerViewModel();
                    state.CurrentStreet = action.Street;
                    TableStateList.Add(state);  // added new state between actions like flop/river
                    state.CurrentAction = action;

                    state = new ReplayerTableState();
                }

                state.CurrentAction = action;
                state.ActionAmount = action.Amount;
                state.CurrentStreet = action.Street;

                ReplayerPlayerViewModel activePlayer = GetActivePlayerLastState(action);
                state.ActivePlayer = new ReplayerPlayerViewModel();
                ReplayerPlayerViewModel.Copy(activePlayer, state.ActivePlayer);

                state.UpdatePlayerState(action);

                if (state.ActivePlayer.IsWin)
                {
                    if (!TableStateList.Any(x => x.ActivePlayer != null && x.ActivePlayer.IsWin))
                    {
                        totalPotValue = currentPotValue;
                    }
                    totalPotValue -= state.ActionAmount;
                }
                else
                {
                    currentPotValue = currentPotValue - state.ActionAmount;
                }
                state.TotalPotValue = totalPotValue;
                state.CurrentPotValue = currentPotValue;
                TableStateList.Add(state);
            }

            StateIndex = 0;
            SliderMax = TableStateList.Count - 1;
        }

        private void SetCommunityCards(BoardCards communityCards)
        {
            foreach (var card in communityCards)
            {
                var newCard = new ReplayerCardViewModel();
                newCard.CardId = card.CardIntValue;
                CommunityCards.Add(newCard);
            }
        }

        private static bool IsSkipAction(HandAction action)
        {
            return action.HandActionType == HandActionType.SHOW
                || action.HandActionType == HandActionType.SHOWS_FOR_LOW
                || action.HandActionType == HandActionType.UNKNOWN;
        }

        private ReplayerPlayerViewModel GetActivePlayerLastState(HandAction action)
        {
            ReplayerPlayerViewModel activePlayer;

            string activePlayerName = action.PlayerName;

            ReplayerTableState activePlayerLastState = TableStateList.LastOrDefault(x => x.ActivePlayer != null && x.ActivePlayer.Name == activePlayerName);

            if (activePlayerLastState == null)
            {
                activePlayer = PlayersCollection.LastOrDefault(x => x.Name == activePlayerName);
            }
            else
            {
                activePlayer = activePlayerLastState.ActivePlayer;
            }

            if (activePlayer == null)
            {
                throw new ArgumentNullException(nameof(activePlayer), $"Cannot find player with name: {activePlayerName}");
            }

            return activePlayer;
        }

        private void SetPlayersDefaults()
        {
            PlayersCollection.ForEach(x => x.Reset());

            if (CurrentGame == null)
            {
                return;
            }

            var dealer = CurrentGame.Players.FirstOrDefault(x => x.SeatNumber == CurrentGame.DealerButtonPosition);

            foreach (Player player in CurrentGame.Players)
            {
                int i = GetPlayersSeatNumber(player, CurrentGame.Players, CurrentGame.GameDescription.SeatType.MaxPlayers);

                PlayersCollection[i].Name = player.PlayerName;
                PlayersCollection[i].IsFinished = false;
                PlayersCollection[i].Bank = player.StartingStack;

                var anteAction = CurrentGame.HandActions.FirstOrDefault(x => x.HandActionType == HandActionType.ANTE && x.PlayerName == player.PlayerName);

                if (anteAction != null)
                {
                    PlayersCollection[i].Bank -= Math.Abs(anteAction.Amount);
                }

                if (dealer == player)
                {
                    PlayersCollection[i].IsDealer = true;
                }

                if (player.hasHoleCards)
                {
                    bool canHideCards = !IsShowHoleCards && ActivePlayerName != player.PlayerName;

                    PlayersCollection[i].SetCards(player.HoleCards.Select(x => x.CardIntValue).ToArray(), canHideCards);
                    PlayersCollection[i].HideCards();
                }
            }
        }

        public List<Player> ActivePlayerHasHoleCard { get; set; }

        public List<Player> ActivePlayerHasHoleCardFolded { get; set; }

        public List<HoleCards> AllDeadCards = new List<HoleCards>();

        public string AllDeadCardsString = string.Empty;

        public string CurrentBoardCards { get; set; }

        public Card[] CurrentBoard { get; set; }

        private void UpdatePlayersEquityWin(ReplayerTableState state)
        {
            if (state == null)
            {
                return;
            }

            //preparing for formula Card on the Board in dependence of street in current state
            switch (state.CurrentAction.Street)
            {
                case Street.Preflop:
                    CurrentBoard = new Card[] { };
                    CurrentBoardCards = string.Empty;
                    break;
                case Street.Flop:
                    CurrentBoard = CurrentGame.CommunityCards.Take(3).ToArray();
                    CurrentBoardCards = new string(CurrentGame.CommunityCardsString.Take(6).ToArray());
                    break;
                case Street.Turn:
                    CurrentBoard = CurrentGame.CommunityCards.Take(4).ToArray();
                    CurrentBoardCards = new string(CurrentGame.CommunityCardsString.Take(8).ToArray());
                    break;
                case Street.River:
                    CurrentBoard = CurrentGame.CommunityCards.ToArray();
                    CurrentBoardCards = CurrentGame.CommunityCardsString;
                    break;
                case Street.Showdown:
                    CurrentBoard = CurrentGame.CommunityCards.ToArray();
                    CurrentBoardCards = CurrentGame.CommunityCardsString;
                    break;
                case Street.Summary:
                    CurrentBoard = CurrentGame.CommunityCards.ToArray();
                    CurrentBoardCards = CurrentGame.CommunityCardsString;
                    break;
            }

            // finding all players having hole cards  
            ActivePlayerHasHoleCard = CurrentGame.Players.Where(pl => pl.hasHoleCards).ToList();

            // searching for dead cards and removing this player from list of ActivePlayerHasHoleCard 
            ActivePlayerHasHoleCardFolded = new List<Player>();

            AllDeadCards.Clear();

            foreach (ReplayerTableState replayerTableState in TableStateList)
            {
                Player playerInTableState = CurrentGame.Players.FirstOrDefault(x => x.PlayerName == replayerTableState.CurrentAction.PlayerName);

                if (playerInTableState != null
                    && TableStateList.IndexOf(replayerTableState) <= TableStateList.IndexOf(state)
                    && replayerTableState.CurrentAction.IsFold
                    && playerInTableState.hasHoleCards)
                {
                    ActivePlayerHasHoleCardFolded.Add(playerInTableState);
                    ActivePlayerHasHoleCard.Remove(playerInTableState);
                    AllDeadCards.Add(playerInTableState.HoleCards);
                    AllDeadCardsString += playerInTableState.Cards;
                }
            }

            var equitySolver = ServiceLocator.Current.GetInstance<IEquitySolver>();

            var gameType = new GeneralGameTypeEnum().ParseGameType(CurrentGame.GameDescription.GameType);

            var isShortDeck = CurrentGame.GameDescription.TableTypeDescriptors.Contains(HandHistories.Objects.GameDescription.TableTypeDescription.ShortDeck);

            var equitySolverParams = new EquitySolverParams
            {
                PlayersCards = ActivePlayerHasHoleCard.Select(x => x.HoleCards).ToArray(),
                BoardCards = CurrentBoard,
                Dead = isShortDeck ?
                    AllDeadCards
                        .Distinct(new LambdaComparer<HoleCards>((x, y) => x.ToString().Equals(y.ToString())))
                        .SelectMany(x => x)
                        .Concat(CardGroup.GetDeadCardsForHoldem6Plus())
                        .ToArray() :
                    AllDeadCards
                        .Distinct(new LambdaComparer<HoleCards>((x, y) => x.ToString().Equals(y.ToString())))
                        .SelectMany(x => x)
                        .ToArray(),
                GameType = gameType
            };

            var equities = equitySolver.CalculateEquity(equitySolverParams)
                .Select(x => Math.Round((decimal)x.Equity * 100, 2))
                .ToArray();

            // updating states in replayer view             
            if (equities != null)
            {
                RefreshBoard(equities, state.CurrentStreet);
            }

            //case of last state. Needed for All-in before River for some cases
            if (TableStateList.IndexOf(state) + 1 == TableStateList.Count &&
                equities != null)
            {
                // updating states in replayer view
                RefreshBoard(equities, Street.Preflop);
            }
        }

        private ReplayerPlayerViewModel _playerInState { get; set; }

        private void RefreshBoard(decimal[] equities, Street street)
        {
            foreach (ReplayerTableState replayerTableState in TableStateList.Where(st => st.CurrentStreet == street))
            {
                try
                {
                    _playerInState = PlayersCollection.FirstOrDefault(u => u.Name == replayerTableState.CurrentAction.PlayerName);
                    if (_playerInState != null
                        && ActivePlayerHasHoleCard.FirstOrDefault(x => x.PlayerName == replayerTableState.CurrentAction.PlayerName) != null
                        && replayerTableState.CurrentAction != null
                        && ActivePlayerHasHoleCard.Count > 1)
                        replayerTableState.ActivePlayer.EquityWin = equities[ActivePlayerHasHoleCard.IndexOf(ActivePlayerHasHoleCard.FirstOrDefault(x => x.PlayerName == replayerTableState.CurrentAction.PlayerName))];
                    else
                        replayerTableState.ActivePlayer.EquityWin = -1;


                    ReplayerPlayerViewModel.CopyEquityWin(replayerTableState.ActivePlayer, _playerInState);
                }
                catch (Exception ex)
                {
                    LogProvider.Log.Error(typeof(Converter), $"Player with name '{replayerTableState.CurrentAction.PlayerName}' has not been found in PlayerCollection in method RefreshBoard in ReplayerViewModel class", ex);
                }
            }
        }

        private void LoadState(int stateIndex)
        {
            if (stateIndex < 0 || stateIndex >= TableStateList.Count)
            {
                return;
            }

            bool isBackward = stateIndex < StateIndex;
            bool isRefreshPlayersRequired = Math.Abs(stateIndex - StateIndex) > 1;
            ReplayerTableState state = TableStateList[stateIndex];
            UpdatePlayersEquityWin(state);
            CurrentStreet = state.CurrentStreet;
            CurrentPotValue = state.CurrentPotValue;
            UpdateTotalPot(state.TotalPotValue);

            if (!IsShowHoleCards)
            {
                ProcessPlayersCards();
            }

            ResetLastActivePlayer(isBackward);

            if (isRefreshPlayersRequired)
            {
                UpdatePlayersToState(state);
            }

            if (state.IsStreetChangedAction)
            {
                ResetPlayersPot(PlayersCollection);
                return;
            }

            var activePlayer = GetActivePlayerForState(state);

            if (activePlayer != null)
            {
                if (activePlayer.IsWin)
                {
                    ResetPlayersPot(PlayersCollection.AsQueryable().Where(x => x != activePlayer));
                }

                activePlayer.UpdateChips();
            }
        }

        private void ProcessPlayersCards()
        {
            foreach (var player in PlayersCollection)
            {
                if (CurrentStreet > Street.River)
                {
                    player.ShowCards();
                }
                else
                {
                    player.HideCards();
                }
            }
        }

        private void UpdatePlayersToState(ReplayerTableState state)
        {
            var prevStates = TableStateList.Take(TableStateList.IndexOf(state));

            foreach (var player in PlayersCollection)
            {
                var playerAction = prevStates.LastOrDefault(x => x.ActivePlayer?.Name == player.Name);

                if (playerAction == null)
                {
                    playerAction = TableStateList.FirstOrDefault(x => x.ActivePlayer.Name == player.Name);

                    if (playerAction != null)
                    {
                        player.Bank = playerAction.ActivePlayer.OldBank;
                        player.ActiveAmount = playerAction.ActivePlayer.OldAmount;
                        player.IsActive = playerAction.ActivePlayer.IsActive;
                        player.UpdateChips();
                    }

                    continue;
                }

                ReplayerPlayerViewModel.Copy(playerAction.ActivePlayer, player);
            }
        }

        private ReplayerPlayerViewModel GetActivePlayerForState(ReplayerTableState state)
        {
            if (state.ActivePlayer == null)
            {
                return null;
            }

            var activePlayer = PlayersCollection.FirstOrDefault(x => x.Name == state.ActivePlayer.Name);

            if (activePlayer != null)
            {
                ReplayerPlayerViewModel.Copy(state.ActivePlayer, activePlayer);
            }

            return activePlayer;
        }

        private void ResetLastActivePlayer(bool resetChipsToPreviousState = false)
        {
            var prevActive = PlayersCollection.FirstOrDefault(x => x.IsActive);

            if (prevActive != null)
            {
                prevActive.IsActive = false;
                prevActive.ActionString = string.Empty;

                if (resetChipsToPreviousState)
                {
                    prevActive.Bank = prevActive.OldBank;
                    prevActive.ActiveAmount = prevActive.OldAmount;
                    prevActive.UpdateChips();
                    prevActive.IsFinished = false;
                }
            }
        }

        private void ResetPlayersPot(IEnumerable<ReplayerPlayerViewModel> collection)
        {
            collection.ForEach(x =>
            {
                x.ActiveAmount = 0;
                x.IsActive = false;
                x.ActionString = string.Empty;
                x.ChipsContainer.ChipsShape.Content = null;
            });
        }

        private void UpdateTotalPot(decimal amount)
        {
            if (TotalPotValue != amount)
            {
                TotalPotValue = amount;
                TotalPotChipsContainer.UpdateChips(TotalPotValue);
            }
        }

        private int GetPlayersSeatNumber(Player player, PlayerList players, int tableSize)
        {
            if (players.Count > tableSize)
            {
                throw new ArgumentException(String.Format("GetPlayersSeatNumber: players amount can't be higher than table size"), "tableSize");
            }

            bool anyZeroSeat = players.Any(x => x.SeatNumber == 0);
            int maxSeat = players.Max(x => x.SeatNumber);

            /* There shouldn't be players with Seat Number equals to 0 */
            if (!anyZeroSeat && (maxSeat <= tableSize))
            {
                return player.SeatNumber - 1;
            }
            /* In case we have  Seat Number equals to 0 */
            else if (anyZeroSeat && maxSeat < tableSize)
            {
                return player.SeatNumber;
            }

            return players.OrderBy(x => x.SeatNumber).ToList().IndexOf(player);
        }

        private void LoadGame(ReplayerDataModel value)
        {
            try
            {
                CurrentGame = value == null ? null : dataService.GetGame(value.GameNumber, value.PokersiteId);
                Update();
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, "Could not load game in replayer.", ex);
            }
        }

        internal void RaiseNotification(string content, string title)
        {
            NotificationRequest.Raise(
                    new PopupActionNotification
                    {
                        Content = content,
                        Title = title,
                    },
                    n => { });
        }

        #endregion

        #region ICommand Implementations

        private void ToEnd(object obj)
        {
            StopTimer(null);

            ResetPlayersPot(PlayersCollection);
            PlayersCollection.Where(x => !x.IsFinished && TableStateList.Any(t => t.ActivePlayer.Name == x.Name && t.ActivePlayer.IsFinished)).ForEach(x => x.IsFinished = true);
            StateIndex = TableStateList.Count - 1;

            ReplayerTableState replayerTableStateBeforeSummary = TableStateList.FirstOrDefault(x => x.CurrentStreet == Street.Summary);
            UpdatePlayersEquityWin(replayerTableStateBeforeSummary); //added in order to update equity state for winning actions when we go to the end of TableStateList
        }

        private void NextStep(object obj)
        {
            StopTimer(null);
            StateIndex++;
        }

        private void PrevStep(object obj)
        {
            StopTimer(null);
            StateIndex--;
        }

        private void ToStart(object obj)
        {
            StopTimer(null);

            PlayersCollection.Where(x => x.IsFinished && TableStateList.Any(t => t.ActivePlayer.Name == x.Name)).ForEach(x => x.IsFinished = false);
            StateIndex = 0;
        }

        private void StartTimer(object obj)
        {
            if (!timer.IsEnabled)
            {
                timer.Start();
            }
        }

        private void StopTimer(object obj)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }
        }

        private void ToStreet(object obj)
        {
            if (obj == null)
            {
                return;
            }

            StopTimer(null);

            if (Enum.TryParse(obj.ToString(), out Street outStreet))
            {
                var streetState = TableStateList.FirstOrDefault(x => x.CurrentStreet == outStreet);

                if (streetState != null)
                {
                    if (outStreet <= Street.Preflop)
                    {
                        PlayersCollection.Where(x => x.IsFinished && TableStateList.Any(t => t.ActivePlayer.Name == x.Name)).ForEach(x => x.IsFinished = false);
                    }

                    StateIndex = TableStateList.IndexOf(streetState);

                    UpdatePlayersEquityWin(TableStateList.FirstOrDefault(x => x.CurrentStreet == outStreet)); //update equity win property of all players
                }
            }
        }

        private void FacebookOAuthCommandHandler()
        {
            OpenWindowDialog(new Social.FacebookOAuth());
        }

        private void TwitterOAuthCommandHandler()
        {
            OpenWindowDialog(new Social.TwitterOAuth());
        }

        private void OpenWindowDialog(Window window)
        {
            var owner = ReplayerWindow.ParentOfType<Window>();
            window.Owner = owner ?? System.Windows.Application.Current.MainWindow;
            window.ShowDialog();
        }

        private void HandNoteShow()
        {
            if (CurrentHand == null)
            {
                return;
            }

            var viewModel = new HandNoteViewModel(CurrentHand.GameNumber, CurrentHand.PokersiteId);

            var handNoteView = new HandNoteView(viewModel);

            OpenWindowDialog(handNoteView);

            if (viewModel.HandNoteEntity == null)
            {
                return;
            }

            CurrentHand.Statistic.HandNote = viewModel.HandNoteEntity;

            eventAggregator
                .GetEvent<HandNoteUpdatedEvent>()
                .Publish(new HandNoteUpdatedEventArgs(CurrentHand.GameNumber, viewModel.HandNoteEntity.Note));
        }

        private void ShowSupportForums(object obj)
        {
            try
            {
                Process.Start(BrowserHelper.GetDefaultBrowserPath(), CommonResourceManager.Instance.GetResourceString("SystemSettings_ForumsLink"));
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Couldn't show support forum", e);
            }
        }

        private void TagHand(object obj)
        {
            var tag = EnumHandTag.None;

            if (!Enum.TryParse(obj.ToString(), out tag))
            {
                return;
            }

            try
            {
                var dataService = ServiceLocator.Current.GetInstance<IDataService>();

                var handNote = dataService.GetHandNote(CurrentHand.GameNumber, CurrentHand.PokersiteId);

                if (handNote == null)
                {
                    handNote = new Handnotes
                    {
                        Gamenumber = CurrentHand.GameNumber,
                        PokersiteId = CurrentHand.PokersiteId
                    };
                }

                handNote.HandTag = (int)tag;

                dataService.Store(handNote);

                var storageModel = ServiceLocator.Current.GetInstance<SingletonStorageModel>();

                var statistic = storageModel.FindStatistic(x => x.GameNumber == CurrentHand.GameNumber && x.PokersiteId == CurrentHand.PokersiteId);

                if (statistic == null)
                {
                    return;
                }

                statistic.HandNote = handNote;
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, $"Could not tag hand #{CurrentHand.GameNumber} for {(EnumPokerSites)CurrentHand.PokersiteId} from replayer.", e);
            }
        }

        private void MoveToSessionHand(bool forward)
        {
            if (SessionHandsCollection == null)
            {
                return;
            }

            var currentIndex = SessionHandsCollection.IndexOf(CurrentHand);

            if ((forward && currentIndex >= SessionHandsCollection.Count - 1) ||
                (!forward && currentIndex <= 0))
            {
                return;

            }

            var nextIndex = forward ? ++currentIndex : --currentIndex;

            CurrentHand.IsActive = false;
            CurrentHand = SessionHandsCollection[nextIndex];
            CurrentHand.IsActive = true;
        }

        #endregion

        #region ICommand

        public ICommand NextStepCommand { get; set; }

        public ICommand PlayCommand { get; set; }

        public ICommand StopCommand { get; set; }

        public ICommand PrevStepCommand { get; set; }

        public ICommand ToStartCommand { get; set; }

        public ICommand ToEndCommand { get; set; }

        public ICommand ToStreetCommand { get; set; }

        public ICommand TwitterOAuthCommand { get; set; }

        public ICommand FacebookOAuthCommand { get; set; }

        public ICommand HandNoteCommand { get; set; }

        public ICommand ShowSupportForumsCommand { get; set; }

        public ICommand LoadLayoutCommand { get; set; }

        public ICommand SaveHUDPositionsCommand { get; set; }

        public ICommand TagHandCommand { get; set; }

        public ICommand NextSessionHandCommand { get; set; }

        public ICommand PreviousSessionHandCommand { get; set; }

        public ICommand OpenEquityCalculatorCommand { get; set; }

        public ICommand RotateHUDToRightCommand { get; set; }

        public ICommand RotateHUDToLeftCommand { get; set; }

        #endregion

        #region Properties

        public RadWindow ReplayerWindow { get; set; }

        public InteractionRequest<INotification> NotificationRequest { get; private set; }

        private ReplayerDataModel replayerDataModel { get; set; }

        private ReplayerDataModel currentHand;

        public ReplayerDataModel CurrentHand
        {
            get
            {
                return currentHand;
            }
            set
            {
                if (value == currentHand || value == null)
                {
                    return;
                }

                replayerDataModel = value;

                LoadGame(value);

                SetProperty(ref currentHand, value);

                selectedSessionHand = SessionHandsCollection?.FirstOrDefault(x => x?.GameNumber == value?.GameNumber);
                selectedLastHand = LastHandsCollection?.FirstOrDefault(x => x?.GameNumber == value?.GameNumber);

                RaisePropertyChanged(nameof(SelectedLastHand));
                RaisePropertyChanged(nameof(SelectedSessionHand));
            }
        }

        private ReplayerDataModel selectedLastHand;

        public ReplayerDataModel SelectedLastHand
        {
            get
            {
                return selectedLastHand;
            }
            set
            {
                CurrentHand = value;
            }
        }

        private ReplayerDataModel selectedSessionHand;

        public ReplayerDataModel SelectedSessionHand
        {
            get
            {
                return selectedSessionHand;
            }
            set
            {
                CurrentHand = value;
            }
        }

        private ObservableCollection<ReplayerDataModel> lastHandsCollection;

        public ObservableCollection<ReplayerDataModel> LastHandsCollection
        {
            get
            {
                return lastHandsCollection;
            }
            set
            {
                SetProperty(ref lastHandsCollection, value);
            }
        }

        private ObservableCollection<ReplayerDataModel> sessionHandsCollection;

        public ObservableCollection<ReplayerDataModel> SessionHandsCollection
        {
            get
            {
                return sessionHandsCollection;
            }
            set
            {
                SetProperty(ref sessionHandsCollection, value);
            }
        }

        private HandHistory currentGame;

        internal HandHistory CurrentGame
        {
            get
            {
                return currentGame;
            }
            set
            {
                SetProperty(ref currentGame, value);
            }
        }

        private ObservableCollection<ReplayerPlayerViewModel> playersCollection;

        public ObservableCollection<ReplayerPlayerViewModel> PlayersCollection
        {
            get
            {
                return playersCollection;
            }
            set
            {
                SetProperty(ref playersCollection, value);
            }
        }

        private List<ReplayerTableState> tableStateList;

        internal List<ReplayerTableState> TableStateList
        {
            get
            {
                return tableStateList;
            }
            set
            {
                SetProperty(ref tableStateList, value);
            }
        }

        private decimal currentPotValue;

        public decimal CurrentPotValue
        {
            get
            {
                return currentPotValue;
            }
            set
            {
                SetProperty(ref currentPotValue, value);
            }
        }

        private decimal totalPotValue;

        public decimal TotalPotValue
        {
            get
            {
                return totalPotValue;
            }
            set
            {
                SetProperty(ref totalPotValue, value);
            }
        }

        private int stateIndex;

        public int StateIndex
        {
            get
            {
                return stateIndex;
            }
            set
            {
                if (value < 0 || value >= TableStateList.Count)
                {
                    StopTimer(null);
                    return;
                }

                LoadState(value);

                SetProperty(ref stateIndex, value);
            }
        }

        private IList<ReplayerCardViewModel> communityCards;

        internal IList<ReplayerCardViewModel> CommunityCards
        {
            get
            {
                return communityCards;
            }
            set
            {
                SetProperty(ref communityCards, value);
            }
        }

        private Street currentStreet;

        public Street CurrentStreet
        {
            get
            {
                return currentStreet;
            }
            set
            {
                SetProperty(ref currentStreet, value);
            }
        }

        private ReplayerChipsContainer totalPotChipsContainer;

        internal ReplayerChipsContainer TotalPotChipsContainer
        {
            get
            {
                return totalPotChipsContainer;
            }
            set
            {
                SetProperty(ref totalPotChipsContainer, value);
            }
        }

        private int sliderMax;

        public int SliderMax
        {
            get
            {
                return sliderMax;
            }
            set
            {
                SetProperty(ref sliderMax, value);
            }
        }

        private bool isShowHoleCards;

        internal bool IsShowHoleCards
        {
            get
            {
                return isShowHoleCards;
            }
            set
            {
                SetProperty(ref isShowHoleCards, value);
            }
        }

        private string activePlayerName;

        internal string ActivePlayerName
        {
            get
            {
                return activePlayerName;
            }
            set
            {
                SetProperty(ref activePlayerName, value);
            }
        }

        private ObservableCollection<string> layoutsCollection;

        public ObservableCollection<string> LayoutsCollection
        {
            get
            {
                return layoutsCollection;
            }
            set
            {
                SetProperty(ref layoutsCollection, value);
            }
        }

        private string layoutName;

        public string LayoutName
        {
            get
            {
                return layoutName;
            }
            set
            {
                layoutName = value;
                RaisePropertyChanged();
            }
        }

        private bool isLoadingHUD;

        public bool IsLoadingHUD
        {
            get
            {
                return isLoadingHUD;
            }
            set
            {
                SetProperty(ref isLoadingHUD, value);
            }
        }

        private decimal bbFilter;

        public decimal BBFilter
        {
            get
            {
                return bbFilter;
            }
            set
            {
                SetProperty(ref bbFilter, value);

                var settings = settingsService.GetSettings();
                settings.GeneralSettings.ReplayerBBFilter = bbFilter;
                settingsService.SaveSettings(settings);

                if (CurrentHand == null || CurrentHand.Statistic == null ||
                    string.IsNullOrWhiteSpace(CurrentHand.Statistic.PlayerName) ||
                    !CurrentGame.Players.Any(x => x.PlayerName == CurrentHand.Statistic.PlayerName && !x.IsSittingOut))
                {
                    return;
                }

                var statistics = CurrentHand.Statistic.IsTourney ?
                    storageModel.GetFilteredTournamentPlayerStatistic() :
                    storageModel.GetFilteredCashPlayerStatistic();

                SessionHandsCollection = new ObservableCollection<ReplayerDataModel>(ReplayerHelpers.CreateSessionHandsList(statistics, CurrentHand.Statistic));
            }
        }

        #endregion
    }
}