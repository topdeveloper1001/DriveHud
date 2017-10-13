//-----------------------------------------------------------------------
// <copyright file="NoteProcessingServiceProgressChangedEventArgs.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using System;

namespace DriveHUD.PlayerXRay.Services
{
    public class NoteProcessingServiceProgressChangedEventArgs : EventArgs
    {
        public NoteProcessingServiceProgressChangedEventArgs(int progress)
        {
            Progress = progress;
        }

        public int Progress
        {
            get;
            private set;
        }
    }
}