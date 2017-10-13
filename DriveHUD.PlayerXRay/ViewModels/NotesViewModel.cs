﻿//-----------------------------------------------------------------------
// <copyright file="NotesViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects.ActionsObjects;
using DriveHUD.PlayerXRay.Events;
using DriveHUD.PlayerXRay.ViewModels.PopupViewModels;
using DriveHUD.PlayerXRay.Views.PopupViews;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class NotesViewModel : WorkspaceViewModel
    {
        private readonly IEventAggregator eventAggregator;

        public NotesViewModel()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            InitializeStages();
            InitializeHoleCardsCollection();
            InitializeCommands();
            InitializeActions();
            InitializeFilters();
        }

        private void InitializeStages()
        {
            stages = new ReactiveList<StageObject>();
            ReloadStages();
        }

        private void InitializeActions()
        {
            var actions = Enum.GetValues(typeof(ActionTypeEnum)).Cast<ActionTypeEnum>().Where(x => x != ActionTypeEnum.Fold);

            firstActions = new ObservableCollection<ActionTypeEnum>(actions);
            secondActions = new ObservableCollection<ActionTypeEnum>(actions);
            thirdActions = new ObservableCollection<ActionTypeEnum>(actions);
            fourthActions = new ObservableCollection<ActionTypeEnum>(actions);
        }

        private void InitializeFilters()
        {
            filters = new ObservableCollection<FilterObject>(FiltersHelper.GetFiltersObjects());
            filtersCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(filters);
            filtersCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterObject.Stage)));

            selectedFilters = new ReactiveList<FilterObject>();
            selectedFilters.Changed.Subscribe(x =>
            {
                if (SelectedNote == null || SelectedNote.Settings == null)
                {
                    return;
                }

                if (x.Action == NotifyCollectionChangedAction.Add)
                {
                    var addedItems = x.NewItems.OfType<FilterObject>();

                    if (addedItems != null)
                    {
                        foreach (var addedItem in addedItems)
                        {
                            if (!SelectedNote.Settings.SelectedFilters.Any(f => f.Filter == addedItem.Filter))
                            {
                                SelectedNote.Settings.SelectedFilters.Add(addedItem);
                            }
                        }
                    }

                }
                else if (x.Action == NotifyCollectionChangedAction.Remove)
                {
                    var removedItems = x.OldItems.OfType<FilterObject>();

                    if (removedItems != null)
                    {
                        foreach (var removeItem in removedItems)
                        {
                            var itemToRemove = SelectedNote.Settings
                                .SelectedFilters
                                .FirstOrDefault(f => f.Filter == removeItem.Filter);

                            if (itemToRemove != null)
                            {
                                SelectedNote.Settings.SelectedFilters.Remove(itemToRemove);
                            }
                        }
                    }
                }
            });

            selectedFiltersCollectionView = (CollectionView)CollectionViewSource.GetDefaultView(selectedFilters);
            selectedFiltersCollectionView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(FilterObject.Stage)));
        }

        private void InitializeCommands()
        {
            var canAdd = this.WhenAny(x => x.SelectedNote, x => x.SelectedStage, (x1, x2) => x1.Value == null && x2.Value != null);

            AddNoteCommand = ReactiveCommand.Create(canAdd);
            AddNoteCommand.Subscribe(x => AddNote());

            var canEdit = this.WhenAny(x => x.SelectedStage, x => x.Value != null && x.Value is NoteTreeEditableObject);

            EditNoteCommand = ReactiveCommand.Create(canEdit);
            EditNoteCommand.Subscribe(x => EditNote());

            var canRemove = this.WhenAny(x => x.SelectedStage, x => (x.Value is InnerGroupObject) || (x.Value is NoteObject));

            RemoveNoteCommand = ReactiveCommand.Create(canRemove);
            RemoveNoteCommand.Subscribe(x => RemoveNote());

            SwitchModeCommand = ReactiveCommand.Create();
            SwitchModeCommand.Subscribe(x => IsAdvancedMode = !IsAdvancedMode);

            HoleCardsLeftClickCommand = ReactiveCommand.Create();
            HoleCardsLeftClickCommand.Subscribe(x => (x as HoleCardsViewModel).IsChecked = true);

            HoleCardsDoubleLeftClickCommand = ReactiveCommand.Create();
            HoleCardsDoubleLeftClickCommand.Subscribe(x => (x as HoleCardsViewModel).IsChecked = false);

            HoleCardsMouseEnterCommand = ReactiveCommand.Create();
            HoleCardsMouseEnterCommand.Subscribe(x => (x as HoleCardsViewModel).IsChecked = true);

            HoleCardsSelectAllCommand = ReactiveCommand.Create();
            HoleCardsSelectAllCommand.Subscribe(o => HoleCardsCollection.ForEach(x => x.IsChecked = true));

            HoleCardsSelectNoneCommand = ReactiveCommand.Create();
            HoleCardsSelectNoneCommand.Subscribe(o => HoleCardsCollection.ForEach(x => x.IsChecked = false));

            HoleCardsSelectSuitedGappersCommand = ReactiveCommand.Create();
            HoleCardsSelectSuitedGappersCommand.Subscribe(x => SelectSuitedGappers());

            HoleCardsSelectSuitedConnectorsCommand = ReactiveCommand.Create();
            HoleCardsSelectSuitedConnectorsCommand.Subscribe(x => SelectedSuitedConnectors());

            HoleCardsSelectPocketPairsCommand = ReactiveCommand.Create();
            HoleCardsSelectPocketPairsCommand.Subscribe(x => SelectPocketPairs());

            HoleCardsSelectOffSuitedGappersCommand = ReactiveCommand.Create();
            HoleCardsSelectOffSuitedGappersCommand.Subscribe(x => SelectOffSuitedGappers());

            HoleCardsSelectOffSuitedConnectorsCommand = ReactiveCommand.Create();
            HoleCardsSelectOffSuitedConnectorsCommand.Subscribe(x => SelectOffSuitedConnectors());

            AddToSelectedFiltersCommand = ReactiveCommand.Create();
            AddToSelectedFiltersCommand.Subscribe(x =>
            {
                var selectedItem = filters.FirstOrDefault(f => f.IsSelected);

                if (selectedItem != null && !selectedFilters.Any(f => f.Filter == selectedItem.Filter))
                {
                    var filterToAdd = selectedItem.Clone();
                    filterToAdd.IsSelected = false;

                    selectedFilters.Add(filterToAdd);
                }
            });

            RemoveFromSelectedFiltersCommand = ReactiveCommand.Create();
            RemoveFromSelectedFiltersCommand.Subscribe(x =>
            {
                var selectedItem = selectedFilters.FirstOrDefault(f => f.IsSelected);

                if (selectedItem != null)
                {
                    selectedFilters.Remove(selectedItem);
                }
            });
        }

        private void InitializeHoleCardsCollection()
        {
            HoleCardsCollection = new ReactiveList<HoleCardsViewModel>();

            var rankValues = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst;

            for (int i = 0; i < rankValues.Length; i++)
            {
                var startS = false;

                for (int j = 0; j < rankValues.Length; j++)
                {
                    var card1 = i < j ? rankValues.ElementAt(i) : rankValues.ElementAt(j);
                    var card2 = i < j ? rankValues.ElementAt(j) : rankValues.ElementAt(i);

                    if (startS)
                    {
                        HoleCardsCollection.Add(new HoleCardsViewModel
                        {
                            Name = $"{card1}{card2}s",
                            ItemType = RangeSelectorItemType.Suited,
                            IsChecked = true
                        });
                    }
                    else
                    {
                        if (!card1.Equals(card2))
                        {
                            HoleCardsCollection.Add(new HoleCardsViewModel
                            {
                                Name = $"{card1}{card2}o",
                                ItemType = RangeSelectorItemType.OffSuited,
                                IsChecked = true
                            });
                        }
                        else
                        {
                            HoleCardsCollection.Add(new HoleCardsViewModel
                            {
                                Name = $"{card1}{card2}",
                                ItemType = RangeSelectorItemType.Pair,
                                IsChecked = true
                            });

                            startS = true;
                        }
                    }
                }
            }

            HoleCardsCollection.ChangeTrackingEnabled = true;
            HoleCardsCollection.ItemChanged
                .Where(x => x.PropertyName == nameof(HoleCardsViewModel.IsChecked))
                .Select(x => x.Sender)
                .Subscribe(x =>
                {
                    if (SelectedNote != null && SelectedNote.Settings != null)
                    {
                        var excludedCardsList = SelectedNote.Settings.ExcludedCardsList;

                        if (!x.IsChecked && !excludedCardsList.Contains(x.Name))
                        {
                            excludedCardsList.Add(x.Name);
                            SelectedNote.Settings.ExcludedCardsList = excludedCardsList;
                        }
                        else if (x.IsChecked && excludedCardsList.Contains(x.Name))
                        {
                            excludedCardsList.Remove(x.Name);
                            SelectedNote.Settings.ExcludedCardsList = excludedCardsList;
                        }
                    }
                });
        }

        /// <summary>
        /// Reloads <see cref="Stages"/> collection accordingly to selected <see cref="NoteStageType"/>
        /// </summary>
        private void ReloadStages()
        {
            Stages?.ForEach(x => x.IsSelected = false);

            Stages?.Clear();

            Stages?.AddRange(NoteService
                .CurrentNotesAppSettings
                .StagesList
                .Where(x => x.StageType == NoteStageType));
        }

        private void LoadNote()
        {
            if (SelectedNote != null)
            {
                HoleCardsCollection.ForEach(x => x.IsChecked = !SelectedNote.Settings.ExcludedCardsList.Contains(x.Name));
            }

            RefreshCurrentActionSettings();
            RefreshFiltersSettings();

            this.RaisePropertyChanged(nameof(MBCWentToShowdown));
            this.RaisePropertyChanged(nameof(MBCAllInPreFlop));
        }

        private void SaveNote()
        {
            NoteService.SaveAppSettings();
        }

        private void RefreshFiltersSettings()
        {
            if (SelectedNote == null || SelectedNote.Settings == null)
            {
                return;
            }

            selectedFilters.Clear();
            SelectedNote.Settings.SelectedFilters.ForEach(x =>
            {
                x.IsSelected = false;
                selectedFilters.Add(x);
            });
        }

        private void RefreshCurrentActionSettings()
        {
            if (SelectedNote == null || SelectedNote.Settings == null)
            {
                return;
            }

            switch (ActionStageType)
            {
                case NoteStageType.PreFlop:
                    CurrentActionSettings = SelectedNote.Settings.PreflopActions;
                    return;
                case NoteStageType.Flop:
                    CurrentActionSettings = SelectedNote.Settings.FlopActions;
                    return;
                case NoteStageType.Turn:
                    CurrentActionSettings = SelectedNote.Settings.TurnActions;
                    return;
                case NoteStageType.River:
                    CurrentActionSettings = SelectedNote.Settings.RiverActions;
                    return;
            }
        }

        #region Properties

        public override WorkspaceType WorkspaceType
        {
            get
            {
                return WorkspaceType.Notes;
            }
        }

        private bool isAdvancedMode;

        public bool IsAdvancedMode
        {
            get
            {
                return isAdvancedMode;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isAdvancedMode, value);
            }
        }

        private ReactiveList<StageObject> stages;

        public ReactiveList<StageObject> Stages
        {
            get
            {
                return stages;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref stages, value);
            }
        }

        private NoteStageType noteStageType;

        public NoteStageType NoteStageType
        {
            get
            {
                return noteStageType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref noteStageType, value);
                ReloadStages();
            }
        }

        private NoteStageType actionStageType;

        public NoteStageType ActionStageType
        {
            get
            {
                return actionStageType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref actionStageType, value);
                RefreshCurrentActionSettings();
            }
        }

        private ReactiveList<HoleCardsViewModel> holeCardsCollection;

        public ReactiveList<HoleCardsViewModel> HoleCardsCollection
        {
            get
            {
                return holeCardsCollection;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref holeCardsCollection, value);
            }
        }

        private ActionTypeEnum firstAction;

        public ActionTypeEnum FirstAction
        {
            get
            {
                return firstAction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref firstAction, value);
            }
        }

        private ObservableCollection<ActionTypeEnum> firstActions;

        public ObservableCollection<ActionTypeEnum> FirstActions
        {
            get
            {
                return firstActions;
            }
        }

        private ActionTypeEnum secondAction;

        public ActionTypeEnum SecondAction
        {
            get
            {
                return secondAction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref secondAction, value);
            }
        }

        private ObservableCollection<ActionTypeEnum> secondActions;

        public ObservableCollection<ActionTypeEnum> SecondActions
        {
            get
            {
                return secondActions;
            }
        }

        private ActionTypeEnum thirdAction;

        public ActionTypeEnum ThirdAction
        {
            get
            {
                return thirdAction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref thirdAction, value);
            }
        }

        private ObservableCollection<ActionTypeEnum> thirdActions;

        public ObservableCollection<ActionTypeEnum> ThirdActions
        {
            get
            {
                return thirdActions;
            }
        }

        private ActionTypeEnum fourthAction;

        public ActionTypeEnum FourthAction
        {
            get
            {
                return fourthAction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref fourthAction, value);

            }
        }

        private ObservableCollection<ActionTypeEnum> fourthActions;

        public ObservableCollection<ActionTypeEnum> FourthActions
        {
            get
            {
                return fourthActions;
            }
        }

        private ObservableCollection<FilterObject> filters;

        private CollectionView filtersCollectionView;

        public CollectionView FiltersCollectionView
        {
            get
            {
                return filtersCollectionView;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref filtersCollectionView, value);
            }
        }

        private ReactiveList<FilterObject> selectedFilters;

        private CollectionView selectedFiltersCollectionView;

        public CollectionView SelectedFiltersCollectionView
        {
            get
            {
                return selectedFiltersCollectionView;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedFiltersCollectionView, value);
            }
        }

        private NoteTreeObjectBase selectedStage;

        public NoteTreeObjectBase SelectedStage
        {
            get
            {
                return selectedStage;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedStage, value);
                SelectedNote = SelectedStage as NoteObject;
            }
        }

        private NoteObject selectedNote;

        public NoteObject SelectedNote
        {
            get
            {
                return selectedNote;
            }
            private set
            {
                SaveNote();
                this.RaiseAndSetIfChanged(ref selectedNote, value);
                LoadNote();
            }
        }

        public bool MBCWentToShowdown
        {
            get
            {
                return SelectedNote != null ? SelectedNote.Settings.MBCWentToShowdown : false;
            }
            set
            {
                if (value)
                {
                    AddFilterItem(FilterEnum.SawShowdown);
                }
                else
                {
                    RemoveFilterItem(FilterEnum.SawShowdown);
                }

                this.RaisePropertyChanged();
            }
        }

        public bool MBCAllInPreFlop
        {
            get
            {
                return SelectedNote != null ? SelectedNote.Settings.MBCAllInPreFlop : false;
            }
            set
            {
                if (value)
                {
                    AddFilterItem(FilterEnum.AllinPreflop);
                }
                else
                {
                    RemoveFilterItem(FilterEnum.AllinPreflop);
                }

                this.RaisePropertyChanged();
            }
        }

        private ActionSettings currentActionSettings;

        public ActionSettings CurrentActionSettings
        {
            get
            {
                return currentActionSettings;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref currentActionSettings, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> AddNoteCommand { get; private set; }

        public ReactiveCommand<object> EditNoteCommand { get; private set; }

        public ReactiveCommand<object> RemoveNoteCommand { get; private set; }

        public ReactiveCommand<object> ExportCommand { get; private set; }

        public ReactiveCommand<object> SwitchModeCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsLeftClickCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsDoubleLeftClickCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsMouseEnterCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectSuitedGappersCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectSuitedConnectorsCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectPocketPairsCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectOffSuitedGappersCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectOffSuitedConnectorsCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectAllCommand { get; private set; }

        public ReactiveCommand<object> HoleCardsSelectNoneCommand { get; private set; }

        public ReactiveCommand<object> AddToSelectedFiltersCommand { get; private set; }

        public ReactiveCommand<object> RemoveFromSelectedFiltersCommand { get; private set; }

        #endregion

        #region Commands implementation

        private void SelectSuitedGappers()
        {
            var length = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Length;

            for (var i = 0; i < length - 2; i++)
            {
                HoleCardsCollection.ElementAt(i * length + i + 2).IsChecked = true;
            }
        }

        private void SelectOffSuitedGappers()
        {
            var length = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Length;

            for (var i = 2; i < length; i++)
            {
                HoleCardsCollection.ElementAt(i * length + i - 2).IsChecked = true;
            }
        }

        private void SelectOffSuitedConnectors()
        {
            var length = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Length;

            for (int i = 1; i < length; i++)
            {
                HoleCardsCollection.ElementAt(i * length + i - 1).IsChecked = true;
            }
        }

        private void SelectedSuitedConnectors()
        {
            var length = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Length;

            for (int i = 0; i < length - 1; i++)
            {
                HoleCardsCollection.ElementAt(i * length + i + 1).IsChecked = true;
            }
        }

        private void SelectPocketPairs()
        {
            var length = HandHistories.Objects.Cards.Card.PossibleRanksHighCardFirst.Length;

            for (var i = 0; i < length; i++)
            {
                HoleCardsCollection.ElementAt(i * length + i).IsChecked = true;
            }
        }

        private void AddNote()
        {
            var addNoteViewModel = new AddEditNoteViewModel
            {
                IsGroupPossible = SelectedStage is StageObject
            };

            addNoteViewModel.OnSaveAction = () =>
            {
                ReactiveList<NoteObject> noteList = null;

                if (SelectedStage is StageObject)
                {
                    if (addNoteViewModel.IsGroup)
                    {
                        var group = new InnerGroupObject
                        {
                            Name = addNoteViewModel.Name,
                            IsSelected = true
                        };

                        (SelectedStage as StageObject).InnerGroups.Add(group);
                        return;
                    }

                    noteList = (SelectedStage as StageObject).Notes;
                }
                else if (SelectedStage is InnerGroupObject)
                {
                    noteList = (SelectedStage as InnerGroupObject).Notes;
                }

                if (noteList == null)
                {
                    return;
                }

                var note = new NoteObject
                {
                    Name = addNoteViewModel.Name,
                    IsSelected = true
                };

                noteList.Add(note);
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Add Note/Group",
                Content = new AddEditNoteView(addNoteViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        private void EditNote()
        {
            var treeEditableObject = SelectedStage as NoteTreeEditableObject;

            if (treeEditableObject == null)
            {
                return;
            }

            var addNoteViewModel = new AddEditNoteViewModel
            {
                Name = treeEditableObject.Name
            };

            addNoteViewModel.OnSaveAction = () =>
            {
                treeEditableObject.Name = addNoteViewModel.Name;
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = treeEditableObject is NoteObject ? "Edit Note" : "Edit Group",
                Content = new AddEditNoteView(addNoteViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        private void RemoveNote()
        {
            var confirmationViewModel = new YesNoConfirmationViewModel
            {
                ConfirmationMessage = "Are you sure you want to delete the selected item?"
            };

            confirmationViewModel.OnYesAction = () =>
            {
                foreach (var stage in Stages)
                {
                    if (SelectedStage is NoteObject)
                    {
                        var noteToRemove = SelectedStage as NoteObject;

                        stage.Notes.Remove(noteToRemove);

                        foreach (var group in stage.InnerGroups)
                        {
                            group.Notes.Remove(noteToRemove);
                        }
                    }
                    else if (SelectedStage is InnerGroupObject)
                    {
                        var groupToRemove = SelectedStage as InnerGroupObject;

                        stage.InnerGroups.Remove(groupToRemove);
                    }
                }
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Confirm Delete",
                Content = new YesNoConfirmationView(confirmationViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        #endregion

        #region Helpers

        private void AddFilterItem(FilterEnum filter)
        {
            var selectedfilterItem = selectedFilters.FirstOrDefault(x => x.Filter == filter);

            if (selectedfilterItem == null)
            {
                var filterItem = filters.FirstOrDefault(x => x.Filter == filter);

                if (filterItem != null)
                {
                    selectedFilters.Add(filterItem.Clone());
                }
            }
        }

        private void RemoveFilterItem(FilterEnum filter)
        {
            var selectedfilterItem = selectedFilters.FirstOrDefault(x => x.Filter == filter);

            if (selectedfilterItem != null)
            {
                selectedFilters.Remove(selectedfilterItem);
            }
        }

        #endregion
    }
}