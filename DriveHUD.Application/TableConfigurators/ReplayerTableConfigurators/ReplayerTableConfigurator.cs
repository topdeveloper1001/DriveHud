//-----------------------------------------------------------------------
// <copyright file="ReplayerTableConfigurator.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.Controls;
using DriveHUD.Application.TableConfigurators.PositionProviders;
using DriveHUD.Application.ValueConverters;
using DriveHUD.Application.ViewModels.Hud;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Application.ViewModels.Popups;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.Application.Views.Popups;
using DriveHUD.Application.Views.Replayer;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Reflection;
using DriveHUD.Common.Wpf.Converters;
using DriveHUD.Entities;
using DriveHUD.HUD.Service;
using HandHistories.Objects.Cards;
using HandHistories.Objects.GameDescription;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Data;
using Model.Enums;
using Model.Interfaces;
using Model.Stats;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Telerik.Windows.Controls;

namespace DriveHUD.Application.TableConfigurators
{
    public class ReplayerTableConfigurator : IReplayerTableConfigurator
    {
        #region Fields

        public const int PLAYER_WIDTH = 150;
        public const int PLAYER_HEIGHT = 60;
        public const int BUTTON_HEIGHT = 31;
        public const int BUTTON_WIDTH = 31;

        public const int CARD_WIDTH = 84;
        public const int CARD_HEIGHT = 112;

        private EnumReplayerTableType CurrentCapacity;

        private const string BackgroundImage = "/DriveHUD.Common.Resources;component/images/ReplayerTable.png";
        private const string DealerImage = "/DriveHUD.Common.Resources;component/images/D.png";

        private readonly Point DefaultTablePosition = new Point(120, 60);
        private readonly Point TotalPotChipsPosition = new Point(560, 100);

        private readonly Dictionary<int, Tuple<double, double>> predefinedTableSizes = new Dictionary<int, Tuple<double, double>>
        {
            { 6, new Tuple<double, double>(420, 820) },
            { 9, new Tuple<double, double>(420, 820) },
            { 10, new Tuple<double, double>(420, 820) }
        };

        private readonly Dictionary<int, double[,]> predefinedChipsPositions = new Dictionary<int, double[,]>
        {
            { 6, new double[,] { { 240, 260 }, { 360, 120 }, { 680, 120 }, { 820, 260 }, { 680, 320 }, { 370, 320 } } },
            { 9, new double[,] { { 240, 270 }, { 250, 160 }, { 390, 150 }, { 680, 160 }, { 790, 190 }, { 790, 270 }, { 700, 320 }, { 520, 320 }, { 310, 320 } } },
            { 10, new double[,] { { 280, 150 }, { 370, 160 }, { 645, 160 }, { 760, 150 }, { 780, 230 }, { 760, 320 }, { 620, 320 }, { 410, 320 }, { 250, 320 }, { 240, 250 } } },
        };

        private RadDiagramShape table;

        private readonly IHudLayoutsService hudLayoutsService;
        private readonly IDataService dataService;
        private readonly IPlayerStatisticRepository playerStatisticRepository;
        private readonly SingletonStorageModel storageModel;

        #endregion

        public ReplayerTableConfigurator()
        {
            hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
            storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();
            playerStatisticRepository = ServiceLocator.Current.GetInstance<IPlayerStatisticRepository>();
        }

        public void ConfigureTable(RadDiagram diagram, HudDragCanvas dgCanvas, ReplayerViewModel viewModel)
        {
            try
            {
                if (viewModel == null || viewModel.CurrentGame == null)
                {
                    LogProvider.Log.Info("Cannot find handHistory");
                    return;
                }

                CurrentCapacity = GetTableSize(viewModel);

                CreateTable(diagram);
                CreatePlayersLayout(diagram, dgCanvas, viewModel);
                CreateCurrentPotValueLabel(diagram, viewModel);
                CreateTotalPotValueLabel(diagram, viewModel);
                PlaceTableCardPanels(diagram, viewModel);
                InitializeHUD(dgCanvas, viewModel);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "ReplayerTable: Cannot configure table", e);
            }
        }

        private void CreateTable(RadDiagram diagram)
        {
            diagram.Clear();

            var seats = (int)CurrentCapacity;

            table = new RadDiagramShape()
            {
                Name = "Table",
                Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), BackgroundImage))),
                Height = predefinedTableSizes[seats].Item1,
                Width = predefinedTableSizes[seats].Item2,
                StrokeThickness = 0,
                IsEnabled = false
            };

            table.X = DefaultTablePosition.X;
            table.Y = DefaultTablePosition.Y;

            diagram.AddShape(table);
        }

        private const EnumPokerSites ReplayerPokerSite = EnumPokerSites.DriveHUDReplayer;

        private void CreatePlayersLayout(RadDiagram diagram, HudDragCanvas dgCanvas, ReplayerViewModel viewModel)
        {
            var seats = (int)CurrentCapacity;

            var positionProvider = ServiceLocator.Current.GetInstance<IPositionProvider>(ReplayerPokerSite.ToString());

            var labelPositions = positionProvider.Positions[seats];
            var hasHoleCards = viewModel.CurrentGame.Players.Any(x => x.hasHoleCards);
            var cardsCount = hasHoleCards ? viewModel.CurrentGame.Players.Where(x => x.hasHoleCards).Max(x => x.HoleCards.Count) : 2;

            for (var i = 0; i < seats; i++)
            {
                var player = viewModel.PlayersCollection[i];
                var label = CreatePlayerLabel(player);

                label.X = labelPositions[i, 0];
                label.Y = labelPositions[i, 1];

                PlaceCardLabels(diagram, label, cardsCount);

                player.ChipsContainer.ChipsShape.X = predefinedChipsPositions[seats][i, 0];
                player.ChipsContainer.ChipsShape.Y = predefinedChipsPositions[seats][i, 1];

                AddPotPlayerLabel(diagram, viewModel.PlayersCollection[i], predefinedChipsPositions[seats][i, 0], predefinedChipsPositions[seats][i, 1]);

                diagram.AddShape(label);
                diagram.AddShape(player.ChipsContainer.ChipsShape);
            }
        }

        private ConcurrentDictionary<string, HudIndicators> playerIndicators = new ConcurrentDictionary<string, HudIndicators>();
        private HashSet<Stat> heatMapStats;

        private async void InitializeHUD(HudDragCanvas dgCanvas, ReplayerViewModel viewModel)
        {
            var gameNumber = viewModel.CurrentHand.GameNumber;
            var pokerSite = viewModel.CurrentHand.PokersiteId;

            ClearHUD(dgCanvas);

            viewModel.IsLoadingHUD = true;

            var seats = (int)CurrentCapacity;

            var tableType = (EnumTableType)viewModel.CurrentGame.GameDescription.SeatType.MaxPlayers;

            // need to add DriveHUDReplayer to sites, add provider, panel service etc.
            var activeLayout = hudLayoutsService.GetActiveLayout(ReplayerPokerSite,
                tableType,
                EnumGameType.CashHoldem);

            viewModel.LayoutName = activeLayout.Name;
            viewModel.LayoutsCollection = new ObservableCollection<string>(hudLayoutsService.GetAvailableLayouts(tableType));
            viewModel.LoadLayoutCommand = new RelayCommand(obj =>
            {
                try
                {
                    var layoutName = obj?.ToString();

                    if (string.IsNullOrWhiteSpace(layoutName))
                    {
                        return;
                    }

                    viewModel.LayoutName = layoutName;

                    var layout = hudLayoutsService.GetLayout(layoutName);

                    var layoutHeatMapStats = new HashSet<Stat>(layout.GetHeatMapStats());

                    // there are no loaded heat map stats
                    if (layoutHeatMapStats.Count > 0 &&
                        layoutHeatMapStats.Any(x => !heatMapStats.Contains(x)))
                    {
                        viewModel.IsLoadingHUD = true;
                        playerIndicators.Clear();
                        LoadIndicators(seats, viewModel, layoutHeatMapStats);
                        heatMapStats = layoutHeatMapStats;
                        viewModel.IsLoadingHUD = false;
                    }

                    ClearHUD(dgCanvas);
                    LoadHUD(dgCanvas, viewModel, layout);

                    hudLayoutsService.SetActiveLayout(layout, ReplayerPokerSite, EnumGameType.CashHoldem, tableType);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, "Failed to load layout", e);
                }
            });

            viewModel.SaveHUDPositionsCommand = new RelayCommand(obj => SaveHUDPositions(dgCanvas, viewModel));
            viewModel.RotateHUDToLeftCommand = new RelayCommand(() => RotateHUD(false, tableType, dgCanvas));
            viewModel.RotateHUDToRightCommand = new RelayCommand(() => RotateHUD(true, tableType, dgCanvas));

            heatMapStats = new HashSet<Stat>(activeLayout.GetHeatMapStats());

            await Task.Run(() => LoadIndicators(seats, viewModel, heatMapStats));

            if (gameNumber != viewModel.CurrentHand.GameNumber ||
                pokerSite != viewModel.CurrentHand.PokersiteId)
            {
                return;
            }

            LoadHUD(dgCanvas, viewModel, activeLayout);

            viewModel.IsLoadingHUD = false;
        }

        private void LoadIndicators(int seats, ReplayerViewModel viewModel, IEnumerable<Stat> heatMapStats)
        {
            var tasksToLoad = new List<Task>();

            var selectedPlayer = storageModel.PlayerSelectedItem;

            var players = viewModel.PlayersCollection.Select(x => x.Name).ToArray();

            for (var i = 0; i < seats; i++)
            {
                var player = players[i];

                if (playerIndicators.ContainsKey(player))
                {
                    continue;
                }

                var playerData = new HudIndicators(heatMapStats);

                // read data from statistic
                var taskToReadPlayerStats = Task.Run(() =>
                {
                    if (selectedPlayer != null &&
                        player == selectedPlayer.Name &&
                        (short?)selectedPlayer.PokerSite == viewModel.CurrentHand.PokersiteId)
                    {
                        storageModel
                            .GetStatisticCollection()
                            .Where(stat => (stat.PokersiteId == (short)viewModel.CurrentGame.GameDescription.Site) &&
                                stat.IsTourney == viewModel.CurrentGame.GameDescription.IsTournament &&
                                GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, viewModel.CurrentGame.GameDescription.GameType))
                            .ForEach(stat => playerData.AddStatistic(stat));

                        playerIndicators.AddOrUpdate(player, playerData, (key, old) => playerData);
                        return;
                    }

                    playerStatisticRepository
                       .GetPlayerStatistic(player, (short)viewModel.CurrentGame.GameDescription.Site)
                       .Where(stat => (stat.PokersiteId == (short)viewModel.CurrentGame.GameDescription.Site) &&
                           stat.IsTourney == viewModel.CurrentGame.GameDescription.IsTournament &&
                           GameTypeUtils.CompareGameType((GameType)stat.PokergametypeId, viewModel.CurrentGame.GameDescription.GameType))
                       .ForEach(stat => playerData.AddStatistic(stat));

                    playerIndicators.AddOrUpdate(player, playerData, (key, old) => playerData);
                });

                tasksToLoad.Add(taskToReadPlayerStats);
            }

            Task.WhenAll(tasksToLoad).Wait();
        }

        private void ClearHUD(HudDragCanvas dgCanvas)
        {
            foreach (var panel in dgCanvas.Children.OfType<FrameworkElement>().ToList())
            {
                dgCanvas.Children.Remove(panel);
            }

            dgCanvas.UpdateLayout();
            EmptySeats.Clear();
        }

        private readonly List<HudElementViewModel> EmptySeats = new List<HudElementViewModel>();

        private void LoadHUD(HudDragCanvas dgCanvas, ReplayerViewModel viewModel, HudLayoutInfoV2 activeLayout)
        {
            var seats = (int)CurrentCapacity;

            var hudPanelService = ServiceLocator.Current.GetInstance<IHudPanelService>(ReplayerPokerSite.ToString());

            var nonToolLayoutStats = activeLayout
                .HudPlayerTypes
                .SelectMany(x => x.Stats)
                .Select(x => x.Stat)
                .Concat(activeLayout
                    .HudBumperStickerTypes
                    .SelectMany(x => x.Stats)
                    .Select(x => x.Stat))
                .Concat(new[] { Stat.TotalHands })
                .Distinct()
                .ToArray();

            var hudElements = new List<HudElementViewModel>();

            var hudElementCreator = ServiceLocator.Current.GetInstance<IHudElementViewModelCreator>();

            var hudElementCreationInfo = new HudElementViewModelCreationInfo
            {
                GameType = EnumGameType.CashHoldem,
                HudLayoutInfo = activeLayout,
                PokerSite = ReplayerPokerSite
            };

            for (var i = 0; i < seats; i++)
            {
                var replayerPlayer = viewModel.PlayersCollection[i];

                hudElementCreationInfo.SeatNumber = i + 1;

                var hudElement = hudElementCreator.Create(hudElementCreationInfo);

                if (string.IsNullOrEmpty(replayerPlayer.Name))
                {
                    if (hudElement != null) EmptySeats.Add(hudElement);
                    continue;
                }

                var player = dataService.GetPlayer(replayerPlayer.Name, viewModel.CurrentHand.PokersiteId);

                if (player == null)
                {
                    if (hudElement != null) EmptySeats.Add(hudElement);
                    continue;
                }

                if (hudElement == null ||
                    !playerIndicators.TryGetValue(replayerPlayer.Name, out HudIndicators playerData))
                {
                    continue;
                }

                hudElement.PlayerId = player.PlayerId;
                hudElement.PlayerName = replayerPlayer.Name;
                hudElement.TotalHands = playerData.TotalHands;

                var playerNotes = dataService.GetPlayerNotes(player.PlayerId);
                hudElement.NoteToolTip = NoteBuilder.BuildNote(playerNotes);
                hudElement.IsXRayNoteVisible = playerNotes.Any(x => x.IsAutoNote);

                var graphTools = hudElement.Tools.OfType<HudGraphViewModel>().ToArray();

                foreach (var graphTool in graphTools)
                {
                    graphTool.StatSessionCollection = new ReactiveList<decimal>();
                }

                var heatMapTools = hudElement.Tools.OfType<HudHeatMapViewModel>()
                    .Concat(hudElement.Tools.OfType<HudGaugeIndicatorViewModel>()
                        .SelectMany(x => x.GroupedStats)
                        .SelectMany(x => x.Stats)
                        .Where(x => x.HeatMapViewModel != null)
                        .Select(x => x.HeatMapViewModel))
                    .ToArray();

                heatMapTools.ForEach(x =>
                {
                    var heatMapKey = playerData.HeatMaps.Keys
                        .ToArray()
                        .FirstOrDefault(p => p.Stat == x.BaseStat.Stat);

                    if (heatMapKey != null)
                    {
                        x.HeatMap = playerData.HeatMaps[heatMapKey];
                    }
                });

                var gaugeIndicatorTools = hudElement.Tools.OfType<HudGaugeIndicatorViewModel>().ToArray();

                hudElement.SessionMoneyWonCollection = new ObservableCollection<decimal>();

                var activeLayoutHudStats = hudElement.ToolsStatInfoCollection
                    .Concat(heatMapTools.Select(x => x.BaseStat))
                    .Concat(gaugeIndicatorTools.Select(x => x.BaseStat))
                    .ToList();

                var extraStats = (from nonToolLayoutStat in nonToolLayoutStats
                                  join activateLayoutHudStat in activeLayoutHudStats on nonToolLayoutStat equals activateLayoutHudStat.Stat into grouped
                                  from extraStat in grouped.DefaultIfEmpty()
                                  where extraStat == null
                                  select new StatInfo
                                  {
                                      Stat = nonToolLayoutStat
                                  }).ToArray();

                activeLayoutHudStats.AddRange(extraStats);

                StatsProvider.UpdateStats(activeLayoutHudStats);

                foreach (var statInfo in activeLayoutHudStats)
                {
                    var propertyName = StatsProvider.GetStatProperyName(statInfo.Stat);

                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        if (playerData.TotalHands < statInfo.MinSample)
                        {
                            statInfo.IsNotVisible = true;
                        }

                        statInfo.AssignStatInfoValues(playerData, propertyName);
                    }
                    else if (!(statInfo is StatInfoBreak) && statInfo.Stat != Stat.PlayerInfoIcon)
                    {
                        continue;
                    }
                }

                hudElement.StatInfoCollection = activeLayoutHudStats;

                var isNoteIconSet = false;

                foreach (var toolViewModel in hudElement.Tools.Where(x => x is IHudNonPopupToolViewModel).ToArray())
                {
                    if (!isNoteIconSet && toolViewModel is HudPlainStatBoxViewModel && !(toolViewModel is HudFourStatsBoxViewModel))
                    {
                        (toolViewModel as HudPlainStatBoxViewModel).IsNoteIconEnabled = true;
                        isNoteIconSet = true;
                    }

                    var panel = hudPanelService.Create(toolViewModel);

                    if (panel != null)
                    {
                        dgCanvas.Children.Add(panel);

                        Canvas.SetLeft(panel, toolViewModel.Position.X);
                        Canvas.SetTop(panel, toolViewModel.Position.Y);
                    }
                }

                hudElements.Add(hudElement);
            }

            hudLayoutsService.SetPlayerTypeIcon(hudElements, activeLayout);
        }

        private void SaveHUDPositions(HudDragCanvas dgCanvas, ReplayerViewModel viewModel)
        {
            var layout = hudLayoutsService.GetLayout(viewModel.LayoutName);

            if (layout == null)
            {
                LogProvider.Log.Warn($"Failed to save HUD positions. Could not find layout '{viewModel.LayoutName}'");
                return;
            }

            try
            {
                var hudLayoutContract = new HudLayoutContract
                {
                    LayoutName = viewModel.LayoutName,
                    GameType = EnumGameType.CashHoldem,
                    PokerSite = ReplayerPokerSite,
                    TableType = layout.TableType,
                    HudPositions = new List<HudPositionContract>()
                };

                // clone is needed
                var toolViewModels = dgCanvas.Children.OfType<FrameworkElement>()
                    .Where(x => x != null && (x.DataContext is IHudNonPopupToolViewModel))
                    .Select(x => x.DataContext as HudBaseToolViewModel)
                    .Concat(EmptySeats
                        .SelectMany(x => x.Tools)
                        .OfType<IHudNonPopupToolViewModel>()
                        .Cast<HudBaseToolViewModel>());

                foreach (var toolViewModel in toolViewModels)
                {
                    var seatNumber = toolViewModel.Parent != null ? toolViewModel.Parent.Seat : 1;

                    var xPos = toolViewModel.OffsetX ?? toolViewModel.Position.X;
                    var yPos = toolViewModel.OffsetY ?? toolViewModel.Position.Y;

                    hudLayoutContract.HudPositions.Add(new HudPositionContract
                    {
                        Id = toolViewModel.Id,
                        Position = new Point(xPos, yPos),
                        SeatNumber = seatNumber
                    });
                }

                hudLayoutsService.Save(hudLayoutContract);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to save HUD positions in replayer.", e);
            }
        }

        private void RotateHUD(bool clockwise, EnumTableType tableType, HudDragCanvas dgCanvas)
        {
            try
            {
                var tableSize = (int)tableType;

                var toolsBySeats = dgCanvas.Children.OfType<FrameworkElement>()
                       .Where(x => x != null && (x.DataContext is IHudNonPopupToolViewModel))
                       .Select(x => new
                       {
                           Tool = x.DataContext as HudBaseToolViewModel,
                           Panel = x
                       })
                       .Concat(EmptySeats
                           .SelectMany(x => x.Tools)
                           .OfType<IHudNonPopupToolViewModel>()
                           .Cast<HudBaseToolViewModel>()
                           .Select(x => new
                           {
                               Tool = x as HudBaseToolViewModel,
                               Panel = (FrameworkElement)null
                           }))
                       .GroupBy(x => x.Tool.Parent.Seat)
                       .ToDictionary(x => x.Key, x => x.ToArray());

                var toolsById = toolsBySeats.Values
                    .SelectMany(x => x)
                    .GroupBy(x => x.Tool.Id, x => new
                    {
                        OffsetX = x.Tool.OffsetX ?? x.Tool.Position.X,
                        OffsetY = x.Tool.OffsetY ?? x.Tool.Position.Y,
                        x.Tool.Parent.Seat
                    })
                    .ToDictionary(x => x.Key, x => x.GroupBy(p => p.Seat).ToDictionary(p => p.Key, p => p.FirstOrDefault()));

                for (var seat = 1; seat <= tableSize; seat++)
                {
                    var newSeat = clockwise ? seat + 1 : seat - 1;

                    if (newSeat > tableSize)
                    {
                        newSeat = 1;
                    }
                    else if (newSeat < 1)
                    {
                        newSeat = tableSize;
                    }

                    var nonPopupTools = toolsBySeats[seat];

                    foreach (var nonPopupTool in nonPopupTools)
                    {
                        if (!toolsById.ContainsKey(nonPopupTool.Tool.Id) ||
                            !toolsById[nonPopupTool.Tool.Id].ContainsKey(newSeat))
                        {
                            continue;
                        }

                        var newOffsets = toolsById[nonPopupTool.Tool.Id][newSeat];

                        if (newOffsets == null)
                        {
                            continue;
                        }

                        nonPopupTool.Tool.OffsetX = newOffsets.OffsetX;
                        nonPopupTool.Tool.OffsetY = newOffsets.OffsetY;

                        if (nonPopupTool.Panel == null)
                        {
                            continue;
                        }

                        Canvas.SetLeft(nonPopupTool.Panel, newOffsets.OffsetX);
                        Canvas.SetTop(nonPopupTool.Panel, newOffsets.OffsetY);
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Failed to rotate HUD positions in replayer.", e);
            }
        }

        private void AddPotPlayerLabel(RadDiagram diagram, ReplayerPlayerViewModel player, double x, double y)
        {
            try
            {
                System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
                Binding myBinding = new Binding(ReflectionHelper.GetPath<ReplayerPlayerViewModel>(o => o.ActiveAmount)) { Source = player, Mode = BindingMode.TwoWay, Converter = new DecimalToPotConverter(), ConverterParameter = "{0:C2}" };
                lbl.SetBinding(ContentControl.ContentProperty, myBinding);
                lbl.Foreground = Brushes.White;

                lbl.Margin = new Thickness(x - 15, y + 20, 100, 100);

                diagram.AddShape(lbl);
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, e);
            }
        }

        private RadDiagramShape CreatePlayerLabel(ReplayerPlayerViewModel p)
        {
            var label = new RadDiagramShape()
            {
                DataContext = p,
                Tag = p,
                Height = PLAYER_HEIGHT,
                Width = PLAYER_WIDTH,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = false,
                IsHitTestVisible = false,
                FontSize = 13
            };

            BindingOperations.ClearBinding(label, Control.BackgroundProperty);

            Binding myBinding = new Binding(nameof(ReplayerPlayerViewModel.IsFinished)) { Source = p, Mode = BindingMode.TwoWay, Converter = new ReplayerBrushPlayerConverter() };
            label.SetBinding(Control.BackgroundProperty, myBinding);

            return label;
        }

        private ReplayerHudPanel CreateHudPanel(ReplayerViewModel viewModel, int zeroBasedSeatNumber)
        {
            var player = viewModel.PlayersCollection[zeroBasedSeatNumber];

            player.Parent = new HudElementViewModel { Seat = zeroBasedSeatNumber };

            var panel = new ReplayerHudPanel
            {
                DataContext = player
            };

            panel.Height = double.NaN;
            panel.Width = double.NaN;

            var playerNotes = dataService.GetPlayerNotes(player.Name, viewModel.CurrentHand.PokersiteId);
            player.NoteToolTip = NoteBuilder.BuildNote(playerNotes);
            player.Parent.IsXRayNoteVisible = playerNotes.Any(x => x.IsAutoNote);

            var contextMenu = CreateContextMenu(viewModel.CurrentHand.PokersiteId, player.Name, player);
            contextMenu.EventName = "MouseRightButtonUp";

            RadContextMenu.SetContextMenu(panel, contextMenu);

            return panel;
        }

        private RadContextMenu CreateContextMenu(short pokerSiteId, string playerName, ReplayerPlayerViewModel datacontext)
        {
            RadContextMenu radMenu = new RadContextMenu();

            var item = new RadMenuItem();

            var binding = new Binding(nameof(ReplayerPlayerViewModel.NoteMenuItemText)) { Source = datacontext, Mode = BindingMode.OneWay };
            item.SetBinding(HeaderedItemsControl.HeaderProperty, binding);

            item.Click += (s, e) =>
            {
                PlayerNoteViewModel viewModel = new PlayerNoteViewModel(pokerSiteId, playerName);

                var frm = new PlayerNoteView(viewModel)
                {
                    Owner = System.Windows.Application.Current.MainWindow
                };

                frm.ShowDialog();

                var clickedItem = s as FrameworkElement;

                if (clickedItem == null || !(clickedItem.DataContext is ReplayerPlayerViewModel))
                {
                    return;
                }

                var hudElement = clickedItem.DataContext as ReplayerPlayerViewModel;
                hudElement.NoteToolTip = viewModel.HasNotes ? viewModel.Note : string.Empty;
            };

            radMenu.Items.Add(item);

            return radMenu;
        }

        private void LoadPlayerHudStats(ReplayerPlayerViewModel replayerPlayer, ReplayerViewModel replayerViewModel, IList<StatInfo> statInfoCollection)
        {
            replayerPlayer.StatInfoCollection.Clear();

            HudLightIndicators hudIndicators;

            if (storageModel.PlayerSelectedItem.Name == replayerPlayer.Name &&
                (short?)storageModel.PlayerSelectedItem.PokerSite == replayerViewModel.CurrentHand.PokersiteId)
            {
                hudIndicators = new HudLightIndicators(storageModel.GetStatisticCollection());
            }
            else
            {
                hudIndicators = new HudLightIndicators();

                playerStatisticRepository
                    .GetPlayerStatistic(replayerPlayer.Name, replayerViewModel.CurrentHand.PokersiteId)
                    .ForEach(stat => hudIndicators.AddStatistic(stat));
            }

            if (hudIndicators != null)
            {
                var statList = new List<StatInfo>();

                var counter = 0;

                foreach (var selectedStatInfo in statInfoCollection)
                {
                    if (selectedStatInfo is StatInfoBreak)
                    {
                        continue;
                    }

                    var statInfo = selectedStatInfo.Clone();

                    var propertyName = StatsProvider.GetStatProperyName(statInfo.Stat);

                    if (!string.IsNullOrWhiteSpace(propertyName))
                    {
                        statInfo.AssignStatInfoValues(hudIndicators, propertyName);
                    }

                    replayerPlayer.StatInfoCollection.Add(statInfo);

                    if ((counter + 1) % 4 == 0)
                    {
                        replayerPlayer.StatInfoCollection.Add(new StatInfoBreak());
                    }

                    counter++;
                }
            }
        }

        private void PlaceCardLabels(RadDiagram diagram, RadDiagramShape playerLabel, int cardsCount = 2)
        {
            var player = playerLabel.DataContext as ReplayerPlayerViewModel;

            if (player == null)
            {
                throw new ArgumentNullException("playerLabel", "Cannot place card labels for player because player is null");
            }

            Binding myBinding = new Binding(nameof(ReplayerPlayerViewModel.IsFinished)) { Source = player, Mode = BindingMode.TwoWay, Converter = new BoolToVisibilityConverter(), ConverterParameter = "Inverse" };

            for (int i = 0; i < cardsCount; i++)
            {
                var card = CreateCardLabel(diagram, player.Cards[i]);
                card.SetBinding(UIElement.VisibilityProperty, myBinding);

                if (cardsCount == 4)
                {
                    double offset = card.Width / 4;
                    var start = playerLabel.X - offset + 2;
                    var width = playerLabel.Width + offset;
                    card.X = start + i * width / 5;
                }
                else if (cardsCount == 2)
                {
                    card.X = playerLabel.X + 5 + i * (playerLabel.Width - 10 - card.Width);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("cardsCount", "Supported cardsCount values are 2 and 4");
                }

                card.Y = playerLabel.Y - 60;

                diagram.AddShape(card);
            }

            if (player.IsDealer)
            {
                var button = CreateDealerLabel(diagram, player);
                button.X = playerLabel.X - BUTTON_WIDTH - 5;
                button.Y = playerLabel.Y + playerLabel.Height / 2 - BUTTON_HEIGHT / 2;

                diagram.AddShape(button);
            }
        }

        private RadDiagramShape CreateCardLabel(RadDiagram diagram, ReplayerCardViewModel card)
        {
            var label = new RadDiagramShape()
            {
                Height = CARD_HEIGHT,
                Width = CARD_WIDTH,
                MaxHeight = CARD_HEIGHT,
                MaxWidth = CARD_WIDTH,
                StrokeThickness = 0,
                BorderThickness = new Thickness(0),
                IsEnabled = false,
                IsHitTestVisible = false,
                DataContext = card
            };

            try
            {
                BindingOperations.ClearBinding(label, UIElement.VisibilityProperty);
                BindingOperations.ClearBinding(label, Control.BackgroundProperty);

                Binding cardBinding = new Binding(nameof(ReplayerCardViewModel.CardId)) { Source = card, Mode = BindingMode.TwoWay, Converter = new IntToCardConverter(), ConverterParameter = label };
                label.SetBinding(Control.BackgroundProperty, cardBinding);
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(ex);
            }

            return label;
        }

        private RadDiagramShape CreateDealerLabel(RadDiagram diagram, ReplayerPlayerViewModel player)
        {
            var button = new RadDiagramShape()
            {
                Background = new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), DealerImage))),
                Height = BUTTON_HEIGHT,
                Width = BUTTON_WIDTH,
                StrokeThickness = 0,
                IsEnabled = false,
            };

            Binding myBinding = new Binding(nameof(ReplayerPlayerViewModel.IsDealer)) { Source = player, Mode = BindingMode.TwoWay, Converter = new BoolToVisibilityConverter() };
            button.SetBinding(UIElement.VisibilityProperty, myBinding);

            return button;
        }

        private void CreateCurrentPotValueLabel(RadDiagram diagram, ReplayerViewModel viewModel)
        {
            System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
            Binding myBinding = new Binding(nameof(ReplayerViewModel.CurrentPotValue)) { Source = viewModel, Mode = BindingMode.TwoWay, Converter = new DecimalToPotConverter(), ConverterParameter = "Pot {0:C2}" };
            lbl.SetBinding(ContentControl.ContentProperty, myBinding);
            lbl.Background = Brushes.LightGray;
            lbl.Foreground = Brushes.Black;
            lbl.BorderBrush = Brushes.Black;
            lbl.BorderThickness = new Thickness(1);
            lbl.Padding = new Thickness(3, 3, 3, 3);
            lbl.Margin = new Thickness(480, 315, 100, 100);
            diagram.AddShape(lbl);
        }

        private void CreateTotalPotValueLabel(RadDiagram diagram, ReplayerViewModel viewModel)
        {
            System.Windows.Controls.Label lbl = new System.Windows.Controls.Label();
            Binding myBinding = new Binding(nameof(ReplayerViewModel.TotalPotValue)) { Source = viewModel, Mode = BindingMode.TwoWay, Converter = new DecimalToPotConverter(), ConverterParameter = "{0:C2}" };
            lbl.SetBinding(ContentControl.ContentProperty, myBinding);
            lbl.Foreground = Brushes.White;
            lbl.Margin = new Thickness(480, 100, 100, 100);
            diagram.AddShape(lbl);

            viewModel.TotalPotChipsContainer.ChipsShape.X = TotalPotChipsPosition.X;
            viewModel.TotalPotChipsContainer.ChipsShape.Y = TotalPotChipsPosition.Y;

            diagram.AddShape(viewModel.TotalPotChipsContainer.ChipsShape);
        }

        private void PlaceTableCardPanels(RadDiagram diagram, ReplayerViewModel viewModel)
        {
            double y = table.Y + table.Height / 2 - CARD_HEIGHT / 2.0 - 20;
            double x = table.X + table.Width / 2 - CARD_WIDTH / 2.0 - 2 * CARD_WIDTH - 10;

            for (int i = 0; i < viewModel.CommunityCards.Count; i++)
            {
                var card = CreateCardLabel(diagram, viewModel.CommunityCards[i]);

                Street cardStreet = i < 3 ? Street.Flop : i < 4 ? Street.Turn : Street.River;
                Binding myBinding = new Binding(ReflectionHelper.GetPath<ViewModels.Replayer.ReplayerViewModel>(o => o.CurrentStreet)) { Source = viewModel, Mode = BindingMode.TwoWay, Converter = new StreetToVisibilityConverter(), ConverterParameter = cardStreet };
                card.SetBinding(UIElement.VisibilityProperty, myBinding);

                card.X = x;
                card.Y = y;

                diagram.AddShape(card);

                x = card.X + CARD_WIDTH + 5;
            }
        }

        private EnumReplayerTableType GetTableSize(ReplayerViewModel viewModel)
        {
            SeatType seatType = SeatType.FromMaxPlayers(viewModel.CurrentGame.GameDescription.SeatType.MaxPlayers, true);

            if (seatType.MaxPlayers <= 6)
            {
                return EnumReplayerTableType.Six;
            }

            if (seatType.MaxPlayers <= 9)
            {
                return EnumReplayerTableType.Nine;
            }

            return EnumReplayerTableType.Ten;
        }

        private IList<StatInfo> GetHudStats(EnumPokerSites pokerSite, EnumGameType gameType, EnumTableType tableType)
        {
            var activeLayout = hudLayoutsService.GetActiveLayout(pokerSite, tableType, gameType);

            if (activeLayout == null)
            {
                LogProvider.Log.Error("Could not find active layout");
                return null;
            }

            return activeLayout.LayoutTools.OfType<HudLayoutPlainBoxTool>().SelectMany(x => x.Stats).ToArray();
        }

        private EnumPokerSites GetPokerSite(EnumPokerSites site)
        {
            switch (site)
            {
                case EnumPokerSites.Unknown:
                case EnumPokerSites.Ignition:
                case EnumPokerSites.IPoker:
                case EnumPokerSites.Bovada:
                    return EnumPokerSites.Ignition;
                case EnumPokerSites.Bodog:
                case EnumPokerSites.BetOnline:
                    return EnumPokerSites.BetOnline;
                case EnumPokerSites.SportsBetting:
                case EnumPokerSites.PokerStars:
                case EnumPokerSites.Poker888:
                case EnumPokerSites.TigerGaming:
                case EnumPokerSites.PartyPoker:
                default:
                    return site;
            }
        }

        private EnumGameType GetGameType(GameDescriptor description, string playerName)
        {
            bool isCash = !description.IsTournament;
            bool isMtt = false;
            if (description.IsTournament)
            {
                var tournament = dataService.GetTournament(description.Tournament.TournamentId, playerName, (short)description.Site);
                TournamentsTags tag = TournamentsTags.STT;
                if (tournament != null && Enum.TryParse(tournament.Tourneytagscsv, out tag))
                {
                    isMtt = tag == TournamentsTags.MTT;
                }
            }

            switch (description.GameType)
            {
                case GameType.NoLimitHoldem:
                case GameType.FixedLimitHoldem:
                case GameType.PotLimitHoldem:
                case GameType.CapNoLimitHoldem:
                case GameType.SpreadLimitHoldem:
                    return isCash ? EnumGameType.CashHoldem :
                           isMtt ? EnumGameType.MTTHoldem : EnumGameType.SNGHoldem;

                case GameType.PotLimitOmaha:
                case GameType.PotLimitOmahaHiLo:
                case GameType.CapPotLimitOmaha:
                case GameType.FixedLimitOmahaHiLo:
                case GameType.FixedLimitOmaha:
                case GameType.NoLimitOmahaHiLo:
                case GameType.NoLimitOmaha:
                case GameType.FiveCardPotLimitOmaha:
                case GameType.FiveCardPotLimitOmahaHiLo:
                    return isCash ? EnumGameType.CashOmaha :
                           isMtt ? EnumGameType.MTTOmaha : EnumGameType.SNGOmaha;
                default:
                    break;
            }
            return EnumGameType.CashHoldem;
        }
    }
}
