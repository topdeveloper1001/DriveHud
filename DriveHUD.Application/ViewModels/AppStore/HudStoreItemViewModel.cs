//-----------------------------------------------------------------------
// <copyright file="HudStoreItemViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using Model.AppStore.HudStore.Model;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.Application.ViewModels.AppStore
{
    public class HudStoreItemViewModel : ViewModelBase
    {
        private readonly HudStoreItem hudStoreItem;

        public HudStoreItemViewModel(HudStoreItem hudStoreItem)
        {
            this.hudStoreItem = hudStoreItem;

            images = new ReactiveList<SelectableItemViewModel<HudStoreImageItem>>(
                hudStoreItem.Images.Select(x => new SelectableItemViewModel<HudStoreImageItem>(x)))
            {
                ChangeTrackingEnabled = true
            };

            images.ItemChanged.Subscribe(x =>
            {
                if (x.Sender.IsSelected &&
                    x.PropertyName == nameof(SelectableItemViewModel<HudStoreImageItem>.IsSelected))
                {
                    SelectedImage = x.Sender.Item;
                }
            });

            var imageToSelect = images.FirstOrDefault();

            if (imageToSelect != null)
            {
                imageToSelect.IsSelected = true;
            }
        }

        public HudStoreItem Item
        {
            get
            {
                return hudStoreItem;
            }
        }

        private HudStoreImageItem selectedImage;

        public HudStoreImageItem SelectedImage
        {
            get
            {
                return selectedImage;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedImage, value);
            }
        }

        private ReactiveList<SelectableItemViewModel<HudStoreImageItem>> images;

        public ReactiveList<SelectableItemViewModel<HudStoreImageItem>> Images
        {
            get
            {
                return images;
            }
        }

        private bool isImported;

        public bool IsImported
        {
            get
            {
                return isImported;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isImported, value);
            }
        }
    }
}