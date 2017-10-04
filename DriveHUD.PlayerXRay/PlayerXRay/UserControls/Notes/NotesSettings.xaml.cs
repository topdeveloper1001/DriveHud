using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.CustomControls;
using DriveHUD.PlayerXRay.UserControls.BoardTexture;
using AcePokerSolutions.UIControls.CustomControls;

namespace DriveHUD.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for NotesSettings.xaml
    /// </summary>
    public partial class NotesSettings
    {
        private StakesUC m_stakesUC;
        private HoleCardsUC m_holeCardsUC;

        private PlayerSettingUC m_playerTag;
        private PlayerSettingUC m_playerWhale;
        private PlayerSettingUC m_playerGambler;
        private PlayerSettingUC m_playerNit;
        private PlayerSettingUC m_playerFish;
        private PlayerSettingUC m_playerLag;
        private PlayerSettingUC m_playerRock;
        private BoardTextureUC m_boardTextureUc;

        public NoteObject Note { get; set; }

        public NotesSettings()
        {
            InitializeComponent();
        }

        public bool PendingChanges()
        {
            NoteSettingsObject settingsObject = GetNoteSettingsObject();
            return !settingsObject.Equals(Note.Settings) || Note.DisplayedNote != txtDisplayedNote.Text;
        }

        public void SaveChanges()
        {
            Note.Settings = GetNoteSettingsObject();
            Note.DisplayedNote = txtDisplayedNote.Text;
            NotesAppSettingsHelper.SaveAppSettings();
        }

        private NoteSettingsObject GetNoteSettingsObject()
        {
            NoteSettingsObject settings = new NoteSettingsObject
                                              {
                                                  MBCMinSizeOfPot = (double) txtMinSizeOfPot.Value,
                                                  PositionBB = (bool) chkPositionBB.IsChecked,
                                                  PositionSB = (bool) chkPositionSB.IsChecked,
                                                  PositionMiddle = (bool) chkPositionMiddle.IsChecked,
                                                  PositionButton = (bool) chkPositionButton.IsChecked,
                                                  PositionCutoff = (bool) chkPositionCutoff.IsChecked,
                                                  PositionEarly = (bool) chkPositionEarly.IsChecked,
                                                  PositionBBRaiser = (bool)chkPositionBB1.IsChecked,
                                                  PositionSBRaiser = (bool)chkPositionSB1.IsChecked,
                                                  PositionMiddleRaiser = (bool)chkPositionMiddle1.IsChecked,
                                                  PositionButtonRaiser = (bool)chkPositionButton1.IsChecked,
                                                  PositionCutoffRaiser = (bool)chkPositionCutoff1.IsChecked,
                                                  PositionEarlyRaiser = (bool)chkPositionEarly1.IsChecked,
                                                  Facing1Limper = (bool)chkFacing1Limper.IsChecked,
                                                  Facing2PlusLimpers = (bool)chkFacing2PlusLimpers.IsChecked,
                                                  Facing1Raiser = (bool)chkFacing1Raiser.IsChecked,
                                                  Facing2Raisers = (bool)chkFacing2Raisers.IsChecked,
                                                  FacingRaisersCallers = (bool)chkFacingRaisersCallers.IsChecked,
                                                  FacingUnopened = (bool)chkFacingUnopened.IsChecked,
                                                  PositionBB3Bet = (bool)chkPositionBB2.IsChecked,
                                                  PositionSB3Bet = (bool)chkPositionSB2.IsChecked,
                                                  PositionMiddle3Bet = (bool)chkPositionMiddle2.IsChecked,
                                                  PositionButton3Bet = (bool)chkPositionButton2.IsChecked,
                                                  PositionCutoff3Bet = (bool)chkPositionCutoff2.IsChecked,
                                                  PositionEarly3Bet = (bool)chkPositionEarly2.IsChecked,
                                                  PlayersNo34 = (bool) chkPlayersNo34.IsChecked,
                                                  PlayersNo56 = (bool) chkPlayersNo56.IsChecked,
                                                  PlayersNoHeadsUp = (bool) chkPlayersNoHeadsUp.IsChecked,
                                                  PlayersNoMax = (bool) chkPlayersNoMax.IsChecked,
                                                  PlayersNoCustom = (bool) chkPlayersNoCustom.IsChecked,
                                                  PlayersNoMinVal = (int) txtPlayersNoMin.Value,
                                                  PlayersNoMaxVal = (int) txtPlayersNoMax.Value,
                                                  TypeLimit = (bool) chkTypeLimit.IsChecked,
                                                  TypeNoLimit = (bool) chkTypeNoLimit.IsChecked,
                                                  TypePotLimit = (bool) chkTypePotLimit.IsChecked,
                                                  FishPlayer = m_playerFish.GetPlayerObject(),
                                                  GamblerPlayer = m_playerGambler.GetPlayerObject(),
                                                  WhalePlayer = m_playerWhale.GetPlayerObject(),
                                                  NitPlayer = m_playerNit.GetPlayerObject(),
                                                  TagPlayer = m_playerTag.GetPlayerObject(),
                                                  RockPlayer = m_playerRock.GetPlayerObject(),
                                                  LagPlayer = m_playerLag.GetPlayerObject(),
                                                  Cash = (bool) chkCash.IsChecked,
                                                  Tournament = (bool) chkTournaments.IsChecked
                                              };

            settings.ExcludedStakes.Clear();
            settings.ExcludedStakes.AddRange(StaticStorage.Stakes.FindAll(p => !p.IsSelected));
            settings.ExcludedCardsList = m_holeCardsUC.ExcludedCards;
            settings.SelectedFilters = GetSelectedFilters();
            notesHvTurn.FillSettingsObject(settings);
            notesHvRiver.FillSettingsObject(settings);
            notesHvFlop.FillSettingsObject(settings);
            settings.SelectedFiltersComparison = Note.Settings.SelectedFiltersComparison;
            if (m_boardTextureUc != null)
                m_boardTextureUc.FillSettings(settings);
            else
            {
                settings.TurnTextureSettings = Note.Settings.TurnTextureSettings;
                settings.FlopTextureSettings = Note.Settings.FlopTextureSettings;
                settings.RiverTextureSettings = Note.Settings.RiverTextureSettings;
            }
            settings.PreflopActions = preFlopActionUC.GetInfo();
            settings.FlopActions = flopActionUC.GetInfo();
            settings.TurnActions = turnActionUC.GetInfo();
            settings.RiverActions = riverActionUC.GetInfo();
            return settings;
        }

        private List<FilterObject> GetSelectedFilters()
        {
            List<FilterObject> result = new List<FilterObject>();

            foreach (TreeViewItem item in treeSelectedFilters.Items)
            {
                foreach (TreeViewItem child in item.Items)
                {
                    result.Add((FilterObject)child.Tag);
                }
            }

            return result;
        }
        
        public void Initialize(NoteObject note)
        {
            Note = note;

            if (m_stakesUC == null)
            {
                m_stakesUC = new StakesUC();
                gridStakes.Children.Add(m_stakesUC);
            }
            m_stakesUC.Initialize(Note.Settings.ExcludedStakes);

            if (m_holeCardsUC == null)
            {
                m_holeCardsUC = new HoleCardsUC();
                tabHoleCards.Content = m_holeCardsUC;
            }

            txtDisplayedNote.Text = note.DisplayedNote;
            m_holeCardsUC.Initialize(Note.Settings.ExcludedCardsList);
            lblDescription.Text = Note.Name;
            notesHvFlop.Initialize(note.Settings, NoteStageType.Flop);
            notesHvTurn.Initialize(note.Settings, NoteStageType.Turn);
            notesHvRiver.Initialize(note.Settings, NoteStageType.River);

            if (m_boardTextureUc != null)
                m_boardTextureUc.Initialize(Note.Settings);
            FillInfo();
        }

        private void FillInfo()
        {
            DisableEventHandlers();

            chkTypePotLimit.IsChecked = Note.Settings.TypePotLimit;
            chkTypeLimit.IsChecked = Note.Settings.TypeLimit;
            chkTypeNoLimit.IsChecked = Note.Settings.TypeNoLimit;

            chkCash.IsChecked = Note.Settings.Cash;
            chkTournaments.IsChecked = Note.Settings.Tournament;

            chkPlayersNo34.IsChecked = Note.Settings.PlayersNo34;
            chkPlayersNo56.IsChecked = Note.Settings.PlayersNo56;
            chkPlayersNoHeadsUp.IsChecked = Note.Settings.PlayersNoHeadsUp;
            chkPlayersNoMax.IsChecked = Note.Settings.PlayersNoMax;

            txtPlayersNoMin.Value = Note.Settings.PlayersNoMinVal;
            txtPlayersNoMax.Value = Note.Settings.PlayersNoMaxVal;
            chkPlayersNoCustom.IsChecked = Note.Settings.PlayersNoCustom;

            chkPositionSB.IsChecked = Note.Settings.PositionSB;
            chkPositionBB.IsChecked = Note.Settings.PositionBB;
            chkPositionEarly.IsChecked = Note.Settings.PositionEarly;
            chkPositionCutoff.IsChecked = Note.Settings.PositionCutoff;
            chkPositionMiddle.IsChecked = Note.Settings.PositionMiddle;
            chkPositionButton.IsChecked = Note.Settings.PositionButton;

            chkPositionSB1.IsChecked = Note.Settings.PositionSBRaiser;
            chkPositionBB1.IsChecked = Note.Settings.PositionBBRaiser;
            chkPositionEarly1.IsChecked = Note.Settings.PositionEarlyRaiser;
            chkPositionCutoff1.IsChecked = Note.Settings.PositionCutoffRaiser;
            chkPositionMiddle1.IsChecked = Note.Settings.PositionMiddleRaiser;
            chkPositionButton1.IsChecked = Note.Settings.PositionButtonRaiser;

            chkFacing1Limper.IsChecked = Note.Settings.Facing1Limper;
            chkFacing1Raiser.IsChecked = Note.Settings.Facing1Raiser;
            chkFacing2PlusLimpers.IsChecked = Note.Settings.Facing2PlusLimpers;
            chkFacing2Raisers.IsChecked = Note.Settings.Facing2Raisers;
            chkFacingRaisersCallers.IsChecked = Note.Settings.FacingRaisersCallers;
            chkFacingUnopened.IsChecked = Note.Settings.FacingUnopened;

            chkPositionSB2.IsChecked = Note.Settings.PositionSB3Bet;
            chkPositionBB2.IsChecked = Note.Settings.PositionBB3Bet;
            chkPositionEarly2.IsChecked = Note.Settings.PositionEarly3Bet;
            chkPositionCutoff2.IsChecked = Note.Settings.PositionCutoff3Bet;
            chkPositionMiddle2.IsChecked = Note.Settings.PositionMiddle3Bet;
            chkPositionButton2.IsChecked = Note.Settings.PositionButton3Bet;

            txtMinSizeOfPot.Value = Note.Settings.MBCMinSizeOfPot;
            chkMBCAllInPreFlop.IsChecked = Note.Settings.MBCAllInPreFlop;
            chkMBCWentToShowdown.IsChecked = Note.Settings.MBCWentToShowdown;

            EnableEventHandlers();

            if (m_playerTag == null)
            {
                m_playerTag = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerTag);
            }
            m_playerTag.Initialize(Note.Settings.TagPlayer);

            if (m_playerFish == null)
            {
                m_playerFish = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerFish);
            }
            m_playerFish.Initialize(Note.Settings.FishPlayer);

            if (m_playerGambler == null)
            {
                m_playerGambler = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerGambler);
            }
            m_playerGambler.Initialize(Note.Settings.GamblerPlayer);

            if (m_playerLag == null)
            {
                m_playerLag = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerLag);
            }
            m_playerLag.Initialize(Note.Settings.LagPlayer);

            if (m_playerNit == null)
            {
                m_playerNit = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerNit);
            }
            m_playerNit.Initialize(Note.Settings.NitPlayer);

            if (m_playerRock == null)
            {
                m_playerRock = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerRock);
            }
            m_playerRock.Initialize(Note.Settings.RockPlayer);

            if (m_playerWhale == null)
            {
                m_playerWhale = new PlayerSettingUC();
                stackPlayers.Children.Add(m_playerWhale);
            }
            m_playerWhale.Initialize(Note.Settings.WhalePlayer);

            m_stakesUC.RefreshList(Note.Settings.TypeNoLimit, Note.Settings.TypeLimit, Note.Settings.TypePotLimit,
                                   (bool) chkPlayersNoHeadsUp.IsChecked,
                                   (bool) chkPlayersNo34.IsChecked, (bool) chkPlayersNo56.IsChecked,
                                   (bool) chkPlayersNoMax.IsChecked, (bool) chkPlayersNoCustom.IsChecked,
                                   (int)txtPlayersNoMin.Value, (int)txtPlayersNoMax.Value);

            flopActionUC.FillInfo(Note.Settings.FlopActions);
            turnActionUC.FillInfo(Note.Settings.TurnActions);
            riverActionUC.FillInfo(Note.Settings.RiverActions);
            preFlopActionUC.FillInfo(Note.Settings.PreflopActions);

            FillFilters();
        }

        private void FillFilters()
        {
            foreach (TreeViewItem parentItem in treeSelectedFilters.Items)
            {
                parentItem.Items.Clear();
            }

            foreach (FilterObject filter in Note.Settings.SelectedFilters)
            {
                TreeViewItem parent = (TreeViewItem)treeSelectedFilters.Items[(int) filter.Stage];
                string description;
                if (filter.Value != null)
                    description = filter.Description + " (" + filter.Value + ")";
                else
                    description = filter.Description;
                TreeViewItem item = new TreeViewItem {Tag = filter, Header = description};
                parent.Items.Add(item);
            }
        }

        private void BtnCheckAllClick(object sender, RoutedEventArgs e)
        {
            StakesUC.CheckAll();
        }

        private void BtnUncheckAllClick(object sender, RoutedEventArgs e)
        {
            StakesUC.UncheckAll();
        }

        private void ChkPlayersNoCustomChecked(object sender, RoutedEventArgs e)
        {
            chkPlayersNo34.IsChecked = false;
            chkPlayersNo56.IsChecked = false;
            chkPlayersNoMax.IsChecked = false;
            chkPlayersNoHeadsUp.IsChecked = false;

            txtPlayersNoMin.IsEnabled = true;
            txtPlayersNoMax.IsEnabled = true;

            RefreshStakesList();
        }

        private void DisableEventHandlers()
        {
            chkTypeLimit.Checked -= ChkTypeCheckedChanged;
            chkTypeNoLimit.Checked -= ChkTypeCheckedChanged;
            chkTypePotLimit.Checked -= ChkTypeCheckedChanged;

            chkPlayersNo34.Checked -= ChkNoPlayersChecked;
            chkPlayersNo34.Unchecked -= ChkNoPlayersUnChecked;
            chkPlayersNo56.Checked -= ChkNoPlayersChecked;
            chkPlayersNo56.Unchecked -= ChkNoPlayersUnChecked;
            chkPlayersNoMax.Checked -= ChkNoPlayersChecked;
            chkPlayersNoMax.Unchecked -= ChkNoPlayersUnChecked;
            chkPlayersNoHeadsUp.Checked -= ChkNoPlayersChecked;
            chkPlayersNoHeadsUp.Unchecked -= ChkNoPlayersUnChecked;
        }

        private void EnableEventHandlers()
        {
            chkTypeLimit.Checked += ChkTypeCheckedChanged;
            chkTypeNoLimit.Checked += ChkTypeCheckedChanged;
            chkTypePotLimit.Checked += ChkTypeCheckedChanged;

            chkPlayersNo34.Checked += ChkNoPlayersChecked;
            chkPlayersNo34.Unchecked += ChkNoPlayersUnChecked;
            chkPlayersNo56.Checked += ChkNoPlayersChecked;
            chkPlayersNo56.Unchecked += ChkNoPlayersUnChecked;
            chkPlayersNoMax.Checked += ChkNoPlayersChecked;
            chkPlayersNoMax.Unchecked += ChkNoPlayersUnChecked;
            chkPlayersNoHeadsUp.Checked += ChkNoPlayersChecked;
            chkPlayersNoHeadsUp.Unchecked += ChkNoPlayersUnChecked;
        }

        private void ChkPlayersNoCustomUnchecked(object sender, RoutedEventArgs e)
        {
            DisableEventHandlers();

            txtPlayersNoMin.IsEnabled = false;
            txtPlayersNoMax.IsEnabled = false;

            chkPlayersNo34.IsChecked = true;
            chkPlayersNo56.IsChecked = true;
            chkPlayersNoMax.IsChecked = true;
            chkPlayersNoHeadsUp.IsChecked = true;

            RefreshStakesList();

            EnableEventHandlers();
        }

        private void ChkNoPlayersChecked(object sender, RoutedEventArgs e)
        {
            if ((bool)chkPlayersNoCustom.IsChecked)
            {
                chkPlayersNoCustom.IsChecked = false;
                return;
            }

            RefreshStakesList();
        }

        private void ChkNoPlayersUnChecked(object sender, RoutedEventArgs e)
        {
            if (!(bool)chkPlayersNoHeadsUp.IsChecked &&
                !(bool)chkPlayersNo34.IsChecked &&
                !(bool)chkPlayersNo56.IsChecked &&
                !(bool)chkPlayersNoMax.IsChecked)
            {
                chkPlayersNoCustom.IsChecked = true;
                return;
            }

            RefreshStakesList();
        }

        private void ChkTypeCheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshStakesList();
        }

        private void TxtPlayersNoValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!txtPlayersNoMin.IsEnabled || !txtPlayersNoMax.IsEnabled)
                return;
            RefreshStakesList();
        }

        private void RefreshStakesList()
        {
            m_stakesUC.RefreshList((bool)chkTypeNoLimit.IsChecked, (bool)chkTypeLimit.IsChecked, (bool)chkTypePotLimit.IsChecked,
                                   (bool)chkPlayersNoHeadsUp.IsChecked,
                                   (bool)chkPlayersNo34.IsChecked, (bool)chkPlayersNo56.IsChecked,
                                   (bool)chkPlayersNoMax.IsChecked, (bool)chkPlayersNoCustom.IsChecked,
                                   (int)txtPlayersNoMin.Value, (int)txtPlayersNoMax.Value);
        }

        private void StackPlayersInitialized(object sender, EventArgs e)
        {
        	
        }

        private void BtnAdvancedClick(object sender, RoutedEventArgs e)
        {
            if (tabAdvanced.Visibility == Visibility.Hidden)
            {
                btnAdvanced.Content = "SIMPLE MODE";
                tabAdvanced.Visibility = Visibility.Visible;
                tabGeneral.Visibility = Visibility.Hidden;
            }
            else
            {
                btnAdvanced.Content = "ADVANCED MODE";
                tabAdvanced.Visibility = Visibility.Hidden;
                tabGeneral.Visibility = Visibility.Visible;
            }
        }

        private void BtnRiverClick(object sender, RoutedEventArgs e)
        {
        	Grid.SetColumn(rectSelectedMenu, 3);
            gridActions.Children.Remove(riverActionUC);
            gridActions.Children.Add(riverActionUC);
        }

        private void BtnTurnClick(object sender, RoutedEventArgs e)
        {
            Grid.SetColumn(rectSelectedMenu, 2);
            gridActions.Children.Remove(turnActionUC);
            gridActions.Children.Add(turnActionUC);
        }

        private void BtnFlopClick(object sender, RoutedEventArgs e)
        {
            Grid.SetColumn(rectSelectedMenu, 1);
            gridActions.Children.Remove(flopActionUC);
            gridActions.Children.Add(flopActionUC);
        }

        private void BtnPreflopClick(object sender, RoutedEventArgs e)
        {
            Grid.SetColumn(rectSelectedMenu, 0);
            gridActions.Children.Remove(preFlopActionUC);
            gridActions.Children.Add(preFlopActionUC);
        }

        private void BtnAddToListMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (treeFilters.SelectedItem == null)
                return;

            TreeViewItem selectedFilter = (TreeViewItem)treeFilters.SelectedItem;
            if (!(selectedFilter.Parent is TreeViewItem))
                return;

            TreeViewItem selectedFilterParent = (TreeViewItem)selectedFilter.Parent;
            int tag = Int32.Parse(selectedFilter.Tag.ToString());

            TreeViewItem targetFilterItem = GetExistingItem(tag);
            FilterObject filter;

            if (targetFilterItem == null)
            {
                filter = new FilterObject
                             {
                                 Description = GetFilterDescription(tag),
                                 Tag = tag,
                                 Stage = (NoteStageType) (Int32.Parse(selectedFilterParent.Tag.ToString()))
                             };
            }
            else
            {
                filter = (FilterObject) targetFilterItem.Tag;
            }

            if (filter.Description.Contains("..."))
            {
                NoteFilterValueAdd win = new NoteFilterValueAdd(filter.Description, filter.Value);
                win.ShowDialog();
                if (win.SelectedValue == null)
                    return;

                filter.Value = win.SelectedValue.Value;
            }

            if (targetFilterItem == null)
            {
                targetFilterItem = new TreeViewItem {Header = filter.Description};
                TreeViewItem parentSelected = (TreeViewItem)treeSelectedFilters.Items[(int)filter.Stage];
                parentSelected.Items.Add(targetFilterItem);
            }
            targetFilterItem.Tag = filter;
            targetFilterItem.Header = filter.Value.HasValue
                                          ? filter.Description + " (" + filter.Value.Value + ")"
                                          : filter.Description;

            if (filter.Tag == (int)FilterEnum.AllinPreflop)
                chkMBCAllInPreFlop.IsChecked = true;
            if (filter.Tag == (int)FilterEnum.SawShowdown)
                chkMBCWentToShowdown.IsChecked = true;
        }

        private TreeViewItem GetExistingItem(int tag)
        {
            foreach (TreeViewItem item in treeSelectedFilters.Items)
            {
                foreach (TreeViewItem childItem in item.Items)
                {
                    if (!(childItem.Tag is FilterObject)) continue;
                    FilterObject filter = (FilterObject) childItem.Tag;
                    if (filter.Tag == tag)
                        return childItem;
                }
            }
            return null;
        }

        private string GetFilterDescription(int value)
        {
            foreach (TreeViewItem item in treeFilters.Items)
            {
                foreach (TreeViewItem childItem in item.Items)
                {
                    if (value.ToString() == childItem.Tag.ToString())
                        return childItem.Header.ToString();
                }
            }

            return "";
        }

        private void BtnRemoveFromListMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (treeSelectedFilters.SelectedItem == null)
                return;
            TreeViewItem selectedFilter = (TreeViewItem)treeSelectedFilters.SelectedItem;
            if (!(selectedFilter.Parent is TreeViewItem))
                return;
            TreeViewItem selectedFilterParent = (TreeViewItem)selectedFilter.Parent;
            selectedFilterParent.Items.Remove(selectedFilter);

            FilterObject filter = (FilterObject) selectedFilter.Tag;
            if (filter.Tag == (int)FilterEnum.AllinPreflop)
                chkMBCAllInPreFlop.IsChecked = false;
            if (filter.Tag == (int)FilterEnum.SawShowdown)
                chkMBCWentToShowdown.IsChecked = false;
        }

        private void TreeFiltersMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	BtnAddToListMouseLeftButtonUp(null, null);
        }

        private void TreeSelectedFiltersMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	BtnRemoveFromListMouseLeftButtonUp(null, null);
        }

        private void BtnHvRiverClick(object sender, RoutedEventArgs e)
        {
            notesHvFlop.Visibility = Visibility.Hidden;
            notesHvTurn.Visibility = Visibility.Hidden;
            notesHvRiver.Visibility = Visibility.Visible;

            Grid.SetColumn(rectHVSelectedMenu, 2);
        }

        private void BtnHvTurnClick(object sender, RoutedEventArgs e)
        {
            notesHvFlop.Visibility = Visibility.Hidden;
            notesHvTurn.Visibility = Visibility.Visible;
            notesHvRiver.Visibility = Visibility.Hidden;

            Grid.SetColumn(rectHVSelectedMenu, 1);
        }

        private void BtnHvFlopClick(object sender, RoutedEventArgs e)
        {
            notesHvFlop.Visibility = Visibility.Visible;
            notesHvTurn.Visibility = Visibility.Hidden;
            notesHvRiver.Visibility = Visibility.Hidden;

            Grid.SetColumn(rectHVSelectedMenu, 0);
        }

        private void TabAdvancedSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(tabAdvanced.SelectedItem is TabItem))
                return;

            TabItem tab = (TabItem) tabAdvanced.SelectedItem;

            if (tab.Header.ToString() != "Board Texture") return;
            if (m_boardTextureUc != null) return;
            m_boardTextureUc = new BoardTextureUC();
            m_boardTextureUc.Initialize(Note.Settings);
            panelBoardTexture.Content = m_boardTextureUc;
        }

        private void ChkMBCAllInPreFlopClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem parentItem = null;

            foreach (TreeViewItem itm in treeSelectedFilters.Items)
            {
                if (itm.Header.ToString() != "PreFlop") continue;

                parentItem = itm;
                break;
            }

            if (parentItem == null)
                return;

            if ((bool)chkMBCAllInPreFlop.IsChecked)
            {
                foreach (TreeViewItem itm in parentItem.Items)
                {
                    if (((FilterObject)itm.Tag).Tag == (int)FilterEnum.AllinPreflop)
                        return;
                }

                FilterObject filter = new FilterObject
                                          {
                                              Description = GetFilterDescription((int)FilterEnum.AllinPreflop),
                                              Tag = (int)FilterEnum.AllinPreflop,
                                              Stage = NoteStageType.PreFlop
                                          };

                TreeViewItem allinItem = new TreeViewItem {Header = filter.Description, Tag = filter};
                allinItem.Header = filter.Value.HasValue
                                         ? filter.Description + " (" + filter.Value.Value + ")"
                                         : filter.Description;
                parentItem.Items.Add(allinItem);
            }
            else
            {
                TreeViewItem allInItem = null;
                foreach (TreeViewItem itm in parentItem.Items)
                {
                    if (((FilterObject) itm.Tag).Tag != (int) FilterEnum.AllinPreflop) continue;
                    allInItem = itm;
                    break;
                }

                parentItem.Items.Remove(allInItem);
            }
        }

        private void ChkMBCWentToShowdownClick(object sender, RoutedEventArgs e)
        {
            TreeViewItem parentItem = null;

            foreach (TreeViewItem itm in treeSelectedFilters.Items)
            {
                if (itm.Header.ToString() != "Other") continue;

                parentItem = itm;
                break;
            }

            if (parentItem == null)
                return;

            if ((bool)chkMBCWentToShowdown.IsChecked)
            {
                foreach (TreeViewItem itm in parentItem.Items)
                {
                    if (((FilterObject)itm.Tag).Tag == (int)FilterEnum.SawShowdown)
                        return;
                }

                FilterObject filter = new FilterObject
                                          {
                                              Description = GetFilterDescription((int)FilterEnum.SawShowdown),
                                              Tag = (int)FilterEnum.SawShowdown,
                                              Stage = NoteStageType.Other
                                          };

                TreeViewItem allinItem = new TreeViewItem {Header = filter.Description, Tag = filter};
                allinItem.Header = filter.Value.HasValue
                                         ? filter.Description + " (" + filter.Value.Value + ")"
                                         : filter.Description;
                parentItem.Items.Add(allinItem);
            }
            else
            {
                TreeViewItem allInItem = null;
                foreach (TreeViewItem itm in parentItem.Items)
                {
                    if (((FilterObject) itm.Tag).Tag != (int) FilterEnum.SawShowdown) continue;
                    allInItem = itm;
                    break;
                }

                parentItem.Items.Remove(allInItem);
            }
        }
    }
}
