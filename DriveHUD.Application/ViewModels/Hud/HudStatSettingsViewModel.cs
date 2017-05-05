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
using DriveHUD.Common.Wpf.Mvvm;
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

            SaveCommand = ReactiveCommand.Create();
            SaveCommand.Subscribe(x =>
            {
                viewModelInfo.Save?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                viewModelInfo.Cancel?.Invoke();
            });

            SelectColorCommand = ReactiveCommand.Create();
            SelectColorCommand.Subscribe(x => SelectColor(x as StatInfoOptionValueRange));

            PickerSelectColorCommand = ReactiveCommand.Create();
            PickerSelectColorCommand.Subscribe(x => IsColorPickerPopupOpened = false);

            this.ObservableForProperty(x => x.SelectedColor).Subscribe(x =>
            {
                if (selectedStatInfoOptionValueRange != null)
                {
                    selectedStatInfoOptionValueRange.Color = x.Value;
                }
            });
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }

        public ReactiveCommand<object> SelectColorCommand { get; private set; }

        public ReactiveCommand<object> PickerSelectColorCommand { get; private set; }

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
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedColor, value);
            }
        }

        private StatInfoOptionValueRange selectedStatInfoOptionValueRange;

        private void SelectColor(StatInfoOptionValueRange statInfoValueRange)
        {
            IsColorPickerPopupOpened = true;
            selectedStatInfoOptionValueRange = statInfoValueRange;
        }
    }
}