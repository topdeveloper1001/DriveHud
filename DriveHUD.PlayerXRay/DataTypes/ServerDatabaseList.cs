#region Usings

using System.Collections.ObjectModel;
using System.Collections.Specialized;

#endregion

namespace DriveHUD.PlayerXRay.DataTypes
{
    public sealed class ServerDatabaseList : ObservableCollection<ServerDatabase>
    {
        public ServerDatabaseList()
        {
            CollectionChanged += ServerDatabaseListCollectionChanged;
        }

        private void ServerDatabaseListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            foreach (ServerDatabase db in e.NewItems)
            {
                db.DatabaseSelected += DbDatabaseSelected;
            }
        }

        private void DbDatabaseSelected(ServerDatabase db)
        {
            foreach (ServerDatabase database in this)
            {
                if (db == database)
                    continue;
                database.Unselect();
            }
        }
    }
}