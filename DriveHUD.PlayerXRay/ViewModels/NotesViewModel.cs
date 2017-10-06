//-----------------------------------------------------------------------
// <copyright file="NotesViewModel.cs" company="Ace Poker Solutions">
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
    public class NotesViewModel : WorkspaceViewModel
    {
        public NotesViewModel()
        {

        }

        #region Properties

        public override WorkspaceType WorkspaceType
        {
            get
            {
                return WorkspaceType.Notes;
            }
        }

        private bool isAdvancedMode;

        public bool IsAdvancedMode
        {
            get
            {
                return isAdvancedMode;
            }
            set
            {
                this.RaiseAndSetIfChanged(ref isAdvancedMode, value);
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
            }
        }




        #endregion

        #region Commands

        public ReactiveCommand<object> AddNoteCommand { get; private set; }

        public ReactiveCommand<object> EditNoteCommand { get; private set; }

        public ReactiveCommand<object> RemoveNoteCommand { get; private set; }

        public ReactiveCommand<object> ExportCommand { get; private set; }

        #endregion 

    }
}