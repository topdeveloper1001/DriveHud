//-----------------------------------------------------------------------
// <copyright file="SettingsSiteViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Application.ViewModels.Registration;
using DriveHUD.Common.Infrastructure.Base;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Utils;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model;
using Model.Settings;
using Model.Site;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
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
                { EnumPokerSites.Ignition, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.Ignition) },
                { EnumPokerSites.IPoker, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.IPoker) },
                { EnumPokerSites.BetOnline, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.BetOnline) },
                { EnumPokerSites.BlackChipPoker, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.BlackChipPoker) },
                { EnumPokerSites.PartyPoker, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.PartyPoker) },
                { EnumPokerSites.PokerStars, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.PokerStars) },
                { EnumPokerSites.Poker888, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.Poker888) },
                { EnumPokerSites.SportsBetting, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.SportsBetting) },
                { EnumPokerSites.TigerGaming, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.TigerGaming) },
                { EnumPokerSites.TruePoker, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.TruePoker) },
                { EnumPokerSites.YaPoker, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.YaPoker) },
                { EnumPokerSites.Horizon, CommonResourceManager.Instance.GetEnumResource(EnumPokerSites.Horizon) }
            };

            SelectedSiteViewModel = new SiteViewModel();

            AddonViewRequest = new InteractionRequest<INotification>();
        }

        public void InitializeCommands()
        {
            SelectDirectoryCommand = new RelayCommand(SelectDirectory);
            AddHandHistoryLocationCommand = new RelayCommand(AddHandHistoryLocation);
            DeleteHandHistoryLocationCommand = new RelayCommand(DeleteHandHistoryLocation);
            AutoDetectHandHistoryLocationCommand = new RelayCommand(AutoDetectHandHistoryLocation);
            EnableCommand = new RelayCommand(x => SelectedSite.Enabled = !SelectedSite.Enabled);
            AddonCommand = new RelayCommand(ShowAddon);
            HelpCommand = new RelayCommand(Help);
        }

        public override void SetSettingsModel(ISettingsBase model)
        {
            base.SetSettingsModel(model);

            UpdateSelectedSite(SelectedSiteType);
        }

        #region Properties

        private Dictionary<EnumPokerSites, string> pokerSitesDictionary;
        private Dictionary<EnumTableType, string> tableTypeDictionary;
        private EnumPokerSites selectedSiteType;
        private EnumTableType selectedTableType;
        private SiteModel selectedSite;
        private SiteViewModel siteViewModel;
        private string selectedHandHistoryLocation;
        private bool isPreferredSeatingVisible;
        private bool isAutoCenterVisible;
        private bool fastPokerVisible;
        private bool isHandHistoryLocationRequired;
        private bool isAddonButtonVisible;
        private string addonText;
        private string addonTooltip;

        public InteractionRequest<INotification> AddonViewRequest { get; private set; }

        public Dictionary<EnumPokerSites, string> PokerSitesDictionary
        {
            get { return pokerSitesDictionary; }
            set
            {
                SetProperty(ref pokerSitesDictionary, value);
            }
        }

        public Dictionary<EnumTableType, string> TableTypeDictionary
        {
            get { return tableTypeDictionary; }
            set
            {
                SetProperty(ref tableTypeDictionary, value);
            }
        }

        public EnumPokerSites SelectedSiteType
        {
            get { return selectedSiteType; }
            set
            {
                UpdateTableTypeDictionary(value);
                UpdateSelectedSite(value);

                var siteConfiguration = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(value);

                IsPreferredSeatingVisible = siteConfiguration.IsPrefferedSeatsAllowed;
                IsHandHistoryLocationRequired = siteConfiguration.IsHandHistoryLocationRequired;
                IsAddonButtonVisible = siteConfiguration.IsAddon;
                IsAutoCenterVisible = siteConfiguration.IsAutoCenterAllowed;
                FastPokerVisible = siteConfiguration.FastPokerAllowed;
                FastPokerModeName = siteConfiguration.FastPokerModeName;
                AddonText = siteConfiguration.AddonText;
                AddonTooltip = siteConfiguration.AddonTooltip;

                SetProperty(ref selectedSiteType, value);
            }
        }

        public EnumTableType SelectedTableType
        {
            get { return selectedTableType; }
            set
            {
                SetProperty(ref selectedTableType, value);
                SetCurrentSeatModel();
            }
        }

        public SiteModel SelectedSite
        {
            get { return selectedSite; }
            set
            {
                SetProperty(ref selectedSite, value);
                SetCurrentSeatModel();
            }
        }

        public SiteViewModel SelectedSiteViewModel
        {
            get { return siteViewModel; }
            set
            {
                SetProperty(ref siteViewModel, value);
            }
        }

        public string SelectedHandHistoryLocation
        {
            get { return selectedHandHistoryLocation; }
            set
            {
                SetProperty(ref selectedHandHistoryLocation, value);
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
            get
            {
                return isPreferredSeatingVisible;
            }
            set
            {
                SetProperty(ref isPreferredSeatingVisible, value);
            }

        }

        public bool IsAutoCenterVisible
        {
            get
            {
                return isAutoCenterVisible;
            }
            set
            {
                SetProperty(ref isAutoCenterVisible, value);
            }

        }

        public bool FastPokerVisible
        {
            get
            {
                return fastPokerVisible;
            }
            set
            {
                SetProperty(ref fastPokerVisible, value);
            }
        }

        private string fastPokerModeName;

        public string FastPokerModeName
        {
            get
            {
                return fastPokerModeName;
            }
            set
            {
                SetProperty(ref fastPokerModeName, value);
            }
        }

        public bool IsHandHistoryLocationRequired
        {
            get
            {
                return isHandHistoryLocationRequired;
            }
            set
            {
                SetProperty(ref isHandHistoryLocationRequired, value);
            }
        }

        public string AddonText
        {
            get
            {
                return addonText;
            }
            set
            {
                SetProperty(ref addonText, value);
            }
        }

        public string AddonTooltip
        {
            get
            {
                return addonTooltip;
            }
            set
            {
                SetProperty(ref addonTooltip, value);
            }
        }

        public bool IsAddonButtonVisible
        {
            get
            {
                return isAddonButtonVisible;
            }
            set
            {
                SetProperty(ref isAddonButtonVisible, value);
            }
        }

        #endregion

        #region ICommand

        public ICommand SelectDirectoryCommand { get; set; }

        public ICommand AddHandHistoryLocationCommand { get; set; }

        public ICommand DeleteHandHistoryLocationCommand { get; set; }

        public ICommand AutoDetectHandHistoryLocationCommand { get; set; }

        public ICommand EnableCommand { get; set; }

        public ICommand AddonCommand { get; set; }

        public ICommand HelpCommand { get; set; }

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

        private void Help(object obj)
        {
            if (SelectedSite == null || !ResourceStrings.PokerSiteHelpLinks.ContainsKey(SelectedSite.PokerSite))
            {
                return;
            }

            var helpLinkKey = ResourceStrings.PokerSiteHelpLinks[SelectedSite.PokerSite];
            var helpLink = CommonResourceManager.Instance.GetResourceString(helpLinkKey);

            BrowserHelper.OpenLinkInBrowser(helpLink);
        }

        private void ShowAddon(object obj)
        {
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