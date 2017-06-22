//-----------------------------------------------------------------------
// <copyright file="AliasAddEditViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.Base;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Events;
using Prism.Events;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Alias
{
    public class AliasAddEditViewModel : BaseViewModel
    {
        internal AliasAddEditViewModel(AliasViewModelInfo<AliasCollectionItem> info)
        {
            InitializeBindings();
            InitializeData(info);
        }

        internal void InitializeBindings()
        {
            OKCommand = new RelayCommand(SaveChanges);
            CancelCommand = new RelayCommand(Cancel);
            SelectCommand = new RelayCommand(SwitchSelect);

            AllPlayersSorted = new CollectionViewSource();
            SelectedPlayersSorted = new CollectionViewSource();
        }

        internal void InitializeData(AliasViewModelInfo<AliasCollectionItem> info)
        {
            infoViewModel = info;
            newAlias = info?.Model;

            if (newAlias == null)
            {
                isAdd = true;
                newAlias = new AliasCollectionItem();
            }

            this.AliasName = newAlias?.Name ?? string.Empty;
            this.PlayersInAlias = new ObservableCollection<PlayerCollectionItem>(newAlias?.PlayersInAlias);

            AllPlayers = new ObservableCollection<PlayerCollectionItem>(StorageModel.PlayerCollection.Except(this.PlayersInAlias).OfType<PlayerCollectionItem>());

            AllPlayersSorted.Source = AllPlayers;
            AllPlayersSorted.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            SelectedPlayersSorted.Source = PlayersInAlias;
            SelectedPlayersSorted.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        #region Methods

        private void SaveChanges()
        {
            newAlias.Name = aliasName;
            newAlias.PlayersInAlias = playersInAlias;

            if (isAdd)
            {
                infoViewModel?.Add(newAlias);
            }
            else
            {
                infoViewModel?.Save(newAlias);
                infoViewModel?.Close();
            }

            if ((StorageModel.PlayerSelectedItem is AliasCollectionItem) && StorageModel.PlayerSelectedItem.Name.Equals(newAlias.Name))
            {
                var eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

                var args = new LoadDataRequestedEventArgs();

                eventAggregator.GetEvent<LoadDataRequestedEvent>().Publish(args);
            }
        }

        private void Cancel()
        {
            infoViewModel?.Close();
        }

        private void SwitchSelect(object obj)
        {
            IEnumerable args = obj as IEnumerable;
            List<PlayerCollectionItem> switchPlayers = new List<PlayerCollectionItem>(args.OfType<PlayerCollectionItem>());

            if (!switchPlayers.Any())
                return;

            if (PlayersInAlias.Contains(switchPlayers[0]))
            {
                int index = PlayersInAlias.IndexOf(switchPlayers[0]);

                switchPlayers.ForEach(player =>
                {
                    PlayersInAlias.Remove(player);
                    AllPlayers.Add(player);
                    player.LinkedAliases.Add(newAlias);
                });

                SelectedPlayersSelectdIndex = index + 1 > PlayersInAlias.Count ? 0 : index;
            }
            else
            {
                int index = AllPlayers.IndexOf(switchPlayers[0]);

                switchPlayers.ForEach(player =>
                {
                    PlayersInAlias.Add(player);
                    AllPlayers.Remove(player);
                    player.LinkedAliases.Remove(newAlias);
                });

                AllPlayersSelectdIndex = index + 1 > AllPlayers.Count ? 0 : index;
            }
        }

        private void AddFilterAll()
        {
            AllPlayersSorted.Filter -= new FilterEventHandler(FilterAll);
            AllPlayersSorted.Filter += new FilterEventHandler(FilterAll);
        }

        private void FilterAll(object sender, FilterEventArgs e)
        {
            var src = e.Item as PlayerCollectionItem;

            if (src == null)
            {
                e.Accepted = false;
            }
            else if (src.DecodedName != null && !src.DecodedName.ToLower().Contains(AllSearchFilter.ToLower()))
            {
                e.Accepted = false;
            }
        }

        private void AddFilterSelected()
        {
            SelectedPlayersSorted.Filter -= new FilterEventHandler(FilterSelected);
            SelectedPlayersSorted.Filter += new FilterEventHandler(FilterSelected);
        }

        private void FilterSelected(object sender, FilterEventArgs e)
        {
            var src = e.Item as PlayerCollectionItem;

            if (src == null)
            {
                e.Accepted = false;
            }
            else if (src.DecodedName != null && !src.DecodedName.ToLower().Contains(SelectedSearchFilter.ToLower()))
            {
                e.Accepted = false;
            }
        }

        #endregion

        #region Properties

        private int allPlayersSelectdIndex = -1;
        private int selectedPlayersSelectdIndex = -1;
        public int AllPlayersSelectdIndex
        {
            get
            {
                return allPlayersSelectdIndex;
            }
            set
            {
                SetProperty(ref allPlayersSelectdIndex, value);
            }
        }
        public int SelectedPlayersSelectdIndex
        {
            get
            {
                return selectedPlayersSelectdIndex;
            }
            set
            {
                SetProperty(ref selectedPlayersSelectdIndex, value);
            }
        }

        private ObservableCollection<PlayerCollectionItem> allPlayers;

        public ObservableCollection<PlayerCollectionItem> AllPlayers
        {
            get
            {
                return allPlayers;
            }
            set
            {
                SetProperty(ref allPlayers, value);
            }
        }

        private AliasViewModelInfo<AliasCollectionItem> infoViewModel;
        private AliasCollectionItem newAlias;

        private bool isAdd = false;
        private string aliasName;
        private ObservableCollection<PlayerCollectionItem> playersInAlias;

        public string AliasName
        {
            get { return aliasName; }
            set { SetProperty(ref aliasName, value); }
        }

        public ObservableCollection<PlayerCollectionItem> PlayersInAlias
        {
            get { return playersInAlias; }
            set
            {
                SetProperty(ref playersInAlias, value);
            }
        }

        public CollectionViewSource AllPlayersSorted { get; set; }
        public CollectionViewSource SelectedPlayersSorted { get; set; }

        private string _allSearchFilter = string.Empty;
        public string AllSearchFilter
        {
            get { return _allSearchFilter; }
            set
            {
                _allSearchFilter = value;

                if (!string.IsNullOrEmpty(AllSearchFilter))
                    AddFilterAll();

                AllPlayersSorted.View.Refresh();
            }
        }

        private string _selectedSearchFilter = string.Empty;
        public string SelectedSearchFilter
        {
            get { return _selectedSearchFilter; }
            set
            {
                _selectedSearchFilter = value;

                if (!string.IsNullOrEmpty(SelectedSearchFilter))
                    AddFilterSelected();

                SelectedPlayersSorted.View.Refresh();
            }
        }

        #endregion

        #region ICommands

        public ICommand OKCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand SelectCommand { get; set; }

        #endregion
    }
}
