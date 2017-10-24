//-----------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.PlayerXRay.Events;
using DriveHUD.PlayerXRay.Services;
using DriveHUD.PlayerXRay.ViewModels.PopupViewModels;
using DriveHUD.PlayerXRay.Views.PopupViews;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using ReactiveUI;
using System;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class SettingsViewModel : WorkspaceViewModel
    {
        private readonly IEventAggregator eventAggregator;

        public SettingsViewModel()
        {
            eventAggregator = ServiceLocator.Current.GetInstance<IEventAggregator>();

            DeleteAllNotesCommand = ReactiveCommand.Create();
            DeleteAllNotesCommand.Subscribe(x => DeleteNotes(null));

            DeleteNotesCommand = ReactiveCommand.Create();
            DeleteNotesCommand.Subscribe(x => DeleteNotes(OlderThanDate));

            olderThanDate = DateTime.Today;
        }

        #region Properties 

        public bool AutoNotesEnabled
        {
            get
            {
                return NoteService.CurrentNotesAppSettings.AutoNotesEnabled;
            }
            set
            {
                if (NoteService.CurrentNotesAppSettings.AutoNotesEnabled == value)
                {
                    return;
                }

                NoteService.CurrentNotesAppSettings.AutoNotesEnabled = value;

                this.RaisePropertyChanged(nameof(AutoNotesEnabled));

                NoteService.SaveAppSettings();
            }
        }

        public bool TakesNotesOnHero
        {
            get
            {
                return NoteService.CurrentNotesAppSettings.TakesNotesOnHero;
            }
            set
            {
                if (NoteService.CurrentNotesAppSettings.TakesNotesOnHero == value)
                {
                    return;
                }

                NoteService.CurrentNotesAppSettings.TakesNotesOnHero = value;

                this.RaisePropertyChanged(nameof(TakesNotesOnHero));

                NoteService.SaveAppSettings();
            }
        }

        public bool IsNoteCreationSinceDate
        {
            get
            {
                return NoteService.CurrentNotesAppSettings.IsNoteCreationSinceDate;
            }
            set
            {
                if (NoteService.CurrentNotesAppSettings.IsNoteCreationSinceDate == value)
                {
                    return;
                }

                NoteService.CurrentNotesAppSettings.IsNoteCreationSinceDate = value;

                this.RaisePropertyChanged(nameof(IsNoteCreationSinceDate));

                NoteService.SaveAppSettings();
            }
        }

        public DateTime NoteCreationSinceDate
        {
            get
            {
                return NoteService.CurrentNotesAppSettings.NoteCreationSinceDate;
            }
            set
            {
                if (NoteService.CurrentNotesAppSettings.NoteCreationSinceDate == value)
                {
                    return;
                }

                NoteService.CurrentNotesAppSettings.NoteCreationSinceDate = value;

                this.RaisePropertyChanged(nameof(NoteCreationSinceDate));

                NoteService.SaveAppSettings();
            }
        }

        private DateTime olderThanDate;

        public DateTime OlderThanDate
        {
            get
            {
                return olderThanDate;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref olderThanDate, value);
            }
        }

        private double progress;

        public double Progress
        {
            get
            {
                return progress;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref progress, value);
            }
        }

        private bool progressEnabled;

        public bool ProgressEnabled
        {
            get
            {
                return progressEnabled;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref progressEnabled, value);
            }
        }

        public override WorkspaceType WorkspaceType
        {
            get
            {
                return WorkspaceType.Settings;
            }
        }

        #endregion

        #region Commands

        public ReactiveCommand<object> DeleteAllNotesCommand { get; private set; }

        public ReactiveCommand<object> DeleteNotesCommand { get; private set; }

        #endregion

        #region Infrastructure

        private void DeleteNotes(DateTime? date)
        {
            var confirmationViewModel = new YesNoConfirmationViewModel
            {
                ConfirmationMessage = "Are you sure you want to delete all notes?"
            };

            confirmationViewModel.OnYesAction = () =>
            {
                StartAsyncOperation(() =>
                {
                    Progress = 0;
                    ProgressEnabled = true;

                    var noteProcessingService = ServiceLocator.Current.GetInstance<INoteProcessingService>();

                    noteProcessingService.ProgressChanged += (s, e) =>
                    {
                        Progress = e.Progress;
                    };

                    noteProcessingService.DeletesNotes(date);

                }, () => { ProgressEnabled = false; Progress = 0; });
            };

            var popupEventArgs = new RaisePopupEventArgs()
            {
                Title = "Confirm Delete",
                Content = new YesNoConfirmationView(confirmationViewModel)
            };

            eventAggregator.GetEvent<RaisePopupEvent>().Publish(popupEventArgs);
        }

        #endregion
    }
}