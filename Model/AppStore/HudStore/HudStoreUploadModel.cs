//-----------------------------------------------------------------------
// <copyright file="HudStoreUploadModel.cs" company="Ace Poker Solutions">
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
using Model.AppStore.HudStore.Model;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.AppStore.HudStore
{
    public class HudStoreUploadModel : BindableBase, IHudStoreUploadModel
    {
        private readonly IHudStoreWebService service;

        public HudStoreUploadModel()
        {
            service = ServiceLocator.Current.GetInstance<IHudStoreWebService>();
        }

        #region Properties

        private IList<GameVariant> selectedGameVariants;

        public IList<GameVariant> SelectedGameVariants
        {
            get
            {
                return selectedGameVariants;
            }
            private set
            {
                SetProperty(ref selectedGameVariants, value);
            }
        }

        private IEnumerable<GameVariant> gameVariants;

        public IEnumerable<GameVariant> GameVariants
        {
            get
            {
                return gameVariants;
            }
            private set
            {
                SetProperty(ref gameVariants, value);
            }
        }

        private IList<GameType> selectedGameTypes;

        public IList<GameType> SelectedGameTypes
        {
            get
            {
                return selectedGameTypes;
            }
            private set
            {
                SetProperty(ref selectedGameTypes, value);
            }
        }

        private IEnumerable<GameType> gameTypes;

        public IEnumerable<GameType> GameTypes
        {
            get
            {
                return gameTypes;
            }
            private set
            {
                SetProperty(ref gameTypes, value);
            }
        }

        private IList<TableType> selectedTableTypes;

        public IList<TableType> SelectedTableTypes
        {
            get
            {
                return selectedTableTypes;
            }
            set
            {
                SetProperty(ref selectedTableTypes, value);
            }
        }

        private IEnumerable<TableType> tableTypes;

        public IEnumerable<TableType> TableTypes
        {
            get
            {
                return tableTypes;
            }
            private set
            {
                SetProperty(ref tableTypes, value);
            }
        }

        private IEnumerable<string> layoutsNamesInUse;

        public IEnumerable<string> LayoutsNamesInUse
        {
            get
            {
                return layoutsNamesInUse;
            }
            private set
            {
                SetProperty(ref layoutsNamesInUse, value);
            }
        }

        #endregion

        public void Load()
        {
            SelectedGameTypes = new List<GameType>();
            SelectedGameVariants = new List<GameVariant>();
            SelectedTableTypes = new List<TableType>();

            var hudStoreData = service.GetUploadInfo();

            GameVariants = hudStoreData.GameVariants.ToList();
            GameTypes = hudStoreData.GameTypes.ToList();
            TableTypes = hudStoreData.TableTypes.ToList();
            LayoutsNamesInUse = hudStoreData.LayoutsNames.ToList();
        }

        public void Upload(HudStoreUploadInfo uploadInfo)
        {
            service.Upload(uploadInfo);
        }
    }
}