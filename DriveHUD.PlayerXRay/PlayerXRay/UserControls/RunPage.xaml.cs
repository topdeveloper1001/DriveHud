using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataTypes;
using AcePokerSolutions.DataTypes.NotesTreeObjects;
using AcePokerSolutions.PlayerXRay.CustomControls;
using AcePokerSolutions.PlayerXRay.Helpers;

namespace AcePokerSolutions.PlayerXRay.UserControls
{
    /// <summary>
    /// Interaction logic for RunPage.xaml
    /// </summary>
    public partial class RunPage
    {
        private NoteStageType m_stageType;
        private NotesInsertManager m_manager;
        private WorkerState m_workerState;

        public RunPage()
        {
            Initialized += RunPageInitialized;
            Loaded += RunPage_Loaded;
            InitializeComponent();
        }

        void RunPage_Loaded(object sender, RoutedEventArgs e)
        {
            ListCollectionView notesView = new ListCollectionView(NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList);
            treeNotes.ItemsSource = notesView;
            treeNotes.Items.Filter = TreeFilter;
            ListCollectionView profilesView = new ListCollectionView(NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles);
            lstProfiles.ItemsSource = profilesView;
        }

        void RunPageInitialized(object sender, System.EventArgs e)
        {
            m_stageType = NoteStageType.PreFlop;
            imgLogo.Source = UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.PlayerXrayLogo);
        }

        private void RefreshTree()
        {
            treeNotes.Items.Refresh();
            treeNotes.Items.Filter = TreeFilter;
        }

        private bool TreeFilter(object obj)
        {
            StageObject stage = (StageObject)obj;
            return stage.StageType == m_stageType;
        }

        private void InitializeInsertManager(List<NoteObject> notes)
        {
            if (m_manager == null)
            {
                m_manager = new NotesInsertManager(notes);
                m_manager.ManagerMessageReceived +=
                    MManagerManagerMessageReceived;
                m_manager.ManagerStateChanged += MManagerManagerStateChanged;
                m_manager.ManagerProgressChanged += MManagerManagerProgressChanged;
            }
            else
                m_manager.Notes = notes;
        }

        void MManagerManagerProgressChanged(int percent)
        {
            progressBar.Value = percent;
        }

        void MManagerManagerStateChanged(WorkerState state)
        {
            m_workerState = state;
            Dispatcher.Invoke(new Action(delegate
                                             {
                                                 btnStop.IsEnabled = true;
                                                 btnPause.IsEnabled = true;
                                                 switch (state)
                                                 {
                                                     case WorkerState.Working:
                                                         stackOptions.IsEnabled = false;
                                                         gridNotes.IsEnabled = false;
                                                         gridProfiles.IsEnabled = false;
                                                         btnPause.Visibility = Visibility.Visible;
                                                         btnStop.Visibility = Visibility.Visible;
                                                         btnPause.Source =
                                                             UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.pause24);
                                                         break;
                                                     case WorkerState.Paused:
                                                         lblStatus.Text = "Paused";
                                                         btnPause.Source =
                                                             UIHelpers.GetBitmapSource(DriveHUD.PlayerXRay.Properties.Resources.play24);
                                                         break;
                                                     case WorkerState.Idle:
                                                         stackOptions.IsEnabled = true;
                                                         gridNotes.IsEnabled = true;
                                                         gridProfiles.IsEnabled = true;
                                                         btnPause.Visibility = Visibility.Collapsed;
                                                         btnStop.Visibility = Visibility.Collapsed;
                                                         break;
                                                 }
                                             }));
        }

        void MManagerManagerMessageReceived(string message, bool inWindow)
        {
            if (inWindow)
                MessageBoxHelper.ShowInfoMessageBox(message);
            else
                lblStatus.Text = message;
        }

        private void BtnTypeClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Tag.ToString())
            {
                case "PreFlop":
                    m_stageType = NoteStageType.PreFlop;
                    break;
                case "Flop":
                    m_stageType = NoteStageType.Flop;
                    break;
                case "Turn":
                    m_stageType = NoteStageType.Turn;
                    break;
                case "River":
                    m_stageType = NoteStageType.River;
                    break;
            }
            RefreshTree();
        }

        private void BtnStartClick(object sender, RoutedEventArgs e)
        {
            if (!Validate())
            {
                MessageBoxHelper.ShowWarningMessageBox("Please select an item from the list");
                return;
            }

            if (!MessageBoxHelper.ShowYesNoDialogBox("Are you sure you want to start adding notes?", NotesAppSettingsHelper.MainWindow))
                return;  
 
            if ((bool)rdoAllNotes.IsChecked)
                RunAllNotes();
            if ((bool)rdoByNote.IsChecked)
                RunByNote();
            if ((bool)rdoByProfile.IsChecked)
                RunByProfile();
        }

        private void RunByNote()
        {
            NoteObject note = (NoteObject) treeNotes.SelectedItem;
            InitializeInsertManager(new List<NoteObject>() {note});
            m_manager.StartWork();
        }

        private void RunByProfile()
        {
            List<NoteObject> notes = new List<NoteObject>();
            ProfileObject profile = (ProfileObject) lstProfiles.SelectedItem;

            foreach (int selectedNote in profile.ContainingNotes)
            {
                NoteObject note = NotesAppSettingsHelper.CurrentNotesAppSettings.AllNotes.Find(p => p.ID == selectedNote);
                if (note != null)
                    notes.Add(note);
            }

            InitializeInsertManager(notes);
            m_manager.StartWork();
        }

        private void RunAllNotes()
        {
            InitializeInsertManager(NotesAppSettingsHelper.CurrentNotesAppSettings.AllNotes);
            m_manager.StartWork();
        }

        private bool Validate()
        {
            if ((bool)rdoAllNotes.IsChecked)
                return true;

            if ((bool)rdoByNote.IsChecked)
                return treeNotes.SelectedItem is NoteObject;

            if ((bool)rdoByProfile.IsChecked)
                return lstProfiles.SelectedItem != null;

            return true;
        }

        private void RdoByNoteChecked(object sender, RoutedEventArgs e)
        {
            gridProfiles.Visibility = Visibility.Hidden;
            gridNotes.Visibility = Visibility.Visible;
        }

        private void RdoByProfileChecked(object sender, RoutedEventArgs e)
        {
            if (!IsInitialized)
                return;

            gridProfiles.Visibility = Visibility.Visible;
            gridNotes.Visibility = Visibility.Hidden;
        }

        private void RdoAllNotesChecked(object sender, RoutedEventArgs e)
        {
            gridProfiles.Visibility = Visibility.Hidden;
            gridNotes.Visibility = Visibility.Hidden;
        }

        private void btnPause_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (m_workerState == WorkerState.Paused)
                m_manager.Resume();
            else
            {
                btnStop.IsEnabled = false;
                btnPause.IsEnabled = false;
                lblStatus.Text = "Pausing";
                m_manager.Pause();
            }
        }

        private void btnStop_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!MessageBoxHelper.ShowYesNoDialogBox("Are you sure you want to stop the process?",
                NotesAppSettingsHelper.MainWindow))
                return;

            btnStop.IsEnabled = false;
            btnPause.IsEnabled = false;
            lblStatus.Text = "Cancelling";
            m_manager.Stop();
        }
    }

    
}
