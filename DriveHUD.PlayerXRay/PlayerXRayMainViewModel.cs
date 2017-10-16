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

using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PlayerXRay.ViewModels;
using Microsoft.Practices.ServiceLocation;
using Model;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;
using DriveHUD.PlayerXRay.Events;
using DriveHUD.PlayerXRay.Views;

namespace DriveHUD.PlayerXRay
{
    public class PlayerXRayMainViewModel : WindowViewModelBase, INotification
    {
        private readonly Dictionary<WorkspaceType, WorkspaceViewModel> workspaces;

        private readonly SingletonStorageModel storageModel;

        private readonly IEventAggregator eventAggregator;

        public PlayerXRayMainViewModel()
        {
            title = "Player X-Ray";

            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            workspaces = new Dictionary<WorkspaceType, WorkspaceViewModel>();
            storageModel = ServiceLocator.Current.TryResolve<SingletonStorageModel>();

            NavigateCommand = ReactiveCommand.Create();
            NavigateCommand.Subscribe(x => Navigate((WorkspaceType)x));

            eventAggregator.GetEvent<RaisePopupEvent>().Subscribe(RaisePopup);
        }

        public void Initialize()
        {
            StaticStorage.CurrentPlayer = StorageModel.PlayerSelectedItem?.PlayerId.ToString();
            StaticStorage.CurrentPlayerName = StorageModel.PlayerSelectedItem?.Name;
            Navigate(WorkspaceType.Notes);
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
                return "2017/10/02";
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

        #region Infrastructure

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