using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.CustomControls;

namespace DriveHUD.PlayerXRay.UserControls
{
    /// <summary>
    /// Interaction logic for ProfilesUC.xaml
    /// </summary>
    public partial class ProfilesUC
    {
        private ProfileObject m_selectedObject;
        private ObservableCollection<NoteObject> m_selectedNotesList = new ObservableCollection<NoteObject>();

        public ProfilesUC()
        {
            Loaded += ProfilesUCLoaded;
            Initialized += ProfilesUCInitialized;
            InitializeComponent();
        }

        void ProfilesUCLoaded(object sender, RoutedEventArgs e)
        {
            List<StageObject> stages = new List<StageObject>();
            stages.AddRange(NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList);
            treeNotes.ItemsSource = stages;
        }

        void ProfilesUCInitialized(object sender, EventArgs e)
        {
            list.ItemsSource = NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles;
        }

        private void EnableEditMode()
        {
            list.IsEnabled = false;
            panel.IsEnabled = false;
            stackButtons.Visibility = Visibility.Collapsed;
            stackAddEdit.Visibility = Visibility.Visible;
        }

        private void DisableEditMode()
        {
            list.IsEnabled = true;
            panel.IsEnabled = true;
            stackButtons.Visibility = Visibility.Visible;
            stackAddEdit.Visibility = Visibility.Collapsed;
        }

        private void BtnAddMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            m_selectedObject = (ProfileObject) list.SelectedItem;

            m_selectedObject = new ProfileObject();
            EnableEditMode();
            txtName.Text = "";
            return;
        }

        private void BtnEditMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (list.SelectedItem == null)
                return;

            m_selectedObject = (ProfileObject) list.SelectedItem;
            txtName.Text = m_selectedObject.Name;

            EnableEditMode();
            return;
        }

        private void BtnDeleteMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!MessageBoxHelper.ShowYesNoDialogBox("Are you sure you want to delete the selected profile?", NotesAppSettingsHelper.MainWindow))
                return;

            NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles.Remove(m_selectedObject);

            NotesAppSettingsHelper.SaveAppSettings();
            list.Items.Refresh();
        }

        private void TxtNameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                BtnSaveMouseLeftButtonUp(null, null);
            if (e.Key == Key.Escape)
                BtnCloseMouseLeftButtonUp(null, null);
        }

        private void BtnCloseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisableEditMode();
        }

        private void BtnSaveMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisableEditMode();

            m_selectedObject.Name = txtName.Text;

            if (!NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles.Contains(m_selectedObject))
            {
                NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles.Add(m_selectedObject);
            }

            NotesAppSettingsHelper.SaveAppSettings();
            list.Items.Refresh();
        }

        public void CheckForChanges()
        {
            if (m_selectedObject == null)
                return;

            List<int> selectedNotes = new List<int>();

            foreach (NoteObject note in m_selectedNotesList)
            {
                selectedNotes.Add(note.ID);
            }

            if (selectedNotes.Count != m_selectedObject.ContainingNotes.Count)
                goto ChangesExits;

            foreach (int value in selectedNotes)
            {
                if (!m_selectedObject.ContainingNotes.Contains(value))
                    goto ChangesExits;
            }

            foreach (int value in m_selectedObject.ContainingNotes)
            {
                if (!selectedNotes.Contains(value))
                    goto ChangesExits;
            }

            return;

            ChangesExits:
            if (!MessageBoxHelper.ShowYesNoDialogBox("Do you want to save pending changes?", NotesAppSettingsHelper.MainWindow))
                return;

            m_selectedObject.ContainingNotes = selectedNotes;
            NotesAppSettingsHelper.SaveAppSettings();
        }

        private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem == null)
            {
                panel.Visibility = Visibility.Hidden;
                return;
            }

            panel.Visibility = Visibility.Visible;
            CheckForChanges();
            btnAdd.Visibility = Visibility.Collapsed;


            btnAdd.Visibility = Visibility.Visible;
            btnDelete.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Visible;

            btnAdd.ContextMenu = null;
            btnDelete.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Visible;
            m_selectedObject = (ProfileObject) list.SelectedItem;
            FillInfo();
        }

        private void FillInfo()
        {
            m_selectedNotesList = new ObservableCollection<NoteObject>();

            foreach (NoteObject note in NotesAppSettingsHelper.CurrentNotesAppSettings.AllNotes)
            {
                if (m_selectedObject.ContainingNotes.Contains(note.ID))
                    m_selectedNotesList.Add(note);
            }

            listSelectedNotes.ItemsSource = m_selectedNotesList;
            ICollectionView cv = CollectionViewSource.GetDefaultView(m_selectedNotesList);
            if (cv.SortDescriptions.Count == 0)
            {
                cv.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            }
        }

        private void BtnAddToListMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (treeNotes.SelectedItem is StageObject)
            {
                StageObject stage = (StageObject)treeNotes.SelectedItem;

                foreach (NoteObject note in stage.Notes)
                {
                    if (!(m_selectedNotesList.Contains(note)))
                        m_selectedNotesList.Add(note);
                }

                foreach (InnerGroupObject group in stage.InnerGroups)
                {
                    foreach (NoteObject note in group.Notes)
                    {
                        if (!(m_selectedNotesList.Contains(note)))
                            m_selectedNotesList.Add(note);
                    }
                }
            }

            if (treeNotes.SelectedItem is InnerGroupObject)
            {
                InnerGroupObject group = (InnerGroupObject)treeNotes.SelectedItem;

                foreach (NoteObject note in group.Notes)
                {
                    if (!(m_selectedNotesList.Contains(note)))
                        m_selectedNotesList.Add(note);
                }
            }

            if (treeNotes.SelectedItem is NoteObject)
            {
                NoteObject note = (NoteObject)treeNotes.SelectedItem;
                if (!(m_selectedNotesList.Contains(note)))
                    m_selectedNotesList.Add(note);
            }

            listSelectedNotes.Items.Refresh();
        }

        private void BtnRemoveFromListMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (listSelectedNotes.SelectedItem == null) return;

            for (int index = 0; index < listSelectedNotes.SelectedItems.Count; index++)
            {
                object obj = listSelectedNotes.SelectedItems[index];
                m_selectedNotesList.Remove(obj as NoteObject);
                index--;
            }
        }
    }
}
