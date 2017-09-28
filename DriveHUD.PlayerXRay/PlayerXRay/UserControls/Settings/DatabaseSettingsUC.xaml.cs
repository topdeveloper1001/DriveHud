using System.Windows.Controls;
using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.ClientHelpers.HoldemManager;
using AcePokerSolutions.ClientHelpers.PokerTracker;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.DataTypes;
using System.Linq;

namespace AcePokerSolutions.PlayerXRay.UserControls.Settings
{
    /// <summary>
    /// Interaction logic for DatabaseSettingsUC.xaml
    /// </summary>
    public partial class DatabaseSettingsUC
    {
        private ServerDatabaseList m_databases;
        private string m_serverConnectionString;

        public bool CheckMandatory()
        {
            return m_databases.Count > 0 && m_databases.Where(p => p.IsSelected).Count() > 0;
        }

        public void Save()
        {
            NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType = cmb.SelectedIndex == 0
                                                                            ? ClientType.HoldemManager
                                                                            : ClientType.PokerTracker;
            NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName =
                m_databases.First(p => p.IsSelected).Name;
            NotesAppSettingsHelper.CurrentNotesAppSettings.ServerConnectionString = m_serverConnectionString;

            NotesAppSettingsHelper.SaveAppSettings();

            if (NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType == ClientType.HoldemManager)
            {
                HoldemManagerConfiguration config = new HoldemManagerConfiguration();
                config.LoadFromDefaultPath();

                string currentPlayerName = "";
                StaticStorage.CurrentPlayer = DAL.GetCurrentPayer(config.CurrentPlayerID, out currentPlayerName);
                StaticStorage.CurrentPlayerName = currentPlayerName;

            }
            else
            {
                PokerTrackerConfiguration config = new PokerTrackerConfiguration();
                config.LoadFromDefaultPath();
                string currentPlayerName = "";
                StaticStorage.CurrentPlayer = DAL.GetCurrentPayer(0, out currentPlayerName);
                StaticStorage.CurrentPlayerName = currentPlayerName;
            }
            StaticStorage.LoadStaticObjects();
        }

        public DatabaseSettingsUC()
        {
            Loaded += DatabaseSettingsUCLoaded;
            InitializeComponent();
        }

        void DatabaseSettingsUCLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            RefreshList(true);
        }

        private void RefreshList(bool firstStart)
        {
            if (m_databases != null)
                m_databases.Clear();

            if (firstStart)
            {
                cmb.SelectionChanged -= CmbSelectionChanged;

                cmb.SelectedIndex = NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType ==
                                    ClientType.HoldemManager
                                        ? 0
                                        : 1;
                cmb.SelectionChanged += CmbSelectionChanged;
            }
            if (cmb.SelectedIndex == 0)
            {
                HoldemManagerConfiguration config = new HoldemManagerConfiguration();
                config.LoadFromDefaultPath();
                //m_databases = PostgresServerHelper.RetrieveHoldemManagerDatabases(config.ConnectionString);
                m_serverConnectionString = config.ConnectionString;

                string dbName = string.Empty;

                if (NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType == ClientType.HoldemManager)
                {
                    dbName = NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName;
                }

                foreach (ServerDatabase db in m_databases)
                {
                    if (db.Name != dbName) continue;

                    db.IsSelected = true;
                    break;
                }
                if (m_databases.Where(p => p.IsSelected).Count() == 0)
                {
                    dbName = config.CurrentDatabase;

                    foreach (ServerDatabase db in m_databases)
                    {
                        if (db.Name != dbName) continue;

                        db.IsSelected = true;
                        break;
                    }
                }
            }
            else
            {
                PokerTrackerConfiguration config = new PokerTrackerConfiguration();
                config.LoadFromDefaultPath();
                //m_databases = PostgresServerHelper.RetrievePokerTrackerDatabases(config.ConnectionString);
                m_serverConnectionString = config.ConnectionString;

                string dbName = string.Empty;

                if (NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType == ClientType.PokerTracker)
                {
                    dbName = NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName;
                }

                foreach (ServerDatabase db in m_databases)
                {
                    if (db.Name != dbName) continue;

                    db.IsSelected = true;
                    break;
                }

                if (m_databases.Where(p => p.IsSelected).Count() == 0)
                {
                    dbName = config.CurrentDatabase;

                    foreach (ServerDatabase db in m_databases)
                    {
                        if (db.Name != dbName) continue;

                        db.IsSelected = true;
                        break;
                    }
                }
            }

            lst.ItemsSource = m_databases;
        }

        private void CmbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!IsInitialized)
                return;
            RefreshList(false);
        }
    }
}
