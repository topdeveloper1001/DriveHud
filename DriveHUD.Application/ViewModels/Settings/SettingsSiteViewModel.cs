using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Model;
using Model.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            InitializeCommands();
        }

        public void Initialize()
        {
            PokerSitesDictionary = new Dictionary<EnumPokerSites, string>()
            {
                { EnumPokerSites.Bovada, "Bodog / Ignition" },
                { EnumPokerSites.BetOnline, "BetOnline" },
                { EnumPokerSites.PokerStars, "Pokerstars" },
                { EnumPokerSites.TigerGaming, "Tigergaming" },
                { EnumPokerSites.SportsBetting, "Sportbetting.ag" }
            };
        }

        public void InitializeCommands()
        {
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            AddHandHistoryLocationCommand = new RelayCommand(AddHandHistoryLocation);
            DeleteHandHistoryLocationCommand = new RelayCommand(DeleteHandHistoryLocation);
            AutoDetectHandHistoryLocationCommand = new RelayCommand(AutoDetectHandHistoryLocation);
        }

        #region Properties

        private Dictionary<EnumPokerSites, string> _pokerSitesDictionary;
        private EnumPokerSites _selectedSiteType;
        private SiteModel _selectedSite;
        private string _selectedHandHistoryLocation;

        public Dictionary<EnumPokerSites, string> PokerSitesDictionary
        {
            get { return _pokerSitesDictionary; }
            set
            {
                SetProperty(ref _pokerSitesDictionary, value);
            }
        }

        public EnumPokerSites SelectedSiteType
        {
            get { return _selectedSiteType; }
            set
            {
                SelectedSite = SettingsModel?.SitesModelList.FirstOrDefault(x => x.PokerSite == value);
                SetProperty(ref _selectedSiteType, value);
            }
        }

        public SiteModel SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                SetProperty(ref _selectedSite, value);
            }
        }

        public string SelectedHandHistoryLocation
        {
            get { return _selectedHandHistoryLocation; }
            set
            {
                SetProperty(ref _selectedHandHistoryLocation, value);
            }
        }

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
        public ICommand AddHandHistoryLocationCommand { get; set; }
        public ICommand DeleteHandHistoryLocationCommand { get; set; }
        public ICommand AutoDetectHandHistoryLocationCommand { get; set; }

        #endregion

        #region Infrastructure

        private void SelectDirectory(object obj)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            var result = dialog.ShowDialog(new Form { TopMost = true });

            if (result == DialogResult.OK)
            {
                if (FileHelper.IsDirectoryWritable(dialog.SelectedPath))
                {
                    CustomProcessedDataLocation = dialog.SelectedPath;
                }
                else
                {
                    System.Windows.MessageBox.Show("Unable to write to the directory specified");
                }
            }

        }

        private void AddHandHistoryLocation(object obj)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            var result = dialog.ShowDialog(new Form { TopMost = true });

            if (result == DialogResult.OK)
            {
                if (SelectedSite.HandHistoryLocationList == null)
                {
                    SelectedSite.HandHistoryLocationList = new ObservableCollection<string>();
                }

                SelectedSite.HandHistoryLocationList.Add(dialog.SelectedPath);
            }
        }

        private void DeleteHandHistoryLocation(object obj)
        {
            if (SelectedHandHistoryLocation != null)
            {
                SelectedSite.HandHistoryLocationList.Remove(SelectedHandHistoryLocation);
            }
        }

        private void AutoDetectHandHistoryLocation(object obj)
        {

        }

        #endregion

    }
}
