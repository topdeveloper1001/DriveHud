﻿//-----------------------------------------------------------------------
// <copyright file="UpdateViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2015 Ace Poker Solutions. All Rights Reserved.
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
using DriveHUD.Importers;
using Microsoft.Practices.ServiceLocation;
using Prism.Interactivity.InteractionRequest;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Web;

namespace DriveHUD.Application.ViewModels.Update
{
    /// <summary>
    /// Represents the view model of update form
    /// </summary>
    public class UpdateViewModel : ViewModelBase, INotification, IInteractionRequestAware
    {
        private dynamic appInfo;

        public UpdateViewModel(dynamic appInfo)
        {
            this.appInfo = appInfo;

            title = CommonResourceManager.Instance.GetResourceString("Notifications_Update_Title");

            BuildNotes();

            CancelCommand = ReactiveCommand.Create(() => Cancel());
            UpdateCommand = ReactiveCommand.Create(() => Update());
        }

        #region Properties

        public string Version
        {
            get
            {
                return appInfo.Version.Version;
            }
        }

        private bool isBusy;

        public bool IsBusy
        {
            get
            {
                return isBusy;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isBusy, value);
            }
        }

        private ObservableCollection<UpdateReleaseNoteViewModel> notes;

        public ObservableCollection<UpdateReleaseNoteViewModel> Notes
        {
            get
            {
                return notes;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref notes, value);
            }
        }

        #endregion

        #region Infrastructure

        private void BuildNotes()
        {
            Notes = new ObservableCollection<UpdateReleaseNoteViewModel>();

            if (appInfo == null || appInfo.VersionsSinceLastUpdate == null)
            {
                return;
            }

            foreach (var versionInfo in appInfo.VersionsSinceLastUpdate)
            {
                if (versionInfo == null)
                {
                    continue;
                }

                var updateReleaseNoteViewModel = new UpdateReleaseNoteViewModel
                {
                    Notes = HttpUtility.HtmlDecode(versionInfo.ReleaseNotes),
                    Version = versionInfo.Version
                };

                Notes.Add(updateReleaseNoteViewModel);
            }

            var orderedNotes = Notes.OrderByDescending(x => x.Version).ToArray();

            var firstNote = orderedNotes.FirstOrDefault();

            if (firstNote == null)
            {
                return;
            }

            firstNote.Notes = string.Join(Environment.NewLine, orderedNotes.Select(x => x.Notes.Trim()));

            Notes.Clear();
            Notes.Add(firstNote);
        }

        private void Update()
        {
            LogProvider.Log.Info("Updating is running");

            var importerService = ServiceLocator.Current.GetInstance<IImporterService>();

            if (importerService.IsStarted)
            {
                importerService.ImportingStopped += (s, e) => RunUpdater();
                IsBusy = true;
                importerService.StopImport();
                return;
            }

            RunUpdater();
        }

        private void RunUpdater()
        {
            try
            {
                var updaterProcessInfo = new ProcessStartInfo(UpdaterInfo.UpdaterAssembly);

                var updaterProcess = new Process()
                {
                    StartInfo = updaterProcessInfo
                };

                updaterProcess.Start();

                App.Current.Dispatcher.Invoke(() => App.Current.Shutdown());
            }
            catch (Exception e)
            {
                LogProvider.Log.Error(this, "Could not start updater", e);
            }
        }

        private void Cancel()
        {
            FinishInteraction?.Invoke();
        }

        #endregion

        #region Implementation of INotification

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

        #endregion

        #region Commands

        public ReactiveCommand CancelCommand { get; private set; }

        public ReactiveCommand UpdateCommand { get; private set; }

        #endregion

        #region Implementation of IInteractionRequestAware

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

        #endregion
    }
}