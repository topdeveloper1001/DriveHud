//-----------------------------------------------------------------------
// <copyright file="BaseAppStoreModel.cs" company="Ace Poker Solutions">
// Copyright © 2018 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Model.AppStore
{
    public abstract class BaseAppStoreModel<T> : BindableBase, IBaseAppStoreModel<T>
    {
        protected IList<T> allItems = new List<T>();

        protected IList<T> activeItems = new List<T>();

        #region Properties

        protected ObservableCollection<T> items = new ObservableCollection<T>();

        public virtual ObservableCollection<T> Items
        {
            get
            {
                return items;
            }
            protected set
            {
                SetProperty(ref items, value);
            }
        }

        public virtual int ItemsCount
        {
            get
            {
                return activeItems != null ? activeItems.Count : default(int);
            }
        }

        #endregion

        #region Model methods

        public abstract void Load(object loadInfo);

        public virtual void Refresh(int start, int amount)
        {
            if (activeItems == null)
            {
                return;
            }

            Items.Clear();
            Items.AddRange(activeItems.Skip(start).Take(amount));
        }

        /// <summary>
        /// Search data
        /// </summary>
        public abstract void Search(string searchText);

        #endregion
    }
}