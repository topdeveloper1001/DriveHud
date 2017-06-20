using DriveHUD.Common.Infrastructure.Base;
using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections;

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
            _infoViewModel = info;
            _newAlias = info?.Model;

            if (_newAlias == null)
            {
                _isAdd = true;
                _newAlias = new AliasCollectionItem();
            }

            this.AliasName = _newAlias?.Name ?? string.Empty;
            this.PlayersInAlias = new ObservableCollection<PlayerCollectionItem>(_newAlias?.PlayersInAlias);

            AllPlayers = new ObservableCollection<PlayerCollectionItem>(StorageModel.PlayerCollection.Except(this.PlayersInAlias).OfType<PlayerCollectionItem>());

            AllPlayersSorted.Source = AllPlayers;
            AllPlayersSorted.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            SelectedPlayersSorted.Source = PlayersInAlias;
            SelectedPlayersSorted.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
        }

        #region Methods

        private void SaveChanges()
        {
            _newAlias.Name = _aliasName;
            _newAlias.PlayersInAlias = _playersInAlias;

            if (_isAdd)
            {
                _infoViewModel?.Add(_newAlias);
            }
            else
            {
                _infoViewModel?.Save(_newAlias);
                _infoViewModel?.Close();
            }
        }

        private void Cancel()
        {
            _infoViewModel?.Close();
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
                    player.LinkedAliases.Add(_newAlias);
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
                    player.LinkedAliases.Remove(_newAlias);
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
                e.Accepted = false;
            else if (src.DecodedName != null && !src.DecodedName.ToLower().Contains(AllSearchFilter.ToLower()))
                e.Accepted = false;
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
                e.Accepted = false;
            else if (src.DecodedName != null && !src.DecodedName.ToLower().Contains(SelectedSearchFilter.ToLower()))
                e.Accepted = false;
        }

        #endregion

        #region Properties

        private int _allPlayersSelectdIndex = -1;
        private int _selectedPlayersSelectdIndex = -1;
        public int AllPlayersSelectdIndex
        {
            get { return _allPlayersSelectdIndex; }

            set { SetProperty(ref _allPlayersSelectdIndex, value); }
        }
        public int SelectedPlayersSelectdIndex
        {
            get { return _selectedPlayersSelectdIndex; }

            set { SetProperty(ref _selectedPlayersSelectdIndex, value); }
        }

        private ObservableCollection<PlayerCollectionItem> _allPlayers;
        public ObservableCollection<PlayerCollectionItem> AllPlayers
        {
            get { return _allPlayers; }

            set { SetProperty(ref _allPlayers, value); }
        }

        private AliasViewModelInfo<AliasCollectionItem> _infoViewModel;
        private AliasCollectionItem _newAlias;

        private bool _isAdd = false;
        private string _aliasName;
        private ObservableCollection<PlayerCollectionItem> _playersInAlias;

        public string AliasName
        {
            get { return _aliasName; }
            set { SetProperty(ref _aliasName, value); }
        }

        public ObservableCollection<PlayerCollectionItem> PlayersInAlias
        {
            get { return _playersInAlias; }
            set
            {
                SetProperty(ref _playersInAlias, value);
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
