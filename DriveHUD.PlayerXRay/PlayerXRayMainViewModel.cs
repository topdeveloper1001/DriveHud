//-----------------------------------------------------------------------
// <copyright file="AppStoreViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Exceptions;
using DriveHUD.Common.Log;
using DriveHUD.Common.Resources;
using DriveHUD.Common.Security;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.Events;
using DriveHUD.PlayerXRay.ViewModels;
using DriveHUD.PlayerXRay.Views;
using Microsoft.Practices.ServiceLocation;
using Model;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DriveHUD.PlayerXRay
{
    public class PlayerXRayMainViewModel : WindowViewModelBase, IModuleEntryViewModel
    {
        private const string binDirectory = "bin";

        private readonly Dictionary<WorkspaceType, WorkspaceViewModel> workspaces;

        private readonly SingletonStorageModel storageModel;

        private readonly IEventAggregator eventAggregator;

        private readonly SubscriptionToken raisePopupSubscriptionToken;

        public PlayerXRayMainViewModel()
        {
#if !DEBUG
            ValidateLicenseAssemblies();
#endif

            title = "Player X-Ray";

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            workspaces = new Dictionary<WorkspaceType, WorkspaceViewModel>();
            storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

            NavigateCommand = ReactiveCommand.Create();
            NavigateCommand.Subscribe(x => Navigate((WorkspaceType)x));

            raisePopupSubscriptionToken = eventAggregator.GetEvent<RaisePopupEvent>().Subscribe(RaisePopup, false);
        }

        public void Initialize()
        {
            StaticStorage.CurrentPlayer = StorageModel.PlayerSelectedItem?.PlayerId.ToString();
            StaticStorage.CurrentPlayerName = StorageModel.PlayerSelectedItem?.Name;
            Navigate(WorkspaceType.Run);
        }

        #region Properties

        public Version Version
        {
            get
            {
                return Assembly.GetAssembly(typeof(PlayerXRayMainViewModel)).GetName().Version;
            }
        }

        public string BuildDate
        {
            get
            {
                return "2017/10/17";
            }
        }

        public SingletonStorageModel StorageModel
        {
            get
            {
                return storageModel;
            }
        }

        private WorkspaceViewModel workspace;

        public WorkspaceViewModel Workspace
        {
            get
            {
                return workspace;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref workspace, value);
            }
        }

        private string popupTitle;

        public string PopupTitle
        {
            get
            {
                return popupTitle;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupTitle, value);
            }
        }

        private object popupContent;

        public object PopupContent
        {
            get
            {
                return popupContent;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupContent, value);
            }
        }

        private bool popupIsOpen;

        public bool PopupIsOpen
        {
            get
            {
                return popupIsOpen;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref popupIsOpen, value);
            }

        }

        #endregion

        #region Commands

        public ReactiveCommand<object> NavigateCommand { get; private set; }

        #endregion

        #region INotification implementation

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

        #region IDisposable implementation

        protected override void Disposing()
        {
            base.Disposing();

            eventAggregator.GetEvent<RaisePopupEvent>().Unsubscribe(raisePopupSubscriptionToken);
        }

        #endregion

        #region Infrastructure

        private void ValidateLicenseAssemblies()
        {
            var assemblies = new string[] { "DeployLX.Licensing.v5.dll", "XRCReg.dll", "XRReg.dll", "XROReg.dll" };
            var assembliesHashes = new string[] { "c1d67b8e8d38540630872e9d4e44450ce2944700", "41eefbf5455fc80e9f56fa7495f2d1a4e0d30a52", "af7320210803634d0cc182face213af077670f2c", "f157a0fed2ed9707a1e8e84f239068e35e907d7b" };
            var assemblySizes = new int[] { 1032192, 53592, 54616, 53592 };

            for (var i = 0; i < assemblies.Length; i++)
            {
                var assemblyInfo = new FileInfo(assemblies[i]);

                if (!assemblyInfo.Exists)
                {
                    assemblyInfo = new FileInfo(Path.Combine(binDirectory, assemblies[i]));
                }

                var isValid = SecurityUtils.ValidateFileHash(assemblyInfo.FullName, assembliesHashes[i]) && assemblyInfo.Length == assemblySizes[i];

                if (!isValid)
                {
                    throw new DHInternalException(new NonLocalizableString("PlayerXRay could not be initialized"));
                }
            }
        }

        private void Navigate(WorkspaceType workspaceType)
        {
            if (workspaces.ContainsKey(workspaceType))
            {
                Workspace = workspaces[workspaceType];
                return;
            }

            WorkspaceViewModel workspace = null;

            switch (workspaceType)
            {
                case WorkspaceType.Run:
                    workspace = new RunViewModel();
                    break;
                case WorkspaceType.Notes:
                    workspace = new NotesViewModel();
                    break;
                case WorkspaceType.Profiles:
                    workspace = new ProfilesViewModel();
                    break;
                case WorkspaceType.Settings:
                    workspace = new SettingsViewModel();
                    break;
                case WorkspaceType.Help:
                    workspace = new HelpViewModel();
                    break;
            }

            if (workspace == null)
            {
                return;
            }

            workspaces.Add(workspaceType, workspace);
            Workspace = workspace;
        }

        private void RaisePopup(RaisePopupEventArgs e)
        {
            var containerView = e.Content as IPopupContainerView;

            if (containerView != null && containerView.ViewModel != null)
            {
                containerView.ViewModel.FinishInteraction = () => ClosePopup();
            }

            PopupTitle = e.Title;
            PopupContent = containerView;
            PopupIsOpen = true;
        }

        private void ClosePopup()
        {
            PopupIsOpen = false;
            PopupContent = null;
            PopupTitle = null;
        }

        #endregion
    }
}