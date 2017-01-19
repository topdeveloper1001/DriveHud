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

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectLayoutViewModel : ViewModelBase
    {
        private readonly HudSelectLayoutViewModelInfo _viewModelInfo;

        public HudSelectLayoutViewModel(HudSelectLayoutViewModelInfo viewModelInfo)
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this._viewModelInfo = viewModelInfo;

            Initialize();
        }

        private void Initialize()
        {
            var hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var layouts =
                hudLayoutService.GetLayoutsNames().ToList();

            items = new ObservableCollection<string>(layouts);
            if (!items.Contains(_viewModelInfo.LayoutName))
                items.Insert(0, _viewModelInfo.LayoutName);
            selectedItem = _viewModelInfo.LayoutName;



            ShowInput = _viewModelInfo.IsSaveAsMode;

            if (ShowInput)
            {
                Name = _viewModelInfo.LayoutName;
            }

            var canSave = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            SaveCommand = ReactiveCommand.Create(canSave);
            SaveCommand.Subscribe(x =>
            {
                _viewModelInfo.Save?.Invoke();
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                _viewModelInfo.Cancel?.Invoke();
            });
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }


        public string Header
        {
            get
            {
                return _viewModelInfo.IsDeleteMode
                    ? CommonResourceManager.Instance.GetResourceString("Common_HudLayout_DeleteLabel")
                    : CommonResourceManager.Instance.GetResourceString("Common_HudLayout_SelectLabel");
            }
        }

        private bool showInput;

        public bool ShowInput
        {
            get { return showInput; }
            private set { this.RaiseAndSetIfChanged(ref showInput, value); }
        }

        private ObservableCollection<string> items;

        public ObservableCollection<string> Items
        {
            get { return items; }
            private set { this.RaiseAndSetIfChanged(ref items, value); }
        }

        private string selectedItem;

        public string SelectedItem
        {
            get { return selectedItem; }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
                Name = SelectedItem;
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { this.RaiseAndSetIfChanged(ref name, value); }
        }
    }
}