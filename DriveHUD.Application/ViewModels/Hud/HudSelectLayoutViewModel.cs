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

using System;
using System.Linq;
using ReactiveUI;
using Microsoft.Practices.ServiceLocation;
using System.Reactive.Linq;
using DriveHUD.Common;
using DriveHUD.Common.Resources;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels
{
    public class HudSelectLayoutViewModel : ViewModelBase
    {
        private HudSelectLayoutViewModelInfo viewModelInfo;

        public HudSelectLayoutViewModel(HudSelectLayoutViewModelInfo viewModelInfo)
        {
            Check.ArgumentNotNull(() => viewModelInfo);

            this.viewModelInfo = viewModelInfo;

            Initialize();
        }

        private void Initialize()
        {
            var hudLayoutService = ServiceLocator.Current.GetInstance<IHudLayoutsService>();

            var layouts = hudLayoutService.Layouts.Where(x => x.LayoutId == viewModelInfo.LayoutId).ToList();

            if (!viewModelInfo.IsDeleteMode)
            {
                var defaultLayout = layouts.FirstOrDefault(x => x.IsDefault);

                if (defaultLayout != null)
                {
                    selectedItem = defaultLayout.Name;
                }
            }

            items = new ObservableCollection<string>(layouts.Select(x => x.Name));

            ShowInput = viewModelInfo.IsSaveAsMode;

            if (ShowInput)
            {
                Name = selectedItem;
            }

            var canSave = this.WhenAny(x => x.Name, x => !string.IsNullOrWhiteSpace(x.Value));

            SaveCommand = ReactiveCommand.Create(canSave);
            SaveCommand.Subscribe(x =>
            {
                if (viewModelInfo.Save != null)
                {
                    viewModelInfo.Save();
                }
            });

            CancelCommand = ReactiveCommand.Create();
            CancelCommand.Subscribe(x =>
            {
                if (viewModelInfo.Cancel != null)
                {
                    viewModelInfo.Cancel();
                }
            });
        }

        public ReactiveCommand<object> SaveCommand { get; private set; }

        public ReactiveCommand<object> CancelCommand { get; private set; }


        public string Header
        {
            get
            {
                return viewModelInfo.IsDeleteMode ?
                            CommonResourceManager.Instance.GetResourceString("Common_HudLayout_DeleteLabel") :
                            CommonResourceManager.Instance.GetResourceString("Common_HudLayout_SelectLabel");
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