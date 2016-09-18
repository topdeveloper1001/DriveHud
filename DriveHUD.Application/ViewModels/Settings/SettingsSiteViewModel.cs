using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace DriveHUD.Application.ViewModels.Settings
{
    public class SettingsSiteViewModel : SettingsViewModel<SiteSettingsModel>
    {
        public SettingsSiteViewModel(string name) : base(name)
        {
            Initialize();
        }

        public void Initialize()
        {
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
        }

        #region Properties

        public bool IsCustomProcessedDataLocationEnabled
        {
            get
            {
                return SettingsModel?.IsCustomProcessedDataLocationEnabled ?? false;
            }
            set
            {
                if (SettingsModel != null && SettingsModel.IsCustomProcessedDataLocationEnabled != value)
                {
                    SettingsModel.IsCustomProcessedDataLocationEnabled = value;
                    OnPropertyChanged(nameof(IsCustomProcessedDataLocationEnabled));
                }
            }
        }

        public string CustomProcessedDataLocation
        {
            get
            {
                return SettingsModel?.CustomProcessedDataLocation ?? StringFormatter.GetAppDataFolderPath();
            }

            set
            {
                if (SettingsModel != null && SettingsModel.CustomProcessedDataLocation != value)
                {
                    SettingsModel.CustomProcessedDataLocation = value;
                    OnPropertyChanged(nameof(CustomProcessedDataLocation));
                }
            }
        }

        #endregion

        #region ICommand

        public ICommand SelectDirectoryCommand { get; set; }

        #endregion

        #region Infrastructure

        /// <summary>
        /// Checks if it is possible to write to the directory specified
        /// </summary>
        /// <param name="dirPath"></param>
        /// <param name="throwIfFails"></param>
        /// <returns></returns>
        private bool IsDirectoryWritable(string dirPath, bool throwIfFails = false)
        {
            try
            {
                using (FileStream fs = File.Create(Path.Combine(dirPath, Path.GetRandomFileName()), 1, FileOptions.DeleteOnClose))
                { }
                return true;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Can't write to the directory {dirPath}", ex);

                if (throwIfFails)
                    throw;
                else
                    return false;
            }
        }


        private void SelectDirectory(object obj)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            var result = dialog.ShowDialog(new Form { TopMost = true });

            if (result == DialogResult.OK)
            {
                if (IsDirectoryWritable(dialog.SelectedPath))
                {
                    CustomProcessedDataLocation = dialog.SelectedPath;
                }
                else
                {
                    System.Windows.MessageBox.Show("Unable to write to the directory specified");
                }
            }

        }

        #endregion

    }
}
