//-----------------------------------------------------------------------
// <copyright file="IAppStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System.ComponentModel;

namespace Model.AppStore
{
    public interface IAppStoreModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Total amount of items
        /// </summary>
        int ItemsCount { get; }

        /// <summary>
        /// Refreshes Items
        /// </summary>        
        /// <param name="start">Start index of items to be selected</param>
        /// <param name="amount">Amount of items to be selected</param>
        void Refresh(int start, int amount);

        /// <summary>
        /// Searches items and updates Items
        /// </summary>
        /// <param name="searchText">Search text</param>
        void Search(string searchText);
    }
}