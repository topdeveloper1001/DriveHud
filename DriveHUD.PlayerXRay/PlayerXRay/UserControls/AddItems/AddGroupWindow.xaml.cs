using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataTypes.NotesTreeObjects;
using AcePokerSolutions.PlayerXRay.CustomControls;
using Microsoft.Win32;
using DriveHUD.Common.Log;

namespace AcePokerSolutions.PlayerXRay.UserControls.AddItems
{
    /// <summary>
    /// Interaction logic for AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow
    {
        private readonly StageObject m_stage;
        private InnerGroupObject m_innerGroup;

        public AddGroupWindow(StageObject stage)
        {
            m_stage = stage;
            InitializeComponent();
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
                m_innerGroup = (InnerGroupObject)Serializer.FromXml(xml, typeof(InnerGroupObject));
                txtName.Text = m_innerGroup.Name;
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

            InnerGroupObject group;

            if ((bool)rdoFromFile.IsChecked)
                group = m_innerGroup;
            else
                group = new InnerGroupObject();

            group.Name = txtName.Text;
            m_stage.InnerGroups.Add(group);
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
                    if (group.Name.ToLower() != txtName.Text.ToLower()) continue;

                    MessageBoxHelper.ShowWarningMessageBox("Group name already exists", this);
                    return false;
                }
            }
            return true;
        }
    }
}
