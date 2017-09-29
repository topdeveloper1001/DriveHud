//-----------------------------------------------------------------------
// <copyright file="PlayerXRayMainView.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.ClientHelpers.DriveHUD;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.Helpers;
using AcePokerSolutions.PlayerXRay.CustomControls;
using AcePokerSolutions.PlayerXRay.UserControls;
using AcePokerSolutions.PlayerXRay.UserControls.Notes;
using AcePokerSolutions.UIControls.CustomControls;
using DriveHUD.Common.Wpf.Actions;
using Model;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DriveHUD.PlayerXRay
{
    /// <summary>
    /// Interaction logic for PlayerXRayMainView.xaml
    /// </summary>
    public partial class PlayerXRayMainView : UserControl, IViewModelContainer<PlayerXRayMainViewModel>, IViewContainer
    {
        private bool m_debugShown;

        private RunPage m_runPage;
        private NotesPage m_notesPage;
        private SettingsUC m_settingsUC;
        private ProfilesUC m_profilesUC;
        private DebugModeUC m_debugUC;

        public PlayerXRayMainView()
        {
            InitializeComponent();
            Loaded += PlayerXRayMainView_Loaded;
        }

        public PlayerXRayMainViewModel ViewModel
        {
            get
            {
                return DataContext as PlayerXRayMainViewModel;
            }
        }

        public ContentControl Window
        {
            get; set;
        }

        private void PlayerXRayMainView_Loaded(object sender, EventArgs e)
        {
            NotesAppSettingsHelper.LoadAppSettings();

            NotesAppSettingsHelper.MainWindow = Window;
            NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType = ClientType.DriveHUD;
            NotesAppSettingsHelper.SaveAppSettings();

            DAL.DalError += DALDalError;

            StaticStorage.CurrentPlayer = SingletonStorageModel.Instance.PlayerSelectedItem?.PlayerId.ToString();
            StaticStorage.CurrentPlayerName = SingletonStorageModel.Instance.PlayerSelectedItem?.Name;

            var loadStaticObjectsWorker = new BackgroundWorker();

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
            //StaticStorage.LoadStaticObjects();
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

        public void SetWindowTitle()
        {
            lblClient.Text = NotesAppSettingsHelper.CurrentNotesAppSettings.ClientType.ToString();
            //lblDatabase.Text = NotesAppSettingsHelper.CurrentNotesAppSettings.DatabaseName;
            lblBuildDate.Text = AssemblyHelpers.DateCompiled(Assembly.GetExecutingAssembly());
            lblPlayer.Text = StaticStorage.CurrentPlayerName;
            lblVersion.Text = AssemblyHelpers.GetStringVersion(Assembly.GetExecutingAssembly());
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