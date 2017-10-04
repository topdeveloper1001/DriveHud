using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.CustomControls;
using DriveHUD.PlayerXRay.UserControls.AddItems;
using Microsoft.Win32;
using DriveHUD.Common.Log;

namespace DriveHUD.PlayerXRay.UserControls.Notes
{
    /// <summary>
    /// Interaction logic for NotesPage.xaml
    /// </summary>
    public partial class NotesPage
    {
        private NoteStageType m_stageType;
        private NotesSettings m_notesSettingsUC;

        public void ClearPanel()
        {
            panel.Content = null;
            if (treeNotes.SelectedItem is NoteObject)
            {
                NoteObject note = (NoteObject) treeNotes.SelectedItem;
                if (note == null)
                    return;
                note.IsSelected = false;
            }
        }

        public NotesPage()
        {
            Initialized += NotesPageInitialized;
            InitializeComponent();
        }

        void NotesPageInitialized(object sender, EventArgs e)
        {
            m_stageType = NoteStageType.PreFlop;
            treeNotes.ItemsSource =
                NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList;
            treeNotes.Items.Filter = TreeFilter;
        }

        private void RefreshTree()
        {
            treeNotes.Items.Refresh();
            treeNotes.Items.Filter = TreeFilter;
        }

        private bool TreeFilter(object obj)
        {
            StageObject stage = (StageObject) obj;
            return stage.StageType == m_stageType;
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

        public void CheckForChanges()
        {
            if (!(panel.Content is NotesSettings))
                return;

            NotesSettings settings = (NotesSettings) panel.Content;
            bool windowShowed;

            if (!settings.PendingChanges())
            {
                if (settings.Note.Settings.SelectedFiltersComparison.Count == 0)
                {
                    
                        CheckComparison(out windowShowed);

                    if (windowShowed)
                        goto ShowWarning;
                }
                return;
            }
            if (!MessageBoxHelper.ShowYesNoDialogBox("Do you want to save pending changes?", NotesAppSettingsHelper.MainWindow))
                return;

            settings.SaveChanges();
            CheckComparison(out windowShowed);

            ShowWarning:
                if (settings.Note.Settings.SelectedFiltersComparison.Count == 0 &&
                   settings.Note.Settings.ExcludedCardsList.Count == 0)
                MessageBoxHelper.ShowWarningMessageBox("No comparison will be run for this note. You must either select a filter, or hole cards in order to create a comparison note");
        }

        private void CheckComparison(out bool windowShowed)
        {
            windowShowed = false;
            NotesSettings settings = (NotesSettings)panel.Content;

            if (settings.Note.Settings.SelectedFilters.Count > 0)
            {
                ComparisonFiltersWindow filters = new ComparisonFiltersWindow(settings.Note);
                filters.ShowDialog();
                windowShowed = true;
            }
            else
            {
                settings.Note.Settings.SelectedFiltersComparison.Clear();
            }

            NotesAppSettingsHelper.SaveAppSettings();
        }

        private void TreeSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (treeNotes.SelectedItem == null)
            {
                panel.Content = null;
                return;
            }
            CheckForChanges();
            btnExport.Visibility = Visibility.Collapsed;
            btnAdd.Visibility = Visibility.Collapsed;

            if (treeNotes.SelectedItem is StageObject)
            {
                btnAdd.ContextMenu = addContextMenu;
                btnAdd.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Collapsed;
                panel.Content = null;
                return;
            }
            btnAdd.ContextMenu = null;
            btnDelete.Visibility = Visibility.Visible;
            btnEdit.Visibility = Visibility.Visible;
            btnExport.Visibility = Visibility.Visible;

            if (treeNotes.SelectedItem is InnerGroupObject)
            {
                btnAdd.Visibility = Visibility.Visible;
                panel.Content = null;
                return;
            }
            
            NoteObject note = (NoteObject) treeNotes.SelectedItem;
            if (m_notesSettingsUC == null)
            {
                m_notesSettingsUC = new NotesSettings();
            }
            panel.Content = m_notesSettingsUC;
            m_notesSettingsUC.Initialize(note);
        }

        private void BtnCloseMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
           DisableEditMode();
        }

        private void EnableEditMode()
        {
            txtName.Text = "";
            treeNotes.IsEnabled = false;
            panel.IsEnabled = false;
            stackButtons.Visibility = Visibility.Collapsed;
            stackAddEdit.Visibility = Visibility.Visible;
        }

        private void DisableEditMode()
        {
            treeNotes.IsEnabled = true;
            panel.IsEnabled = true;
            stackButtons.Visibility = Visibility.Visible;
            stackAddEdit.Visibility = Visibility.Collapsed;
        }

        private void BtnAddMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!(treeNotes.SelectedItem is InnerGroupObject)) return;

            AddNoteWindow window = new AddNoteWindow(treeNotes.SelectedItem as InnerGroupObject);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                RefreshTree();
            }
        }

        private void BtnEditMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            EnableEditMode();
            if (treeNotes.SelectedItem is InnerGroupObject)
            {
                lblHeader.Text = "Group Name:";
                txtName.Text = ((InnerGroupObject) treeNotes.SelectedItem).Name;
                return;
            }

            if (!(treeNotes.SelectedItem is NoteObject)) return;
            lblHeader.Text = "Note Name:";
            txtName.Text = ((NoteObject)treeNotes.SelectedItem).Name;
            return;
        }

        private void BtnDeleteMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!MessageBoxHelper.ShowYesNoDialogBox("Are you sure you want to delete the selected object?", NotesAppSettingsHelper.MainWindow))
                return;

            foreach (StageObject stage in NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList)
            {
                foreach (InnerGroupObject group in stage.InnerGroups)
                {
                    if (group == treeNotes.SelectedItem)
                    {
                        stage.InnerGroups.Remove(group);
                        goto End;
                    }

                    foreach (NoteObject note in group.Notes)
                    {
                        if (note != treeNotes.SelectedItem) continue;
                        group.Notes.Remove(note);
                        goto End;
                    }
                }
                foreach (NoteObject note in stage.Notes)
                {
                    if (note != treeNotes.SelectedItem) continue;
                    stage.Notes.Remove(note);
                    goto End;
                }
            }

            End:
                NotesAppSettingsHelper.SaveAppSettings();
                RefreshTree();
        }

        private void TxtNameKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
                BtnSaveMouseLeftButtonUp(null, null);
            if (e.Key == Key.Escape)
                BtnCloseMouseLeftButtonUp(null, null);
        }

        private void BtnSaveMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            DisableEditMode();

            foreach (StageObject stage in NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList)
            {
                foreach (InnerGroupObject group in stage.InnerGroups)
                {
                    if (group == treeNotes.SelectedItem)
                    {
                        group.Name = txtName.Text;
                        goto End;
                    }

                    foreach (NoteObject note in group.Notes)
                    {
                        if (note != treeNotes.SelectedItem) continue;
                        note.Name = txtName.Text;
                        goto End;
                    }
                }
                foreach (NoteObject note in stage.Notes)
                {
                    if (note != treeNotes.SelectedItem) continue;
                    note.Name = txtName.Text;
                    goto End;
                }
            }

            End:
            NotesAppSettingsHelper.SaveAppSettings();
            RefreshTree();
        }

        private void BtnExportMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string xml = string.Empty;
            string name = string.Empty;
            string objectType = string.Empty;

            if (treeNotes.SelectedItem is NoteObject)
            {
                NoteObject note = (NoteObject)treeNotes.SelectedItem;
                xml = Serializer.ToXml(note, typeof(NoteObject));
                name = note.Name;
                objectType = "Note";
            }
            if (treeNotes.SelectedItem is InnerGroupObject)
            {
                InnerGroupObject group = (InnerGroupObject)treeNotes.SelectedItem;
                xml = Serializer.ToXml(group, typeof(InnerGroupObject));
                name = group.Name;
                objectType = "Group";
            }

            if (string.IsNullOrEmpty(xml))
                return;

            SaveFileDialog diag = new SaveFileDialog
                                      {
                                          AddExtension = true,
                                          DefaultExt = ".pxe",
                                          Filter = "Player X-ray Export Files (*.pxe)|*.pxe|All files (*.*)|*.*",
                                          FilterIndex = 0,
                                          RestoreDirectory = true,
                                          FileName = "export-" + name
                                      };
            string path;

            if ((bool)diag.ShowDialog())
                path = diag.FileName;
            else
                return;

            try
            {
                File.WriteAllText(path, xml);   
                MessageBoxHelper.ShowInfoMessageBox(objectType + " exported successfully");
            }
            catch(Exception ex)
            {
                MessageBoxHelper.ShowErrorMessageBox("Could not export the " + objectType + " to the selected destination");
                LogProvider.Log.Error(this, "Could not export to file", ex);                
            }
        }

        private void BtnAddMouseDown(object sender, MouseButtonEventArgs e)
        {
            Image image = sender as Image;
            if (image == null || image.ContextMenu == null)
                return;

            if (e.ChangedButton != MouseButton.Left || (treeNotes.SelectedItem is InnerGroupObject)) return;

            ContextMenu contextMenu = image.ContextMenu;
            contextMenu.PlacementTarget = image;
            contextMenu.IsOpen = true;
        }

        private void BtnAddGroupClick(object sender, RoutedEventArgs e)
        {
        	AddGroupWindow window = new AddGroupWindow(treeNotes.SelectedItem as StageObject);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                RefreshTree();
            }
        }

        private void BtnAddNoteClick(object sender, RoutedEventArgs e)
        {
        	AddNoteWindow window = new AddNoteWindow(treeNotes.SelectedItem as StageObject);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                RefreshTree();
            }
        }
    }
}
