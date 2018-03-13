//-----------------------------------------------------------------------
// <copyright file="HudStatSettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using Model.Enums;
using Model.Stats;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudStatSettingsViewModel : ViewModelBase
    {
        public HudStatSettingsViewModel(HudStatSettingsViewModelInfo viewModelInfo)
        {
            Initialize(viewModelInfo);
        }

        private void Initialize(HudStatSettingsViewModelInfo viewModelInfo)
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            var clonedItems = viewModelInfo.SelectedStatInfoCollection.Where(x => !(x is StatInfoBreak) && x.Stat != Stat.PlayerInfoIcon).Select(x => x.Clone()).ToArray();

            if (viewModelInfo.SelectedStatInfo != null)
            {
                var clonedSelectedItem = clonedItems.FirstOrDefault(x => x.Id == viewModelInfo.SelectedStatInfo.Id);

                if (clonedSelectedItem != null)
                {
                    selectedItem = clonedSelectedItem;
                }
            }
            else
            {
                selectedItem = clonedItems.FirstOrDefault();
            }

            HudOpacity = viewModelInfo.HudOpacity;
            items = new ObservableCollection<StatInfo>(clonedItems);

            filterTableTypes = new ReactiveList<TableTypeFilterViewModel>(Enum.GetValues(typeof(EnumTableType))
                .Cast<EnumTableType>()
                .Select(x => new TableTypeFilterViewModel(x)
                {
                    IsSelected = viewModelInfo.SelectedTableTypes != null && viewModelInfo.SelectedTableTypes.Contains(x)
                }));

            filterTableTypes.ChangeTrackingEnabled = true;

            filterTableTypes.ItemChanged.Subscribe(x =>
            {
                if (x.PropertyName == nameof(TableTypeFilterViewModel.IsSelected))
                {
                    RaisePropertyChanged(() => TableTypeFilterText);
                }
            });

            dataFreshnessItems = new ObservableCollection<HudStatsDataFreshness>(Enum.GetValues(typeof(HudStatsDataFreshness))
                .Cast<HudStatsDataFreshness>());

            DataFreshness = viewModelInfo.DataFreshness;

            InitializeCommands(viewModelInfo);

            this.ObservableForProperty(x => x.SelectedColor).Subscribe(x =>
            {
                if (selectedStatInfoOptionValueRange != null)
                {
                    selectedStatInfoOptionValueRange.Color = x.Value;
                }
            });
        }

        private void InitializeCommands(HudStatSettingsViewModelInfo viewModelInfo)
        {
            SaveCommand = ReactiveCommand.Create(() => viewModelInfo.Save?.Invoke());
            CancelCommand = ReactiveCommand.Create(() => viewModelInfo.Cancel?.Invoke());
            SelectColorCommand = ReactiveCommand.Create<StatInfoOptionValueRange>(x => SelectColor(x));
            PickerSelectColorCommand = ReactiveCommand.Create(() => IsColorPickerPopupOpened = false);
            ResetRangeColorsCommand = ReactiveCommand.Create(() => SelectedItem?.Initialize());
        }

        public ReactiveCommand SaveCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand SelectColorCommand { get; private set; }

        public ReactiveCommand PickerSelectColorCommand { get; private set; }

        public ReactiveCommand ResetRangeColorsCommand { get; private set; }

        private ObservableCollection<StatInfo> items;

        public ObservableCollection<StatInfo> Items
        {
            get
            {
                return items;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref items, value);
            }
        }

        private double hudOpacity;

        public double HudOpacity
        {
            get
            {
                return hudOpacity;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref hudOpacity, value);
            }
        }

        private StatInfo selectedItem;

        public StatInfo SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
            }
        }

        private bool isColorPickerPopupOpened;

        public bool IsColorPickerPopupOpened
        {
            get
            {
                return isColorPickerPopupOpened;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref isColorPickerPopupOpened, value);
            }
        }

        private Color selectedColor;

        public Color SelectedColor
        {
            get
            {
                return selectedColor;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedColor, value);
            }
        }

        public string TableTypeFilterText
        {
            get
            {
                if (FilterTableTypes == null || FilterTableTypes.All(x => !x.IsSelected))
                {
                    return CommonResourceManager.Instance.GetResourceString("Common_HudStatSettings_TableFilterEmptyText");
                }

                var tableTypeFilterText = string.Join(" ", FilterTableTypes
                    .Where(x => x.IsSelected)
                    .OrderBy(x => x.TableType)
                    .Select(x => x.TableTypeText)
                    .ToArray());

                return tableTypeFilterText;
            }
        }

        private StatInfoOptionValueRange selectedStatInfoOptionValueRange;

        private ReactiveList<TableTypeFilterViewModel> filterTableTypes;

        public ReactiveList<TableTypeFilterViewModel> FilterTableTypes
        {
            get
            {
                return filterTableTypes;
            }
        }

        private bool isDataFreshnessLocked;

        private double? dataFreshness;

        public double? DataFreshness
        {
            get
            {
                return dataFreshness;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref dataFreshness, value);

                if (!isDataFreshnessLocked)
                {
                    isDataFreshnessLocked = true;

                    SelectedDataFreshnessItem = value.HasValue && Enum.IsDefined(typeof(HudStatsDataFreshness), (int)value.Value) ?
                        (HudStatsDataFreshness)(int)value.Value :
                        value.HasValue ? (HudStatsDataFreshness?)null : HudStatsDataFreshness.All;

                    isDataFreshnessLocked = false;
                }
            }
        }

        private HudStatsDataFreshness? selectedDataFreshnessItem;

        public HudStatsDataFreshness? SelectedDataFreshnessItem
        {
            get
            {
                return selectedDataFreshnessItem;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedDataFreshnessItem, value);

                if (!isDataFreshnessLocked)
                {
                    isDataFreshnessLocked = true;

                    DataFreshness = selectedDataFreshnessItem.HasValue ? (int)selectedDataFreshnessItem : 0;

                    isDataFreshnessLocked = false;
                }
            }
        }

        private ObservableCollection<HudStatsDataFreshness> dataFreshnessItems;

        public ObservableCollection<HudStatsDataFreshness> DataFreshnessItems
        {
            get
            {
                return dataFreshnessItems;
            }
        }

        private void SelectColor(StatInfoOptionValueRange statInfoValueRange)
        {
            IsColorPickerPopupOpened = true;
            selectedStatInfoOptionValueRange = statInfoValueRange;
        }

        public class TableTypeFilterViewModel : ViewModelBase
        {
            public TableTypeFilterViewModel(EnumTableType tableType)
            {
                TableType = tableType;
            }

            public EnumTableType TableType { get; private set; }

            public string TableTypeText
            {
                get
                {
                    return CommonResourceManager.Instance.GetEnumResource(TableType);
                }
            }

            private bool isSelected;

            public bool IsSelected
            {
                get
                {
                    return isSelected;
                }
                set
                {
                    this.RaiseAndSetIfChanged(ref isSelected, value);
                }
            }
        }
    }
}