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
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;
using DriveHUD.PlayerXRay.DataTypes;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;

namespace DriveHUD.PlayerXRay.ViewModels
{
    public class RunViewModel : WorkspaceViewModel
    {
        public RunViewModel()
        {
            profiles = new ObservableCollection<ProfileObject>(NotesAppSettingsHelper.CurrentNotesAppSettings.Profiles);
            stages = new ObservableCollection<StageObject>();

            RunCommand = ReactiveCommand.Create();
            RunCommand.Subscribe(x => Run());

            ReloadStages();
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

        public ReactiveCommand<object> RunCommand { get; private set; }

        /// <summary>
        /// Runs notes engine
        /// </summary>
        private void Run()
        {
            // add run logic here
        }

        /// <summary>
        /// Reloads <see cref="Stages"/> collection accordingly to selected <see cref="NoteStageType"/>
        /// </summary>
        private void ReloadStages()
        {
            Stages?.ForEach(x => x.IsSelected = false);

            Stages?.Clear();

            Stages?.AddRange(NotesAppSettingsHelper
                .CurrentNotesAppSettings.StagesList
                .Where(x => x.StageType == NoteStageType));
        }
    }
}