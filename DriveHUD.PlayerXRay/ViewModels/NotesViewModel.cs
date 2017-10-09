//-----------------------------------------------------------------------
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
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class NotesViewModel : WorkspaceViewModel
    {
        public NotesViewModel()
        {
            InitializeHoleCardsCollection();
            InitializeCommands();
            InitializeActions();

            IsAdvancedMode = true;
        }

        private void InitializeActions()
        {
            var actions = Enum.GetValues(typeof(ActionTypeEnum)).Cast<ActionTypeEnum>().Where(x => x != ActionTypeEnum.Fold);

            firstActions = new ObservableCollection<ActionTypeEnum>(actions);
            secondActions = new ObservableCollection<ActionTypeEnum>(actions);
            thirdActions = new ObservableCollection<ActionTypeEnum>(actions);
            fourthActions = new ObservableCollection<ActionTypeEnum>(actions);
        }

        private void InitializeCommands()
        {
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
        }

        private void InitializeHoleCardsCollection()
        {
            HoleCardsCollection = new ObservableCollection<HoleCardsViewModel>();

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

        private ObservableCollection<StageObject> stages;

        public ObservableCollection<StageObject> Stages
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
            }
        }

        private ObservableCollection<HoleCardsViewModel> holeCardsCollection;

        public ObservableCollection<HoleCardsViewModel> HoleCardsCollection
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

        #endregion
    }
}