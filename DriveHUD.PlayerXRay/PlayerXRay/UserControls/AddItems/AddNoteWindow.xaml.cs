using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using DriveHUD.PlayerXRay.BusinessHelper;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.CustomControls;
using Microsoft.Win32;
using DriveHUD.Common.Log;

namespace DriveHUD.PlayerXRay.UserControls.AddItems
{
    /// <summary>
    /// Interaction logic for AddNoteWindow.xaml
    /// </summary>
    public partial class AddNoteWindow
    {
        private readonly InnerGroupObject m_group;
        private readonly StageObject m_stage;
        private readonly bool m_stageParent;

        private NoteObject m_note;

        public AddNoteWindow()
        {
            InitializeComponent();
        }

        public AddNoteWindow(InnerGroupObject group)
            : this()
        {
            m_group = group;
            m_stageParent = false;
        }

        public AddNoteWindow(StageObject stage)
            : this()
        {
            m_stage = stage;
            m_stageParent = true;
        }

        private void RdoFromFileChecked(object sender, RoutedEventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog
            {
                AddExtension = true,
                DefaultExt = ".pxe",
                Filter = "Player X-ray Export Files (*.pxe)|*.pxe|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true
            };

            string path;

            if ((bool)diag.ShowDialog())
                path = diag.FileName;
            else
                return;

            try
            {
                string xml = File.ReadAllText(path);
                m_note = (NoteObject)Serializer.FromXml(xml, typeof(NoteObject));
                txtName.Text = m_note.Name;
            }
            catch (Exception ex)
            {
                rdoBlank.IsChecked = true;
                MessageBoxHelper.ShowErrorMessageBox("Could not read export file", this);
                LogProvider.Log.Error(this, "Could not read export file", ex);                
            }
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtName.Text))
                goto Error;

            if (!CheckNameExists())
                goto Error;

            txtName.BorderBrush = new SolidColorBrush(Colors.DarkGray);

            NoteObject note;

            if ((bool)rdoFromFile.IsChecked)
                note = m_note;
            else
            {
                if ((bool)rdoBlank.IsChecked)
                    note = new NoteObject();
                else
                    if (!(treeNotes.SelectedItem is NoteObject))
                    {
                        MessageBoxHelper.ShowWarningMessageBox("Please select a note from the list", this);
                        return;
                    }
                    else
                    {
                        string xml = Serializer.ToXml(treeNotes.SelectedItem, typeof (NoteObject));
                        note = (NoteObject)Serializer.FromXml(xml, typeof (NoteObject));
                    }
            }

            note.Name = txtName.Text;
            if (m_stageParent)
                m_stage.Notes.Add(note);
            else
                m_group.Notes.Add(note);

            if (note.ID == 0)
                note.ID = ObjectsHelper.GetNextID(NotesAppSettingsHelper.CurrentNotesAppSettings.AllNotes);

            NotesAppSettingsHelper.SaveAppSettings();
            DialogResult = true;
            Close();

        Error:
            txtName.BorderBrush = new SolidColorBrush(Colors.Red);
        }

        private bool CheckNameExists()
        {
            foreach (StageObject stage in NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList)
            {
                foreach (InnerGroupObject group in stage.InnerGroups)
                {
                    foreach (NoteObject note in group.Notes)
                    {
                        if (note.Name.ToLower() != txtName.Text.ToLower()) continue;
                        MessageBoxHelper.ShowWarningMessageBox("Note name already exists", this);
                        BringIntoView();
                        return false;
                    }
                }
                foreach (NoteObject note in stage.Notes)
                {
                    if (note.Name.ToLower() != txtName.Text.ToLower()) continue;
                    MessageBoxHelper.ShowWarningMessageBox("Note name already exists", this);
                    BringIntoView();
                    return false;
                }
            }
           
            return true;
        }

        private void RdoFromTemplateChecked(object sender, RoutedEventArgs e)
        {
            treeNotes.Visibility = Visibility.Visible;
        }

        private void RdoFromTemplateUnchecked(object sender, RoutedEventArgs e)
        {
            treeNotes.Visibility = Visibility.Collapsed;
        }

        private void WindowInitialized(object sender, EventArgs e)
        {
            List<StageObject> stages = new List<StageObject>();
            stages.AddRange(NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList);
            treeNotes.ItemsSource = stages;
        }
    }
}
