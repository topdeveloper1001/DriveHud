//-----------------------------------------------------------------------
// <copyright file="RunViewModel.cs" company="Ace Poker Solutions">
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
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using DriveHUD.PlayerXRay.Services;
using Microsoft.Practices.ServiceLocation;
using Model;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class RunViewModel : WorkspaceViewModel
    {
        private CancellationTokenSource cancellationTokenSource;

        public RunViewModel()
        {
            profiles = new ObservableCollection<ProfileObject>(NoteService.CurrentNotesAppSettings.Profiles);
            stages = new ObservableCollection<StageObject>();

            var canRun = this.WhenAny(x => x.RunMode, x => x.SelectedNoteObject, x => x.SelectedProfile,
                (x1, x2, x3) => (x1.Value == RunMode.AllNotes) ||
                    (x1.Value == RunMode.ByNote && x2.Value != null && x2.Value is NoteObject) ||
                    (x1.Value == RunMode.ByProfile && x3.Value != null));

            RunCommand = ReactiveCommand.Create(canRun);
            RunCommand.Subscribe(x => Run());

            ReloadStages();

            RunMode = RunMode.AllNotes;
        }

        public override WorkspaceType WorkspaceType
        {
            get
            {
                return WorkspaceType.Run;
            }
        }

        private RunMode runMode;

        public RunMode RunMode
        {
            get
            {
                return runMode;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref runMode, value);
            }
        }

        private NoteStageType noteStageType;

        public NoteStageType NoteStageType
        {
            get
            {
                return noteStageType;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref noteStageType, value);
                ReloadStages();
            }
        }

        private ObservableCollection<ProfileObject> profiles;

        public ObservableCollection<ProfileObject> Profiles
        {
            get
            {
                return profiles;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref profiles, value);
            }
        }

        private ProfileObject selectedProfile;

        public ProfileObject SelectedProfile
        {
            get
            {
                return selectedProfile;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref selectedProfile, value);
            }
        }

        private ObservableCollection<StageObject> stages;

        public ObservableCollection<StageObject> Stages
        {
            get
            {
                return stages;
            }
            private set
            {
                this.RaiseAndSetIfChanged(ref stages, value);
            }
        }

        private NoteTreeObjectBase selectedNoteObject;

        public NoteTreeObjectBase SelectedNoteObject
        {
            get
            {
                return selectedNoteObject;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref selectedNoteObject, value);
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


        private bool isRunning;

        public bool IsRunning
        {
            get
            {
                return isRunning;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isRunning, value);
            }
        }

        private RunningStatus status;

        public RunningStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref status, value);
            }
        }

        public ReactiveCommand<object> RunCommand { get; private set; }

        /// <summary>
        /// Runs notes engine
        /// </summary>
        private void Run()
        {
            if (IsRunning)
            {
                return;
            }

            IsRunning = true;
            Status = RunningStatus.Processing;

            StartAsyncOperation(() =>
            {
                List<NoteObject> notes;

                // get all notes
                if (RunMode == RunMode.ByNote && selectedNoteObject != null && selectedNoteObject is NoteObject)
                {
                    notes = new List<NoteObject> { selectedNoteObject as NoteObject };
                }
                else if (RunMode == RunMode.AllNotes)
                {
                    notes = NoteService.CurrentNotesAppSettings.AllNotes;
                }
                else if (RunMode == RunMode.ByProfile && SelectedProfile != null)
                {
                    notes = (from noteId in SelectedProfile.ContainingNotes
                             join note in NoteService.CurrentNotesAppSettings.AllNotes on noteId equals note.ID
                             select note).ToList();
                }
                else
                {
                    return;
                }

                cancellationTokenSource = new CancellationTokenSource();

                var noteProcessingService = ServiceLocator.Current.GetInstance<INoteProcessingService>();

                noteProcessingService.ProgressChanged += (s, e) =>
                {
                    Progress = e.Progress;
                };

                noteProcessingService.ProcessNotes(notes, cancellationTokenSource);

            }, e =>
            {
                IsRunning = false;

                if (e != null)
                {
                    LogProvider.Log.Error(CustomModulesNames.PlayerXRay, $"Could not process notes.", e);
                    Status = RunningStatus.Failed;
                    return;
                }

                Progress = 100;
                Status = RunningStatus.Completed;
            });
        }

        /// <summary>
        /// Reloads <see cref="Stages"/> collection accordingly to selected <see cref="NoteStageType"/>
        /// </summary>
        private void ReloadStages()
        {
            Stages?.ForEach(x => x.IsSelected = false);

            Stages?.Clear();

            SelectedNoteObject = null;

            Stages?.AddRange(NoteService
                .CurrentNotesAppSettings
                .StagesList
                .Where(x => x.StageType == NoteStageType));
        }

        protected override void Disposing()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }

            base.Disposing();
        }
    }
}