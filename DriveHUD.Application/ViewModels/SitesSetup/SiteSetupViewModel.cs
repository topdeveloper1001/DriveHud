//-----------------------------------------------------------------------
// <copyright file="SiteSetupViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.Entities;
using Microsoft.Practices.ServiceLocation;
using Model.Settings;
using Model.Site;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;

namespace DriveHUD.Application.ViewModels
{
    /// <summary>
    /// Defines view model for site setup
    /// </summary>
    public class SiteSetupViewModel : ViewModelBase
    {
        private readonly ISiteValidationResult validationResult;

        private readonly ISiteConfiguration siteConfiguration;

        private readonly SiteModel siteModel;

        public SiteSetupViewModel(ISiteValidationResult validationResult, SiteModel siteModel)
        {
            if (validationResult == null)
            {
                throw new ArgumentNullException(nameof(validationResult));
            }

            if (siteModel == null)
            {
                throw new ArgumentNullException(nameof(siteModel));
            }

            this.validationResult = validationResult;
            this.siteModel = siteModel;

            siteConfiguration = ServiceLocator.Current.GetInstance<ISiteConfigurationService>().Get(validationResult.PokerSite);

            issues = new ObservableCollection<string>(validationResult.Issues);
            handHistoryLocations = new ObservableCollection<string>(validationResult.HandHistoryLocations);

            EnableCommand = ReactiveCommand.Create(() => Enabled = !Enabled);
        }

        #region Properties

        public EnumPokerSites PokerSite
        {
            get
            {
                return validationResult.PokerSite;
            }
        }

        public bool Enabled
        {
            get
            {
                return siteModel.Enabled;
            }
            set
            {
                if (siteModel.Enabled == value)
                {
                    return;
                }

                siteModel.Enabled = value;
                validationResult.IsEnabled = value;

                this.RaisePropertyChanged();
            }
        }

        public bool IsDetected
        {
            get
            {
                return validationResult.IsDetected;
            }
        }

        private ObservableCollection<string> issues;

        public ObservableCollection<string> Issues
        {
            get
            {
                return issues;
            }
        }

        public bool HasIssues
        {
            get
            {
                return validationResult.HasIssue;
            }
        }

        private ObservableCollection<string> handHistoryLocations;

        public ObservableCollection<string> HandHistoryLocations
        {
            get
            {
                return handHistoryLocations;
            }
        }

        public bool HasHandHistoryLocations
        {
            get
            {
                return handHistoryLocations != null && handHistoryLocations.Count > 0;
            }
        }

        public bool IsConfigurationCorrect
        {
            get
            {
                return IsDetected && !HasIssues;
            }
        }

        public string SiteLogo
        {
            get
            {
                return siteConfiguration.LogoSource;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand EnableCommand { get; private set; }

        #endregion
    }
}