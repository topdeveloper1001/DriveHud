//-----------------------------------------------------------------------
// <copyright file="IHudStoreUploadModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Model.AppStore.HudStore.Model;
using System.Collections.Generic;

namespace Model.AppStore.HudStore
{
    public interface IHudStoreUploadModel
    {
        IEnumerable<GameVariant> GameVariants { get; }

        IList<GameVariant> SelectedGameVariants { get; }

        IEnumerable<GameType> GameTypes { get; }

        IList<GameType> SelectedGameTypes { get; }

        IEnumerable<TableType> TableTypes { get; }

        IList<TableType> SelectedTableTypes { get; }

        IEnumerable<string> LayoutsNamesInUse { get; }

        void Load();

        void Upload(HudStoreUploadInfo uploadInfo);
    }
}