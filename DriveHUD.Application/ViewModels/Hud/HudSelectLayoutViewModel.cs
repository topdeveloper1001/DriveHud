//-----------------------------------------------------------------------
// <copyright file="HudSelectLayoutViewModel.cs" company="Ace Poker Solutions">
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
using Microsoft.Practices.ServiceLocation;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudSelectLayoutViewModel : ViewModelBase
    {
        private readonly HudSelectLayoutViewModelInfo viewModelInfo;

        public HudSelectLayoutViewModel(HudSelectLayoutViewModelInfo viewModelInfo)
        {
            Check.Require(viewModelInfo != null);

            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        private void Initialize()
        {
            var hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var layouts = hudLayoutService.GetLayoutsNames(viewModelInfo.TableType).ToList();

            items = new ObservableCollection<string>(layouts);

            if (!items.Contains(viewModelInfo.LayoutName))
            {
                items.Insert(0, viewModelInfo.LayoutName);
            }

            selectedItem = viewModelInfo.LayoutName;

            ShowInput = viewModelInfo.IsSaveAsMode;

            if (ShowInput)
            {
                Name = viewModelInfo.LayoutName;
            }

            var canSave = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            SaveCommand = ReactiveCommand.Create(() => viewModelInfo.Save?.Invoke(), canSave);
            CancelCommand = ReactiveCommand.Create(() => viewModelInfo.Cancel?.Invoke());
        }

        public ReactiveCommand SaveCommand { get; private set; }

        public ReactiveCommand CancelCommand { get; private set; }

        public string Header
        {
            get
            {
                return viewModelInfo.IsDeleteMode
                    ? CommonResourceManager.Instance.GetResourceString("Common_HudLayout_DeleteLabel")
                    : CommonResourceManager.Instance.GetResourceString("Common_HudLayout_SelectLabel");
            }
        }

        private bool showInput;

        public bool ShowInput
        {
            get
            {
                return showInput;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref showInput, value);
            }
        }

        private ObservableCollection<string> items;

        public ObservableCollection<string> Items
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

        private string selectedItem;

        public string SelectedItem
        {
            get
            {
                return selectedItem;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
                Name = SelectedItem;
            }
        }

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
    }
}