using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriveHUD.Application.ViewModels.Replayer;
using DriveHUD.ViewModels;
using Telerik.Windows.Controls;
using Model.Enums;
using DriveHUD.Application.ViewModels;
using HandHistories.Objects.GameDescription;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using DriveHUD.Common.Log;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using DriveHUD.Application.ValueConverters;
using System.Windows.Media.Animation;
using DriveHUD.Common.Wpf.Converters;
using DriveHUD.Common.Reflection;
using HandHistories.Objects.Cards;
using System.Collections.ObjectModel;
using Model.Interfaces;
using Model.Data;
using Model.Reports;
using DriveHUD.Common.Linq;
using Microsoft.Practices.ServiceLocation;
using System.Threading;
using System.Diagnostics;
using DriveHUD.Application.Controls;
using DriveHUD.Application.ViewModels.Layouts;
using DriveHUD.Application.Views.Replayer;
using DriveHUD.Entities;
using DriveHUD.Application.ViewModels.Popups;
using DriveHUD.Application.Views.Popups;

namespace DriveHUD.Application.TableConfigurators
{
    public class ReplayerTableConfigurator : IReplayerTableConfigurator
    {
        #region Fields

        private const int PLAYER_WIDTH = 150;
        private const int PLAYER_HEIGHT = 60;
        private const int BUTTON_HEIGHT = 31;
        private const int BUTTON_WIDTH = 31;

        public const int CARD_WIDTH = 84;
        public const int CARD_HEIGHT = 112;

        private EnumReplayerTableType CurrentCapacity;

        private const string BackgroundImage = "/DriveHUD.Common.Resources;component/images/ReplayerTable.png";
        private const string DealerImage = "/DriveHUD.Common.Resources;component/images/D.png";
        private readonly Point DefaultTablePosition = new Point(120, 60);
        private readonly Point TotalPotChipsPosition = new Point(560, 100);

        private readonly Dictionary<int, Tuple<double, double>> predefinedTableSizes = new Dictionary<int, Tuple<double, double>>
        {
            {6, new Tuple<double, double>(420, 820)},
            {9, new Tuple<double, double>(420, 820)},
            {10, new Tuple<double, double>(420, 820)}
        };

        private readonly Dictionary<int, double[,]> predefinedLabelPositions = new Dictionary<int, double[,]>
        {
            { 6, new double[,] { { 50, 230 }, { 190, 70 }, { 720, 70 }, { 890, 230 }, { 720, 410 }, { 190, 410 } } },
            { 9, new double[,] { { 75, 310}, {75, 130}, {325, 70}, {605, 70}, {835, 130}, {835, 310}, {645, 420}, {455, 420}, {245, 420} } },
            { 10, new double[,] { { 125, 90}, { 315, 70}, {585, 70}, {815, 90}, {825, 260}, {765, 420}, {555, 420}, {355, 420}, {145, 420}, {65, 260} } }
        };

        private readonly Dictionary<int, double[,]> predefinedChipsPositions = new Dictionary<int, double[,]>
        {
            {6, new double[,] { { 240, 260 }, { 360, 120 }, { 680, 120 }, { 820, 260 }, { 680, 320 }, { 370, 320 } } },
            {9, new double[,] { { 240, 270 }, { 250, 160 }, { 390, 150 }, { 680, 160 }, { 790, 190 }, { 790, 270 }, { 700, 320 }, { 520, 320 }, { 310, 320 } } },
            {10, new double[,] { { 280, 150 }, { 370, 160 }, { 645, 160 }, { 760, 150 }, { 780, 230 }, { 760, 320 }, { 620, 320 }, { 410, 320 }, { 250, 320 }, { 240, 250 } } },
        };

        private RadDiagramShape table;

        private IHudLayoutsService hudLayoutsService;
        private IDataService dataService;
        #endregion

        public ReplayerTableConfigurator()
        {
            hudLayoutsService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();
            dataService = ServiceLocator.Current.GetInstance<IDataService>();
        }

        public void ConfigureTable(RadDiagram diagram, ReplayerViewModel viewModel)
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
                CreatePlayersLayout(diagram, viewModel);
                CreateCurrentPotValueLabel(diagram, viewModel);
                CreateTotalPotValueLabel(diagram, viewModel);
                PlaceTableCardPanels(diagram, viewModel);

                var currentGame = viewModel.CurrentGame;
                var statInfoCollection = GetHudStats(GetPokerSite(currentGame.GameDescription.Site), GetGameType(currentGame.GameDescription, viewModel.CurrentHand.Statistic.PlayerName), (EnumTableType)currentGame.GameDescription.SeatType.MaxPlayers);

                if (statInfoCollection != null && statInfoCollection.Any())
                {
                    viewModel.PlayersCollection.ForEach(x => LoadPlayerHudStats(x, viewModel, statInfoCollection, dataService));
                }
                else
                {
                    // Load default
                    statInfoCollection = GetHudStats(EnumPokerSites.Ignition, EnumGameType.CashHoldem, EnumTableType.Six);

                    if (statInfoCollection != null && statInfoCollection.Any())
                    {
                        viewModel.PlayersCollection.ForEach(x => LoadPlayerHudStats(x, viewModel, statInfoCollection, dataService));
                    }
                }
            }
            catch (Exception e)
            {
                LogProvider.Log.Error("ReplayerTable: Cannot configure table", e);
            }
        }

        private void CreateTable(RadDiagram diagram)
        {
            int seats = (int)CurrentCapacity;
            diagram.Clear();
            table = new RadDiagramShape()
            {
                Name = "Table",
                Background =
                    new ImageBrush(new BitmapImage(new Uri(BaseUriHelper.GetBaseUri(diagram), BackgroundImage))),
                Height = predefinedTableSizes[seats].Item1,
                Width = predefinedTableSizes[seats].Item2,
                StrokeThickness = 0,
                IsEnabled = false
            };

            table.X = DefaultTablePosition.X;
            table.Y = DefaultTablePosition.Y;

            diagram.AddShape(table);
        }

        private void CreatePlayersLayout(RadDiagram diagram, ReplayerViewModel viewModel)
        {
            int seats = (int)CurrentCapacity;
            var labelPositions = predefinedLabelPositions[seats];
            bool hasHoleCards = viewModel.CurrentGame.Players.Any(x => x.hasHoleCards);
            int cardsCount = hasHoleCards ? viewModel.CurrentGame.Players.Where(x => x.hasHoleCards).Max(x => x.HoleCards.Count) : 2;

            for (int i = 0; i < seats; i++)
            {
                var player = viewModel.PlayersCollection[i];
                var label = CreatePlayerLabel(player);

                label.X = labelPositions[i, 0];
                label.Y = labelPositions[i, 1];

                PlaceCardLabels(diagram, label, cardsCount);

                player.ChipsContainer.ChipsShape.X = predefinedChipsPositions[seats][i, 0];
                player.ChipsContainer.ChipsShape.Y = predefinedChipsPositions[seats][i, 1];

                AddPotPlayerLabel(diagram, viewModel.PlayersCollection[i], predefinedChipsPositions[seats][i, 0], predefinedChipsPositions[seats][i, 1]);

                ReplayerHudPanel panel = CreateHudPanel(viewModel, i);

                var hudOffsetX = (PLAYER_WIDTH - 150) / 2;
                var hudOffsetY = PLAYER_HEIGHT - 10;
                Point hudPanelPosition = new Point(labelPositions[i, 0] + hudOffsetX, labelPositions[i, 1] + hudOffsetY);

                diagram.AddShape(label);
                diagram.AddShape(player.ChipsContainer.ChipsShape);
                diagram.AddShape(panel, position: hudPanelPosition);
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

            ReplayerHudPanel panel = new ReplayerHudPanel
            {
                DataContext = player
            };

            panel.Height = double.NaN;
            panel.Width = 150;

            player.NoteToolTip = dataService.GetPlayerNote(player.Name, viewModel.CurrentHand.PokersiteId)?.Note ?? string.Empty;

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
            item.SetBinding(RadMenuItem.HeaderProperty, binding);

            item.Click += (s, e) =>
            {
                PlayerNoteViewModel viewModel = new PlayerNoteViewModel(pokerSiteId, playerName);
                var frm = new PlayerNoteView(viewModel);
                frm.ShowDialog();

                if (viewModel.PlayerNoteEntity == null)
                {
                    return;
                }

                var clickedItem = s as FrameworkElement;
                if (clickedItem == null || !(clickedItem.DataContext is ReplayerPlayerViewModel))
                {
                    return;
                }

                var hudElement = clickedItem.DataContext as ReplayerPlayerViewModel;
                hudElement.NoteToolTip = viewModel.Note;
            };
            radMenu.Items.Add(item);

            return radMenu;
        }

        private void LoadPlayerHudStats(ReplayerPlayerViewModel replayerPlayer, ReplayerViewModel replayerViewModel, IList<StatInfo> statInfoCollection, IDataService dataService)
        {
            replayerPlayer.StatInfoCollection.Clear();

            var statisticCollection = dataService.GetPlayerStatisticFromFile(replayerPlayer.Name, replayerViewModel.CurrentHand.PokersiteId);
            var hudIndicators = new HudLightIndicators(statisticCollection);

            if (hudIndicators != null)
            {
                var statList = new List<StatInfo>();
                foreach (var selectedStatInfo in statInfoCollection)
                {
                    if (selectedStatInfo is StatInfoBreak)
                    {
                        replayerPlayer.StatInfoCollection.Add((selectedStatInfo as StatInfoBreak).Clone());
                        continue;
                    }

                    var statInfo = selectedStatInfo.Clone();

                    if (!string.IsNullOrWhiteSpace(statInfo.PropertyName))
                    {
                        statInfo.AssignStatInfoValues(hudIndicators);
                    }

                    replayerPlayer.StatInfoCollection.Add(statInfo);
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
            return activeLayout.HudStats.ToArray();
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
