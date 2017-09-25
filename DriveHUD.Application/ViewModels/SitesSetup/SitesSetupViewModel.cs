//-----------------------------------------------------------------------
// <copyright file="PopupContainerSitesSetupViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Linq;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Model.Site;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace DriveHUD.Application.ViewModels
{
    public class SitesSetupViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private SettingsModel settingsModel;

        private readonly IEnumerable<ISiteValidationResult> validationResults;

        public SitesSetupViewModel(IEnumerable<ISiteValidationResult> validationResults)
        {
            this.validationResults = validationResults;
            Initialize();
        }

        #region Properties

        private ObservableCollection<NetworkSetupViewModel> networkSetups;

        public ObservableCollection<NetworkSetupViewModel> NetworkSetups
        {
            get
            {
                return networkSetups;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref networkSetups, value);
            }
        }

        public bool DoNotShowForm
        {
            get
            {
                return settingsModel != null && settingsModel.GeneralSettings != null ? !settingsModel.GeneralSettings.RunSiteDetection : false;
            }
            set
            {
                if (settingsModel != null && settingsModel.GeneralSettings != null)
                {
                    settingsModel.GeneralSettings.RunSiteDetection = !value;
                }

                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public IReactiveCommand<object> ApplyCommand { get; private set; }

        #endregion

        #region Implementation of INotification & IInteractionRequestAware

        private Action finishInteraction;

        public Action FinishInteraction
        {
            get
            {
                return finishInteraction;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref finishInteraction, value);
            }
        }

        private INotification notification;

        public INotification Notification
        {
            get
            {
                return notification;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref notification, value);
            }
        }

        private string title;

        public string Title
        {
            get
            {
                return title;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref title, value);
            }
        }

        private object content;

        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref content, value);
            }
        }

        #endregion

        #region Infrastructure

        private void Initialize()
        {
            Title = CommonResourceManager.Instance.GetResourceString("Common_SiteSetup_Header");

            networkSetups = new ObservableCollection<NetworkSetupViewModel>();

            var networks = EntityUtils.GetNetworkSites();

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            settingsModel = settingsService.GetSettings();

            var siteModels = settingsModel.SiteSettings.SitesModelList.ToDictionary(x => x.PokerSite);

            foreach (var network in networks.Keys)
            {
                try
                {
                    var networkSetupViewModel = new NetworkSetupViewModel(network);

                    if (!networks.ContainsKey(network))
                    {
                        continue;
                    }

                    var sitesToAdd = (from site in networks[network]
                                      join validationResult in validationResults on site equals validationResult.PokerSite
                                      where siteModels.ContainsKey(site)
                                      select new SiteSetupViewModel(validationResult, siteModels[site])
                                      {
                                          Enabled = validationResult.IsDetected
                                      }).ToArray();

                    if (sitesToAdd.Length == 0)
                    {
                        continue;
                    }

                    networkSetupViewModel.NetworkSites.AddRange(sitesToAdd);

                    networkSetups.Add(networkSetupViewModel);
                }
                catch (Exception e)
                {
                    LogProvider.Log.Error(this, e);
                }
            }

            ApplyCommand = ReactiveCommand.Create();
            ApplyCommand.Subscribe(x =>
            {
                validationResults.ForEach(p =>
                {
                    if (siteModels.ContainsKey(p.PokerSite))
                    {
                        var siteModel = siteModels[p.PokerSite];

                        if (!siteModel.Configured)
                        {
                            if (siteModel.HandHistoryLocationList.Count == 0)
                            {
                                siteModel.HandHistoryLocationList = new ObservableCollection<string>(p.HandHistoryLocations);
                            }

                            siteModel.IsAutoCenter = p.IsAutoCenter;

                            siteModel.Configured = true;
                        }
                    }
                });

                settingsService.SaveSettings(settingsModel);
                FinishInteraction?.Invoke();
            });
        }

        #endregion
    }
}