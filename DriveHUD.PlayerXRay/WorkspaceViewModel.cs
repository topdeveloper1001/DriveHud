//-----------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Common.Infrastructure.CustomServices;
using DriveHUD.Common.Wpf.Mvvm;
using DriveHUD.PlayerXRay.Services;
using Microsoft.Practices.ServiceLocation;
using System;

namespace DriveHUD.PlayerXRay
{
    public abstract class WorkspaceViewModel : WindowViewModelBase
    {
        private Lazy<IPlayerXRayNoteService> noteService = new Lazy<IPlayerXRayNoteService>(() => ServiceLocator.Current.GetInstance<IPlayerNotesService>() as IPlayerXRayNoteService);

        public IPlayerXRayNoteService NoteService
        {
            get
            {
                return noteService.Value;
            }
        }

        public abstract WorkspaceType WorkspaceType
        {
            get;
        }
    }
}