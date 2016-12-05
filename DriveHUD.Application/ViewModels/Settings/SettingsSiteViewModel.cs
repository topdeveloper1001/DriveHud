using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Enums;
using Model.Settings;
using Model.Site;
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
                { EnumPokerSites.AmericasCardroom, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.AmericasCardroom) },
                { EnumPokerSites.Ignition, "Bodog / Ignition" },
                { EnumPokerSites.BetOnline, "BetOnline" },
                { EnumPokerSites.PokerStars, "Pokerstars" },
                { EnumPokerSites.Poker888, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.Poker888) },
                { EnumPokerSites.TigerGaming, "Tigergaming" },
                { EnumPokerSites.SportsBetting, "Sportbetting.ag" },
            };

            SelectedSiteViewModel = new SiteViewModel();
        }

        public void InitializeCommands()
        {
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            AddHandHistoryLocationCommand = new RelayCommand(AddHandHistoryLocation);
            DeleteHandHistoryLocationCommand = new RelayCommand(DeleteHandHistoryLocation);
            AutoDetectHandHistoryLocationCommand = new RelayCommand(AutoDetectHandHistoryLocation);
        }

        public override void SetSettingsModel(ISettingsBase model)
        {
            base.SetSettingsModel(model);

            UpdateSelectedSite(SelectedSiteType);
        }

        #region Properties

        private Dictionary<EnumPokerSites, string> _pokerSitesDictionary;
        private Dictionary<EnumTableType, string> _tableTypeDictionary;
        private EnumPokerSites _selectedSiteType;
        private EnumTableType _selectedTableType;
        private SiteModel _selectedSite;
        private SiteViewModel _siteViewModel;
        private string _selectedHandHistoryLocation;
        private bool _isPreferredSeatingVisible;

        public Dictionary<EnumPokerSites, string> PokerSitesDictionary
        {
            get { return _pokerSitesDictionary; }
            set
            {
                SetProperty(ref _pokerSitesDictionary, value);
            }
        }

        public Dictionary<EnumTableType, string> TableTypeDictionary
        {
            get { return _tableTypeDictionary; }
            set
            {
                SetProperty(ref _tableTypeDictionary, value);
            }
        }

        public EnumPokerSites SelectedSiteType
        {
            get { return _selectedSiteType; }
            set
            {
                UpdateTableTypeDictionary(value);
                UpdateSelectedSite(value);
                IsPreferredSeatingVisible = (value != EnumPokerSites.BetOnline 
                    && value != EnumPokerSites.SportsBetting 
                    && value != EnumPokerSites.TigerGaming);

                SetProperty(ref _selectedSiteType, value);
            }
        }

        public EnumTableType SelectedTableType
        {
            get { return _selectedTableType; }
            set
            {
                SetProperty(ref _selectedTableType, value);
                SetCurrentSeatModel();
            }
        }

        public SiteModel SelectedSite
        {
            get { return _selectedSite; }
            set
            {
                SetProperty(ref _selectedSite, value);
                SetCurrentSeatModel();
            }
        }

        public SiteViewModel SelectedSiteViewModel
        {
            get { return _siteViewModel; }
            set
            {
                SetProperty(ref _siteViewModel, value);
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
                return SettingsModel?.IsProcessedDataLocationEnabled ?? false;
            }
            set
            {
                if (SettingsModel != null && SettingsModel.IsProcessedDataLocationEnabled != value)
                {
                    SettingsModel.IsProcessedDataLocationEnabled = value;
                    OnPropertyChanged(nameof(IsCustomProcessedDataLocationEnabled));
                }
            }
        }

        public string CustomProcessedDataLocation
        {
            get
            {
                return SettingsModel?.ProcessedDataLocation ?? StringFormatter.GetProcessedDataFolderPath();
            }

            set
            {
                if (SettingsModel != null && SettingsModel.ProcessedDataLocation != value)
                {
                    SettingsModel.ProcessedDataLocation = value;
                    OnPropertyChanged(nameof(CustomProcessedDataLocation));
                }
            }
        }

        public bool IsPreferredSeatingVisible
        {
            get { return _isPreferredSeatingVisible; }
            set
            {
                SetProperty(ref _isPreferredSeatingVisible, value);
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

        private void UpdateTableTypeDictionary(EnumPokerSites pokerSite)
        {
            try
            {
                var configuration = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(pokerSite);
                TableTypeDictionary = configuration.TableTypes.ToDictionary(x => x, x => GetTableTypeString(x));
                SelectedTableType = TableTypeDictionary.FirstOrDefault().Key;
            }
            catch (Exception ex)
            {
                LogProvider.Log.Error(this, $"Failed to load configuration for {pokerSite}.", ex);
            }
        }

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

                if (!SelectedSite.HandHistoryLocationList.Any(x => x == dialog.SelectedPath))
                {
                    SelectedSite.HandHistoryLocationList.Add(dialog.SelectedPath);
                }
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
            var folders = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(SelectedSiteType).GetHandHistoryFolders();
            SelectedSite.HandHistoryLocationList.AddRange(folders.Except(SelectedSite.HandHistoryLocationList));
        }

        private string GetTableTypeString(EnumTableType tableType)
        {
            return $"{(byte)tableType}-max";
        }

        private void SetCurrentSeatModel()
        {
            if (SelectedSite == null)
            {
                return;
            }

            var seatModel = SelectedSite.PrefferedSeats.FirstOrDefault(x => x.TableType == SelectedTableType);

            if (seatModel == null)
            {
                seatModel = new PreferredSeatModel() { IsPreferredSeatEnabled = false, PreferredSeat = -1, TableType = SelectedTableType };
                SelectedSite.PrefferedSeats.Add(seatModel);
            }

            SelectedSiteViewModel.SelectedSeatModel = seatModel;
        }

        private void UpdateSelectedSite(EnumPokerSites pokerSite)
        {
            SelectedSite = SettingsModel?.SitesModelList.FirstOrDefault(x => x.PokerSite == pokerSite);
        }

        #endregion
    }

    public class SiteViewModel : BaseViewModel
    {
        private PreferredSeatModel _selectedSeatModel;

        public PreferredSeatModel SelectedSeatModel
        {
            get
            {
                return _selectedSeatModel;
            }
            set
            {
                SetProperty(ref _selectedSeatModel, value);
            }
        }

        public void RaisePropertyChanged()
        {
            OnPropertyChanged(nameof(SelectedSeatModel));
        }
    }

}
