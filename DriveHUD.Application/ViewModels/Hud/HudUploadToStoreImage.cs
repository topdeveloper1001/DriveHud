﻿//-----------------------------------------------------------------------
// <copyright file="HudUploadToStoreImage.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Common.Wpf.Validation;
using ReactiveUI;
using System.Windows;

namespace DriveHUD.Application.ViewModels.Hud
{
    public class HudUploadToStoreImage : WpfViewModelBase<HudUploadToStoreImage>
    {
        static HudUploadToStoreImage()
        {
            Rules.Add(new DelegateRule<HudUploadToStoreImage>(nameof(Caption),
                new LocalizableString("Common_HudUploadToStoreView_CaptionMustBeNotEmpty"),
                x => !string.IsNullOrEmpty(x.Caption)));

            Rules.Add(new DelegateRule<HudUploadToStoreImage>(nameof(Path),
                new LocalizableString("Common_HudUploadToStoreView_ImageWidthOutOfRange"),
                x => x.ImageSize.Width <= 4000 && x.ImageSize.Width >= 100));

            Rules.Add(new DelegateRule<HudUploadToStoreImage>(nameof(Path),
                new LocalizableString("Common_HudUploadToStoreView_ImageHeightOutOfRange"),
                x => x.ImageSize.Height <= 4000 && x.ImageSize.Height >= 100));
        }

        public HudUploadToStoreImage() : base()
        {
            ApplyRules();
        }

        private string caption;

        public string Caption
        {
            get
            {
                return caption;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref caption, value);
            }
        }

        private string path;

        public string Path
        {
            get
            {
                return path;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref path, value);
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

        private Size imageSize;

        public Size ImageSize
        {
            get
            {
                return imageSize;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref imageSize, value);
            }
        }
    }
}