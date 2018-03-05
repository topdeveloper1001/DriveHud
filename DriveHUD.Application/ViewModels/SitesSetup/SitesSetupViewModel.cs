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

        private ObservableCollection<SiteSetupViewModel> sites;

        public ObservableCollection<SiteSetupViewModel> Sites
        {
            get
            {
                return sites;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref sites, value);
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand ApplyCommand { get; private set; }

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

            sites = new ObservableCollection<SiteSetupViewModel>();

            var allSites = EntityUtils.GetNetworkSites().SelectMany(x => x.Value);

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            settingsModel = settingsService.GetSettings();

            var siteModels = settingsModel.SiteSettings.SitesModelList.ToDictionary(x => x.PokerSite);

            var sitesToAdd = (from site in allSites
                              join validationResult in validationResults on site equals validationResult.PokerSite
                              where siteModels.ContainsKey(site)
                              select new SiteSetupViewModel(validationResult, siteModels[site])
                              {
                                  Enabled = validationResult.IsDetected
                              }).ToArray();

            sites.AddRange(sitesToAdd);

            ApplyCommand = ReactiveCommand.Create(() =>
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
                            siteModel.FastPokerEnabled = p.FastPokerEnabled;

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