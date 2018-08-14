//-----------------------------------------------------------------------
// <copyright file="HudUploadToStoreRequestInfo.cs" company="Ace Poker Solutions">
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
using Prism.Interactivity.InteractionRequest;

namespace DriveHUD.Application.ViewModels.Hud
{
    internal class HudUploadToStoreRequestInfo : INotification
    {
        public HudUploadToStoreRequestInfo(HudUploadToStoreViewModelInfo viewModelInfo)
        {
            Content = viewModelInfo;
            Title = CommonResourceManager.Instance.GetResourceString("Common_HudUploadToStorePopup_Title");
        }

        public object Content
        {
            get;
            set;
        }

        public string Title
        {
            get; set;
        }
    }
}