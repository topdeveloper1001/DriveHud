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

using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Model.Site;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using Remotion.Linq.Collections;
using System;
using System.Collections.Generic;

namespace DriveHUD.Application.ViewModels.SitesSetup
{
    public class NotConfiguredSitesViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private readonly IEnumerable<ISiteValidationResult> validationResults;

        public NotConfiguredSitesViewModel(IEnumerable<ISiteValidationResult> validationResults)
        {
            this.validationResults = validationResults;
            Initialize();
        }

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
