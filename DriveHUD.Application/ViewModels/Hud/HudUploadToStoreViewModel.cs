//-----------------------------------------------------------------------
// <copyright file="HudUploadToStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Wpf.Validation;
using Model.AppStore.HudStore;
using Model.AppStore.HudStore.Model;
using ReactiveUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudUploadToStoreViewModel : WindowViewModel<HudUploadToStoreViewModel, IHudStoreUploadModel>, IHudUploadToStoreViewModel, INotifyDataErrorInfo
    {
        static HudUploadToStoreViewModel()
        {
            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Name),
                new NonLocalizableString("Name must be not empty"),
                x => !string.IsNullOrEmpty(x.Name)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Name),
                new NonLocalizableString("Name length must be greater than 10, but less than 50"),
                x => x.Name != null && x.Name.Length >= 10 && x.Name.Length <= 50));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Description),
                new NonLocalizableString("Description must be not empty"),
                x => { Task.Delay(2000).Wait(); return !string.IsNullOrEmpty(x.Description); }, true));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(Cost),
                new NonLocalizableString("Cost be must be 0 or positive number."),
                x => x.Cost >= 0));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(GameVariants),
                new NonLocalizableString("At least one game variant must be selected"),
                x => x.GameVariants != null && x.GameVariants.Any(p => p.IsSelected)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(GameTypes),
                new NonLocalizableString("At least one game type must be selected"),
                x => x.GameTypes != null && x.GameTypes.Any(p => p.IsSelected)));

            Rules.Add(new DelegateRule<HudUploadToStoreViewModel>(
                nameof(TableTypes),
                new NonLocalizableString("At least one table type must be selected"),
                x => x.TableTypes != null && x.TableTypes.Any(p => p.IsSelected)));
        }

        public override void Configure(object viewModelInfo)
        {
            InitializeModelAsync(() => Model.Load());
            Initialize();
        }

        #region Properties

        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref name, value);
            }
        }

        private string description;

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref description, value);
            }
        }

        private decimal cost;

        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref cost, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<GameVariant>> gameVariants;

        public ReactiveList<SelectableItemViewModel<GameVariant>> GameVariants
        {
            get
            {
                return gameVariants;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref gameVariants, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<GameType>> gameTypes;

        public ReactiveList<SelectableItemViewModel<GameType>> GameTypes
        {
            get
            {
                return gameTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref gameTypes, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<TableType>> tableTypes;

        public ReactiveList<SelectableItemViewModel<TableType>> TableTypes
        {
            get
            {
                return tableTypes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref tableTypes, value);
            }
        }

        #endregion

        #region Command

        public ReactiveCommand SubmitCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand ResetCommand { get; private set; }

        #endregion

        #region Infrastructure

        protected override void InitializeCommands()
        {
            var canSubmit = this.WhenAny(x => x.HasErrors, x => x.IsValidating, (x1, x2) => !HasErrors && !IsValidating);

            SubmitCommand = ReactiveCommand.Create(() => Upload(), canSubmit);
            CancelCommand = ReactiveCommand.Create(() => OnClosed());
            ResetCommand = ReactiveCommand.Create(() => Reset());
        }

        protected override void ModelInitialized()
        {
            base.ModelInitialized();
            InitializeSelectableData();
        }

        protected CompositeDisposable selectableDataSubscription = new CompositeDisposable();

        private void InitializeSelectableData()
        {
            GameVariants = InitializeSelectableData(Model.GameVariants, () => new GameVariant { Name = "All" }, nameof(GameVariants));
            GameTypes = InitializeSelectableData(Model.GameTypes, () => new GameType { Name = "All" }, nameof(GameTypes));
            TableTypes = InitializeSelectableData(Model.TableTypes, () => new TableType { Name = "All" }, nameof(TableTypes));
        }

        private ReactiveList<SelectableItemViewModel<T>> InitializeSelectableData<T>(IEnumerable<T> source, Func<T> creator, string propertyName)
        {
            var allItem = new SelectableItemViewModel<T>(creator());

            var data = new ReactiveList<SelectableItemViewModel<T>>(source.Select(x => new SelectableItemViewModel<T>(x)))
            {
                ChangeTrackingEnabled = true
            };

            data.Insert(0, allItem);

            var ignoreSelectionChanged = false;

            selectableDataSubscription.Add(
                allItem.Changed
                    .Where(x => x.PropertyName == nameof(SelectableItemViewModel<T>.IsSelected))
                    .Subscribe(x =>
                    {
                        if (!ignoreSelectionChanged)
                        {
                            data.ChangeTrackingEnabled = false;
                            data.Where(p => !ReferenceEquals(allItem, p)).ForEach(p => p.IsSelected = allItem.IsSelected);
                            data.ChangeTrackingEnabled = true;
                            ApplyRules(propertyName);
                        }
                    }));

            selectableDataSubscription.Add(
                data.ItemChanged
                    .Where(x => !ReferenceEquals(allItem, x.Sender))
                    .Subscribe(x =>
                    {
                        if (!x.Sender.IsSelected && allItem.IsSelected)
                        {
                            ignoreSelectionChanged = true;
                            allItem.IsSelected = false;
                            ignoreSelectionChanged = false;
                        }
                        else if (!allItem.IsSelected &&
                            x.Sender.IsSelected &&
                            data.Where(p => !ReferenceEquals(allItem, p)).All(p => p.IsSelected))
                        {
                            ignoreSelectionChanged = true;
                            allItem.IsSelected = true;
                            ignoreSelectionChanged = false;
                        }

                        ApplyRules(propertyName);
                    }));

            return data;
        }

        private void Upload()
        {
            StartAsyncOperation(() =>
            {
                Task.Delay(3000).Wait();

                // exception or just error ?
                // e.g. timeout or some server error
                // duplicate?
                // show error form with Retry or Back button

            },
            () => { });
        }

        private void Reset()
        {
            Name = string.Empty;
        }

        #endregion
    }
}