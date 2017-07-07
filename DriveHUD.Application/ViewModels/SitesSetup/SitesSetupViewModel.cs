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
using DriveHUD.Common.Wpf.Mvvm;
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
        private readonly IEnumerable<ISiteValidationResult> validationResults;

        public SitesSetupViewModel(IEnumerable<ISiteValidationResult> validationResults)
        {
            this.validationResults = validationResults;
            Initialize();
        }

        #region Properties

        private ObservableCollection<SiteSetupViewModel> siteSetups;

        public ObservableCollection<SiteSetupViewModel> SiteSetups
        {
            get
            {
                return siteSetups;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref siteSetups, value);
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
            siteSetups = new ObservableCollection<SiteSetupViewModel>();

            var settingsService = ServiceLocator.Current.GetInstance<ISettingsService>();
            var settingsModel = settingsService.GetSettings();

            var siteModels = settingsModel.SiteSettings.SitesModelList.ToDictionary(x => x.PokerSite);

            foreach (var validationResult in validationResults)
            {
                if (!siteModels.ContainsKey(validationResult.PokerSite))
                {
                    continue;
                }

                var siteModel = siteModels[validationResult.PokerSite];

                try
                {
                    var siteSetupViewModel = new SiteSetupViewModel(validationResult, siteModel)
                    {
                        Enabled = validationResult.IsDetected
                    };

                    siteSetups.Add(siteSetupViewModel);
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