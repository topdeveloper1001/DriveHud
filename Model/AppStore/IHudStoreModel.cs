//-----------------------------------------------------------------------
// <copyright file="IHudStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.AppStore.HudStore;
using Model.AppStore.HudStore.Model;
using System.IO;

namespace Model.AppStore
{
    public interface IHudStoreModel : IBaseAppStoreModel<HudStoreItem>
    {
        void Refresh();

        Stream DownloadLayout(int layoutId, string serial);

        HudStoreFilter SelectedFilter { get; set; }

        HudStoreSorting SelectedSorting { get; set; }
    }
}