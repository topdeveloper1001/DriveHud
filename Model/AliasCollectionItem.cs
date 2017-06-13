using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class AliasCollectionItem : BindableBase, IPlayer
    {
        #region Fields

        private string _name = string.Empty;
        private ObservableCollection<PlayerCollectionItem> _playersInAlias = new ObservableCollection<PlayerCollectionItem>();

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;

                OnPropertyChanged(propertyName: "Name");
                OnPropertyChanged(propertyName: "DecodedName");
            }
        }

        public string DecodedName { get { return Name; } }

        public int PlayerId { get; set; }

        public EnumPokerSites PokerSite { get { return EnumPokerSites.Alias; } }

        public string Description
        {
            get { return $"Players: {_playersInAlias.Count}."; }
        }

        public ObservableCollection<PlayerCollectionItem> PlayersInAlias
        {
            get { return _playersInAlias; }

            set
            {
                _playersInAlias = value;
                OnPropertyChanged(propertyName: "Description");
            }
        }

        #endregion

        #region Methods

        public string ConvertPlayersToString()
        {
            return string.Join(",", PlayersInAlias.Select(x => x.PlayerId));
        }

        public static ObservableCollection<PlayerCollectionItem> ConvertFromDB(string encodedPlayers)
        {
            if (string.IsNullOrEmpty(encodedPlayers))
                return null;

            var result = new ObservableCollection<PlayerCollectionItem>();
            var allPlayers = ServiceLocator.Current.TryResolve<SingletonStorageModel>().PlayerCollection.Where(x => x is PlayerCollectionItem).Select(x => x as PlayerCollectionItem);

            foreach (int id in encodedPlayers.Split(',').Select(ch => Convert.ToInt32(ch)))
                result.Add(allPlayers.Where(x => x.PlayerId == id).FirstOrDefault());

            return result;
        }

        #endregion
    }
}
