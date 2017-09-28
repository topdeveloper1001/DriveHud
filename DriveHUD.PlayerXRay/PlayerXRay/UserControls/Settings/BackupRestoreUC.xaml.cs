using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using AcePokerSolutions.BusinessHelper;
using AcePokerSolutions.BusinessHelper.ApplicationSettings;
using AcePokerSolutions.DataTypes.NotesTreeObjects;
using AcePokerSolutions.Helpers;
using AcePokerSolutions.PlayerXRay.CustomControls;
using Microsoft.Win32;
using DriveHUD.Common.Log;

namespace AcePokerSolutions.PlayerXRay.UserControls.Settings
{
    /// <summary>
    /// Interaction logic for BackupRestoreUC.xaml
    /// </summary>
    public partial class BackupRestoreUC
    {
        public BackupRestoreUC()
        {
            Initialized += BackupRestoreUCInitialized;
            InitializeComponent();
        }

        void BackupRestoreUCInitialized(object sender, EventArgs e)
        {
            lblLastBackup.Text = NotesAppSettingsHelper.CurrentNotesAppSettings.LastBackupDate != DateTime.MinValue
                                     ? NotesAppSettingsHelper.CurrentNotesAppSettings.LastBackupDate.ToString()
                                     : "Never";
        }

        private void BtnRestoreClick(object sender, RoutedEventArgs e)
        {
            if (!MessageBoxHelper.ShowYesNoDialogBox("Are you sure you wish to overwrite existing configuration? After the operation is completed the application will be restarted automatically", NotesAppSettingsHelper.MainWindow))
                return;

            OpenFileDialog diag = new OpenFileDialog
            {
                DefaultExt = ".pxb",
                Filter = "Player X-ray Backup Files (*.pxb)|*.pxb|All files (*.*)|*.*",
                RestoreDirectory = true,
                FilterIndex = 0
            };

            string path;

            if ((bool)diag.ShowDialog())
                path = diag.FileName;
            else
                return;

            bool result = true;
            try
            {
                string xml = File.ReadAllText(path);
                List<StageObject> backup = (List<StageObject>)Serializer.FromXml(xml, typeof(List<StageObject>));
                NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList = backup;
                NotesAppSettingsHelper.SaveAppSettings();
            }
            catch (Exception ex)
            {
                result = false;
                MessageBoxHelper.ShowInfoMessageBox("Could not restore configuration");
                LogProvider.Log.Error(this, "Could not restore configuration", ex);
            }

            if (!result) return;

            NotesAppSettingsHelper.MainWindow.Hide();
            Process p = new Process();
            ProcessStartInfo info = new ProcessStartInfo
            {
                FileName = AssemblyHelpers.GetRunningPath(Assembly.GetExecutingAssembly()) + "\\Player X-Ray.exe"
            };

            p.StartInfo = info;
            p.Start();
            Environment.Exit(0);
        }

        private void BtnBackupClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog diag = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".pxb",
                Filter = "Player X-ray Backup Files (*.pxb)|*.pxb|All files (*.*)|*.*",
                FilterIndex = 0,
                RestoreDirectory = true,
                FileName = string.Format("backup-{0}{1}{2}-{3}{4}",
                                                                   DateTime.Now.Year, DateTime.Now.Month,
                                                                   DateTime.Now.Day, DateTime.Now.Hour,
                                                                   DateTime.Now.Minute)
            };
            string path;

            if ((bool)diag.ShowDialog())
                path = diag.FileName;
            else
                return;

            string xml = Serializer.ToXml(NotesAppSettingsHelper.CurrentNotesAppSettings.StagesList,
                                          typeof(List<StageObject>));

            try
            {
                File.WriteAllText(path, xml);
                MessageBoxHelper.ShowInfoMessageBox("Backup saved successfully");
                NotesAppSettingsHelper.CurrentNotesAppSettings.LastBackupDate = DateTime.Now;
                lblLastBackup.Text = DateTime.Now.ToString();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowInfoMessageBox("Could not backup configuration");
                LogProvider.Log.Error(this, "Could not backup configuration", ex);
            }
        }
    }
}
