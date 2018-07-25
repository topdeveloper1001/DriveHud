//-----------------------------------------------------------------------
// <copyright file="IHudStoreWebService.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.AppStore.HudStore.ServiceData;
using Model.AppStore.HudStore.ServiceResponses;
using System.IO;

namespace Model.AppStore.HudStore
{
    /// <summary>
    /// Interface for hud store web service
    /// </summary>
    public interface IHudStoreWebService
    {
        Stream DownloadHud(HudStoreDownloadHudRequest request);

        HudStoreHudsData GetHuds(HudStoreGetHudsRequest request);

        HudStoreData GetUploadInfo();

        void Upload(HudStoreUploadInfo uploadInfo);
    }
}