//-----------------------------------------------------------------------
// <copyright file="NotConfiguredSitesViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Common.Utils;
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
using System.Diagnostics;
using System.Linq;

namespace DriveHUD.Application.ViewModels
{
    public class IncorrectlyConfiguredSitesViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private readonly IEnumerable<ISiteValidationResult> validationResults;

        private SettingsModel settingsModel;

        public IncorrectlyConfiguredSitesViewModel(IEnumerable<ISiteValidationResult> validationResults)
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

        public IReactiveCommand<object> HelpCommand { get; private set; }

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

            ApplyCommand = ReactiveCommand.Create();
            ApplyCommand.Subscribe(x =>
            {
                settingsService.SaveSettings(settingsModel);
                FinishInteraction?.Invoke();
            });

            HelpCommand = ReactiveCommand.Create();
            HelpCommand.Subscribe(x => OpenHelp(x as SiteSetupViewModel));
        }

        private void OpenHelp(SiteSetupViewModel siteSetupViewModel)
        {
            if (siteSetupViewModel == null || !ResourceStrings.PokerSiteHelpLinks.ContainsKey(siteSetupViewModel.PokerSite))
            {
                return;
            }

            var helpLinkKey = ResourceStrings.PokerSiteHelpLinks[siteSetupViewModel.PokerSite];
            var helpLink = CommonResourceManager.Instance.GetResourceString(helpLinkKey);

            BrowserHelper.OpenLinkInBrowser(helpLink);
        }

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
    }
}