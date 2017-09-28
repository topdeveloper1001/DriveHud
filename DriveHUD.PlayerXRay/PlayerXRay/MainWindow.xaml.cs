using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.ClientHelpers.DriveHUD;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.Helpers;
using AcePokerSolutions.PlayerXRay.CustomControls;
using AcePokerSolutions.PlayerXRay.UserControls;
using AcePokerSolutions.PlayerXRay.UserControls.Notes;
using AcePokerSolutions.UIControls.CustomControls;

namespace AcePokerSolutions.PlayerXRay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private bool m_debugShown;

        private RunPage m_runPage;
        private NotesPage m_notesPage;
        private SettingsUC m_settingsUC;
        private ProfilesUC m_profilesUC;
        private DebugModeUC m_debugUC;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            NotesAppSettingsHelper.MainWindow = this;
            Hide();

            bool result = WcfHelper.CheckConnection();

            if (!result)
            {
                DriveHudConfiguration dhConfig = new DriveHudConfiguration();

                bool dhInstalled = dhConfig.CheckDhInstalled();

                if (!dhInstalled)
                {
                    MessageBoxHelper.ShowErrorMessageBox("Player X-ray could not find DriveHUD. Application will now exit", this);
                    Environment.Exit(0);
                }

                NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType = ClientType.DriveHUD;
                NotesAppSettingsHelper.SaveAppSettings();

            }

            NotesAppSettingsHelper.MainWindow = this;
            DAL.DalError += DALDalError;
            Show();


            string playerName = "";
            StaticStorage.CurrentPlayer = DAL.GetCurrentPayer(0, out playerName);
            StaticStorage.CurrentPlayerName = playerName;

            BackgroundWorker loadStaticObjectsWorker = new BackgroundWorker();

            Closed += MainWindowClosed;
            InitializeMenu();
            SetWindowTitle();
            ShowOverlayGrid("Retrieving existing Stakes");

            loadStaticObjectsWorker.DoWork += LoadStaticObjectsWorkerDoWork;
            loadStaticObjectsWorker.RunWorkerCompleted += LoadStaticObjectsWorkerRunWorkerCompleted;
            loadStaticObjectsWorker.RunWorkerAsync();
        }

        void LoadStaticObjectsWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HideOverlayGrid();
        }

        static void LoadStaticObjectsWorkerDoWork(object sender, DoWorkEventArgs e)
        {
            StaticStorage.LoadStaticObjects();
        }

        private void ShowOverlayGrid(string message)
        {
            gridOverlay.Visibility = Visibility.Visible;
            lblStatus.Text = message;
        }

        private void HideOverlayGrid()
        {
            gridOverlay.Visibility = Visibility.Hidden;
        }

        static void DALDalError(string message, bool fatal)
        {
            MessageBoxHelper.ShowErrorMessageBox(message, NotesAppSettingsHelper.MainWindow);
            if (fatal)
            {
                Environment.Exit(0);
            }
        }

        void MainWindowClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void InitializeMenu()
        {
            mainMenu.AddMenuItem(MainMenuType.Run);
            mainMenu.AddMenuItem(MainMenuType.Notes);
            mainMenu.AddMenuItem(MainMenuType.Profiles);
            mainMenu.AddMenuItem(MainMenuType.Settings);
            //mainMenu.AddMenuItem(MainMenuType.HUD);
            mainMenu.AddMenuItem(MainMenuType.Help);

            mainMenu.MainMenuChanged += MainMenuMainMenuChanged;
            mainMenu.SelectMenu(MainMenuType.Run);
        }

        void MainMenuMainMenuChanged(MainMenuType selectedMenuType)
        {
            if (panel.Content is NotesPage)
            {
                NotesPage page = (NotesPage)panel.Content;
                page.CheckForChanges();
            }
            if (panel.Content is ProfilesUC)
            {
                ProfilesUC page = (ProfilesUC)panel.Content;
                page.CheckForChanges();
            }

            switch (selectedMenuType)
            {
                case MainMenuType.Run:
                    if (m_runPage == null)
                        m_runPage = new RunPage();
                    panel.Content = m_runPage;
                    break;
                case MainMenuType.Notes:
                    if (m_notesPage == null)
                        m_notesPage = new NotesPage();
                    else
                        m_notesPage.ClearPanel();
                    panel.Content = m_notesPage;
                    break;
                case MainMenuType.Settings:
                    if (m_settingsUC == null)
                        m_settingsUC = new SettingsUC();
                    panel.Content = m_settingsUC;
                    break;
                case MainMenuType.Profiles:
                    if (m_profilesUC == null)
                        m_profilesUC = new ProfilesUC();
                    panel.Content = m_profilesUC;
                    break;
                case MainMenuType.Debug:
                    if (m_debugUC == null)
                        m_debugUC = new DebugModeUC();
                    panel.Content = m_debugUC;
                    break;
                default:
                    break;
            }
        }

        private void ExitDialogDelegate(ExitDialog.ExitDialogResult result, CancelEventArgs e)
        {
            if (result != ExitDialog.ExitDialogResult.Cancel)
            {
                if (panel.Content is NotesPage)
                {
                    NotesPage page = (NotesPage)panel.Content;
                    page.CheckForChanges();
                }
                if (panel.Content is ProfilesUC)
                {
                    ProfilesUC page = (ProfilesUC)panel.Content;
                    page.CheckForChanges();
                }
            }

            switch (result)
            {
                case ExitDialog.ExitDialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case ExitDialog.ExitDialogResult.Quit:
                    Environment.Exit(0);
                    break;
                case ExitDialog.ExitDialogResult.Restart:
                    RestartApp();
                    break;
            }
        }

        public void SetWindowTitle()
        {
            lblClient.Text = NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType.ToString();
            //lblDatabase.Text = NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName;
            lblBuildDate.Text = AssemblyHelpers.DateCompiled(Assembly.GetExecutingAssembly());
            lblPlayer.Text = StaticStorage.CurrentPlayerName;
            lblVersion.Text = AssemblyHelpers.GetStringVersion(Assembly.GetExecutingAssembly());
        }

        private void RestartApp()
        {
            Hide();
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = AssemblyHelpers.GetRunningPath(Assembly.GetExecutingAssembly()) + "\\Player X-Ray.exe"
            };

            p.StartInfo = info;
            p.Start();
            Environment.Exit(0);
        }

        private void WindowClosing(object sender, CancelEventArgs e)
        {

            ExitDialog diag = new ExitDialog(e)
            {
                Owner =
                                          this
            };
            diag.ExitDialogEvent += ExitDialogDelegate;
            diag.ShowDialog();
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftAlt) && !Keyboard.IsKeyDown(Key.RightAlt)) return;

            if (!Keyboard.IsKeyDown(Key.D) || !Keyboard.IsKeyDown(Key.B)) return;

            if (!m_debugShown)
            {
                m_debugShown = true;
                if (mainMenu.ContainsMenu(MainMenuType.Debug))
                    mainMenu.ShowMenu(MainMenuType.Debug);
                else
                {
                    mainMenu.AddMenuItem(MainMenuType.Debug);
                }
            }
            else
            {
                m_debugShown = false;
                mainMenu.HideMenu(MainMenuType.Debug);
            }
        }
    }
}
