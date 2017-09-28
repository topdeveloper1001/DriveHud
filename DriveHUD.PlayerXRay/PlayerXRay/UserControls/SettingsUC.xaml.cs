using System;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataAccessHelper;
using AcePokerSolutions.PlayerXRay.CustomControls;
using AcePokerSolutions.PlayerXRay.UserControls.Settings;

namespace AcePokerSolutions.PlayerXRay.UserControls
{
    /// <summary>
    /// Interaction logic for SettingsUC.xaml
    /// </summary>
    public partial class SettingsUC
    {
        private DatabaseSettingsUC m_databaseSettingsUC;
        private GeneralSettingsUC m_generalSettingsUC;
        private BackupRestoreUC m_backup;

        public SettingsUC()
        {
            Initialized += SettingsUCInitialized;
            InitializeComponent();
        }

        void SettingsUCInitialized(object sender, EventArgs e)
        {
            m_databaseSettingsUC = new DatabaseSettingsUC();
            panel.Content = m_databaseSettingsUC;
        }

        private void BtnDatabaseClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (m_databaseSettingsUC == null)
                m_databaseSettingsUC = new DatabaseSettingsUC();
            panel.Content = m_databaseSettingsUC;
            btnSave.Visibility = System.Windows.Visibility.Visible;
        }

        private void BtnGeneralClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (m_generalSettingsUC == null)
                m_generalSettingsUC = new GeneralSettingsUC();
            panel.Content = m_generalSettingsUC;
            btnSave.Visibility = System.Windows.Visibility.Visible;
        }

        private void BtnSaveClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (!m_databaseSettingsUC.CheckMandatory())
        	{
        	    MessageBoxHelper.ShowWarningMessageBox("Please select a database from the list");
        	    return;
        	}

            m_databaseSettingsUC.Save();

            //if (!PostgresServerHelper.CheckConnection(NotesAppSettingsHelper.CurrentNotesAppSettings.ServerConnectionString,
            //    NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName))
            //{
            //    MessageBoxHelper.ShowWarningMessageBox("The database connection is unavailable. Please select a database");
            //}
            //else
            //{
            //    ((MainWindow)NotesAppSettingsHelper.MainWindow).SetWindowTitle();
            //    MessageBoxHelper.ShowInfoMessageBox("Settings saved successfully");
            //}
        }

        private void BtnBackupClick(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (m_backup == null)
                m_backup = new BackupRestoreUC();
            panel.Content = m_backup;
            btnSave.Visibility = System.Windows.Visibility.Hidden;
        }
    }
}
