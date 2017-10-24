//-----------------------------------------------------------------------
// <copyright file="INoteProcessingService.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using DriveHUD.Entities;
using DriveHUD.PlayerXRay.DataTypes.NotesTreeObjects;
using HandHistories.Objects.Hand;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DriveHUD.PlayerXRay.Services
{
    public interface INoteProcessingService
    {
        event EventHandler<NoteProcessingServiceProgressChangedEventArgs> ProgressChanged;

        void ProcessNotes(IEnumerable<NoteObject> notes, CancellationTokenSource cancellationTokenSource);

        IEnumerable<Playernotes> ProcessHand(IEnumerable<NoteObject> notes, Playerstatistic stats, HandHistory handHistory);

        void DeletesNotes(DateTime? beforeDate);
    }
}