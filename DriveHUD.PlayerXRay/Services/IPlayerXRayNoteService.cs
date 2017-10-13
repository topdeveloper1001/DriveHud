﻿//-----------------------------------------------------------------------
// <copyright file="IPlayerXRayNoteService.cs" company="Ace Poker Solutions">
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
using DriveHUD.PlayerXRay.BusinessHelper.ApplicationSettings;

namespace DriveHUD.PlayerXRay.Services
{
    public interface IPlayerXRayNoteService : IPlayerNotesService
    {
        void SaveAppSettings();

        void LoadAppSettings();

        NotesAppSettings CurrentNotesAppSettings { get; }
    }
}