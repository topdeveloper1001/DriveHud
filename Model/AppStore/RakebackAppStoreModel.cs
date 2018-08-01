//-----------------------------------------------------------------------
// <copyright file="RakebackAppStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Microsoft.Practices.ServiceLocation;
using System.Linq;

namespace Model.AppStore
{
    internal class RakebackAppStoreModel: BaseProductAppStoreModel<AppStoreRakeback>, IRakebackAppStoreModel
    {
        public RakebackAppStoreModel()
        {
            repository = ServiceLocator.Current.GetInstance<IRakebackAppStoreRepository>();
        }

        /// <summary>
        /// Search data
        /// </summary>
        public override void Search(string searchText)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                activeItems = allItems;
            }

            activeItems = allItems.Where(x => x.Network.Contains(searchText) || x.Description.Contains(searchText)).ToList();
        }
    }
}